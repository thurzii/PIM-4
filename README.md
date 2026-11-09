<<<<<<< HEAD
# Plataforma de Cursos Online - Versão Mobile

Versão mobile do projeto original, desenvolvida em JavaScript.

## Arquitetura

- Mobile: React Native + Expo + React Navigation
- API: Node.js + Express
- Banco: SQL Server (configurado para Azure SQL; também pode usar SQL Server local)
- Comunicação: HTTP + JSON
- Autenticação: JWT
- Persistência local: AsyncStorage

Fluxo:

`Aplicativo Mobile -> REST API -> SQL Server`

## Funcionalidades

### Aluno
- Login
- Recuperação de senha demonstrativa
- Catálogo e busca de cursos
- Detalhes do curso e lista de aulas
- Marcar/desmarcar aula concluída
- Percentual de progresso sincronizado com o banco
- Perfil e logout

### Administrador
- Login por perfil
- Cadastro, edição e exclusão de cursos
- Cadastro e exclusão de aulas
- Ativar/desativar cursos
- Listagem e busca de usuários
- Ativação/desativação de usuários

## Usuários de demonstração

Os usuários abaixo só funcionarão se já estiverem cadastrados no banco conectado à API:

- ADMIN: `admin@escola.com` / `admin123`
- ALUNO: `maria@escola.com` / `aluno123`
- ALUNO: `joao@escola.com` / `aluno123`

## 1. Configurar o banco de dados na nuvem (Azure SQL)

O projeto está preparado com estes valores iniciais:

- Servidor: `database-pim-iv.database.windows.net`
- Porta: `1433`
- Banco provisório: `banco_pim`
- Usuário: `maria`

**Importante:** `banco_pim` é apenas um nome provisório. Quando você confirmar o nome real do banco no portal Azure, altere `DB_DATABASE` no arquivo `api/.env`. O banco precisa existir no servidor e conter as tabelas esperadas pela API: `Usuarios`, `Cursos`, `Aulas` e `Progresso`.

No portal Azure, confira também se o firewall do servidor SQL permite conexões a partir do IP público da máquina onde a API será executada. Não libere acesso irrestrito (`0.0.0.0` a `255.255.255.255`) apenas para testar.

## 2. Configurar e iniciar a API

Abra um terminal na pasta `api` e instale as dependências:

```bash
npm install
```

Copie `api/.env.example` para `api/.env`. No Windows PowerShell, estando na pasta `api`, você pode usar:

```powershell
Copy-Item .env.example .env
```

Abra `api/.env` e confira os valores. Substitua `DB_PASSWORD` pela senha do usuário SQL. Não envie o arquivo `.env` ao GitHub nem compartilhe a senha.

```env
PORT=3000
JWT_SECRET=troque-por-uma-chave-longa-e-aleatoria
DB_SERVER=database-pim-iv.database.windows.net
DB_PORT=1433
DB_DATABASE=banco_pim
DB_USER=maria
DB_PASSWORD=COLOQUE_SUA_SENHA_AQUI
=======
# Plataforma de Cursos Online

Aplicativo mobile para uma plataforma de cursos, desenvolvido em
JavaScript. O projeto reúne aplicativo React Native, API REST em Node.js
e banco de dados SQL Server.

## Sumário

-   [Visão geral](#visão-geral)
-   [Tecnologias e arquitetura](#tecnologias-e-arquitetura)
-   [Funcionalidades por perfil](#funcionalidades-por-perfil)
-   [Requisitos](#requisitos)
-   [Configuração do banco de dados](#configuração-do-banco-de-dados)
-   [Configuração e execução da API](#configuração-e-execução-da-api)
-   [Configuração e execução do
    aplicativo](#configuração-e-execução-do-aplicativo)
-   [Modo de demonstração](#modo-de-demonstração)
-   [Usuários de demonstração no
    banco](#usuários-de-demonstração-no-banco)
-   [Testes](#testes)
-   [Regras de acesso e segurança](#regras-de-acesso-e-segurança)
-   [Roteiro de validação manual](#roteiro-de-validação-manual)
-   [Etapas de desenvolvimento](#etapas-de-desenvolvimento)
-   [Limitações conhecidas e próximos
    cuidados](#limitações-conhecidas-e-próximos-cuidados)
-   [Apresentação do projeto](#apresentação-do-projeto)

------------------------------------------------------------------------

## Visão geral

A plataforma permite que alunos encontrem cursos, realizem matrículas,
acompanhem o próprio progresso e recebam pontos de experiência (XP) por
estudar. Professores gerenciam seus próprios cursos e acompanham os
alunos matriculados. Administradores gerenciam usuários, cursos e aulas
em âmbito global.

A aplicação utiliza uma API para autenticação, autorização e regras de
negócio. O banco SQL Server armazena os usuários, cursos, aulas,
matrículas, progresso e dados de gamificação.

## Tecnologias e arquitetura

  Camada                             Tecnologia
  ---------------------------------- ---------------------------------------
  Aplicativo mobile                  React Native, Expo e React Navigation
  API                                Node.js e Express
  Banco de dados                     Microsoft SQL Server / Azure SQL
  Comunicação                        HTTP e JSON
  Autenticação                       JWT
  Persistência de sessão no mobile   AsyncStorage

Fluxo principal:

``` text
Aplicativo Mobile (React Native)
              |
              | HTTP / JSON
              v
         API (Express)
              |
              | SQL
              v
       SQL Server / Azure SQL
```

A API e o aplicativo possuem arquivos `package.json` independentes e
devem ter suas dependências instaladas separadamente.

## Funcionalidades por perfil

### ALUNO

-   Login e encerramento de sessão.
-   Catálogo de cursos ativos.
-   Visualização dos detalhes e das aulas de um curso.
-   Matrícula em cursos.
-   Marcação e desmarcação de aulas concluídas.
-   Acompanhamento do percentual de progresso.
-   Painel de gamificação com XP, nível, sequência de estudos e
    conquistas.

### PROFESSOR

-   Área exclusiva **Meus cursos**.
-   Criação e edição dos próprios cursos.
-   Cadastro e exclusão de aulas dos próprios cursos.
-   Consulta dos alunos matriculados.
-   Acompanhamento do progresso dos alunos nos próprios cursos.

### ADMIN

-   Gerenciamento global de cursos e aulas.
-   Cadastro de usuários com perfil `ALUNO`, `PROFESSOR` ou `ADMIN`.
-   Pesquisa e consulta de usuários.
-   Ativação e desativação de usuários.
-   Gerenciamento administrativo dos cursos, independentemente do
    professor responsável.

## Requisitos

-   Node.js e npm instalados.
-   SQL Server ou Azure SQL acessível, caso seja utilizado o modo
    conectado ao banco.
-   Expo Go em um celular físico ou emulador Android configurado.
-   Acesso à rede entre o dispositivo e a máquina que executa a API.

Confira a versão do Node.js e do npm:

``` powershell
node --version
npm --version
```

## Configuração do banco de dados

### Banco novo

O arquivo `sql/CursosDB.sql` contém o script de criação da estrutura
inicial e dados demonstrativos. Ele também contém instruções para criar
e selecionar o banco `CursosDB`.

Use esse script somente quando estiver preparando um banco novo e tiver
confirmado o destino. **Não execute o script integralmente em um banco
que já esteja em uso**, pois ele contém comandos de criação de banco e
inserção de dados de demonstração.

### Banco já existente

Se as tabelas originais do projeto já existem, aplique somente as
migrações necessárias, na ordem correspondente às funcionalidades que
serão utilizadas:

1.  `sql/Etapa1_Professor.sql` --- adiciona o vínculo entre cursos e
    professores e a tabela de matrículas.
2.  `sql/Etapa4_Gamificacao.sql` --- cria as tabelas e os dados iniciais
    de gamificação.

Execute cada script conectado ao banco configurado para a aplicação.
Faça backup antes de qualquer alteração de estrutura ou dados.

O script `sql/DiagnosticoBanco.sql` é somente de consulta. Ele verifica
o banco atual, as colunas esperadas, relacionamentos principais e
quantidade de registros. Execute-o no banco que a API realmente
utilizará.

### Estrutura esperada

A API utiliza, entre outras, as seguintes tabelas e estruturas:

-   `Usuarios`: identificação, nome, e-mail, senha, perfil e status.
-   `Cursos`: título, descrição, carga horária, status, data de cadastro
    e professor responsável.
-   `Aulas`: curso relacionado, título, ordem e duração.
-   `Matriculas`: vínculo entre aluno e curso e data da matrícula.
-   `Progresso`: vínculo entre aluno e aula e data de conclusão.
-   `Gamificacao`: saldo de XP, nível e dados de sequência de estudos.
-   `Conquistas`: catálogo de conquistas.
-   `UsuarioConquistas`: conquistas liberadas por usuário.
-   `HistoricoXP`: registro da origem das pontuações.

O endpoint `GET /api/health` verifica a conexão e as colunas
obrigatórias. Em uma instalação completa, o resultado esperado é:

``` json
{
  "api": "ok",
  "database": "ok",
  "schema": "ok"
}
```

A resposta real pode incluir campos adicionais. Se `database` estiver
como `error`, verifique conexão, credenciais, firewall e disponibilidade
do SQL Server. Se `database` estiver como `ok`, mas `schema` estiver
como `error`, confira as migrações e as colunas ausentes informadas pela
API.

### Azure SQL

A configuração do servidor e do banco deve ser feita no arquivo
`api/.env`. Os valores do `.env.example` são exemplos e precisam ser
conferidos com os dados reais fornecidos pela equipe.

No portal Azure, verifique se a regra de firewall permite conexões a
partir do IP público da máquina que executará a API. Evite liberar
acesso irrestrito ao servidor apenas para facilitar testes.

## Configuração e execução da API

Abra um terminal na pasta `api`:

``` powershell
cd api
npm install
```

Crie o arquivo `.env` a partir do exemplo:

``` powershell
Copy-Item .env.example .env
```

Edite `api/.env` com os dados corretos do ambiente. Exemplo de
configuração:

``` env
PORT=3000
JWT_SECRET=SUBSTITUA_POR_UMA_CHAVE_LONGA_E_ALEATORIA
DB_SERVER=SEU_SERVIDOR_SQL
DB_PORT=1433
DB_DATABASE=SEU_BANCO
DB_USER=SEU_USUARIO
DB_PASSWORD=SUA_SENHA
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
DB_ENCRYPT=true
DB_TRUST_SERVER_CERTIFICATE=false
DB_INSTANCE=
```

<<<<<<< HEAD
Depois inicie a API:

```bash
npm run dev
```

Para verificar a conexão, abra `http://localhost:3000/api/health` no navegador. Se a API responder com `database: "ok"`, a conexão com o banco foi estabelecida. Se houver erro, confira o nome real do banco, as credenciais e a regra de firewall do Azure.

### Usar SQL Server local (alternativa)

Se precisar voltar para um SQL Server local, ajuste o `api/.env`, por exemplo:

```env
=======
Ajuste os parâmetros de acordo com o método de autenticação e o servidor
utilizados. Não compartilhe o arquivo `.env`, não envie credenciais para
repositórios e não coloque senhas do banco no aplicativo mobile.

Inicie a API:

``` powershell
npm run dev
```

Para conferir a disponibilidade, acesse:

``` text
http://localhost:3000/api/health
```

Mantenha o terminal da API aberto enquanto estiver utilizando o
aplicativo no modo conectado.

### SQL Server local

Também é possível configurar um SQL Server local. Os parâmetros dependem
da instalação e do método de autenticação disponíveis na máquina. Um
exemplo de referência para LocalDB é:

``` env
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
DB_SERVER=localhost
DB_DATABASE=CursosDB
DB_USER=
DB_PASSWORD=
DB_ENCRYPT=false
DB_TRUST_SERVER_CERTIFICATE=true
DB_INSTANCE=MSSQLLocalDB
```

<<<<<<< HEAD
A configuração local depende da instalação e do método de autenticação disponível na máquina.

## 3. Iniciar o aplicativo mobile

Entre na pasta `mobile` e instale as dependências:

```bash
npm install
npx expo start
```

Confira `mobile/src/services/api.js` e configure o endereço da API conforme o dispositivo:

- Emulador Android: `http://10.0.2.2:3000/api`
- Celular físico: `http://IP-DA-MAQUINA:3000/api`, com o celular e o computador na mesma rede.

O aplicativo conversa com a API; as credenciais do banco ficam exclusivamente no backend.

## 4. Banco e estrutura esperada

O arquivo `sql/CursosDB.sql` contém o script de criação do banco usado originalmente pelo projeto. Use-o apenas se precisar criar a estrutura em um banco novo e tiver autorização para executar esse script. Antes de rodá-lo no Azure, confirme o banco de destino para evitar criar objetos no lugar errado.

## 5. Apresentação da etapa mobile

Fluxo sugerido:

`Login -> Catálogo -> Curso -> Aulas -> Concluir aula -> Progresso`

Demonstre também:

`Login Admin -> Administração -> Cursos/Usuários -> API -> Banco`

## Observação de segurança

O sistema original armazena senhas de usuários em texto simples para fins de demonstração. Em um sistema real, use hash seguro de senha, HTTPS, uma chave JWT forte e fluxo de recuperação de senha com verificação. O usuário SQL deve ter apenas as permissões necessárias para a aplicação.
=======
Confirme a compatibilidade dessa configuração com o driver e a
autenticação utilizados no ambiente.

## Configuração e execução do aplicativo

Abra outro terminal na pasta `mobile`:

``` powershell
cd mobile
npm install
```

Crie o arquivo de ambiente:

``` powershell
Copy-Item .env.example .env
```

Configure `EXPO_PUBLIC_API_URL` em `mobile/.env`.

**Emulador Android:**

``` env
EXPO_PUBLIC_API_URL=http://10.0.2.2:3000/api
```

**Celular físico com Expo Go:**

``` env
EXPO_PUBLIC_API_URL=http://IP-LOCAL-DO-COMPUTADOR:3000/api
```

No celular físico, substitua o endereço pelo IP local do computador que
está executando a API. Os dois dispositivos precisam estar na mesma rede
Wi-Fi, e o firewall do computador deve permitir a comunicação na porta
utilizada pela API.

Inicie o Expo:

``` powershell
npx expo start -c
```

O parâmetro `-c` limpa o cache do Metro. Se o endereço da API for
alterado no `.env`, reinicie o Expo para carregar a configuração.

**Importante:** o arquivo `.env` não deve ser substituído por uma versão
de exemplo sem antes preservar os valores locais. Os arquivos `.env` não
devem ser enviados em ZIP ou repositórios compartilhados.

## Modo de demonstração

O aplicativo mobile possui um modo de demonstração local para permitir a
navegação e os testes básicos sem depender da disponibilidade do SQL
Server.

Credenciais do modo de demonstração:

  Perfil      E-mail                   Senha
  ----------- ------------------------ ----------------
  ALUNO       `maria@escola.com`       `aluno123`
  PROFESSOR   `professor@escola.com`   `professor123`
  ADMIN       `admin@escola.com`       `admin123`

No modo de demonstração, os dados são simulados no próprio aplicativo.
Eles não são gravados no SQL Server e podem ser reiniciados quando o
aplicativo é fechado ou reiniciado.

O modo de demonstração serve para apresentar e testar a navegação e os
fluxos implementados localmente. Ele não substitui a validação da API,
das permissões no servidor ou da persistência no banco de dados. Algumas
operações administrativas ou de professor podem não estar disponíveis
nesse modo.

Para validar o funcionamento integrado, utilize o modo conectado à API e
um banco preparado com as tabelas e os usuários necessários.

## Usuários de demonstração no banco

Estes usuários são destinados ao modo conectado à API e **só funcionarão
se estiverem cadastrados e ativos no banco utilizado pela aplicação**:

  Perfil      E-mail                   Senha
  ----------- ------------------------ ----------------
  ADMIN       `admin@escola.com`       `admin123`
  ALUNO       `maria@escola.com`       `aluno123`
  ALUNO       `joao@escola.com`        `aluno123`
  PROFESSOR   `professor@escola.com`   `professor123`

O usuário professor é incluído pela preparação da Etapa 1 ou pode ser
criado pela área administrativa. A criação dos usuários de demonstração
depende do script utilizado e do estado do banco.

As credenciais acima são apenas para demonstração. Não as utilize em um
ambiente público ou de produção.

## Testes

### Testes automatizados da API

Na pasta `api`, instale as dependências e execute:

``` powershell
npm install
npm test
```

Também é possível executar as suítes separadamente:

``` powershell
npm run test:unit
npm run test:integration
```

A suíte unitária verifica guards de autorização e regras de acesso. A
suíte de integração inicia a aplicação Express e envia requisições HTTP
reais, utilizando uma camada de banco simulada em memória.

A cobertura documentada inclui:

-   Health check e validação do schema esperado.
-   Login válido, senha incorreta e usuário inativo.
-   Bloqueio de rotas protegidas sem token.
-   Catálogo e filtro de cursos inativos.
-   Restrições de acesso por perfil.
-   Consulta de cursos do professor.
-   Bloqueio de progresso antes da matrícula.
-   Matrícula e conclusão de aula.
-   Atualização do percentual de progresso.
-   Validação do campo `concluida`.
-   Recuperação de senha mantida bloqueada até existir um fluxo seguro.
-   XP, conquistas e prevenção de pontuação duplicada.
-   Resposta 404 para rotas inexistentes.

Os testes de integração utilizam banco simulado. Eles não exigem
credenciais reais e não escrevem no banco configurado no `.env`.
Portanto, não substituem a validação das migrações e dos fluxos no SQL
Server/Azure SQL.

### Validação mobile

A interface React Native deve ser testada manualmente no Expo/Expo Go ou
emulador. Confira login, navegação por perfil, carregamento, mensagens
de erro, matrícula, progresso e gamificação.

## Regras de acesso e segurança

A API aplica as permissões no backend. A separação de telas no
aplicativo melhora a experiência, mas não é considerada uma barreira de
segurança.

Regras principais:

  -----------------------------------------------------------------------
  Recurso           ADMIN             PROFESSOR         ALUNO
  ----------------- ----------------- ----------------- -----------------
  Gerenciar         Sim               Não               Não
  usuários                                              

  Gerenciar         Sim               Não               Não
  qualquer curso                                        

  Criar cursos      Sim, pela área    Sim               Não
  próprios          administrativa                      

  Editar cursos     Qualquer curso    Somente os        Não
                                      próprios          

  Gerenciar aulas   Qualquer curso    Somente dos       Não
                                      próprios cursos   

  Consultar alunos  Administração do  Alunos dos        Somente o próprio
  e progresso       sistema           próprios cursos   progresso

  Consultar         Não pela rota de  Não pela rota de  Sim
  catálogo de       aluno             aluno             
  cursos                                                

  Matricular-se     Não               Não               Sim

  Marcar aula       Não               Não               Sim, se estiver
  concluída                                             matriculado
  -----------------------------------------------------------------------

Proteções adicionais implementadas:

-   Rotas administrativas exigem token válido e perfil `ADMIN`.
-   Rotas de professor exigem token válido e perfil `PROFESSOR`.
-   Matrícula e progresso exigem perfil `ALUNO`.
-   O professor só acessa cursos vinculados ao seu próprio ID.
-   O ID do professor é obtido do usuário autenticado, não do corpo da
    requisição.
-   A matrícula é única por combinação de aluno e curso.
-   O backend consulta o usuário atual no banco em cada requisição
    autenticada, verificando perfil e status da conta.
-   Um administrador não pode desativar a própria conta pela tela de
    usuários nem desativar o último administrador ativo.
-   A exclusão de aula pelo professor remove os registros relacionados
    de progresso e a aula dentro de uma transação, após validar a
    propriedade do curso.
-   A redefinição de senha aberta apenas por e-mail foi bloqueada. A
    recuperação só deve ser reativada quando houver verificação segura
    de identidade.

## Roteiro de validação manual

Após configurar banco, API e aplicativo, execute os testes abaixo.

### Autenticação e navegação

1.  Entrar como ALUNO e confirmar que o catálogo é carregado.
2.  Encerrar a sessão e entrar com outro perfil.
3.  Fechar e reabrir o aplicativo e conferir o comportamento da sessão.
4.  Confirmar que cada perfil visualiza as áreas correspondentes.

### Fluxo do aluno

1.  Localizar um curso ativo.
2.  Abrir os detalhes e consultar as aulas.
3.  Realizar a matrícula.
4.  Marcar uma aula como concluída.
5.  Conferir o percentual de progresso.
6.  Desmarcar e marcar novamente a mesma aula e confirmar que não há XP
    duplicado.
7.  Conferir XP, nível, sequência e conquistas no perfil.

### Fluxo do professor

1.  Entrar como PROFESSOR.
2.  Criar um curso com pelo menos duas aulas.
3.  Editar os dados do curso.
4.  Cadastrar e excluir uma aula.
5.  Entrar como ALUNO e realizar matrícula no curso.
6.  Voltar ao perfil PROFESSOR e conferir o aluno matriculado e seu
    progresso.
7.  Criar ou utilizar um segundo professor e confirmar que ele não
    acessa nem altera o curso do primeiro.

### Fluxo do administrador

1.  Entrar como ADMIN.
2.  Cadastrar um usuário e selecionar o perfil desejado.
3.  Pesquisar um usuário.
4.  Ativar ou desativar uma conta.
5.  Cadastrar, editar e desativar cursos.
6.  Gerenciar aulas.
7.  Confirmar que um usuário desativado não consegue autenticar-se.

### Permissões da API

1.  Tentar acessar `/api/admin/usuarios` com ALUNO: esperado `403`.
2.  Tentar acessar `/api/admin/cursos` com PROFESSOR: esperado `403`.
3.  Tentar acessar `/api/professor/cursos` com ALUNO: esperado `403`.
4.  Tentar acessar um curso de outro professor: a API deve negar o
    acesso ou retornar `404`, conforme a rota.
5.  Tentar atualizar o progresso sem matrícula: a API deve bloquear a
    operação.
6.  Desativar um usuário e confirmar que o token antigo deixa de
    autorizar novas requisições autenticadas.

## Etapas de desenvolvimento

### Etapa 0 --- Auditoria e estabilização

-   Auditoria estática da API, configuração SQL Server, estrutura
    esperada do banco, aplicativo mobile e documentação.
-   Ajuste do endpoint `/api/health` para verificar colunas esperadas no
    schema `dbo`.
-   Inclusão do script `sql/DiagnosticoBanco.sql`.
-   Documentação das configurações de ambiente e dos testes manuais.

**Validação pendente:** conexão com o banco definitivo, conferência do
schema e execução dos fluxos no dispositivo da apresentação.

### Etapa 1 --- Perfil PROFESSOR

-   Implementação do perfil `PROFESSOR`.
-   Criação e edição de cursos próprios.
-   Cadastro e exclusão de aulas dos próprios cursos.
-   Consulta de alunos matriculados e progresso.
-   Inclusão de `Cursos.ProfessorId` e da tabela `Matriculas`.

**Migração:** `sql/Etapa1_Professor.sql`, para banco existente.

**Limitação:** cursos antigos com `ProfessorId = NULL` não são
atribuídos automaticamente a um professor.

### Etapa 2 --- Matriz de permissões

-   Centralização das verificações de perfil e acesso.
-   Proteção das rotas de administrador, professor e aluno.
-   Consulta do status e perfil atual do usuário no banco a cada
    requisição autenticada.
-   Proteção contra desativação da própria conta do administrador e do
    último administrador ativo.
-   Bloqueio temporário da recuperação insegura de senha.

### Etapa 3 --- Testes automatizados

-   Testes unitários dos middlewares e das permissões.
-   Testes de integração HTTP da API com banco simulado em memória.
-   Comandos separados para testes unitários e de integração.
-   Cobertura de autenticação, autorização, cursos, matrícula, progresso
    e rotas inexistentes.

### Etapa 4 --- Gamificação

-   10 XP por primeira conclusão de aula.
-   Um nível a cada 100 XP; nível inicial 1.
-   Sequência atual e melhor sequência de dias de estudo.
-   Cinco conquistas iniciais.
-   Histórico de XP e proteção contra pontuação duplicada.
-   Painel de gamificação no perfil do aluno.

**Migração obrigatória:** `sql/Etapa4_Gamificacao.sql`, para banco
existente.

## Limitações conhecidas e próximos cuidados

-   A validação estática e os testes com banco simulado não comprovam
    que o banco definitivo esteja configurado corretamente. Execute os
    scripts e valide `/api/health` no ambiente da equipe.
-   A interface mobile precisa ser validada no dispositivo ou emulador
    utilizado na apresentação.
-   A recuperação automática de senha está desabilitada até que exista
    um fluxo com verificação de identidade, como token de uso único.
-   A documentação das etapas registra que as senhas ainda seguem o
    modelo anterior do projeto. Antes de uso real, implemente hash
    seguro de senhas.
-   Antes de produção, configure HTTPS, segredo JWT forte e proteção
    adequada das credenciais do banco.
-   Cursos antigos sem `ProfessorId` não são associados automaticamente
    a professores.
-   O modo de demonstração utiliza dados locais simulados e não comprova
    persistência no banco.
-   As suítes automatizadas utilizam banco simulado; a validação real no
    SQL Server continua necessária.
-   As etapas documentam verificações estáticas e testes automatizados,
    mas não afirmam que todos os testes de banco e dispositivo foram
    executados no ambiente final.

## Apresentação do projeto

Fluxo sugerido para demonstrar a experiência do aluno:

``` text
Login ALUNO
    -> Catálogo
    -> Detalhes do curso
    -> Matrícula
    -> Aulas
    -> Concluir aula
    -> Progresso e gamificação
```

Fluxo sugerido para demonstrar a administração:

``` text
Login ADMIN
    -> Gerenciamento de usuários
    -> Gerenciamento de cursos e aulas
    -> API
    -> SQL Server
```

Demonstre também o perfil PROFESSOR:

``` text
Login PROFESSOR
    -> Meus cursos
    -> Criar/editar curso
    -> Gerenciar aulas
    -> Consultar alunos matriculados
    -> Acompanhar progresso
```

------------------------------------------------------------------------

**Importante:** este README reúne a documentação das Etapas 0 a 4 e as
instruções gerais do projeto. Mantenha os arquivos `.env` fora de
compartilhamentos e confirme o banco de destino antes de executar
qualquer script SQL.
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
