/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Controllers/ControladorBase.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   O que todo controller da API precisa, escrito uma vez so (mesmo
   raciocinio de heranca do ControllerPadrao da web).

   POR QUE O CORPO E LIDO "NA MAO"
   A API do Gui confere o TIPO de cada campo ("typeof concluida !==
   'boolean'" responde 400). Se o corpo fosse convertido direto numa
   classe C#, um "concluida": "sim" viraria false sem aviso. Lendo como
   JSON cru, da para recusar com a mesma mensagem que ele usa.
   ===================================================================== */

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PlataformaCursosApi.Infra;

namespace PlataformaCursosApi.Controllers
{
    [ApiController]
    public abstract class ControladorBase : ControllerBase
    {
        protected readonly Banco afr_banco;

        protected ControladorBase(Banco afr_bancoInjetado)
        {
            afr_banco = afr_bancoInjetado;
        }

        // Quem fez o pedido (o [Autenticado] guardou aqui)
        protected UsuarioLogado Usuario
        {
            get { return HttpContext.Items["afr_usuario"] as UsuarioLogado; }
        }

        // { message } - o formato de mensagem da API do Gui
        protected IActionResult Mensagem(int afr_codigo, string afr_texto)
        {
            return StatusCode(afr_codigo, new { message = afr_texto });
        }

        protected IActionResult Falha(string afr_texto, Exception afr_erro)
        {
            return StatusCode(500, new { message = afr_texto, detail = afr_erro.Message });
        }

        // Corpo da requisicao como JSON. Corpo vazio ou invalido vira
        // um objeto vazio: cada rota decide o que falta.
        protected async Task<JsonElement> LerCorpo()
        {
            try
            {
                using (JsonDocument afr_doc = await JsonDocument.ParseAsync(Request.Body))
                {
                    if (afr_doc.RootElement.ValueKind == JsonValueKind.Object)
                    {
                        return afr_doc.RootElement.Clone();
                    }
                }
            }
            catch (JsonException)
            {
            }

            return JsonDocument.Parse("{}").RootElement.Clone();
        }

        // Texto do campo, ou null se nao vier como texto
        protected static string Texto(JsonElement afr_corpo, string afr_campo)
        {
            JsonElement afr_valor;

            if (afr_corpo.TryGetProperty(afr_campo, out afr_valor) &&
                afr_valor.ValueKind == JsonValueKind.String)
            {
                return afr_valor.GetString();
            }

            return null;
        }

        // Numero inteiro do campo (aceita "12" tambem, como o Number()
        // do JavaScript), ou null se nao for inteiro
        protected static int? Inteiro(JsonElement afr_corpo, string afr_campo)
        {
            JsonElement afr_valor;

            if (!afr_corpo.TryGetProperty(afr_campo, out afr_valor))
            {
                return null;
            }

            if (afr_valor.ValueKind == JsonValueKind.Number && afr_valor.TryGetInt32(out int afr_numero))
            {
                return afr_numero;
            }

            if (afr_valor.ValueKind == JsonValueKind.String &&
                int.TryParse(afr_valor.GetString(), out int afr_doTexto))
            {
                return afr_doTexto;
            }

            return null;
        }

        // true/false do campo, ou null se nao vier como booleano
        protected static bool? Booleano(JsonElement afr_corpo, string afr_campo)
        {
            JsonElement afr_valor;

            if (afr_corpo.TryGetProperty(afr_campo, out afr_valor) &&
                (afr_valor.ValueKind == JsonValueKind.True || afr_valor.ValueKind == JsonValueKind.False))
            {
                return afr_valor.GetBoolean();
            }

            return null;
        }

        // Identificador da rota: inteiro positivo, ou 0 (invalido)
        protected static int IdValido(string afr_texto)
        {
            int afr_id;

            return int.TryParse(afr_texto, out afr_id) && afr_id > 0 ? afr_id : 0;
        }

        protected static Dictionary<string, object> Parametros(params object[] afr_pares)
        {
            Dictionary<string, object> afr_lista = new Dictionary<string, object>();

            for (int i = 0; i + 1 < afr_pares.Length; i += 2)
            {
                afr_lista[(string)afr_pares[i]] = afr_pares[i + 1];
            }

            return afr_lista;
        }
    }
}
