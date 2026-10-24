/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Services/Servicos.cs  (regras de negocio)
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   VERSAO 2 - cada metodo publico corresponde a uma mensagem dos
   diagramas de sequencia do Astah. O nome do metodo foi mantido igual
   ao da mensagem sempre que possivel.

   CLASSES DESTE ARQUIVO:
   1) EmailService ......... DS02 - enviarCodigo / codigoEnviado
   2) LogService ........... DS12 - buscarHistoricoAcoes
   3) AutenticacaoService .. DS01, DS02, DS06
   4) UsuarioService ....... DS11, DS12, DS13, DS14
   5) CursoService ......... DS03, DS04, DS09, DS10
   6) AulaService .......... DS05 - abrirAula / assistirAula
   7) ProgressoService ..... DS05, DS07, DS08
   ===================================================================== */

using System;
using System.Collections.Generic;
using System.IO;
using PlataformaCursosWeb.Models;

namespace PlataformaCursosWeb.Services
{
    // =================================================================
    // CLASSE 1 - EmailService
    // DS02 - a linha de vida "Email" do diagrama.
    //
    // SIMULACAO: o trabalho nao tem servidor de email, entao a mensagem
    // e gravada em dados/emails.txt e mostrada na tela. O importante e
    // que o FLUXO do diagrama acontece: o sistema gera um codigo, o
    // "envia" e so aceita a nova senha depois de conferir esse codigo.
    // Para valer de verdade, bastaria trocar o corpo deste metodo por
    // um SmtpClient.
    // =================================================================
    public class EmailService
    {
        private const string afr_PASTA = "dados";
        private const string afr_ARQUIVO = "emails.txt";

        private static readonly Random afr_sorteio = new Random();

        // Gera um codigo de 6 digitos
        public string GerarCodigo()
        {
            return afr_sorteio.Next(100000, 999999).ToString();
        }

        // DS02 - enviarCodigo(email)
        public bool EnviarCodigo(string afr_email, string afr_nome,
                                 string afr_codigo, int afr_minutos,
                                 out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                Directory.CreateDirectory(afr_PASTA);

                string afr_corpo =
                    "===================================================" + Environment.NewLine +
                    "Para...: " + afr_email + Environment.NewLine +
                    "Assunto: Codigo de recuperacao de senha" + Environment.NewLine +
                    "Enviado: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + Environment.NewLine +
                    "---------------------------------------------------" + Environment.NewLine +
                    "Ola, " + afr_nome + "." + Environment.NewLine + Environment.NewLine +
                    "Seu codigo de recuperacao e: " + afr_codigo + Environment.NewLine +
                    "Ele vale por " + afr_minutos + " minutos." + Environment.NewLine + Environment.NewLine +
                    "Se nao foi voce quem pediu, ignore esta mensagem." + Environment.NewLine +
                    "===================================================" + Environment.NewLine +
                    Environment.NewLine;

                File.AppendAllText(Path.Combine(afr_PASTA, afr_ARQUIVO), afr_corpo);

                afr_mensagem = "Codigo enviado para " + afr_email + ".";
                return true;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao enviar o codigo: " + afr_erro.Message;
                return false;
            }
        }

        public string ObterCaminhoCaixaDeEntrada()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), afr_PASTA, afr_ARQUIVO);
        }

        // -------------------------------------------------------------
        // DIFERENCA EM RELACAO AO DESKTOP
        // No desktop o botao "Ver o codigo" abria o arquivo no Bloco de
        // Notas, porque o programa roda na mesma maquina da pessoa. Na
        // web o arquivo fica no SERVIDOR, e o navegador nao tem como
        // abrir um arquivo de la. Entao a aplicacao le o arquivo e
        // mostra o conteudo numa pagina (/Conta/CaixaDeEntrada).
        //
        // Devolve as mensagens da mais recente para a mais antiga.
        // -------------------------------------------------------------
        public string LerCaixaDeEntrada()
        {
            try
            {
                string afr_caminho = Path.Combine(afr_PASTA, afr_ARQUIVO);

                if (!File.Exists(afr_caminho))
                {
                    return "";
                }

                string afr_tudo = File.ReadAllText(afr_caminho);

                // As mensagens estao separadas pela linha de iguais. Para
                // mostrar a mais nova primeiro, basta inverter a ordem dos
                // blocos.
                string afr_separador = "===================================================";

                List<string> afr_blocos = new List<string>(
                    afr_tudo.Split(new string[] { afr_separador },
                                   StringSplitOptions.RemoveEmptyEntries));

                afr_blocos.Reverse();

                return afr_separador + string.Join(afr_separador, afr_blocos);
            }
            catch (Exception afr_erro)
            {
                return "Nao foi possivel ler a caixa de entrada: " + afr_erro.Message;
            }
        }
    }

    // =================================================================
    // CLASSE 2 - LogService
    // DS12 - buscarHistoricoAcoes(usuario) /
    //        historicoAcoes(loginLog, acoesCrud, cursosRelacionados)
    // =================================================================
    public class LogService
    {
        private readonly BancoService afr_bancoService;

        // INJECAO DE DEPENDENCIA: no desktop era "new BancoService()".
        // Aqui o ASP.NET cria e entrega o servico, uma instancia por
        // requisicao do navegador (ver Program.cs).
        public LogService(BancoService afr_banco)
        {
            this.afr_bancoService = afr_banco;
        }

        public void RegistrarLogin(int afr_usuarioId, string afr_descricao)
        {
            afr_bancoService.RegistrarAcao(
                new LogAcao(afr_usuarioId, LogAcao.TIPO_LOGIN, afr_descricao));
        }

        public void RegistrarCrud(int afr_usuarioId, string afr_descricao)
        {
            afr_bancoService.RegistrarAcao(
                new LogAcao(afr_usuarioId, LogAcao.TIPO_CRUD, afr_descricao));
        }

        public void RegistrarCurso(int afr_usuarioId, string afr_descricao)
        {
            afr_bancoService.RegistrarAcao(
                new LogAcao(afr_usuarioId, LogAcao.TIPO_CURSO, afr_descricao));
        }

        // DS12 - loginLog
        public List<LogAcao> BuscarLoginLog(int afr_usuarioId)
        {
            return afr_bancoService.ListarLogDoUsuario(afr_usuarioId, LogAcao.TIPO_LOGIN, 50);
        }

        // DS12 - acoesCrud
        public List<LogAcao> BuscarAcoesCrud(int afr_usuarioId)
        {
            return afr_bancoService.ListarLogDoUsuario(afr_usuarioId, LogAcao.TIPO_CRUD, 50);
        }

        // DS12 - cursosRelacionados
        public List<LogAcao> BuscarCursosRelacionados(int afr_usuarioId)
        {
            return afr_bancoService.ListarLogDoUsuario(afr_usuarioId, LogAcao.TIPO_CURSO, 50);
        }

        public List<LogAcao> BuscarHistoricoCompleto(int afr_usuarioId)
        {
            return afr_bancoService.ListarLogDoUsuario(afr_usuarioId, "", 150);
        }
    }

    // =================================================================
    // CLASSE 3 - AutenticacaoService
    // DS01 Login, DS02 Recuperar Senha, DS06 Logout
    // =================================================================
    public class AutenticacaoService
    {
        private readonly BancoService afr_bancoService;
        private readonly EmailService afr_emailService;
        private readonly LogService afr_logService;
        private readonly SessaoAtual afr_sessao;

        public AutenticacaoService(BancoService afr_banco, EmailService afr_email,
                                   LogService afr_log, SessaoAtual afr_sessaoAtual)
        {
            this.afr_bancoService = afr_banco;
            this.afr_emailService = afr_email;
            this.afr_logService = afr_log;
            this.afr_sessao = afr_sessaoAtual;
        }

        // Quanto tempo o codigo de recuperacao vale
        public const int MINUTOS_VALIDADE_CODIGO = 15;

        // -------------------------------------------------------------
        // DS01 - validarLogin(email, senha)
        //        buscarUsuario(email) -> usuarioValido() -> acessoPermitido()
        // -------------------------------------------------------------
        public Usuario ValidarLogin(string afr_email, string afr_senha, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (afr_email.Trim() == "" || afr_senha.Trim() == "")
            {
                afr_mensagem = "Informe o email e a senha.";
                return null;
            }

            // DS01 - buscarUsuario(email)
            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorEmail(afr_email.Trim());

            if (afr_usuario == null)
            {
                // Mensagem generica de proposito: nao revela se o email existe
                afr_mensagem = "Email ou senha incorretos.";
                return null;
            }

            if (afr_usuario.afr_Senha != afr_senha)
            {
                afr_mensagem = "Email ou senha incorretos.";
                return null;
            }

            if (!afr_usuario.afr_Ativo)
            {
                afr_mensagem = "Este usuario esta inativo. Procure o administrador.";
                return null;
            }

            // DS01 - acessoPermitido()
            // No desktop esta linha era "Sessao.Abrir(afr_usuario)". Na web
            // quem abre a sessao e o ContaController, gravando o cookie
            // assinado com SignInAsync: o servico apenas aprova o acesso e
            // devolve o usuario.
            afr_logService.RegistrarLogin(afr_usuario.afr_Id,
                "Entrou no sistema como " + afr_usuario.afr_Perfil + ".");

            afr_mensagem = "Bem-vindo, " + afr_usuario.ObterPrimeiroNome() + ".";
            return afr_usuario;
        }

        // -------------------------------------------------------------
        // DS06 - encerrarSessao() / finalizarSessao() / sessaoEncerrada()
        //
        // Na web o que encerra a sessao de verdade e apagar o cookie, e
        // isso quem faz e o ContaController com SignOutAsync. Este metodo
        // cuida da parte que e regra de negocio: registrar a saida no
        // historico de acoes (DS12) antes do cookie ir embora.
        // -------------------------------------------------------------
        public void EncerrarSessao()
        {
            if (afr_sessao.EstaAberta())
            {
                afr_logService.RegistrarLogin(afr_sessao.ObterUsuarioId(),
                    "Saiu do sistema.");
            }
        }

        // -------------------------------------------------------------
        // DS02 - ETAPA 1: solicitarRecuperacao(email) -> enviarCodigo(email)
        // -------------------------------------------------------------
        public bool SolicitarRecuperacao(string afr_email, out string afr_codigoGerado,
                                         out string afr_mensagem)
        {
            afr_codigoGerado = "";
            afr_mensagem = "";

            if (afr_email.Trim() == "")
            {
                afr_mensagem = "Informe o email cadastrado.";
                return false;
            }

            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorEmail(afr_email.Trim());

            if (afr_usuario == null)
            {
                afr_mensagem = "Email nao encontrado no sistema.";
                return false;
            }

            // DS02 - enviarCodigo(email)
            string afr_codigo = afr_emailService.GerarCodigo();

            DateTime afr_expiracao = DateTime.Now.AddMinutes(MINUTOS_VALIDADE_CODIGO);

            if (!afr_bancoService.GravarCodigo(afr_usuario.afr_Id, afr_codigo,
                                               afr_expiracao, out afr_mensagem))
            {
                return false;
            }

            string afr_msgEmail;

            if (!afr_emailService.EnviarCodigo(afr_usuario.afr_Email,
                                               afr_usuario.ObterPrimeiroNome(),
                                               afr_codigo, MINUTOS_VALIDADE_CODIGO,
                                               out afr_msgEmail))
            {
                afr_mensagem = afr_msgEmail;
                return false;
            }

            // DS02 - codigoEnviado()
            afr_codigoGerado = afr_codigo;

            afr_mensagem = "Codigo enviado para " + afr_usuario.afr_Email +
                           ". Ele vale por " + MINUTOS_VALIDADE_CODIGO + " minutos.";

            return true;
        }

        // -------------------------------------------------------------
        // DS02 - ETAPA 2: conferir o codigo informado
        // -------------------------------------------------------------
        public bool ConferirCodigo(string afr_email, string afr_codigo, out string afr_mensagem)
        {
            afr_mensagem = "";

            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorEmail(afr_email.Trim());

            if (afr_usuario == null)
            {
                afr_mensagem = "Email nao encontrado no sistema.";
                return false;
            }

            if (afr_codigo.Trim() == "")
            {
                afr_mensagem = "Informe o codigo que foi enviado.";
                return false;
            }

            CodigoRecuperacao afr_registro =
                afr_bancoService.BuscarCodigo(afr_usuario.afr_Id, afr_codigo.Trim());

            if (afr_registro == null)
            {
                afr_mensagem = "Codigo incorreto.";
                return false;
            }

            if (afr_registro.afr_Usado)
            {
                afr_mensagem = "Este codigo ja foi usado. Peca um novo.";
                return false;
            }

            if (!afr_registro.EstaValido())
            {
                afr_mensagem = "Este codigo expirou. Peca um novo.";
                return false;
            }

            afr_mensagem = "Codigo conferido. Escolha a nova senha (" +
                           afr_registro.MinutosRestantes() + " min restantes).";

            return true;
        }

        // -------------------------------------------------------------
        // DS02 - ETAPA 3: recuperacaoConcluida()
        // -------------------------------------------------------------
        public bool RedefinirSenha(string afr_email, string afr_codigo,
                                   string afr_novaSenha, string afr_confirmacao,
                                   out string afr_mensagem)
        {
            afr_mensagem = "";

            // Confere o codigo de novo: ninguem pula a etapa 2
            if (!ConferirCodigo(afr_email, afr_codigo, out afr_mensagem))
            {
                return false;
            }

            if (afr_novaSenha.Length < 6)
            {
                afr_mensagem = "A nova senha precisa ter pelo menos 6 caracteres.";
                return false;
            }

            // Operador logico DIFERENTE (!=)
            if (afr_novaSenha != afr_confirmacao)
            {
                afr_mensagem = "A confirmacao nao confere com a nova senha.";
                return false;
            }

            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorEmail(afr_email.Trim());

            if (!afr_bancoService.AtualizarSenha(afr_usuario.afr_Email, afr_novaSenha, out afr_mensagem))
            {
                return false;
            }

            // Queima o codigo para nao ser reaproveitado
            CodigoRecuperacao afr_registro =
                afr_bancoService.BuscarCodigo(afr_usuario.afr_Id, afr_codigo.Trim());

            if (afr_registro != null)
            {
                afr_bancoService.MarcarCodigoUsado(afr_registro.afr_Id);
            }

            afr_logService.RegistrarLogin(afr_usuario.afr_Id, "Redefiniu a senha por codigo.");

            afr_mensagem = "Senha redefinida com sucesso.";
            return true;
        }

        // -------------------------------------------------------------
        // DS10 e DS14 - confirmarIdentidadeEAcao(ator, alvo)
        // Antes de apagar, o sistema pede a senha de quem esta logado.
        // -------------------------------------------------------------
        public bool ConfirmarIdentidade(string afr_senhaDigitada, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!afr_sessao.EstaAberta())
            {
                afr_mensagem = "Sessao encerrada. Entre novamente.";
                return false;
            }

            Usuario afr_logado = afr_sessao.ObterUsuario();

            if (afr_senhaDigitada != afr_logado.afr_Senha)
            {
                afr_mensagem = "Senha incorreta. A exclusao foi cancelada.";
                return false;
            }

            afr_mensagem = "Identidade confirmada.";
            return true;
        }

        public string ObterCaminhoCaixaDeEntrada()
        {
            return afr_emailService.ObterCaminhoCaixaDeEntrada();
        }

        // O conteudo da caixa de entrada simulada, para a pagina
        // /Conta/CaixaDeEntrada mostrar
        public string LerCaixaDeEntrada()
        {
            return afr_emailService.LerCaixaDeEntrada();
        }
    }

    // =================================================================
    // CLASSE 4 - UsuarioService
    // DS11 Criar, DS12 Visualizar, DS13 Editar, DS14 Excluir
    // =================================================================
    public class UsuarioService
    {
        private readonly BancoService afr_bancoService;
        private readonly LogService afr_logService;
        private readonly SessaoAtual afr_sessao;

        public UsuarioService(BancoService afr_banco, LogService afr_log,
                              SessaoAtual afr_sessaoAtual)
        {
            this.afr_bancoService = afr_banco;
            this.afr_logService = afr_log;
            this.afr_sessao = afr_sessaoAtual;
        }

        // DS12 - verificarPermissaoAdmin(admin)
        public bool VerificarPermissaoAdmin(out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!afr_sessao.EstaAberta())
            {
                afr_mensagem = "Sessao encerrada. Entre novamente.";
                return false;
            }

            if (!afr_sessao.PodeGerenciarUsuarios())
            {
                afr_mensagem = "Apenas o administrador pode gerenciar usuarios.";
                return false;
            }

            return true;
        }

        public List<Usuario> ListarUsuarios(string afr_termo)
        {
            return afr_bancoService.ListarUsuarios(afr_termo);
        }

        // DS07 - a lista de alunos que o tutor pode acompanhar
        public List<Usuario> ListarAlunos(string afr_termo)
        {
            return afr_bancoService.ListarUsuarios(afr_termo, Usuario.PERFIL_ALUNO);
        }

        // DS12 - buscarDadosUsuario(usuario) / dadosUsuario(...)
        public Usuario BuscarDadosUsuario(int afr_id)
        {
            return afr_bancoService.BuscarUsuarioPorId(afr_id);
        }

        // DS11 - verificarDuplicidade(email) / emailDisponivel(true)
        public bool VerificarDuplicidade(string afr_email, int afr_idIgnorar)
        {
            return afr_bancoService.EmailJaExiste((afr_email ?? "").Trim(), afr_idIgnorar);
        }

        // DS11 - validaECria(dados) / salvarNovoUsuario(dados)
        public bool CriarUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            if (!Validar(afr_usuario, out afr_mensagem))
            {
                return false;
            }

            if (VerificarDuplicidade(afr_usuario.afr_Email, 0))
            {
                afr_mensagem = "Ja existe um usuario com este email.";
                return false;
            }

            if (!afr_bancoService.InserirUsuario(afr_usuario, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCrud(afr_sessao.ObterUsuarioId(),
                "Cadastrou o usuario " + afr_usuario.afr_Nome +
                " (" + afr_usuario.afr_Perfil + ").");

            return true;
        }

        // DS13 - validaEAtualiza(novosDados)
        public bool EditarUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            if (!Validar(afr_usuario, out afr_mensagem))
            {
                return false;
            }

            if (VerificarDuplicidade(afr_usuario.afr_Email, afr_usuario.afr_Id))
            {
                afr_mensagem = "Ja existe outro usuario com este email.";
                return false;
            }

            if (!afr_bancoService.AtualizarUsuario(afr_usuario, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCrud(afr_sessao.ObterUsuarioId(),
                "Alterou o usuario " + afr_usuario.afr_Nome +
                " (Id " + afr_usuario.afr_Id + ").");

            return true;
        }

        // -------------------------------------------------------------
        // DS14 - verificarElegibilidadeExclusao(usuario)
        //        elegivelParaExclusao(true, dadosDetalhados)
        // -------------------------------------------------------------
        public ElegibilidadeExclusao VerificarElegibilidadeExclusao(int afr_id)
        {
            ElegibilidadeExclusao afr_resultado = new ElegibilidadeExclusao();

            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorId(afr_id);

            if (afr_usuario == null)
            {
                afr_resultado.afr_Elegivel = false;
                afr_resultado.afr_Motivo = "Usuario nao encontrado.";
                return afr_resultado;
            }

            // REGRA: ninguem exclui a propria conta
            if (afr_id == afr_sessao.ObterUsuarioId())
            {
                afr_resultado.afr_Elegivel = false;
                afr_resultado.afr_Motivo = "Voce nao pode excluir a sua propria conta.";
                return afr_resultado;
            }

            // REGRA: o sistema nao pode ficar sem administrador
            if (afr_usuario.EhAdministrador())
            {
                int afr_admins = afr_bancoService
                                     .ListarUsuarios("", Usuario.PERFIL_ADMIN).Count;

                if (afr_admins <= 1)
                {
                    afr_resultado.afr_Elegivel = false;
                    afr_resultado.afr_Motivo =
                        "Este e o unico administrador do sistema. " +
                        "Promova outro usuario antes de excluir este.";
                    return afr_resultado;
                }
            }

            afr_resultado.afr_Elegivel = true;
            afr_resultado.afr_Motivo = "O usuario pode ser excluido.";

            // dadosDetalhados: o que sai junto
            afr_resultado.afr_DadosDetalhados.Add(
                "Usuario: " + afr_usuario.afr_Nome + " (" + afr_usuario.afr_Email + ")");

            afr_resultado.afr_DadosDetalhados.Add(
                "Perfil: " + afr_usuario.afr_Perfil);

            afr_resultado.afr_DadosDetalhados.Add(
                "Cadastrado em: " + afr_usuario.afr_DataCadastro.ToString("dd/MM/yyyy"));

            int afr_progresso = afr_bancoService.ContarProgressoDoUsuario(afr_id);
            int afr_log = afr_bancoService.ContarLogDoUsuario(afr_id);

            afr_resultado.afr_DadosDetalhados.Add(
                afr_progresso + " aula(s) concluida(s) serao apagadas");

            afr_resultado.afr_DadosDetalhados.Add(
                afr_log + " registro(s) de historico serao apagados");

            return afr_resultado;
        }

        // DS14 - removerDefinitivamente(usuario)
        public bool RemoverDefinitivamente(int afr_id, out string afr_mensagem)
        {
            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorId(afr_id);

            string afr_nome = afr_usuario != null ? afr_usuario.afr_Nome : "Id " + afr_id;

            if (!afr_bancoService.ExcluirUsuario(afr_id, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCrud(afr_sessao.ObterUsuarioId(),
                "Excluiu o usuario " + afr_nome + " (Id " + afr_id + ").");

            return true;
        }

        private bool Validar(Usuario afr_usuario, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (afr_usuario.afr_Nome.Trim() == "")
            {
                afr_mensagem = "Informe o nome do usuario.";
                return false;
            }

            string afr_email = afr_usuario.afr_Email.Trim();
            int afr_posArroba = afr_email.IndexOf('@');

            // Operador logico OU (||)
            if (afr_posArroba < 1 || afr_email.IndexOf('.', afr_posArroba) < afr_posArroba + 2)
            {
                afr_mensagem = "Informe um email valido (exemplo: nome@escola.com).";
                return false;
            }

            if (afr_usuario.afr_Senha.Length < 6)
            {
                afr_mensagem = "A senha precisa ter pelo menos 6 caracteres.";
                return false;
            }

            // Operador logico E (&&): o perfil precisa ser um dos tres
            if (afr_usuario.afr_Perfil != Usuario.PERFIL_ADMIN &&
                afr_usuario.afr_Perfil != Usuario.PERFIL_TUTOR &&
                afr_usuario.afr_Perfil != Usuario.PERFIL_ALUNO)
            {
                afr_mensagem = "Escolha o perfil do usuario.";
                return false;
            }

            return true;
        }
    }

    // =================================================================
    // CLASSE 5 - CursoService
    // DS03 Visualizar, DS04 Buscar, DS09 Editar, DS10 Excluir
    // =================================================================
    public class CursoService
    {
        private readonly BancoService afr_bancoService;
        private readonly LogService afr_logService;
        private readonly SessaoAtual afr_sessao;

        public CursoService(BancoService afr_banco, LogService afr_log,
                            SessaoAtual afr_sessaoAtual)
        {
            this.afr_bancoService = afr_banco;
            this.afr_logService = afr_log;
            this.afr_sessao = afr_sessaoAtual;
        }

        // DS09 e DS10 - quem pode mexer em curso e o TUTOR (e o admin)
        public bool VerificarPermissaoTutor(out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!afr_sessao.EstaAberta())
            {
                afr_mensagem = "Sessao encerrada. Entre novamente.";
                return false;
            }

            if (!afr_sessao.PodeGerenciarCursos())
            {
                afr_mensagem = "Apenas tutor ou administrador pode gerenciar cursos.";
                return false;
            }

            return true;
        }

        // DS03 - exibirCursos(cursos): o catalogo do aluno
        public List<Curso> ExibirCursos()
        {
            FiltroCurso afr_filtro = new FiltroCurso();
            afr_filtro.afr_SomenteAtivos = true;

            return afr_bancoService.ListarCursos(afr_filtro);
        }

        // DS04 - buscarCurso(termo, filtros): a busca do aluno
        public List<Curso> BuscarCurso(FiltroCurso afr_filtro)
        {
            if (afr_filtro == null)
            {
                afr_filtro = new FiltroCurso();
            }

            afr_filtro.afr_SomenteAtivos = true;

            return afr_bancoService.ListarCursos(afr_filtro);
        }

        // DS04 - localizarCurso(termo, filtros): a busca do TUTOR,
        // que enxerga tambem os cursos inativos
        public List<Curso> LocalizarCurso(FiltroCurso afr_filtro)
        {
            if (afr_filtro == null)
            {
                afr_filtro = new FiltroCurso();
            }

            afr_filtro.afr_SomenteAtivos = false;

            return afr_bancoService.ListarCursos(afr_filtro);
        }

        // DS09 - getDadosCurso(curso) / dadosCurso(titulo, desc)
        public Curso GetDadosCurso(int afr_id)
        {
            return afr_bancoService.BuscarCursoPorId(afr_id);
        }

        public bool CriarCurso(Curso afr_curso, out string afr_mensagem)
        {
            if (!Validar(afr_curso, out afr_mensagem))
            {
                return false;
            }

            if (!afr_bancoService.InserirCurso(afr_curso, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCurso(afr_sessao.ObterUsuarioId(),
                "Criou o curso " + afr_curso.afr_Titulo + ".");

            return true;
        }

        // DS09 - validarEAtualizar(novosDados)
        public bool EditarCurso(Curso afr_curso, out string afr_mensagem)
        {
            if (!Validar(afr_curso, out afr_mensagem))
            {
                return false;
            }

            if (!afr_bancoService.AtualizarCurso(afr_curso, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCurso(afr_sessao.ObterUsuarioId(),
                "Alterou o curso " + afr_curso.afr_Titulo +
                " (Id " + afr_curso.afr_Id + ").");

            return true;
        }

        // -------------------------------------------------------------
        // DS10 - verificarElegibilidadeExclusao(curso)
        //        elegivelParaExclusao(true, dadosDetalhados)
        // -------------------------------------------------------------
        public ElegibilidadeExclusao VerificarElegibilidadeExclusao(int afr_cursoId)
        {
            ElegibilidadeExclusao afr_resultado = new ElegibilidadeExclusao();

            Curso afr_curso = afr_bancoService.BuscarCursoPorId(afr_cursoId);

            if (afr_curso == null)
            {
                afr_resultado.afr_Elegivel = false;
                afr_resultado.afr_Motivo = "Curso nao encontrado.";
                return afr_resultado;
            }

            afr_resultado.afr_Elegivel = true;
            afr_resultado.afr_Motivo = "O curso pode ser excluido.";

            afr_resultado.afr_DadosDetalhados.Add(
                "Curso: " + afr_curso.afr_Titulo + " (" + afr_curso.afr_CargaHoraria + "h)");

            afr_resultado.afr_DadosDetalhados.Add(
                afr_curso.afr_TotalAulas + " aula(s) serao apagadas junto");

            int afr_alunos = afr_bancoService.ContarAlunosComProgressoNoCurso(afr_cursoId);

            afr_resultado.afr_DadosDetalhados.Add(
                afr_alunos + " aluno(s) perderao o progresso neste curso");

            // AVISO (nao impede, mas aparece em destaque na confirmacao)
            if (afr_alunos > 0)
            {
                afr_resultado.afr_Motivo =
                    "ATENCAO: " + afr_alunos + " aluno(s) ja tem progresso neste curso. " +
                    "Considere apenas desativar o curso, em vez de excluir.";
            }

            return afr_resultado;
        }

        // DS10 - removerDefinitivamente(curso)
        public bool RemoverDefinitivamente(int afr_cursoId, out string afr_mensagem)
        {
            Curso afr_curso = afr_bancoService.BuscarCursoPorId(afr_cursoId);

            string afr_titulo = afr_curso != null ? afr_curso.afr_Titulo : "Id " + afr_cursoId;

            if (!afr_bancoService.ExcluirCurso(afr_cursoId, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCurso(afr_sessao.ObterUsuarioId(),
                "Excluiu o curso " + afr_titulo + " (Id " + afr_cursoId + ").");

            return true;
        }

        private bool Validar(Curso afr_curso, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (afr_curso.afr_Titulo.Trim() == "")
            {
                afr_mensagem = "Informe o titulo do curso.";
                return false;
            }

            if (afr_curso.afr_CargaHoraria <= 0)
            {
                afr_mensagem = "A carga horaria precisa ser maior que zero.";
                return false;
            }

            // Operador logico OU (||)
            if (afr_curso.afr_PercentualMinimo < 1 || afr_curso.afr_PercentualMinimo > 100)
            {
                afr_mensagem = "O percentual minimo precisa ficar entre 1 e 100.";
                return false;
            }

            return true;
        }
    }

    // =================================================================
    // CLASSE 6 - AulaService
    // DS05 - abrirAula(aula) / exibirConteudo() / assistirAula(aula)
    // =================================================================
    public class AulaService
    {
        private readonly BancoService afr_bancoService;

        public AulaService(BancoService afr_banco)
        {
            this.afr_bancoService = afr_banco;
        }

        public List<Aula> ListarAulas(int afr_cursoId, int afr_usuarioId)
        {
            return afr_bancoService.ListarAulasDoCurso(afr_cursoId, afr_usuarioId);
        }

        // DS05 - abrirAula(aula): traz a aula com o conteudo
        public Aula AbrirAula(int afr_aulaId, int afr_usuarioId)
        {
            return afr_bancoService.BuscarAulaPorId(afr_aulaId, afr_usuarioId);
        }

        public bool CriarAula(Aula afr_aula, out string afr_mensagem)
        {
            if (!Validar(afr_aula, out afr_mensagem))
            {
                return false;
            }

            return afr_bancoService.InserirAula(afr_aula, out afr_mensagem);
        }

        public bool EditarAula(Aula afr_aula, out string afr_mensagem)
        {
            if (!Validar(afr_aula, out afr_mensagem))
            {
                return false;
            }

            return afr_bancoService.AtualizarAula(afr_aula, out afr_mensagem);
        }

        public bool ExcluirAula(int afr_id, out string afr_mensagem)
        {
            return afr_bancoService.ExcluirAula(afr_id, out afr_mensagem);
        }

        public int ProximaOrdem(int afr_cursoId)
        {
            List<Aula> afr_aulas = ListarAulas(afr_cursoId, 0);

            int afr_maior = 0;

            foreach (Aula afr_aula in afr_aulas)
            {
                if (afr_aula.afr_Ordem > afr_maior)
                {
                    afr_maior = afr_aula.afr_Ordem;
                }
            }

            return afr_maior + 1;
        }

        private bool Validar(Aula afr_aula, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (afr_aula.afr_Titulo.Trim() == "")
            {
                afr_mensagem = "Informe o titulo da aula.";
                return false;
            }

            if (afr_aula.afr_CursoId <= 0)
            {
                afr_mensagem = "Escolha o curso ao qual a aula pertence.";
                return false;
            }

            if (afr_aula.afr_DuracaoMin <= 0)
            {
                afr_mensagem = "A duracao da aula precisa ser maior que zero.";
                return false;
            }

            return true;
        }
    }

    // =================================================================
    // CLASSE 7 - ProgressoService
    // DS05 Marcar aula, DS07 Progresso do aluno, DS08 Conclusao
    // =================================================================
    public class ProgressoService
    {
        private readonly BancoService afr_bancoService;
        private readonly LogService afr_logService;
        private readonly SessaoAtual afr_sessao;

        public ProgressoService(BancoService afr_banco, LogService afr_log,
                                SessaoAtual afr_sessaoAtual)
        {
            this.afr_bancoService = afr_banco;
            this.afr_logService = afr_log;
            this.afr_sessao = afr_sessaoAtual;
        }

        // DS05 - atualizarProgresso(aluno, aula) / salvarProgresso(progresso)
        public bool AtualizarProgresso(int afr_usuarioId, int afr_aulaId,
                                       bool afr_concluida, out string afr_mensagem)
        {
            bool afr_ok;

            if (afr_concluida)
            {
                afr_ok = afr_bancoService.MarcarAulaConcluida(afr_usuarioId, afr_aulaId, out afr_mensagem);
            }
            else
            {
                afr_ok = afr_bancoService.DesmarcarAulaConcluida(afr_usuarioId, afr_aulaId, out afr_mensagem);
            }

            if (afr_ok)
            {
                string afr_texto = afr_concluida
                    ? "Concluiu a aula Id " + afr_aulaId + "."
                    : "Desmarcou a aula Id " + afr_aulaId + ".";

                afr_logService.RegistrarCurso(afr_usuarioId, afr_texto);
            }

            return afr_ok;
        }

        public int ObterAulasConcluidas(int afr_usuarioId, int afr_cursoId)
        {
            return afr_bancoService.ContarAulasConcluidas(afr_usuarioId, afr_cursoId);
        }

        // -------------------------------------------------------------
        // DS08 - obterRequisitosCurso(curso) / requisitosCurso(dados)
        // -------------------------------------------------------------
        public string ObterRequisitosCurso(Curso afr_curso)
        {
            if (afr_curso == null)
            {
                return "Curso nao encontrado.";
            }

            return afr_curso.ObterRequisitos();
        }

        // -------------------------------------------------------------
        // DS08 - verificarConclusao(aluno, curso)
        //        resultadoConclusao(status, percentual)
        // -------------------------------------------------------------
        public ResultadoConclusao VerificarConclusao(int afr_usuarioId, Curso afr_curso)
        {
            if (afr_curso == null)
            {
                return new ResultadoConclusao();
            }

            int afr_concluidas = ObterAulasConcluidas(afr_usuarioId, afr_curso.afr_Id);

            return afr_bancoService.MontarResultado(
                       afr_concluidas, afr_curso.afr_TotalAulas, afr_curso);
        }

        // Atalho usado pelas telas que so querem o numero
        public int CalcularPercentual(int afr_usuarioId, Curso afr_curso)
        {
            return VerificarConclusao(afr_usuarioId, afr_curso).afr_Percentual;
        }

        // -------------------------------------------------------------
        // DS07 - buscarProgresso(aluno) / progresso(dados)
        //        exibirProgresso(dados)
        // -------------------------------------------------------------
        public List<ProgressoCurso> BuscarProgresso(int afr_alunoId)
        {
            return afr_bancoService.BuscarProgressoDoAluno(afr_alunoId);
        }

        // -------------------------------------------------------------
        // DS07 - elegivelParaVisualizacao(true)
        // So tutor e admin consultam o progresso de OUTRA pessoa.
        // Qualquer um pode ver o proprio.
        // -------------------------------------------------------------
        public bool ElegivelParaVisualizacao(int afr_alunoId, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!afr_sessao.EstaAberta())
            {
                afr_mensagem = "Sessao encerrada. Entre novamente.";
                return false;
            }

            // O proprio progresso sempre pode ser visto
            if (afr_alunoId == afr_sessao.ObterUsuarioId())
            {
                return true;
            }

            if (!afr_sessao.PodeVerProgressoDeOutros())
            {
                afr_mensagem = "Apenas tutor ou administrador pode consultar " +
                               "o progresso de outro aluno.";
                return false;
            }

            return true;
        }
    }
}
