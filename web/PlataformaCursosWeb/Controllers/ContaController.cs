/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Controllers/ContaController.cs
   ---------------------------------------------------------------------
   CASOS DE USO: DC01 Login, DC02 Recuperar Senha, DC06 Logout
   DIAGRAMAS...: DS01, DS02, DS06

   O controller e o equivalente ao code-behind do formulario desktop:
   recebe o que o usuario enviou, chama os servicos e escolhe qual
   pagina mostrar. Ele nao tem SQL nem regra de negocio dentro.

   [HttpGet]  = o navegador esta ABRINDO a pagina
   [HttpPost] = o usuario ENVIOU o formulario

   A RECUPERACAO DE SENHA usa o codigo pessoal que a pessoa recebe ao
   criar a conta: email + codigo + nova senha, num formulario so. A
   API confere o codigo, troca a senha e devolve um codigo novo.
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
        // A conexao testada e descrita e a da API do grupo
        private readonly PonteApi afr_ponte;

        // Os servicos chegam prontos pela INJECAO DE DEPENDENCIA
        public ContaController(AutenticacaoService afr_autenticacao, PonteApi afr_ponteApi)
        {
            afr_autenticacaoService = afr_autenticacao;
            afr_ponte = afr_ponteApi;
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

            // Avisa na tela se a API (ou o banco dela) nao respondeu
            string afr_mensagemBanco;

            bool afr_conectou = afr_ponte.AguardarApi(15, out afr_mensagemBanco);

            if (!afr_conectou)
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

            var resposta = afr_ponte.LoginPublico(afr_dados.afr_Email, afr_dados.afr_Senha);
            var login = resposta.Converter<LoginApi>();
            if (resposta.afr_Ok && login != null && login.PrimeiroAcesso)
            {
                Response.Headers["Cache-Control"] = "no-store";
                ViewBag.Token = login.TokenPrimeiroAcesso;
                return View("PrimeiroAcesso");
            }
            if (!resposta.afr_Ok || login?.afr_Usuario == null || string.IsNullOrEmpty(login.afr_Token))
            {
                ViewBag.Erro = resposta.afr_CodigoHttp == 401 ? "Email ou senha incorretos." : resposta.afr_Mensagem;
                return View(afr_dados);
            }
            Usuario afr_usuario = login.afr_Usuario.ParaUsuario();
            string afr_mensagem = "Bem-vindo!";
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

            // VERSAO 4 - no modo API o token do login vai junto no cookie.
            // O cookie e cifrado e HttpOnly: o navegador guarda, mas nem a
            // pessoa nem um script da pagina conseguem ler o token.
            string afr_tokenApi = login.afr_Token;

            if (afr_tokenApi != "")
            {
                afr_claims.Add(new Claim(SessaoAtual.CLAIM_TOKEN_API, afr_tokenApi));
            }

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

            TempData["Sucesso"] = "Sessão encerrada. Entre novamente para continuar.";

            return RedirectToAction("Login");
        }

        // =============================================================
        // DC02 / DS02 - RECUPERAR SENHA (abrir a pagina)
        // =============================================================
        [HttpGet]
        public IActionResult RecuperarSenha(string email)
        {
            var afr_dados = new RecuperarSenhaViewModel();

            // Aproveita o email que ja estava digitado no login
            if (!string.IsNullOrWhiteSpace(email))
            {
                afr_dados.afr_Email = email;
            }

            return View(afr_dados);
        }

        // =============================================================
        // DS02 - email + codigo pessoal + nova senha -> API
        //        -> senha trocada e codigo novo na tela
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RedefinirSenha(RecuperarSenhaViewModel afr_dados)
        {
            Response.Headers["Cache-Control"] = "no-store";
            if (string.IsNullOrEmpty(afr_dados.afr_NovaSenha) || afr_dados.afr_NovaSenha.Length < 8 || afr_dados.afr_NovaSenha != afr_dados.afr_Confirmacao)
                ViewBag.Erro = "Informe senhas iguais com pelo menos 8 caracteres.";
            else
            {
                var r = afr_ponte.RecuperarComCodigo(afr_dados.afr_Email, afr_dados.afr_Codigo, afr_dados.afr_NovaSenha);
                if (r.afr_Ok) ViewBag.NovoCodigo = r.CodigoRecuperacao;
                else ViewBag.Erro = r.afr_Mensagem;
            }
            ModelState.Clear();
            return View("RecuperarSenha", new RecuperarSenhaViewModel { afr_Email = afr_dados.afr_Email });
        }

        // =============================================================
        // Pagina mostrada quando alguem tenta abrir uma tela que o
        // perfil dele nao alcanca
        // =============================================================
        [HttpGet]
        public IActionResult Cadastro() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Cadastro(string nome, string email, string senha, string confirmarSenha)
        {
            Response.Headers["Cache-Control"] = "no-store";
            var r = afr_ponte.Registrar(nome, email, senha, confirmarSenha);
            if (r.afr_Ok) ViewBag.Codigo = r.CodigoRecuperacao;
            else ViewBag.Erro = r.afr_Mensagem;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult PrimeiroAcesso(string token, string senhaTemporaria, string novaSenha, string confirmarSenha)
        {
            Response.Headers["Cache-Control"] = "no-store";
            ViewBag.Token = token;
            if (string.IsNullOrEmpty(novaSenha) || novaSenha != confirmarSenha)
                ViewBag.Erro = "Confirme a nova senha.";
            else
            {
                var r = afr_ponte.PrimeiroAcesso(token, senhaTemporaria, novaSenha);
                if (r.afr_Ok) ViewBag.Codigo = r.CodigoRecuperacao;
                else ViewBag.Erro = r.afr_Mensagem;
            }
            return View();
        }

        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
}
