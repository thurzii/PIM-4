/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Controllers/CatalogoController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   A TELA DO ALUNO, equivalente ao FrmCatalogo do desktop.

   CASOS DE USO E DIAGRAMAS:
   DC03 / DS03 - Visualizar Cursos ..... Index sem filtro
   DC04 / DS04 - Buscar Cursos ......... Index com termo E filtros
   DC08 / DS08 - Visualizar Curso ...... Detalhes
   DC05 / DS05 - Marcar Aula ........... Aula e MarcarAula

   O QUE MUDOU NESTA VERSAO:

   1) DS04 manda buscarCurso(termo, FILTROS), no plural. A busca ganhou
      carga horaria minima, maxima e a opcao de esconder os cursos que
      o aluno ja concluiu. Tudo isso vira um objeto FiltroCurso.

   2) DS05 manda abrirAula(aula) e exibirConteudo(). A aula agora tem
      PAGINA PROPRIA (acao Aula), que e o FrmAula do desktop. A caixa
      de selecao da lista continua funcionando como atalho.
   ===================================================================== */

using Microsoft.AspNetCore.Mvc;
using PlataformaCursosWeb.Models;
using PlataformaCursosWeb.Services;

namespace PlataformaCursosWeb.Controllers
{
    public class CatalogoController : ControllerPadrao
    {
        private readonly CursoService afr_cursoService;
        private readonly AulaService afr_aulaService;
        private readonly ProgressoService afr_progressoService;

        public CatalogoController(CursoService afr_curso,
                                  AulaService afr_aula,
                                  ProgressoService afr_progresso)
        {
            afr_cursoService = afr_curso;
            afr_aulaService = afr_aula;
            afr_progressoService = afr_progresso;
        }

        // =============================================================
        // DC03 / DS03 - lista de cursos
        // DC04 / DS04 - busca com termo e filtros
        //
        // processarBuscar(termo, filtros) -> buscarCurso(termo, filtros)
        // -> cursosEncontrados(listaCursos) -> exibirResultados(...)
        // =============================================================
        public IActionResult Index(CatalogoViewModel afr_dados)
        {
            if (afr_dados == null)
            {
                afr_dados = new CatalogoViewModel();
            }

            int afr_alunoId = ObterUsuarioId();

            // Confere a faixa de carga horaria antes de ir ao banco
            if (afr_dados.afr_CargaMaxima > 0
                && afr_dados.afr_CargaMaxima < afr_dados.afr_CargaMinima)
            {
                ViewBag.Erro = "A carga maxima nao pode ser menor que a minima. "
                             + "Use 0 na maxima para nao ter limite.";

                afr_dados.afr_Cursos = new List<Curso>();
                return View(afr_dados);
            }

            if (afr_dados.TemFiltro())
            {
                // DS04 - buscarCurso(termo, filtros)
                FiltroCurso afr_filtro = afr_dados.MontarFiltro(afr_alunoId);

                afr_dados.afr_Cursos = afr_cursoService.BuscarCurso(afr_filtro);

                ViewBag.Resumo = afr_dados.afr_Cursos.Count
                               + " curso(s) encontrado(s).  Filtros: "
                               + afr_filtro.ObterResumo();
            }
            else
            {
                // DS03 - listarCursos() / exibirCursos(cursos)
                afr_dados.afr_Cursos = afr_cursoService.ExibirCursos();

                ViewBag.Resumo = "Todos os cursos disponiveis: "
                               + afr_dados.afr_Cursos.Count + " curso(s).";
            }

            // O progresso do aluno em cada curso da lista, para o
            // cartao mostrar a barra e o status (DS08)
            foreach (Curso afr_curso in afr_dados.afr_Cursos)
            {
                afr_dados.afr_Progressos[afr_curso.afr_Id] =
                    afr_progressoService.VerificarConclusao(afr_alunoId, afr_curso);
            }

            return View(afr_dados);
        }

        // =============================================================
        // DC08 / DS08 - um curso, com as aulas e o progresso DESTE aluno
        //
        // obterRequisitosCurso(curso) -> requisitosCurso(dados)
        // verificarConclusao(aluno, curso) -> resultadoConclusao(...)
        // =============================================================
        public IActionResult Detalhes(int id)
        {
            Curso afr_curso = afr_cursoService.GetDadosCurso(id);

            if (afr_curso == null)
            {
                AvisarErro("Curso nao encontrado.");
                return RedirectToAction("Index");
            }

            // Regra: o aluno nao abre curso inativo pelo endereco direto.
            // Tutor e admin podem, para conferir antes de publicar.
            if (!afr_curso.afr_Ativo && !PodeGerenciarCursos())
            {
                AvisarErro("Este curso nao esta disponivel.");
                return RedirectToAction("Index");
            }

            return View(MontarTela(afr_curso));
        }

        // =============================================================
        // DC05 / DS05 - A PAGINA DA AULA
        //
        // assistirAula(aula) -> abrirAula(aula) -> exibirConteudo()
        //
        // E o FrmAula do desktop. Sem esta pagina o modelo nao ficava
        // atendido: uma caixa de selecao na lista nao mostra conteudo.
        // =============================================================
        public IActionResult Aula(int id)
        {
            int afr_alunoId = ObterUsuarioId();

            // DS05 - abrirAula(aula)
            Aula afr_aula = afr_aulaService.AbrirAula(id, afr_alunoId);

            if (afr_aula == null)
            {
                AvisarErro("Aula nao encontrada.");
                return RedirectToAction("Index");
            }

            Curso afr_curso = afr_cursoService.GetDadosCurso(afr_aula.afr_CursoId);

            if (afr_curso == null)
            {
                AvisarErro("O curso desta aula nao esta mais no banco.");
                return RedirectToAction("Index");
            }

            if (!afr_curso.afr_Ativo && !PodeGerenciarCursos())
            {
                AvisarErro("Este curso nao esta disponivel.");
                return RedirectToAction("Index");
            }

            var afr_dados = new AulaViewModel();

            afr_dados.afr_Aula = afr_aula;
            afr_dados.afr_Curso = afr_curso;

            // DS05 - exibirConclusao(): como esta o curso inteiro
            afr_dados.afr_Resultado = afr_progressoService.VerificarConclusao(
                                          afr_alunoId, afr_curso);

            // Links de navegacao: aula anterior e seguinte pela ordem
            List<Aula> afr_todas = afr_aulaService.ListarAulas(afr_curso.afr_Id, afr_alunoId);

            for (int afr_i = 0; afr_i < afr_todas.Count; afr_i++)
            {
                if (afr_todas[afr_i].afr_Id != afr_aula.afr_Id)
                {
                    continue;
                }

                if (afr_i > 0)
                {
                    afr_dados.afr_AulaAnterior = afr_todas[afr_i - 1].afr_Id;
                }

                if (afr_i < afr_todas.Count - 1)
                {
                    afr_dados.afr_AulaSeguinte = afr_todas[afr_i + 1].afr_Id;
                }

                break;
            }

            return View(afr_dados);
        }

        // =============================================================
        // DC05 / DS05 - concluirAula()
        //        -> atualizarProgresso(aluno, aula)
        //        -> salvarProgresso(progresso) -> progressoAtualizado()
        //
        // O parametro "voltarParaAula" decide para onde ir depois: a
        // mesma acao atende o botao da pagina da aula e a caixa de
        // selecao da lista do curso.
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarAula(int cursoId, int aulaId, bool concluida,
                                        bool voltarParaAula = false)
        {
            int afr_alunoId = ObterUsuarioId();

            string afr_mensagem;

            bool afr_ok = afr_progressoService.AtualizarProgresso(
                              afr_alunoId, aulaId, concluida, out afr_mensagem);

            if (!afr_ok)
            {
                AvisarErro(afr_mensagem);
            }
            else
            {
                if (concluida)
                {
                    AvisarSucesso("INSERT em dbo.Progresso: aula " + aulaId +
                                  " marcada como concluida.");
                }
                else
                {
                    AvisarSucesso("DELETE em dbo.Progresso: aula " + aulaId +
                                  " voltou para nao concluida.");
                }

                // DS05 - exibirConclusao(): avisa quando o curso fecha
                Curso afr_curso = afr_cursoService.GetDadosCurso(cursoId);

                if (afr_curso != null && concluida)
                {
                    ResultadoConclusao afr_resultado =
                        afr_progressoService.VerificarConclusao(afr_alunoId, afr_curso);

                    if (afr_resultado.afr_Aprovado)
                    {
                        AvisarSucesso("Curso concluido. " + afr_resultado.ObterResumo());
                    }
                }
            }

            if (voltarParaAula)
            {
                return RedirectToAction("Aula", new { id = aulaId });
            }

            return RedirectToAction("Detalhes", new { id = cursoId });
        }

        // =============================================================
        // METODO AUXILIAR - monta o ViewModel do curso com o progresso
        // =============================================================
        private CursoAlunoViewModel MontarTela(Curso afr_curso)
        {
            int afr_alunoId = ObterUsuarioId();

            var afr_dados = new CursoAlunoViewModel();

            afr_dados.afr_Curso = afr_curso;
            afr_dados.afr_Aulas = afr_aulaService.ListarAulas(afr_curso.afr_Id, afr_alunoId);

            // DS08 - obterRequisitosCurso(curso) / requisitosCurso(dados)
            afr_dados.afr_Requisitos = afr_progressoService.ObterRequisitosCurso(afr_curso);

            // DS08 - verificarConclusao -> resultadoConclusao(status, percentual)
            afr_dados.afr_Resultado = afr_progressoService.VerificarConclusao(
                                          afr_alunoId, afr_curso);

            return afr_dados;
        }
    }
}
