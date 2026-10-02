# Trabalho Grupo PIM 4ª semestre

Repositório central do grupo. Cada matéria ou etapa do trabalho tem a sua pasta.


---

## O que tem em cada pasta

| Pasta | Conteúdo |
|---|---|
| **[CSharp/](CSharp/)** | O sistema em Windows Forms + SQL Server. Instruções completas em [CSharp/README.md](CSharp/README.md) |
| **[Modelagem/](Modelagem/)** | O arquivo do Astah e os diagramas exportados |
| **[Documentacao/](Documentacao/)** | Relatórios, textos e documentos das demais matérias |
| **[Apresentacao/](Apresentacao/)** | Slides e roteiros. Vídeos ficam fora, só o link |

---

## Começando (para quem acabou de entrar no repositório)

### 1. Baixar o repositório

No Visual Studio: **Git → Clonar Repositório**, cole o endereço e escolha uma pasta no seu computador.

> Escolha uma pasta em **Documentos**, não em Downloads. O Windows apaga arquivos antigos da pasta Downloads automaticamente.

### 2. Rodar o sistema

1. Abra `CSharp/PlataformaCursos.sln`
2. Aguarde a restauração do pacote NuGet (precisa de internet na primeira vez)
3. Pressione **Ctrl+F5**

O banco de dados é criado automaticamente na primeira execução. Não é preciso configurar conexão nem rodar script.

### 3. Entrar no sistema

| Perfil | Email | Senha |
|---|---|---|
| Administrador | `admin@escola.com` | `admin123` |
| Aluno | `maria@escola.com` | `aluno123` |

Deu algum erro? A seção de problemas comuns está em [CSharp/README.md](CSharp/README.md).

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
| *(preencher)* | *(ex.: FrmLogin e FrmRecuperarSenha)* |
| *(preencher)* | *(ex.: FrmPrincipal e FrmCatalogo)* |
| *(preencher)* | *(ex.: FrmCursos e FrmUsuarios)* |

Os arquivos mais sensíveis são os `.Designer.cs`: quando alguém arrasta um controle na tela, o Visual Studio reescreve o arquivo quase inteiro, e o Git não consegue juntar duas versões dessas.

`BancoService.cs` e `Servicos.cs` são de uso comum — avise no grupo antes de mexer.

---

## Regras do repositório

**Arquivos acima de 100 MB não sobem.** O GitHub recusa, e o push falha inteiro — não só aquele arquivo. Vídeos de apresentação quase sempre passam disso: deixe no Teams ou no Drive e coloque o link em `Apresentacao/README.md`.

**As pastas `bin` e `obj` do projeto C# não sobem**, de propósito. São geradas na compilação e mudam em cada máquina. Isso vale só dentro de `CSharp/` — o trabalho das outras matérias não sofre nenhuma restrição.

**Trabalho de outra matéria vai na pasta certa**, não na raiz. Se nenhuma pasta servir, crie uma nova e avisar o grupo.

---

## Integrantes

- *Arthur Fernandes Reis*
- *Leonardo Alves Fudoli*
- *Lucas Matheus Gonçalves Marques*
- *Nicolas Gramani Machado França*
- *Maria Clara Vilas Boas de Souza*
- *Guilherme Alves Martins dos Santos*
