/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Controllers/ControllerPadrao.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   CLASSE BASE dos controllers que exigem login.

   E o mesmo raciocinio de heranca que usamos em Pessoa -> Usuario:
   o que todo controller logado precisa fica aqui uma vez so, e os
   demais herdam. Assim nenhum deles repete o codigo de ler o cookie.

   O atributo [Authorize] vale para a classe inteira: quem nao estiver
   logado e mandado para a tela de login automaticamente, sem precisar
   escrever nenhuma verificacao dentro das acoes.

   A PROPRIEDADE Sessao

   No desktop as telas escreviam Sessao.PodeGerenciarCursos(), porque
   Sessao era uma classe estatica. Aqui a sessao e um servico por
   requisicao (Services/SessaoAtual.cs), e esta propriedade o pega do
   container de injecao de dependencia. O resultado e que o codigo dos
   controllers fica quase igual ao do desktop:

       desktop:  if (Sessao.PodeGerenciarCursos())
       web....:  if (Sessao.PodeGerenciarCursos())

   DUAS CAMADAS DE PERMISSAO, igual ao desktop:
     1) [Authorize(Roles = ...)] na classe do controller barra a
        requisicao antes de qualquer codigo rodar - e o equivalente a
        esconder o botao no menu;
     2) os servicos conferem de novo (VerificarPermissaoTutor,
        VerificarPermissaoAdmin) - e o equivalente a checagem no Load
        de cada formulario.
   ===================================================================== */

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PlataformaCursosWeb.Models;
using PlataformaCursosWeb.Services;

namespace PlataformaCursosWeb.Controllers
{
    [Authorize]
    public class ControllerPadrao : Controller
    {
        // -------------------------------------------------------------
        // A sessao desta requisicao, vinda do container de injecao.
        // Substitui a classe estatica Sessao do desktop.
        // -------------------------------------------------------------
        protected SessaoAtual Sessao
        {
            get
            {
                return HttpContext.RequestServices.GetRequiredService<SessaoAtual>();
            }
        }

        // -------------------------------------------------------------
        // Atalhos lidos direto do cookie, sem passar pelo servico.
        // -------------------------------------------------------------
        protected int ObterUsuarioId()
        {
            string afr_texto = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int afr_id;

            if (int.TryParse(afr_texto, out afr_id))
            {
                return afr_id;
            }

            return 0;
        }

        protected string ObterNome()
        {
            return User.FindFirstValue(ClaimTypes.Name) ?? "";
        }

        protected string ObterPrimeiroNome()
        {
            return User.FindFirstValue("PrimeiroNome") ?? ObterNome();
        }

        protected string ObterEmail()
        {
            return User.FindFirstValue(ClaimTypes.Email) ?? "";
        }

        protected string ObterPerfil()
        {
            return User.FindFirstValue(ClaimTypes.Role) ?? Usuario.PERFIL_ALUNO;
        }

        // -------------------------------------------------------------
        // AS PERMISSOES DOS TRES PERFIS
        // Quem decide continua sendo a classe Usuario, igual ao desktop.
        // -------------------------------------------------------------
        protected bool EhAdministrador()
        {
            return ObterPerfil() == Usuario.PERFIL_ADMIN;
        }

        protected bool EhTutor()
        {
            return ObterPerfil() == Usuario.PERFIL_TUTOR;
        }

        protected bool PodeGerenciarCursos()
        {
            return EhTutor() || EhAdministrador();
        }

        protected bool PodeGerenciarUsuarios()
        {
            return EhAdministrador();
        }

        protected bool PodeVerProgressoDeOutros()
        {
            return EhTutor() || EhAdministrador();
        }

        // -------------------------------------------------------------
        // Atalhos para as faixas de aviso do topo das paginas.
        // TempData sobrevive a um redirecionamento; ViewBag nao.
        // -------------------------------------------------------------
        protected void AvisarSucesso(string afr_mensagem)
        {
            TempData["Sucesso"] = afr_mensagem;
        }

        protected void AvisarErro(string afr_mensagem)
        {
            TempData["Erro"] = afr_mensagem;
        }
    }
}
