# Plataforma de Cursos Online

> Esta é a pasta do sistema em C#. Para a visão geral do repositório do grupo,
> volte ao [README principal](../README.md).

Sistema desenvolvido a partir da modelagem UML do grupo (`Projeto_UML.asta`): uma plataforma de cursos com **três perfis de acesso** (aluno, professor e administrador), catálogo com filtros de busca, conteúdo de aula, requisito de conclusão por curso, recuperação de senha por código, histórico de ações e exclusão em duas etapas.

**Windows Forms (.NET 8).** O programa não tem banco próprio: todos os dados vêm da **API do grupo** (a mesma do aplicativo e da web), hospedada no Render, que guarda tudo no Azure SQL.

> **Versão 2** — o programa foi revisado mensagem por mensagem contra os 14 diagramas de sequência do Astah. O mapa completo (cada mensagem do modelo → o método que a implementa) está em [`RASTREABILIDADE.md`](RASTREABILIDADE.md).

---

## Como rodar (primeira vez)

### 1. O que você precisa ter instalado

- **Visual Studio 2022** com a carga de trabalho *Desenvolvimento para desktop com .NET*
- **Internet**: os dados vêm da API do grupo

### 2. Baixar o repositório

Pelo Visual Studio: **Git → Clonar Repositório**, cole o endereço do repositório do grupo e escolha uma pasta em Documentos.

### 3. Abrir e executar

1. Dentro da pasta baixada, abra `desktop/PlataformaCursos.sln`
2. Pressione **Ctrl+F5**

A tela de login confere se a API responde. Na primeira vez do dia o servidor gratuito pode levar alguns segundos para acordar: a tela espera antes de avisar.

---

## Entrar no sistema

As contas são criadas pelo administrador, em *Usuários*. Não há contas nem senhas publicadas aqui.

O menu principal tem três áreas e mostra só as que o perfil alcança:

| | Catálogo | Cursos e Aulas | Progresso dos Alunos | Usuários |
|---|---|---|---|---|
| **Aluno** | ✓ | | | |
| **Professor** | ✓ | ✓ | ✓ | |
| **Admin** | ✓ | ✓ | ✓ | ✓ |

Quem decide isso são os métodos `PodeGerenciarCursos`, `PodeVerProgressoDeOutros` e `PodeGerenciarUsuarios`, da classe `Usuario`. As telas consultam esses métodos pela classe `Sessao` — nenhuma tela compara texto de perfil na mão. Além de esconder o botão, cada tela confere a permissão de novo no evento `Load`.

> As senhas são gravadas como texto simples para facilitar a demonstração. Em um sistema real seriam guardadas como *hash*.

---

## Sobre a conexão com a API

O endereço da API fica em `ConfiguracaoApi`, no início de `ClienteApi.cs`: por padrão, `https://pim-iv-api.onrender.com`. Para usar outro (uma cópia da API rodando no próprio computador, por exemplo), defina a variável de ambiente `API_URL` no perfil de inicialização (`Properties/launchSettings.json`).

### Se aparecer erro de conexão

| Mensagem | O que fazer |
|---|---|
| Não foi possível falar com a API | Conferir a internet. Se for a primeira vez do dia, esperar o servidor acordar e tentar de novo |
| A API respondeu, mas o banco não | O firewall do Azure bloqueou o IP. Quem administra o banco libera o IP no portal (servidor SQL → Rede) |

---

## Estrutura do projeto

### Telas

| Arquivo | O que faz |
|---|---|
| `FrmLogin` | Entrada no sistema e link de recuperação de senha |
| `FrmRecuperarSenha` | Recuperação de senha com o código pessoal de recuperação |
| `FrmPrincipal` | Menu com as três áreas de perfil |
| `FrmCatalogo` | Visão do aluno: cursos, busca com filtros, aulas e progresso |
| `FrmAula` | **Nova** — conteúdo da aula e botão de concluir |
| `FrmCursos` | CRUD de cursos e de aulas (professor e admin) |
| `FrmUsuarios` | CRUD de usuários (admin) |
| `FrmProgressoAlunos` | **Nova** — professor acompanha o progresso dos alunos |
| `FrmPerfilUsuario` | **Nova** — perfil do usuário com histórico de ações |
| `FrmConfirmarExclusao` | **Nova** — confirmação de exclusão com senha |

### Classes

**`Modelo.cs`** — as entidades. `Pessoa` é a classe base e tem `ObterResumo()` como `virtual`; `Usuario` herda dela e faz o `override` (é o polimorfismo pedido na disciplina).

| Classe | Papel |
|---|---|
| `Pessoa` | classe base: Id, nome, email |
| `Usuario : Pessoa` | senha, perfil, permissões |
| `Curso` | título, descrição, carga, percentual mínimo de conclusão |
| `Aula` | pertence a um curso; ordem, duração, conteúdo |
| `Progresso` | liga um aluno a uma aula concluída |
| `FiltroCurso` | os "filtros" do DS04 |
| `ResultadoConclusao` | o "resultadoConclusao" do DS08 |
| `ProgressoCurso` | o "dados" do DS07 |
| `LogAcao` | o histórico do DS12 |
| `ElegibilidadeExclusao` | os "dadosDetalhados" do DS10 e DS14 |
| `Sessao` | estática; guarda quem está logado |

**`ClienteApi.cs`** — a conversa HTTP com a API: uma função por rota, com o token do login. **`PonteApi.cs`** — sabe qual rota cada tela precisa e converte a resposta nas classes do `Modelo.cs`.

**`Servicos.cs`** — as regras de tela (validar formulário, conferir perfil): `LogService`, `AutenticacaoService`, `UsuarioService`, `CursoService`, `AulaService`, `ProgressoService` e `GamificacaoService`. O que mexe em dados, e as regras de XP, matrícula e permissão, é da API.

### Banco de dados

O banco fica no **Azure SQL** e é o mesmo do aplicativo e da web. O desktop nunca fala com ele direto: tudo passa pela API.

---

## Rastreabilidade com a modelagem UML

| Caso de uso | Onde está | Método |
|---|---|---|
| DC01 Fazer Login | `FrmLogin` | `ValidarLogin` |
| DC02 Recuperar Senha | `FrmRecuperarSenha` | `PonteApi.RecuperarComCodigo` |
| DC03 Visualizar Cursos | `FrmCatalogo` | `ExibirCursos` |
| DC04 Buscar Cursos | `FrmCatalogo` | `BuscarCurso(FiltroCurso)` |
| DC05 Marcar Aula como Concluída | `FrmAula` | `AbrirAula` → `AtualizarProgresso` |
| DC06 Fazer Logout | `FrmPrincipal` | `EncerrarSessao` |
| DC07 Criar Curso | `FrmCursos` | `CriarCurso` |
| DC08 Visualizar Curso | `FrmCatalogo`, `FrmAula` | `ObterRequisitosCurso` → `VerificarConclusao` |
| DC09 Editar Curso | `FrmCursos` | `GetDadosCurso` → `EditarCurso` |
| DC10 Excluir Curso | `FrmCursos` + `FrmConfirmarExclusao` | `VerificarElegibilidadeExclusao` → `ConfirmarIdentidade` → `RemoverDefinitivamente` |
| DC11 Criar Usuário | `FrmUsuarios` | `VerificarDuplicidade` → `CriarUsuario` |
| DC12 Visualizar Usuário | `FrmPerfilUsuario` | `BuscarDadosUsuario` + `BuscarLoginLog` / `BuscarAcoesCrud` / `BuscarCursosRelacionados` |
| DC13 Editar Usuário | `FrmUsuarios` | `BuscarDadosUsuario` → `EditarUsuario` |
| DC14 Excluir Usuário | `FrmUsuarios` + `FrmConfirmarExclusao` | `VerificarElegibilidadeExclusao` → `ConfirmarIdentidade` → `RemoverDefinitivamente` |

O detalhe **mensagem por mensagem** dos 14 diagramas de sequência está em [`RASTREABILIDADE.md`](RASTREABILIDADE.md) — é o documento para levar na apresentação do grupo.

---

## Como demonstrar as partes novas

**Primeiro acesso.** O administrador cadastra um usuário em *Usuários* com uma senha temporária. Ao entrar com ela, o programa pede uma nova senha e mostra o **código de recuperação**, exibido uma vez só.

**Conteúdo da aula.** Entre com uma conta de **professor**, abra *Gerenciar Cursos e Aulas*, clique em um curso e em uma aula: o conteúdo dela aparece na caixa à direita. As aulas em laranja ainda estão sem conteúdo.

**Abrir a aula como aluno.** Entre com uma conta de **aluno**, abra o catálogo, clique num curso, numa aula, e em **Abrir a aula selecionada**.

**Filtros da busca.** No catálogo, preencha a carga horária mínima ou máxima e clique em *Buscar cursos*. Marque *Somente os cursos que eu ainda não concluí* para esconder os concluídos.

**Exclusão em duas etapas.** Clique num registro e em *Excluir*: abre a tela de confirmação, que lista o que vai ser apagado junto e pede a **sua** senha. Tente excluir a própria conta de admin, ou o último administrador ativo: a tela abre em modo aviso e explica por que não pode.

---

## Trabalhando neste projeto

As regras de convivência no repositório (pull antes de mexer, divisão de arquivos, conflitos em `.Designer.cs`) estão no [README principal](../README.md).

Vale repetir uma: **os arquivos `.Designer.cs` são os mais sensíveis a conflito.** Quando alguém move um controle na tela, o Visual Studio reescreve o arquivo quase inteiro, e o Git não consegue juntar duas versões dessas. Combinem quem cuida de cada tela.
