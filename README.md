# Trabalho em Grupo — Linguagem de Programação C#

Repositório central do grupo. Cada matéria ou etapa do trabalho tem a sua pasta.

**Professor:** Antonio Graça

---

## O que tem em cada pasta

| Pasta | Conteúdo |
|---|---|
| **[CSharp/](CSharp/)** | O sistema em **Windows Forms** + SQL Server. Instruções em [CSharp/README.md](CSharp/README.md) |
| **[Web/](Web/)** | O **mesmo sistema** em ASP.NET Core MVC. Instruções em [Web/README.md](Web/README.md) |
| **[Api/](Api/)** | O serviço que vai atender os dois. Ainda não começou — o briefing está em [Api/README.md](Api/README.md) |
| **[Modelagem/](Modelagem/)** | O arquivo do Astah e os diagramas exportados |
| **[Documentacao/](Documentacao/)** | Relatórios, textos e documentos das demais matérias |
| **[Apresentacao/](Apresentacao/)** | Slides e roteiros. Vídeos ficam fora, só o link |

### Os três projetos, e como se encaixam

```
hoje:
  Windows Forms  →  Servicos  →  BancoService  →  SQL Server (CursosDB)
  ASP.NET MVC    →  Servicos  →  BancoService  →  SQL Server (CursosDB)

depois da API:
  Windows Forms  →  Servicos  →  ApiService  ┐
                                             ├→  API  →  BancoService  →  SQL Server
  ASP.NET MVC    →  Servicos  →  ApiService  ┘
```

**O banco de dados é o mesmo para os dois sistemas.** Quem entrar no desktop e depois na web vê o mesmo progresso, os mesmos cursos e os mesmos usuários. O script está em [CSharp/Banco_CursosDB.sql](CSharp/Banco_CursosDB.sql), numa cópia só.

**A versão 2 está alinhada com a modelagem do Astah.** Os dois sistemas foram revisados mensagem por mensagem contra os 14 diagramas de sequência. O mapa completo está em [CSharp/RASTREABILIDADE.md](CSharp/RASTREABILIDADE.md) e [Web/RASTREABILIDADE_WEB.md](Web/RASTREABILIDADE_WEB.md) — são os documentos para levar na apresentação.

---

## Começando (para quem acabou de entrar no repositório)

### 1. Baixar o repositório

No Visual Studio: **Git → Clonar Repositório**, cole o endereço e escolha uma pasta no seu computador.

> Escolha uma pasta em **Documentos**, não em Downloads. O Windows apaga arquivos antigos da pasta Downloads automaticamente.

### 2. Rodar o sistema

**Versão desktop:** abra `CSharp/PlataformaCursos.sln` e pressione **Ctrl+F5**.

**Versão web:** abra `Web/PlataformaCursosWeb.sln` e pressione **F5**. O navegador abre sozinho em `http://localhost:5000`.

Nos dois casos, aguarde a restauração do pacote NuGet na primeira vez (precisa de internet). O banco de dados é criado automaticamente na primeira execução — não é preciso configurar conexão nem rodar script.

> Quem já rodou a versão 1 **não precisa apagar o banco**. Na primeira execução da versão 2 o programa acrescenta sozinho as colunas e tabelas novas, sem perder nenhum dado.

### 3. Entrar no sistema

| Perfil | Email | Senha | O que pode fazer |
|---|---|---|---|
| Administrador | `admin@escola.com` | `admin123` | tudo |
| Tutor | `carla@escola.com` | `tutor123` | cursos, aulas e progresso dos alunos |
| Aluno | `maria@escola.com` | `aluno123` | catálogo e as próprias aulas |

Deu algum erro? A seção de problemas comuns está em [CSharp/README.md](CSharp/README.md) e [Web/README.md](Web/README.md).

---

## Como trabalhar em grupo sem dor de cabeça

### Antes de começar a mexer, sempre puxe

No Visual Studio, a seta para baixo na barra do Git, ou **Git → Pull**.

Isso traz para o seu computador o que os outros já enviaram. Se você trabalhar o dia inteiro sem puxar, vai estar mexendo numa versão antiga — e na hora de enviar terá que juntar tudo na mão.

### Para enviar o que você fez

**Git → Confirmar Tudo** (escreva uma frase do que mudou) e depois **Git → Push**.

Escreva mensagens que digam o que mudou: `"corrigi a validacao do email"` ajuda; `"alteracoes"` não ajuda ninguém.

### Divisão de arquivos

Dois integrantes mexendo no mesmo arquivo ao mesmo tempo gera conflito, e alguém perde trabalho. Combinem quem cuida do quê:

| Integrante | Responsável por |
|---|---|
| *(preencher)* | *(ex.: Api/ — o serviço e a integração)* |
| *(preencher)* | *(ex.: Web/Views e Web/Controllers)* |
| *(preencher)* | *(ex.: CSharp/ — formulários)* |
| *(preencher)* | *(ex.: Modelagem e Documentacao)* |

Os arquivos mais sensíveis são os **`.Designer.cs`** do projeto desktop: quando alguém arrasta um controle na tela, o Visual Studio reescreve o arquivo quase inteiro, e o Git não consegue juntar duas versões dessas. A web não tem esse problema — as views `.cshtml` são texto escrito à mão.

`BancoService.cs`, `Servicos.cs` e `Modelo.cs` são de uso comum, e **existem nos dois projetos** — avise no grupo antes de mexer, e lembre de aplicar a mesma correção nos dois lugares.

---

## Regras do repositório

**Arquivos acima de 100 MB não sobem.** O GitHub recusa, e o push falha inteiro — não só aquele arquivo. Vídeos de apresentação quase sempre passam disso: deixe no Teams ou no Drive e coloque o link em `Apresentacao/README.md`.

**As pastas `bin` e `obj` não sobem**, de propósito. São geradas na compilação e mudam em cada máquina. Isso vale dentro de `CSharp/`, `Web/` e `Api/` — o trabalho das outras matérias não sofre nenhuma restrição.

> **Criou uma pasta de projeto nova?** Copie o bloco correspondente no `.gitignore`, trocando o prefixo. Se esquecer, as pastas `bin` e `obj` daquele projeto vão para o GitHub: são centenas de arquivos que mudam a cada compilação e geram conflito em todo mundo.

**Senha nenhuma vai para o repositório.** Hoje a conexão com o banco usa a conta do Windows de quem está rodando, então não há senha em lugar algum. Quando a API nascer, se alguém configurar usuário e senha do SQL Server num `appsettings.json`, esse arquivo precisa sair do Git.

**Trabalho de outra matéria vai na pasta certa**, não na raiz. Se nenhuma pasta servir, crie uma nova e avise o grupo.

---

## Integrantes

- *(preencher)*
- *(preencher)*
- *(preencher)*
- *(preencher)*
