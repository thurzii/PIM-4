/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Controllers/AuthController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   GET  /api/health        a API esta no ar e fala com o banco?
   POST /api/auth/login    { email, senha } -> { token, user }
   POST /api/auth/recover  501, igual a API do Gui

   Mesmas mensagens e mesmos codigos HTTP da API do Gui.
   ===================================================================== */

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PlataformaCursosApi.Infra;

namespace PlataformaCursosApi.Controllers
{
    [Route("api")]
    public class AuthController : ControladorBase
    {
        public AuthController(Banco afr_b) : base(afr_b) { }

        [HttpGet("health")]
        public IActionResult Saude()
        {
            try
            {
                afr_banco.Escalar("SELECT 1 AS ok");
                return Ok(new { api = "ok", database = "ok" });
            }
            catch (Exception afr_erro)
            {
                return StatusCode(500, new { api = "ok", database = "error", message = afr_erro.Message });
            }
        }

        [HttpPost("auth/login")]
        public async Task<IActionResult> Entrar()
        {
            JsonElement afr_corpo = await LerCorpo();

            string afr_email = Texto(afr_corpo, "email");
            string afr_senha = Texto(afr_corpo, "senha");

            if (afr_email == null || afr_senha == null || afr_email.Trim() == "" || afr_senha == "")
            {
                return Mensagem(400, "Informe email e senha.");
            }

            try
            {
                // Email sempre em minusculas, como a API do Gui grava e procura
                List<Dictionary<string, object>> afr_linhas = afr_banco.Consultar(
                    "SELECT TOP 1 Id, Nome, Email, Senha, Perfil, Ativo FROM dbo.Usuarios WHERE Email = @email",
                    Parametros("@email", afr_email.Trim().ToLowerInvariant()));

                if (afr_linhas.Count == 0)
                {
                    return Mensagem(401, "Email ou senha incorretos.");
                }

                Dictionary<string, object> afr_usuario = afr_linhas[0];
                string afr_gravada = Convert.ToString(afr_usuario["Senha"]);

                // Conferir aceita scrypt (o formato da API do Gui) e a
                // senha antiga em texto puro, como a API dele aceita
                bool afr_eraTexto;

                if (!SenhaHash.Conferir(afr_senha, afr_gravada, out afr_eraTexto))
                {
                    return Mensagem(401, "Email ou senha incorretos.");
                }

                if (!Convert.ToBoolean(afr_usuario["Ativo"]))
                {
                    return Mensagem(403, "Usuario inativo.");
                }

                int afr_id = Convert.ToInt32(afr_usuario["Id"]);

                // Senha que ainda estava em texto: aproveita o login para
                // gravar em scrypt. A API do Gui le scrypt, entao o
                // aplicativo continua entrando com a mesma senha.
                if (afr_eraTexto)
                {
                    afr_banco.Executar("UPDATE dbo.Usuarios SET Senha = @senha WHERE Id = @id",
                                       Parametros("@senha", SenhaHash.Gerar(afr_senha), "@id", afr_id));
                }

                string afr_nome = Convert.ToString(afr_usuario["Nome"]);
                string afr_emailGravado = Convert.ToString(afr_usuario["Email"]);
                string afr_perfil = Convert.ToString(afr_usuario["Perfil"]);

                return Ok(new
                {
                    token = Token.Gerar(afr_id, afr_nome, afr_emailGravado, afr_perfil),
                    user = new { id = afr_id, nome = afr_nome, email = afr_emailGravado, perfil = afr_perfil }
                });
            }
            catch (Exception)
            {
                return Mensagem(500, "Erro ao autenticar.");
            }
        }

        // A recuperacao de senha ainda nao existe no banco compartilhado
        // (nao ha tabela para o codigo). Mesma resposta da API do Gui.
        [HttpPost("auth/recover")]
        public IActionResult Recuperar()
        {
            return Mensagem(501, "Recuperacao automatica indisponivel. Solicite a redefinicao ao administrador.");
        }
    }
}
