# Plataforma de Cursos Online

> Esta é a pasta do sistema em C#. Para a visão geral do repositório do grupo,
> volte ao [README principal](../README.md).

Sistema desenvolvido a partir da modelagem UML do grupo ([`Modelagem/Projeto_UML.asta`](../Modelagem/)): uma plataforma de cursos com **três perfis de acesso** (aluno, tutor e administrador), catálogo com filtros de busca, conteúdo de aula, requisito de conclusão por curso, recuperação de senha por código, histórico de ações e exclusão em duas etapas.

**Windows Forms (.NET 8) + SQL Server.** O banco de dados é criado automaticamente na primeira execução — ninguém precisa rodar script nem configurar conexão.

> **Versão 2** — o programa foi revisado mensagem por mensagem contra os 14 diagramas de sequência do Astah. O mapa completo (cada mensagem do modelo → o método que a implementa) está em [`RASTREABILIDADE.md`](RASTREABILIDADE.md).
>
> **Quem já rodou a versão 1 não precisa apagar o banco.** Na primeira execução desta versão o programa acrescenta sozinho as colunas e tabelas novas, sem perder dado nenhum (método `MigrarEstrutura`, em `BancoService.cs`).

---

## Como rodar (primeira vez)

### 1. O que você precisa ter instalado

- **Visual Studio 2022** com a carga de trabalho *Desenvolvimento para desktop com .NET*
- **Um SQL Server** — o mais simples é o **LocalDB**, que já vem com o Visual Studio

Para conferir se você tem o LocalDB, abra o Prompt de Comando e digite:

```
sqllocaldb info
```

Se aparecer `MSSQLLocalDB`, está tudo certo. Se o comando não for reconhecido, instale assim:

> Visual Studio Installer → botão **Modificar** → aba **Componentes individuais** → busque por `LocalDB` → marque **SQL Server Express LocalDB** → **Modificar**

### 2. Baixar o repositório

Pelo Visual Studio: **Git → Clonar Repositório**, cole o endereço do repositório do grupo e escolha uma pasta em Documentos.

### 3. Abrir e executar

1. Dentro da pasta baixada, abra `CSharp/PlataformaCursos.sln`
2. Aguarde o Visual Studio restaurar o pacote `Microsoft.Data.SqlClient` (precisa de internet na primeira vez). Se não restaurar sozinho: **Ferramentas → Gerenciador de Pacotes NuGet → Restaurar pacotes NuGet**
3. Pressione **Ctrl+F5**

Na primeira execução o programa cria o banco `CursosDB`, as seis tabelas e os dados de exemplo. Não é preciso fazer mais nada.

---

## Entrar no sistema

O sistema já vem com usuários cadastrados:

| Perfil | Email | Senha | O que pode fazer |
|---|---|---|---|
| Administrador | `admin@escola.com` | `admin123` | tudo |
| Tutor | `carla@escola.com` | `tutor123` | cursos, aulas e progresso dos alunos |
| Aluno | `maria@escola.com` | `aluno123` | catálogo e as próprias aulas |
| Aluno | `joao@escola.com` | `aluno123` | catálogo e as próprias aulas |

O menu principal tem três áreas e mostra só as que o perfil alcança:

| | Catálogo | Cursos e Aulas | Progresso dos Alunos | Usuários |
|---|---|---|---|---|
| **Aluno** | ✓ | | | |
| **Tutor** | ✓ | ✓ | ✓ | |
| **Admin** | ✓ | ✓ | ✓ | ✓ |

Quem decide isso são os métodos `PodeGerenciarCursos`, `PodeVerProgressoDeOutros` e `PodeGerenciarUsuarios`, da classe `Usuario`. As telas consultam esses métodos pela classe `Sessao` — nenhuma tela compara texto de perfil na mão. Além de esconder o botão, cada tela confere a permissão de novo no evento `Load`.

> As senhas são gravadas como texto simples para facilitar a demonstração. Em um sistema real seriam guardadas como *hash*.

---

## Sobre a conexão com o banco

**Você não precisa editar nada.** O sistema testa uma lista de servidores e usa o primeiro que responder:

1. `(localdb)\MSSQLLocalDB` — LocalDB, acompanha o Visual Studio
2. `.\SQLEXPRESS` — SQL Server Express
3. `localhost` e `.` — instância padrão local

O servidor em uso aparece na barra cinza do rodapé das telas, e o botão **Testar conexão** no menu principal confirma a comunicação.

Se o seu SQL Server tiver outro nome, acrescente-o na lista `afr_SERVIDORES`, no início de `BancoService.cs` — mas **avise o grupo antes**, porque é um arquivo compartilhado.

### Se aparecer erro de banco de dados

O programa abre mesmo assim e mostra a mensagem na tela de login. As causas mais comuns:

| Mensagem | O que fazer |
|---|---|
| Nenhum SQL Server encontrado | Instalar o LocalDB (passo 1 acima) |
| Erro de rede ou instância | Rodar `sqllocaldb start MSSQLLocalDB` no Prompt |
| Falha ao restaurar pacote | Conferir a internet e restaurar o NuGet |

---

## Ver os dados no banco

Dentro do Visual Studio: **Exibir → Pesquisador de Objetos do SQL Server** → expanda até `CursosDB → Tabelas` → botão direito em uma tabela → **Exibir Dados**.

O arquivo `Banco_CursosDB.sql` traz o script completo do banco e as consultas usadas pelo programa — útil para a apresentação e caso o professor peça o modelo físico.

---

## Estrutura do projeto

### Telas

| Arquivo | O que faz |
|---|---|
| `FrmLogin` | Entrada no sistema e link de recuperação de senha |
| `FrmRecuperarSenha` | Recuperação em 3 etapas, com código de 6 dígitos |
| `FrmPrincipal` | Menu com as três áreas de perfil |
| `FrmCatalogo` | Visão do aluno: cursos, busca com filtros, aulas e progresso |
| `FrmAula` | **Nova** — conteúdo da aula e botão de concluir |
| `FrmCursos` | CRUD de cursos e de aulas (tutor e admin) |
| `FrmUsuarios` | CRUD de usuários (admin) |
| `FrmProgressoAlunos` | **Nova** — tutor acompanha o progresso dos alunos |
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
| `CodigoRecuperacao` | o código do DS02 |
| `LogAcao` | o histórico do DS12 |
| `ElegibilidadeExclusao` | os "dadosDetalhados" do DS10 e DS14 |
| `Sessao` | estática; guarda quem está logado |

**`BancoService.cs`** — a única classe que conhece SQL. Detecta o servidor, cria o banco, migra o banco da versão 1 e concentra todos os comandos. Todo valor vai por parâmetro `@`, nunca colado no texto do comando.

**`Servicos.cs`** — as regras de negócio: `EmailService`, `LogService`, `AutenticacaoService`, `UsuarioService`, `CursoService`, `AulaService` e `ProgressoService`.

### Banco de dados

Banco `CursosDB`, com seis tabelas:

```
Cursos   1 ──── N  Aulas                 (uma aula pertence a um curso)
Usuarios N ──── N  Aulas                 (pela tabela Progresso)
Usuarios 1 ──── N  CodigosRecuperacao    (códigos de recuperação de senha)
Usuarios 1 ──── N  LogAcoes              (histórico de ações)
```

Todas as chaves estrangeiras usam `ON DELETE CASCADE`: apagar o curso apaga as aulas, apagar o usuário apaga o progresso e o histórico dele. É por isso que a tela de confirmação mostra antes o que vai sair junto.

---

## Rastreabilidade com a modelagem UML

| Caso de uso | Onde está | Método |
|---|---|---|
| DC01 Fazer Login | `FrmLogin` | `ValidarLogin` |
| DC02 Recuperar Senha | `FrmRecuperarSenha` | `SolicitarRecuperacao` → `ConferirCodigo` → `RedefinirSenha` |
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

**Recuperação de senha (3 etapas).** Na tela de login, clique em *Esqueci minha senha*. Digite `maria@escola.com` e clique em **1) Enviar código**. Clique em **Ver o código**: abre o arquivo `dados/emails.txt`, que faz o papel da caixa de entrada (o trabalho não tem servidor de e-mail). Copie os 6 dígitos, clique em **2) Conferir código**, escolha a nova senha e clique em **3) Redefinir senha**. O código vale 15 minutos e serve uma vez só.

**Conteúdo da aula.** Entre como `carla@escola.com`, abra *Gerenciar Cursos e Aulas*, clique em um curso e em uma aula: o conteúdo dela aparece na caixa à direita. As aulas em laranja ainda estão sem conteúdo.

**Abrir a aula como aluno.** Entre como `maria@escola.com`, abra o catálogo, clique num curso, numa aula, e em **Abrir a aula selecionada** (ou dois cliques na aula).

**Requisito de conclusão.** O curso *Banco de Dados com SQL Server* exige 75% das aulas, não 100% — o status vira `CONCLUÍDO` antes de o aluno fazer tudo. O de *Lógica* exige 100%.

**Filtros da busca.** No catálogo, preencha `Carga horária de 50 até 0` e clique em *Buscar cursos*: sai só o curso de 60h. Marque *Somente os cursos que eu ainda não concluí* para esconder os concluídos.

**Exclusão em duas etapas.** Clique num registro e em *Excluir*: abre a tela de confirmação, que lista o que vai ser apagado junto e pede a **sua** senha. Tente excluir a própria conta de admin, ou o último administrador ativo: a tela abre em modo aviso e explica por que não pode.

---

## Trabalhando neste projeto

As regras de convivência no repositório (pull antes de mexer, divisão de arquivos, conflitos em `.Designer.cs`) estão no [README principal](../README.md).

Vale repetir uma: **os arquivos `.Designer.cs` são os mais sensíveis a conflito.** Quando alguém move um controle na tela, o Visual Studio reescreve o arquivo quase inteiro, e o Git não consegue juntar duas versões dessas. Combinem quem cuida de cada tela.
