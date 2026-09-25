/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: BancoService.cs  (toda a comunicacao com o SQL Server)
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   Esta e a UNICA classe do projeto que conhece SQL. Nenhuma tela e
   nenhuma outra classe escreve comando de banco: todas passam por aqui.

   BANCO....: CursosDB        SERVIDOR: detectado automaticamente
   TABELAS..: Usuarios, Cursos, Aulas, Progresso

   O banco, as tabelas e os dados iniciais sao criados automaticamente
   na primeira execucao, pelo metodo PrepararBanco.

   OBSERVACAO SOBRE SENHAS: neste trabalho a senha e gravada como texto
   simples, para a demonstracao ficar clara. Em um sistema real ela
   seria guardada como hash (BCrypt, PBKDF2), nunca legível na tabela.
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
        //
        // O trabalho e feito em grupo, e cada integrante pode ter um SQL
        // Server diferente instalado. Por isso o sistema NAO fixa um
        // servidor: ele testa a lista abaixo, em ordem, e usa o primeiro
        // que responder. Assim ninguem precisa editar o codigo, e o
        // arquivo nao entra em conflito no controle de versao.
        //
        // Se o servidor de alguem nao estiver na lista, basta acrescentar
        // o nome dele aqui.
        // -------------------------------------------------------------
        private static readonly string[] afr_SERVIDORES = new string[]
        {
            @"(localdb)\MSSQLLocalDB",   // LocalDB, acompanha o Visual Studio
            @".\SQLEXPRESS",             // SQL Server Express
            @"localhost",                // instancia padrao local
            @"."                         // idem, forma abreviada
        };

        private const string afr_BANCO = "CursosDB";

        // Guarda o servidor que respondeu. E "static" para a deteccao
        // acontecer UMA vez so, e valer para o programa inteiro.
        private static string afr_servidorAtual = "";

        // Devolve o servidor em uso (ou o primeiro da lista, antes do teste)
        private string ObterServidor()
        {
            if (afr_servidorAtual == "")
            {
                return afr_SERVIDORES[0];
            }

            return afr_servidorAtual;
        }

        // Monta a string de conexao para um servidor e um banco
        private string MontarConexao(string afr_servidor, string afr_nomeBanco)
        {
            return "Server=" + afr_servidor + ";" +
                   "Database=" + afr_nomeBanco + ";" +
                   "Integrated Security=True;" +
                   "TrustServerCertificate=True;" +
                   "Connect Timeout=5;";
        }

        // Monta a conexao usando o servidor ja detectado
        private string ObterConexao(string afr_nomeBanco)
        {
            return MontarConexao(ObterServidor(), afr_nomeBanco);
        }

        public string ObterConexaoPadrao()
        {
            return ObterConexao(afr_BANCO);
        }

        // Texto para a barra de status das telas
        public string ObterDescricao()
        {
            return "Servidor: " + ObterServidor() + "  |  Banco: " + afr_BANCO;
        }

        // -------------------------------------------------------------
        // DETECTA O SERVIDOR: tenta abrir o banco "master" em cada
        // servidor da lista e fica com o primeiro que aceitar a conexao.
        // -------------------------------------------------------------
        public bool DetectarServidor(out string afr_mensagem)
        {
            // Ja detectado nesta execucao: nao testa de novo
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

                        // Funcionou: guarda e para de procurar
                        afr_servidorAtual = afr_servidor;

                        afr_mensagem = "Servidor encontrado: " + afr_servidor;
                        return true;
                    }
                }
                catch (Exception)
                {
                    // Este servidor nao respondeu: anota e tenta o proximo
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
        // CRIACAO DO BANCO, DAS TABELAS E DOS DADOS INICIAIS
        // =============================================================
        public bool PrepararBanco(out string afr_mensagem)
        {
            afr_mensagem = "";

            // 0) descobre qual SQL Server existe neste computador
            if (!DetectarServidor(out afr_mensagem))
            {
                return false;
            }

            try
            {
                // 1) conecta no "master" para criar o banco CursosDB
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexao("master")))
                {
                    afr_conexao.Open();

                    string afr_sqlBanco = "IF DB_ID('" + afr_BANCO + "') IS NULL " +
                                          "CREATE DATABASE " + afr_BANCO + ";";

                    using (SqlCommand afr_comando = new SqlCommand(afr_sqlBanco, afr_conexao))
                    {
                        afr_comando.ExecuteNonQuery();
                    }
                }

                // 2) cria as quatro tabelas, se ainda nao existirem
                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

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
                        "  Ativo BIT NOT NULL DEFAULT 1," +
                        "  DataCadastro DATETIME NOT NULL DEFAULT GETDATE());");

                    // A aula pertence a um curso. ON DELETE CASCADE faz o
                    // banco apagar as aulas junto quando o curso e excluido.
                    ExecutarComando(afr_conexao,
                        "IF OBJECT_ID('dbo.Aulas','U') IS NULL " +
                        "CREATE TABLE dbo.Aulas (" +
                        "  Id INT IDENTITY(1,1) PRIMARY KEY," +
                        "  CursoId INT NOT NULL," +
                        "  Titulo VARCHAR(120) NOT NULL," +
                        "  Ordem INT NOT NULL DEFAULT 1," +
                        "  DuracaoMin INT NOT NULL DEFAULT 0," +
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

                    // 3) dados iniciais, so na primeira vez
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

        // Atalho para rodar um comando que nao devolve dados
        private void ExecutarComando(SqlConnection afr_conexao, string afr_sql)
        {
            using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
            {
                afr_comando.ExecuteNonQuery();
            }
        }

        // -------------------------------------------------------------
        // DADOS INICIAIS - dois usuarios e dois cursos de exemplo
        // -------------------------------------------------------------
        private void CriarDadosIniciais(SqlConnection afr_conexao)
        {
            // Se ja existe algum usuario, nao faz nada
            using (SqlCommand afr_conta = new SqlCommand("SELECT COUNT(*) FROM dbo.Usuarios;", afr_conexao))
            {
                if (Convert.ToInt32(afr_conta.ExecuteScalar()) > 0)
                {
                    return;
                }
            }

            ExecutarComando(afr_conexao,
                "INSERT INTO dbo.Usuarios (Nome, Email, Senha, Perfil) VALUES " +
                "('Administrador do Sistema','admin@escola.com','admin123','ADMIN')," +
                "('Maria Souza','maria@escola.com','aluno123','ALUNO')," +
                "('Joao Lima','joao@escola.com','aluno123','ALUNO');");

            ExecutarComando(afr_conexao,
                "INSERT INTO dbo.Cursos (Titulo, Descricao, CargaHoraria) VALUES " +
                "('Logica de Programacao','Introducao a algoritmos, variaveis e estruturas de controle.',40)," +
                "('Banco de Dados com SQL Server','Modelagem relacional, comandos SQL e consultas.',60);");

            // Aulas do curso 1
            ExecutarComando(afr_conexao,
                "INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin) " +
                "SELECT Id, 'O que e um algoritmo', 1, 45 FROM dbo.Cursos WHERE Titulo = 'Logica de Programacao';");

            ExecutarComando(afr_conexao,
                "INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin) " +
                "SELECT Id, 'Variaveis e tipos de dados', 2, 50 FROM dbo.Cursos WHERE Titulo = 'Logica de Programacao';");

            ExecutarComando(afr_conexao,
                "INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin) " +
                "SELECT Id, 'Condicoes e repeticoes', 3, 60 FROM dbo.Cursos WHERE Titulo = 'Logica de Programacao';");

            // Aulas do curso 2
            ExecutarComando(afr_conexao,
                "INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin) " +
                "SELECT Id, 'Modelo relacional', 1, 50 FROM dbo.Cursos WHERE Titulo = 'Banco de Dados com SQL Server';");

            ExecutarComando(afr_conexao,
                "INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin) " +
                "SELECT Id, 'SELECT, INSERT, UPDATE e DELETE', 2, 70 FROM dbo.Cursos WHERE Titulo = 'Banco de Dados com SQL Server';");
        }

        // =============================================================
        // TESTE DE CONEXAO
        // =============================================================
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

        // SELECT - lista usuarios, filtrando por nome ou email
        public List<Usuario> ListarUsuarios(string afr_termo)
        {
            List<Usuario> afr_lista = new List<Usuario>();

            try
            {
                string afr_sql = "SELECT Id, Nome, Email, Senha, Perfil, Ativo " +
                                 "FROM dbo.Usuarios " +
                                 "WHERE (@termo = '' OR Nome LIKE '%' + @termo + '%' " +
                                 "                  OR Email LIKE '%' + @termo + '%') " +
                                 "ORDER BY Nome;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@termo", afr_termo ?? "");

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
                // Banco fora do ar: devolve lista vazia
            }

            return afr_lista;
        }

        // Transforma a linha lida em um OBJETO Usuario
        private Usuario LerUsuario(SqlDataReader afr_leitor)
        {
            Usuario afr_usuario = new Usuario();

            afr_usuario.afr_Id = Convert.ToInt32(afr_leitor["Id"]);
            afr_usuario.afr_Nome = afr_leitor["Nome"].ToString();
            afr_usuario.afr_Email = afr_leitor["Email"].ToString();
            afr_usuario.afr_Senha = afr_leitor["Senha"].ToString();
            afr_usuario.afr_Perfil = afr_leitor["Perfil"].ToString();
            afr_usuario.afr_Ativo = Convert.ToBoolean(afr_leitor["Ativo"]);

            return afr_usuario;
        }

        // SELECT - procura um usuario pelo email (usado no login)
        public Usuario BuscarUsuarioPorEmail(string afr_email)
        {
            try
            {
                string afr_sql = "SELECT Id, Nome, Email, Senha, Perfil, Ativo " +
                                 "FROM dbo.Usuarios WHERE Email = @email;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@email", afr_email);

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

        // INSERT - grava um usuario novo
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
                // 2627 e 2601 = violacao de chave unica (email repetido)
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

        // UPDATE - altera um usuario existente (WHERE Id e obrigatorio!)
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

        // UPDATE - troca apenas a senha (caso de uso Recuperar Senha)
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

        // DELETE - apaga um usuario pelo Id
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

        // =============================================================
        // ==================        C U R S O S       =================
        // =============================================================

        // SELECT - lista cursos. O termo filtra por titulo ou descricao;
        // afr_somenteAtivos = true e o que o aluno enxerga.
        public List<Curso> ListarCursos(string afr_termo, bool afr_somenteAtivos)
        {
            List<Curso> afr_lista = new List<Curso>();

            try
            {
                string afr_sql =
                    "SELECT c.Id, c.Titulo, c.Descricao, c.CargaHoraria, c.Ativo, " +
                    "       (SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId = c.Id) AS TotalAulas " +
                    "FROM dbo.Cursos c " +
                    "WHERE (@termo = '' OR c.Titulo LIKE '%' + @termo + '%' " +
                    "                   OR c.Descricao LIKE '%' + @termo + '%') " +
                    "  AND (@somenteAtivos = 0 OR c.Ativo = 1) " +
                    "ORDER BY c.Titulo;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@termo", afr_termo ?? "");
                        afr_comando.Parameters.AddWithValue("@somenteAtivos", afr_somenteAtivos ? 1 : 0);

                        using (SqlDataReader afr_leitor = afr_comando.ExecuteReader())
                        {
                            while (afr_leitor.Read())
                            {
                                Curso afr_curso = new Curso();

                                afr_curso.afr_Id = Convert.ToInt32(afr_leitor["Id"]);
                                afr_curso.afr_Titulo = afr_leitor["Titulo"].ToString();
                                afr_curso.afr_Descricao = afr_leitor["Descricao"].ToString();
                                afr_curso.afr_CargaHoraria = Convert.ToInt32(afr_leitor["CargaHoraria"]);
                                afr_curso.afr_Ativo = Convert.ToBoolean(afr_leitor["Ativo"]);
                                afr_curso.afr_TotalAulas = Convert.ToInt32(afr_leitor["TotalAulas"]);

                                afr_lista.Add(afr_curso);
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

        // INSERT - grava um curso novo
        public bool InserirCurso(Curso afr_curso, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "INSERT INTO dbo.Cursos (Titulo, Descricao, CargaHoraria, Ativo) " +
                                 "VALUES (@titulo, @descricao, @carga, @ativo);";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@titulo", afr_curso.afr_Titulo);
                        afr_comando.Parameters.AddWithValue("@descricao", afr_curso.afr_Descricao);
                        afr_comando.Parameters.AddWithValue("@carga", afr_curso.afr_CargaHoraria);
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

        // UPDATE - altera um curso existente
        public bool AtualizarCurso(Curso afr_curso, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "UPDATE dbo.Cursos SET " +
                                 "  Titulo = @titulo, Descricao = @descricao," +
                                 "  CargaHoraria = @carga, Ativo = @ativo " +
                                 "WHERE Id = @id;";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@titulo", afr_curso.afr_Titulo);
                        afr_comando.Parameters.AddWithValue("@descricao", afr_curso.afr_Descricao);
                        afr_comando.Parameters.AddWithValue("@carga", afr_curso.afr_CargaHoraria);
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

        // DELETE - apaga o curso (as aulas saem junto, pelo ON DELETE CASCADE)
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

        // =============================================================
        // ===================        A U L A S       ==================
        // =============================================================

        // SELECT - aulas de um curso. Quando afr_usuarioId > 0, tambem
        // traz se AQUELE aluno ja concluiu cada aula (LEFT JOIN).
        public List<Aula> ListarAulasDoCurso(int afr_cursoId, int afr_usuarioId)
        {
            List<Aula> afr_lista = new List<Aula>();

            try
            {
                string afr_sql =
                    "SELECT a.Id, a.CursoId, a.Titulo, a.Ordem, a.DuracaoMin, " +
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
                                Aula afr_aula = new Aula();

                                afr_aula.afr_Id = Convert.ToInt32(afr_leitor["Id"]);
                                afr_aula.afr_CursoId = Convert.ToInt32(afr_leitor["CursoId"]);
                                afr_aula.afr_Titulo = afr_leitor["Titulo"].ToString();
                                afr_aula.afr_Ordem = Convert.ToInt32(afr_leitor["Ordem"]);
                                afr_aula.afr_DuracaoMin = Convert.ToInt32(afr_leitor["DuracaoMin"]);
                                afr_aula.afr_Concluida = Convert.ToInt32(afr_leitor["Concluida"]) == 1;

                                afr_lista.Add(afr_aula);
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

        // INSERT - grava uma aula nova
        public bool InserirAula(Aula afr_aula, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin) " +
                                 "VALUES (@cursoId, @titulo, @ordem, @duracao);";

                using (SqlConnection afr_conexao = new SqlConnection(ObterConexaoPadrao()))
                {
                    afr_conexao.Open();

                    using (SqlCommand afr_comando = new SqlCommand(afr_sql, afr_conexao))
                    {
                        afr_comando.Parameters.AddWithValue("@cursoId", afr_aula.afr_CursoId);
                        afr_comando.Parameters.AddWithValue("@titulo", afr_aula.afr_Titulo);
                        afr_comando.Parameters.AddWithValue("@ordem", afr_aula.afr_Ordem);
                        afr_comando.Parameters.AddWithValue("@duracao", afr_aula.afr_DuracaoMin);

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

        // DELETE - apaga uma aula
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

        // INSERT - marca a aula como concluida para o aluno
        public bool MarcarAulaConcluida(int afr_usuarioId, int afr_aulaId, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                // O IF NOT EXISTS evita gravar duas vezes a mesma aula
                string afr_sql =
                    "IF NOT EXISTS (SELECT 1 FROM dbo.Progresso " +
                    "               WHERE UsuarioId = @usuarioId AND AulaId = @aulaId) " +
                    "INSERT INTO dbo.Progresso (UsuarioId, AulaId) VALUES (@usuarioId, @aulaId);";

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

                afr_mensagem = "Aula marcada como concluida.";
                return true;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao marcar a aula: " + afr_erro.Message;
                return false;
            }
        }

        // DELETE - desfaz a marcacao
        public bool DesmarcarAulaConcluida(int afr_usuarioId, int afr_aulaId, out string afr_mensagem)
        {
            afr_mensagem = "";

            try
            {
                string afr_sql = "DELETE FROM dbo.Progresso " +
                                 "WHERE UsuarioId = @usuarioId AND AulaId = @aulaId;";

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

                afr_mensagem = "Marcacao desfeita.";
                return true;
            }
            catch (Exception afr_erro)
            {
                afr_mensagem = "Erro ao desmarcar a aula: " + afr_erro.Message;
                return false;
            }
        }

        // SELECT COUNT - quantas aulas do curso o aluno ja concluiu
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
    }
}
