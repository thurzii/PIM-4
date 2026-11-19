/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: BancoService.cs  (camada de acesso a dados)
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   VERSAO 2 - alinhada com a modelagem UML (Projeto_UML.asta)

   Esta e a UNICA classe do projeto que conhece SQL.

   BANCO....: CursosDB
   TABELAS..: Usuarios, Cursos, Aulas, Progresso,
              CodigosRecuperacao (DS02), LogAcoes (DS12)

   O banco e criado na primeira execucao. Quem ja tinha o banco da
   versao 1 nao precisa apagar nada: o metodo MigrarEstrutura
   acrescenta as colunas e tabelas novas sem tocar nos dados.

   OBSERVACAO SOBRE SENHAS: neste trabalho a senha e gravada como texto
   simples, para a demonstracao ficar clara. Em um sistema real ela
   seria guardada como hash (BCrypt, PBKDF2).
   ===================================================================== */

using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace PlataformaCursos
{
    public class BancoService
    {
        // -------------------------------------------------------------
        // CONFIGURACAO DO SERVIDOR
        // O sistema testa a lista abaixo, em ordem, e usa o primeiro
        // que responder. Assim cada integrante do grupo pode ter um
        // SQL Server diferente sem editar o codigo.
        // -------------------------------------------------------------
        private static readonly string[] afr_SERVIDORES = new string[]
        {
            @"(localdb)\MSSQLLocalDB",   // LocalDB, acompanha o Visual Studio
            @".\SQLEXPRESS",             // SQL Server Express
            @"localhost",                // instancia padrao local
            @"."                         // idem, forma abreviada
        };

        private const string afr_BANCO = "CursosDB";

        private static string afr_servidorAtual = "";

        private string ObterServidor()
        {
            if (afr_servidorAtual == "")
            {
                return afr_SERVIDORES[0];
            }

            return afr_servidorAtual;
        }

        private string MontarConexao(string afr_servidor, string afr_nomeBanco)
        {
            return "Server=" + afr_servidor + ";" +
                   "Database=" + afr_nomeBanco + ";" +
                   "Integrated Security=True;" +
                   "TrustServerCertificate=True;" +
                   "Connect Timeout=5;";
        }

        private string ObterConexao(string afr_nomeBanco)
        {
            return MontarConexao(ObterServidor(), afr_nomeBanco);
        }

        public string ObterConexaoPadrao()
        {
            return ObterConexao(afr_BANCO);
        }

        public string ObterDescricao()
        {
            return "Servidor: " + ObterServidor() + "  |  Banco: " + afr_BANCO;
        }

        public bool DetectarServidor(out string afr_mensagem)
        {
            if (afr_servidorAtual != "")
            {
                afr_mensagem = "Servidor em uso: " + afr_servidorAtual;
                return true;
            }

            List<string> afr_tentativas = new List<string>();

            foreach (string afr_servidor in afr_SERVIDORES)
            {
                try
                {
                    using (SqlConnection afr_conexao =
                           new SqlConnection(MontarConexao(afr_servidor, "master")))
                    {
                        afr_conexao.Open();

                        afr_servidorAtual = afr_servidor;

                        afr_mensagem = "Servidor encontrado: " + afr_servidor;
                        return true;
                    }
                }
                catch (Exception)
                {
                    afr_tentativas.Add(afr_servidor);
                }
            }

            afr_mensagem = "Nenhum SQL Server foi encontrado neste computador." +
                           Environment.NewLine +
                           "Servidores testados: " + string.Join(", ", afr_tentativas) +
                           Environment.NewLine +
                           "Instale o LocalDB pelo Visual Studio Installer " +
                           "(Componentes individuais > SQL Server Express LocalDB) " +
                           "ou acrescente o seu servidor na lista afr_SERVIDORES, " +
                           "no arquivo BancoService.cs.";

            return false;
        }

        // =============================================================
        // CRIACAO E MIGRACAO DO BANCO
        // =============================================================
        public bool PrepararBanco(out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!DetectarServidor(out afr_mensagem))
            {
                return false;
            }

            try
            {
                // 1) cria o banco, se nao existir
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexao("master")))
                {
                    afr_conexao.Open();

                    ExecutarComando(afr_conexao,
                        "IF DB_ID('" + afr_BANCO + "') IS NULL CREATE DATABASE " + afr_BANCO + ";");
                }

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    CriarTabelas(afr_conexao);
                    MigrarEstrutura(afr_conexao);
                    CriarDadosIniciais(afr_conexao);
                }

                afr_mensagem = "Banco de dados pronto.";
                return true;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao preparar o banco: " + afr_erro.Message;
                return false;
            }
        }

        private void ExecutarComando(SqlConnection afr_conexao, string afr_sql)
        {
            using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
            {
                afr_comando.ExecuteNonQuery();
            }
        }

        // -------------------------------------------------------------
        // AS SEIS TABELAS
        // -------------------------------------------------------------
        private void CriarTabelas(SqlConnection afr_conexao)
        {
            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.Usuarios','U') IS NULL " +
                "CREATE TABLE dbo.Usuarios (" +
                "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                "  Nome VARCHAR(100) NOT NULL," +
                "  Email VARCHAR(100) NOT NULL UNIQUE," +
                "  Senha VARCHAR(100) NOT NULL," +
                "  Perfil VARCHAR(10) NOT NULL," +
                "  Ativo BIT NOT NULL DEFAULT 1," +
                "  DataCadastro DATETIME NOT NULL DEFAULT GETDATE());");

            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.Cursos','U') IS NULL " +
                "CREATE TABLE dbo.Cursos (" +
                "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                "  Titulo VARCHAR(120) NOT NULL," +
                "  Descricao VARCHAR(500) NULL," +
                "  CargaHoraria INT NOT NULL DEFAULT 0," +
                "  PercentualMinimo INT NOT NULL DEFAULT 100," +
                "  Ativo BIT NOT NULL DEFAULT 1," +
                "  DataCadastro DATETIME NOT NULL DEFAULT GETDATE());");

            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.Aulas','U') IS NULL " +
                "CREATE TABLE dbo.Aulas (" +
                "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                "  CursoId INT NOT NULL," +
                "  Titulo VARCHAR(120) NOT NULL," +
                "  Ordem INT NOT NULL DEFAULT 1," +
                "  DuracaoMin INT NOT NULL DEFAULT 0," +
                "  Conteudo VARCHAR(MAX) NULL," +
                "  CONSTRAINT FK_Aulas_Cursos FOREIGN KEY (CursoId) " +
                "    REFERENCES dbo.Cursos(Id) ON DELETE CASCADE);");

            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.Progresso','U') IS NULL " +
                "CREATE TABLE dbo.Progresso (" +
                "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                "  UsuarioId INT NOT NULL," +
                "  AulaId INT NOT NULL," +
                "  DataConclusao DATETIME NOT NULL DEFAULT GETDATE()," +
                "  CONSTRAINT FK_Prog_Usuarios FOREIGN KEY (UsuarioId) " +
                "    REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE," +
                "  CONSTRAINT FK_Prog_Aulas FOREIGN KEY (AulaId) " +
                "    REFERENCES dbo.Aulas(Id) ON DELETE CASCADE," +
                "  CONSTRAINT UQ_Prog UNIQUE (UsuarioId, AulaId));");

            // DS02 - codigos enviados na recuperacao de senha
            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.CodigosRecuperacao','U') IS NULL " +
                "CREATE TABLE dbo.CodigosRecuperacao (" +
                "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                "  UsuarioId INT NOT NULL," +
                "  Codigo VARCHAR(10) NOT NULL," +
                "  Expiracao DATETIME NOT NULL," +
                "  Usado BIT NOT NULL DEFAULT 0," +
                "  DataGeracao DATETIME NOT NULL DEFAULT GETDATE()," +
                "  CONSTRAINT FK_Cod_Usuarios FOREIGN KEY (UsuarioId) " +
                "    REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE);");

            // DS12 - historico de acoes do usuario
            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.LogAcoes','U') IS NULL " +
                "CREATE TABLE dbo.LogAcoes (" +
                "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                "  UsuarioId INT NOT NULL," +
                "  Tipo VARCHAR(20) NOT NULL," +
                "  Descricao VARCHAR(300) NOT NULL," +
                "  DataHora DATETIME NOT NULL DEFAULT GETDATE()," +
                "  CONSTRAINT FK_Log_Usuarios FOREIGN KEY (UsuarioId) " +
                "    REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE);");

            // -------------------------------------------------------------
            // VERSAO 3.1 - GAMIFICACAO, no esquema da API do grupo
            //
            // Estas cinco tabelas sao copia fiel dos scripts dela:
            //   sql/Etapa1_Professor.sql    -> Matriculas, Cursos.ProfessorId
            //   sql/Etapa4_Gamificacao.sql  -> as quatro de gamificacao
            //
            // A versao 3.0 tinha tabelas proprias e COLIDIA com as dela:
            // as duas criavam dbo.Conquistas com significados diferentes
            // (aqui era "medalha que um aluno ganhou", la e o CATALOGO
            // de medalhas que existem). No mesmo banco, quem criasse
            // primeiro quebraria o outro. Agora ha um esquema so.
            //
            // Os IF OBJECT_ID fazem o programa conviver com o banco ja
            // montado pela API: se ela criou, aqui so se encontra pronto.
            // -------------------------------------------------------------

            // Matriculas - em quais cursos o aluno esta inscrito
            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.Matriculas','U') IS NULL " +
                "CREATE TABLE dbo.Matriculas (" +
                "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                "  UsuarioId INT NOT NULL," +
                "  CursoId INT NOT NULL," +
                "  DataMatricula DATETIME NOT NULL DEFAULT GETDATE()," +
                "  CONSTRAINT UQ_Matriculas_Usuario_Curso UNIQUE (UsuarioId, CursoId));");

            // Cursos.ProfessorId - qual professor responde pelo curso
            ExecutarComando(afr_conexao,
                "IF COL_LENGTH('dbo.Cursos','ProfessorId') IS NULL " +
                "ALTER TABLE dbo.Cursos ADD ProfessorId INT NULL;");

            // Gamificacao - XP e sequencia de dias, uma linha por usuario
            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.Gamificacao','U') IS NULL " +
                "CREATE TABLE dbo.Gamificacao (" +
                "  UsuarioId INT NOT NULL PRIMARY KEY," +
                "  XP INT NOT NULL CONSTRAINT DF_Gamificacao_XP DEFAULT 0," +
                "  SequenciaAtual INT NOT NULL CONSTRAINT DF_Gamificacao_Sequencia DEFAULT 0," +
                "  MelhorSequencia INT NOT NULL CONSTRAINT DF_Gamificacao_MelhorSequencia DEFAULT 0," +
                "  UltimaAtividade DATE NULL," +
                "  CONSTRAINT FK_Gamificacao_Usuarios FOREIGN KEY (UsuarioId) " +
                "    REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE);");

            // Conquistas - o CATALOGO das medalhas que existem
            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.Conquistas','U') IS NULL " +
                "CREATE TABLE dbo.Conquistas (" +
                "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                "  Codigo NVARCHAR(50) NOT NULL UNIQUE," +
                "  Nome NVARCHAR(100) NOT NULL," +
                "  Descricao NVARCHAR(300) NOT NULL," +
                "  Icone NVARCHAR(20) NOT NULL," +
                "  XPBonus INT NOT NULL CONSTRAINT DF_Conquistas_XPBonus DEFAULT 0);");

            // UsuarioConquistas - quais medalhas cada aluno ganhou
            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.UsuarioConquistas','U') IS NULL " +
                "CREATE TABLE dbo.UsuarioConquistas (" +
                "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                "  UsuarioId INT NOT NULL," +
                "  ConquistaId INT NOT NULL," +
                "  DataConquista DATETIME NOT NULL CONSTRAINT DF_UsuarioConquistas_Data DEFAULT GETDATE()," +
                "  CONSTRAINT UQ_UsuarioConquistas UNIQUE (UsuarioId, ConquistaId)," +
                "  CONSTRAINT FK_UsuarioConquistas_Usuarios FOREIGN KEY (UsuarioId) " +
                "    REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE," +
                "  CONSTRAINT FK_UsuarioConquistas_Conquistas FOREIGN KEY (ConquistaId) " +
                "    REFERENCES dbo.Conquistas(Id) ON DELETE CASCADE);");

            // HistoricoXP - uma linha por ganho de XP.
            // UQ_HistoricoXP_Evento e o que impede a mesma aula de pagar
            // duas vezes: a idempotencia mora aqui, no banco, e por isso
            // vale igual para o desktop, para o site e para o celular.
            ExecutarComando(afr_conexao,
                "IF OBJECT_ID('dbo.HistoricoXP','U') IS NULL " +
                "CREATE TABLE dbo.HistoricoXP (" +
                "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                "  UsuarioId INT NOT NULL," +
                "  Quantidade INT NOT NULL," +
                "  Motivo NVARCHAR(200) NOT NULL," +
                "  ReferenciaTipo NVARCHAR(30) NOT NULL," +
                "  ReferenciaId INT NOT NULL," +
                "  DataRegistro DATETIME NOT NULL CONSTRAINT DF_HistoricoXP_Data DEFAULT GETDATE()," +
                "  CONSTRAINT FK_HistoricoXP_Usuarios FOREIGN KEY (UsuarioId) " +
                "    REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE," +
                "  CONSTRAINT UQ_HistoricoXP_Evento UNIQUE (UsuarioId, ReferenciaTipo, ReferenciaId));");

            SemearConquistas(afr_conexao);
        }

        // -------------------------------------------------------------
        // VERSAO 3.1 - as cinco medalhas do catalogo.
        //
        // Os codigos, nomes, descricoes, icones e bonus sao exatamente
        // os de sql/Etapa4_Gamificacao.sql. Se divergissem, a mesma
        // medalha valeria XP diferente no celular e no desktop.
        //
        // O IF NOT EXISTS por codigo deixa rodar de novo sem duplicar.
        // -------------------------------------------------------------
        private void SemearConquistas(SqlConnection afr_conexao)
        {
            InserirConquista(afr_conexao, "PRIMEIRA_AULA", "Primeiros passos",
                "Conclua sua primeira aula.", "\U0001F3AF", 20);

            InserirConquista(afr_conexao, "CINCO_AULAS", "Ritmo de estudos",
                "Conclua cinco aulas.", "\U0001F4DA", 30);

            InserirConquista(afr_conexao, "DEZ_AULAS", "Estudante dedicado",
                "Conclua dez aulas.", "\U0001F3C5", 50);

            InserirConquista(afr_conexao, "PRIMEIRO_CURSO", "Curso concluido",
                "Conclua todas as aulas de um curso.", "\U0001F393", 100);

            InserirConquista(afr_conexao, "SEQUENCIA_7", "Uma semana de foco",
                "Estude por sete dias consecutivos.", "\U0001F525", 75);
        }

        private void InserirConquista(SqlConnection afr_conexao, string afr_codigo,
                                      string afr_nome, string afr_descricao,
                                      string afr_icone, int afr_xpBonus)
        {
            try
            {
                string afr_sql =
                    "IF NOT EXISTS (SELECT 1 FROM dbo.Conquistas WHERE Codigo = @codigo) " +
                    "INSERT INTO dbo.Conquistas (Codigo, Nome, Descricao, Icone, XPBonus) " +
                    "VALUES (@codigo, @nome, @descricao, @icone, @xpBonus);";

                using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                {
                    afr_comando.Parameters.AddWithValue("@codigo", afr_codigo);
                    afr_comando.Parameters.AddWithValue("@nome", afr_nome);
                    afr_comando.Parameters.AddWithValue("@descricao", afr_descricao);
                    afr_comando.Parameters.AddWithValue("@icone", afr_icone);
                    afr_comando.Parameters.AddWithValue("@xpBonus", afr_xpBonus);
                    afr_comando.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
            }
        }

        // -------------------------------------------------------------
        // MIGRACAO: acrescenta o que a versao 1 nao tinha.
        // COL_LENGTH devolve NULL quando a coluna nao existe, entao o
        // ALTER so roda uma vez. Nenhum dado e perdido.
        // -------------------------------------------------------------
        private void MigrarEstrutura(SqlConnection afr_conexao)
        {
            ExecutarComando(afr_conexao,
                "IF COL_LENGTH('dbo.Cursos','PercentualMinimo') IS NULL " +
                "ALTER TABLE dbo.Cursos ADD PercentualMinimo INT NOT NULL DEFAULT 100;");

            ExecutarComando(afr_conexao,
                "IF COL_LENGTH('dbo.Aulas','Conteudo') IS NULL " +
                "ALTER TABLE dbo.Aulas ADD Conteudo VARCHAR(MAX) NULL;");

            // O perfil passou de 2 para 3 opcoes; a coluna precisa caber PROFESSOR
            ExecutarComando(afr_conexao,
                "IF COL_LENGTH('dbo.Usuarios','Perfil') < 10 " +
                "ALTER TABLE dbo.Usuarios ALTER COLUMN Perfil VARCHAR(10) NOT NULL;");

            // VERSAO 3: o perfil chamava-se TUTOR e passou a PROFESSOR, para
            // bater com o contrato da API do grupo. Sem este UPDATE, quem ja
            // estava cadastrado como tutor ficaria sem nenhuma permissao,
            // porque o perfil gravado no banco nao casa com nenhuma constante.
            ExecutarComando(afr_conexao,
                "UPDATE dbo.Usuarios SET Perfil = 'PROFESSOR' WHERE Perfil = 'TUTOR';");
        }

        // -------------------------------------------------------------
        // DADOS INICIAIS - um usuario de cada perfil e dois cursos
        // -------------------------------------------------------------
        private void CriarDadosIniciais(SqlConnection afr_conexao)
        {
            using (SqlCommand afr_conta =
                   new SqlCommand("SELECT COUNT(*) FROM dbo.Usuarios;", afr_conexao))
            {
                if (Convert.ToInt32(afr_conta.ExecuteScalar()) > 0)
                {
                    // Banco da versao 1: garante que exista um professor
                    ExecutarComando(afr_conexao,
                        "IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Perfil = 'PROFESSOR') " +
                        "INSERT INTO dbo.Usuarios (Nome, Email, Senha, Perfil) VALUES " +
                        "('Carla Mendes','carla@escola.com','professor123','PROFESSOR');");

                    return;
                }
            }

            ExecutarComando(afr_conexao,
                "INSERT INTO dbo.Usuarios (Nome, Email, Senha, Perfil) VALUES " +
                "('Administrador do Sistema','admin@escola.com','admin123','ADMIN')," +
                "('Carla Mendes','carla@escola.com','professor123','PROFESSOR')," +
                "('Maria Souza','maria@escola.com','aluno123','ALUNO')," +
                "('Joao Lima','joao@escola.com','aluno123','ALUNO');");

            ExecutarComando(afr_conexao,
                "INSERT INTO dbo.Cursos (Titulo, Descricao, CargaHoraria, PercentualMinimo) VALUES " +
                "('Logica de Programacao','Introducao a algoritmos, variaveis e estruturas de controle.',40,100)," +
                "('Banco de Dados com SQL Server','Modelagem relacional, comandos SQL e consultas.',60,75);");

            InserirAulaInicial(afr_conexao, "Logica de Programacao", "O que e um algoritmo", 1, 45,
                "Um algoritmo e uma sequencia finita de passos que resolve um problema. " +
                "Nesta aula vemos exemplos do dia a dia e como descrever um passo a passo " +
                "sem ambiguidade.");

            InserirAulaInicial(afr_conexao, "Logica de Programacao", "Variaveis e tipos de dados", 2, 50,
                "Variavel e um espaco na memoria com nome. Cada variavel guarda um tipo de " +
                "dado: inteiro, decimal, texto ou logico. Escolher o tipo certo evita erro " +
                "de conversao mais tarde.");

            InserirAulaInicial(afr_conexao, "Logica de Programacao", "Condicoes e repeticoes", 3, 60,
                "Condicao (if) escolhe um caminho; repeticao (for, while) executa o mesmo " +
                "trecho varias vezes. Juntas, elas dao ao programa a capacidade de decidir " +
                "e de insistir.");

            InserirAulaInicial(afr_conexao, "Banco de Dados com SQL Server", "Modelo relacional", 1, 50,
                "Os dados ficam em tabelas ligadas por chaves. A chave primaria identifica " +
                "a linha; a chave estrangeira aponta para outra tabela. E o que permite " +
                "montar o sistema sem repetir informacao.");

            InserirAulaInicial(afr_conexao, "Banco de Dados com SQL Server", "SELECT, INSERT, UPDATE e DELETE", 2, 70,
                "Os quatro comandos do CRUD. Atencao ao WHERE: sem ele, o UPDATE altera a " +
                "tabela inteira e o DELETE apaga tudo. Valores sempre por parametro, nunca " +
                "colados no texto do comando.");
        }

        private void InserirAulaInicial(SqlConnection afr_conexao, string afr_curso,
                                        string afr_titulo, int afr_ordem,
                                        int afr_duracao, string afr_conteudo)
        {
            string afr_sql = "INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin, Conteudo) " +
                             "SELECT Id, @titulo, @ordem, @duracao, @conteudo " +
                             "FROM dbo.Cursos WHERE Titulo = @curso;";

            using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
            {
                afr_comando.Parameters.AddWithValue("@curso", afr_curso);
                afr_comando.Parameters.AddWithValue("@titulo", afr_titulo);
                afr_comando.Parameters.AddWithValue("@ordem", afr_ordem);
                afr_comando.Parameters.AddWithValue("@duracao", afr_duracao);
                afr_comando.Parameters.AddWithValue("@conteudo", afr_conteudo);

                afr_comando.ExecuteNonQuery();
            }
        }

        public bool TestarConexao(out string afr_mensagem)
        {
            try
            {
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    afr_mensagem = "Conexao OK com " + afr_BANCO +
                                   " (versao " + afr_conexao.ServerVersion + ").";
                    return true;
                }
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Falha na conexao: " + afr_erro.Message;
                return false;
            }
        }

        // =============================================================
        // ================        U S U A R I O S       ===============
        // =============================================================

        public List<Usuario> ListarUsuarios(string afr_termo)
        {
            return ListarUsuarios(afr_termo, "");
        }

        // afr_perfil vazio = todos os perfis
        public List<Usuario> ListarUsuarios(string afr_termo, string afr_perfil)
        {
            List<Usuario> afr_lista = new List<Usuario>();

            try
            {
                string afr_sql = "SELECT Id, Nome, Email, Senha, Perfil, Ativo, DataCadastro " +
                                 "FROM dbo.Usuarios " +
                                 "WHERE (@termo = '' OR Nome LIKE '%' + @termo + '%' " +
                                 "                  OR Email LIKE '%' + @termo + '%') " +
                                 "  AND (@perfil = '' OR Perfil = @perfil) " +
                                 "ORDER BY Nome;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@termo", afr_termo ?? "");
                        afr_comando.Parameters.AddWithValue("@perfil", afr_perfil ?? "");

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            while (afr_leitor.Read())
                            {
                                afr_lista.Add(LerUsuario(afr_leitor));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return afr_lista;
        }

        private Usuario LerUsuario(SqlDataReader afr_leitor)
        {
            Usuario afr_usuario = new Usuario();

            afr_usuario.afr_Id = Convert.ToInt32(afr_leitor["Id"]);
            afr_usuario.afr_Nome = afr_leitor["Nome"].ToString();
            afr_usuario.afr_Email = afr_leitor["Email"].ToString();
            afr_usuario.afr_Senha = afr_leitor["Senha"].ToString();
            afr_usuario.afr_Perfil = afr_leitor["Perfil"].ToString();
            afr_usuario.afr_Ativo = Convert.ToBoolean(afr_leitor["Ativo"]);

            if (afr_leitor["DataCadastro"] != DBNull.Value)
            {
                afr_usuario.afr_DataCadastro = Convert.ToDateTime(afr_leitor["DataCadastro"]);
            }

            return afr_usuario;
        }

        public Usuario BuscarUsuarioPorEmail(string afr_email)
        {
            return BuscarUsuario("Email = @chave", afr_email);
        }

        public Usuario BuscarUsuarioPorId(int afr_id)
        {
            return BuscarUsuario("Id = @chave", afr_id);
        }

        private Usuario BuscarUsuario(string afr_condicao, object afr_valor)
        {
            try
            {
                string afr_sql = "SELECT Id, Nome, Email, Senha, Perfil, Ativo, DataCadastro " +
                                 "FROM dbo.Usuarios WHERE " + afr_condicao + ";";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@chave", afr_valor);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            if (afr_leitor.Read())
                            {
                                return LerUsuario(afr_leitor);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        // DS11 - verificarDuplicidade(email) / emailDisponivel(true)
        // afr_idIgnorar serve na EDICAO: o proprio registro nao conta.
        public bool EmailJaExiste(string afr_email, int afr_idIgnorar)
        {
            try
            {
                string afr_sql = "SELECT COUNT(*) FROM dbo.Usuarios " +
                                 "WHERE Email = @email AND Id <> @id;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@email", afr_email);
                        afr_comando.Parameters.AddWithValue("@id", afr_idIgnorar);

                        return Convert.ToInt32(afr_comando.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InserirUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "INSERT INTO dbo.Usuarios (Nome, Email, Senha, Perfil, Ativo) " +
                                 "VALUES (@nome, @email, @senha, @perfil, @ativo);";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@nome", afr_usuario.afr_Nome);
                        afr_comando.Parameters.AddWithValue("@email", afr_usuario.afr_Email);
                        afr_comando.Parameters.AddWithValue("@senha", afr_usuario.afr_Senha);
                        afr_comando.Parameters.AddWithValue("@perfil", afr_usuario.afr_Perfil);
                        afr_comando.Parameters.AddWithValue("@ativo", afr_usuario.afr_Ativo);

                        afr_comando.ExecuteNonQuery();
                    }
                }

                afr_mensagem = "Usuario cadastrado com sucesso.";
                return true;
            }
            catch (SqlException afr_erro)
            {
                if (afr_erro.Number == 2627 || afr_erro.Number == 2601)
                {
                    afr_mensagem = "Ja existe um usuario com este email.";
                }
                else
                {
                    afr_mensagem = "Erro ao gravar: " + afr_erro.Message;
                }

                return false;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao gravar: " + afr_erro.Message;
                return false;
            }
        }

        public bool AtualizarUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "UPDATE dbo.Usuarios SET " +
                                 "  Nome = @nome, Email = @email, Senha = @senha," +
                                 "  Perfil = @perfil, Ativo = @ativo " +
                                 "WHERE Id = @id;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@nome", afr_usuario.afr_Nome);
                        afr_comando.Parameters.AddWithValue("@email", afr_usuario.afr_Email);
                        afr_comando.Parameters.AddWithValue("@senha", afr_usuario.afr_Senha);
                        afr_comando.Parameters.AddWithValue("@perfil", afr_usuario.afr_Perfil);
                        afr_comando.Parameters.AddWithValue("@ativo", afr_usuario.afr_Ativo);
                        afr_comando.Parameters.AddWithValue("@id", afr_usuario.afr_Id);

                        int afr_linhas = afr_comando.ExecuteNonQuery();

                        if (afr_linhas > 0)
                        {
                            afr_mensagem = "Usuario alterado com sucesso.";
                            return true;
                        }

                        afr_mensagem = "Nenhum usuario foi alterado.";
                        return false;
                    }
                }
            }
            catch (SqlException afr_erro)
            {
                if (afr_erro.Number == 2627 || afr_erro.Number == 2601)
                {
                    afr_mensagem = "Ja existe outro usuario com este email.";
                }
                else
                {
                    afr_mensagem = "Erro ao alterar: " + afr_erro.Message;
                }

                return false;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao alterar: " + afr_erro.Message;
                return false;
            }
        }

        public bool AtualizarSenha(string afr_email, string afr_novaSenha, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "UPDATE dbo.Usuarios SET Senha = @senha WHERE Email = @email;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@senha", afr_novaSenha);
                        afr_comando.Parameters.AddWithValue("@email", afr_email);

                        int afr_linhas = afr_comando.ExecuteNonQuery();

                        if (afr_linhas > 0)
                        {
                            afr_mensagem = "Senha redefinida com sucesso.";
                            return true;
                        }

                        afr_mensagem = "Email nao encontrado.";
                        return false;
                    }
                }
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao redefinir a senha: " + afr_erro.Message;
                return false;
            }
        }

        public bool ExcluirUsuario(int afr_id, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando =
                           new SqlCommand("DELETE FROM dbo.Usuarios WHERE Id = @id;", afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@id", afr_id);

                        int afr_linhas = afr_comando.ExecuteNonQuery();

                        afr_mensagem = afr_linhas + " usuario(s) excluido(s).";
                        return afr_linhas > 0;
                    }
                }
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao excluir: " + afr_erro.Message;
                return false;
            }
        }

        // DS14 - dados que serao apagados junto com o usuario
        public int ContarProgressoDoUsuario(int afr_usuarioId)
        {
            return ContarEscalar(
                "SELECT COUNT(*) FROM dbo.Progresso WHERE UsuarioId = @id;", afr_usuarioId);
        }

        public int ContarLogDoUsuario(int afr_usuarioId)
        {
            return ContarEscalar(
                "SELECT COUNT(*) FROM dbo.LogAcoes WHERE UsuarioId = @id;", afr_usuarioId);
        }

        private int ContarEscalar(string afr_sql, int afr_id)
        {
            try
            {
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@id", afr_id);
                        return Convert.ToInt32(afr_comando.ExecuteScalar());
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // =============================================================
        // ==================        C U R S O S       =================
        // =============================================================

        // DS04 - buscarCurso(termo, FILTROS)
        public List<Curso> ListarCursos(FiltroCurso afr_filtro)
        {
            List<Curso> afr_lista = new List<Curso>();

            if (afr_filtro == null)
            {
                afr_filtro = new FiltroCurso();
            }

            try
            {
                string afr_sql =
                    "SELECT c.Id, c.Titulo, c.Descricao, c.CargaHoraria, c.Ativo, " +
                    "       c.PercentualMinimo, " +
                    "       (SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId = c.Id) AS TotalAulas " +
                    "FROM dbo.Cursos c " +
                    "WHERE (@termo = '' OR c.Titulo LIKE '%' + @termo + '%' " +
                    "                   OR c.Descricao LIKE '%' + @termo + '%') " +
                    "  AND (@somenteAtivos = 0 OR c.Ativo = 1) " +
                    "  AND (@cargaMin = 0 OR c.CargaHoraria >= @cargaMin) " +
                    "  AND (@cargaMax = 0 OR c.CargaHoraria <= @cargaMax) " +
                    "  AND (@soNaoConcluidos = 0 " +
                    "       OR (SELECT COUNT(*) FROM dbo.Aulas a1 WHERE a1.CursoId = c.Id) = 0 " +
                    "       OR (SELECT COUNT(*) FROM dbo.Progresso p " +
                    "             INNER JOIN dbo.Aulas a2 ON a2.Id = p.AulaId " +
                    "            WHERE a2.CursoId = c.Id AND p.UsuarioId = @usuarioId) " +
                    "          < (SELECT COUNT(*) FROM dbo.Aulas a3 WHERE a3.CursoId = c.Id)) " +
                    "ORDER BY c.Titulo;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@termo", (afr_filtro.afr_Termo ?? "").Trim());
                        afr_comando.Parameters.AddWithValue("@somenteAtivos", afr_filtro.afr_SomenteAtivos ? 1 : 0);
                        afr_comando.Parameters.AddWithValue("@cargaMin", afr_filtro.afr_CargaMinima);
                        afr_comando.Parameters.AddWithValue("@cargaMax", afr_filtro.afr_CargaMaxima);
                        afr_comando.Parameters.AddWithValue("@soNaoConcluidos", afr_filtro.afr_SomenteNaoConcluidos ? 1 : 0);
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_filtro.afr_UsuarioId);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            while (afr_leitor.Read())
                            {
                                afr_lista.Add(LerCurso(afr_leitor));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return afr_lista;
        }

        private Curso LerCurso(SqlDataReader afr_leitor)
        {
            Curso afr_curso = new Curso();

            afr_curso.afr_Id = Convert.ToInt32(afr_leitor["Id"]);
            afr_curso.afr_Titulo = afr_leitor["Titulo"].ToString();
            afr_curso.afr_Descricao = afr_leitor["Descricao"].ToString();
            afr_curso.afr_CargaHoraria = Convert.ToInt32(afr_leitor["CargaHoraria"]);
            afr_curso.afr_Ativo = Convert.ToBoolean(afr_leitor["Ativo"]);
            afr_curso.afr_TotalAulas = Convert.ToInt32(afr_leitor["TotalAulas"]);

            if (afr_leitor["PercentualMinimo"] != DBNull.Value)
            {
                afr_curso.afr_PercentualMinimo = Convert.ToInt32(afr_leitor["PercentualMinimo"]);
            }

            return afr_curso;
        }

        public Curso BuscarCursoPorId(int afr_id)
        {
            try
            {
                string afr_sql =
                    "SELECT c.Id, c.Titulo, c.Descricao, c.CargaHoraria, c.Ativo, " +
                    "       c.PercentualMinimo, " +
                    "       (SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId = c.Id) AS TotalAulas " +
                    "FROM dbo.Cursos c WHERE c.Id = @id;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@id", afr_id);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            if (afr_leitor.Read())
                            {
                                return LerCurso(afr_leitor);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public bool InserirCurso(Curso afr_curso, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "INSERT INTO dbo.Cursos " +
                                 "(Titulo, Descricao, CargaHoraria, PercentualMinimo, Ativo) " +
                                 "VALUES (@titulo, @descricao, @carga, @minimo, @ativo);";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@titulo", afr_curso.afr_Titulo);
                        afr_comando.Parameters.AddWithValue("@descricao", afr_curso.afr_Descricao ?? "");
                        afr_comando.Parameters.AddWithValue("@carga", afr_curso.afr_CargaHoraria);
                        afr_comando.Parameters.AddWithValue("@minimo", afr_curso.afr_PercentualMinimo);
                        afr_comando.Parameters.AddWithValue("@ativo", afr_curso.afr_Ativo);

                        afr_comando.ExecuteNonQuery();
                    }
                }

                afr_mensagem = "Curso cadastrado com sucesso.";
                return true;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao gravar o curso: " + afr_erro.Message;
                return false;
            }
        }

        public bool AtualizarCurso(Curso afr_curso, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "UPDATE dbo.Cursos SET " +
                                 "  Titulo = @titulo, Descricao = @descricao," +
                                 "  CargaHoraria = @carga, PercentualMinimo = @minimo," +
                                 "  Ativo = @ativo " +
                                 "WHERE Id = @id;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@titulo", afr_curso.afr_Titulo);
                        afr_comando.Parameters.AddWithValue("@descricao", afr_curso.afr_Descricao ?? "");
                        afr_comando.Parameters.AddWithValue("@carga", afr_curso.afr_CargaHoraria);
                        afr_comando.Parameters.AddWithValue("@minimo", afr_curso.afr_PercentualMinimo);
                        afr_comando.Parameters.AddWithValue("@ativo", afr_curso.afr_Ativo);
                        afr_comando.Parameters.AddWithValue("@id", afr_curso.afr_Id);

                        int afr_linhas = afr_comando.ExecuteNonQuery();

                        if (afr_linhas > 0)
                        {
                            afr_mensagem = "Curso alterado com sucesso.";
                            return true;
                        }

                        afr_mensagem = "Nenhum curso foi alterado.";
                        return false;
                    }
                }
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao alterar o curso: " + afr_erro.Message;
                return false;
            }
        }

        public bool ExcluirCurso(int afr_id, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando =
                           new SqlCommand("DELETE FROM dbo.Cursos WHERE Id = @id;", afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@id", afr_id);

                        int afr_linhas = afr_comando.ExecuteNonQuery();

                        afr_mensagem = afr_linhas + " curso(s) excluido(s), com as aulas.";
                        return afr_linhas > 0;
                    }
                }
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao excluir o curso: " + afr_erro.Message;
                return false;
            }
        }

        // DS10 - dados detalhados: quantos alunos ja mexeram no curso
        public int ContarAlunosComProgressoNoCurso(int afr_cursoId)
        {
            try
            {
                string afr_sql =
                    "SELECT COUNT(DISTINCT p.UsuarioId) FROM dbo.Progresso p " +
                    "INNER JOIN dbo.Aulas a ON a.Id = p.AulaId " +
                    "WHERE a.CursoId = @id;";

                return ContarEscalar(afr_sql, afr_cursoId);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // =============================================================
        // ===================        A U L A S       ==================
        // =============================================================

        public List<Aula> ListarAulasDoCurso(int afr_cursoId, int afr_usuarioId)
        {
            List<Aula> afr_lista = new List<Aula>();

            try
            {
                string afr_sql =
                    "SELECT a.Id, a.CursoId, a.Titulo, a.Ordem, a.DuracaoMin, a.Conteudo, " +
                    "       CASE WHEN p.Id IS NULL THEN 0 ELSE 1 END AS Concluida " +
                    "FROM dbo.Aulas a " +
                    "LEFT JOIN dbo.Progresso p " +
                    "       ON p.AulaId = a.Id AND p.UsuarioId = @usuarioId " +
                    "WHERE a.CursoId = @cursoId " +
                    "ORDER BY a.Ordem, a.Id;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@cursoId", afr_cursoId);
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            while (afr_leitor.Read())
                            {
                                afr_lista.Add(LerAula(afr_leitor));
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return afr_lista;
        }

        private Aula LerAula(SqlDataReader afr_leitor)
        {
            Aula afr_aula = new Aula();

            afr_aula.afr_Id = Convert.ToInt32(afr_leitor["Id"]);
            afr_aula.afr_CursoId = Convert.ToInt32(afr_leitor["CursoId"]);
            afr_aula.afr_Titulo = afr_leitor["Titulo"].ToString();
            afr_aula.afr_Ordem = Convert.ToInt32(afr_leitor["Ordem"]);
            afr_aula.afr_DuracaoMin = Convert.ToInt32(afr_leitor["DuracaoMin"]);
            afr_aula.afr_Concluida = Convert.ToInt32(afr_leitor["Concluida"]) == 1;

            if (afr_leitor["Conteudo"] != DBNull.Value)
            {
                afr_aula.afr_Conteudo = afr_leitor["Conteudo"].ToString();
            }

            return afr_aula;
        }

        // DS05 - abrirAula(aula): busca UMA aula com o conteudo
        public Aula BuscarAulaPorId(int afr_aulaId, int afr_usuarioId)
        {
            try
            {
                string afr_sql =
                    "SELECT a.Id, a.CursoId, a.Titulo, a.Ordem, a.DuracaoMin, a.Conteudo, " +
                    "       CASE WHEN p.Id IS NULL THEN 0 ELSE 1 END AS Concluida " +
                    "FROM dbo.Aulas a " +
                    "LEFT JOIN dbo.Progresso p " +
                    "       ON p.AulaId = a.Id AND p.UsuarioId = @usuarioId " +
                    "WHERE a.Id = @aulaId;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@aulaId", afr_aulaId);
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            if (afr_leitor.Read())
                            {
                                return LerAula(afr_leitor);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public bool InserirAula(Aula afr_aula, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin, Conteudo) " +
                                 "VALUES (@cursoId, @titulo, @ordem, @duracao, @conteudo);";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@cursoId", afr_aula.afr_CursoId);
                        afr_comando.Parameters.AddWithValue("@titulo", afr_aula.afr_Titulo);
                        afr_comando.Parameters.AddWithValue("@ordem", afr_aula.afr_Ordem);
                        afr_comando.Parameters.AddWithValue("@duracao", afr_aula.afr_DuracaoMin);
                        afr_comando.Parameters.AddWithValue("@conteudo", afr_aula.afr_Conteudo ?? "");

                        afr_comando.ExecuteNonQuery();
                    }
                }

                afr_mensagem = "Aula cadastrada com sucesso.";
                return true;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao gravar a aula: " + afr_erro.Message;
                return false;
            }
        }

        public bool AtualizarAula(Aula afr_aula, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "UPDATE dbo.Aulas SET " +
                                 "  Titulo = @titulo, Ordem = @ordem," +
                                 "  DuracaoMin = @duracao, Conteudo = @conteudo " +
                                 "WHERE Id = @id;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@titulo", afr_aula.afr_Titulo);
                        afr_comando.Parameters.AddWithValue("@ordem", afr_aula.afr_Ordem);
                        afr_comando.Parameters.AddWithValue("@duracao", afr_aula.afr_DuracaoMin);
                        afr_comando.Parameters.AddWithValue("@conteudo", afr_aula.afr_Conteudo ?? "");
                        afr_comando.Parameters.AddWithValue("@id", afr_aula.afr_Id);

                        int afr_linhas = afr_comando.ExecuteNonQuery();

                        if (afr_linhas > 0)
                        {
                            afr_mensagem = "Aula alterada com sucesso.";
                            return true;
                        }

                        afr_mensagem = "Nenhuma aula foi alterada.";
                        return false;
                    }
                }
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao alterar a aula: " + afr_erro.Message;
                return false;
            }
        }

        public bool ExcluirAula(int afr_id, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando =
                           new SqlCommand("DELETE FROM dbo.Aulas WHERE Id = @id;", afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@id", afr_id);

                        int afr_linhas = afr_comando.ExecuteNonQuery();

                        afr_mensagem = afr_linhas + " aula(s) excluida(s).";
                        return afr_linhas > 0;
                    }
                }
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao excluir a aula: " + afr_erro.Message;
                return false;
            }
        }

        // =============================================================
        // ================        P R O G R E S S O       =============
        // =============================================================

        public bool MarcarAulaConcluida(int afr_usuarioId, int afr_aulaId, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql =
                    "IF NOT EXISTS (SELECT 1 FROM dbo.Progresso " +
                    "               WHERE UsuarioId = @usuarioId AND AulaId = @aulaId) " +
                    "INSERT INTO dbo.Progresso (UsuarioId, AulaId) VALUES (@usuarioId, @aulaId);";

                ExecutarComProgresso(afr_sql, afr_usuarioId, afr_aulaId);

                afr_mensagem = "Aula marcada como concluida.";
                return true;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao marcar a aula: " + afr_erro.Message;
                return false;
            }
        }

        public bool DesmarcarAulaConcluida(int afr_usuarioId, int afr_aulaId, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "DELETE FROM dbo.Progresso " +
                                 "WHERE UsuarioId = @usuarioId AND AulaId = @aulaId;";

                ExecutarComProgresso(afr_sql, afr_usuarioId, afr_aulaId);

                afr_mensagem = "Marcacao desfeita.";
                return true;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao desmarcar a aula: " + afr_erro.Message;
                return false;
            }
        }

        private void ExecutarComProgresso(string afr_sql, int afr_usuarioId, int afr_aulaId)
        {
            using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
            {
                afr_conexao.Open();

                using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                {
                    afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);
                    afr_comando.Parameters.AddWithValue("@aulaId", afr_aulaId);

                    afr_comando.ExecuteNonQuery();
                }
            }
        }

        public int ContarAulasConcluidas(int afr_usuarioId, int afr_cursoId)
        {
            try
            {
                string afr_sql =
                    "SELECT COUNT(*) FROM dbo.Progresso p " +
                    "INNER JOIN dbo.Aulas a ON a.Id = p.AulaId " +
                    "WHERE p.UsuarioId = @usuarioId AND a.CursoId = @cursoId;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);
                        afr_comando.Parameters.AddWithValue("@cursoId", afr_cursoId);

                        return Convert.ToInt32(afr_comando.ExecuteScalar());
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        // DS07 - buscarProgresso(aluno): uma linha por curso em que o
        // aluno concluiu pelo menos uma aula, ou todos os cursos ativos.
        public List<ProgressoCurso> BuscarProgressoDoAluno(int afr_alunoId)
        {
            List<ProgressoCurso> afr_lista = new List<ProgressoCurso>();

            try
            {
                string afr_sql =
                    "SELECT c.Id, c.Titulo, c.Descricao, c.CargaHoraria, c.Ativo, " +
                    "       c.PercentualMinimo, " +
                    "       (SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId = c.Id) AS TotalAulas, " +
                    "       (SELECT COUNT(*) FROM dbo.Progresso p " +
                    "          INNER JOIN dbo.Aulas a2 ON a2.Id = p.AulaId " +
                    "         WHERE a2.CursoId = c.Id AND p.UsuarioId = @alunoId) AS Concluidas " +
                    "FROM dbo.Cursos c " +
                    "WHERE c.Ativo = 1 " +
                    "ORDER BY c.Titulo;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@alunoId", afr_alunoId);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            while (afr_leitor.Read())
                            {
                                ProgressoCurso afr_item = new ProgressoCurso();

                                afr_item.afr_Curso = LerCurso(afr_leitor);

                                int afr_total = afr_item.afr_Curso.afr_TotalAulas;
                                int afr_conc = Convert.ToInt32(afr_leitor["Concluidas"]);

                                afr_item.afr_Resultado = MontarResultado(
                                    afr_conc, afr_total, afr_item.afr_Curso);

                                afr_lista.Add(afr_item);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return afr_lista;
        }

        // DS08 - resultadoConclusao(status, percentual)
        public ResultadoConclusao MontarResultado(int afr_concluidas, int afr_total, Curso afr_curso)
        {
            ResultadoConclusao afr_resultado = new ResultadoConclusao();

            afr_resultado.afr_Concluidas = afr_concluidas;
            afr_resultado.afr_Total = afr_total;
            afr_resultado.afr_PercentualMinimo = afr_curso != null ? afr_curso.afr_PercentualMinimo : 100;

            if (afr_total > 0)
            {
                double afr_calculo = (double)afr_concluidas / afr_total * 100;
                afr_resultado.afr_Percentual = (int)Math.Round(afr_calculo);
            }
            else
            {
                afr_resultado.afr_Percentual = 0;
            }

            if (afr_resultado.afr_Percentual > 100)
            {
                afr_resultado.afr_Percentual = 100;
            }

            if (afr_resultado.afr_Percentual < 0)
            {
                afr_resultado.afr_Percentual = 0;
            }

            // Operador logico E (&&)
            afr_resultado.afr_Aprovado = afr_total > 0 &&
                                         afr_resultado.afr_Percentual >= afr_resultado.afr_PercentualMinimo;

            if (afr_curso != null)
            {
                afr_resultado.afr_Requisitos = afr_curso.ObterRequisitos();
            }

            return afr_resultado;
        }

        // =============================================================
        // ===  C O D I G O S   D E   R E C U P E R A C A O  (DS02)  ===
        // =============================================================

        public bool GravarCodigo(int afr_usuarioId, string afr_codigo,
                                 DateTime afr_expiracao, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    // Um codigo novo invalida os anteriores do mesmo usuario
                    using (SqlCommand afr_limpar = new SqlCommand(
                           "UPDATE dbo.CodigosRecuperacao SET Usado = 1 WHERE UsuarioId = @id;",
                           afr_conexao))
                    {
                        afr_limpar.Parameters.AddWithValue("@id", afr_usuarioId);
                        afr_limpar.ExecuteNonQuery();
                    }

                    using (SqlCommand afr_comando = new SqlCommand(
                           "INSERT INTO dbo.CodigosRecuperacao (UsuarioId, Codigo, Expiracao) " +
                           "VALUES (@id, @codigo, @expiracao);", afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@id", afr_usuarioId);
                        afr_comando.Parameters.AddWithValue("@codigo", afr_codigo);
                        afr_comando.Parameters.AddWithValue("@expiracao", afr_expiracao);

                        afr_comando.ExecuteNonQuery();
                    }
                }

                afr_mensagem = "Codigo gerado.";
                return true;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao gerar o codigo: " + afr_erro.Message;
                return false;
            }
        }

        public CodigoRecuperacao BuscarCodigo(int afr_usuarioId, string afr_codigo)
        {
            try
            {
                string afr_sql =
                    "SELECT TOP 1 Id, UsuarioId, Codigo, Expiracao, Usado " +
                    "FROM dbo.CodigosRecuperacao " +
                    "WHERE UsuarioId = @id AND Codigo = @codigo " +
                    "ORDER BY Id DESC;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@id", afr_usuarioId);
                        afr_comando.Parameters.AddWithValue("@codigo", afr_codigo);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            if (afr_leitor.Read())
                            {
                                CodigoRecuperacao afr_item = new CodigoRecuperacao();

                                afr_item.afr_Id = Convert.ToInt32(afr_leitor["Id"]);
                                afr_item.afr_UsuarioId = Convert.ToInt32(afr_leitor["UsuarioId"]);
                                afr_item.afr_Codigo = afr_leitor["Codigo"].ToString();
                                afr_item.afr_Expiracao = Convert.ToDateTime(afr_leitor["Expiracao"]);
                                afr_item.afr_Usado = Convert.ToBoolean(afr_leitor["Usado"]);

                                return afr_item;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return null;
        }

        public void MarcarCodigoUsado(int afr_codigoId)
        {
            try
            {
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(
                           "UPDATE dbo.CodigosRecuperacao SET Usado = 1 WHERE Id = @id;",
                           afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@id", afr_codigoId);
                        afr_comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        // =============================================================
        // =======  H I S T O R I C O   D E   A C O E S  (DS12)  =======
        // =============================================================

        public void RegistrarAcao(LogAcao afr_log)
        {
            if (afr_log == null || afr_log.afr_UsuarioId <= 0)
            {
                return;
            }

            try
            {
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(
                           "INSERT INTO dbo.LogAcoes (UsuarioId, Tipo, Descricao) " +
                           "VALUES (@id, @tipo, @descricao);", afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@id", afr_log.afr_UsuarioId);
                        afr_comando.Parameters.AddWithValue("@tipo", afr_log.afr_Tipo);

                        string afr_texto = afr_log.afr_Descricao ?? "";

                        if (afr_texto.Length > 300)
                        {
                            afr_texto = afr_texto.Substring(0, 300);
                        }

                        afr_comando.Parameters.AddWithValue("@descricao", afr_texto);

                        afr_comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // O historico nunca pode derrubar a operacao principal
            }
        }

        // DS12 - historicoAcoes(loginLog, acoesCrud, cursosRelacionados)
        // afr_tipo vazio = todos os tipos
        public List<LogAcao> ListarLogDoUsuario(int afr_usuarioId, string afr_tipo, int afr_limite)
        {
            List<LogAcao> afr_lista = new List<LogAcao>();

            try
            {
                string afr_sql =
                    "SELECT TOP (@limite) Id, UsuarioId, Tipo, Descricao, DataHora " +
                    "FROM dbo.LogAcoes " +
                    "WHERE UsuarioId = @id AND (@tipo = '' OR Tipo = @tipo) " +
                    "ORDER BY DataHora DESC, Id DESC;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@id", afr_usuarioId);
                        afr_comando.Parameters.AddWithValue("@tipo", afr_tipo ?? "");
                        afr_comando.Parameters.AddWithValue("@limite", afr_limite);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            while (afr_leitor.Read())
                            {
                                LogAcao afr_item = new LogAcao();

                                afr_item.afr_Id = Convert.ToInt32(afr_leitor["Id"]);
                                afr_item.afr_UsuarioId = Convert.ToInt32(afr_leitor["UsuarioId"]);
                                afr_item.afr_Tipo = afr_leitor["Tipo"].ToString();
                                afr_item.afr_Descricao = afr_leitor["Descricao"].ToString();
                                afr_item.afr_DataHora = Convert.ToDateTime(afr_leitor["DataHora"]);

                                afr_lista.Add(afr_item);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return afr_lista;
        }


        // =============================================================
        // VERSAO 3.1 - GAMIFICACAO (esquema da API do grupo)
        //
        // A versao 3.0 tinha tabelas proprias (PontosHistorico e uma
        // Conquistas com UsuarioId). Ao ler o codigo da API, no branch
        // main, descobriu-se uma COLISAO: ela tambem cria dbo.Conquistas,
        // com outro significado - la e o CATALOGO de medalhas que
        // existem (Codigo, Nome, Descricao, Icone, XPBonus), nao as que
        // um aluno ganhou. Mesmo nome, estruturas incompativeis: no
        // mesmo banco, quem criasse primeiro quebraria o outro.
        //
        // A solucao foi adotar o esquema DELA por inteiro:
        //
        //   dbo.Gamificacao       XP, sequencia de dias, por usuario
        //   dbo.Conquistas        catalogo das medalhas que existem
        //   dbo.UsuarioConquistas quais medalhas cada aluno ganhou
        //   dbo.HistoricoXP       uma linha por ganho de XP
        //   dbo.Matriculas        em quais cursos o aluno esta inscrito
        //
        // O SQL abaixo e um porte quase literal de awardLessonCompletion
        // (api/src/gamificationService.js). Manter a MESMA regra, no
        // mesmo lugar - o banco - e o que garante que concluir uma aula
        // pelo desktop, pelo site ou pelo celular de o mesmo resultado.
        // =============================================================

        // -------------------------------------------------------------
        // Concede o XP de uma aula concluida e avalia as conquistas.
        //
        // Idempotente: a restricao UQ_HistoricoXP_Evento, na tabela
        // dela, impede a mesma aula de pagar duas vezes. O IF NOT EXISTS
        // com UPDLOCK/HOLDLOCK evita a corrida entre duas chamadas
        // simultaneas, exatamente como no codigo dela.
        // -------------------------------------------------------------
        public GanhoGamificacao ConcederXpPorAula(int afr_usuarioId, int afr_aulaId)
        {
            GanhoGamificacao afr_ganho = new GanhoGamificacao();

            try
            {
                string afr_sql = @"
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @NovasConquistas TABLE (ConquistaId INT);
DECLARE @XPConquistas INT = 0;
DECLARE @XPConcedido INT = 0;

BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.Gamificacao WITH (UPDLOCK, HOLDLOCK)
        WHERE UsuarioId = @usuarioId
    )
    BEGIN
        INSERT INTO dbo.Gamificacao (UsuarioId, XP) VALUES (@usuarioId, 0);
    END;

    IF NOT EXISTS (
        SELECT 1 FROM dbo.HistoricoXP WITH (UPDLOCK, HOLDLOCK)
        WHERE UsuarioId = @usuarioId
          AND ReferenciaTipo = 'AULA'
          AND ReferenciaId = @aulaId
    )
    BEGIN
        INSERT INTO dbo.HistoricoXP
            (UsuarioId, Quantidade, Motivo, ReferenciaTipo, ReferenciaId)
        VALUES
            (@usuarioId, 10, N'Conclusao de aula', 'AULA', @aulaId);

        DECLARE @Hoje DATE = CAST(GETDATE() AS DATE);

        UPDATE dbo.Gamificacao
        SET
            XP = XP + 10,
            SequenciaAtual = CASE
                WHEN UltimaAtividade = @Hoje THEN SequenciaAtual
                WHEN UltimaAtividade = DATEADD(DAY, -1, @Hoje) THEN SequenciaAtual + 1
                ELSE 1
            END,
            MelhorSequencia = CASE
                WHEN UltimaAtividade = @Hoje THEN MelhorSequencia
                WHEN UltimaAtividade = DATEADD(DAY, -1, @Hoje)
                    THEN CASE WHEN SequenciaAtual + 1 > MelhorSequencia
                              THEN SequenciaAtual + 1 ELSE MelhorSequencia END
                WHEN MelhorSequencia < 1 THEN 1
                ELSE MelhorSequencia
            END,
            UltimaAtividade = @Hoje
        WHERE UsuarioId = @usuarioId;

        SET @XPConcedido = 10;
    END;

    DECLARE @AulasConcluidas INT = (
        SELECT COUNT(*) FROM dbo.HistoricoXP
        WHERE UsuarioId = @usuarioId AND ReferenciaTipo = 'AULA'
    );

    DECLARE @CursosConcluidos INT = (
        SELECT COUNT(*)
        FROM dbo.Matriculas m
        WHERE m.UsuarioId = @usuarioId
          AND EXISTS (SELECT 1 FROM dbo.Aulas a WHERE a.CursoId = m.CursoId)
          AND NOT EXISTS (
              SELECT 1 FROM dbo.Aulas a
              WHERE a.CursoId = m.CursoId
                AND NOT EXISTS (
                    SELECT 1 FROM dbo.Progresso p
                    WHERE p.UsuarioId = @usuarioId AND p.AulaId = a.Id
                )
          )
    );

    INSERT INTO dbo.UsuarioConquistas (UsuarioId, ConquistaId)
    OUTPUT INSERTED.ConquistaId INTO @NovasConquistas (ConquistaId)
    SELECT @usuarioId, c.Id
    FROM dbo.Conquistas c
    WHERE NOT EXISTS (
        SELECT 1 FROM dbo.UsuarioConquistas uc
        WHERE uc.UsuarioId = @usuarioId AND uc.ConquistaId = c.Id
    )
    AND (
        (c.Codigo = 'PRIMEIRA_AULA'  AND @AulasConcluidas >= 1)
     OR (c.Codigo = 'CINCO_AULAS'    AND @AulasConcluidas >= 5)
     OR (c.Codigo = 'DEZ_AULAS'      AND @AulasConcluidas >= 10)
     OR (c.Codigo = 'PRIMEIRO_CURSO' AND @CursosConcluidos >= 1)
     OR (c.Codigo = 'SEQUENCIA_7'    AND EXISTS (
            SELECT 1 FROM dbo.Gamificacao
            WHERE UsuarioId = @usuarioId AND SequenciaAtual >= 7
        ))
    );

    SELECT @XPConquistas = ISNULL(SUM(c.XPBonus), 0)
    FROM dbo.Conquistas c
    INNER JOIN @NovasConquistas n ON n.ConquistaId = c.Id;

    IF @XPConquistas > 0
    BEGIN
        UPDATE dbo.Gamificacao SET XP = XP + @XPConquistas
        WHERE UsuarioId = @usuarioId;

        INSERT INTO dbo.HistoricoXP
            (UsuarioId, Quantidade, Motivo, ReferenciaTipo, ReferenciaId)
        SELECT @usuarioId, c.XPBonus, CONCAT(N'Conquista: ', c.Nome), 'CONQUISTA', c.Id
        FROM dbo.Conquistas c
        INNER JOIN @NovasConquistas n ON n.ConquistaId = c.Id;
    END;

    COMMIT TRANSACTION;

    SELECT @XPConcedido AS xpAula, @XPConquistas AS xpConquistas,
           g.XP AS xpTotal, g.SequenciaAtual AS sequenciaAtual,
           g.MelhorSequencia AS melhorSequencia
    FROM dbo.Gamificacao g WHERE g.UsuarioId = @usuarioId;

    SELECT c.Id AS id, c.Codigo AS codigo, c.Nome AS nome,
           c.Descricao AS descricao, c.Icone AS icone, c.XPBonus AS xpBonus
    FROM dbo.Conquistas c
    INNER JOIN @NovasConquistas n ON n.ConquistaId = c.Id;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);
                        afr_comando.Parameters.AddWithValue("@aulaId", afr_aulaId);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            // Primeiro resultado: o resumo
                            if (afr_leitor.Read())
                            {
                                afr_ganho.afr_XpAula = Convert.ToInt32(afr_leitor["xpAula"]);
                                afr_ganho.afr_XpConquistas = Convert.ToInt32(afr_leitor["xpConquistas"]);
                                afr_ganho.afr_XpTotal = Convert.ToInt32(afr_leitor["xpTotal"]);
                                afr_ganho.afr_SequenciaAtual = Convert.ToInt32(afr_leitor["sequenciaAtual"]);
                                afr_ganho.afr_MelhorSequencia = Convert.ToInt32(afr_leitor["melhorSequencia"]);
                            }

                            // Segundo resultado: as medalhas ganhas agora
                            if (afr_leitor.NextResult())
                            {
                                while (afr_leitor.Read())
                                {
                                    afr_ganho.afr_NovasConquistas.Add(LerConquista(afr_leitor));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Falha na gamificacao nao pode derrubar a conclusao da
                // aula: a aula ja foi gravada antes desta chamada.
            }

            return afr_ganho;
        }

        // -------------------------------------------------------------
        // O aluno DESMARCOU a aula: devolve o XP daquela aula.
        //
        // A API dela nao faz isso (ela so chama awardLessonCompletion
        // quando concluida=true e ignora o caso contrario), entao aqui
        // ha uma diferenca consciente: sem devolver, o XP ficaria maior
        // do que as aulas realmente concluidas. O XP das CONQUISTAS nao
        // volta - medalha conquistada nao se perde.
        // -------------------------------------------------------------
        public bool RemoverXpDaAula(int afr_usuarioId, int afr_aulaId)
        {
            try
            {
                string afr_sql = @"
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Quantidade INT;

BEGIN TRANSACTION;

    SELECT @Quantidade = Quantidade FROM dbo.HistoricoXP
    WHERE UsuarioId = @usuarioId AND ReferenciaTipo = 'AULA' AND ReferenciaId = @aulaId;

    IF @Quantidade IS NOT NULL
    BEGIN
        DELETE FROM dbo.HistoricoXP
        WHERE UsuarioId = @usuarioId AND ReferenciaTipo = 'AULA' AND ReferenciaId = @aulaId;

        UPDATE dbo.Gamificacao
        SET XP = CASE WHEN XP - @Quantidade < 0 THEN 0 ELSE XP - @Quantidade END
        WHERE UsuarioId = @usuarioId;
    END;

COMMIT TRANSACTION;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);
                        afr_comando.Parameters.AddWithValue("@aulaId", afr_aulaId);
                        afr_comando.ExecuteNonQuery();
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // -------------------------------------------------------------
        // Matricula o aluno no curso.
        //
        // VERSAO 3.3 - antes isto se chamava GarantirMatricula e era
        // chamado por dentro, ao concluir a primeira aula: a matricula
        // acontecia sem o aluno pedir. Virou uma acao explicita por
        // decisao do grupo em 19/11, igual ao botao do aplicativo e a
        // rota POST /api/cursos/:id/matricula.
        //
        // O IF NOT EXISTS continua aqui, e nao no C#: duas janelas do
        // programa abertas no mesmo curso poderiam chamar isto ao mesmo
        // tempo, e quem decide e o banco.
        //
        // Devolve true quando criou a matricula e false quando ja
        // existia - por isso a tela nao trata false como erro.
        // -------------------------------------------------------------
        public bool Matricular(int afr_usuarioId, int afr_cursoId)
        {
            try
            {
                string afr_sql =
                    "IF NOT EXISTS (SELECT 1 FROM dbo.Matriculas " +
                    "               WHERE UsuarioId = @usuarioId AND CursoId = @cursoId) " +
                    "INSERT INTO dbo.Matriculas (UsuarioId, CursoId, DataMatricula) " +
                    "VALUES (@usuarioId, @cursoId, GETDATE());";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);
                        afr_comando.Parameters.AddWithValue("@cursoId", afr_cursoId);
                        return afr_comando.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // -------------------------------------------------------------
        // O aluno esta matriculado neste curso?
        //
        // VERSAO 3.3 - usado como porteiro da conclusao de aula, no
        // ProgressoService. Em caso de erro devolve false: sem conseguir
        // confirmar a matricula, o certo e nao deixar passar.
        // -------------------------------------------------------------
        public bool EstaMatriculado(int afr_usuarioId, int afr_cursoId)
        {
            try
            {
                string afr_sql =
                    "SELECT 1 FROM dbo.Matriculas " +
                    "WHERE UsuarioId = @usuarioId AND CursoId = @cursoId;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);
                        afr_comando.Parameters.AddWithValue("@cursoId", afr_cursoId);
                        return afr_comando.ExecuteScalar() != null;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        // -------------------------------------------------------------
        // O painel do aluno: XP, sequencia e contagens.
        // -------------------------------------------------------------
        public Pontuacao ObterPontuacao(int afr_usuarioId)
        {
            Pontuacao afr_pontuacao = new Pontuacao();
            afr_pontuacao.afr_UsuarioId = afr_usuarioId;

            try
            {
                string afr_sql = @"
SELECT
    ISNULL(g.XP, 0)              AS xpTotal,
    ISNULL(g.SequenciaAtual, 0)  AS sequenciaAtual,
    ISNULL(g.MelhorSequencia, 0) AS melhorSequencia,
    (SELECT COUNT(*) FROM dbo.HistoricoXP h
      WHERE h.UsuarioId = @usuarioId AND h.ReferenciaTipo = 'AULA') AS aulasConcluidas,
    (SELECT COUNT(*) FROM dbo.Matriculas m
      WHERE m.UsuarioId = @usuarioId
        AND EXISTS (SELECT 1 FROM dbo.Aulas a WHERE a.CursoId = m.CursoId)
        AND NOT EXISTS (
            SELECT 1 FROM dbo.Aulas a
            WHERE a.CursoId = m.CursoId
              AND NOT EXISTS (
                  SELECT 1 FROM dbo.Progresso p
                  WHERE p.UsuarioId = @usuarioId AND p.AulaId = a.Id
              )
        )) AS cursosConcluidos
FROM (SELECT 1 AS x) AS dummy
LEFT JOIN dbo.Gamificacao g ON g.UsuarioId = @usuarioId;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            if (afr_leitor.Read())
                            {
                                afr_pontuacao.afr_XpTotal = Convert.ToInt32(afr_leitor["xpTotal"]);
                                afr_pontuacao.afr_SequenciaAtual = Convert.ToInt32(afr_leitor["sequenciaAtual"]);
                                afr_pontuacao.afr_MelhorSequencia = Convert.ToInt32(afr_leitor["melhorSequencia"]);
                                afr_pontuacao.afr_AulasConcluidas = Convert.ToInt32(afr_leitor["aulasConcluidas"]);
                                afr_pontuacao.afr_CursosConcluidos = Convert.ToInt32(afr_leitor["cursosConcluidos"]);
                            }
                        }
                    }
                }

                afr_pontuacao.afr_Conquistas = ListarConquistas(afr_usuarioId);
            }
            catch (Exception)
            {
            }

            return afr_pontuacao;
        }

        // -------------------------------------------------------------
        // TODAS as medalhas do catalogo, marcando quais o aluno ja tem.
        //
        // A tela mostra tambem as que faltam, apagadas: saber o que da
        // para conquistar e metade da graca.
        // -------------------------------------------------------------
        public List<Conquista> ListarConquistas(int afr_usuarioId)
        {
            List<Conquista> afr_lista = new List<Conquista>();

            try
            {
                string afr_sql = @"
SELECT c.Id AS id, c.Codigo AS codigo, c.Nome AS nome, c.Descricao AS descricao,
       c.Icone AS icone, c.XPBonus AS xpBonus, uc.DataConquista AS dataConquista
FROM dbo.Conquistas c
LEFT JOIN dbo.UsuarioConquistas uc
       ON uc.ConquistaId = c.Id AND uc.UsuarioId = @usuarioId
ORDER BY c.XPBonus, c.Id;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            while (afr_leitor.Read())
                            {
                                Conquista afr_item = LerConquista(afr_leitor);

                                // DataConquista nula = o aluno ainda nao tem
                                if (afr_leitor["dataConquista"] != DBNull.Value)
                                {
                                    afr_item.afr_Conquistada = true;
                                    afr_item.afr_DataConquista =
                                        Convert.ToDateTime(afr_leitor["dataConquista"]);
                                }

                                afr_lista.Add(afr_item);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return afr_lista;
        }

        // -------------------------------------------------------------
        // O extrato: de onde veio cada ponto.
        // -------------------------------------------------------------
        public List<RegistroXP> ListarHistoricoXp(int afr_usuarioId, int afr_limite)
        {
            List<RegistroXP> afr_lista = new List<RegistroXP>();

            try
            {
                string afr_sql =
                    "SELECT TOP (@limite) Id, UsuarioId, Quantidade, Motivo, " +
                    "       ReferenciaTipo, ReferenciaId, DataRegistro " +
                    "FROM dbo.HistoricoXP WHERE UsuarioId = @usuarioId " +
                    "ORDER BY DataRegistro DESC, Id DESC;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@usuarioId", afr_usuarioId);
                        afr_comando.Parameters.AddWithValue("@limite", afr_limite);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            while (afr_leitor.Read())
                            {
                                RegistroXP afr_item = new RegistroXP();

                                afr_item.afr_Id = Convert.ToInt32(afr_leitor["Id"]);
                                afr_item.afr_UsuarioId = Convert.ToInt32(afr_leitor["UsuarioId"]);
                                afr_item.afr_Quantidade = Convert.ToInt32(afr_leitor["Quantidade"]);
                                afr_item.afr_Motivo = afr_leitor["Motivo"].ToString();
                                afr_item.afr_ReferenciaTipo = afr_leitor["ReferenciaTipo"].ToString();
                                afr_item.afr_ReferenciaId = Convert.ToInt32(afr_leitor["ReferenciaId"]);
                                afr_item.afr_DataRegistro = Convert.ToDateTime(afr_leitor["DataRegistro"]);

                                afr_lista.Add(afr_item);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return afr_lista;
        }

        // -------------------------------------------------------------
        // Le uma medalha do catalogo. Usado em dois lugares, por isso
        // esta separado.
        // -------------------------------------------------------------
        private Conquista LerConquista(SqlDataReader afr_leitor)
        {
            Conquista afr_item = new Conquista();

            afr_item.afr_Id = Convert.ToInt32(afr_leitor["id"]);
            afr_item.afr_Codigo = afr_leitor["codigo"].ToString();
            afr_item.afr_Nome = afr_leitor["nome"].ToString();
            afr_item.afr_Descricao = afr_leitor["descricao"].ToString();
            afr_item.afr_Icone = afr_leitor["icone"].ToString();
            afr_item.afr_XpBonus = Convert.ToInt32(afr_leitor["xpBonus"]);

            return afr_item;
        }


        // -------------------------------------------------------------
        // VERSAO 3.2 - a classificacao por XP.
        //
        // Devolve TODOS os usuarios ativos, nao so os de perfil ALUNO:
        // professor e administrador tambem podem cursar, e some-los da
        // lista faria parecer defeito quando o XP deles nao aparecesse.
        // A coluna Perfil deixa claro quem e quem.
        //
        // Quem chama precisa conferir a permissao ANTES: esta consulta
        // nao sabe quem esta pedindo. A regra de "so professor e admin
        // veem" fica na tela e no servico.
        // -------------------------------------------------------------
        public List<LinhaRanking> ListarRanking(int afr_limite)
        {
            List<LinhaRanking> afr_lista = new List<LinhaRanking>();

            try
            {
                // Consulta espelhada na da rota /api/gamificacao/turma,
                // inclusive no criterio de desempate (nome e depois Id):
                // duas ordens diferentes para o mesmo ranking colocariam
                // dois alunos empatados em posicoes trocadas conforme a
                // tela fosse aberta no celular ou aqui.
                string afr_sql = @"
SELECT TOP (@limite)
    u.Id                          AS id,
    u.Nome                        AS nome,
    u.Email                       AS email,
    ISNULL(g.XP, 0)               AS xpTotal,
    ISNULL(g.SequenciaAtual, 0)   AS sequenciaAtual,
    ISNULL(g.MelhorSequencia, 0)  AS melhorSequencia
FROM dbo.Usuarios u
LEFT JOIN dbo.Gamificacao g ON g.UsuarioId = u.Id
WHERE u.Perfil = 'ALUNO' AND u.Ativo = 1
ORDER BY ISNULL(g.XP, 0) DESC, u.Nome ASC, u.Id ASC;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@limite", afr_limite);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            int afr_posicao = 0;

                            while (afr_leitor.Read())
                            {
                                afr_posicao = afr_posicao + 1;

                                LinhaRanking afr_linha = new LinhaRanking();

                                afr_linha.afr_Posicao = afr_posicao;
                                afr_linha.afr_UsuarioId = Convert.ToInt32(afr_leitor["id"]);
                                afr_linha.afr_Nome = afr_leitor["nome"].ToString();
                                afr_linha.afr_Email = afr_leitor["email"].ToString();
                                afr_linha.afr_XpTotal = Convert.ToInt32(afr_leitor["xpTotal"]);
                                afr_linha.afr_SequenciaAtual = Convert.ToInt32(afr_leitor["sequenciaAtual"]);
                                afr_linha.afr_MelhorSequencia = Convert.ToInt32(afr_leitor["melhorSequencia"]);

                                afr_lista.Add(afr_linha);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return afr_lista;
        }

    }
}
