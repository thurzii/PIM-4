# Rastreabilidade: modelo do Astah x programa em Windows Forms

**Trabalho em grupo — Plataforma de Cursos Online**
Modelo: `Modelagem/Projeto_UML.asta` · Programa: `CSharp/PlataformaCursos`

Este documento liga **cada mensagem** dos 14 diagramas de sequência do Astah ao
método que a implementa no programa. Serve para a apresentação do grupo e para
qualquer pessoa conferir que o código segue o modelo, e não o contrário.

A coluna "Onde" usa `Classe.Metodo`. Os métodos de tela estão em `FrmXxx.cs`, os
de negócio em `Servicos.cs`, os de dados em `BancoService.cs` e as classes de
domínio em `Modelo.cs`.

---

## Como o programa está dividido

```
Telas (FrmXxx)          ->  Serviços (Servicos.cs)  ->  BancoService  ->  SQL Server
10 formulários              7 classes de negócio        ADO.NET           6 tabelas
```

A tela nunca fala com o banco. O serviço nunca monta tela. Essa separação é o que
permite apontar, para cada mensagem do diagrama, um método com o mesmo papel.

**Atores do modelo e perfis do programa**

| Ator no Astah | Perfil no programa | Constante |
|---|---|---|
| Aluno | ALUNO | `Usuario.PERFIL_ALUNO` |
| Tutor | TUTOR | `Usuario.PERFIL_TUTOR` |
| Admin | ADMIN | `Usuario.PERFIL_ADMIN` |

Quem decide o que cada perfil pode fazer são os métodos `PodeGerenciarCursos`,
`PodeGerenciarUsuarios` e `PodeVerProgressoDeOutros`, da classe `Usuario`. As
telas consultam esses métodos pela classe `Sessao`; nenhuma tela compara texto de
perfil na mão.

---

## DS01 — Fazer Login

Linhas de vida no modelo: Aluno, TelaLogin, Sistema, Usuario.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `digitarEmailSenha()` | `FrmLogin` — campos `afr_txtEmail` e `afr_txtSenha` |
| `validarLogin(email, senha)` | `AutenticacaoService.ValidarLogin` |
| `buscarUsuario(email)` | `BancoService.BuscarUsuarioPorEmail` |
| `usuarioValido()` | retorno de `ValidarLogin` (o objeto `Usuario`, ou `null`) |
| `acessoPermitido()` | `Sessao.Abrir(usuario)` |
| `abrirTelaInicial()` | `FrmLogin.afr_btnEntrar_Click` abre o `FrmPrincipal` |

O login também grava o acesso no histórico: `LogService.RegistrarLogin`, que
alimenta o `loginLog` pedido no DS12.

---

## DS02 — Recuperar Senha

Linhas de vida: Aluno, TelaRecuperacao, Sistema, **Email**.

A linha "Email" é o motivo da tela ter três etapas. Antes do modelo, a pessoa
trocava a senha direto, sem código. Agora não dá para pular a etapa do código.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `informarEmail(email)` | `FrmRecuperarSenha` etapa 1 — `afr_txtEmail` |
| `solicitarRecuperacao(email)` | `AutenticacaoService.SolicitarRecuperacao` |
| `enviarCodigo(email)` | `EmailService.EnviarCodigo` |
| `codigoEnviado()` | retorno de `SolicitarRecuperacao` + `BancoService.GravarCodigo` |
| `exibirMensagem()` | `FrmRecuperarSenha.IrParaEtapa(2)` e `afr_lblStatus` |
| `recuperacaoConcluida()` | `AutenticacaoService.RedefinirSenha` |

**Sobre o "Email" do modelo:** o trabalho não tem servidor de e-mail, então a
caixa de entrada é o arquivo `dados/emails.txt`, criado na pasta do programa. O
botão "Ver o código" abre esse arquivo. O resto do fluxo é igual ao modelo: o
código tem 6 dígitos, vale 15 minutos (`MINUTOS_VALIDADE_CODIGO`), é conferido em
`AutenticacaoService.ConferirCodigo` e é queimado em
`BancoService.MarcarCodigoUsado` depois da troca, para não servir duas vezes.

---

## DS03 — Visualizar Cursos

Linhas de vida: Aluno, TelaCursos, Sistema, Curso.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `visualizarCursos()` | `FrmPrincipal.afr_btnCatalogo_Click` |
| `mostrarCursos()` | `FrmCatalogo.ExibirTodosOsCursos` |
| `listarCursos()` | `CursoService.ExibirCursos` |
| `buscarCursos()` | `BancoService.ListarCursos(FiltroCurso)` |
| `cursosEncontrados()` | retorno `List<Curso>` |
| `exibirCursos(cursos)` | `FrmCatalogo.PreencherListaDeCursos` |

---

## DS04 — Buscar Cursos

Linhas de vida: Aluno, TelaBusca, Sistema, Curso.

O diagrama escreve `buscarCurso(termo, **filtros**)`, no plural. Era o ponto em
que o programa estava mais atrás do modelo: a busca só tinha o termo. Os filtros
viraram a classe `FiltroCurso`.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `mostrarResultados()` | `FrmCatalogo.afr_btnBuscar_Click` |
| `processarBuscar(termo, filtros)` | `FrmCatalogo.MontarFiltro` monta o `FiltroCurso` |
| `buscarCurso(termo, filtros)` | `CursoService.BuscarCurso(FiltroCurso)` |
| `localizarCurso(termo, filtros)` | `CursoService.LocalizarCurso(FiltroCurso)` |
| `curosEncontrados(listaCursos)` | retorno `List<Curso>` |
| `exibirResultados(listaCursos)` | `FrmCatalogo.PreencherListaDeCursos` |

**O que o `FiltroCurso` carrega:** termo, carga horária mínima, carga máxima,
somente ativos e somente os cursos que o aluno ainda não concluiu. O SQL que usa
esses campos está em `BancoService.ListarCursos`, tudo por parâmetro `@`.

A diferença entre `BuscarCurso` e `LocalizarCurso` é o público: o aluno vê só os
cursos ativos, o tutor vê ativos e inativos.

---

## DS05 — Marcar Aula como Concluída

Linhas de vida: Aluno, TelaAula, Aula, Progresso, Sistema.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `assistirAula(aula)` | `FrmCatalogo.AbrirAulaSelecionada` abre o `FrmAula` |
| `abrirAula(aula)` | `AulaService.AbrirAula` → `BancoService.BuscarAulaPorId` |
| `exibirConteudo()` | `FrmAula.AbrirAula` preenche `afr_txtConteudo` |
| `concluirAula()` | `FrmAula.afr_btnConcluir_Click` |
| `atualizarProgresso(aluno, aula)` | `ProgressoService.AtualizarProgresso` |
| `salvarProgresso(progresso)` | `BancoService.MarcarAulaConcluida` (ou `DesmarcarAulaConcluida`) |
| `confirmacao()` | parâmetro `out afr_mensagem` |
| `progressoAtualizado()` | retorno `bool` de `AtualizarProgresso` |
| `aulaConcluida()` | `FrmAula.AtualizarBotao` |
| `exibirConclusao()` | `FrmAula.ExibirConclusao` |

**A tela de aula é nova.** O modelo pede `exibirConteudo()`, e uma caixa de
seleção no catálogo não mostra conteúdo nenhum. Por isso a aula ganhou a coluna
`Conteudo` na tabela `dbo.Aulas` e uma tela própria. A caixa de seleção do
catálogo continua funcionando como atalho rápido, mas o caminho do diagrama é
o `FrmAula`.

---

## DS06 — Fazer Logout

Linhas de vida: Sistema, TelaSistema, Sessao, TelaLogin.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `clicarLogout()` | `FrmPrincipal.afr_btnLogout_Click` |
| `encerrarSessao()` | `AutenticacaoService.EncerrarSessao` |
| `finalizarSessao()` | `Sessao.Encerrar` |
| `sessaoEncerrada()` | `Sessao.EstaAberta` passa a devolver `false` |
| `retornarLogin()` | `FrmPrincipal.Close()` devolve o controle ao `FrmLogin` |
| `logoutConcluido()` | `FrmLogin` volta a aparecer com os campos limpos |

A linha de vida "Sessao" do modelo é a classe estática `Sessao`, em `Modelo.cs`.
Depois do logout, qualquer tela que ainda esteja aberta é barrada por
`Sessao.EstaAberta()` no evento `Load`.

---

## DS07 — Progresso do aluno

> **Observação para a apresentação:** no Astah este diagrama está salvo com o
> nome **"Criar Curso"**, mas as mensagens dentro dele são todas de progresso e
> a linha de vida do Tutor aparece nele. Vale corrigir o nome no arquivo antes
> de entregar.

Linhas de vida: Aluno, **Tutor**, TelaProgresso, Sistema, Aula, Progresso, Curso.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `acessarProgresso()` | `FrmPrincipal.afr_btnProgresso_Click` |
| `solicitarProgresso(aluno)` | `FrmProgressoAlunos.afr_btnVer_Click` |
| `buscarProgresso(aluno)` | `ProgressoService.BuscarProgresso` |
| `aulasConcluidas()` | `BancoService.ContarAulasConcluidas` |
| `progresso(dados)` | `List<ProgressoCurso>` devolvida por `BuscarProgressoDoAluno` |
| `exibirProgresso(dados)` | `FrmProgressoAlunos.ExibirProgresso` |
| `mostrarProgresso()` | `afr_lblResumo` com o total e a média de conclusão |

A permissão é conferida duas vezes: a tela só aparece no menu para quem tem
`PodeVerProgressoDeOutros`, e o serviço confere de novo em
`ProgressoService.ElegivelParaVisualizacao` — que é o `elegivelParaVisualizacao`
do diagrama. Qualquer pessoa pode ver o próprio progresso; o de outro aluno é só
tutor e admin.

---

## DS08 — Verificar conclusão do curso

> No Astah este diagrama está salvo como **"Visualizar Curso"**, mas o conteúdo é
> a verificação de conclusão. É o segundo nome a corrigir no arquivo.

Linhas de vida: Aluno, **Tutor**, TelaCurso, Sistema, Curso, Progresso.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `verificarConclusao(aluno, curso)` | `ProgressoService.VerificarConclusao` |
| `obterProgressoCurso(aluno, curso)` | `ProgressoService.ObterAulasConcluidas` |
| `progressoCurso()` | classe `ProgressoCurso` (curso + resultado) |
| `obterRequisitosCurso(curso)` | `ProgressoService.ObterRequisitosCurso` |
| `requisitosCurso(dados)` | `Curso.ObterRequisitos()` |
| `resultadoConclusao(status, percentual)` | classe `ResultadoConclusao` + `BancoService.MontarResultado` |
| `exibirResultado(status, percentual)` | `FrmCatalogo.AtualizarProgresso` e `FrmAula.ExibirConclusao` |

**Por que existe `PercentualMinimo`:** o modelo fala em "requisitos do curso" e
em "status", não em "concluiu tudo ou não". Um curso com `PercentualMinimo = 75`
é dado como concluído com três quartos das aulas. O status sai de
`ResultadoConclusao.ObterStatus()`, que devolve `SEM AULAS`, `NÃO INICIADO`,
`EM ANDAMENTO` ou `CONCLUÍDO`.

---

## DS09 — Editar Curso

Linhas de vida: **Tutor**, TelaGerenciamento, Sistema, Curso.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `solicitarEdicao(curso)` | `FrmCursos.afr_lstCursos_SelectedIndexChanged` |
| `buscarCurso(curso)` | `CursoService.GetDadosCurso` |
| `getDadosCurso(curso)` | `BancoService.BuscarCursoPorId` |
| `dadosCurso(titulo, desc)` | objeto `Curso` devolvido |
| `exibirDadosParaEdicao(dados, curso)` | `FrmCursos.PreencherCampos` |
| `confirmarEdicao(novosDados)` | `FrmCursos.afr_btnSalvarCurso_Click` |
| `realizarAtualizacao(novosDados)` | `CursoService.EditarCurso` |
| `validarEAtualizar(novosDados)` | validações em `EditarCurso` + `BancoService.AtualizarCurso` |
| `atualizacaoSucesso()` | retorno `bool` |
| `exibirMensagemSucesso()` / `confirmacaoSucesso()` | `MessageBox` de sucesso |

O clique na lista relê o curso no banco em vez de usar o objeto que já estava na
tela. É o que o `getDadosCurso` do modelo pede, e evita salvar sobre um dado
velho se outra pessoa do grupo mexeu no mesmo registro.

---

## DS10 — Excluir Curso

Linhas de vida: **Tutor**, TelaGerenciamentoCurso, Sistema, Curso.

Este é o diagrama mais detalhado do modelo: onze mensagens, e nenhuma delas é
"apagar e pronto".

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `solicitarExclusao(curso)` | `FrmCursos.afr_btnExcluirCurso_Click` |
| `verificarElegibilidadeExclusao(curso)` | `CursoService.VerificarElegibilidadeExclusao` |
| `elegivelParaExclusao(true, dadosDetalhados)` | classe `ElegibilidadeExclusao` |
| `solicitarConfirmacaoExclusao(dadosDetalhados)` | `FrmConfirmarExclusao.CarregarInterface` |
| `confirmarExclusaoFinal(curso)` | `FrmConfirmarExclusao.afr_btnConfirmar_Click` |
| `confirmarIdentidadeEAcao(tutor, curso)` | `AutenticacaoService.ConfirmarIdentidade` |
| `realizarExclusaoDefinitiva(curso)` | `CursoService.RemoverDefinitivamente` |
| `removerDefinitivamente(curso)` | `BancoService.ExcluirCurso` |
| `remocaoSucesso()` | retorno `bool` + `out afr_mensagem` |
| `exibirMensagemSucesso()` / `confirmacaoSucesso()` | `MessageBox` de sucesso |

**O que são os `dadosDetalhados`:** a lista do que vai ser apagado junto — quantas
aulas o curso tem e quantos alunos já têm progresso nele
(`BancoService.ContarAlunosComProgressoNoCurso`). A tela mostra essa lista antes
de pedir a senha, para a pessoa saber o tamanho do estrago.

---

## DS11 — Criar Usuário

Linhas de vida: Admin, TelaAdmin, Sistema, Usuario.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `solicitarCadastro(dados)` | `FrmUsuarios.afr_btnSalvar_Click` (modo NOVO) |
| `verificarDuplicidade(email)` | `UsuarioService.VerificarDuplicidade` |
| `emailDisponivel(true)` | `BancoService.EmailJaExiste` |
| `validaECria(dados)` | `UsuarioService.CriarUsuario` |
| `salvarNovoUsuario(dados)` | `BancoService.InserirUsuario` |
| `exibirMensagemSucesso()` / `sucessoConfirmado()` | `MessageBox` de sucesso |

A duplicidade é barrada em três lugares: na tela (aviso direto no campo), no
serviço (antes do INSERT) e no banco (`Email VARCHAR(100) NOT NULL UNIQUE`).

---

## DS12 — Visualizar Usuário

Linhas de vida: Admin, TelaAdminDashboard, Sistema, UsuarioEntidade.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `realizarBuscarPerfil(admin, usuario)` | `FrmUsuarios.afr_btnVerPerfil_Click` |
| `verificarPermissaoAdmin(admin)` | `UsuarioService.VerificarPermissaoAdmin` |
| `solicitarVisualizacaoPerfil(usuario)` | `FrmPerfilUsuario.CarregarPerfil` |
| `elegivelParaVisualizacao(true)` | `VerificarPermissaoAdmin` no `Load` da tela |
| `buscarDadosUsuario(usuario)` | `UsuarioService.BuscarDadosUsuario` |
| `dadosUsuario(nome, email, papel, dataCadastro)` | labels `afr_lblNome`, `afr_lblEmail`, `afr_lblPapel`, `afr_lblCadastro` |
| `buscarHistoricoAcoes(usuario)` | `LogService.BuscarLoginLog`, `BuscarAcoesCrud`, `BuscarCursosRelacionados` |
| `historicoAcoes(loginLog, acoesCrud, cursosRelacionados)` | as três listas da tela |
| `perfilCompleto(dadosEHistorico)` | `afr_lblResumo` com os totais |
| `exibirPerfilUsuario(dadosEHistorico)` | `FrmPerfilUsuario.PreencherLista` |

**Por que são três listas e não uma:** o modelo nomeia os três conjuntos
separadamente. Por isso a tabela `dbo.LogAcoes` tem a coluna `Tipo`, com os
valores `LOGIN`, `CRUD` e `CURSO` (constantes em `LogAcao`), e a tela mostra uma
lista para cada um.

---

## DS13 — Editar Usuário

Linhas de vida: Admin, TelaAdminDashboard, Sistema, UsuarioEntidade.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `solicitarEdicaoUsuario(usuario)` | `FrmUsuarios.afr_lstUsuarios_SelectedIndexChanged` |
| `buscarDadosUsuario(usuario)` | `UsuarioService.BuscarDadosUsuario` |
| `getDadosUsuario(usuario)` | `BancoService.BuscarUsuarioPorId` |
| `dadosUsuario(...)` | objeto `Usuario` devolvido |
| `carregarInterface()` | `FrmUsuarios.PreencherCampos` |
| `exibirFormularioEdicao(dadosUsuario)` | `FrmUsuarios.PreencherCampos` |
| `confirmarEdicao(novosDados)` | `FrmUsuarios.afr_btnSalvar_Click` (modo EDIÇÃO) |
| `realizarAtualizacao(novosDados)` | `UsuarioService.EditarUsuario` |
| `validaEAtualiza(novosDados)` | validações em `EditarUsuario` + `BancoService.AtualizarUsuario` |
| `atualizacaoSucesso()` | retorno `bool` |
| `exibirMensagemSucesso()` / `confirmacaoSucesso()` | `MessageBox` de sucesso |

---

## DS14 — Excluir Usuário

Linhas de vida: Admin, TelaAdminDashboard, Sistema, UsuarioEntidade.

Mesma estrutura de duas etapas do DS10, agora sobre usuário.

| Mensagem no diagrama | Onde está no programa |
|---|---|
| `solicitarExclusaoUsuario(usuario)` | `FrmUsuarios.afr_btnExcluir_Click` |
| `verificarElegibilidadeExclusao(usuario)` | `UsuarioService.VerificarElegibilidadeExclusao` |
| `elegivelParaExclusao(true, dadosDetalhados)` | classe `ElegibilidadeExclusao` |
| `solicitarConfirmacaoExclusao(dadosDetalhados)` | `FrmConfirmarExclusao.CarregarInterface` |
| `carregarInterface()` | `FrmConfirmarExclusao.CarregarInterface` |
| `confirmarExclusaoFinal(usuario)` | `FrmConfirmarExclusao.afr_btnConfirmar_Click` |
| `confirmarIdentidadeEAcao(admin, usuario)` | `AutenticacaoService.ConfirmarIdentidade` |
| `realizarExclusaoDefinitiva(usuario)` | `UsuarioService.RemoverDefinitivamente` |
| `removerDefinitivamente(usuario)` | `BancoService.ExcluirUsuario` |
| `remocaoSucesso()` | retorno `bool` + `out afr_mensagem` |
| `exibirMensagemSucesso()` / `confirmacaoSucesso()` | `MessageBox` de sucesso |

**Os `dadosDetalhados` do usuário:** nome, e-mail, perfil, data de cadastro,
quantas aulas ele já concluiu (`ContarProgressoDoUsuario`) e quantos registros de
histórico ele tem (`ContarLogDoUsuario`).

**Dois casos em que a exclusão é barrada** pelo
`UsuarioService.VerificarElegibilidadeExclusao`, e a tela de confirmação abre só
como aviso, sem campo de senha:

1. o admin tentando apagar a própria conta;
2. o último administrador ativo do sistema — se ele sair, ninguém mais entra na
   área de administração.

---

## As dez telas e os diagramas que cada uma atende

| Tela | Diagramas | Novidade na versão 2 |
|---|---|---|
| `FrmLogin` | DS01, DS02 | — |
| `FrmRecuperarSenha` | DS02 | três etapas com código de 6 dígitos |
| `FrmPrincipal` | DS06 | três áreas, uma por perfil |
| `FrmCatalogo` | DS03, DS04, DS05, DS08 | filtros da busca e botão de abrir a aula |
| `FrmAula` | DS05, DS08 | **tela nova** |
| `FrmCursos` | DS09, DS10 | percentual mínimo, conteúdo da aula, exclusão em 2 etapas |
| `FrmUsuarios` | DS11, DS12, DS13, DS14 | perfil TUTOR, exclusão em 2 etapas, botão de perfil |
| `FrmProgressoAlunos` | DS07 | **tela nova** |
| `FrmPerfilUsuario` | DS12 | **tela nova** |
| `FrmConfirmarExclusao` | DS10, DS14 | **tela nova** |

## As classes de domínio (`Modelo.cs`)

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
| `Sessao` | a linha de vida "Sessao" do DS06 | DS06 |

`Pessoa` tem `ObterResumo()` como `virtual` e `Usuario` faz o `override`: é o
polimorfismo pedido na disciplina, e é o que monta a linha de boas-vindas do menu.

---

## O que ainda falta no arquivo do Astah

Isto é sobre a **modelagem**, não sobre o programa. Para a apresentação do grupo,
vale ajustar no `.asta`:

1. **Diagrama de atividades** — o enunciado pede quatro tipos de diagrama e o
   arquivo não tem nenhum diagrama de atividades.
2. **Classes no diagrama de classes** — o diagrama existe, mas está vazio. As
   onze classes já implementadas em `Modelo.cs` podem ser desenhadas nele, com a
   herança `Usuario -> Pessoa`.
3. **Atores no diagrama de casos de uso** — os casos de uso estão lá, mas sem os
   bonequinhos de Aluno, Tutor e Admin ligados a eles.
4. **Nomes do DS07 e do DS08** — hoje estão como "Criar Curso" e
   "Visualizar Curso", que não combinam com o conteúdo.

---

## Conferência do código

Como o projeto é Windows Forms com `.Designer.cs` escrito à mão, duas
conferências automáticas rodaram sobre o código antes da entrega:

- **assinaturas**: toda chamada de tela a um serviço, e de serviço ao banco,
  existe e recebe o número certo de argumentos (117 métodos, 47 chamadas ao
  banco);
- **telas**: todo controle usado no código existe no `.Designer.cs`, todo evento
  ligado no designer tem método no code-behind, e nenhuma tela chama `Close()`
  dentro do construtor — o que descartaria o formulário antes de ele aparecer e
  derrubaria o `ShowDialog()` seguinte;
- **modelo**: 238 acessos a propriedades e métodos das classes de domínio batem
  com o que está declarado em `Modelo.cs`;
- **SQL**: todo parâmetro `@` usado nos comandos tem o `AddWithValue`
  correspondente — nenhum valor é colado no texto do comando.

A compilação e o teste em execução são feitos no Visual Studio, com o SQL Server
LocalDB, conforme o `LEIAME.txt`.
