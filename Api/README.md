# API do desktop e da web

A API que o sistema desktop (Windows Forms) e o site (ASP.NET Core MVC) usam para falar com o **banco do Azure**. Feita em C# / ASP.NET Core (.NET 8), na mesma linguagem dos dois programas.

## Por que uma API própria

Cada projeto do grupo tem a sua API: o aplicativo do Guilherme usa a dele (Node.js, branch `main`), e o desktop e a web usam esta. **As duas gravam no mesmo banco do Azure.** Se uma mudar uma regra e a outra não, os dados ficam incoerentes.

Por isso esta API segue a do aplicativo em tudo que mexe em dados:

| O quê | Como está garantido |
|---|---|
| Rotas, nomes de campo, mensagens e códigos HTTP | Iguais aos da API do aplicativo (comparados lado a lado no teste) |
| Senha | `scrypt:<salt>:<hash>`, o mesmo formato. Uma senha criada por uma API entra pela outra |
| Token de login | JWT com a mesma chave, os mesmos campos e a mesma validade (8 h). O token de uma vale na outra |
| Regra de XP | O T-SQL do `gamificationService.js` copiado **por programa**, sem redigitar ([Infra/RegraXp.cs](PlataformaCursosApi/Infra/RegraXp.cs)) |
| Matrícula e permissões | Mesmas regras: só aluno se matricula e ganha XP; professor só mexe nos cursos dele |

**Se uma regra mudar na API do aplicativo, tem que mudar aqui também, e vice-versa.**

## Como rodar

1. Copie `PlataformaCursosApi/.env.exemplo` para `PlataformaCursosApi/.env` e preencha (o mesmo `.env` da API do aplicativo serve sem mudar nada).
2. Abra `Api/PlataformaCursosApi.sln` no Visual Studio e aperte **F5**, ou rode `dotnet run` dentro de `PlataformaCursosApi/`.
3. Confira em `http://localhost:3000/api/health`. A resposta esperada é `{"api":"ok","database":"ok"}`.

Sem `JWT_SECRET` no `.env`, a API **não inicia** e diz por quê. É de propósito: uma chave padrão escrita no código estaria no GitHub, e com ela qualquer um fabricaria um token de administrador.

### O `.env` nunca vai para o GitHub

Ele tem o usuário e a senha do banco do Azure. Está no `.gitignore` do repositório inteiro; só o `.env.exemplo`, sem segredos, sobe.

## Ligando o desktop e a web nesta API

| Programa | Onde | O que mudar |
|---|---|---|
| Web | `Web/PlataformaCursosWeb/appsettings.json` | `"Ativa": true` e `"UrlBase"` com o endereço da API |
| Desktop | `CSharp/PlataformaCursos/ClienteApi.cs` | `ATIVA = true` e `URL_BASE` com o endereço da API |

Com a chave ligada, os dois programas **não abrem conexão com banco nenhum**: tudo passa pela API. Com ela desligada, usam o banco local, para testes.

## Rotas

### As mesmas da API do aplicativo

| Rota | Quem |
|---|---|
| `GET /api/health` | todos |
| `POST /api/auth/login` · `POST /api/auth/recover` (501) | todos |
| `GET /api/cursos/:id` | logado (professor só os dele) |
| `POST /api/cursos/:id/matricula` · `PUT /api/cursos/:c/aulas/:a/progresso` | aluno |
| `GET /api/gamificacao/me` | aluno |
| `GET /api/gamificacao/turma` | professor e admin |
| `GET/POST /api/professor/cursos` · `PUT /api/professor/cursos/:id` | professor |
| `GET/POST /api/professor/cursos/:id/aulas` · `DELETE /api/professor/aulas/:id` | professor |
| `GET /api/professor/cursos/:id/alunos` | professor |
| `GET/POST /api/admin/cursos` · `PUT/DELETE /api/admin/cursos/:id` | admin |
| `POST /api/admin/cursos/:id/aulas` · `DELETE /api/admin/aulas/:id` | admin |
| `GET/POST /api/admin/usuarios` · `PUT /api/admin/usuarios/:id/status` | admin |

### A mais, para as telas do desktop e da web

Todas usam **só tabelas que já existem** no banco do Azure.

| Rota | Para quê |
|---|---|
| `GET /api/cursos` | Catálogo também para professor (os cursos dele) e admin. Na API do aplicativo é só de aluno |
| `GET /api/admin/usuarios/:id` · `PUT /api/admin/usuarios/:id` · `DELETE /api/admin/usuarios/:id` | Ver, editar e excluir usuário. Ninguém exclui a própria conta, e o sistema nunca fica sem administrador ativo |
| `GET /api/aulas/:id` | Abrir uma aula pelo id |
| `PUT /api/professor/aulas/:id` · `PUT /api/admin/aulas/:id` | Editar título e duração da aula |
| `GET /api/progresso/alunos/:id` | Progresso de um aluno em cada curso (professor vê só os cursos dele) |
| `GET /api/gamificacao/alunos/:id` | XP e medalhas de um aluno, para a tela do professor |

A lista de usuários e o ranking também trazem campos a mais (`dataCadastro`, `aulasConcluidas`, `conquistas`). Quem não usa ignora.

**Uma correção em relação à API do aplicativo:** em `DELETE /api/admin/aulas/:id`, o progresso da aula é apagado antes. Na outra API, essa rota dá erro 500 se algum aluno já concluiu a aula.

## O que ainda não dá

Precisam de tabela ou coluna que o banco do Azure não tem:

- **Texto da aula** (coluna `Conteudo` em `Aulas`)
- **Histórico de ações** (tabela `LogAcoes`)
- **Recuperação de senha** (tabela `CodigosRecuperacao`)

O banco é compartilhado: **combinar com o grupo antes de criar.** Até lá, essas telas mostram um aviso em vez de gravar escondido no banco local.

## Como foi testada

Num SQL Server de teste montado com os scripts do branch `main` (`sql/CursosDB.sql`, `Etapa1_Professor.sql` e `Etapa4_Gamificacao.sql`, a mesma estrutura do Azure):

- **Lado a lado com a API do aplicativo, no mesmo banco:** as mesmas requisições deram as mesmas respostas. Dois alunos, um por API, fizeram os mesmos passos e terminaram com o mesmo XP, sequência e medalhas.
- **Rotas a mais:** regras de edição e exclusão, aula por id, progresso e XP de um aluno.
- **Web e desktop ligados nela**, com os três perfis.

Os testes não foram rodados contra o Azure de verdade: eles criam usuários e cursos de mentira, que iriam parar no banco que o aplicativo usa.
