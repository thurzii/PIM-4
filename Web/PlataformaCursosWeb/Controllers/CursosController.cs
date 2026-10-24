/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Controllers/CursosController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   GERENCIAMENTO DE CURSOS E AULAS, equivalente ao FrmCursos do desktop.

   CASOS DE USO E DIAGRAMAS:
   DC07 ....... Criar Curso ...... Salvar em modo novo   -> INSERT
   DC08 ....... Visualizar Curso . Index e Aulas         -> SELECT
   DC09 / DS09  Editar Curso ..... Salvar em modo edicao -> UPDATE
   DC10 / DS10  Excluir Curso .... ConfirmarExclusao + Excluir -> DELETE

   QUEM ENTRA AQUI E O TUTOR (e o admin, que pode tudo). Nos diagramas
   DS09 e DS10 a linha de vida e "Tutor", nao "Admin" - por isso o
   [Authorize] aceita os dois perfis.

   A EXCLUSAO TEM DUAS ETAPAS (DS10):
     1) ConfirmarExclusao (GET) .. verificarElegibilidadeExclusao e
        mostra os dadosDetalhados numa pagina
     2) Excluir (POST) ........... confirmarIdentidadeEAcao (pede a
        senha do tutor) e removerDefinitivamente
   ===================================================================== */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaCursosWeb.Models;
using PlataformaCursosWeb.Services;

namespace PlataformaCursosWeb.Controllers
{
    // Tutor e admin: e o PodeGerenciarCursos() da classe Usuario,
    // escrito do jeito que o ASP.NET entende.
    [Authorize(Roles = Usuario.PERFIL_TUTOR + "," + Usuario.PERFIL_ADMIN)]
    public class CursosController : ControllerPadrao
    {
        private readonly CursoService afr_cursoService;
        private readonly AulaService afr_aulaService;
        private readonly AutenticacaoService afr_autenticacaoService;
        private readonly BancoService afr_bancoService;

        public CursosController(CursoService afr_curso,
                                AulaService afr_aula,
                                AutenticacaoService afr_autenticacao,
                                BancoService afr_banco)
        {
            afr_cursoService = afr_curso;
            afr_aulaService = afr_aula;
            afr_autenticacaoService = afr_autenticacao;
            afr_bancoService = afr_banco;
        }

        // =============================================================
        // LISTA DE CURSOS
        // O tutor enxerga cursos ativos E inativos, por isso usa
        // LocalizarCurso em vez de BuscarCurso.
        // =============================================================
        public IActionResult Index(string termo)
        {
            FiltroCurso afr_filtro = new FiltroCurso();

            afr_filtro.afr_Termo = (termo ?? "").Trim();
            afr_filtro.afr_SomenteAtivos = false;
            afr_filtro.afr_UsuarioId = ObterUsuarioId();

            ViewBag.Termo = afr_filtro.afr_Termo;
            ViewBag.StatusBanco = afr_bancoService.ObterDescricao();

            List<Curso> afr_cursos = afr_cursoService.LocalizarCurso(afr_filtro);

            return View(afr_cursos);
        }

        // =============================================================
        // DC07 - formulario de curso novo
        // =============================================================
        [HttpGet]
        public IActionResult Novo()
        {
            ViewBag.Titulo = "Novo curso";

            return View("Form", new CursoViewModel());
        }

        // =============================================================
        // DC09 / DS09 - formulario de edicao
        // solicitarEdicao(curso) -> buscarCurso -> getDadosCurso
        // -> dadosCurso(titulo, desc) -> exibirDadosParaEdicao(...)
        // =============================================================
        [HttpGet]
        public IActionResult Editar(int id)
        {
            // DS09 - getDadosCurso(curso): le do banco, nao da lista
            Curso afr_curso = afr_cursoService.GetDadosCurso(id);

            if (afr_curso == null)
            {
                AvisarErro("Curso nao encontrado.");
                return RedirectToAction("Index");
            }

            var afr_dados = new CursoViewModel();

            afr_dados.afr_Id = afr_curso.afr_Id;
            afr_dados.afr_Titulo = afr_curso.afr_Titulo;
            afr_dados.afr_Descricao = afr_curso.afr_Descricao;
            afr_dados.afr_CargaHoraria = afr_curso.afr_CargaHoraria;
            afr_dados.afr_PercentualMinimo = afr_curso.afr_PercentualMinimo;
            afr_dados.afr_Ativo = afr_curso.afr_Ativo;

            ViewBag.Titulo = "Editar o curso Id " + afr_curso.afr_Id;
            ViewBag.TotalAulas = afr_curso.afr_TotalAulas;

            return View("Form", afr_dados);
        }

        // =============================================================
        // DC07 e DC09 - salvar (INSERT ou UPDATE)
        // DS09 - confirmarEdicao -> realizarAtualizacao
        //        -> validarEAtualizar -> atualizacaoSucesso
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Salvar(CursoViewModel afr_dados)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Titulo = afr_dados.EhEdicao()
                               ? "Editar o curso Id " + afr_dados.afr_Id
                               : "Novo curso";

                return View("Form", afr_dados);
            }

            Curso afr_curso = new Curso();

            afr_curso.afr_Id = afr_dados.afr_Id;
            afr_curso.afr_Titulo = afr_dados.afr_Titulo.Trim();
            afr_curso.afr_Descricao = (afr_dados.afr_Descricao ?? "").Trim();
            afr_curso.afr_CargaHoraria = afr_dados.afr_CargaHoraria;
            afr_curso.afr_PercentualMinimo = afr_dados.afr_PercentualMinimo;
            afr_curso.afr_Ativo = afr_dados.afr_Ativo;

            string afr_mensagem;
            bool afr_ok;
            string afr_comando;

            if (afr_dados.EhEdicao())
            {
                afr_comando = "UPDATE";
                afr_ok = afr_cursoService.EditarCurso(afr_curso, out afr_mensagem);
            }
            else
            {
                afr_comando = "INSERT";
                afr_ok = afr_cursoService.CriarCurso(afr_curso, out afr_mensagem);
            }

            if (!afr_ok)
            {
                ViewBag.Erro = afr_mensagem;
                ViewBag.Titulo = afr_dados.EhEdicao()
                               ? "Editar o curso Id " + afr_dados.afr_Id
                               : "Novo curso";

                return View("Form", afr_dados);
            }

            // DS09 - exibirMensagemSucesso() / confirmacaoSucesso()
            AvisarSucesso(afr_comando + " executado. " + afr_mensagem);

            return RedirectToAction("Index");
        }

        // =============================================================
        // DC10 / DS10 - ETAPA 1 DA EXCLUSAO
        // solicitarExclusao(curso) -> verificarElegibilidadeExclusao
        // -> elegivelParaExclusao(true, dadosDetalhados)
        // -> solicitarConfirmacaoExclusao(dadosDetalhados)
        // =============================================================
        [HttpGet]
        public IActionResult ConfirmarExclusao(int id)
        {
            Curso afr_curso = afr_cursoService.GetDadosCurso(id);

            if (afr_curso == null)
            {
                AvisarErro("Curso nao encontrado.");
                return RedirectToAction("Index");
            }

            var afr_dados = new ConfirmarExclusaoViewModel();

            afr_dados.afr_Tipo = ConfirmarExclusaoViewModel.TIPO_CURSO;
            afr_dados.afr_Id = afr_curso.afr_Id;
            afr_dados.afr_Titulo = afr_curso.afr_Titulo;

            // verificarElegibilidadeExclusao(curso)
            afr_dados.afr_Elegibilidade =
                afr_cursoService.VerificarElegibilidadeExclusao(afr_curso.afr_Id);

            return View("~/Views/Shared/ConfirmarExclusao.cshtml", afr_dados);
        }

        // =============================================================
        // DC10 / DS10 - ETAPA 2 DA EXCLUSAO
        // confirmarExclusaoFinal(curso)
        // -> confirmarIdentidadeEAcao(tutor, curso)
        // -> realizarExclusaoDefinitiva -> removerDefinitivamente
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(ConfirmarExclusaoViewModel afr_dados)
        {
            // Recarrega a elegibilidade: o estado pode ter mudado entre
            // abrir a pagina e enviar o formulario.
            afr_dados.afr_Tipo = ConfirmarExclusaoViewModel.TIPO_CURSO;
            afr_dados.afr_Elegibilidade =
                afr_cursoService.VerificarElegibilidadeExclusao(afr_dados.afr_Id);

            if (!afr_dados.afr_Elegibilidade.afr_Elegivel)
            {
                ViewBag.Erro = afr_dados.afr_Elegibilidade.afr_Motivo;
                return View("~/Views/Shared/ConfirmarExclusao.cshtml", afr_dados);
            }

            // DS10 - confirmarIdentidadeEAcao(tutor, curso)
            string afr_mensagem;

            if (!afr_autenticacaoService.ConfirmarIdentidade(afr_dados.afr_Senha,
                                                            out afr_mensagem))
            {
                ViewBag.Erro = afr_mensagem;
                afr_dados.afr_Senha = "";

                return View("~/Views/Shared/ConfirmarExclusao.cshtml", afr_dados);
            }

            // DS10 - removerDefinitivamente(curso)
            if (!afr_cursoService.RemoverDefinitivamente(afr_dados.afr_Id, out afr_mensagem))
            {
                ViewBag.Erro = afr_mensagem;
                afr_dados.afr_Senha = "";

                return View("~/Views/Shared/ConfirmarExclusao.cshtml", afr_dados);
            }

            // DS10 - remocaoSucesso() / exibirMensagemSucesso()
            AvisarSucesso("DELETE executado. " + afr_mensagem);

            return RedirectToAction("Index");
        }

        // =============================================================
        // AS AULAS DE UM CURSO
        // =============================================================
        [HttpGet]
        public IActionResult Aulas(int id)
        {
            Curso afr_curso = afr_cursoService.GetDadosCurso(id);

            if (afr_curso == null)
            {
                AvisarErro("Curso nao encontrado.");
                return RedirectToAction("Index");
            }

            var afr_dados = new AulasAdminViewModel();

            afr_dados.afr_Curso = afr_curso;

            // Passa 0 no usuario: aqui nao interessa o progresso de ninguem
            afr_dados.afr_Aulas = afr_aulaService.ListarAulas(afr_curso.afr_Id, 0);

            return View(afr_dados);
        }

        // =============================================================
        // INCLUIR UMA AULA no curso
        // A aula ja nasce com o conteudo que o tutor escreveu (DS05).
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult IncluirAula(int cursoId, string titulo, int duracao,
                                        string conteudo)
        {
            Aula afr_aula = new Aula();

            afr_aula.afr_CursoId = cursoId;
            afr_aula.afr_Titulo = (titulo ?? "").Trim();
            afr_aula.afr_DuracaoMin = duracao;
            afr_aula.afr_Conteudo = (conteudo ?? "").Trim();

            // A ordem e calculada: entra sempre como a ultima aula
            afr_aula.afr_Ordem = afr_aulaService.ProximaOrdem(cursoId);

            string afr_mensagem;

            if (afr_aulaService.CriarAula(afr_aula, out afr_mensagem))
            {
                AvisarSucesso("INSERT em dbo.Aulas: " + afr_mensagem);
            }
            else
            {
                AvisarErro(afr_mensagem);
            }

            return RedirectToAction("Aulas", new { id = cursoId });
        }

        // =============================================================
        // DS05 - EDITAR UMA AULA (abrir o formulario)
        // E aqui que o tutor escreve o conteudo que o aluno vai ler.
        // =============================================================
        [HttpGet]
        public IActionResult EditarAula(int id)
        {
            Aula afr_aula = afr_aulaService.AbrirAula(id, 0);

            if (afr_aula == null)
            {
                AvisarErro("Aula nao encontrada.");
                return RedirectToAction("Index");
            }

            Curso afr_curso = afr_cursoService.GetDadosCurso(afr_aula.afr_CursoId);

            var afr_dados = new AulaFormViewModel();

            afr_dados.afr_Id = afr_aula.afr_Id;
            afr_dados.afr_CursoId = afr_aula.afr_CursoId;
            afr_dados.afr_CursoTitulo = afr_curso == null ? "" : afr_curso.afr_Titulo;
            afr_dados.afr_Ordem = afr_aula.afr_Ordem;
            afr_dados.afr_Titulo = afr_aula.afr_Titulo;
            afr_dados.afr_DuracaoMin = afr_aula.afr_DuracaoMin;
            afr_dados.afr_Conteudo = afr_aula.afr_Conteudo;

            return View("AulaForm", afr_dados);
        }

        // =============================================================
        // DS05 - SALVAR A AULA (UPDATE em dbo.Aulas)
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SalvarAula(AulaFormViewModel afr_dados)
        {
            if (!ModelState.IsValid)
            {
                return View("AulaForm", afr_dados);
            }

            // Reaproveita a aula do banco para nao perder a ordem nem o curso
            Aula afr_aula = afr_aulaService.AbrirAula(afr_dados.afr_Id, 0);

            if (afr_aula == null)
            {
                AvisarErro("Esta aula nao esta mais no banco.");
                return RedirectToAction("Index");
            }

            afr_aula.afr_Titulo = afr_dados.afr_Titulo.Trim();
            afr_aula.afr_DuracaoMin = afr_dados.afr_DuracaoMin;
            afr_aula.afr_Conteudo = (afr_dados.afr_Conteudo ?? "").Trim();

            string afr_mensagem;

            if (afr_aulaService.EditarAula(afr_aula, out afr_mensagem))
            {
                AvisarSucesso("UPDATE em dbo.Aulas: " + afr_mensagem);
            }
            else
            {
                ViewBag.Erro = afr_mensagem;
                return View("AulaForm", afr_dados);
            }

            return RedirectToAction("Aulas", new { id = afr_aula.afr_CursoId });
        }

        // =============================================================
        // EXCLUIR UMA AULA
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirAula(int cursoId, int aulaId)
        {
            string afr_mensagem;

            if (afr_aulaService.ExcluirAula(aulaId, out afr_mensagem))
            {
                AvisarSucesso("DELETE em dbo.Aulas: " + afr_mensagem);
            }
            else
            {
                AvisarErro(afr_mensagem);
            }

            return RedirectToAction("Aulas", new { id = cursoId });
        }
    }
}
