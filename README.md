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
- **API:** uma só para as três interfaces, em Node.js e Express, unificada pelo Guilherme e hospedada em `https://pim-iv-api.onrender.com`.
- **Banco de dados:** Azure SQL. Só a API fala com ele; o desktop e a web não têm banco próprio.
- **Autenticação:** senhas armazenadas com `scrypt`, no mesmo formato nas três interfaces.

## Funcionalidades

| Perfil | Funcionalidades principais |
|---|---|
| **ADMIN** | Cadastrar e gerenciar usuários (alunos, professores e administradores) e gerenciar cursos. |
| **PROFESSOR** | Gerenciar cursos e aulas, acompanhar o progresso dos alunos e consultar a classificação por XP. |
| **ALUNO** | Consultar o catálogo, acessar cursos, concluir aulas e visualizar o próprio XP e conquistas. |

**O cadastro de usuários é exclusivo do administrador.** Não existe cadastro público em nenhuma das interfaces.

### Progresso e gamificação

O aluno **se matricula no curso antes de começar**, pelo botão, nas três interfaces. Sem matrícula o conteúdo continua aberto para leitura, mas nenhuma aula pode ser marcada como concluída — a mesma regra e a mesma mensagem nos três lugares, porque quem recusa é a camada de serviço, não a tela.

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
   Windows Forms     ASP.NET Core MVC       React Native (Expo)
         |                  |                       |
         +------------------+-----------------------+
                            | HTTP / JWT
                            v
          API Node.js / Express (unica, hospedada no Render)
                            |
                            v
                 Azure SQL (o mesmo banco)
```

As três interfaces usam **a mesma API**, unificada pelo Guilherme. O desktop e a web vêm configurados para ela: endereço em `appsettings.json` (web) e no perfil **API (Azure)** do `launchSettings.json` (desktop e web).

**O banco de dados é o mesmo para todas as interfaces.** Quem concluir uma aula no celular vê o mesmo XP no desktop, porque a regra e os dados estão num lugar só.

## Estrutura do projeto

```text
CSharp/          Sistema desktop em Windows Forms e o script do banco
Web/             O mesmo sistema em ASP.NET Core MVC
Ferramentas/     Scripts de verificação do projeto
Modelagem/       Arquivo do Astah e diagramas exportados
Documentacao/    Relatórios e documentos das demais matérias
Apresentacao/    Slides e roteiros (vídeos ficam fora, só o link)
```

## Como executar

**Pré-requisitos:** Visual Studio 2022 ou mais novo, com a carga de trabalho **.NET desktop** (desktop) ou **ASP.NET e desenvolvimento web** (web). Precisa de **internet**: todos os dados vêm da API do grupo.

### 1. Baixar o repositório

No Visual Studio: **Git → Clonar Repositório**, cole o endereço e escolha uma pasta em **Documentos** — não em Downloads, de onde o Windows apaga arquivos antigos sozinho.

### 2. Rodar o sistema desktop

Abra `CSharp/PlataformaCursos.sln` e pressione **Ctrl+F5**.

### 3. Rodar o site

Abra `Web/PlataformaCursosWeb.sln` e pressione **Ctrl+F5**. O navegador abre sozinho em `http://localhost:5000`.

Use **Ctrl+F5** (*Iniciar sem Depuração*), e não F5: com F5 o Visual Studio encerra o servidor assim que a janela do navegador fecha, o que atrapalha abrir uma segunda sessão.

Na primeira vez o servidor gratuito da API pode demorar alguns segundos para acordar: a tela de login espera por ele antes de avisar que não respondeu.

### 4. Entrar no sistema

- **Aluno** pode criar a própria conta em *Criar conta de aluno*, na tela de login.
- **Professor e administrador** são cadastrados pelo administrador, em *Usuários*, com uma **senha temporária**. No primeiro login, a pessoa troca essa senha.
- Ao criar a conta ou concluir o primeiro acesso, o sistema mostra um **código de recuperação**, exibido uma vez só. É com ele que se redefine a senha em *Esqueci minha senha*.

A senha precisa ter pelo menos **8 caracteres** e é guardada pela API em `scrypt`, nunca como texto.

## Banco de dados

O banco fica no **Azure SQL** e é o mesmo para as três interfaces. Quem fala com ele é só a API: o desktop e a web pedem e enviam dados por ela, nunca direto ao banco. Por isso a regra de XP, de matrícula e de permissão existe num lugar só.

O esquema (tabelas e colunas) é mantido junto com a API, no projeto do Guilherme, e administrado pela Maria no portal do Azure.

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

| Integrante | Responsável por | Onde fica |
|---|---|---|
| **Arthur Fernandes Reis** | Sistema desktop e web, em C# | `CSharp/`, `Web/` (branch `master`) |
| **Guilherme Alves Martins dos Santos** | Aplicativo mobile e API | `api/`, `mobile/` (branch `main`) |
| **Maria Clara Vilas Boas de Souza** | Banco de dados | Azure SQL e os scripts do banco |
| **Leonardo Alves Fudoli** | Modelagem UML e gestão ágil | `Modelagem/` |
| **Lucas Matheus Marques Gonçalves** | Documentação das disciplinas a distância | `Documentacao/` |
| **Nícolas Gramani Machado França** | Documentação das disciplinas a distância | `Documentacao/` |

Os arquivos mais sensíveis são os **`.Designer.cs`** do projeto desktop: quando alguém arrasta um controle na tela, o Visual Studio reescreve o arquivo quase inteiro, e o Git não consegue juntar duas versões dessas. As views `.cshtml` não têm esse problema, por serem texto escrito à mão.

`ClienteApi.cs`, `PonteApi.cs`, `Servicos.cs` e `Modelo.cs` **existem nos dois projetos C#** — avise no grupo antes de mexer e lembre de aplicar a mesma correção nos dois lugares.

## Observações

- Arquivos acima de **100 MB não sobem**. O GitHub recusa e o push falha inteiro, não só aquele arquivo. Vídeos de apresentação quase sempre passam disso: deixe no Teams ou no Drive e coloque o link em `Apresentacao/README.md`.
- As pastas `bin`, `obj` e `.vs` **não sobem**, de propósito. São geradas pela compilação e pelo Visual Studio, mudam em cada máquina e geram conflito.
- **Criou uma pasta de projeto nova?** Copie o bloco correspondente no `.gitignore`, trocando o prefixo. Se esquecer, centenas de arquivos gerados vão para o GitHub.
- **Senhas e credenciais não vão para o repositório.** O `.env` da API, com o usuário e a senha do Azure, fica fora do Git (está no `.gitignore`).

## Integrantes

- Arthur Fernandes Reis
- Guilherme Alves Martins dos Santos
- Leonardo Alves Fudoli
- Lucas Matheus Marques Gonçalves
- Maria Clara Vilas Boas de Souza
- Nícolas Gramani Machado França

---

**Projeto acadêmico — PIM IV 
