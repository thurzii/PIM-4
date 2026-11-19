/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Infra/ArquivoEnv.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   Le o arquivo .env - o mesmo formato da API do Gui (CHAVE=valor, uma
   por linha, # comeca comentario).

   POR QUE .env E NAO appsettings.json
   O appsettings.json vai para o GitHub junto com o projeto. O .env nao
   vai (esta no .gitignore), e e nele que ficam a senha do banco do
   Azure e a chave dos tokens. Usar os mesmos nomes da API do Gui deixa
   o arquivo que ele mandou funcionar aqui sem nenhuma mudanca.

   Uma variavel de ambiente de verdade tem prioridade sobre o arquivo:
   assim da para trocar um valor na hora de rodar sem editar nada.
   ===================================================================== */

namespace PlataformaCursosApi.Infra
{
    public static class ArquivoEnv
    {
        private static readonly Dictionary<string, string> afr_valores =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public static string CaminhoUsado { get; private set; } = "";

        // Procura o .env na pasta do projeto e na pasta do executavel.
        // O Visual Studio roda o programa de dentro de bin/Debug/...,
        // entao so a pasta atual nao bastaria.
        public static void Carregar()
        {
            string[] afr_candidatos =
            {
                Path.Combine(Directory.GetCurrentDirectory(), ".env"),
                Path.Combine(AppContext.BaseDirectory, ".env")
            };

            foreach (string afr_caminho in afr_candidatos)
            {
                if (!File.Exists(afr_caminho))
                {
                    continue;
                }

                foreach (string afr_linha in File.ReadAllLines(afr_caminho))
                {
                    string afr_texto = afr_linha.Trim();

                    if (afr_texto == "" || afr_texto.StartsWith("#"))
                    {
                        continue;
                    }

                    int afr_igual = afr_texto.IndexOf('=');

                    if (afr_igual <= 0)
                    {
                        continue;
                    }

                    string afr_chave = afr_texto.Substring(0, afr_igual).Trim();
                    string afr_valor = afr_texto.Substring(afr_igual + 1).Trim().Trim('"');

                    afr_valores[afr_chave] = afr_valor;
                }

                CaminhoUsado = afr_caminho;
                return;
            }
        }

        public static string Ler(string afr_chave, string afr_padrao = "")
        {
            string afr_doSistema = Environment.GetEnvironmentVariable(afr_chave);

            if (!string.IsNullOrEmpty(afr_doSistema))
            {
                return afr_doSistema;
            }

            string afr_doArquivo;

            if (afr_valores.TryGetValue(afr_chave, out afr_doArquivo) && afr_doArquivo != "")
            {
                return afr_doArquivo;
            }

            return afr_padrao;
        }

        public static bool LerBool(string afr_chave, bool afr_padrao)
        {
            string afr_texto = Ler(afr_chave, afr_padrao ? "true" : "false");

            return afr_texto.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
