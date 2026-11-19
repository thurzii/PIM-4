/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Infra/Seguranca.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   O token de login e o porteiro das rotas.

   TOKEN COMPATIVEL COM A API DO GUI
   Mesmo algoritmo (HS256), mesma chave (JWT_SECRET do .env), mesmos
   campos (id, nome, email, perfil) e mesma validade (8 horas). Com
   isso um token emitido por uma API e aceito pela outra.

   O PORTEIRO CONFERE O BANCO A CADA PEDIDO
   Igual ao middleware "auth" da API do Gui: o token so diz QUEM e. O
   perfil e a situacao (ativo/inativo) sao lidos do banco a cada
   requisicao. Assim, um usuario desativado pelo admin perde o acesso
   na hora, sem esperar o token vencer.
   ===================================================================== */

using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace PlataformaCursosApi.Infra
{
    // Quem esta fazendo o pedido, lido do banco pelo porteiro
    public class UsuarioLogado
    {
        public int afr_Id { get; set; }
        public string afr_Nome { get; set; } = "";
        public string afr_Email { get; set; } = "";
        public string afr_Perfil { get; set; } = "";
    }

    public static class Token
    {
        private const int afr_HORAS_VALIDADE = 8;

        private static SymmetricSecurityKey Chave()
        {
            string afr_segredo = ArquivoEnv.Ler("JWT_SECRET", "");

            // A biblioteca exige chave de pelo menos 256 bits. Sem chave
            // no .env, a API nao sobe (ver Program.cs) - nada de chave
            // padrao escrita no codigo, que qualquer um leria no GitHub.
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(afr_segredo));
        }

        public static string Gerar(int afr_id, string afr_nome, string afr_email, string afr_perfil)
        {
            DateTime afr_agora = DateTime.UtcNow;

            JwtHeader afr_cabecalho = new JwtHeader(
                new SigningCredentials(Chave(), SecurityAlgorithms.HmacSha256));

            // O id vai como NUMERO, como no jsonwebtoken do Node. Como
            // texto, a API do Gui ate aceitaria, mas o token ficaria
            // diferente do dela.
            JwtPayload afr_corpo = new JwtPayload();
            afr_corpo.Add("id", afr_id);
            afr_corpo.Add("nome", afr_nome);
            afr_corpo.Add("email", afr_email);
            afr_corpo.Add("perfil", afr_perfil);
            afr_corpo.Add("iat", new DateTimeOffset(afr_agora).ToUnixTimeSeconds());
            afr_corpo.Add("exp", new DateTimeOffset(afr_agora.AddHours(afr_HORAS_VALIDADE)).ToUnixTimeSeconds());

            return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(afr_cabecalho, afr_corpo));
        }

        // Devolve o id de dentro do token, ou 0 se o token nao vale
        public static int Validar(string afr_token)
        {
            try
            {
                TokenValidationParameters afr_regras = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = Chave(),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                JwtSecurityTokenHandler afr_leitor = new JwtSecurityTokenHandler();
                afr_leitor.ValidateToken(afr_token, afr_regras, out SecurityToken afr_validado);

                object afr_id;

                if (((JwtSecurityToken)afr_validado).Payload.TryGetValue("id", out afr_id))
                {
                    return Convert.ToInt32(afr_id);
                }
            }
            catch (Exception)
            {
                // assinatura errada, token vencido ou mal formado
            }

            return 0;
        }
    }

    // -----------------------------------------------------------------
    // [Autenticado] na rota = precisa de token valido.
    // [Autenticado("ALUNO")] = e ainda precisa ser aluno.
    //
    // As mensagens sao as da API do Gui, para o celular, o desktop e a
    // web mostrarem o mesmo texto no mesmo caso.
    // -----------------------------------------------------------------
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AutenticadoAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string[] afr_perfis;

        // Mensagem de "sem permissao" quando a padrao nao serve
        public string Mensagem { get; set; } = "";

        public AutenticadoAttribute(params string[] afr_perfisPermitidos)
        {
            afr_perfis = afr_perfisPermitidos ?? new string[0];
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext afr_contexto,
                                                 ActionExecutionDelegate afr_seguir)
        {
            string afr_cabecalho = afr_contexto.HttpContext.Request.Headers.Authorization.ToString();

            if (!afr_cabecalho.StartsWith("Bearer "))
            {
                afr_contexto.Result = Resposta(401, "Token nao informado.");
                return;
            }

            int afr_id = Token.Validar(afr_cabecalho.Substring(7));

            if (afr_id <= 0)
            {
                afr_contexto.Result = Resposta(401, "Token invalido ou expirado.");
                return;
            }

            Banco afr_banco = afr_contexto.HttpContext.RequestServices.GetRequiredService<Banco>();

            List<Dictionary<string, object>> afr_linhas = afr_banco.Consultar(
                "SELECT TOP 1 Id, Nome, Email, Perfil, Ativo FROM dbo.Usuarios WHERE Id = @id",
                new Dictionary<string, object> { { "@id", afr_id } });

            if (afr_linhas.Count == 0 || !Convert.ToBoolean(afr_linhas[0]["Ativo"]))
            {
                afr_contexto.Result = Resposta(401, "Conta inexistente ou inativa.");
                return;
            }

            UsuarioLogado afr_usuario = new UsuarioLogado
            {
                afr_Id = afr_id,
                afr_Nome = Convert.ToString(afr_linhas[0]["Nome"]),
                afr_Email = Convert.ToString(afr_linhas[0]["Email"]),
                afr_Perfil = Convert.ToString(afr_linhas[0]["Perfil"]).ToUpperInvariant()
            };

            if (afr_perfis.Length > 0 && Array.IndexOf(afr_perfis, afr_usuario.afr_Perfil) < 0)
            {
                afr_contexto.Result = Resposta(403, Mensagem != "" ? Mensagem : MensagemPadrao());
                return;
            }

            afr_contexto.HttpContext.Items["afr_usuario"] = afr_usuario;

            await afr_seguir();
        }

        private string MensagemPadrao()
        {
            if (afr_perfis.Length == 1 && afr_perfis[0] == "ALUNO") return "Acesso restrito a alunos.";
            if (afr_perfis.Length == 1 && afr_perfis[0] == "PROFESSOR") return "Acesso restrito a professores.";
            if (afr_perfis.Length == 1 && afr_perfis[0] == "ADMIN") return "Acesso permitido somente para administradores.";

            return "Voce nao tem permissao para realizar esta operacao.";
        }

        private static ObjectResult Resposta(int afr_codigo, string afr_mensagem)
        {
            return new ObjectResult(new { message = afr_mensagem }) { StatusCode = afr_codigo };
        }
    }
}
