# Instruções para o Claude

Este arquivo é lido automaticamente ao abrir o projeto. Ele existe para que
uma sessão nova comece sabendo o que já foi decidido e onde estão as
armadilhas — não para repetir o que o `README.md` já explica.

---

## O que é este projeto

Trabalho em grupo do PIM IV (UNIP). Uma plataforma de cursos online com
**três interfaces sobre um único banco de dados**:

| Interface | Tecnologia | Branch | Responsável |
|---|---|---|---|
| Desktop | C# / Windows Forms (.NET 8) | `master` | Arthur |
| Web | C# / ASP.NET Core MVC (.NET 8) | `master` | Arthur |
| Mobile + API | React Native / Node.js | `main` | Guilherme |

**Os dois branches não têm ancestral comum.** `git merge-base master main`
não devolve nada. Para ler o código da API: `git fetch origin main` e depois
`git show FETCH_HEAD:api/src/server.js`.

---

## Convenções obrigatórias

**Prefixo `afr_` em tudo.** Variáveis, campos, propriedades e controles de
tela levam `afr_` na frente (`afr_usuario`, `afr_btnSalvar`, `afr_XpTotal`).
É exigência do professor Antonio Graça — são as iniciais do aluno. Código
novo sem o prefixo está errado, mesmo que funcione.

**Comentários em português sem acento.** O código inteiro segue isso
(`nao`, `versao`, `conclusao`). Mantenha, por consistência.

**Comentários explicam o PORQUÊ, não o quê.** O padrão do projeto é
comentar decisões e armadilhas, não narrar a linha seguinte.

**Desktop e web são espelhos.** Estes arquivos existem nos dois projetos e
precisam ser alterados **juntos**:

```
CSharp/PlataformaCursos/Modelo.cs    ↔  Web/PlataformaCursosWeb/Models/Modelo.cs
CSharp/PlataformaCursos/BancoService.cs ↔ Web/PlataformaCursosWeb/Services/BancoService.cs
CSharp/PlataformaCursos/Servicos.cs  ↔  Web/PlataformaCursosWeb/Services/Servicos.cs
CSharp/PlataformaCursos/SenhaHash.cs ↔  Web/PlataformaCursosWeb/Services/SenhaHash.cs
CSharp/PlataformaCursos/ClienteApi.cs ↔ Web/PlataformaCursosWeb/Services/ClienteApi.cs
```

A diferença entre eles é só o namespace e a sessão (ver abaixo). O validador
confere essa simetria.

---

## Verificação

**Sempre rode antes de entregar qualquer alteração em C#:**

```bash
python3 Ferramentas/validar.py
```

Ele confere, sem compilador: chaves e parênteses equilibrados, controles
usados × declarados no Designer, sobreposição de controles no layout,
propriedades usadas nas views × existentes nos ViewModels, serviços
injetados × registrados no `Program.cs`, parâmetros SQL × `AddWithValue`,
chamadas a métodos que não existem mais, e padrões proibidos (senha
comparada como texto, `new Random()`).

**Se você puder compilar** (`dotnet build`), compile — o validador é a rede
de segurança de quem não pode, não substituto.

**Ao mexer no `ClienteApi.cs`**, confira contra o código real da API:

```bash
python3 Ferramentas/conferir_api.py ../caminho/para/api/src
```

---

## Armadilhas que já custaram caro

### 1. O hash de senha (scrypt) tem dois detalhes silenciosos

O formato é o da API: `scrypt:<salt>:<hash>`. Duas coisas que compilam,
rodam, produzem um hash de aparência normal e **nunca conferem**:

- **O salt entra como os bytes ASCII do TEXTO hexadecimal**, não como os 16
  bytes que esse texto representa. No Node, `scrypt(password, salt, 64)`
  recebe o salt como string e converte com utf8. Usar
  `Convert.FromHexString(salt)` dá outro hash.
- **O hexadecimal tem que ser minúsculo.** A API valida com
  `/^[a-f0-9]{32}$/`. O `Convert.ToHexString` do .NET devolve MAIÚSCULO.

O validador tem regra para as duas. Não remova.

Parâmetros: N=16384, r=8, p=1, dkLen=64 (padrões do Node). Usa BouncyCastle,
porque o .NET não traz scrypt.

**Três formatos convivem** em `SenhaHash.Conferir()`: texto puro (contas
antigas), `pbkdf2$sha256$...` (de uma versão intermediária) e `scrypt:` (o
atual). Os dois primeiros são aceitos uma última vez e regravados em scrypt
no mesmo login. Remover essa ponte tranca usuários para fora **sem mensagem
de erro** — já aconteceu.

### 2. O esquema de gamificação é o DA API, não nosso

`Gamificacao`, `Conquistas`, `UsuarioConquistas`, `HistoricoXP` e
`Matriculas` são cópia fiel de `sql/Etapa1_Professor.sql` e
`sql/Etapa4_Gamificacao.sql`, no branch `main`.

**`dbo.Conquistas` é o CATÁLOGO** de medalhas que existem
(`Id, Codigo, Nome, Descricao, Icone, XPBonus`) — não as que um aluno ganhou.
Quem guarda isso é `dbo.UsuarioConquistas`. Uma versão anterior criou uma
`dbo.Conquistas` própria, com `UsuarioId`, e os dois programas colidiram no
mesmo banco.

**Nunca crie tabela de gamificação nova sem conferir o branch `main`.**

### 3. A regra de XP mora no banco, não no C#

`BancoService.ConcederXpPorAula` é um porte quase literal do T-SQL de
`api/src/gamificationService.js`. É de propósito: três programas mexem no
mesmo banco, e cada um com a sua cópia da regra divergiria.

A idempotência (não pagar duas vezes pela mesma aula) vem da restrição
`UQ_HistoricoXP_Evento UNIQUE (UsuarioId, ReferenciaTipo, ReferenciaId)`.
Não a substitua por `if` no código.

Valores: **10 XP por aula**, **100 XP por nível**, `nivel = XP / 100 + 1`.
Bônus de conquista: 20 / 30 / 50 / 75 / 100.

### 4. A API não tem envelope

O briefing antigo em `Api/README.md` propunha `{ ok, mensagem, dados }`.
**Não é o que a API faz.** Ela devolve `{ token, user }` no login,
`{ message }` nos erros (em inglês) e **array puro** nas listas. Quem diz se
deu certo é o código HTTP.

### 5. A matrícula virou explícita na API — e automática no nosso lado

**Resolvido em 19/11** (commit `ae4c5b9` do branch `main`): o `courseRoutes`
foi montado em `/api/cursos` e as cópias que estavam soltas no `server.js`
foram removidas. `adminRoutes.js` e `authRoutes.js` continuam desmontados,
mas as rotas deles existem no `server.js` — são arquivos mortos, não falhas.

Três consequências para o C#:

1. `GET /api/cursos` passou a ser **`studentOnly`**. Professor e admin levam
   403. O catálogo das telas deles só funciona no modo SQL até isso mudar.
2. `PUT /api/cursos/:c/aulas/:a/progresso` **exige matrícula** e responde
   403 sem ela. No modo API, chame `ClienteApi.MatricularEmCurso` antes.
3. `GET /api/cursos/:id` passou a trazer `matriculado` e `professorId`.

**Divergência de comportamento, não bug:** o mobile tem um botão
"matricular-se"; o desktop e a web matriculam sozinhos, em
`BancoService.GarantirMatricula`, ao concluir a primeira aula. Mesmo banco,
duas portas de entrada. **Decisão do grupo ainda em aberto.**

### 6. Sessão: estática no desktop, por requisição na web

No desktop, `Sessao` é uma classe **estática** — um usuário na memória do
programa. Num servidor web isso não funciona, porque ele atende várias
pessoas ao mesmo tempo. Virou `SessaoAtual`, um serviço por requisição,
**com os mesmos nomes de método** — e por isso o `Servicos.cs` foi
reaproveitado quase inteiro.

Ao copiar código do desktop para a web, troque `Sessao.X()` por
`afr_sessao.X()`, nada mais.

### 7. Editar `.Designer.cs` é editar coordenadas às cegas

As telas do Windows Forms têm posição fixa em pixels. Mexer sem ver já
colocou um painel por cima de dois botões. O validador tem uma regra que
detecta sobreposição e controle fora da janela — rode sempre depois de mexer
em Designer.

---

## Banco de dados

Criado automaticamente na primeira execução, em `(localdb)\MSSQLLocalDB`,
banco `CursosDB`. Script: `CSharp/Banco_CursosDB.sql`.

**Nunca rode esse script no Azure.** Ele monta o esquema local; o da nuvem é
o do mobile. Os `IF OBJECT_ID` protegem contra recriar tabela, mas não contra
os `ALTER TABLE` e o `UPDATE` da seção de migração.

Para repetir testes: `CSharp/Zerar_Testes.sql` apaga progresso, XP e
conquistas, preservando usuários, cursos e o catálogo de medalhas.

### Contas de teste

| Perfil | E-mail | Senha |
|---|---|---|
| ADMIN | `admin@escola.com` | `admin123` |
| PROFESSOR | `carla@escola.com` | `professor123` (em bancos antigos: `tutor123`) |
| ALUNO | `maria@escola.com` | `aluno123` |
| ALUNO | `joao@escola.com` | `aluno123` |

Senha mínima: **8 caracteres**. E-mail sempre em **minúsculas** — a API faz
`email.trim().toLowerCase()`, e sem normalizar o mesmo endereço vira dois
usuários diferentes.

---

## Integração com a API

`ClienteApi.cs` está pronto nos dois projetos e **desligado**:

- Desktop: constante `ConfiguracaoApi.ATIVA`
- Web: `Api:Ativa` no `appsettings.json`

As 18 rotas e os nomes de campo foram conferidos um a um contra o
`server.js`. Ao ligar, nenhuma tela muda — o ponto de troca é um arquivo.

**A rota de ranking já existe:** `GET /api/gamificacao/turma`, criada em
19/11 a nosso pedido, aceita PROFESSOR e ADMIN e devolve os alunos já
ordenados por XP, com a posição pronta. Ela **não** devolve aulas concluídas
nem número de conquistas — essas duas colunas da tela de ranking ficam em
zero no modo API até a rota passar a devolvê-las.

---

## Decisões pendentes do grupo

1. Matrícula explícita (botão, como no mobile) × automática (desktop e web)
   — o `courseRoutes` já foi montado; falta alinhar os dois comportamentos
2. Qual fluxo de recuperação de senha vale (a API responde 501; o desktop e
   a web têm código com prazo de 15 minutos)
3. Comparação de esquema entre o banco local e o do Azure
4. Unir os dois branches em uma linha só

---

## Onde ler mais

| Documento | Conteúdo |
|---|---|
| `README.md` | Visão geral, como executar, trabalho em grupo |
| `Documentacao/RESPOSTA_ANALISE_INTEGRACAO.md` | O relato completo da sincronização com a API: o que mudou, o que foi testado e como |
| `CSharp/RASTREABILIDADE.md` | Mapa das telas contra os 14 diagramas de sequência do Astah |
| `Web/RASTREABILIDADE_WEB.md` | O mesmo, para a web |

---

## Como trabalhar aqui

- **Não compile pelo usuário sem avisar o resultado.** Se `dotnet build`
  falhar, mostre o erro antes de seguir.
- **Não dê push sem o usuário ter rodado o programa**, a menos que ele peça.
  O `master` é compartilhado com o grupo.
- **Mensagens de commit explicam o porquê**, não só o quê. O histórico deste
  projeto segue esse padrão — leia `git log` para ver o tom.
