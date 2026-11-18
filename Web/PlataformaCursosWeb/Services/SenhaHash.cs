/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: SenhaHash.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   VERSAO 3.1 - reescrito para falar a MESMA lingua da API do grupo

   O QUE MUDOU DA 3.0 PARA A 3.1
   A versao 3.0 usava PBKDF2. Ao ler o codigo da API (branch main,
   api/src/server.js) ficou claro que ela usa scrypt, e que os dois
   gravam na MESMA coluna dbo.Usuarios.Senha. Com formatos diferentes,
   uma senha criada no desktop nao entraria no aplicativo e vice-versa.
   Este arquivo passou a usar exatamente o formato dela.

   O FORMATO (identico ao de hashPassword no server.js)

       scrypt:<salt>:<hash>

       salt = 16 bytes sorteados, escritos como 32 caracteres hexadecimais
       hash = 64 bytes derivados, escritos como 128 caracteres hexadecimais

   DOIS DETALHES QUE QUEBRARIAM EM SILENCIO

   1. O SALT NAO E O QUE PARECE. No codigo dela:

          const salt = crypto.randomBytes(16).toString('hex');
          const hash = await scrypt(password, salt, 64);

      O salt chega ao scrypt como STRING, e o Node converte string em
      bytes usando utf8. Ou seja: o que alimenta o scrypt sao os 32
      bytes ASCII do TEXTO hexadecimal, e nao os 16 bytes que esse texto
      representa. Usar os 16 bytes decodificados produz um hash
      diferente, valido, e que nunca confere. Conferido rodando a
      funcao dela no Node e comparando byte a byte.

   2. O HEXADECIMAL TEM QUE SER MINUSCULO. A funcao de verificacao dela
      exige /^[a-f0-9]{32}$/ e /^[a-f0-9]{128}$/. O Convert.ToHexString
      do .NET devolve MAIUSCULO. Sem o ToLowerInvariant abaixo, a API
      recusaria todo hash gerado aqui sem nem tentar calcular.

   PARAMETROS: N=16384, r=8, p=1, dkLen=64 - os padroes do Node, que e
   o que ela usa ao nao passar opcoes.

   POR QUE BouncyCastle
   O .NET nao traz scrypt pronto. As alternativas eram escrever o
   algoritmo a mao - criptografia artesanal, cujo erro tipico e gerar um
   hash diferente em silencio - ou usar uma biblioteca testada.
   BouncyCastle e a biblioteca de criptografia mais usada no mundo .NET.

   A versao esta como "2.*" no .csproj de proposito: resolve para a
   ultima 2.x disponivel. A chamada SCrypt.Generate e estavel em toda a
   linha 2.x, e assim o projeto nao quebra por um numero de versao
   errado.

   SENHAS ANTIGAS
   A API dela tambem aceita senha em texto puro para contas antigas
   ("compatibilidade com contas antigas", no server.js). Este arquivo faz
   o mesmo, e ainda avisa quem chama, por afr_precisaAtualizar, que
   aquela senha deve ser regravada. Assim o banco se converte sozinho, um
   usuario por login, dos dois lados.
   ===================================================================== */

using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Generators;

namespace PlataformaCursosWeb.Services
{
    public static class SenhaHash
    {
        // O prefixo que marca o formato, igual ao da API
        private const string PREFIXO = "scrypt:";

        // Parametros do scrypt - os padroes do Node, que a API usa
        private const int CUSTO = 16384;        // N
        private const int BLOCO = 8;            // r
        private const int PARALELISMO = 1;      // p
        private const int TAMANHO_HASH = 64;    // dkLen, em bytes
        private const int TAMANHO_SALT = 16;    // bytes sorteados

        // Quantos caracteres cada parte tem quando virada hexadecimal
        private const int CHARS_SALT = TAMANHO_SALT * 2;   // 32
        private const int CHARS_HASH = TAMANHO_HASH * 2;   // 128

        // -----------------------------------------------------------------
        // Transforma a senha digitada no texto que vai para o banco.
        // Chamar sempre que uma senha for criada ou trocada.
        // -----------------------------------------------------------------
        public static string Gerar(string afr_senha)
        {
            if (afr_senha == null)
            {
                afr_senha = "";
            }

            byte[] afr_saltBytes = RandomNumberGenerator.GetBytes(TAMANHO_SALT);

            // ToLowerInvariant NAO e enfeite: a API recusa hexadecimal
            // maiusculo, e Convert.ToHexString devolve maiusculo.
            string afr_saltHex = Convert.ToHexString(afr_saltBytes).ToLowerInvariant();

            byte[] afr_hash = Calcular(afr_senha, afr_saltHex, TAMANHO_HASH);

            return PREFIXO + afr_saltHex + ":" +
                   Convert.ToHexString(afr_hash).ToLowerInvariant();
        }

        // -----------------------------------------------------------------
        // Confere a senha digitada contra o que esta gravado.
        //
        // afr_precisaAtualizar volta true quando o valor gravado ainda
        // esta no formato antigo (texto puro) e a senha estava certa.
        // Quem chama deve, nesse caso, regravar com Gerar().
        // -----------------------------------------------------------------
        public static bool Conferir(string afr_senhaDigitada,
                                    string afr_valorGravado,
                                    out bool afr_precisaAtualizar)
        {
            afr_precisaAtualizar = false;

            if (afr_senhaDigitada == null)
            {
                afr_senhaDigitada = "";
            }

            if (string.IsNullOrEmpty(afr_valorGravado))
            {
                return false;
            }

            // -------------------------------------------------------------
            // CAMINHO 1 - senha antiga, gravada como texto puro
            // (a API faz o mesmo: "compatibilidade com contas antigas")
            // -------------------------------------------------------------
            if (!EhHash(afr_valorGravado))
            {
                if (afr_valorGravado == afr_senhaDigitada)
                {
                    afr_precisaAtualizar = true;
                    return true;
                }

                return false;
            }

            // -------------------------------------------------------------
            // CAMINHO 2 - senha ja convertida em hash
            // -------------------------------------------------------------
            string[] afr_partes = afr_valorGravado.Split(':');

            // Esperado: scrypt / salt / hash
            if (afr_partes.Length != 3)
            {
                return false;
            }

            string afr_saltHex = afr_partes[1];
            string afr_hashHex = afr_partes[2];

            // Os mesmos tamanhos que a API exige nas expressoes regulares
            // dela. Barrar aqui evita calcular scrypt a toa e, mais
            // importante, fecha o caso do hash vazio: sem esta linha, um
            // hash de tamanho zero produziria um calculo de tamanho zero,
            // e comparar dois vetores vazios da VERDADEIRO - qualquer
            // senha entraria naquela conta.
            if (afr_saltHex.Length != CHARS_SALT || afr_hashHex.Length != CHARS_HASH)
            {
                return false;
            }

            byte[] afr_hashGravado;

            try
            {
                afr_hashGravado = Convert.FromHexString(afr_hashHex);
            }
            catch (FormatException)
            {
                // Valor corrompido no banco: trata como senha errada
                return false;
            }

            byte[] afr_hashCalculado =
                Calcular(afr_senhaDigitada, afr_saltHex, afr_hashGravado.Length);

            // FixedTimeEquals compara sempre no mesmo tempo, nao importa
            // onde esta a primeira diferenca. Comparar byte a byte com ==
            // vazaria, pelo tempo de resposta, o quanto o chute acertou.
            // A API usa crypto.timingSafeEqual, que faz o mesmo.
            return CryptographicOperations.FixedTimeEquals(afr_hashCalculado, afr_hashGravado);
        }

        // -----------------------------------------------------------------
        // Versao curta, para quem nao se importa com a conversao
        // -----------------------------------------------------------------
        public static bool Conferir(string afr_senhaDigitada, string afr_valorGravado)
        {
            bool afr_ignorado;
            return Conferir(afr_senhaDigitada, afr_valorGravado, out afr_ignorado);
        }

        // -----------------------------------------------------------------
        // Diz se o texto gravado ja esta no formato de hash
        // -----------------------------------------------------------------
        public static bool EhHash(string afr_valor)
        {
            if (string.IsNullOrEmpty(afr_valor))
            {
                return false;
            }

            return afr_valor.StartsWith(PREFIXO);
        }

        // -----------------------------------------------------------------
        // O calculo em si.
        //
        // afr_saltHex entra como TEXTO e e convertido em bytes ASCII -
        // ver a explicacao no cabecalho. Trocar isto por
        // Convert.FromHexString(afr_saltHex) compila, roda, devolve um
        // hash de aparencia normal e nunca confere com a API.
        // -----------------------------------------------------------------
        private static byte[] Calcular(string afr_senha, string afr_saltHex, int afr_tamanho)
        {
            byte[] afr_senhaBytes = Encoding.UTF8.GetBytes(afr_senha);
            byte[] afr_saltBytes = Encoding.UTF8.GetBytes(afr_saltHex);

            return SCrypt.Generate(afr_senhaBytes, afr_saltBytes,
                                   CUSTO, BLOCO, PARALELISMO, afr_tamanho);
        }
    }
}
