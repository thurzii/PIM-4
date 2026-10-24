/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Controllers/UsuariosController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   CRUD DE USUARIOS, equivalente ao FrmUsuarios do desktop.

   CASOS DE USO E DIAGRAMAS:
   DC11 / DS11 - Criar Usuario ...... Salvar em modo novo   -> INSERT
   DC12 / DS12 - Visualizar Usuario . Perfil                -> SELECT
   DC13 / DS13 - Editar Usuario ..... Salvar em modo edicao -> UPDATE
   DC14 / DS14 - Excluir Usuario .... ConfirmarExclusao + Excluir

   SO O ADMIN ENTRA AQUI - e o verificarPermissaoAdmin(admin) que
   aparece no comeco dos diagramas DS12, DS13 e DS14.

   TRES NOVIDADES DESTA VERSAO:

   1) Existem tres perfis: ALUNO, TUTOR e ADMIN.

   2) DS12 nao e so a lista. Pede o perfil completo com o historico de
      acoes em TRES listas separadas (loginLog, acoesCrud,
      cursosRelacionados). Isso virou a acao Perfil.

   3) DS14 exclui em duas etapas, igual ao DS10 dos cursos.
   ===================================================================== */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlataformaCursosWeb.Models;
using PlataformaCursosWeb.Services;

namespace PlataformaCursosWeb.Controllers
{
    [Authorize(Roles = Usuario.PERFIL_ADMIN)]
    public class UsuariosController : ControllerPadrao
    {
        private readonly UsuarioService afr_usuarioService;
        private readonly LogService afr_logService;
        private readonly AutenticacaoService afr_autenticacaoService;
        private readonly BancoService afr_bancoService;

        public UsuariosController(UsuarioService afr_usuario,
                                  LogService afr_log,
                                  AutenticacaoService afr_autenticacao,
                                  BancoService afr_banco)
        {
            afr_usuarioService = afr_usuario;
            afr_logService = afr_log;
            afr_autenticacaoService = afr_autenticacao;
            afr_bancoService = afr_banco;
        }

        // =============================================================
        // LISTA DE USUARIOS
        // =============================================================
        public IActionResult Index(string termo)
        {
            ViewBag.Termo = (termo ?? "").Trim();
            ViewBag.StatusBanco = afr_bancoService.ObterDescricao();

            // Para a lista marcar "(voce)" e esconder o botao de excluir
            // da propria conta
            ViewBag.MeuId = ObterUsuarioId();

            List<Usuario> afr_usuarios = afr_usuarioService.ListarUsuarios(ViewBag.Termo);

            // Conta quantos tem de cada perfil, para a faixa de resumo
            int afr_admins = 0;
            int afr_tutores = 0;
            int afr_alunos = 0;

            foreach (Usuario afr_usuario in afr_usuarios)
            {
                if (afr_usuario.EhAdministrador())
                {
                    afr_admins = afr_admins + 1;
                }
                else if (afr_usuario.EhTutor())
                {
                    afr_tutores = afr_tutores + 1;
                }
                else
                {
                    afr_alunos = afr_alunos + 1;
                }
            }

            ViewBag.Resumo = afr_usuarios.Count + " usuario(s): "
                           + afr_admins + " admin, "
                           + afr_tutores + " tutor, "
                           + afr_alunos + " aluno.";

            return View(afr_usuarios);
        }

        // =============================================================
        // DC11 / DS11 - formulario de usuario novo
        // =============================================================
        [HttpGet]
        public IActionResult Novo()
        {
            ViewBag.Titulo = "Novo usuario";

            return View("Form", new UsuarioViewModel());
        }

        // =============================================================
        // DC13 / DS13 - formulario de edicao
        // solicitarEdicaoUsuario -> getDadosUsuario -> dadosUsuario
        // -> exibirFormularioEdicao(dadosUsuario)
        // =============================================================
        [HttpGet]
        public IActionResult Editar(int id)
        {
            // DS13 - buscarDadosUsuario(usuario): le do banco
            Usuario afr_usuario = afr_usuarioService.BuscarDadosUsuario(id);

            if (afr_usuario == null)
            {
                AvisarErro("Usuario nao encontrado.");
                return RedirectToAction("Index");
            }

            var afr_dados = new UsuarioViewModel();

            afr_dados.afr_Id = afr_usuario.afr_Id;
            afr_dados.afr_Nome = afr_usuario.afr_Nome;
            afr_dados.afr_Email = afr_usuario.afr_Email;
            afr_dados.afr_Senha = afr_usuario.afr_Senha;
            afr_dados.afr_Perfil = afr_usuario.afr_Perfil;
            afr_dados.afr_Ativo = afr_usuario.afr_Ativo;
            afr_dados.afr_DataCadastro = afr_usuario.afr_DataCadastro;

            ViewBag.Titulo = "Editar o usuario Id " + afr_usuario.afr_Id;

            return View("Form", afr_dados);
        }

        // =============================================================
        // DC11 e DC13 - salvar (INSERT ou UPDATE)
        // DS11 - verificarDuplicidade(email) -> emailDisponivel(true)
        //        -> validaECria(dados) -> salvarNovoUsuario(dados)
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Salvar(UsuarioViewModel afr_dados)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Titulo = afr_dados.EhEdicao()
                               ? "Editar o usuario Id " + afr_dados.afr_Id
                               : "Novo usuario";

                return View("Form", afr_dados);
            }

            // DS11 - verificarDuplicidade(email) / emailDisponivel(true)
            // Confere antes de mandar para o servico, para o aviso sair
            // junto do campo do email.
            if (afr_usuarioService.VerificarDuplicidade(afr_dados.afr_Email, afr_dados.afr_Id))
            {
                ModelState.AddModelError("afr_Email",
                    "Este email ja esta cadastrado para outro usuario.");

                ViewBag.Titulo = afr_dados.EhEdicao()
                               ? "Editar o usuario Id " + afr_dados.afr_Id
                               : "Novo usuario";

                return View("Form", afr_dados);
            }

            Usuario afr_usuario = new Usuario();

            afr_usuario.afr_Id = afr_dados.afr_Id;
            afr_usuario.afr_Nome = afr_dados.afr_Nome.Trim();
            afr_usuario.afr_Email = afr_dados.afr_Email.Trim();
            afr_usuario.afr_Senha = afr_dados.afr_Senha;
            afr_usuario.afr_Perfil = afr_dados.afr_Perfil;
            afr_usuario.afr_Ativo = afr_dados.afr_Ativo;

            string afr_mensagem;
            bool afr_ok;
            string afr_comando;

            if (afr_dados.EhEdicao())
            {
                afr_comando = "UPDATE";
                afr_ok = afr_usuarioService.EditarUsuario(afr_usuario, out afr_mensagem);
            }
            else
            {
                afr_comando = "INSERT";
                afr_ok = afr_usuarioService.CriarUsuario(afr_usuario, out afr_mensagem);
            }

            if (!afr_ok)
            {
                ViewBag.Erro = afr_mensagem;
                ViewBag.Titulo = afr_dados.EhEdicao()
                               ? "Editar o usuario Id " + afr_dados.afr_Id
                               : "Novo usuario";

                return View("Form", afr_dados);
            }

            // DS11 - exibirMensagemSucesso() / sucessoConfirmado()
            AvisarSucesso(afr_comando + " executado. " + afr_mensagem);

            return RedirectToAction("Index");
        }

        // =============================================================
        // DC12 / DS12 - O PERFIL DO USUARIO COM O HISTORICO
        //
        // realizarBuscarPerfil(admin, usuario)
        // -> verificarPermissaoAdmin(admin)
        // -> solicitarVisualizacaoPerfil(usuario)
        // -> buscarDadosUsuario -> dadosUsuario(nome, email, papel, data)
        // -> buscarHistoricoAcoes(usuario)
        // -> historicoAcoes(loginLog, acoesCrud, cursosRelacionados)
        // -> perfilCompleto -> exibirPerfilUsuario(dadosEHistorico)
        // =============================================================
        [HttpGet]
        public IActionResult Perfil(int id)
        {
            // verificarPermissaoAdmin(admin): o [Authorize] da classe ja
            // barrou, mas o servico confere de novo - duas camadas, igual
            // ao desktop (menu + evento Load).
            string afr_mensagem;

            if (!afr_usuarioService.VerificarPermissaoAdmin(out afr_mensagem))
            {
                AvisarErro(afr_mensagem);
                return RedirectToAction("AcessoNegado", "Conta");
            }

            Usuario afr_usuario = afr_usuarioService.BuscarDadosUsuario(id);

            if (afr_usuario == null)
            {
                AvisarErro("Usuario nao encontrado.");
                return RedirectToAction("Index");
            }

            var afr_dados = new PerfilUsuarioViewModel();

            afr_dados.afr_Usuario = afr_usuario;

            // AS TRES LISTAS que o diagrama pede, separadas
            afr_dados.afr_LoginLog = afr_logService.BuscarLoginLog(id);
            afr_dados.afr_AcoesCrud = afr_logService.BuscarAcoesCrud(id);
            afr_dados.afr_CursosRelacionados = afr_logService.BuscarCursosRelacionados(id);

            return View(afr_dados);
        }

        // =============================================================
        // DC14 / DS14 - ETAPA 1 DA EXCLUSAO
        // solicitarExclusaoUsuario -> verificarElegibilidadeExclusao
        // -> elegivelParaExclusao(true, dadosDetalhados)
        // -> solicitarConfirmacaoExclusao(dadosDetalhados)
        // =============================================================
        [HttpGet]
        public IActionResult ConfirmarExclusao(int id)
        {
            Usuario afr_usuario = afr_usuarioService.BuscarDadosUsuario(id);

            if (afr_usuario == null)
            {
                AvisarErro("Usuario nao encontrado.");
                return RedirectToAction("Index");
            }

            var afr_dados = new ConfirmarExclusaoViewModel();

            afr_dados.afr_Tipo = ConfirmarExclusaoViewModel.TIPO_USUARIO;
            afr_dados.afr_Id = afr_usuario.afr_Id;
            afr_dados.afr_Titulo = afr_usuario.afr_Nome;

            afr_dados.afr_Elegibilidade =
                afr_usuarioService.VerificarElegibilidadeExclusao(afr_usuario.afr_Id);

            return View("~/Views/Shared/ConfirmarExclusao.cshtml", afr_dados);
        }

        // =============================================================
        // DC14 / DS14 - ETAPA 2 DA EXCLUSAO
        // confirmarExclusaoFinal(usuario)
        // -> confirmarIdentidadeEAcao(admin, usuario)
        // -> realizarExclusaoDefinitiva -> removerDefinitivamente
        // =============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Excluir(ConfirmarExclusaoViewModel afr_dados)
        {
            afr_dados.afr_Tipo = ConfirmarExclusaoViewModel.TIPO_USUARIO;
            afr_dados.afr_Elegibilidade =
                afr_usuarioService.VerificarElegibilidadeExclusao(afr_dados.afr_Id);

            if (!afr_dados.afr_Elegibilidade.afr_Elegivel)
            {
                ViewBag.Erro = afr_dados.afr_Elegibilidade.afr_Motivo;
                return View("~/Views/Shared/ConfirmarExclusao.cshtml", afr_dados);
            }

            // DS14 - confirmarIdentidadeEAcao(admin, usuario)
            string afr_mensagem;

            if (!afr_autenticacaoService.ConfirmarIdentidade(afr_dados.afr_Senha,
                                                            out afr_mensagem))
            {
                ViewBag.Erro = afr_mensagem;
                afr_dados.afr_Senha = "";

                return View("~/Views/Shared/ConfirmarExclusao.cshtml", afr_dados);
            }

            // DS14 - removerDefinitivamente(usuario)
            if (!afr_usuarioService.RemoverDefinitivamente(afr_dados.afr_Id, out afr_mensagem))
            {
                ViewBag.Erro = afr_mensagem;
                afr_dados.afr_Senha = "";

                return View("~/Views/Shared/ConfirmarExclusao.cshtml", afr_dados);
            }

            AvisarSucesso("DELETE executado. " + afr_mensagem);

            return RedirectToAction("Index");
        }
    }
}
