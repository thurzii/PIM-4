/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Infra/Banco.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   A conexao com o banco do Azure e os tres jeitos de falar com ele.

   CONSULTAR DEVOLVE LINHAS COMO DICIONARIO
   Cada linha vira um dicionario com o NOME DA COLUNA como chave. As
   consultas usam os mesmos apelidos da API do Gui ("c.Titulo AS
   titulo"), entao o JSON que sai daqui tem exatamente os mesmos nomes
   de campo que o dele - o desktop, a web e o celular leem o mesmo
   formato.

   PARAMETROS SEMPRE
   Nenhum valor que vem de fora entra no texto do SQL: tudo vai como
   parametro (@nome). E o que impede SQL injection.
   ===================================================================== */

using Microsoft.Data.SqlClient;

namespace PlataformaCursosApi.Infra
{
    public class Banco
    {
        private readonly string afr_conexao;

        public Banco()
        {
            afr_conexao = MontarConexao();
        }

        // -------------------------------------------------------------
        // Monta a conexao a partir do .env, com os mesmos nomes da API
        // do Gui (DB_SERVER, DB_DATABASE, DB_USER...).
        //
        // No Azure, DB_ENCRYPT=true e obrigatorio: o servidor recusa
        // conexao sem criptografia.
        // -------------------------------------------------------------
        private static string MontarConexao()
        {
            SqlConnectionStringBuilder afr_dados = new SqlConnectionStringBuilder();

            string afr_servidor = ArquivoEnv.Ler("DB_SERVER", "localhost");
            string afr_porta = ArquivoEnv.Ler("DB_PORT", "");
            string afr_instancia = ArquivoEnv.Ler("DB_INSTANCE", "");

            if (afr_instancia != "")
            {
                afr_servidor = afr_servidor + "\\" + afr_instancia;
            }
            else if (afr_porta != "")
            {
                afr_servidor = "tcp:" + afr_servidor + "," + afr_porta;
            }

            afr_dados.DataSource = afr_servidor;
            afr_dados.InitialCatalog = ArquivoEnv.Ler("DB_DATABASE", "CursosDB");
            afr_dados.Encrypt = ArquivoEnv.LerBool("DB_ENCRYPT", false);
            afr_dados.TrustServerCertificate = ArquivoEnv.LerBool("DB_TRUST_SERVER_CERTIFICATE", true);
            afr_dados.ConnectTimeout = 15;

            string afr_usuario = ArquivoEnv.Ler("DB_USER", "");

            if (afr_usuario != "")
            {
                afr_dados.UserID = afr_usuario;
                afr_dados.Password = ArquivoEnv.Ler("DB_PASSWORD", "");
            }
            else
            {
                // Sem usuario no .env: login do Windows, para testar
                // contra um SQL Server local
                afr_dados.IntegratedSecurity = true;
            }

            return afr_dados.ConnectionString;
        }

        // Para o /api/health e para o console: servidor e banco, SEM a senha
        public string Descrever()
        {
            SqlConnectionStringBuilder afr_dados = new SqlConnectionStringBuilder(afr_conexao);

            return afr_dados.DataSource + " / " + afr_dados.InitialCatalog;
        }

        public SqlConnection Abrir()
        {
            SqlConnection afr_cn = new SqlConnection(afr_conexao);
            afr_cn.Open();
            return afr_cn;
        }

        // Cada linha vira um dicionario { nomeDaColuna: valor }.
        // DBNull vira null, para o JSON sair com null.
        public List<Dictionary<string, object>> Consultar(string afr_sql,
                                                         Dictionary<string, object> afr_parametros = null,
                                                         SqlConnection afr_cn = null,
                                                         SqlTransaction afr_tx = null)
        {
            List<Dictionary<string, object>> afr_linhas = new List<Dictionary<string, object>>();

            bool afr_minhaConexao = afr_cn == null;
            SqlConnection afr_conexaoUsada = afr_cn ?? Abrir();

            try
            {
                using (SqlCommand afr_cmd = Comando(afr_sql, afr_parametros, afr_conexaoUsada, afr_tx))
                using (SqlDataReader afr_leitor = afr_cmd.ExecuteReader())
                {
                    while (afr_leitor.Read())
                    {
                        Dictionary<string, object> afr_linha = new Dictionary<string, object>();

                        for (int i = 0; i < afr_leitor.FieldCount; i++)
                        {
                            afr_linha[afr_leitor.GetName(i)] =
                                afr_leitor.IsDBNull(i) ? null : afr_leitor.GetValue(i);
                        }

                        afr_linhas.Add(afr_linha);
                    }
                }
            }
            finally
            {
                if (afr_minhaConexao)
                {
                    afr_conexaoUsada.Dispose();
                }
            }

            return afr_linhas;
        }

        // Para comandos que devolvem MAIS DE UM resultado (a regra de
        // XP devolve o resumo e depois a lista de conquistas novas)
        public List<List<Dictionary<string, object>>> ConsultarVarios(string afr_sql,
                                                                     Dictionary<string, object> afr_parametros)
        {
            List<List<Dictionary<string, object>>> afr_resultados =
                new List<List<Dictionary<string, object>>>();

            using (SqlConnection afr_cn = Abrir())
            using (SqlCommand afr_cmd = Comando(afr_sql, afr_parametros, afr_cn, null))
            using (SqlDataReader afr_leitor = afr_cmd.ExecuteReader())
            {
                do
                {
                    List<Dictionary<string, object>> afr_linhas = new List<Dictionary<string, object>>();

                    while (afr_leitor.Read())
                    {
                        Dictionary<string, object> afr_linha = new Dictionary<string, object>();

                        for (int i = 0; i < afr_leitor.FieldCount; i++)
                        {
                            afr_linha[afr_leitor.GetName(i)] =
                                afr_leitor.IsDBNull(i) ? null : afr_leitor.GetValue(i);
                        }

                        afr_linhas.Add(afr_linha);
                    }

                    // Resultado sem colunas (de um INSERT, por exemplo)
                    // nao conta
                    if (afr_leitor.FieldCount > 0)
                    {
                        afr_resultados.Add(afr_linhas);
                    }
                }
                while (afr_leitor.NextResult());
            }

            return afr_resultados;
        }

        // INSERT, UPDATE, DELETE: devolve quantas linhas foram afetadas
        public int Executar(string afr_sql, Dictionary<string, object> afr_parametros = null,
                            SqlConnection afr_cn = null, SqlTransaction afr_tx = null)
        {
            if (afr_cn != null)
            {
                using (SqlCommand afr_cmd = Comando(afr_sql, afr_parametros, afr_cn, afr_tx))
                {
                    return afr_cmd.ExecuteNonQuery();
                }
            }

            using (SqlConnection afr_nova = Abrir())
            using (SqlCommand afr_cmd = Comando(afr_sql, afr_parametros, afr_nova, null))
            {
                return afr_cmd.ExecuteNonQuery();
            }
        }

        // Um valor so (um COUNT, um Id gerado pelo OUTPUT INSERTED.Id)
        public object Escalar(string afr_sql, Dictionary<string, object> afr_parametros = null)
        {
            using (SqlConnection afr_cn = Abrir())
            using (SqlCommand afr_cmd = Comando(afr_sql, afr_parametros, afr_cn, null))
            {
                object afr_valor = afr_cmd.ExecuteScalar();

                return afr_valor == DBNull.Value ? null : afr_valor;
            }
        }

        private static SqlCommand Comando(string afr_sql, Dictionary<string, object> afr_parametros,
                                          SqlConnection afr_cn, SqlTransaction afr_tx)
        {
            SqlCommand afr_cmd = new SqlCommand(afr_sql, afr_cn, afr_tx);

            if (afr_parametros != null)
            {
                foreach (KeyValuePair<string, object> afr_item in afr_parametros)
                {
                    afr_cmd.Parameters.AddWithValue(afr_item.Key, afr_item.Value ?? DBNull.Value);
                }
            }

            return afr_cmd;
        }
    }
}
