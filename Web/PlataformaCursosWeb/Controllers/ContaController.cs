/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Controllers/ContaController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   CASOS DE USO: DC01 Login, DC02 Recuperar Senha, DC06 Logout
   DIAGRAMAS...: DS01, DS02, DS06

   O controller e o equivalente ao code-behind do formulario desktop:
   recebe o que o usuario enviou, chama os servicos e escolhe qual
   pagina mostrar. Ele nao tem SQL nem regra de negocio dentro.

   [HttpGet]  = o navegador esta ABRINDO a pagina
   [HttpPost] = o usuario ENVIOU o formulario

   A RECUPERACAO DE SENHA TEM TRES ETAPAS NESTA VERSAO, porque o
   diagrama DS02 tem a linha de vida "Email" e a mensagem
   enviarCodigo(email). Cada etapa e uma acao [HttpPost] diferente, e
   todas devolvem a MESMA pagina (RecuperarSenha.cshtml) - o que muda
   e o campo afr_Etapa do ViewModel.
   ===================================================================== */

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PlataformaCursosWeb.Models;
using PlataformaCursosWeb.Services;

namespace PlataformaCursosWeb.Controllers
{
    public class ContaController : Controller
    {
        private readonly AutenticacaoService afr_autenticacaoService;
        private readonly BancoService afr_bancoService;

        // Os servicos chegam prontos pela INJECAO DE DEPENDENCIA
        public ContaController(AutenticacaoService afr_autenticacao, BancoService afr_banco)
        {
            afr_autenticacaoService = afr_autenticacao;
            afr_bancoService = afr_banco;
        }

        // =============================================================
        // DC01 / DS01 - LOGIN (abrir a pagina)
        // =============================================================
        [HttpGet]
        public IActionResult Login()
        {
            // Ja esta logado? vai direto para o menu
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            // Avisa na tela se o banco nao respondeu
            string afr_mensagemBanco;

            if (!afr_bancoService.TestarConexao(out afr_mensagemBanco))
            {
                ViewBag.ErroBanco = afr_mensagemBanco;
            }

            return View(new LoginViewModel());
        }

        // =============================================================
        // DC01 / DS01 - LOGIN (enviar o formulario)
        // digitarEmailSenha -> validarLogin -> acessoPermitido
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel afr_dados)
        {
            // Validacao das anotacoes do ViewModel
            if (!ModelState.IsValid)
            {
                return View(afr_dados);
            }

            // DS01 - validarLogin(email, senha)
            string afr_mensagem;

            Usuario afr_usuario = afr_autenticacaoService.ValidarLogin(
                                      afr_dados.afr_Email,
                                      afr_dados.afr_Senha,
                                      out afr_mensagem);

            if (afr_usuario == null)
            {
                ViewBag.Erro = afr_mensagem;
                return View(afr_dados);
            }

            // -------------------------------------------------------------
            // DS01 - acessoPermitido(): ABERTURA DA SESSAO
            // No desktop era Sessao.Abrir(usuario), uma variavel estatica.
            // Aqui gravamos os dados em "claims" dentro de um cookie
            // assinado, que pertence SO a este navegador. E isso que
            // permite dois alunos usarem o sistema ao mesmo tempo sem um
            // ver os dados do outro.
            // -------------------------------------------------------------
            var afr_claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, afr_usuario.afr_Id.ToString()),
                new Claim(ClaimTypes.Name, afr_usuario.afr_Nome),
                new Claim(ClaimTypes.Email, afr_usuario.afr_Email),
                new Claim(ClaimTypes.Role, afr_usuario.afr_Perfil),
                new Claim("PrimeiroNome", afr_usuario.ObterPrimeiroNome())
            };

            var afr_identidade = new ClaimsIdentity(
                                     afr_claims,
                                     CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                      CookieAuthenticationDefaults.AuthenticationScheme,
                      new ClaimsPrincipal(afr_identidade));

            TempData["Sucesso"] = afr_mensagem;

            // DS01 - abrirTelaInicial()
            return RedirectToAction("Index", "Home");
        }

        // =============================================================
        // DC06 / DS06 - LOGOUT
        // clicarLogout -> encerrarSessao -> finalizarSessao
        //              -> sessaoEncerrada -> retornarLogin
        // =============================================================
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // Registra a saida no historico ANTES de apagar o cookie:
            // depois do SignOutAsync nao haveria mais como saber quem era.
            afr_autenticacaoService.EncerrarSessao();

            // Apaga o cookie: equivale ao Sessao.Encerrar() do desktop
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Sucesso"] = "Sessao encerrada. Entre novamente para continuar.";

            return RedirectToAction("Login");
        }

        // =============================================================
        // DC02 / DS02 - RECUPERAR SENHA, ETAPA 1 (abrir a pagina)
        // =============================================================
        [HttpGet]
        public IActionResult RecuperarSenha(string email)
        {
            var afr_dados = new RecuperarSenhaViewModel();

            afr_dados.afr_Etapa = 1;
            afr_dados.afr_MinutosValidade = AutenticacaoService.MINUTOS_VALIDADE_CODIGO;

            // Aproveita o email que ja estava digitado no login
            if (!string.IsNullOrWhiteSpace(email))
            {
                afr_dados.afr_Email = email;
            }

            return View(afr_dados);
        }

        // =============================================================
        // DS02 - ETAPA 1: informarEmail(email)
        //        -> solicitarRecuperacao(email) -> enviarCodigo(email)
        //        -> codigoEnviado() -> exibirMensagem()
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarCodigo(RecuperarSenhaViewModel afr_dados)
        {
            afr_dados.afr_MinutosValidade = AutenticacaoService.MINUTOS_VALIDADE_CODIGO;

            string afr_codigoGerado;
            string afr_mensagem;

            bool afr_ok = afr_autenticacaoService.SolicitarRecuperacao(
                              afr_dados.afr_Email,
                              out afr_codigoGerado,
                              out afr_mensagem);

            if (!afr_ok)
            {
                afr_dados.afr_Etapa = 1;
                ViewBag.Erro = afr_mensagem;

                return View("RecuperarSenha", afr_dados);
            }

            // exibirMensagem(): segue para a etapa 2
            afr_dados.afr_Etapa = 2;
            afr_dados.afr_Codigo = "";

            ViewBag.Sucesso = afr_mensagem;

            return View("RecuperarSenha", afr_dados);
        }

        // =============================================================
        // DS02 - ETAPA 2: conferir o codigo informado
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConferirCodigo(RecuperarSenhaViewModel afr_dados)
        {
            afr_dados.afr_MinutosValidade = AutenticacaoService.MINUTOS_VALIDADE_CODIGO;

            string afr_mensagem;

            bool afr_ok = afr_autenticacaoService.ConferirCodigo(
                              afr_dados.afr_Email,
                              afr_dados.afr_Codigo,
                              out afr_mensagem);

            if (!afr_ok)
            {
                afr_dados.afr_Etapa = 2;
                ViewBag.Erro = afr_mensagem;

                return View("RecuperarSenha", afr_dados);
            }

            afr_dados.afr_Etapa = 3;
            ViewBag.Sucesso = afr_mensagem;

            return View("RecuperarSenha", afr_dados);
        }

        // =============================================================
        // DS02 - ETAPA 3: recuperacaoConcluida()
        //
        // O codigo vai de novo para o servico. Ele confere outra vez e
        // so depois marca o codigo como usado: ninguem pula a etapa 2
        // mandando o formulario direto.
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RedefinirSenha(RecuperarSenhaViewModel afr_dados)
        {
            afr_dados.afr_MinutosValidade = AutenticacaoService.MINUTOS_VALIDADE_CODIGO;

            string afr_mensagem;

            bool afr_ok = afr_autenticacaoService.RedefinirSenha(
                              afr_dados.afr_Email,
                              afr_dados.afr_Codigo,
                              afr_dados.afr_NovaSenha,
                              afr_dados.afr_Confirmacao,
                              out afr_mensagem);

            if (!afr_ok)
            {
                afr_dados.afr_Etapa = 3;
                ViewBag.Erro = afr_mensagem;

                return View("RecuperarSenha", afr_dados);
            }

            TempData["Sucesso"] = afr_mensagem + " Use a nova senha para entrar.";

            return RedirectToAction("Login");
        }

        // =============================================================
        // A CAIXA DE ENTRADA SIMULADA (DS02 - a linha de vida "Email")
        //
        // O trabalho nao tem servidor de email. No desktop, o botao
        // "Ver o codigo" abria o arquivo dados/emails.txt no Bloco de
        // Notas, porque o programa rodava na propria maquina.
        //
        // Na web o arquivo fica no SERVIDOR, e o navegador nao consegue
        // abrir um arquivo de la. Entao a aplicacao le o arquivo e
        // mostra o conteudo nesta pagina.
        //
        // NUM SISTEMA REAL esta pagina nao existiria: o codigo chegaria
        // no email da pessoa e ninguem mais poderia ve-lo.
        // =============================================================
        [HttpGet]
        public IActionResult CaixaDeEntrada()
        {
            ViewBag.Conteudo = afr_autenticacaoService.LerCaixaDeEntrada();
            ViewBag.Caminho = afr_autenticacaoService.ObterCaminhoCaixaDeEntrada();

            return View();
        }

        // =============================================================
        // Pagina mostrada quando alguem tenta abrir uma tela que o
        // perfil dele nao alcanca
        // =============================================================
        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
}
