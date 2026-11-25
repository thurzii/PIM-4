/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Program.cs  (configuracao e ponto de entrada)
   ---------------------------------------------------------------------
   No desktop o Main abria um formulario. Aqui ele monta e sobe um
   servidor web. Este arquivo faz tres coisas (nao ha banco para
   preparar: todos os dados vem da API do grupo):

   1) INJECAO DE DEPENDENCIA
      Registra as classes de servico. Quando um controller pedir um
      CursoService no construtor, o ASP.NET cria e entrega sozinho.
      "Scoped" = uma instancia por requisicao do navegador.

   2) AUTENTICACAO POR COOKIE
      Substitui a classe estatica Sessao do desktop. Cada visitante
      recebe o seu proprio cookie assinado, entao dois usuarios podem
      estar logados ao mesmo tempo sem um atrapalhar o outro.

   3) ROTAS
      Define que endereco leva a qual controller e acao.
   ===================================================================== */

using Microsoft.AspNetCore.Authentication.Cookies;
using PlataformaCursosWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// 1) INJECAO DE DEPENDENCIA
// =====================================================================
builder.Services.AddControllersWithViews();

// A SessaoAtual precisa ler o cookie da requisicao que esta rodando.
// Quem da esse acesso e o IHttpContextAccessor.
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<SessaoAtual>();

builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<AutenticacaoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<CursoService>();
builder.Services.AddScoped<AulaService>();
builder.Services.AddScoped<ProgressoService>();

builder.Services.AddScoped<GamificacaoService>();

// O endereco da API (Api:UrlBase no appsettings.json) e a ponte que
// todos os servicos usam para chegar aos dados
builder.Services.AddScoped<ConfiguracaoApi>();
builder.Services.AddScoped<PonteApi>();

// =====================================================================
// 2) AUTENTICACAO POR COOKIE
// =====================================================================
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(afr_opcoes =>
    {
        // Quem nao esta logado e mandado para a tela de login
        afr_opcoes.LoginPath = "/Conta/Login";
        afr_opcoes.LogoutPath = "/Conta/Logout";

        // Quem esta logado mas nao tem permissao ve esta pagina.
        // E o equivalente ao MessageBox "somente professor ou admin" que o
        // desktop mostrava no evento Load de cada formulario.
        afr_opcoes.AccessDeniedPath = "/Conta/AcessoNegado";

        // A sessao expira depois de 60 minutos sem uso
        afr_opcoes.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        afr_opcoes.SlidingExpiration = true;

        afr_opcoes.Cookie.Name = "PlataformaCursos.Auth";
        afr_opcoes.Cookie.HttpOnly = true;
    });

var app = builder.Build();

// =====================================================================
// PIPELINE DE REQUISICOES
// A ordem importa: autenticar vem antes de autorizar.
// =====================================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Erro");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();   // quem e voce?
app.UseAuthorization();    // voce pode fazer isso?

// =====================================================================
// 3) ROTAS
//    A pagina inicial e o login; depois de entrar, vai para o Home.
// =====================================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Conta}/{action=Login}/{id?}");

// =====================================================================
// 4) ABRIR O NAVEGADOR SOZINHO
// ---------------------------------------------------------------------
// POR QUE ISTO EXISTE
//
// Um programa de desktop abre uma janela. Uma aplicacao web nao: ela
// sobe um SERVIDOR e fica esperando um navegador chegar. Quem costuma
// abrir o navegador e o Visual Studio, pela configuracao
// "launchBrowser" do arquivo Properties/launchSettings.json - mas isso
// depende da versao do Visual Studio e do perfil selecionado, e em
// algumas maquinas simplesmente nao acontece.
//
// Para o trabalho nao depender disso, o proprio programa abre o
// navegador no endereco em que ele subiu. Funciona com F5 e tambem com
// "dotnet run" no terminal, na maquina de qualquer integrante do grupo.
//
// So roda em DESENVOLVIMENTO: num servidor de verdade nao existe
// navegador para abrir. Para desligar, basta trocar para false a chave
// AbrirNavegadorAoIniciar, em appsettings.Development.json.
// =====================================================================
bool afr_abrirNavegador =
    app.Configuration.GetValue<bool?>("AbrirNavegadorAoIniciar") ?? true;

if (app.Environment.IsDevelopment() && afr_abrirNavegador)
{
    // ApplicationStarted dispara depois que o servidor ja esta ouvindo.
    // Antes disso o endereco ainda nao existe.
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        try
        {
            string afr_endereco = app.Urls.FirstOrDefault() ?? "http://localhost:5000";

            // 0.0.0.0 e [::] significam "todas as placas de rede"; o
            // navegador precisa de um nome que ele saiba resolver.
            afr_endereco = afr_endereco.Replace("0.0.0.0", "localhost")
                                       .Replace("[::]", "localhost");

            Console.WriteLine("[WEB] Abrindo o navegador em " + afr_endereco);

            // UseShellExecute = true faz o Windows escolher o navegador
            // padrao, em vez de tentar executar o endereco como programa.
            var afr_abrir = new System.Diagnostics.ProcessStartInfo(afr_endereco);
            afr_abrir.UseShellExecute = true;

            System.Diagnostics.Process.Start(afr_abrir);
        }
        catch (Exception afr_erro)
        {
            // Se nao der, o servidor continua no ar: basta abrir o
            // endereco na mao.
            Console.WriteLine("[WEB] Não deu para abrir o navegador: " + afr_erro.Message);
            Console.WriteLine("[WEB] Abra o endereço acima manualmente.");
        }
    });
}

app.Run();
