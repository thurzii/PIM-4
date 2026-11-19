# Plataforma de Cursos — PIM IV

Repositório central do grupo para o **Projeto Integrado Multidisciplinar (PIM IV)**. O mesmo sistema de cursos online foi construído em três interfaces — desktop, web e mobile — sobre um único banco de dados, com acesso às funcionalidades de acordo com o perfil de cada usuário.

## Organização do repositório

Este repositório tem **dois branches**, com conteúdos diferentes. Eles não compartilham histórico: cada um nasceu separadamente.

| Branch | Conteúdo | Pastas |
|---|---|---|
| **`master`** | Sistema desktop e web, em C# | `CSharp/`, `Web/`, `Documentacao/`, `Modelagem/` |
| **`main`** | Aplicativo mobile, API e scripts do banco | `api/`, `mobile/`, `sql/` |

No GitHub, troque de branch no seletor acima da lista de arquivos. Para ver os dois lados ao mesmo tempo no Visual Studio, use **Git → Gerenciar Branches**.

## Tecnologias

- **Desktop:** C# e Windows Forms (.NET 8).
- **Web:** C# e ASP.NET Core MVC (.NET 8), com autenticação por cookie.
- **Mobile:** React Native e Expo (JavaScript).
- **API:** Node.js e Express.
- **Banco de dados:** Microsoft SQL Server, com suporte à conexão ao Azure SQL.
- **Autenticação:** senhas armazenadas com `scrypt`, no mesmo formato nas três interfaces.

## Funcionalidades

| Perfil | Funcionalidades principais |
|---|---|
| **ADMIN** | Cadastrar e gerenciar usuários (alunos, professores e administradores) e gerenciar cursos. |
| **PROFESSOR** | Gerenciar cursos e aulas, acompanhar o progresso dos alunos e consultar a classificação por XP. |
| **ALUNO** | Consultar o catálogo, acessar cursos, concluir aulas e visualizar o próprio XP e conquistas. |

**O cadastro de usuários é exclusivo do administrador.** Não existe cadastro público em nenhuma das interfaces.

### Progresso e gamificação

A conclusão de aulas concede **10 XP por nova aula**, mais bônus por conquistas. O nível é calculado a partir do XP (`XP / 100 + 1`) e não é armazenado, para nunca divergir dos pontos.

A restrição `UNIQUE (UsuarioId, ReferenciaTipo, ReferenciaId)` da tabela `HistoricoXP` impede que a mesma aula pague XP duas vezes. Como a regra está no banco, vale igual para as três interfaces, sem que nenhuma precise se lembrar de verificar.

| Conquista | Critério | Bônus |
|---|---|---|
| **Primeiros passos** | primeira aula concluída | 20 XP |
| **Ritmo de estudos** | cinco aulas | 30 XP |
| **Estudante dedicado** | dez aulas | 50 XP |
| **Uma semana de foco** | sete dias consecutivos | 75 XP |
| **Curso concluído** | todas as aulas de um curso | 100 XP |

Ao desmarcar uma aula, o desktop e a web estornam o XP daquela aula, para o total continuar batendo com o que foi de fato concluído. As conquistas já obtidas permanecem.

## Arquitetura

```text
   Windows Forms        ASP.NET Core MVC        React Native (Expo)
         |                      |                        |
         |                      |                        | HTTP / JWT
         |                      |                        v
         |                      |                 API Node.js / Express
         |                      |                        |
         +----------------------+------------------------+
                                |
                                v
                      SQL Server / Azure SQL
```

Hoje o desktop e a web acessam o banco diretamente; o aplicativo acessa pela API. O cliente HTTP do lado C# (`ClienteApi.cs`) já está escrito e **desligado** — quando o grupo decidir virar a chave, as três interfaces passam a consumir a mesma API, e nenhuma tela precisa mudar.

**O banco de dados é o mesmo para todas as interfaces.** Quem concluir uma aula no celular vê o mesmo XP no desktop, porque a regra e os dados estão num lugar só.

## Estrutura do projeto

```text
CSharp/          Sistema desktop em Windows Forms e o script do banco
Web/             O mesmo sistema em ASP.NET Core MVC
Api/             Briefing da integração (o código da API está no branch main)
Ferramentas/     Scripts de verificação do projeto
Modelagem/       Arquivo do Astah e diagramas exportados
Documentacao/    Relatórios e documentos das demais matérias
Apresentacao/    Slides e roteiros (vídeos ficam fora, só o link)
```

## Como executar

**Pré-requisitos:** Visual Studio 2022 ou mais novo, com a carga de trabalho **.NET desktop** (desktop) ou **ASP.NET e desenvolvimento web** (web). O banco é criado automaticamente na primeira execução.

### 1. Baixar o repositório

No Visual Studio: **Git → Clonar Repositório**, cole o endereço e escolha uma pasta em **Documentos** — não em Downloads, de onde o Windows apaga arquivos antigos sozinho.

### 2. Rodar o sistema desktop

Abra `CSharp/PlataformaCursos.sln` e pressione **Ctrl+F5**.

### 3. Rodar o site

Abra `Web/PlataformaCursosWeb.sln` e pressione **Ctrl+F5**. O navegador abre sozinho em `http://localhost:5000`.

Use **Ctrl+F5** (*Iniciar sem Depuração*), e não F5: com F5 o Visual Studio encerra o servidor assim que a janela do navegador fecha, o que atrapalha abrir uma segunda sessão.

Nos dois casos, aguarde a restauração dos pacotes NuGet na primeira vez — precisa de internet, e inclui o **BouncyCastle**, usado para o hash de senha.

### 4. Entrar no sistema

| Perfil | E-mail | Senha |
|---|---|---|
| **ADMIN** | `admin@escola.com` | `admin123` |
| **PROFESSOR** | `carla@escola.com` | `professor123` |
| **ALUNO** | `maria@escola.com` | `aluno123` |
| **ALUNO** | `joao@escola.com` | `aluno123` |

**Atenção:** em bancos criados antes da renomeação do perfil, a senha da Carla é `tutor123`. A senha mínima passou a ser de **8 caracteres**, para casar com a validação da API.

As senhas são convertidas para `scrypt` no primeiro login correto de cada conta. Por isso os scripts ainda trazem senhas legíveis: elas são o ponto de partida, não o estado final.

## Banco de dados

O script de criação é [CSharp/Banco_CursosDB.sql](CSharp/Banco_CursosDB.sql), executado automaticamente pelos programas. As tabelas de gamificação e matrícula seguem exatamente o esquema da API, definido em `sql/Etapa1_Professor.sql` e `sql/Etapa4_Gamificacao.sql`, no branch `main`.

| Arquivo | Finalidade |
|---|---|
| `CSharp/Banco_CursosDB.sql` | Estrutura completa: usuários, cursos, aulas, progresso, gamificação e matrículas. |
| `CSharp/Zerar_Testes.sql` | Apaga progresso, XP e conquistas para repetir testes. Preserva usuários, cursos e o catálogo de medalhas. |

**Atenção:** não execute `Banco_CursosDB.sql` no banco do Azure. Ele monta o esquema local; o da nuvem é o do aplicativo mobile. Os `IF OBJECT_ID` evitam recriar tabelas, mas não protegem contra os `ALTER TABLE` e o `UPDATE` da seção de migração. Antes de apontar qualquer interface para o Azure, o grupo precisa comparar os dois esquemas.

## Verificação do projeto

Como as três interfaces compartilham banco e contratos, existe um script que confere o cliente HTTP do C# contra o código real da API — rotas e nomes de campo, um a um:

```bash
python3 Ferramentas/conferir_api.py ../caminho/para/api/src
```

Vale rodar sempre que a API mudar de rota ou de nome de campo. O relato completo da sincronização entre os dois lados está em [Documentacao/RESPOSTA_ANALISE_INTEGRACAO.md](Documentacao/RESPOSTA_ANALISE_INTEGRACAO.md).

## Trabalho em grupo

### Antes de mexer, puxe

No Visual Studio: **Git → Buscar** e depois **Puxar**. Trabalhar o dia inteiro sem puxar significa mexer numa versão antiga e ter que juntar tudo na mão depois.

### Para enviar

**Git → Confirmar Tudo**, com uma frase do que mudou, e depois **Git → Enviar por Push**. Mensagens como `"corrigi a validacao do email"` ajudam; `"alteracoes"` não ajuda ninguém.

### Divisão de arquivos

Dois integrantes no mesmo arquivo ao mesmo tempo gera conflito, e alguém perde trabalho.

| Integrante | Responsável por |
|---|---|
| *(preencher)* | *(ex.: api/ e mobile/)* |
| *(preencher)* | *(ex.: Web/Views e Web/Controllers)* |
| *(preencher)* | *(ex.: CSharp/ — formulários)* |
| *(preencher)* | *(ex.: Modelagem e Documentacao)* |

Os arquivos mais sensíveis são os **`.Designer.cs`** do projeto desktop: quando alguém arrasta um controle na tela, o Visual Studio reescreve o arquivo quase inteiro, e o Git não consegue juntar duas versões dessas. As views `.cshtml` não têm esse problema, por serem texto escrito à mão.

`BancoService.cs`, `Servicos.cs` e `Modelo.cs` **existem nos dois projetos C#** — avise no grupo antes de mexer e lembre de aplicar a mesma correção nos dois lugares.

## Observações

- Arquivos acima de **100 MB não sobem**. O GitHub recusa e o push falha inteiro, não só aquele arquivo. Vídeos de apresentação quase sempre passam disso: deixe no Teams ou no Drive e coloque o link em `Apresentacao/README.md`.
- As pastas `bin`, `obj` e `.vs` **não sobem**, de propósito. São geradas pela compilação e pelo Visual Studio, mudam em cada máquina e geram conflito.
- **Criou uma pasta de projeto nova?** Copie o bloco correspondente no `.gitignore`, trocando o prefixo. Se esquecer, centenas de arquivos gerados vão para o GitHub.
- **Senhas e credenciais não vão para o repositório.** A conexão local usa a conta do Windows de quem está rodando. O `.env` da API e qualquer `appsettings.json` com usuário e senha do SQL Server precisam ficar fora do Git.
- A recuperação automática de senha por e-mail ainda não está implementada na API, que responde `501`. O desktop e a web têm um fluxo próprio, com código de prazo determinado.

## Integrantes

- *(preencher)*
- *(preencher)*
- *(preencher)*
- *(preencher)*

---

**Projeto acadêmico — PIM IV | Professor: Antonio Graça**
