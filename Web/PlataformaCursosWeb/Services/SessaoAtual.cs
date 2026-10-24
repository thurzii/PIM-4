/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Services/SessaoAtual.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   A SESSAO DO DESKTOP, TRADUZIDA PARA A WEB

   No desktop a classe Sessao era estatica: guardava o usuario logado
   numa variavel do programa. Funciona porque o programa de desktop
   atende UMA pessoa.

   O servidor web atende MUITAS pessoas ao mesmo tempo. Uma variavel
   estatica seria compartilhada por todas: o login de um aluno
   sobrescreveria o do outro, e cada um veria os dados de quem entrou
   por ultimo. E um erro de concorrencia classico.

   A solucao do ASP.NET: cada visitante carrega a propria identidade
   num cookie assinado pelo servidor. Esta classe le esse cookie e
   oferece os MESMOS METODOS da Sessao do desktop:

       EstaAberta()                 ObterUsuario()
       ObterUsuarioId()             ObterPerfil()
       EhAdministrador()            EhTutor()
       PodeGerenciarCursos()        PodeGerenciarUsuarios()
       PodeVerProgressoDeOutros()

   Por isso as classes de Servicos.cs sao praticamente iguais as do
   desktop: onde lá estava "Sessao.ObterUsuarioId()", aqui esta
   "afr_sessao.ObterUsuarioId()". A regra de negocio nao mudou; o que
   mudou foi de onde vem a identidade de quem esta pedindo.

   REGISTRO: esta classe entra como "Scoped" no Program.cs, ou seja,
   uma instancia por requisicao do navegador.

   O usuario completo NAO fica no cookie - o cookie guarda apenas Id,
   nome, email e perfil. Quando algum metodo precisa do objeto Usuario
   inteiro, esta classe busca no banco e guarda o resultado para nao
   repetir a consulta na mesma requisicao.
   ===================================================================== */

using System.Security.Claims;
using PlataformaCursosWeb.Models;

namespace PlataformaCursosWeb.Services
{
    public class SessaoAtual
    {
        // De onde a identidade vem: o contexto da requisicao atual
        private readonly IHttpContextAccessor afr_contexto;
        private readonly BancoService afr_bancoService;

        // Cache da requisicao: evita ir ao banco duas vezes no mesmo
        // carregamento de pagina
        private Usuario afr_usuarioCarregado = null;

        public SessaoAtual(IHttpContextAccessor afr_acessor, BancoService afr_banco)
        {
            this.afr_contexto = afr_acessor;
            this.afr_bancoService = afr_banco;
        }

        // -------------------------------------------------------------
        // Le uma informacao do cookie. Devolve "" se nao achar.
        // -------------------------------------------------------------
        private string Ler(string afr_tipo)
        {
            ClaimsPrincipal afr_pessoa = afr_contexto.HttpContext?.User;

            if (afr_pessoa == null)
            {
                return "";
            }

            return afr_pessoa.FindFirst(afr_tipo)?.Value ?? "";
        }

        // -------------------------------------------------------------
        // Equivale ao Sessao.EstaAberta() do desktop
        // -------------------------------------------------------------
        public bool EstaAberta()
        {
            ClaimsPrincipal afr_pessoa = afr_contexto.HttpContext?.User;

            if (afr_pessoa == null || afr_pessoa.Identity == null)
            {
                return false;
            }

            return afr_pessoa.Identity.IsAuthenticated;
        }

        public int ObterUsuarioId()
        {
            int afr_id;

            if (int.TryParse(Ler(ClaimTypes.NameIdentifier), out afr_id))
            {
                return afr_id;
            }

            return 0;
        }

        public string ObterNome()
        {
            return Ler(ClaimTypes.Name);
        }

        public string ObterEmail()
        {
            return Ler(ClaimTypes.Email);
        }

        public string ObterPerfil()
        {
            string afr_perfil = Ler(ClaimTypes.Role);

            if (afr_perfil == "")
            {
                return Usuario.PERFIL_ALUNO;
            }

            return afr_perfil;
        }

        public string ObterPrimeiroNome()
        {
            string afr_nome = ObterNome();

            if (afr_nome.Trim() == "")
            {
                return "";
            }

            return afr_nome.Trim().Split(' ')[0];
        }

        // -------------------------------------------------------------
        // Devolve o objeto Usuario completo, buscando no banco.
        // Usado quando o cookie nao basta - por exemplo, para conferir
        // a senha no confirmarIdentidadeEAcao do DS10 e do DS14.
        // -------------------------------------------------------------
        public Usuario ObterUsuario()
        {
            if (!EstaAberta())
            {
                return null;
            }

            if (afr_usuarioCarregado != null)
            {
                return afr_usuarioCarregado;
            }

            afr_usuarioCarregado = afr_bancoService.BuscarUsuarioPorId(ObterUsuarioId());

            return afr_usuarioCarregado;
        }

        // -------------------------------------------------------------
        // AS PERMISSOES
        // Quem decide continua sendo a classe Usuario, igual ao desktop.
        // Aqui o perfil vem do cookie, entao nem precisa ir ao banco.
        // -------------------------------------------------------------
        private Usuario MontarDoCookie()
        {
            Usuario afr_usuario = new Usuario();

            afr_usuario.afr_Id = ObterUsuarioId();
            afr_usuario.afr_Nome = ObterNome();
            afr_usuario.afr_Email = ObterEmail();
            afr_usuario.afr_Perfil = ObterPerfil();

            return afr_usuario;
        }

        public bool EhAdministrador()
        {
            if (!EstaAberta())
            {
                return false;
            }

            return MontarDoCookie().EhAdministrador();
        }

        public bool EhTutor()
        {
            if (!EstaAberta())
            {
                return false;
            }

            return MontarDoCookie().EhTutor();
        }

        public bool EhAluno()
        {
            if (!EstaAberta())
            {
                return false;
            }

            return MontarDoCookie().EhAluno();
        }

        public bool PodeGerenciarCursos()
        {
            if (!EstaAberta())
            {
                return false;
            }

            return MontarDoCookie().PodeGerenciarCursos();
        }

        public bool PodeGerenciarUsuarios()
        {
            if (!EstaAberta())
            {
                return false;
            }

            return MontarDoCookie().PodeGerenciarUsuarios();
        }

        public bool PodeVerProgressoDeOutros()
        {
            if (!EstaAberta())
            {
                return false;
            }

            return MontarDoCookie().PodeVerProgressoDeOutros();
        }
    }
}
