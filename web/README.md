# Plataforma de Cursos Online — versão web

> **Versão atual:** o site não tem banco próprio. Todos os dados vêm da **API do grupo**
> (a mesma do aplicativo e do desktop), hospedada no Render, que guarda tudo no Azure SQL.


> Faz parte do [repositório do grupo](../README.md). O mesmo sistema em Windows Forms está em [`../desktop/`](../desktop/).

Mesmo sistema do projeto desktop, feito em **ASP.NET Core MVC 8**. Nasceu da mesma modelagem UML do grupo (`Projeto_UML.asta`) e atende os mesmos 14 diagramas de sequência.

**Versão 2** — alinhada mensagem por mensagem com o modelo do Astah, igual ao desktop. O mapa completo (cada mensagem do modelo → o controller, a action e a view que a implementam) está em [`RASTREABILIDADE_WEB.md`](RASTREABILIDADE_WEB.md).

> **Os dados são os mesmos do desktop e do aplicativo**, porque os três usam a mesma API. Quem entra no site com uma conta vê o mesmo progresso e o mesmo XP que vê no celular.

---

## Como rodar

### 1. O que você precisa

- **Visual Studio 2022 ou 2026** com a carga de trabalho *Desenvolvimento web e ASP.NET*
- **Internet**: os dados vêm da API do grupo

### 2. Abrir e executar

1. Extraia a pasta (não rode de dentro do `.zip`)
2. Dois cliques em `PlataformaCursosWeb.sln`
3. Pressione **F5**

O navegador abre em `http://localhost:5000` já na tela de login. Na primeira vez do dia, o servidor gratuito da API pode levar alguns segundos para acordar: a tela de login espera antes de avisar.

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

As contas são criadas pelo administrador, em *Usuários*. Não há contas nem senhas publicadas aqui.

O menu do topo mostra só as áreas que o perfil alcança:

| | Catálogo | Cursos e Aulas | Progresso | Usuários |
|---|---|---|---|---|
| **Aluno** | ✓ | | | |
| **Professor** | ✓ | ✓ | ✓ | |
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
| `new PonteApi()` dentro de cada serviço | injeção de dependência no `Program.cs` | o ASP.NET cria e entrega; uma instância por requisição |
| Validação nos `if` do `Click` | atributos `[Required]`, `[Range]` nos ViewModels | o navegador manda texto solto; precisa validar na entrada |
| Checagem no `Load` de cada formulário | `[Authorize(Roles = ...)]` no controller **mais** a checagem no serviço | duas camadas, como no desktop (esconder o botão + checar) |
| `MessageBox` | faixas de aviso no topo (`TempData`) | numa página não existe caixa de diálogo modal |
| Um `.Designer.cs` por tela | um `_Layout.cshtml` e views que preenchem o miolo | o layout não se repete em cada página |
| `ShowDialog()` para confirmar exclusão | página `/Cursos/ConfirmarExclusao` | mesmo fluxo de duas etapas, em duas requisições |

Nenhuma biblioteca externa de front-end: sem Bootstrap, sem jQuery. O CSS é próprio (`wwwroot/css/site.css`), em um arquivo comentado. Motivo: o sistema funciona sem internet, o que importa no dia da apresentação, e todo o visual fica explicável linha a linha.

---

## Estrutura do projeto

```
PlataformaCursosWeb/
├── Program.cs .................. injeção de dependência, cookie, rotas
├── Models/
│   ├── Modelo.cs ............... as 11 classes de domínio (iguais ao desktop)
│   └── ViewModels.cs ........... o que cada página mostra e recebe
├── Services/
│   ├── ClienteApi.cs ........... a conversa HTTP com a API (uma função por rota)
│   ├── PonteApi.cs ............. qual rota cada tela usa; converte a resposta
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
├── Views/ ...................... 20 páginas
└── wwwroot/css/site.css ........ folha de estilo própria
```

### As páginas

| Página | Diagramas | Novidade na versão 2 |
|---|---|---|
| `Conta/Login` | DS01 | link para criar conta de aluno |
| `Conta/Cadastro` | — | **nova** — o aluno cria a própria conta |
| `Conta/PrimeiroAcesso` | DS01 | **nova** — troca da senha temporária |
| `Conta/RecuperarSenha` | DS02 | **redefinir a senha com o código de recuperação** |
| `Conta/AcessoNegado` | — | tabela dos três perfis |
| `Home/Index` | — | três áreas, uma por perfil |
| `Catalogo/Index` | DS03, DS04 | **filtros de carga horária e de concluídos** |
| `Catalogo/Detalhes` | DS08 | requisito de conclusão e status |
| `Catalogo/Aula` | DS05, DS08 | **nova** — o conteúdo da aula |
| `Cursos/Index` | — | coluna do percentual mínimo |
| `Cursos/Form` | DS09 | campo do percentual mínimo |
| `Cursos/Aulas` | DS05 | conteúdo da aula, e destaque das que faltam |
| `Cursos/AulaForm` | DS05 | **nova** — o professor escreve o conteúdo |
| `Progresso/Index` | DS07 | **nova** — o professor acompanha os alunos |
| `Usuarios/Index` | — | perfil PROFESSOR e botão de perfil |
| `Usuarios/Form` | DS11, DS13 | três perfis no `<select>` |
| `Usuarios/Perfil` | DS12 | **nova** — histórico em três listas |
| `Shared/ConfirmarExclusao` | DS10, DS14 | **nova** — exclusão em duas etapas |

---

## Como demonstrar as partes novas

**Primeiro acesso e código de recuperação.** O administrador cadastra um usuário em *Usuários* com uma senha temporária. Ao entrar com ela, o site pede uma nova senha e mostra o **código de recuperação**, exibido uma vez só. Com ele, *Esqueci minha senha* redefine a senha. Na tela de login também há *Criar conta de aluno*.

**Conteúdo da aula.** Entre com uma conta de **professor** → *Cursos* → clique em *Aulas* de um curso → *Editar* numa aula. Escreva o conteúdo e salve. O botão *Ver como o aluno vê* abre a página da aula em outra aba.

**Abrir a aula como aluno.** Entre com uma conta de **aluno** → *Catálogo* → um curso → clique no **título da aula**. A página mostra o conteúdo, o botão de concluir e o andamento do curso.

**Filtros da busca.** No catálogo, preencha a carga horária mínima ou máxima → *Buscar*. Marque *Somente os cursos que eu ainda não concluí* para esconder os concluídos.

**Progresso dos alunos.** Entre com uma conta de **professor** → *Progresso* → *Ver progresso* num aluno. Sai uma linha por curso, com aulas feitas, percentual, mínimo exigido e status.

**Perfil e histórico.** Entre com uma conta de **administrador** → *Usuários* → *Perfil* num usuário. As três listas são os acessos, os cadastros e as ações em cursos daquela pessoa — exatamente os três conjuntos que o DS12 nomeia. A API ainda não guarda o histórico, então as três listas aparecem vazias até ela ganhar essa rota.

**Exclusão em duas etapas.** Em *Usuários* ou em *Cursos*, clique em *Excluir*. Abre a página de confirmação, que lista o que será apagado junto e pede a **sua** senha. Tente excluir a própria conta de admin, ou o último administrador ativo: a página abre em modo aviso e explica por que não pode.

**As duas camadas de permissão.** Entre com uma conta de **aluno** e digite `/Usuarios` na barra de endereço. Você cai em *Acesso negado*: o `[Authorize(Roles = "ADMIN")]` barrou a requisição antes de qualquer código rodar.

---

## Sobre a conexão com a API

O endereço da API fica em `appsettings.json`, na chave `Api:UrlBase`: por padrão, `https://pim-iv-api.onrender.com`. O painel inicial mostra o endereço em uso, e o botão **Testar conexão** confirma a comunicação.

### Se aparecer erro de conexão

A aplicação sobe mesmo assim e a tela de login mostra a mensagem.

| Mensagem | O que fazer |
|---|---|
| Não foi possível falar com a API | Conferir a internet. Se for a primeira vez do dia, esperar o servidor acordar e recarregar |
| A API respondeu, mas o banco não | O firewall do Azure bloqueou o IP. Quem administra o banco libera o IP no portal (servidor SQL → Rede) |

---

## O banco de dados

O banco fica no **Azure SQL** e é o mesmo do aplicativo e do desktop. O site nunca fala com ele direto: tudo passa pela API, que é quem conhece as tabelas e as regras.

---

## Segurança

O que está feito, e vale citar na apresentação:

- **Senhas não vão no cookie.** O cookie guarda só Id, nome, e-mail e perfil, e é assinado pelo servidor: alterar o perfil no navegador invalida a assinatura.
- **Todo formulário POST tem token anti-CSRF** (`@Html.AntiForgeryToken()` na página, `[ValidateAntiForgeryToken]` na action). Sem ele, outro site poderia enviar formulários em nome de quem está logado.
- **O site não monta SQL nenhum.** Tudo passa pela API, que manda os valores por parâmetro, nunca colados no comando, o que impede injeção de SQL.
- **Cookie `HttpOnly`**, então JavaScript da página não consegue ler.
- **Duas camadas de permissão**: o `[Authorize]` no controller e a checagem no serviço.

- **A senha nunca é guardada como texto.** A API grava só o *hash* (scrypt); nem o administrador consegue vê-la depois de salva.

---

## Trabalhando neste projeto em grupo

A boa notícia em relação ao desktop: **não existe `.Designer.cs` aqui.** Era o arquivo que mais dava conflito no Git, porque o Visual Studio o reescrevia quase inteiro quando alguém movia um controle. As views `.cshtml` são texto escrito à mão, e o Git junta duas edições delas sem drama, desde que sejam em partes diferentes do arquivo.

Mesmo assim, vale combinar quem cuida de qual pasta de `Views/`, e dar `git pull` antes de começar a mexer.
