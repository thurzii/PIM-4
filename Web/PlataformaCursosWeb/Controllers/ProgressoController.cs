/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Controllers/ProgressoController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   PROGRESSO DOS ALUNOS - equivalente ao FrmProgressoAlunos do desktop.

   DIAGRAMA DS07. No Astah esse diagrama esta salvo com o nome
   "Criar Curso", mas as mensagens dentro dele sao todas de progresso e
   a linha de vida do Professor aparece nele. A sequencia e:

       acessarProgresso()
       solicitarProgresso(aluno)
       buscarProgresso(aluno)
       aulasConcluidas()
       progresso(dados)
       exibirProgresso(dados)
       mostrarProgresso()

   QUEM ENTRA: professor e admin. E o PodeVerProgressoDeOutros() da classe
   Usuario. O aluno comum nao entra aqui - ele ve o proprio progresso
   no catalogo.

   DUAS CAMADAS DE PERMISSAO, igual ao desktop:
     1) o [Authorize(Roles = ...)] barra a requisicao;
     2) o ProgressoService.ElegivelParaVisualizacao confere de novo,
        que e o elegivelParaVisualizacao do diagrama.
   ===================================================================== */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaCursosWeb.Models;
using PlataformaCursosWeb.Services;

namespace PlataformaCursosWeb.Controllers
{
    [Authorize(Roles = Usuario.PERFIL_PROFESSOR + "," + Usuario.PERFIL_ADMIN)]
    public class ProgressoController : ControllerPadrao
    {
        private readonly UsuarioService afr_usuarioService;
        private readonly ProgressoService afr_progressoService;

        // VERSAO 3.2 - XP, nivel, conquistas e classificacao
        private readonly GamificacaoService afr_gamificacaoService;

        public ProgressoController(UsuarioService afr_usuario,
                                   ProgressoService afr_progresso,
                                   GamificacaoService afr_gamificacao)
        {
            afr_usuarioService = afr_usuario;
            afr_progressoService = afr_progresso;
            afr_gamificacaoService = afr_gamificacao;
        }

        // =============================================================
        // DS07 - acessarProgresso() e solicitarProgresso(aluno)
        //
        // A MESMA acao atende os dois momentos: sem alunoId mostra so a
        // lista de alunos; com alunoId busca e exibe o progresso dele.
        // =============================================================
        public IActionResult Index(string termo, int alunoId = 0)
        {
            var afr_dados = new ProgressoAlunosViewModel();

            afr_dados.afr_Termo = (termo ?? "").Trim();
            afr_dados.afr_AlunoId = alunoId;

            // A lista de alunos que o professor pode acompanhar
            afr_dados.afr_Alunos = afr_usuarioService.ListarAlunos(afr_dados.afr_Termo);

            // VERSAO 3.2 - a classificacao por XP.
            // Carregada sempre: quem nao pode ver recebe lista vazia do
            // proprio servico, entao nao ha checagem de perfil aqui.
            afr_dados.afr_Ranking = afr_gamificacaoService.ListarRanking(50);

            ViewBag.Perfil = ObterPerfil();

            if (alunoId <= 0)
            {
                if (afr_dados.afr_Alunos.Count == 0)
                {
                    ViewBag.Aviso = "Nenhum aluno encontrado para esta busca.";
                }
                else
                {
                    ViewBag.Aviso = afr_dados.afr_Alunos.Count
                                  + " aluno(s) na lista. Escolha um para ver o progresso.";
                }

                return View(afr_dados);
            }

            // DS07 - elegivelParaVisualizacao(true)
            string afr_mensagem;

            if (!afr_progressoService.ElegivelParaVisualizacao(alunoId, out afr_mensagem))
            {
                ViewBag.Erro = afr_mensagem;
                return View(afr_dados);
            }

            Usuario afr_aluno = afr_usuarioService.BuscarDadosUsuario(alunoId);

            if (afr_aluno == null)
            {
                ViewBag.Erro = "Aluno nao encontrado.";
                return View(afr_dados);
            }

            afr_dados.afr_Aluno = afr_aluno;

            // DS07 - buscarProgresso(aluno) -> progresso(dados)
            //        -> exibirProgresso(dados)
            afr_dados.afr_Progresso = afr_progressoService.BuscarProgresso(alunoId);

            // VERSAO 3.2 - XP, nivel e conquistas deste aluno
            afr_dados.afr_Pontuacao = afr_gamificacaoService.ObterPontuacao(alunoId);

            if (afr_dados.afr_Progresso.Count == 0)
            {
                ViewBag.Aviso = "Nao existe nenhum curso ativo cadastrado no sistema.";
            }

            return View(afr_dados);
        }
    }
}
