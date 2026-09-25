# Plataforma de Cursos Online

> Esta é a pasta do sistema em C#. Para a visão geral do repositório do grupo,
> volte ao [README principal](../README.md).

Sistema desenvolvido a partir da modelagem UML do grupo ([`Modelagem/Projeto_UML.asta`](../Modelagem/)): uma plataforma de cursos com dois perfis de acesso, catálogo de cursos, controle de aulas e acompanhamento de progresso do aluno.

**Windows Forms (.NET 8) + SQL Server.** O banco de dados é criado automaticamente na primeira execução — ninguém precisa rodar script nem configurar conexão.

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

Na primeira execução o programa cria o banco `CursosDB`, as quatro tabelas e os dados de exemplo. Não é preciso fazer mais nada.

---

## Entrar no sistema

O sistema já vem com usuários cadastrados:

| Perfil | Email | Senha |
|---|---|---|
| Administrador | `admin@escola.com` | `admin123` |
| Aluno | `maria@escola.com` | `aluno123` |
| Aluno | `joao@escola.com` | `aluno123` |

O menu principal muda conforme o perfil: o administrador vê a área de administração, o aluno vê apenas o catálogo.

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
| `FrmRecuperarSenha` | Redefinição de senha |
| `FrmPrincipal` | Menu; muda conforme o perfil de quem entrou |
| `FrmCatalogo` | Visão do aluno: cursos, busca, aulas e progresso |
| `FrmCursos` | Visão do admin: CRUD de cursos e de aulas |
| `FrmUsuarios` | Visão do admin: CRUD de usuários |

### Classes

**`Modelo.cs`** — as entidades: `Pessoa` (classe base), `Usuario` (herda de `Pessoa`), `Curso`, `Aula`, `Progresso` e `Sessao` (estática, guarda quem está logado).

**`BancoService.cs`** — a única classe que conhece SQL. Detecta o servidor, cria o banco e concentra todos os comandos.

**`Servicos.cs`** — as regras de negócio: `AutenticacaoService`, `UsuarioService`, `CursoService`, `AulaService` e `ProgressoService`.

### Banco de dados

Banco `CursosDB`, com quatro tabelas:

```
Cursos   1 ──── N  Aulas          (uma aula pertence a um curso)
Usuarios N ──── N  Aulas          (pela tabela Progresso)
```

---

## Rastreabilidade com a modelagem UML

| Caso de uso | Onde está | Método |
|---|---|---|
| DC01 Fazer Login | `FrmLogin` | `ValidarLogin` |
| DC02 Recuperar Senha | `FrmRecuperarSenha` | `RedefinirSenha` |
| DC03 Visualizar Cursos | `FrmCatalogo` | `ExibirCursos` |
| DC04 Buscar Cursos | `FrmCatalogo` | `BuscarCurso` |
| DC05 Marcar Aula como Concluída | `FrmCatalogo` | `AtualizarProgresso` |
| DC06 Fazer Logout | `FrmPrincipal` | `EncerrarSessao` |
| DC07 Criar Curso | `FrmCursos` | `CriarCurso` |
| DC08 Visualizar Curso | `FrmCatalogo` | `VerificarConclusao` |
| DC09 Editar Curso | `FrmCursos` | `EditarCurso` |
| DC10 Excluir Curso | `FrmCursos` | `ExcluirCurso` |
| DC11 Criar Usuário | `FrmUsuarios` | `CriarUsuario` |
| DC12 Visualizar Usuários | `FrmUsuarios` | `ListarUsuarios` |
| DC13 Editar Usuário | `FrmUsuarios` | `EditarUsuario` |
| DC14 Excluir Usuário | `FrmUsuarios` | `ExcluirUsuario` |

---

## Trabalhando neste projeto

As regras de convivência no repositório (pull antes de mexer, divisão de arquivos, conflitos em `.Designer.cs`) estão no [README principal](../README.md).

Vale repetir uma: **os arquivos `.Designer.cs` são os mais sensíveis a conflito.** Quando alguém move um controle na tela, o Visual Studio reescreve o arquivo quase inteiro, e o Git não consegue juntar duas versões dessas. Combinem quem cuida de cada tela.
