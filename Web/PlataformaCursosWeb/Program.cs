/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Program.cs  (configuracao e ponto de entrada)
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   No desktop o Main abria um formulario. Aqui ele monta e sobe um
   servidor web. Este arquivo faz quatro coisas:

   1) INJECAO DE DEPENDENCIA
      Registra as classes de servico. Quando um controller pedir um
      CursoService no construtor, o ASP.NET cria e entrega sozinho.
      "Scoped" = uma instancia por requisicao do navegador.

   2) AUTENTICACAO POR COOKIE
      Substitui a classe estatica Sessao do desktop. Cada visitante
      recebe o seu proprio cookie assinado, entao dois usuarios podem
      estar logados ao mesmo tempo sem um atrapalhar o outro.

   3) PREPARACAO DO BANCO
      Cria o banco CursosDB, as seis tabelas e os dados iniciais quando
      a aplicacao sobe - o mesmo que o desktop fazia ao abrir a tela.
      Tambem roda a MIGRACAO: quem ja tinha o banco da versao 1 ganha
      as colunas e tabelas novas sem perder dado nenhum.

   4) ROTAS
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

builder.Services.AddScoped<BancoService>();
builder.Services.AddScoped<SessaoAtual>();

builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<LogService>();
builder.Services.AddScoped<AutenticacaoService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<CursoService>();
builder.Services.AddScoped<AulaService>();
builder.Services.AddScoped<ProgressoService>();

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
        // E o equivalente ao MessageBox "somente tutor ou admin" que o
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
// 3) PREPARACAO DO BANCO DE DADOS
//    Roda uma vez, quando a aplicacao sobe.
// =====================================================================
using (var afr_escopo = app.Services.CreateScope())
{
    var afr_bancoService = afr_escopo.ServiceProvider.GetRequiredService<BancoService>();

    string afr_mensagem;

    if (afr_bancoService.PrepararBanco(out afr_mensagem))
    {
        Console.WriteLine("[BANCO] " + afr_mensagem);
        Console.WriteLine("[BANCO] " + afr_bancoService.ObterDescricao());
    }
    else
    {
        // A aplicacao sobe mesmo assim, e a tela de login mostra o erro
        Console.WriteLine("[BANCO] ERRO: " + afr_mensagem);
    }
}

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
// 4) ROTAS
//    A pagina inicial e o login; depois de entrar, vai para o Home.
// =====================================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Conta}/{action=Login}/{id?}");

// =====================================================================
// 5) ABRIR O NAVEGADOR SOZINHO
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
            Console.WriteLine("[WEB] Nao deu para abrir o navegador: " + afr_erro.Message);
            Console.WriteLine("[WEB] Abra o endereco acima manualmente.");
        }
    });
}

app.Run();
