# Sincronização com a API — o que foi feito e o que ainda falta

**Lido:** branch `main` do repositório (`api/`, `mobile/`, `sql/`), em 18/11/2026
**Escopo:** programa desktop (C# / Windows Forms) e site (ASP.NET Core MVC)
**Substitui:** a versão anterior deste documento, que foi escrita por suposição

---

## Antes de tudo: os dois branches

O repositório tem duas linhas de commits que nunca se encontraram:

| Branch | Quem | O quê | Commits |
|---|---|---|---|
| `main` | Guilherme | `api/`, `mobile/`, `sql/` | 11 |
| `master` | Arthur | `CSharp/`, `Web/` | 1 |

Não há ancestral comum — o Git as trata como projetos independentes. Só `.gitignore` e `README.md` existem nos dois lados, com conteúdos diferentes. Em algum momento elas precisam ser unidas; como quase não há arquivo repetido, deve ser tranquilo. Fica para depois.

---

## O que mudou agora que dava para ler o código

A versão anterior destas mudanças foi escrita sem acesso ao branch `main`, a partir da tabela de endpoints do documento de análise. Com o código à mão, **três suposições se mostraram erradas** — e uma delas teria quebrado os dois sistemas.

### 1. A colisão de tabelas (teria quebrado tudo)

Os dois lados criavam `dbo.Conquistas`, com significados incompatíveis:

| | Versão anterior do C# | API (`Etapa4_Gamificacao.sql`) |
|---|---|---|
| O que guarda | medalha que um aluno ganhou | **catálogo** de medalhas que existem |
| Colunas | `Id, UsuarioId, Codigo, DataHora` | `Id, Codigo, Nome, Descricao, Icone, XPBonus` |

No mesmo banco, quem criasse primeiro venceria e o outro quebraria: o `IF OBJECT_ID ... IS NULL` veria a tabela existindo, não criaria nada, e o `INSERT` seguinte falharia por coluna inexistente.

**Resolvido adotando o esquema da API por inteiro.** O C# agora cria e usa exatamente `Gamificacao`, `Conquistas`, `UsuarioConquistas`, `HistoricoXP`, `Matriculas` e `Cursos.ProfessorId`, com as mesmas colunas, as mesmas restrições e as mesmas cinco medalhas (mesmo código, nome, ícone e XP).

Mais do que copiar as tabelas: a **regra** também foi para o banco. O método que concede XP no C# é um porte quase literal do T-SQL de `awardLessonCompletion`. Concluir uma aula pelo celular, pelo desktop ou pelo site produz o mesmo XP, as mesmas conquistas e a mesma sequência de dias — porque é o mesmo SQL.

### 2. A senha: scrypt, não PBKDF2 — e um detalhe traiçoeiro

A API usa scrypt no formato `scrypt:salt:hash`. A versão anterior do C# usava PBKDF2. As duas gravam na **mesma coluna** `Usuarios.Senha`: senha criada no desktop não entraria no celular, e vice-versa.

Agora o C# usa scrypt, no formato dela. Dois detalhes que não apareceriam em revisão de código e quebrariam em silêncio:

**O salt não é o que parece.** No código da API:

```js
const salt = crypto.randomBytes(16).toString('hex');   // string de 32 caracteres
const hash = await scrypt(password, salt, 64);         // salt entra como STRING
```

O Node converte string em bytes usando utf8. Então o que alimenta o scrypt são os **32 bytes ASCII do texto hexadecimal**, não os 16 bytes que esse texto representa. Usar os 16 bytes decodificados gera um hash de aparência perfeitamente normal que nunca confere.

**O hexadecimal tem que ser minúsculo.** A verificação da API exige `/^[a-f0-9]{32}$/` e `/^[a-f0-9]{128}$/`. O `Convert.ToHexString` do .NET devolve **maiúsculo** — sem converter, ela recusaria todo hash vindo do C# sem nem tentar calcular.

**Isto foi testado de verdade, não deduzido.** A função de hash da API foi extraída e executada no Node, e o resultado comparado byte a byte com o cálculo do lado C#, nos dois sentidos: senha criada aqui entra na API, senha criada pela API entra aqui. Todos os casos passaram, incluindo valores corrompidos.

O .NET não traz scrypt pronto, então o projeto passou a depender de **BouncyCastle** (`PackageReference` já adicionado nos dois `.csproj`). A alternativa seria escrever o algoritmo à mão, cujo erro típico é exatamente o do parágrafo acima: silencioso.

### 3. Não existe o envelope `{ok, mensagem, dados}`

O briefing em `Api/README.md` propunha esse envelope e o C# foi escrito esperando por ele. A API responde outra coisa:

```
login ....: { token, user: { id, nome, email, perfil } }    <- "user", não "usuario"
erro .....: { message: "..." }                               <- "message", em inglês
listas ...: [ {...}, {...} ]                                 <- array puro
progresso : { message, gamificacao: {...} }
```

Quem diz se deu certo é o código HTTP, não um campo no corpo. O `ClienteApi.cs` foi reescrito. **As 16 rotas e os 28 nomes de campo foram conferidos automaticamente contra o `server.js`** — o script está em `Ferramentas/conferir_api.py` e pode ser rodado de novo sempre que a API mudar:

```
python3 Ferramentas/conferir_api.py ../caminho/para/api/src
```

---

## Um bug encontrado na API

**Três dos cinco arquivos de rota não estão montados.** O `server.js` só carrega dois:

```js
app.use('/api/professor', require('./routes/professorRoutes'));
app.use('/api/gamificacao', require('./routes/gamificationRoutes'));
```

`adminRoutes.js`, `authRoutes.js` e `courseRoutes.js` existem no repositório mas nenhuma rota deles responde. As rotas de curso e de administração que funcionam são as escritas direto no `server.js`.

**A consequência não é só código morto.** A matrícula do aluno num curso é criada em `courseRoutes.js` — que não está no ar. Como a conquista `PRIMEIRO_CURSO` (a mais valiosa, 100 XP) conta cursos concluídos olhando `dbo.Matriculas`, e nada nunca insere lá, **essa conquista não é concedida a ninguém, nem pelo celular.**

Solução provisória do lado C#: ao concluir uma aula, o programa garante que existe a matrícula naquele curso (`BancoService.GarantirMatricula`). Isso faz a conquista funcionar sem mudar a regra — continua sendo "todas as aulas de um curso em que estou matriculado" — e, como a tabela é a mesma, conserta também para o celular.

**Vale decidir em grupo** se é assim que deve ficar, ou se o certo é montar o `courseRoutes` e ter matrícula explícita.

> **Atualização de 19/11 — resolvido.** O grupo escolheu a matrícula explícita, e o responsável pela API montou o `courseRoutes` em `/api/cursos`, removendo as cópias que estavam soltas no `server.js`. O aluno agora se matricula antes de começar o curso, pelo botão, nas três interfaces; `GarantirMatricula` virou `BancoService.Matricular` e só é chamada quando a tela pede. Sem matrícula, o `ProgressoService` recusa gravar progresso, com a mesma mensagem que a API devolve no 403.
>
> Duas pendências nasceram dessa mudança, já comunicadas ao responsável pela API: `GET /api/cursos` ficou `studentOnly` (professor e admin levam 403 ao listar cursos) e a nova rota de ranking `GET /api/gamificacao/turma` não devolve a contagem de aulas nem a de conquistas — por isso essas duas colunas saíram da tela de classificação.

---

## O que já batia

Duas coisas foram escolhidas independentemente e deram no mesmo:

- **10 XP por aula** e **100 XP por nível**, com `nivel = XP / 100 + 1`
- os perfis **`ALUNO` / `PROFESSOR` / `ADMIN`**, em maiúsculas

---

## Diferenças que sobraram, de propósito

| Ponto | API | C# | Por quê |
|---|---|---|---|
| Desmarcar aula | não devolve XP | devolve | sem isso o XP fica maior do que as aulas realmente concluídas |
| Conquista já ganha | — | nunca é retirada | medalha conquistada não se perde |
| Recuperação de senha | 501, "procure o administrador" | fluxo de código com prazo de 15 min | o fluxo do desktop é uma das duas opções que a própria análise aprova |

As duas primeiras são correções conscientes. A terceira é a que precisa de decisão do grupo: ou a API passa a implementar o fluxo, ou o desktop passa a mandar o aluno procurar o administrador.

---

## Ajustes menores feitos para casar

- **Senha mínima de 8 caracteres** (era 6). A API recusa menos que isso; com 6, um usuário criado no desktop seria rejeitado por ela depois.
- **E-mail sempre em minúsculas**, no login, na criação e na edição. A API faz `email.trim().toLowerCase()`; sem normalizar, `Maria@Escola.com` e `maria@escola.com` viram dois usuários diferentes.
- As contas de demonstração (`admin123`, `professor123`, `aluno123`) já têm 8 ou mais caracteres e continuam valendo.

---

## Para ligar a API

Está tudo pronto e **desligado**. Para virar a chave:

- **Site:** em `appsettings.json`, `Api:Ativa` para `true` e `Api:UrlBase` apontando para a máquina onde a API roda
- **Desktop:** em `ClienteApi.cs`, a constante `ConfiguracaoApi.ATIVA`

Nenhuma tela muda. As 10 telas do desktop e as 19 views do site não sabem de onde o dado vem.

---

## O que ainda depende de decisão do grupo

1. **Montar ou não o `courseRoutes`** no `server.js` (matrícula explícita × automática)
2. **Qual fluxo de recuperação de senha** vale para todos
3. **A comparação de esquema com o Azure** — essa é com a Maria, não comigo

---

## Nota sobre verificação

Não há .NET SDK no ambiente onde estas mudanças foram feitas, então **nada foi compilado**. O que foi verificado, e como:

| O quê | Como |
|---|---|
| Compatibilidade do hash | executando a função da API no Node e comparando byte a byte, nos dois sentidos |
| Rotas e nomes de campo | cruzamento automático contra o `server.js` (`Ferramentas/conferir_api.py`) |
| Chaves, parênteses, controles × Designer, sobreposição de layout, views × ViewModels, serviços × `Program.cs`, parâmetros SQL | validador estático, testado com erros injetados para provar que acusa |
| Chamadas a métodos e propriedades que deixaram de existir | regra própria no validador, criada por causa desta renomeação |

Tudo passa. Mas **o primeiro `Ctrl+F5` continua sendo o teste de verdade** — se algo não compilar, é erro de digitação, não de lógica, e é rápido de achar.
