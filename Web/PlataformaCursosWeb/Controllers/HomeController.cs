/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Controllers/HomeController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   Painel inicial, equivalente ao FrmPrincipal do desktop. Mostra
   areas diferentes conforme o perfil de quem entrou.

   TRES AREAS, uma por ator dos diagramas de sequencia:
     ALUNO -> Catalogo
     TUTOR -> Catalogo + Cursos e Aulas + Progresso dos Alunos
     ADMIN -> as tres acima + Usuarios

   Quem decide o que aparece sao os metodos PodeGerenciarCursos,
   PodeVerProgressoDeOutros e PodeGerenciarUsuarios, da classe
   Usuario - os mesmos que o desktop usa. A pagina nao compara texto
   de perfil na mao.
   ===================================================================== */

using Microsoft.AspNetCore.Mvc;
using PlataformaCursosWeb.Models;
using PlataformaCursosWeb.Services;

namespace PlataformaCursosWeb.Controllers
{
    public class HomeController : ControllerPadrao
    {
        private readonly BancoService afr_bancoService;
        private readonly CursoService afr_cursoService;
        private readonly UsuarioService afr_usuarioService;
        private readonly ProgressoService afr_progressoService;

        public HomeController(BancoService afr_banco,
                              CursoService afr_curso,
                              UsuarioService afr_usuario,
                              ProgressoService afr_progresso)
        {
            afr_bancoService = afr_banco;
            afr_cursoService = afr_curso;
            afr_usuarioService = afr_usuario;
            afr_progressoService = afr_progresso;
        }

        // =============================================================
        // Painel inicial
        // =============================================================
        public IActionResult Index()
        {
            var afr_dados = new HomeViewModel();

            afr_dados.afr_Nome = ObterNome();
            afr_dados.afr_Email = ObterEmail();
            afr_dados.afr_Perfil = ObterPerfil();

            afr_dados.afr_EhAdmin = EhAdministrador();
            afr_dados.afr_EhTutor = EhTutor();
            afr_dados.afr_PodeGerenciarCursos = PodeGerenciarCursos();
            afr_dados.afr_PodeGerenciarUsuarios = PodeGerenciarUsuarios();
            afr_dados.afr_PodeVerProgresso = PodeVerProgressoDeOutros();

            // Cursos ativos: e o que o aluno enxerga no catalogo
            afr_dados.afr_TotalCursos = afr_cursoService.ExibirCursos().Count;

            // So o admin ve a contagem de usuarios
            if (afr_dados.afr_PodeGerenciarUsuarios)
            {
                afr_dados.afr_TotalUsuarios = afr_usuarioService.ListarUsuarios("").Count;
            }

            // O proprio progresso de quem esta logado, para o cartao do
            // aluno mostrar quantos cursos ele ja fechou
            List<ProgressoCurso> afr_meuProgresso =
                afr_progressoService.BuscarProgresso(ObterUsuarioId());

            afr_dados.afr_MeusCursos = afr_meuProgresso.Count;

            foreach (ProgressoCurso afr_linha in afr_meuProgresso)
            {
                if (afr_linha.afr_Resultado.afr_Aprovado)
                {
                    afr_dados.afr_MeusConcluidos = afr_dados.afr_MeusConcluidos + 1;
                }
            }

            // A data de cadastro nao vem no cookie: busca no banco
            Usuario afr_usuario = afr_usuarioService.BuscarDadosUsuario(ObterUsuarioId());

            if (afr_usuario != null)
            {
                afr_dados.afr_DataCadastro = afr_usuario.afr_DataCadastro;
            }

            afr_dados.afr_StatusBanco = afr_bancoService.ObterDescricao();

            return View(afr_dados);
        }

        // =============================================================
        // Testar conexao com o banco
        // =============================================================
        public IActionResult TestarConexao()
        {
            string afr_mensagem;

            if (afr_bancoService.TestarConexao(out afr_mensagem))
            {
                AvisarSucesso(afr_mensagem);
            }
            else
            {
                AvisarErro(afr_mensagem);
            }

            return RedirectToAction("Index");
        }

        // =============================================================
        // Pagina de erro generica
        // =============================================================
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult Erro()
        {
            return View();
        }
    }
}
