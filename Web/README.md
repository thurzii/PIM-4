# Plataforma de Cursos Online — versão web

> Faz parte do [repositório do grupo](../README.md). O mesmo sistema em Windows Forms está em [`../CSharp/`](../CSharp/).

Mesmo sistema do projeto desktop, feito em **ASP.NET Core MVC 8**. Nasceu da mesma modelagem UML do grupo (`Projeto_UML.asta`) e atende os mesmos 14 diagramas de sequência.

**Versão 2** — alinhada mensagem por mensagem com o modelo do Astah, igual ao desktop. O mapa completo (cada mensagem do modelo → o controller, a action e a view que a implementam) está em [`RASTREABILIDADE_WEB.md`](RASTREABILIDADE_WEB.md).

> **O banco de dados é o mesmo do desktop.** Os dois projetos leem e escrevem no `CursosDB`. Se você já rodou a versão desktop, pode entrar na web com os mesmos usuários e ver o mesmo progresso. Quem rodou a versão 1 não precisa apagar nada: o programa acrescenta as colunas e tabelas novas sozinho na primeira execução.

---

## Como rodar

### 1. O que você precisa

- **Visual Studio 2022 ou 2026** com a carga de trabalho *Desenvolvimento web e ASP.NET*
- **Um SQL Server** — o mais simples é o **LocalDB**, que já vem com o Visual Studio

### 2. Abrir e executar

1. Extraia a pasta (não rode de dentro do `.zip`)
2. Dois cliques em `PlataformaCursosWeb.sln`
3. Aguarde o Visual Studio restaurar o pacote `Microsoft.Data.SqlClient`
4. Pressione **F5**

O navegador abre em `http://localhost:5000` já na tela de login. Na primeira execução o programa cria o banco `CursosDB`, as seis tabelas e os dados de exemplo — a janela do console mostra `[BANCO] Banco de dados pronto.`

### Rodar pelo terminal, sem o Visual Studio

```
cd PlataformaCursosWeb\PlataformaCursosWeb
dotnet run
```

Depois abra `http://localhost:5000` no navegador.

### Se o Visual Studio nao abrir o navegador sozinho

Ele sobe o servidor mesmo assim. Olhe a janela preta do console: a linha
`Now listening on: http://localhost:5000` diz o endereco. Basta abrir esse
endereco no navegador.

Para o F5 passar a abrir sozinho, confira o que esta escrito ao lado do botao
verde de play na barra de ferramentas: tem de estar **http** (o nome do perfil
em `Properties/launchSettings.json`). Se estiver outra coisa, clique na setinha
ao lado e escolha `http`.

---

## Entrar no sistema

| Perfil | Email | Senha | O que pode fazer |
|---|---|---|---|
| Administrador | `admin@escola.com` | `admin123` | tudo |
| Tutor | `carla@escola.com` | `tutor123` | cursos, aulas e progresso dos alunos |
| Aluno | `maria@escola.com` | `aluno123` | catálogo e as próprias aulas |
| Aluno | `joao@escola.com` | `aluno123` | catálogo e as próprias aulas |

O menu do topo mostra só as áreas que o perfil alcança:

| | Catálogo | Cursos e Aulas | Progresso | Usuários |
|---|---|---|---|---|
| **Aluno** | ✓ | | | |
| **Tutor** | ✓ | ✓ | ✓ | |
| **Admin** | ✓ | ✓ | ✓ | ✓ |

> Abra **duas janelas do navegador** (uma delas anônima) e entre com dois usuários diferentes ao mesmo tempo. Cada janela mantém o seu próprio login. Isso não funcionaria no desktop — e é o assunto da seção seguinte.

---

## A diferença que mais importa: a sessão

No desktop, `Sessao` é uma **classe estática**: guarda o usuário logado numa variável do programa. Funciona porque o programa de desktop atende uma pessoa.

Um servidor web atende muitas pessoas ao mesmo tempo. Uma variável estática seria compartilhada por todas: o login de um aluno sobrescreveria o do outro, e cada um veria os dados de quem entrou por último. É um erro de concorrência clássico.

Na web, cada visitante carrega a própria identidade num **cookie assinado pelo servidor**, e a sessão virou um serviço por requisição: `Services/SessaoAtual.cs`. Ele tem os mesmos nomes de método da `Sessao` do desktop:

```csharp
// desktop                        // web
Sessao.ObterUsuarioId()           afr_sessao.ObterUsuarioId()
Sessao.PodeGerenciarCursos()      afr_sessao.PodeGerenciarCursos()
Sessao.EhAdministrador()          afr_sessao.EhAdministrador()
```

Por isso as classes de `Servicos.cs` são praticamente idênticas às do desktop. **A regra de negócio não mudou; mudou de onde vem a identidade de quem está pedindo.**

Quem abre a sessão é o `ContaController.Login`, com `SignInAsync`; quem a encerra é o `Logout`, com `SignOutAsync`. O serviço só aprova o acesso e registra no histórico.

---

## As outras diferenças em relação ao desktop

| Desktop | Web | Por quê |
|---|---|---|
| `Sessao` estática | cookie + `SessaoAtual` por requisição | o servidor atende vários usuários |
| `new BancoService()` dentro de cada serviço | injeção de dependência no `Program.cs` | o ASP.NET cria e entrega; uma instância por requisição |
| Validação nos `if` do `Click` | atributos `[Required]`, `[Range]` nos ViewModels | o navegador manda texto solto; precisa validar na entrada |
| Checagem no `Load` de cada formulário | `[Authorize(Roles = ...)]` no controller **mais** a checagem no serviço | duas camadas, como no desktop (esconder o botão + checar) |
| `MessageBox` | faixas de aviso no topo (`TempData`) | numa página não existe caixa de diálogo modal |
| Um `.Designer.cs` por tela | um `_Layout.cshtml` e views que preenchem o miolo | o layout não se repete em cada página |
| "Ver o código" abria o `emails.txt` no Bloco de Notas | página `/Conta/CaixaDeEntrada` | o arquivo fica no **servidor**; o navegador não abre arquivo de lá |
| `ShowDialog()` para confirmar exclusão | página `/Cursos/ConfirmarExclusao` | mesmo fluxo de duas etapas, em duas requisições |

Nenhuma biblioteca externa de front-end: sem Bootstrap, sem jQuery. O CSS é próprio (`wwwroot/css/site.css`), em um arquivo comentado. Motivo: o sistema funciona sem internet, o que importa no dia da apresentação, e todo o visual fica explicável linha a linha.

---

## Estrutura do projeto

```
PlataformaCursosWeb/
├── Program.cs .................. injeção de dependência, cookie, banco, rotas
├── Models/
│   ├── Modelo.cs ............... as 11 classes de domínio (iguais ao desktop)
│   └── ViewModels.cs ........... o que cada página mostra e recebe
├── Services/
│   ├── BancoService.cs ......... a única classe que conhece SQL
│   ├── SessaoAtual.cs .......... a Sessao do desktop, traduzida para a web
│   └── Servicos.cs ............. as 7 classes de negócio
├── Controllers/
│   ├── ControllerPadrao.cs ..... classe base dos controllers logados
│   ├── ContaController.cs ...... DS01, DS02, DS06
│   ├── HomeController.cs ....... o painel (FrmPrincipal)
│   ├── CatalogoController.cs ... DS03, DS04, DS05, DS08
│   ├── CursosController.cs ..... DS09, DS10
│   ├── ProgressoController.cs .. DS07
│   └── UsuariosController.cs ... DS11, DS12, DS13, DS14
├── Views/ ...................... 19 páginas
└── wwwroot/css/site.css ........ folha de estilo própria
```

### As páginas

| Página | Diagramas | Novidade na versão 2 |
|---|---|---|
| `Conta/Login` | DS01 | credencial do tutor |
| `Conta/RecuperarSenha` | DS02 | **três etapas, com código de 6 dígitos** |
| `Conta/CaixaDeEntrada` | DS02 | **nova** — a caixa de entrada simulada |
| `Conta/AcessoNegado` | — | tabela dos três perfis |
| `Home/Index` | — | três áreas, uma por perfil |
| `Catalogo/Index` | DS03, DS04 | **filtros de carga horária e de concluídos** |
| `Catalogo/Detalhes` | DS08 | requisito de conclusão e status |
| `Catalogo/Aula` | DS05, DS08 | **nova** — o conteúdo da aula |
| `Cursos/Index` | — | coluna do percentual mínimo |
| `Cursos/Form` | DS09 | campo do percentual mínimo |
| `Cursos/Aulas` | DS05 | conteúdo da aula, e destaque das que faltam |
| `Cursos/AulaForm` | DS05 | **nova** — o tutor escreve o conteúdo |
| `Progresso/Index` | DS07 | **nova** — o tutor acompanha os alunos |
| `Usuarios/Index` | — | perfil TUTOR e botão de perfil |
| `Usuarios/Form` | DS11, DS13 | três perfis no `<select>` |
| `Usuarios/Perfil` | DS12 | **nova** — histórico em três listas |
| `Shared/ConfirmarExclusao` | DS10, DS14 | **nova** — exclusão em duas etapas |

---

## Como demonstrar as partes novas

**Recuperação de senha (3 etapas).** Na tela de login, clique em *Esqueci minha senha*. Digite `maria@escola.com` → **Enviar código**. Clique em *abrir a caixa de entrada*: abre a página `/Conta/CaixaDeEntrada`, que é o arquivo `dados/emails.txt` do servidor — o trabalho não tem servidor de e-mail. Copie os 6 dígitos → **Conferir código** → escolha a nova senha → **Redefinir senha**. O código vale 15 minutos e serve uma vez só.

**Conteúdo da aula.** Entre como `carla@escola.com` → *Cursos* → clique em *Aulas* de um curso → *Editar* numa aula. Escreva o conteúdo e salve. O botão *Ver como o aluno vê* abre a página da aula em outra aba.

**Abrir a aula como aluno.** Entre como `maria@escola.com` → *Catálogo* → um curso → clique no **título da aula**. A página mostra o conteúdo, o botão de concluir e o andamento do curso.

**Requisito de conclusão.** O curso *Banco de Dados com SQL Server* exige 75% das aulas, não 100% — o status vira `CONCLUÍDO` antes de o aluno fazer tudo. O de *Lógica* exige 100%.

**Filtros da busca.** No catálogo, preencha `Carga de 50 até 0` → *Buscar*: sai só o curso de 60h. Marque *Somente os cursos que eu ainda não concluí* para esconder os concluídos.

**Progresso dos alunos.** Entre como `carla@escola.com` → *Progresso* → *Ver progresso* num aluno. Sai uma linha por curso, com aulas feitas, percentual, mínimo exigido e status.

**Perfil e histórico.** Entre como `admin@escola.com` → *Usuários* → *Perfil* num usuário. As três listas são os acessos, os cadastros e as ações em cursos daquela pessoa — exatamente os três conjuntos que o DS12 nomeia.

**Exclusão em duas etapas.** Em *Usuários* ou em *Cursos*, clique em *Excluir*. Abre a página de confirmação, que lista o que será apagado junto e pede a **sua** senha. Tente excluir a própria conta de admin, ou o último administrador ativo: a página abre em modo aviso e explica por que não pode.

**As duas camadas de permissão.** Entre como `maria@escola.com` e digite `/Usuarios` na barra de endereço. Você cai em *Acesso negado*: o `[Authorize(Roles = "ADMIN")]` barrou a requisição antes de qualquer código rodar.

---

## Sobre a conexão com o banco

**Você não precisa editar nada.** O sistema testa uma lista de servidores e usa o primeiro que responder:

1. `(localdb)\MSSQLLocalDB` — LocalDB, acompanha o Visual Studio
2. `.\SQLEXPRESS` — SQL Server Express
3. `localhost` e `.` — instância padrão local

O servidor em uso aparece no painel inicial, e o botão **Testar conexão** confirma a comunicação.

Se o seu SQL Server tiver outro nome, acrescente-o na lista `afr_SERVIDORES`, no início de `Services/BancoService.cs` — mas **avise o grupo antes**, porque é um arquivo compartilhado.

### Se aparecer erro de banco de dados

A aplicação sobe mesmo assim e a tela de login mostra a mensagem.

| Mensagem | O que fazer |
|---|---|
| Nenhum SQL Server encontrado | Instalar o LocalDB pelo Visual Studio Installer (Componentes individuais → SQL Server Express LocalDB) |
| Erro de rede ou instância | Rodar `sqllocaldb start MSSQLLocalDB` no Prompt |
| Falha ao restaurar pacote | Conferir a internet e restaurar o NuGet |

O arquivo [`../CSharp/Banco_CursosDB.sql`](../CSharp/Banco_CursosDB.sql) traz o script completo do banco, os comandos de migração da versão 1 e consultas de conferência. Ele fica numa cópia só, em `CSharp/`, para as duas versões não saírem do ar uma da outra.

---

## O banco de dados

Banco `CursosDB`, com seis tabelas:

```
Cursos   1 ──── N  Aulas                 (uma aula pertence a um curso)
Usuarios N ──── N  Aulas                 (pela tabela Progresso)
Usuarios 1 ──── N  CodigosRecuperacao    (códigos de recuperação de senha)
Usuarios 1 ──── N  LogAcoes              (histórico de ações)
```

Todas as chaves estrangeiras usam `ON DELETE CASCADE`: apagar o curso apaga as aulas, apagar o usuário apaga o progresso e o histórico dele. É por isso que a página de confirmação mostra antes o que vai sair junto.

Para ver os dados, no Visual Studio: **Exibir → Pesquisador de Objetos do SQL Server** → `CursosDB → Tabelas` → botão direito → **Exibir Dados**.

---

## Segurança

O que está feito, e vale citar na apresentação:

- **Senhas não vão no cookie.** O cookie guarda só Id, nome, e-mail e perfil, e é assinado pelo servidor: alterar o perfil no navegador invalida a assinatura.
- **Todo formulário POST tem token anti-CSRF** (`@Html.AntiForgeryToken()` na página, `[ValidateAntiForgeryToken]` na action). Sem ele, outro site poderia enviar formulários em nome de quem está logado.
- **Todo comando SQL usa parâmetros `@`**, nunca texto colado — é o que impede injeção de SQL.
- **Cookie `HttpOnly`**, então JavaScript da página não consegue ler.
- **Duas camadas de permissão**: o `[Authorize]` no controller e a checagem no serviço.

O que **não** está feito, e é honesto dizer: a senha é gravada como texto simples, para a demonstração ficar clara. Em um sistema real seria guardada como *hash* (BCrypt, PBKDF2).

---

## Trabalhando neste projeto em grupo

A boa notícia em relação ao desktop: **não existe `.Designer.cs` aqui.** Era o arquivo que mais dava conflito no Git, porque o Visual Studio o reescrevia quase inteiro quando alguém movia um controle. As views `.cshtml` são texto escrito à mão, e o Git junta duas edições delas sem drama, desde que sejam em partes diferentes do arquivo.

Mesmo assim, vale combinar quem cuida de qual pasta de `Views/`, e dar `git pull` antes de começar a mexer.
