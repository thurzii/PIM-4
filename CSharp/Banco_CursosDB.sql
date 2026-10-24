/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: Banco_CursosDB.sql
   ---------------------------------------------------------------------
   O programa JA CRIA o banco, as tabelas e os dados iniciais sozinho,
   na primeira execucao (metodo PrepararBanco, da classe BancoService).
   Ninguem precisa rodar este script para o sistema funcionar.

   Este script existe para tres coisas:
     1. entregar o modelo fisico junto com o trabalho;
     2. permitir montar tudo na mao pelo SQL Server Management Studio;
     3. documentar a MIGRACAO de quem ja tinha o banco da versao 1.

   VERSAO 2 - alinhada ao modelo do Astah. O que mudou em relacao a
   versao 1:
     - Usuarios.Perfil passou a aceitar TUTOR (os diagramas DS07 a DS10
       tem a linha de vida Tutor, nao Admin);
     - Cursos.PercentualMinimo guarda o requisito de conclusao do curso
       (DS08 - obterRequisitosCurso / resultadoConclusao);
     - Aulas.Conteudo guarda o texto que o aluno le (DS05 - abrirAula /
       exibirConteudo);
     - tabela CodigosRecuperacao, para o codigo de 6 digitos da
       recuperacao de senha (DS02 - enviarCodigo / codigoEnviado);
     - tabela LogAcoes, para o historico de acoes do usuario
       (DS12 - buscarHistoricoAcoes / historicoAcoes).

   COMO RODAR NA MAO:
     SQL Server Management Studio -> conectar em (localdb)\MSSQLLocalDB
     ou em .\SQLEXPRESS -> New Query -> colar este arquivo -> Execute.
   ===================================================================== */

-- =====================================================================
-- 1) BANCO DE DADOS
-- =====================================================================
IF DB_ID('CursosDB') IS NULL
    CREATE DATABASE CursosDB;
GO

USE CursosDB;
GO

-- =====================================================================
-- 2) TABELAS
-- =====================================================================

-- ---------------------------------------------------------------------
-- Usuarios: quem entra no sistema.
-- Perfil define o que a pessoa ve: ALUNO, TUTOR ou ADMIN.
-- O Email e UNIQUE: e isso que sustenta o verificarDuplicidade(email)
-- do DS11. Mesmo que alguem burle a tela, o banco recusa o repetido.
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Usuarios','U') IS NULL
CREATE TABLE dbo.Usuarios (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Nome         VARCHAR(100) NOT NULL,
    Email        VARCHAR(100) NOT NULL UNIQUE,   -- nao pode repetir
    Senha        VARCHAR(100) NOT NULL,
    Perfil       VARCHAR(10)  NOT NULL,          -- ALUNO, TUTOR ou ADMIN
    Ativo        BIT          NOT NULL DEFAULT 1,
    DataCadastro DATETIME     NOT NULL DEFAULT GETDATE()
);
GO

-- ---------------------------------------------------------------------
-- Cursos: o catalogo. Ativo = 0 esconde o curso do aluno.
-- PercentualMinimo e o requisito de conclusao: 100 exige todas as
-- aulas, 75 exige tres quartos delas.
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Cursos','U') IS NULL
CREATE TABLE dbo.Cursos (
    Id               INT IDENTITY(1,1) PRIMARY KEY,
    Titulo           VARCHAR(120) NOT NULL,
    Descricao        VARCHAR(500) NULL,
    CargaHoraria     INT          NOT NULL DEFAULT 0,
    PercentualMinimo INT          NOT NULL DEFAULT 100,
    Ativo            BIT          NOT NULL DEFAULT 1,
    DataCadastro     DATETIME     NOT NULL DEFAULT GETDATE()
);
GO

-- ---------------------------------------------------------------------
-- Aulas: pertencem a um curso.
-- Conteudo e o texto da aula (VARCHAR(MAX) porque nao tem tamanho fixo).
-- ON DELETE CASCADE: apagar o curso apaga as aulas dele.
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Aulas','U') IS NULL
CREATE TABLE dbo.Aulas (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    CursoId    INT          NOT NULL,
    Titulo     VARCHAR(120) NOT NULL,
    Ordem      INT          NOT NULL DEFAULT 1,
    DuracaoMin INT          NOT NULL DEFAULT 0,
    Conteudo   VARCHAR(MAX) NULL,
    CONSTRAINT FK_Aulas_Cursos FOREIGN KEY (CursoId)
        REFERENCES dbo.Cursos(Id) ON DELETE CASCADE
);
GO

-- ---------------------------------------------------------------------
-- Progresso: uma linha por aula concluida por um aluno.
-- UQ_Prog impede a mesma aula ser concluida duas vezes pela mesma
-- pessoa. Nao existe coluna "Concluida": a presenca da linha E a
-- conclusao, e desmarcar e um DELETE.
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Progresso','U') IS NULL
CREATE TABLE dbo.Progresso (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId     INT      NOT NULL,
    AulaId        INT      NOT NULL,
    DataConclusao DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Prog_Usuarios FOREIGN KEY (UsuarioId)
        REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Prog_Aulas FOREIGN KEY (AulaId)
        REFERENCES dbo.Aulas(Id) ON DELETE CASCADE,
    CONSTRAINT UQ_Prog UNIQUE (UsuarioId, AulaId)
);
GO

-- ---------------------------------------------------------------------
-- CodigosRecuperacao (DS02): o codigo de 6 digitos da recuperacao.
-- Expiracao diz ate quando vale; Usado = 1 queima o codigo depois da
-- troca, para ele nao servir duas vezes.
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.CodigosRecuperacao','U') IS NULL
CREATE TABLE dbo.CodigosRecuperacao (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId   INT         NOT NULL,
    Codigo      VARCHAR(10) NOT NULL,
    Expiracao   DATETIME    NOT NULL,
    Usado       BIT         NOT NULL DEFAULT 0,
    DataGeracao DATETIME    NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Cod_Usuarios FOREIGN KEY (UsuarioId)
        REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE
);
GO

-- ---------------------------------------------------------------------
-- LogAcoes (DS12): historico de acoes do usuario.
-- Tipo separa as tres listas que o diagrama pede:
--   LOGIN -> loginLog
--   CRUD  -> acoesCrud
--   CURSO -> cursosRelacionados
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.LogAcoes','U') IS NULL
CREATE TABLE dbo.LogAcoes (
    Id        INT IDENTITY(1,1) PRIMARY KEY,
    UsuarioId INT          NOT NULL,
    Tipo      VARCHAR(20)  NOT NULL,            -- LOGIN, CRUD ou CURSO
    Descricao VARCHAR(300) NOT NULL,
    DataHora  DATETIME     NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Log_Usuarios FOREIGN KEY (UsuarioId)
        REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE
);
GO

-- =====================================================================
-- 3) MIGRACAO DA VERSAO 1 PARA A VERSAO 2
-- ---------------------------------------------------------------------
-- Quem ja rodou a versao 1 tem as tabelas antigas. Os comandos abaixo
-- acrescentam o que faltava SEM apagar nada. COL_LENGTH devolve NULL
-- quando a coluna nao existe, entao cada ALTER roda uma vez so e o
-- script pode ser executado quantas vezes quiser.
--
-- O programa faz exatamente isto sozinho, no metodo MigrarEstrutura.
-- =====================================================================
IF COL_LENGTH('dbo.Cursos','PercentualMinimo') IS NULL
    ALTER TABLE dbo.Cursos ADD PercentualMinimo INT NOT NULL DEFAULT 100;
GO

IF COL_LENGTH('dbo.Aulas','Conteudo') IS NULL
    ALTER TABLE dbo.Aulas ADD Conteudo VARCHAR(MAX) NULL;
GO

-- A versao 1 tinha Perfil VARCHAR(5), que cabia ADMIN e ALUNO mas nao
-- cabe TUTOR. Este ALTER alarga a coluna.
IF COL_LENGTH('dbo.Usuarios','Perfil') < 10
    ALTER TABLE dbo.Usuarios ALTER COLUMN Perfil VARCHAR(10) NOT NULL;
GO

-- =====================================================================
-- 4) DADOS INICIAIS
-- ---------------------------------------------------------------------
-- Um usuario de cada perfil, dois cursos e cinco aulas com conteudo.
-- Os INSERT so rodam se a tabela estiver vazia, para o script nao
-- duplicar nada quando for executado de novo.
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios)
BEGIN
    INSERT INTO dbo.Usuarios (Nome, Email, Senha, Perfil) VALUES
        ('Administrador do Sistema','admin@escola.com','admin123','ADMIN'),
        ('Carla Mendes',            'carla@escola.com','tutor123','TUTOR'),
        ('Maria Souza',             'maria@escola.com','aluno123','ALUNO'),
        ('Joao Lima',               'joao@escola.com', 'aluno123','ALUNO');
END
GO

-- Quem veio da versao 1 tem os usuarios, mas nao tem o tutor.
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Perfil = 'TUTOR')
BEGIN
    INSERT INTO dbo.Usuarios (Nome, Email, Senha, Perfil) VALUES
        ('Carla Mendes','carla@escola.com','tutor123','TUTOR');
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Cursos)
BEGIN
    INSERT INTO dbo.Cursos (Titulo, Descricao, CargaHoraria, PercentualMinimo) VALUES
        ('Logica de Programacao',
         'Introducao a algoritmos, variaveis e estruturas de controle.', 40, 100),
        ('Banco de Dados com SQL Server',
         'Modelagem relacional, comandos SQL e consultas.', 60, 75);
END
GO

-- As aulas sao inseridas com SELECT no curso, para pegar o Id certo
-- sem depender de o IDENTITY ter comecado em 1.
IF NOT EXISTS (SELECT 1 FROM dbo.Aulas)
BEGIN
    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin, Conteudo)
    SELECT Id, 'O que e um algoritmo', 1, 45,
           'Um algoritmo e uma sequencia finita de passos que resolve um problema. ' +
           'Nesta aula vemos exemplos do dia a dia e como descrever um passo a passo ' +
           'sem ambiguidade.'
      FROM dbo.Cursos WHERE Titulo = 'Logica de Programacao';

    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin, Conteudo)
    SELECT Id, 'Variaveis e tipos de dados', 2, 50,
           'Variavel e um espaco na memoria com nome. Cada variavel guarda um tipo de ' +
           'dado: inteiro, decimal, texto ou logico. Escolher o tipo certo evita erro ' +
           'de conversao mais tarde.'
      FROM dbo.Cursos WHERE Titulo = 'Logica de Programacao';

    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin, Conteudo)
    SELECT Id, 'Condicoes e repeticoes', 3, 60,
           'Condicao (if) escolhe um caminho; repeticao (for, while) executa o mesmo ' +
           'trecho varias vezes. Juntas, elas dao ao programa a capacidade de decidir ' +
           'e de insistir.'
      FROM dbo.Cursos WHERE Titulo = 'Logica de Programacao';

    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin, Conteudo)
    SELECT Id, 'Modelo relacional', 1, 50,
           'Os dados ficam em tabelas ligadas por chaves. A chave primaria identifica ' +
           'a linha; a chave estrangeira aponta para outra tabela. E o que permite ' +
           'montar o sistema sem repetir informacao.'
      FROM dbo.Cursos WHERE Titulo = 'Banco de Dados com SQL Server';

    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin, Conteudo)
    SELECT Id, 'SELECT, INSERT, UPDATE e DELETE', 2, 70,
           'Os quatro comandos do CRUD. Atencao ao WHERE: sem ele, o UPDATE altera a ' +
           'tabela inteira e o DELETE apaga tudo. Valores sempre por parametro, nunca ' +
           'colados no texto do comando.'
      FROM dbo.Cursos WHERE Titulo = 'Banco de Dados com SQL Server';
END
GO

-- =====================================================================
-- 5) CONFERENCIA
-- ---------------------------------------------------------------------
-- Rode estas consultas depois do script para ver se tudo entrou.
-- =====================================================================
SELECT 'Usuarios' AS Tabela, COUNT(*) AS Registros FROM dbo.Usuarios
UNION ALL SELECT 'Cursos',             COUNT(*) FROM dbo.Cursos
UNION ALL SELECT 'Aulas',              COUNT(*) FROM dbo.Aulas
UNION ALL SELECT 'Progresso',          COUNT(*) FROM dbo.Progresso
UNION ALL SELECT 'CodigosRecuperacao', COUNT(*) FROM dbo.CodigosRecuperacao
UNION ALL SELECT 'LogAcoes',           COUNT(*) FROM dbo.LogAcoes;
GO

-- Os usuarios e o perfil de cada um
SELECT Id, Nome, Email, Perfil, Ativo, DataCadastro
  FROM dbo.Usuarios
 ORDER BY Perfil, Nome;
GO

-- Os cursos com a quantidade de aulas e o requisito de conclusao
SELECT c.Id,
       c.Titulo,
       c.CargaHoraria,
       c.PercentualMinimo,
       COUNT(a.Id) AS TotalAulas,
       SUM(CASE WHEN a.Conteudo IS NULL OR a.Conteudo = '' THEN 1 ELSE 0 END)
           AS AulasSemConteudo
  FROM dbo.Cursos c
  LEFT JOIN dbo.Aulas a ON a.CursoId = c.Id
 GROUP BY c.Id, c.Titulo, c.CargaHoraria, c.PercentualMinimo
 ORDER BY c.Titulo;
GO

-- Progresso de cada aluno em cada curso, com o status de conclusao.
-- E a mesma conta que o metodo BuscarProgressoDoAluno faz no programa
-- (DS07 - buscarProgresso / DS08 - verificarConclusao).
SELECT u.Nome AS Aluno,
       c.Titulo AS Curso,
       COUNT(DISTINCT p.AulaId) AS Concluidas,
       COUNT(DISTINCT a.Id)     AS TotalAulas,
       c.PercentualMinimo,
       CASE
           WHEN COUNT(DISTINCT a.Id) = 0 THEN 'SEM AULAS'
           WHEN COUNT(DISTINCT p.AulaId) * 100 / COUNT(DISTINCT a.Id)
                >= c.PercentualMinimo THEN 'CONCLUIDO'
           WHEN COUNT(DISTINCT p.AulaId) > 0 THEN 'EM ANDAMENTO'
           ELSE 'NAO INICIADO'
       END AS Status
  FROM dbo.Usuarios u
 CROSS JOIN dbo.Cursos c
  LEFT JOIN dbo.Aulas a ON a.CursoId = c.Id
  LEFT JOIN dbo.Progresso p ON p.AulaId = a.Id AND p.UsuarioId = u.Id
 WHERE u.Perfil = 'ALUNO' AND c.Ativo = 1
 GROUP BY u.Nome, c.Titulo, c.PercentualMinimo
 ORDER BY u.Nome, c.Titulo;
GO

-- Historico de acoes (so tem dados depois de usar o programa)
SELECT u.Nome, l.Tipo, l.Descricao, l.DataHora
  FROM dbo.LogAcoes l
  JOIN dbo.Usuarios u ON u.Id = l.UsuarioId
 ORDER BY l.DataHora DESC;
GO
