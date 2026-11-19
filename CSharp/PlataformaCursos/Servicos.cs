/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: Servicos.cs  (regras de negocio)
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

   VERSAO 4 - MODO API
   Com a API ligada (perfil "API (Azure)"), cada metodo abaixo
   pergunta a API pela PonteApi em vez de ir ao banco local. O bloco
   "if (afr_ponte.Ativa())" no comeco de cada metodo e esse desvio; o
   resto do metodo e o modo local, mantido para os testes enquanto a API
   nao estiver no Azure. E o mesmo desvio da versao web.
   ===================================================================== */

using System;
using System.Collections.Generic;
using System.IO;

namespace PlataformaCursos
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

        // -------------------------------------------------------------
        // Gera o codigo de 6 digitos da recuperacao de senha.
        //
        // VERSAO 3 - trocado Random por RandomNumberGenerator.
        //
        // POR QUE: Random e um gerador PREVISIVEL. Ele e semeado pelo
        // relogio, entao quem souber mais ou menos a hora em que o
        // codigo foi pedido consegue reproduzir a sequencia e descobrir
        // o numero. Para sortear a ordem de um exercicio isso nao tem
        // importancia nenhuma; para um codigo que troca a senha de
        // alguem, tem. RandomNumberGenerator e o gerador criptografico
        // do .NET e nao da para prever.
        //
        // O item 3.5 da analise de integracao do grupo pede "um unico
        // fluxo aprovado (... ou token de recuperacao com prazo)". Este
        // e o fluxo escolhido: codigo sorteado, valido por
        // MINUTOS_VALIDADE_CODIGO minutos e queimado depois do uso.
        // -------------------------------------------------------------
        public string GerarCodigo()
        {
            // 100000 a 999999: sempre 6 digitos, nunca comecando com zero
            int afr_numero = System.Security.Cryptography.RandomNumberGenerator
                                 .GetInt32(100000, 1000000);

            return afr_numero.ToString();
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
    }

    // =================================================================
    // CLASSE 2 - LogService
    // DS12 - buscarHistoricoAcoes(usuario) /
    //        historicoAcoes(loginLog, acoesCrud, cursosRelacionados)
    // =================================================================
    public class LogService
    {
        private BancoService afr_bancoService = new BancoService();
        private PonteApi afr_ponte = new PonteApi();

        // VERSAO 4 - a API nao tem historico de acoes. No modo API nada
        // e gravado no banco local (ele nao e a fonte de verdade) e as
        // consultas voltam vazias ate a rota existir.
        private bool SemHistorico()
        {
            return afr_ponte.Ativa();
        }

        public void RegistrarLogin(int afr_usuarioId, string afr_descricao)
        {
            if (SemHistorico()) return;

            afr_bancoService.RegistrarAcao(
                new LogAcao(afr_usuarioId, LogAcao.TIPO_LOGIN, afr_descricao));
        }

        public void RegistrarCrud(int afr_usuarioId, string afr_descricao)
        {
            if (SemHistorico()) return;

            afr_bancoService.RegistrarAcao(
                new LogAcao(afr_usuarioId, LogAcao.TIPO_CRUD, afr_descricao));
        }

        public void RegistrarCurso(int afr_usuarioId, string afr_descricao)
        {
            if (SemHistorico()) return;

            afr_bancoService.RegistrarAcao(
                new LogAcao(afr_usuarioId, LogAcao.TIPO_CURSO, afr_descricao));
        }

        // DS12 - loginLog
        public List<LogAcao> BuscarLoginLog(int afr_usuarioId)
        {
            if (SemHistorico()) return new List<LogAcao>();

            return afr_bancoService.ListarLogDoUsuario(afr_usuarioId, LogAcao.TIPO_LOGIN, 50);
        }

        // DS12 - acoesCrud
        public List<LogAcao> BuscarAcoesCrud(int afr_usuarioId)
        {
            if (SemHistorico()) return new List<LogAcao>();

            return afr_bancoService.ListarLogDoUsuario(afr_usuarioId, LogAcao.TIPO_CRUD, 50);
        }

        // DS12 - cursosRelacionados
        public List<LogAcao> BuscarCursosRelacionados(int afr_usuarioId)
        {
            if (SemHistorico()) return new List<LogAcao>();

            return afr_bancoService.ListarLogDoUsuario(afr_usuarioId, LogAcao.TIPO_CURSO, 50);
        }

        public List<LogAcao> BuscarHistoricoCompleto(int afr_usuarioId)
        {
            if (SemHistorico()) return new List<LogAcao>();

            return afr_bancoService.ListarLogDoUsuario(afr_usuarioId, "", 150);
        }
    }

    // =================================================================
    // CLASSE 3 - AutenticacaoService
    // DS01 Login, DS02 Recuperar Senha, DS06 Logout
    // =================================================================
    public class AutenticacaoService
    {
        private BancoService afr_bancoService = new BancoService();
        private PonteApi afr_ponte = new PonteApi();
        private EmailService afr_emailService = new EmailService();
        private LogService afr_logService = new LogService();

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

            // VERSAO 4 - modo API: quem confere a senha e a API. O token
            // fica guardado no cliente do programa (ConfiguracaoApi).
            if (afr_ponte.Ativa())
            {
                string afr_tokenNoCliente;
                Usuario afr_daApi = afr_ponte.Entrar(NormalizarEmail(afr_email), afr_senha,
                                                     out afr_tokenNoCliente, out afr_mensagem);

                if (afr_daApi != null)
                {
                    Sessao.Abrir(afr_daApi);
                    afr_mensagem = "Bem-vindo, " + afr_daApi.ObterPrimeiroNome() + ".";
                }

                return afr_daApi;
            }

            // DS01 - buscarUsuario(email)
            //
            // VERSAO 3.1 - email sempre em minusculas.
            // A API grava e procura com email.trim().toLowerCase(), entao
            // um usuario gravado aqui como "Maria@Escola.com" nao seria
            // encontrado pelo celular. Normalizar nos dois lados resolve.
            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorEmail(
                                      NormalizarEmail(afr_email));

            if (afr_usuario == null)
            {
                // Mensagem generica de proposito: nao revela se o email existe
                afr_mensagem = "Email ou senha incorretos.";
                return null;
            }

            // VERSAO 3 - a senha nao e mais comparada como texto.
            // Conferir() refaz o calculo com a senha digitada e compara os
            // resultados. afr_senhaEraTexto volta true quando o banco ainda
            // guardava a senha antiga, em texto puro.
            bool afr_senhaEraTexto;

            if (!SenhaHash.Conferir(afr_senha, afr_usuario.afr_Senha, out afr_senhaEraTexto))
            {
                afr_mensagem = "Email ou senha incorretos.";
                return null;
            }

            if (!afr_usuario.afr_Ativo)
            {
                afr_mensagem = "Este usuario esta inativo. Procure o administrador.";
                return null;
            }

            // Banco da versao 2: a senha estava certa, mas gravada em texto.
            // Aproveita este login para converter em hash. O banco vai se
            // convertendo sozinho, um usuario por login, sem ninguem ficar
            // trancado para fora.
            if (afr_senhaEraTexto)
            {
                string afr_hashNovo = SenhaHash.Gerar(afr_senha);
                string afr_avisoConversao;

                if (afr_bancoService.AtualizarSenha(afr_usuario.afr_Email,
                                                    afr_hashNovo,
                                                    out afr_avisoConversao))
                {
                    afr_usuario.afr_Senha = afr_hashNovo;
                }
            }

            // DS01 - acessoPermitido(): abre a sessao e registra no historico
            Sessao.Abrir(afr_usuario);

            afr_logService.RegistrarLogin(afr_usuario.afr_Id,
                "Entrou no sistema como " + afr_usuario.afr_Perfil + ".");

            afr_mensagem = "Bem-vindo, " + afr_usuario.ObterPrimeiroNome() + ".";
            return afr_usuario;
        }

        // -------------------------------------------------------------
        // DS06 - encerrarSessao() / finalizarSessao() / sessaoEncerrada()
        // -------------------------------------------------------------
        public void EncerrarSessao()
        {
            if (Sessao.EstaAberta())
            {
                afr_logService.RegistrarLogin(Sessao.ObterUsuarioId(),
                    "Saiu do sistema apos " + Sessao.ObterTempoSessao() + ".");
            }

            Sessao.Encerrar();

            // VERSAO 4 - sem sessao, o token da API tambem sai
            if (afr_ponte.Ativa())
            {
                ConfiguracaoApi.ObterCliente().EsquecerToken();
            }
        }

        // -------------------------------------------------------------
        // VERSAO 3.1 - deixa o email no mesmo formato que a API usa.
        //
        // A API faz email.trim().toLowerCase() ao gravar e ao procurar.
        // Sem isto, o mesmo endereco digitado com maiuscula viraria um
        // usuario diferente, e o login falharia sem explicacao.
        // -------------------------------------------------------------
        public static string NormalizarEmail(string afr_email)
        {
            if (afr_email == null)
            {
                return "";
            }

            return afr_email.Trim().ToLowerInvariant();
        }

        // -------------------------------------------------------------
        // DS02 - ETAPA 1: solicitarRecuperacao(email) -> enviarCodigo(email)
        // -------------------------------------------------------------
        public bool SolicitarRecuperacao(string afr_email, out string afr_codigoGerado,
                                         out string afr_mensagem)
        {
            afr_codigoGerado = "";
            afr_mensagem = "";

            // VERSAO 4 - a API ainda nao tem recuperacao (responde 501)
            if (afr_ponte.Ativa())
            {
                afr_mensagem = afr_ponte.RecuperarSenha(afr_email);
                return false;
            }

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

            if (afr_ponte.Ativa())
            {
                afr_mensagem = afr_ponte.RecuperarSenha(afr_email);
                return false;
            }

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

            if (afr_novaSenha.Length < 8)
            {
                afr_mensagem = "A nova senha precisa ter pelo menos 8 caracteres.";
                return false;
            }

            // Operador logico DIFERENTE (!=)
            if (afr_novaSenha != afr_confirmacao)
            {
                afr_mensagem = "A confirmacao nao confere com a nova senha.";
                return false;
            }

            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorEmail(afr_email.Trim());

            // VERSAO 3 - grava o hash, nunca a senha digitada
            if (!afr_bancoService.AtualizarSenha(afr_usuario.afr_Email,
                                                 SenhaHash.Gerar(afr_novaSenha),
                                                 out afr_mensagem))
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

            if (!Sessao.EstaAberta())
            {
                afr_mensagem = "Sessao encerrada. Entre novamente.";
                return false;
            }

            // VERSAO 4 - no modo API a senha nao esta no banco local: quem
            // confere e a API, refazendo o login com a senha digitada
            if (afr_ponte.Ativa())
            {
                if (!afr_ponte.ConfirmarSenha(afr_senhaDigitada, out afr_mensagem))
                {
                    afr_mensagem = "Senha incorreta. A exclusao foi cancelada.";
                    return false;
                }

                afr_mensagem = "Identidade confirmada.";
                return true;
            }

            Usuario afr_logado = Sessao.ObterUsuario();

            // VERSAO 3 - confere pelo hash, igual ao login
            if (!SenhaHash.Conferir(afr_senhaDigitada, afr_logado.afr_Senha))
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
    }

    // =================================================================
    // CLASSE 4 - UsuarioService
    // DS11 Criar, DS12 Visualizar, DS13 Editar, DS14 Excluir
    // =================================================================
    public class UsuarioService
    {
        private BancoService afr_bancoService = new BancoService();
        private PonteApi afr_ponte = new PonteApi();
        private LogService afr_logService = new LogService();

        // DS12 - verificarPermissaoAdmin(admin)
        public bool VerificarPermissaoAdmin(out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!Sessao.EstaAberta())
            {
                afr_mensagem = "Sessao encerrada. Entre novamente.";
                return false;
            }

            if (!Sessao.PodeGerenciarUsuarios())
            {
                afr_mensagem = "Apenas o administrador pode gerenciar usuarios.";
                return false;
            }

            return true;
        }

        public List<Usuario> ListarUsuarios(string afr_termo)
        {
            if (afr_ponte.Ativa()) return afr_ponte.ListarUsuarios(afr_termo);

            return afr_bancoService.ListarUsuarios(afr_termo);
        }

        // DS07 - a lista de alunos que o professor pode acompanhar
        public List<Usuario> ListarAlunos(string afr_termo)
        {
            if (afr_ponte.Ativa()) return afr_ponte.ListarAlunos(afr_termo);

            return afr_bancoService.ListarUsuarios(afr_termo, Usuario.PERFIL_ALUNO);
        }

        // DS12 - buscarDadosUsuario(usuario) / dadosUsuario(...)
        public Usuario BuscarDadosUsuario(int afr_id)
        {
            if (afr_ponte.Ativa()) return afr_ponte.BuscarUsuario(afr_id);

            return afr_bancoService.BuscarUsuarioPorId(afr_id);
        }

        // DS11 - verificarDuplicidade(email) / emailDisponivel(true)
        public bool VerificarDuplicidade(string afr_email, int afr_idIgnorar)
        {
            // VERSAO 4 - no modo API procura na lista que a API devolve
            if (afr_ponte.Ativa())
            {
                string afr_procurado = AutenticacaoService.NormalizarEmail(afr_email);

                foreach (Usuario afr_existente in afr_ponte.ListarUsuarios(""))
                {
                    if (afr_existente.afr_Id != afr_idIgnorar &&
                        AutenticacaoService.NormalizarEmail(afr_existente.afr_Email) == afr_procurado)
                    {
                        return true;
                    }
                }

                return false;
            }

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

            // VERSAO 3.1 - email no mesmo formato da API (minusculas)
            afr_usuario.afr_Email = AutenticacaoService.NormalizarEmail(afr_usuario.afr_Email);

            // VERSAO 4 - no modo API a senha vai em texto pela conexao e
            // quem gera o hash e a propria API
            if (afr_ponte.Ativa())
            {
                return afr_ponte.CriarUsuario(afr_usuario, out afr_mensagem);
            }

            // VERSAO 3 - a senha vai para o banco como hash, nunca como texto
            afr_usuario.afr_Senha = GarantirHash(afr_usuario.afr_Senha);

            if (!afr_bancoService.InserirUsuario(afr_usuario, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCrud(Sessao.ObterUsuarioId(),
                "Cadastrou o usuario " + afr_usuario.afr_Nome +
                " (" + afr_usuario.afr_Perfil + ").");

            return true;
        }

        // DS13 - validaEAtualiza(novosDados)
        public bool EditarUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            // VERSAO 5 - modo API: a nossa API edita o usuario inteiro
            if (afr_ponte.Ativa())
            {
                return EditarUsuarioPelaApi(afr_usuario, out afr_mensagem);
            }

            // VERSAO 3 - senha em branco na edicao significa "manter a atual".
            // A tela nao mostra mais a senha gravada, porque agora o que esta
            // no banco e um hash e nao ha como voltar dele para a senha. Entao
            // o campo chega vazio quando o administrador nao quis troca-la, e
            // o valor que ja estava gravado e reaproveitado aqui.
            if (string.IsNullOrWhiteSpace(afr_usuario.afr_Senha))
            {
                Usuario afr_atual = afr_bancoService.BuscarUsuarioPorId(afr_usuario.afr_Id);

                if (afr_atual != null)
                {
                    afr_usuario.afr_Senha = afr_atual.afr_Senha;
                }
            }

            if (!Validar(afr_usuario, out afr_mensagem))
            {
                return false;
            }

            if (VerificarDuplicidade(afr_usuario.afr_Email, afr_usuario.afr_Id))
            {
                afr_mensagem = "Ja existe outro usuario com este email.";
                return false;
            }

            // VERSAO 3.1 - email no mesmo formato da API (minusculas)
            afr_usuario.afr_Email = AutenticacaoService.NormalizarEmail(afr_usuario.afr_Email);

            // VERSAO 3 - idem. GarantirHash deixa passar o que ja e hash,
            // entao editar nome ou perfil sem mexer na senha nao estraga
            // a senha que ja estava gravada.
            afr_usuario.afr_Senha = GarantirHash(afr_usuario.afr_Senha);

            if (!afr_bancoService.AtualizarUsuario(afr_usuario, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCrud(Sessao.ObterUsuarioId(),
                "Alterou o usuario " + afr_usuario.afr_Nome +
                " (Id " + afr_usuario.afr_Id + ").");

            return true;
        }

        // -------------------------------------------------------------
        // VERSAO 5 - edicao no modo API.
        //
        // As mesmas conferencias do modo local, com a senha opcional:
        // em branco quer dizer "manter a atual", e quem guarda o hash e
        // a API. O email repetido e o admin tirando o proprio acesso sao
        // recusados por ela tambem.
        // -------------------------------------------------------------
        private bool EditarUsuarioPelaApi(Usuario afr_usuario, out string afr_mensagem)
        {
            afr_usuario.afr_Senha = afr_usuario.afr_Senha ?? "";

            if (!Validar(afr_usuario, out afr_mensagem, true))
            {
                return false;
            }

            if (VerificarDuplicidade(afr_usuario.afr_Email, afr_usuario.afr_Id))
            {
                afr_mensagem = "Ja existe outro usuario com este email.";
                return false;
            }

            afr_usuario.afr_Email = AutenticacaoService.NormalizarEmail(afr_usuario.afr_Email);

            return afr_ponte.EditarUsuario(afr_usuario, out afr_mensagem);
        }

        // -------------------------------------------------------------
        // VERSAO 3 - converte em hash so o que ainda nao e hash.
        //
        // Sem esta verificacao, salvar duas vezes o mesmo usuario
        // aplicaria o hash sobre o hash e a senha original deixaria de
        // funcionar para sempre.
        // -------------------------------------------------------------
        private string GarantirHash(string afr_senha)
        {
            if (SenhaHash.EhQualquerHash(afr_senha))
            {
                return afr_senha;
            }

            return SenhaHash.Gerar(afr_senha);
        }

        // -------------------------------------------------------------
        // DS14 - verificarElegibilidadeExclusao(usuario)
        //        elegivelParaExclusao(true, dadosDetalhados)
        // -------------------------------------------------------------
        public ElegibilidadeExclusao VerificarElegibilidadeExclusao(int afr_id)
        {
            ElegibilidadeExclusao afr_resultado = new ElegibilidadeExclusao();

            // VERSAO 5 - modo API: a nossa API exclui. Ela mesma recusa a
            // propria conta e o ultimo admin; aqui a regra se repete so
            // para a tela de confirmacao ja chegar com o motivo.
            if (afr_ponte.Ativa())
            {
                Usuario afr_alvo = afr_ponte.BuscarUsuario(afr_id);

                if (afr_alvo == null)
                {
                    afr_resultado.afr_Elegivel = false;
                    afr_resultado.afr_Motivo = "Usuario nao encontrado.";
                    return afr_resultado;
                }

                if (afr_id == Sessao.ObterUsuarioId())
                {
                    afr_resultado.afr_Elegivel = false;
                    afr_resultado.afr_Motivo = "Voce nao pode excluir a sua propria conta.";
                    return afr_resultado;
                }

                afr_resultado.afr_Elegivel = true;
                afr_resultado.afr_Motivo = "O usuario pode ser excluido.";
                afr_resultado.afr_DadosDetalhados.Add(
                    "Usuario: " + afr_alvo.afr_Nome + " (" + afr_alvo.afr_Email + ")");
                afr_resultado.afr_DadosDetalhados.Add("Perfil: " + afr_alvo.afr_Perfil);
                afr_resultado.afr_DadosDetalhados.Add(
                    "O progresso, o XP e as medalhas dele serao apagados");
                return afr_resultado;
            }

            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorId(afr_id);

            if (afr_usuario == null)
            {
                afr_resultado.afr_Elegivel = false;
                afr_resultado.afr_Motivo = "Usuario nao encontrado.";
                return afr_resultado;
            }

            // REGRA: ninguem exclui a propria conta
            if (afr_id == Sessao.ObterUsuarioId())
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
            if (afr_ponte.Ativa())
            {
                return afr_ponte.ExcluirUsuario(afr_id, out afr_mensagem);
            }

            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorId(afr_id);

            string afr_nome = afr_usuario != null ? afr_usuario.afr_Nome : "Id " + afr_id;

            if (!afr_bancoService.ExcluirUsuario(afr_id, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCrud(Sessao.ObterUsuarioId(),
                "Excluiu o usuario " + afr_nome + " (Id " + afr_id + ").");

            return true;
        }

        // afr_senhaOpcional: na edicao pela API, senha vazia = manter
        private bool Validar(Usuario afr_usuario, out string afr_mensagem,
                             bool afr_senhaOpcional = false)
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

            // A regra dos 8 caracteres vale para a senha DIGITADA. Numa
            // edicao em que a senha nao foi trocada, o que chega aqui ja e
            // o hash - comprido por natureza e sem nada para medir.
            bool afr_mantemSenha = afr_senhaOpcional && afr_usuario.afr_Senha == "";

            if (!afr_mantemSenha && !SenhaHash.EhQualquerHash(afr_usuario.afr_Senha) &&
                afr_usuario.afr_Senha.Length < 8)
            {
                afr_mensagem = "A senha precisa ter pelo menos 8 caracteres.";
                return false;
            }

            // Operador logico E (&&): o perfil precisa ser um dos tres
            if (afr_usuario.afr_Perfil != Usuario.PERFIL_ADMIN &&
                afr_usuario.afr_Perfil != Usuario.PERFIL_PROFESSOR &&
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
        private BancoService afr_bancoService = new BancoService();
        private PonteApi afr_ponte = new PonteApi();
        private LogService afr_logService = new LogService();

        // DS09 e DS10 - quem pode mexer em curso e o PROFESSOR (e o admin)
        public bool VerificarPermissaoProfessor(out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!Sessao.EstaAberta())
            {
                afr_mensagem = "Sessao encerrada. Entre novamente.";
                return false;
            }

            if (!Sessao.PodeGerenciarCursos())
            {
                afr_mensagem = "Apenas professor ou administrador pode gerenciar cursos.";
                return false;
            }

            return true;
        }

        // DS03 - exibirCursos(cursos): o catalogo do aluno
        public List<Curso> ExibirCursos()
        {
            FiltroCurso afr_filtro = new FiltroCurso();
            afr_filtro.afr_SomenteAtivos = true;

            if (afr_ponte.Ativa()) return FiltrarDaApi(afr_filtro);

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

            if (afr_ponte.Ativa()) return FiltrarDaApi(afr_filtro);

            return afr_bancoService.ListarCursos(afr_filtro);
        }

        // DS04 - localizarCurso(termo, filtros): a busca do PROFESSOR,
        // que enxerga tambem os cursos inativos
        public List<Curso> LocalizarCurso(FiltroCurso afr_filtro)
        {
            if (afr_filtro == null)
            {
                afr_filtro = new FiltroCurso();
            }

            afr_filtro.afr_SomenteAtivos = false;

            if (afr_ponte.Ativa()) return FiltrarDaApi(afr_filtro);

            return afr_bancoService.ListarCursos(afr_filtro);
        }

        // -------------------------------------------------------------
        // VERSAO 4 - os filtros da busca (DS04) no modo API.
        //
        // A API so filtra por texto. Carga horaria, situacao e "so os
        // que nao conclui" sao aplicados aqui, sobre a lista que ela
        // devolve - a mesma regra que o BancoService faz em SQL.
        // -------------------------------------------------------------
        private List<Curso> FiltrarDaApi(FiltroCurso afr_filtro)
        {
            List<Curso> afr_resultado = new List<Curso>();
            List<int> afr_concluidos = new List<int>();

            if (afr_filtro.afr_SomenteNaoConcluidos)
            {
                foreach (ProgressoCurso afr_linha in afr_ponte.MeuProgresso())
                {
                    if (afr_linha.afr_Resultado.afr_Aprovado)
                    {
                        afr_concluidos.Add(afr_linha.afr_Curso.afr_Id);
                    }
                }
            }

            foreach (Curso afr_curso in afr_ponte.ListarCursos(afr_filtro.afr_Termo))
            {
                if (afr_filtro.afr_SomenteAtivos && !afr_curso.afr_Ativo) continue;
                if (afr_filtro.afr_CargaMinima > 0 && afr_curso.afr_CargaHoraria < afr_filtro.afr_CargaMinima) continue;
                if (afr_filtro.afr_CargaMaxima > 0 && afr_curso.afr_CargaHoraria > afr_filtro.afr_CargaMaxima) continue;
                if (afr_concluidos.Contains(afr_curso.afr_Id)) continue;

                afr_resultado.Add(afr_curso);
            }

            return afr_resultado;
        }

        // DS09 - getDadosCurso(curso) / dadosCurso(titulo, desc)
        public Curso GetDadosCurso(int afr_id)
        {
            if (afr_ponte.Ativa()) return afr_ponte.ObterCurso(afr_id);

            return afr_bancoService.BuscarCursoPorId(afr_id);
        }

        public bool CriarCurso(Curso afr_curso, out string afr_mensagem)
        {
            if (!Validar(afr_curso, out afr_mensagem))
            {
                return false;
            }

            if (afr_ponte.Ativa()) return afr_ponte.CriarCurso(afr_curso, out afr_mensagem);

            if (!afr_bancoService.InserirCurso(afr_curso, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCurso(Sessao.ObterUsuarioId(),
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

            if (afr_ponte.Ativa()) return afr_ponte.EditarCurso(afr_curso, out afr_mensagem);

            if (!afr_bancoService.AtualizarCurso(afr_curso, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCurso(Sessao.ObterUsuarioId(),
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

            if (afr_ponte.Ativa())
            {
                return ElegibilidadePelaApi(afr_cursoId);
            }

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

        // -------------------------------------------------------------
        // VERSAO 4 - DS10 no modo API. So o admin tem rota de excluir
        // curso, e a API nao conta quantos alunos perdem o progresso.
        // -------------------------------------------------------------
        private ElegibilidadeExclusao ElegibilidadePelaApi(int afr_cursoId)
        {
            ElegibilidadeExclusao afr_resultado = new ElegibilidadeExclusao();
            Curso afr_curso = afr_ponte.ObterCurso(afr_cursoId);

            if (afr_curso == null)
            {
                afr_resultado.afr_Elegivel = false;
                afr_resultado.afr_Motivo = "Curso nao encontrado.";
                return afr_resultado;
            }

            if (!Sessao.EhAdministrador())
            {
                afr_resultado.afr_Elegivel = false;
                afr_resultado.afr_Motivo = "Na API, so o administrador exclui curso. " +
                                           "Voce pode desativar o curso em Editar.";
                return afr_resultado;
            }

            afr_resultado.afr_Elegivel = true;
            afr_resultado.afr_Motivo = "O curso pode ser excluido. Os alunos matriculados " +
                                       "perdem o progresso nele.";

            afr_resultado.afr_DadosDetalhados.Add(
                "Curso: " + afr_curso.afr_Titulo + " (" + afr_curso.afr_CargaHoraria + "h)");

            afr_resultado.afr_DadosDetalhados.Add(
                afr_curso.afr_TotalAulas + " aula(s) serao apagadas junto");

            return afr_resultado;
        }

        // DS10 - removerDefinitivamente(curso)
        public bool RemoverDefinitivamente(int afr_cursoId, out string afr_mensagem)
        {
            if (afr_ponte.Ativa()) return afr_ponte.ExcluirCurso(afr_cursoId, out afr_mensagem);

            Curso afr_curso = afr_bancoService.BuscarCursoPorId(afr_cursoId);

            string afr_titulo = afr_curso != null ? afr_curso.afr_Titulo : "Id " + afr_cursoId;

            if (!afr_bancoService.ExcluirCurso(afr_cursoId, out afr_mensagem))
            {
                return false;
            }

            afr_logService.RegistrarCurso(Sessao.ObterUsuarioId(),
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

        // -------------------------------------------------------------
        // VERSAO 3.3 - MATRICULA EXPLICITA
        //
        // O aluno se matricula antes de comecar o curso, como no
        // aplicativo. Decisao do grupo em 19/11.
        //
        // A PERMISSAO E CONFERIDA AQUI, nao na tela: so ALUNO se
        // matricula, do mesmo jeito que a rota da API e studentOnly.
        // Professor e administrador acompanham cursos, nao cursam.
        // -------------------------------------------------------------
        public bool Matricular(int afr_usuarioId, int afr_cursoId, out string afr_mensagem)
        {
            if (!Sessao.EstaAberta())
            {
                afr_mensagem = "Entre no sistema para se matricular.";
                return false;
            }

            if (Sessao.EhAdministrador() || Sessao.EhProfessor())
            {
                afr_mensagem = "Apenas aluno pode se matricular em um curso.";
                return false;
            }

            // VERSAO 4 - modo API: a propria API confere curso e matricula
            if (afr_ponte.Ativa())
            {
                return afr_ponte.Matricular(afr_cursoId, out afr_mensagem);
            }

            Curso afr_curso = afr_bancoService.BuscarCursoPorId(afr_cursoId);

            if (afr_curso == null || !afr_curso.afr_Ativo)
            {
                afr_mensagem = "Curso nao encontrado ou inativo.";
                return false;
            }

            // false aqui nao e erro: quer dizer que a matricula ja
            // existia. Nos dois casos o aluno termina matriculado, que e
            // o que a tela precisa saber.
            afr_bancoService.Matricular(afr_usuarioId, afr_cursoId);

            afr_logService.RegistrarCurso(afr_usuarioId,
                                          "Matriculou-se no curso Id " + afr_cursoId + ".");

            afr_mensagem = "Matricula realizada. Bons estudos!";
            return true;
        }

        public bool EstaMatriculado(int afr_usuarioId, int afr_cursoId)
        {
            // Pela API so da para saber da matricula de quem esta logado
            if (afr_ponte.Ativa()) return afr_ponte.EstaMatriculado(afr_cursoId);

            return afr_bancoService.EstaMatriculado(afr_usuarioId, afr_cursoId);
        }
    }

    // =================================================================
    // CLASSE 6 - AulaService
    // DS05 - abrirAula(aula) / exibirConteudo() / assistirAula(aula)
    // =================================================================
    public class AulaService
    {
        private BancoService afr_bancoService = new BancoService();
        private PonteApi afr_ponte = new PonteApi();

        public List<Aula> ListarAulas(int afr_cursoId, int afr_usuarioId)
        {
            // Pela API, o "concluida" de cada aula e sempre o de quem
            // esta logado: o token diz quem e
            if (afr_ponte.Ativa()) return afr_ponte.ListarAulas(afr_cursoId);

            return afr_bancoService.ListarAulasDoCurso(afr_cursoId, afr_usuarioId);
        }

        // DS05 - abrirAula(aula): traz a aula com o conteudo
        public Aula AbrirAula(int afr_aulaId, int afr_usuarioId)
        {
            if (afr_ponte.Ativa()) return afr_ponte.AbrirAula(afr_aulaId);

            return afr_bancoService.BuscarAulaPorId(afr_aulaId, afr_usuarioId);
        }

        public bool CriarAula(Aula afr_aula, out string afr_mensagem)
        {
            if (!Validar(afr_aula, out afr_mensagem))
            {
                return false;
            }

            if (afr_ponte.Ativa()) return afr_ponte.CriarAula(afr_aula, out afr_mensagem);

            return afr_bancoService.InserirAula(afr_aula, out afr_mensagem);
        }

        public bool EditarAula(Aula afr_aula, out string afr_mensagem)
        {
            if (!Validar(afr_aula, out afr_mensagem))
            {
                return false;
            }

            // VERSAO 5 - modo API: titulo e duracao (o texto da aula nao
            // tem coluna no banco do Azure)
            if (afr_ponte.Ativa())
            {
                return afr_ponte.EditarAula(afr_aula, out afr_mensagem);
            }

            return afr_bancoService.AtualizarAula(afr_aula, out afr_mensagem);
        }

        public bool ExcluirAula(int afr_id, out string afr_mensagem)
        {
            if (afr_ponte.Ativa()) return afr_ponte.ExcluirAula(afr_id, out afr_mensagem);

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
        private BancoService afr_bancoService = new BancoService();
        private PonteApi afr_ponte = new PonteApi();
        private LogService afr_logService = new LogService();

        // VERSAO 3 - quem paga o XP e as conquistas
        private GamificacaoService afr_gamificacaoService = new GamificacaoService();

        // DS05 - atualizarProgresso(aluno, aula) / salvarProgresso(progresso)
        //
        // Versao curta, para quem nao se importa com o XP.
        public bool AtualizarProgresso(int afr_usuarioId, int afr_aulaId,
                                       bool afr_concluida, out string afr_mensagem)
        {
            GanhoGamificacao afr_ignorado;

            return AtualizarProgresso(afr_usuarioId, afr_aulaId, afr_concluida,
                                      out afr_mensagem, out afr_ignorado);
        }

        // -------------------------------------------------------------
        // VERSAO 3 - a mesma operacao, devolvendo TAMBEM o que o aluno
        // ganhou, para a tela conseguir avisar "+10 XP".
        //
        // Este e o ponto unico de conclusao de aula que o item 3.4 da
        // analise do grupo pede: progresso e pontos sao atualizados
        // juntos, aqui, e em lugar nenhum mais.
        // -------------------------------------------------------------
        public bool AtualizarProgresso(int afr_usuarioId, int afr_aulaId,
                                       bool afr_concluida, out string afr_mensagem,
                                       out GanhoGamificacao afr_ganho)
        {
            afr_ganho = new GanhoGamificacao();

            // VERSAO 4 - SO ALUNO FAZ CURSO E GANHA XP (decisao do grupo).
            // Professor e administrador acompanham os cursos, nao cursam.
            // Igual a API, onde a rota de progresso e studentOnly. A tela
            // ja esconde o botao, mas a regra fica aqui.
            if (Sessao.EhAdministrador() || Sessao.EhProfessor())
            {
                afr_mensagem = "Apenas aluno registra progresso nas aulas.";
                return false;
            }

            // VERSAO 4 - modo API: a API confere a matricula, grava o
            // progresso e paga o XP, tudo na mesma chamada
            if (afr_ponte.Ativa())
            {
                Aula afr_daApi = afr_ponte.AbrirAula(afr_aulaId);

                if (afr_daApi == null)
                {
                    afr_mensagem = "Aula nao encontrada.";
                    return false;
                }

                return afr_ponte.MarcarProgresso(afr_daApi.afr_CursoId, afr_aulaId, afr_concluida,
                                                 out afr_ganho, out afr_mensagem);
            }

            // Precisa da aula antes de qualquer coisa para saber de QUAL
            // curso ela e: a matricula e por curso, nao por aula.
            Aula afr_aula = afr_bancoService.BuscarAulaPorId(afr_aulaId, afr_usuarioId);

            if (afr_aula == null)
            {
                afr_mensagem = "Aula nao encontrada.";
                return false;
            }

            // VERSAO 3.3 - PORTEIRO DA MATRICULA
            //
            // Sem matricula, nao mexe no progresso. A mesma regra e a
            // mesma mensagem da API (courseRoutes.js responde 403 com
            // este texto), para o aluno ler a mesma coisa no celular e
            // aqui.
            //
            // A checagem vale tambem para DESMARCAR, como na API. Na
            // pratica nao deveria acontecer - sem matricula nao ha o que
            // desmarcar -, mas uma porta so e porta se fecha dos dois
            // lados.
            if (!afr_bancoService.EstaMatriculado(afr_usuarioId, afr_aula.afr_CursoId))
            {
                afr_mensagem = "Matricule-se no curso antes de registrar o progresso.";
                return false;
            }

            bool afr_ok;

            if (afr_concluida)
            {
                afr_ok = afr_bancoService.MarcarAulaConcluida(afr_usuarioId, afr_aulaId, out afr_mensagem);
            }
            else
            {
                afr_ok = afr_bancoService.DesmarcarAulaConcluida(afr_usuarioId, afr_aulaId, out afr_mensagem);
            }

            if (!afr_ok)
            {
                return false;
            }

            string afr_texto = afr_concluida
                ? "Concluiu a aula Id " + afr_aulaId + "."
                : "Desmarcou a aula Id " + afr_aulaId + ".";

            afr_logService.RegistrarCurso(afr_usuarioId, afr_texto);

            if (afr_concluida)
            {
                afr_ganho = afr_gamificacaoService.RegistrarAulaConcluida(
                                afr_usuarioId, afr_aula.afr_CursoId, afr_aulaId);
            }
            else
            {
                afr_gamificacaoService.RegistrarAulaDesmarcada(afr_usuarioId, afr_aulaId);
            }

            return true;
        }



        public int ObterAulasConcluidas(int afr_usuarioId, int afr_cursoId)
        {
            // Pela API so se sabe o progresso de quem esta logado
            if (afr_ponte.Ativa())
            {
                return afr_usuarioId == Sessao.ObterUsuarioId()
                    ? afr_ponte.ContarConcluidas(afr_cursoId)
                    : 0;
            }

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
            // VERSAO 5 - modo API: o proprio progresso pelos cursos de quem
            // esta logado; o de outro aluno pela rota da nossa API
            if (afr_ponte.Ativa())
            {
                return afr_alunoId == Sessao.ObterUsuarioId()
                    ? afr_ponte.MeuProgresso()
                    : afr_ponte.ProgressoDoAluno(afr_alunoId);
            }

            return afr_bancoService.BuscarProgressoDoAluno(afr_alunoId);
        }

        // -------------------------------------------------------------
        // DS07 - elegivelParaVisualizacao(true)
        // So professor e admin consultam o progresso de OUTRA pessoa.
        // Qualquer um pode ver o proprio.
        // -------------------------------------------------------------
        public bool ElegivelParaVisualizacao(int afr_alunoId, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!Sessao.EstaAberta())
            {
                afr_mensagem = "Sessao encerrada. Entre novamente.";
                return false;
            }

            // O proprio progresso sempre pode ser visto
            if (afr_alunoId == Sessao.ObterUsuarioId())
            {
                return true;
            }

            if (!Sessao.PodeVerProgressoDeOutros())
            {
                afr_mensagem = "Apenas professor ou administrador pode consultar " +
                               "o progresso de outro aluno.";
                return false;
            }

            return true;
        }
    }

    // =================================================================
    // GamificacaoService   (VERSAO 3.1 - reescrito)
    //
    // Na versao 3.0 este servico continha a regra de XP e conquistas.
    // Agora a regra mora no BANCO, em um lote T-SQL que e porte quase
    // literal de awardLessonCompletion da API (api/src/gamificationService.js).
    //
    // POR QUE MUDOU DE LUGAR
    // Tres programas mexem no mesmo banco: este desktop, o site e o
    // aplicativo. Se cada um tivesse a sua copia da regra em C# ou em
    // JavaScript, bastava um deles mudar para os numeros divergirem.
    // Com a regra no banco, concluir uma aula pelo celular ou pelo
    // desktop produz exatamente o mesmo XP, as mesmas conquistas e a
    // mesma sequencia de dias.
    //
    // Sobrou para esta classe o papel de porta de entrada: traduzir o
    // que as telas pedem para as chamadas certas.
    // =================================================================
    public class GamificacaoService
    {
        private BancoService afr_bancoService = new BancoService();
        private PonteApi afr_ponte = new PonteApi();

        // -------------------------------------------------------------
        // O aluno concluiu uma aula.
        //
        // Devolve o que ele ganhou, para a tela avisar na hora.
        // Idempotente: repetir nao paga de novo - quem garante e a
        // restricao UQ_HistoricoXP_Evento, na tabela.
        // -------------------------------------------------------------
        public GanhoGamificacao RegistrarAulaConcluida(int afr_usuarioId, int afr_cursoId,
                                                       int afr_aulaId)
        {
            // VERSAO 3.3 - aqui havia uma chamada a GarantirMatricula,
            // que matriculava o aluno sozinho ao concluir a primeira
            // aula. Saiu: a matricula virou explicita, e quem confere se
            // ela existe e o ProgressoService, ANTES de gravar qualquer
            // progresso. Chegar ate aqui ja significa matriculado.
            return afr_bancoService.ConcederXpPorAula(afr_usuarioId, afr_aulaId);
        }

        // -------------------------------------------------------------
        // O aluno DESMARCOU a aula: o XP daquela aula volta atras, para
        // o numero continuar batendo com o que ele de fato concluiu.
        // As conquistas ja ganhas permanecem.
        // -------------------------------------------------------------
        public void RegistrarAulaDesmarcada(int afr_usuarioId, int afr_aulaId)
        {
            afr_bancoService.RemoverXpDaAula(afr_usuarioId, afr_aulaId);
        }

        // -------------------------------------------------------------
        // O painel do aluno: XP, nivel, sequencia de dias e medalhas.
        // -------------------------------------------------------------
        public Pontuacao ObterPontuacao(int afr_usuarioId)
        {
            // VERSAO 4 - modo API: o proprio aluno pela rota /me (com as
            // medalhas); outro aluno pelo ranking (sem as medalhas)
            if (afr_ponte.Ativa())
            {
                if (afr_usuarioId == Sessao.ObterUsuarioId())
                {
                    if (Sessao.EhAdministrador() || Sessao.EhProfessor())
                    {
                        Pontuacao afr_vazia = new Pontuacao();
                        afr_vazia.afr_UsuarioId = afr_usuarioId;
                        return afr_vazia;
                    }

                    return afr_ponte.MinhaPontuacao();
                }

                return afr_ponte.PontuacaoDoAluno(afr_usuarioId);
            }

            return afr_bancoService.ObterPontuacao(afr_usuarioId);
        }


        // -------------------------------------------------------------
        // VERSAO 3.2 - a classificacao por XP.
        //
        // A PERMISSAO E CONFERIDA AQUI, nao na tela. Se ficasse so na
        // tela, bastaria alguem chamar este metodo de outro lugar para
        // contornar. Aluno recebe lista vazia.
        //
        // Quem pode: professor e administrador - os mesmos que ja podem
        // ver o progresso de outras pessoas (PodeVerProgressoDeOutros).
        // -------------------------------------------------------------
        public List<LinhaRanking> ListarRanking(int afr_limite)
        {
            if (!Sessao.PodeVerProgressoDeOutros())
            {
                return new List<LinhaRanking>();
            }

            if (afr_ponte.Ativa())
            {
                List<LinhaRanking> afr_lista = afr_ponte.Ranking();

                if (afr_lista.Count > afr_limite)
                {
                    afr_lista = afr_lista.GetRange(0, afr_limite);
                }

                return afr_lista;
            }

            return afr_bancoService.ListarRanking(afr_limite);
        }

        // -------------------------------------------------------------
        // O extrato: de onde veio cada ponto.
        // -------------------------------------------------------------
        public List<RegistroXP> ListarExtrato(int afr_usuarioId, int afr_limite)
        {
            // A API nao devolve o extrato de XP
            if (afr_ponte.Ativa()) return new List<RegistroXP>();

            return afr_bancoService.ListarHistoricoXp(afr_usuarioId, afr_limite);
        }
    }

}
