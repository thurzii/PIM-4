# API — o serviço que vai atender os dois sistemas

> Esta pasta ainda está vazia. É o espaço do projeto de API do grupo.
> Este documento é o briefing: onde encaixar, o que expor e o que **não** mexer.

---

## 1. Como está hoje

Os dois sistemas falam **direto** com o SQL Server:

```
Windows Forms  →  Servicos.cs  →  BancoService  →  SQL Server (CursosDB)
ASP.NET MVC    →  Servicos.cs  →  BancoService  →  SQL Server (CursosDB)
```

Funciona, e para a entrega da disciplina está correto. Mas tem três incômodos reais:

1. **A string de conexão vive dentro de cada cliente.** Quem roda o programa precisa de acesso ao banco. Num sistema de verdade, o aluno não tem — e nem deveria ter.
2. **O `BancoService.cs` está duplicado** nos dois projetos. São 63 KB de código igual em dois lugares. Corrigiu um bug num? Precisa lembrar do outro.
3. **Nada além de C# consegue usar o sistema.** Um aplicativo de celular, por exemplo, ficaria de fora.

## 2. Como fica com a API

```
Windows Forms  →  Servicos.cs  →  ApiService  ┐
                                              ├→  API  →  BancoService  →  SQL Server
ASP.NET MVC    →  Servicos.cs  →  ApiService  ┘
```

Repare: o `BancoService` **não é jogado fora**. Ele se muda para dentro da API, e passa a ser o único lugar do sistema inteiro que conhece SQL. Os clientes ganham um `ApiService`, que fala HTTP em vez de SQL.

---

## 3. Onde exatamente está a emenda

**No `BancoService`.** Ele é a fronteira: tudo acima dele é regra de negócio e tela; ele é a única classe que abre conexão e monta comando SQL.

Isso não é acaso — o projeto foi escrito em camadas justamente para esta troca ser contida:

| Camada | Arquivo | Muda com a API? |
|---|---|---|
| Telas | `Frm*.cs`, `Views/*.cshtml` | **não** |
| Regra de negócio | `Servicos.cs` (7 classes) | **não** |
| Acesso a dados | `BancoService.cs` | **vira `ApiService` no cliente, e se muda para a API** |
| Domínio | `Modelo.cs` (11 classes) | **não** (vira projeto compartilhado) |

> **A meta a perseguir:** o `ApiService` deve ter **exatamente as mesmas assinaturas públicas** do `BancoService`. Se conseguir isso, nenhuma linha do `Servicos.cs`, dos formulários ou das views precisa mudar. É o que torna a troca viável no prazo de um trabalho.

---

## 4. Os 37 métodos que a API precisa cobrir

Lista tirada do `BancoService.cs` atual. A coluna de endpoint é **sugestão** — o importante é cobrir a operação.

### Usuários

| Método do BancoService | Endpoint sugerido |
|---|---|
| `ListarUsuarios(termo)` | `GET /api/usuarios?termo=` |
| `ListarUsuarios(termo, perfil)` | `GET /api/usuarios?termo=&perfil=` |
| `BuscarUsuarioPorId(id)` | `GET /api/usuarios/{id}` |
| `BuscarUsuarioPorEmail(email)` | `GET /api/usuarios/por-email?email=` |
| `EmailJaExiste(email, idIgnorar)` | `GET /api/usuarios/email-existe?email=&idIgnorar=` |
| `InserirUsuario(usuario)` | `POST /api/usuarios` |
| `AtualizarUsuario(usuario)` | `PUT /api/usuarios/{id}` |
| `AtualizarSenha(email, novaSenha)` | `PUT /api/usuarios/senha` |
| `ExcluirUsuario(id)` | `DELETE /api/usuarios/{id}` |
| `ContarProgressoDoUsuario(id)` | `GET /api/usuarios/{id}/progresso/total` |
| `ContarLogDoUsuario(id)` | `GET /api/usuarios/{id}/log/total` |

### Cursos

| Método do BancoService | Endpoint sugerido |
|---|---|
| `ListarCursos(FiltroCurso)` | `GET /api/cursos?termo=&cargaMin=&cargaMax=&somenteAtivos=&somenteNaoConcluidos=&usuarioId=` |
| `BuscarCursoPorId(id)` | `GET /api/cursos/{id}` |
| `InserirCurso(curso)` | `POST /api/cursos` |
| `AtualizarCurso(curso)` | `PUT /api/cursos/{id}` |
| `ExcluirCurso(id)` | `DELETE /api/cursos/{id}` |
| `ContarAlunosComProgressoNoCurso(cursoId)` | `GET /api/cursos/{id}/alunos-com-progresso` |

> O `FiltroCurso` tem seis campos e carrega o `usuarioId`, porque o filtro "somente os que eu ainda não concluí" depende de quem está perguntando. Vale virar *query string*, como acima.

### Aulas

| Método do BancoService | Endpoint sugerido |
|---|---|
| `ListarAulasDoCurso(cursoId, usuarioId)` | `GET /api/cursos/{cursoId}/aulas?usuarioId=` |
| `BuscarAulaPorId(aulaId, usuarioId)` | `GET /api/aulas/{id}?usuarioId=` |
| `InserirAula(aula)` | `POST /api/aulas` |
| `AtualizarAula(aula)` | `PUT /api/aulas/{id}` |
| `ExcluirAula(id)` | `DELETE /api/aulas/{id}` |

> O `usuarioId` aparece aqui porque a aula volta com a marca `afr_Concluida` **daquele** aluno. Passe 0 quando quem pergunta é o tutor, que não tem progresso.

### Progresso

| Método do BancoService | Endpoint sugerido |
|---|---|
| `MarcarAulaConcluida(usuarioId, aulaId)` | `POST /api/progresso` |
| `DesmarcarAulaConcluida(usuarioId, aulaId)` | `DELETE /api/progresso?usuarioId=&aulaId=` |
| `ContarAulasConcluidas(usuarioId, cursoId)` | `GET /api/progresso/concluidas?usuarioId=&cursoId=` |
| `BuscarProgressoDoAluno(alunoId)` | `GET /api/alunos/{id}/progresso` |
| `MontarResultado(concluidas, total, curso)` | **não precisa de endpoint** |

> `MontarResultado` é conta pura: não abre conexão nenhuma. Pode continuar rodando no cliente. Confira o corpo do método antes de decidir.

### Recuperação de senha (DS02)

| Método do BancoService | Endpoint sugerido |
|---|---|
| `GravarCodigo(usuarioId, codigo, expiracao)` | `POST /api/codigos` |
| `BuscarCodigo(usuarioId, codigo)` | `GET /api/codigos?usuarioId=&codigo=` |
| `MarcarCodigoUsado(codigoId)` | `PUT /api/codigos/{id}/usado` |

### Histórico de ações (DS12)

| Método do BancoService | Endpoint sugerido |
|---|---|
| `RegistrarAcao(logAcao)` | `POST /api/log` |
| `ListarLogDoUsuario(usuarioId, tipo, limite)` | `GET /api/usuarios/{id}/log?tipo=&limite=` |

### Infraestrutura — estes **não** viram endpoint de cliente

| Método | O que fazer |
|---|---|
| `ObterConexaoPadrao()` | fica só dentro da API |
| `DetectarServidor(out msg)` | fica só dentro da API |
| `PrepararBanco(out msg)` | a **API** passa a fazer isso ao subir |
| `TestarConexao(out msg)` | vira `GET /api/status` — o cliente passa a testar a API, não o banco |
| `ObterDescricao()` | o `GET /api/status` devolve junto, para a barra de rodapé continuar mostrando algo |

---

## 5. O detalhe que mais atrapalha: os parâmetros `out`

Metade dos métodos tem esta cara:

```csharp
public bool InserirUsuario(Usuario afr_usuario, out string afr_mensagem)
```

HTTP não tem parâmetro `out`. A saída é a API devolver sempre um envelope com as duas informações:

```json
{ "ok": true, "mensagem": "Usuario cadastrado com Id 7.", "dados": { "afr_Id": 7 } }
```

e o `ApiService` desmontar esse envelope de volta na assinatura original:

```csharp
public bool InserirUsuario(Usuario afr_usuario, out string afr_mensagem)
{
    var afr_resposta = PostarEReceber("api/usuarios", afr_usuario);

    afr_mensagem = afr_resposta.mensagem;   // sai do envelope
    return afr_resposta.ok;                 // sai do envelope
}
```

Feito assim, **o `Servicos.cs` não percebe diferença nenhuma.** É o que permite trocar a camada de baixo sem reescrever o de cima.

---

## 6. O que muda em cada cliente

### No desktop — uma linha por classe de serviço

O `Servicos.cs` do Windows Forms cria o banco assim:

```csharp
private BancoService afr_bancoService = new BancoService();
```

Vira:

```csharp
private ApiService afr_bancoService = new ApiService();
```

São 6 ocorrências em `Servicos.cs`, mais algumas nos formulários que usam `ObterDescricao()` e `TestarConexao()` (`FrmLogin`, `FrmPrincipal`, `FrmCursos`, `FrmUsuarios`). Um Ctrl+H resolve quase tudo.

### Na web — uma linha no `Program.cs`

A web usa injeção de dependência, então basta trocar o registro:

```csharp
builder.Services.AddScoped<BancoService>();   // antes
builder.Services.AddScoped<ApiService>();     // depois
```

e o tipo nos construtores dos serviços.

### O caminho mais elegante, se sobrar tempo

Extrair uma interface com as 37 assinaturas:

```csharp
public interface IDadosService { ... }

public class BancoService : IDadosService { ... }   // vai para a API
public class ApiService    : IDadosService { ... }  // fica nos clientes
```

Aí os clientes dependem da interface, e trocar uma implementação pela outra vira configuração. Também permite rodar o desktop sem a API no ar, se alguém precisar demonstrar offline.

---

## 7. O que não deve ser tocado

- **`Modelo.cs`** — as 11 classes de domínio são iguais nos dois projetos. O ideal é virarem um projeto de biblioteca compartilhado (`Modelo/Modelo.csproj`), referenciado pelos três. Se isso der trabalho demais, deixe duplicado mesmo; só não deixe os dois sairem do ar um do outro.
- **`Servicos.cs`** — é a regra de negócio, e ela não muda por causa de transporte. Se você precisar mexer aqui, provavelmente o `ApiService` fugiu das assinaturas originais.
- **Formulários e views** — não têm por que saber que existe uma API.
- **A rastreabilidade com o Astah** — os documentos [`../CSharp/RASTREABILIDADE.md`](../CSharp/RASTREABILIDADE.md) e [`../Web/RASTREABILIDADE_WEB.md`](../Web/RASTREABILIDADE_WEB.md) ligam cada mensagem dos 14 diagramas ao método que a implementa. Se um nome de método mudar, atualize o documento junto — é o que o professor vai usar para conferir.

---

## 8. Ordem de trabalho sugerida

1. **Criar o projeto** `Api/PlataformaCursosApi` (ASP.NET Core Web API, .NET 8) e mover o `BancoService.cs` para dentro dele.
2. **Fazer um endpoint só funcionar de ponta a ponta** — sugiro `GET /api/cursos`. Teste no Swagger antes de mexer em qualquer cliente.
3. **Criar o `ApiService`** num dos clientes, implementando só esse método, e fazer a tela de catálogo funcionar contra a API.
4. **Só então** implementar os 36 restantes. O padrão já estará resolvido; o resto é repetição.
5. **Repetir a troca no segundo cliente**, que agora é copiar e colar.

> Comece pela leitura (`GET`). As escritas (`POST`, `PUT`, `DELETE`) têm o envelope de `ok` e `mensagem` para acertar, e é mais fácil depurar isso quando a leitura já está funcionando.

---

## 9. Pontos de atenção

**O banco é um só.** Desktop e web usam o mesmo `CursosDB`. O script está em [`../CSharp/Banco_CursosDB.sql`](../CSharp/Banco_CursosDB.sql), com as seis tabelas, a migração da versão 1 e consultas de conferência. Enquanto a API não estiver pronta, os dois clientes continuam funcionando direto no banco — a troca pode ser feita sem parar o trabalho dos outros.

**Senha em `appsettings`.** Hoje a conexão usa `Integrated Security`, ou seja, a conta do Windows de quem está rodando: não há senha em lugar nenhum. Se a API for configurada com usuário e senha do SQL Server, **esse arquivo precisa sair do Git**. O `.gitignore` da raiz tem um aviso sobre isso.

**Autenticação.** Hoje o `ValidarLogin` compara a senha dentro do cliente. Com API, isso deveria acontecer do lado do servidor — o cliente manda e-mail e senha e recebe o usuário (ou uma negativa). Dá para fazer a troca mantendo a assinatura do `ValidarLogin`. Um passo adiante seria a API devolver um token, mas aí o `Servicos.cs` muda, e isso já é outro trabalho. **Para a entrega, mantenha o mínimo.**

**CORS.** Não deve ser necessário. O desktop não é navegador, e a web chama a API pelo servidor dela, não pelo JavaScript do navegador. Se aparecer erro de CORS, é sinal de que alguma chamada está saindo do navegador — vale conferir se é mesmo o que se quer.

**As senhas continuam em texto simples.** É assim no trabalho inteiro, para a demonstração ficar clara. Se for mexer nisso, mexa em todo lugar de uma vez: banco, API e os dois clientes. Meio caminho quebra o login.
