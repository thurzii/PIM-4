# Rastreabilidade: modelo do Astah × versão web

**Trabalho em grupo — Plataforma de Cursos Online (ASP.NET Core MVC)**
Modelo: `Modelagem/Projeto_UML.asta`

Este documento liga **cada mensagem** dos 14 diagramas de sequência ao lugar que a
implementa na versão web. É o irmão do `RASTREABILIDADE.md` do projeto desktop: os
mesmos diagramas, o mesmo nome de método nos serviços, mas com controller, action
e view no lugar do formulário.

Formato das colunas: `Controller.Action` para as actions, `Classe.Metodo` para os
serviços e o caminho do arquivo para as views.

---

## Como a versão web está dividida

```
Navegador
   ↓  requisição
Controller (Controllers/)        ← decide o que fazer e qual página mostrar
   ↓
Serviço (Services/Servicos.cs)   ← a regra de negócio, igual ao desktop
   ↓
BancoService                     ← a única classe que conhece SQL
   ↓
SQL Server (CursosDB, 6 tabelas)
```

A view só exibe. O controller não tem SQL. O serviço não monta página. É a mesma
separação do desktop, onde o formulário chamava o serviço e o serviço chamava o
banco.

**Os três atores do modelo e os três perfis:**

| Ator no Astah | Perfil | Como a web barra quem não pode |
|---|---|---|
| Aluno | `ALUNO` | — |
| Tutor | `TUTOR` | `[Authorize(Roles = "TUTOR,ADMIN")]` |
| Admin | `ADMIN` | `[Authorize(Roles = "ADMIN")]` |

Além do `[Authorize]`, os serviços conferem de novo (`VerificarPermissaoTutor`,
`VerificarPermissaoAdmin`, `ElegivelParaVisualizacao`). São as mesmas duas camadas
do desktop: esconder o botão no menu **e** checar no evento `Load`.

---

## DS01 — Fazer Login

| Mensagem no diagrama | Onde está |
|---|---|
| `digitarEmailSenha()` | `Views/Conta/Login.cshtml` |
| `validarLogin(email, senha)` | `AutenticacaoService.ValidarLogin` |
| `buscarUsuario(email)` | `BancoService.BuscarUsuarioPorEmail` |
| `usuarioValido()` | retorno de `ValidarLogin` (objeto `Usuario`, ou `null`) |
| `acessoPermitido()` | `ContaController.Login` → `HttpContext.SignInAsync` (grava o cookie) |
| `abrirTelaInicial()` | `RedirectToAction("Index", "Home")` |

O acesso é registrado no histórico por `LogService.RegistrarLogin`, que alimenta o
`loginLog` do DS12.

**Diferença em relação ao desktop:** lá, `acessoPermitido()` era
`Sessao.Abrir(usuario)` — uma variável estática. Aqui é um cookie assinado, que
pertence só àquele navegador.

---

## DS02 — Recuperar Senha

A linha de vida **Email** do diagrama é o que faz a recuperação ter três etapas.

| Mensagem no diagrama | Onde está |
|---|---|
| `informarEmail(email)` | `Views/Conta/RecuperarSenha.cshtml`, etapa 1 |
| `solicitarRecuperacao(email)` | `ContaController.EnviarCodigo` → `AutenticacaoService.SolicitarRecuperacao` |
| `enviarCodigo(email)` | `EmailService.EnviarCodigo` |
| `codigoEnviado()` | `BancoService.GravarCodigo` + retorno |
| `exibirMensagem()` | a mesma view, agora na etapa 2 |
| `recuperacaoConcluida()` | `ContaController.RedefinirSenha` → `AutenticacaoService.RedefinirSenha` |

A etapa 2 (`AutenticacaoService.ConferirCodigo`) não aparece nomeada no diagrama,
mas é o que torna o `enviarCodigo` útil: sem conferir o código, a senha trocaria
sem ele.

**A linha "Email", na prática:** o trabalho não tem servidor de e-mail, então a
mensagem é gravada em `dados/emails.txt`. No desktop um botão abria esse arquivo no
Bloco de Notas; na web o arquivo fica no **servidor**, e o navegador não abre
arquivo de lá — por isso existe a página `ContaController.CaixaDeEntrada`, que lê o
arquivo e mostra o conteúdo. Num sistema real essa página não existiria.

O código tem 6 dígitos, vale 15 minutos (`MINUTOS_VALIDADE_CODIGO`) e é queimado em
`BancoService.MarcarCodigoUsado` depois da troca.

---

## DS03 — Visualizar Cursos

| Mensagem no diagrama | Onde está |
|---|---|
| `visualizarCursos()` | link *Catálogo* no `_Layout.cshtml` |
| `mostrarCursos()` | `CatalogoController.Index` (sem filtro) |
| `listarCursos()` | `CursoService.ExibirCursos` |
| `buscarCursos()` | `BancoService.ListarCursos(FiltroCurso)` |
| `cursosEncontrados()` | retorno `List<Curso>` |
| `exibirCursos(cursos)` | `Views/Catalogo/Index.cshtml` |

---

## DS04 — Buscar Cursos

O diagrama escreve `buscarCurso(termo, **filtros**)`, no plural — era o ponto em que
o programa estava mais atrás do modelo.

| Mensagem no diagrama | Onde está |
|---|---|
| `mostrarResultados()` | formulário de busca em `Views/Catalogo/Index.cshtml` |
| `processarBuscar(termo, filtros)` | `CatalogoViewModel.MontarFiltro` monta o `FiltroCurso` |
| `buscarCurso(termo, filtros)` | `CursoService.BuscarCurso(FiltroCurso)` |
| `localizarCurso(termo, filtros)` | `CursoService.LocalizarCurso(FiltroCurso)` |
| `curosEncontrados(listaCursos)` | retorno `List<Curso>` |
| `exibirResultados(listaCursos)` | `Views/Catalogo/Index.cshtml` |

**O que o `FiltroCurso` carrega:** termo, carga horária mínima, carga máxima,
somente ativos e somente os cursos que o aluno ainda não concluiu. O SQL está em
`BancoService.ListarCursos`, todo por parâmetro `@`.

`BuscarCurso` serve o aluno (só cursos ativos); `LocalizarCurso` serve o tutor
(ativos e inativos).

**Detalhe de web:** o formulário usa `method="get"`, então os filtros ficam no
endereço. A pessoa pode salvar o link da busca ou usar o botão voltar do navegador
— algo que a tela do desktop não oferece.

---

## DS05 — Marcar Aula como Concluída

| Mensagem no diagrama | Onde está |
|---|---|
| `assistirAula(aula)` | link no título da aula, em `Views/Catalogo/Detalhes.cshtml` |
| `abrirAula(aula)` | `CatalogoController.Aula` → `AulaService.AbrirAula` → `BancoService.BuscarAulaPorId` |
| `exibirConteudo()` | `Views/Catalogo/Aula.cshtml` |
| `concluirAula()` | botão da mesma página → `CatalogoController.MarcarAula` |
| `atualizarProgresso(aluno, aula)` | `ProgressoService.AtualizarProgresso` |
| `salvarProgresso(progresso)` | `BancoService.MarcarAulaConcluida` (ou `DesmarcarAulaConcluida`) |
| `confirmacao()` | parâmetro `out afr_mensagem` |
| `progressoAtualizado()` | retorno `bool` |
| `aulaConcluida()` | faixa de aviso (`TempData`) no topo da página |
| `exibirConclusao()` | cartão "Andamento do curso", na mesma página |

**Por que a aula tem página própria:** o modelo pede `exibirConteudo()`, e uma caixa
de seleção na lista não exibe conteúdo. Por isso a aula ganhou a coluna `Conteudo`
na tabela `dbo.Aulas` e a página `Catalogo/Aula`. Quem escreve esse conteúdo é o
tutor, em `Cursos/AulaForm`. A caixa de seleção da lista continua funcionando como
atalho.

---

## DS06 — Fazer Logout

| Mensagem no diagrama | Onde está |
|---|---|
| `clicarLogout()` | link *Sair* no `_Layout.cshtml` |
| `encerrarSessao()` | `ContaController.Logout` |
| `finalizarSessao()` | `HttpContext.SignOutAsync` (apaga o cookie) |
| `sessaoEncerrada()` | `SessaoAtual.EstaAberta()` passa a devolver `false` |
| `retornarLogin()` | `RedirectToAction("Login")` |
| `logoutConcluido()` | `Views/Conta/Login.cshtml` com a faixa "Sessão encerrada" |

A linha de vida **Sessao** do modelo é, na web, a classe `Services/SessaoAtual.cs`.
`AutenticacaoService.EncerrarSessao` ainda é chamado — mas só para registrar a saída
no histórico, **antes** de o cookie ir embora: depois do `SignOutAsync` não haveria
mais como saber quem era.

---

## DS07 — Progresso do aluno

> No Astah este diagrama está salvo com o nome **"Criar Curso"**, mas as mensagens
> dentro dele são todas de progresso e a linha de vida do Tutor aparece nele. Vale
> corrigir o nome no arquivo antes de entregar.

| Mensagem no diagrama | Onde está |
|---|---|
| `acessarProgresso()` | link *Progresso* no `_Layout.cshtml` |
| `solicitarProgresso(aluno)` | `ProgressoController.Index(termo, alunoId)` |
| `buscarProgresso(aluno)` | `ProgressoService.BuscarProgresso` |
| `aulasConcluidas()` | `BancoService.ContarAulasConcluidas` |
| `progresso(dados)` | `List<ProgressoCurso>` de `BuscarProgressoDoAluno` |
| `exibirProgresso(dados)` | `Views/Progresso/Index.cshtml` |
| `mostrarProgresso()` | os três cartões de resumo da mesma página |

A permissão é conferida duas vezes: `[Authorize(Roles = "TUTOR,ADMIN")]` no
controller, e `ProgressoService.ElegivelParaVisualizacao` no serviço — que é o
`elegivelParaVisualizacao` do diagrama. Qualquer pessoa pode ver o próprio
progresso; o de outro aluno é só tutor e admin.

---

## DS08 — Verificar conclusão do curso

> No Astah está salvo como **"Visualizar Curso"**, mas o conteúdo é a verificação de
> conclusão. É o segundo nome a corrigir no arquivo.

| Mensagem no diagrama | Onde está |
|---|---|
| `verificarConclusao(aluno, curso)` | `ProgressoService.VerificarConclusao` |
| `obterProgressoCurso(aluno, curso)` | `ProgressoService.ObterAulasConcluidas` |
| `progressoCurso()` | classe `ProgressoCurso` |
| `obterRequisitosCurso(curso)` | `ProgressoService.ObterRequisitosCurso` |
| `requisitosCurso(dados)` | `Curso.ObterRequisitos()` |
| `resultadoConclusao(status, percentual)` | classe `ResultadoConclusao` + `BancoService.MontarResultado` |
| `exibirResultado(status, percentual)` | `Views/Catalogo/Detalhes.cshtml` e `Views/Catalogo/Aula.cshtml` |

**Por que existe `PercentualMinimo`:** o modelo fala em "requisitos do curso" e em
"status", não em "concluiu tudo ou não". Um curso com `PercentualMinimo = 75` é dado
como concluído com três quartos das aulas. O status sai de
`ResultadoConclusao.ObterStatus()`: `SEM AULAS`, `NÃO INICIADO`, `EM ANDAMENTO` ou
`CONCLUÍDO`.

---

## DS09 — Editar Curso

| Mensagem no diagrama | Onde está |
|---|---|
| `solicitarEdicao(curso)` | botão *Editar* em `Views/Cursos/Index.cshtml` |
| `buscarCurso(curso)` | `CursosController.Editar` → `CursoService.GetDadosCurso` |
| `getDadosCurso(curso)` | `BancoService.BuscarCursoPorId` |
| `dadosCurso(titulo, desc)` | objeto `Curso` devolvido |
| `exibirDadosParaEdicao(dados, curso)` | `Views/Cursos/Form.cshtml` |
| `confirmarEdicao(novosDados)` | `CursosController.Salvar` |
| `realizarAtualizacao(novosDados)` | `CursoService.EditarCurso` |
| `validarEAtualizar(novosDados)` | validações em `EditarCurso` + `BancoService.AtualizarCurso` |
| `atualizacaoSucesso()` | retorno `bool` |
| `exibirMensagemSucesso()` / `confirmacaoSucesso()` | faixa de aviso (`TempData["Sucesso"]`) |

O `Editar` relê o curso no banco em vez de usar o que estava na lista. É o que o
`getDadosCurso` pede, e evita salvar sobre um dado velho se outra pessoa do grupo
mexeu no mesmo registro — risco bem real numa aplicação web, onde várias pessoas
estão dentro do sistema ao mesmo tempo.

---

## DS10 — Excluir Curso

Onze mensagens, e nenhuma delas é "apagar e pronto".

| Mensagem no diagrama | Onde está |
|---|---|
| `solicitarExclusao(curso)` | botão *Excluir* em `Views/Cursos/Index.cshtml` |
| `verificarElegibilidadeExclusao(curso)` | `CursosController.ConfirmarExclusao` → `CursoService.VerificarElegibilidadeExclusao` |
| `elegivelParaExclusao(true, dadosDetalhados)` | classe `ElegibilidadeExclusao` |
| `solicitarConfirmacaoExclusao(dadosDetalhados)` | `Views/Shared/ConfirmarExclusao.cshtml` |
| `confirmarExclusaoFinal(curso)` | `CursosController.Excluir` (POST) |
| `confirmarIdentidadeEAcao(tutor, curso)` | `AutenticacaoService.ConfirmarIdentidade` |
| `realizarExclusaoDefinitiva(curso)` | `CursoService.RemoverDefinitivamente` |
| `removerDefinitivamente(curso)` | `BancoService.ExcluirCurso` |
| `remocaoSucesso()` | retorno `bool` + `out afr_mensagem` |
| `exibirMensagemSucesso()` / `confirmacaoSucesso()` | faixa de aviso |

**Os `dadosDetalhados`:** quantas aulas o curso tem e quantos alunos já têm
progresso nele (`BancoService.ContarAlunosComProgressoNoCurso`). A página mostra
essa lista antes de pedir a senha, para a pessoa saber o tamanho do estrago.

**Detalhe de web:** as duas etapas são duas requisições — um GET que monta a página
de confirmação e um POST que executa. O POST tem `[ValidateAntiForgeryToken]`: sem
isso, outro site poderia disparar a exclusão em nome de quem está logado.

A elegibilidade é recalculada no POST, e não confiada ao que veio do formulário: o
estado pode ter mudado entre abrir a página e enviá-la.

---

## DS11 — Criar Usuário

| Mensagem no diagrama | Onde está |
|---|---|
| `solicitarCadastro(dados)` | `Views/Usuarios/Form.cshtml` → `UsuariosController.Salvar` |
| `verificarDuplicidade(email)` | `UsuarioService.VerificarDuplicidade` |
| `emailDisponivel(true)` | `BancoService.EmailJaExiste` |
| `validaECria(dados)` | `UsuarioService.CriarUsuario` |
| `salvarNovoUsuario(dados)` | `BancoService.InserirUsuario` |
| `exibirMensagemSucesso()` / `sucessoConfirmado()` | faixa de aviso |

A duplicidade é barrada em três lugares: no controller (com a mensagem aparecendo
junto do campo do e-mail), no serviço antes do INSERT, e no banco, que tem
`Email VARCHAR(100) NOT NULL UNIQUE`.

---

## DS12 — Visualizar Usuário

| Mensagem no diagrama | Onde está |
|---|---|
| `realizarBuscarPerfil(admin, usuario)` | botão *Perfil* em `Views/Usuarios/Index.cshtml` |
| `verificarPermissaoAdmin(admin)` | `[Authorize(Roles = "ADMIN")]` + `UsuarioService.VerificarPermissaoAdmin` |
| `solicitarVisualizacaoPerfil(usuario)` | `UsuariosController.Perfil` |
| `elegivelParaVisualizacao(true)` | `VerificarPermissaoAdmin` no início da action |
| `buscarDadosUsuario(usuario)` | `UsuarioService.BuscarDadosUsuario` |
| `dadosUsuario(nome, email, papel, dataCadastro)` | cabeçalho de `Views/Usuarios/Perfil.cshtml` |
| `buscarHistoricoAcoes(usuario)` | `LogService.BuscarLoginLog`, `BuscarAcoesCrud`, `BuscarCursosRelacionados` |
| `historicoAcoes(loginLog, acoesCrud, cursosRelacionados)` | as três listas da página |
| `perfilCompleto(dadosEHistorico)` | `PerfilUsuarioViewModel` |
| `exibirPerfilUsuario(dadosEHistorico)` | `Views/Usuarios/Perfil.cshtml` |

**Por que são três listas e não uma:** o modelo nomeia os três conjuntos
separadamente. Por isso a tabela `dbo.LogAcoes` tem a coluna `Tipo`, com os valores
`LOGIN`, `CRUD` e `CURSO` (constantes em `LogAcao`), e a página mostra uma lista para
cada um.

---

## DS13 — Editar Usuário

| Mensagem no diagrama | Onde está |
|---|---|
| `solicitarEdicaoUsuario(usuario)` | botão *Editar* em `Views/Usuarios/Index.cshtml` |
| `buscarDadosUsuario(usuario)` | `UsuarioService.BuscarDadosUsuario` |
| `getDadosUsuario(usuario)` | `BancoService.BuscarUsuarioPorId` |
| `dadosUsuario(...)` | objeto `Usuario` devolvido |
| `carregarInterface()` | `UsuariosController.Editar` monta o `UsuarioViewModel` |
| `exibirFormularioEdicao(dadosUsuario)` | `Views/Usuarios/Form.cshtml` |
| `confirmarEdicao(novosDados)` | `UsuariosController.Salvar` |
| `realizarAtualizacao(novosDados)` | `UsuarioService.EditarUsuario` |
| `validaEAtualiza(novosDados)` | validações em `EditarUsuario` + `BancoService.AtualizarUsuario` |
| `atualizacaoSucesso()` | retorno `bool` |
| `exibirMensagemSucesso()` / `confirmacaoSucesso()` | faixa de aviso |

---

## DS14 — Excluir Usuário

Mesma estrutura de duas etapas do DS10, agora sobre usuário. A página de
confirmação é literalmente a mesma (`Views/Shared/ConfirmarExclusao.cshtml`): o
campo `afr_Tipo` do ViewModel diz se é curso ou usuário.

| Mensagem no diagrama | Onde está |
|---|---|
| `solicitarExclusaoUsuario(usuario)` | botão *Excluir* em `Views/Usuarios/Index.cshtml` |
| `verificarElegibilidadeExclusao(usuario)` | `UsuariosController.ConfirmarExclusao` → `UsuarioService.VerificarElegibilidadeExclusao` |
| `elegivelParaExclusao(true, dadosDetalhados)` | classe `ElegibilidadeExclusao` |
| `solicitarConfirmacaoExclusao(dadosDetalhados)` | `Views/Shared/ConfirmarExclusao.cshtml` |
| `carregarInterface()` | a mesma view |
| `confirmarExclusaoFinal(usuario)` | `UsuariosController.Excluir` (POST) |
| `confirmarIdentidadeEAcao(admin, usuario)` | `AutenticacaoService.ConfirmarIdentidade` |
| `realizarExclusaoDefinitiva(usuario)` | `UsuarioService.RemoverDefinitivamente` |
| `removerDefinitivamente(usuario)` | `BancoService.ExcluirUsuario` |
| `remocaoSucesso()` | retorno `bool` + `out afr_mensagem` |
| `exibirMensagemSucesso()` / `confirmacaoSucesso()` | faixa de aviso |

**Os `dadosDetalhados` do usuário:** nome, e-mail, perfil, data de cadastro, quantas
aulas ele já concluiu (`ContarProgressoDoUsuario`) e quantos registros de histórico
ele tem (`ContarLogDoUsuario`).

**Dois casos barrados** por `UsuarioService.VerificarElegibilidadeExclusao`, em que a
página abre só como aviso, sem campo de senha:

1. o admin tentando apagar a própria conta;
2. o último administrador ativo do sistema — se ele sair, ninguém mais entra na área
   de administração.

---

## As classes de domínio

Idênticas às do desktop (`Models/Modelo.cs`), com uma exceção.

| Classe | Papel | Veio de |
|---|---|---|
| `Pessoa` | classe base: Id, Nome, Email | modelo |
| `Usuario : Pessoa` | senha, perfil, permissões | modelo (herança e polimorfismo) |
| `Curso` | título, descrição, carga, percentual mínimo | modelo |
| `Aula` | ordem, duração, conteúdo | modelo |
| `Progresso` | aula concluída por um aluno | modelo |
| `FiltroCurso` | o "filtros" do DS04 | DS04 |
| `ResultadoConclusao` | o "resultadoConclusao" do DS08 | DS08 |
| `ProgressoCurso` | o "dados" do DS07 | DS07 |
| `CodigoRecuperacao` | o código do DS02 | DS02 |
| `LogAcao` | o histórico do DS12 | DS12 |
| `ElegibilidadeExclusao` | os "dadosDetalhados" do DS10 e DS14 | DS10, DS14 |

`Pessoa` tem `ObterResumo()` como `virtual` e `Usuario` faz o `override`: é o
polimorfismo pedido na disciplina.

**A exceção: a classe `Sessao` não existe aqui.** No desktop ela era estática — um
usuário logado na memória do programa. Num servidor web isso faria o login de um
aluno sobrescrever o do outro. Virou `Services/SessaoAtual.cs`, um serviço por
requisição que lê o cookie, com os mesmos nomes de método. O comentário no fim de
`Models/Modelo.cs` explica a troca.

---

## O que ainda falta no arquivo do Astah

Isto é sobre a **modelagem**, não sobre o programa. Para a apresentação do grupo,
vale ajustar no `.asta`:

1. **Diagrama de atividades** — o enunciado pede quatro tipos de diagrama e o arquivo
   não tem nenhum.
2. **Classes no diagrama de classes** — o diagrama existe, mas está vazio. As onze
   classes já implementadas podem ser desenhadas nele, com a herança
   `Usuario → Pessoa`.
3. **Atores no diagrama de casos de uso** — os casos de uso estão lá, mas sem os
   bonequinhos de Aluno, Tutor e Admin ligados a eles.
4. **Nomes do DS07 e do DS08** — hoje estão como "Criar Curso" e "Visualizar Curso",
   que não combinam com o conteúdo.

---

## Conferência do código

Como aqui não há compilador disponível, uma conferência automática rodou sobre o
projeto antes da entrega:

- **assinaturas**: as 67 chamadas de controller a serviço, e as dos serviços entre si
  e com o banco, existem e recebem o número certo de argumentos;
- **injeção de dependência**: todo tipo pedido no construtor de um controller ou
  serviço está registrado no `Program.cs`, e o `AddHttpContextAccessor` (de que a
  `SessaoAtual` depende) está presente;
- **rotas**: todo `asp-controller`/`asp-action` das views aponta para uma action que
  existe, e toda action que devolve uma view tem o arquivo `.cshtml` correspondente;
- **views × modelo**: os 153 acessos a `Model.*` batem com as propriedades e métodos
  declarados em `Modelo.cs` e `ViewModels.cs`;
- **segurança**: todo formulário POST tem `@Html.AntiForgeryToken()` e toda action
  `[HttpPost]` tem `[ValidateAntiForgeryToken]`;
- **sintaxe**: chaves e parênteses balanceados nos 13 arquivos `.cs` e nas 21 views,
  e nenhum `@` de e-mail sem escape nas views (que o Razor leria como código).

A compilação e o teste em execução são feitos no Visual Studio, com o SQL Server
LocalDB, conforme o `README.md`.
