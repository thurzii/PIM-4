/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Program.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   A API que o desktop e a web usam para falar com o banco do Azure.

   POR QUE UMA API PROPRIA
   O aplicativo do Gui tem a API dele. Combinamos que cada projeto tem a
   sua, mas as DUAS gravam no MESMO banco do Azure. Por isso esta API
   segue as regras e os formatos da dele (mesmas rotas, mesmos nomes de
   campo, mesma regra de XP, mesmo hash de senha): qualquer dado gravado
   por uma e lido certo pela outra.

   O que esta API tem a mais sao rotas que as telas do desktop e da web
   precisam e a dele nao tem (editar e excluir usuario, progresso de um
   aluno, editar aula...). Elas usam SO as tabelas que ja existem no
   banco, para nao mudar nada no esquema que o aplicativo usa. A lista
   esta no README.md desta pasta.

   COMO RODAR
   1) Copie .env.exemplo para .env e preencha (a senha NAO vai para o Git)
   2) F5 no Visual Studio, ou "dotnet run" nesta pasta
   3) Confira em http://localhost:3000/api/health
   ===================================================================== */

using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using PlataformaCursosApi.Infra;

ArquivoEnv.Carregar();

// Sem chave dos tokens a API nao sobe. Uma chave padrao escrita aqui
// estaria no GitHub, e com ela qualquer um fabricaria um token de admin.
string afr_segredo = ArquivoEnv.Ler("JWT_SECRET", "");

if (afr_segredo.Length < 32)
{
    Console.WriteLine("[API] ERRO: falta o JWT_SECRET no arquivo .env (minimo 32 caracteres).");
    Console.WriteLine("[API] Copie o .env.exemplo para .env e preencha. .env procurado em: " +
                      Directory.GetCurrentDirectory());
    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Banco>();

builder.Services.AddControllers().AddJsonOptions(afr_opcoes =>
{
    // Os nomes ja saem certos dos apelidos do SQL ("AS titulo");
    // nao deixar o .NET mexer neles
    afr_opcoes.JsonSerializerOptions.DictionaryKeyPolicy = null;
});

// A API do Gui libera chamadas de qualquer origem (cors()). Aqui igual,
// para uma pagina no navegador tambem conseguir chamar.
builder.Services.AddCors(afr_cors =>
    afr_cors.AddDefaultPolicy(afr_regra => afr_regra.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

string afr_porta = ArquivoEnv.Ler("PORT", "3000");
builder.WebHost.UseUrls("http://localhost:" + afr_porta);

var app = builder.Build();

// Erro nao tratado vira { message } com 500, nunca uma pagina HTML:
// quem chama a API e um programa, nao uma pessoa
app.UseExceptionHandler(afr_erro => afr_erro.Run(async afr_contexto =>
{
    afr_contexto.Response.StatusCode = 500;
    afr_contexto.Response.ContentType = "application/json";
    await afr_contexto.Response.WriteAsync(JsonSerializer.Serialize(new { message = "Erro interno da API." }));
}));

app.UseCors();
app.MapControllers();

// Rota que nao existe: a mesma resposta da API do Gui
app.MapFallback(async afr_contexto =>
{
    afr_contexto.Response.StatusCode = 404;
    await afr_contexto.Response.WriteAsJsonAsync(new { message = "Rota nao encontrada." });
});

Banco afr_banco = app.Services.GetRequiredService<Banco>();

Console.WriteLine("[API] .env lido de: " + (ArquivoEnv.CaminhoUsado == "" ? "(nenhum - usando variaveis do sistema)" : ArquivoEnv.CaminhoUsado));
Console.WriteLine("[API] Banco: " + afr_banco.Descrever());
Console.WriteLine("[API] Rodando em http://localhost:" + afr_porta);

// ---------------------------------------------------------------------
// ABRE O NAVEGADOR NA PAGINA DE SITUACAO
//
// Uma API nao tem tela: ela sobe e fica esperando. Para quem esta
// rodando saber na hora se ela conectou no Azure, o navegador abre
// sozinho em /api/health ({"database":"ok"} = tudo certo). E o mesmo
// recurso da versao web (ver Web/PlataformaCursosWeb/Program.cs).
//
// Para desligar: ABRIR_NAVEGADOR=false no .env.
// ---------------------------------------------------------------------
if (ArquivoEnv.LerBool("ABRIR_NAVEGADOR", true))
{
    // ApplicationStarted dispara quando a API ja esta ouvindo; antes
    // disso o endereco ainda nao responde
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        try
        {
            string afr_pagina = "http://localhost:" + afr_porta + "/api/health";

            // UseShellExecute = true faz o Windows abrir no navegador
            // padrao, em vez de tentar executar o endereco como programa
            var afr_abrir = new System.Diagnostics.ProcessStartInfo(afr_pagina);
            afr_abrir.UseShellExecute = true;

            System.Diagnostics.Process.Start(afr_abrir);
        }
        catch (Exception afr_erro)
        {
            // Sem navegador, a API continua no ar: basta abrir na mao
            Console.WriteLine("[API] Nao deu para abrir o navegador: " + afr_erro.Message);
        }
    });
}

app.Run();
