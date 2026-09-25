/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: Banco_CursosDB.sql
   ---------------------------------------------------------------------
   O programa JA CRIA o banco, as tabelas e os dados iniciais sozinho,
   na primeira execucao (metodo PrepararBanco, da classe BancoService).

   Este script existe para entregar junto com o trabalho e para poder
   montar tudo na mao pelo SQL Server Management Studio, se o professor
   pedir. Tambem serve como documentacao do modelo fisico.
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

-- Usuarios: quem entra no sistema. Perfil define o que a pessoa ve.
IF OBJECT_ID('dbo.Usuarios','U') IS NULL
CREATE TABLE dbo.Usuarios (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Nome         VARCHAR(100) NOT NULL,
    Email        VARCHAR(100) NOT NULL UNIQUE,   -- nao pode repetir
    Senha        VARCHAR(100) NOT NULL,
    Perfil       VARCHAR(10)  NOT NULL,          -- ADMIN ou ALUNO
    Ativo        BIT          NOT NULL DEFAULT 1,
    DataCadastro DATETIME     NOT NULL DEFAULT GETDATE()
);
GO

-- Cursos: o catalogo. Ativo = 0 esconde o curso do aluno.
IF OBJECT_ID('dbo.Cursos','U') IS NULL
CREATE TABLE dbo.Cursos (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    Titulo       VARCHAR(120) NOT NULL,
    Descricao    VARCHAR(500) NULL,
    CargaHoraria INT          NOT NULL DEFAULT 0,
    Ativo        BIT          NOT NULL DEFAULT 1,
    DataCadastro DATETIME     NOT NULL DEFAULT GETDATE()
);
GO

-- Aulas: cada aula pertence a UM curso (relacao 1 para N).
-- ON DELETE CASCADE: apagar o curso apaga as aulas dele.
IF OBJECT_ID('dbo.Aulas','U') IS NULL
CREATE TABLE dbo.Aulas (
    Id         INT IDENTITY(1,1) PRIMARY KEY,
    CursoId    INT          NOT NULL,
    Titulo     VARCHAR(120) NOT NULL,
    Ordem      INT          NOT NULL DEFAULT 1,
    DuracaoMin INT          NOT NULL DEFAULT 0,
    CONSTRAINT FK_Aulas_Cursos FOREIGN KEY (CursoId)
        REFERENCES dbo.Cursos(Id) ON DELETE CASCADE
);
GO

-- Progresso: tabela associativa entre Usuarios e Aulas (N para N).
-- A restricao UNIQUE impede gravar a mesma aula duas vezes.
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

-- =====================================================================
-- 3) DADOS INICIAIS
-- =====================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios)
BEGIN
    INSERT INTO dbo.Usuarios (Nome, Email, Senha, Perfil) VALUES
        ('Administrador do Sistema','admin@escola.com','admin123','ADMIN'),
        ('Maria Souza','maria@escola.com','aluno123','ALUNO'),
        ('Joao Lima','joao@escola.com','aluno123','ALUNO');

    INSERT INTO dbo.Cursos (Titulo, Descricao, CargaHoraria) VALUES
        ('Logica de Programacao',
         'Introducao a algoritmos, variaveis e estruturas de controle.', 40),
        ('Banco de Dados com SQL Server',
         'Modelagem relacional, comandos SQL e consultas.', 60);

    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin)
    SELECT Id, 'O que e um algoritmo', 1, 45 FROM dbo.Cursos
     WHERE Titulo = 'Logica de Programacao';

    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin)
    SELECT Id, 'Variaveis e tipos de dados', 2, 50 FROM dbo.Cursos
     WHERE Titulo = 'Logica de Programacao';

    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin)
    SELECT Id, 'Condicoes e repeticoes', 3, 60 FROM dbo.Cursos
     WHERE Titulo = 'Logica de Programacao';

    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin)
    SELECT Id, 'Modelo relacional', 1, 50 FROM dbo.Cursos
     WHERE Titulo = 'Banco de Dados com SQL Server';

    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin)
    SELECT Id, 'SELECT, INSERT, UPDATE e DELETE', 2, 70 FROM dbo.Cursos
     WHERE Titulo = 'Banco de Dados com SQL Server';
END
GO

-- =====================================================================
-- 4) CONSULTAS USADAS PELO PROGRAMA (para conferencia na apresentacao)
-- =====================================================================

-- Login (DS01): procura o usuario pelo email
SELECT Id, Nome, Email, Senha, Perfil, Ativo
  FROM dbo.Usuarios
 WHERE Email = 'admin@escola.com';

-- Catalogo do aluno (DS03): so os cursos ativos, com a contagem de aulas
SELECT c.Id, c.Titulo, c.CargaHoraria,
       (SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId = c.Id) AS TotalAulas
  FROM dbo.Cursos c
 WHERE c.Ativo = 1
 ORDER BY c.Titulo;

-- Busca de cursos (DS04)
SELECT Titulo, CargaHoraria
  FROM dbo.Cursos
 WHERE Ativo = 1
   AND (Titulo LIKE '%banco%' OR Descricao LIKE '%banco%');

-- Aulas de um curso com o progresso de UM aluno (DS05 e DS08)
SELECT a.Id, a.Ordem, a.Titulo, a.DuracaoMin,
       CASE WHEN p.Id IS NULL THEN 'Nao' ELSE 'Sim' END AS Concluida
  FROM dbo.Aulas a
  LEFT JOIN dbo.Progresso p
         ON p.AulaId = a.Id AND p.UsuarioId = 2
 WHERE a.CursoId = 1
 ORDER BY a.Ordem;

-- Percentual concluido por aluno e por curso
SELECT u.Nome, c.Titulo,
       COUNT(p.Id) AS AulasConcluidas,
       (SELECT COUNT(*) FROM dbo.Aulas a2 WHERE a2.CursoId = c.Id) AS TotalAulas
  FROM dbo.Usuarios u
 CROSS JOIN dbo.Cursos c
  LEFT JOIN dbo.Aulas a ON a.CursoId = c.Id
  LEFT JOIN dbo.Progresso p ON p.AulaId = a.Id AND p.UsuarioId = u.Id
 WHERE u.Perfil = 'ALUNO'
 GROUP BY u.Nome, c.Titulo, c.Id
 ORDER BY u.Nome, c.Titulo;

-- =====================================================================
-- 5) EXEMPLOS DOS DEMAIS COMANDOS (comentados de proposito)
-- =====================================================================

-- INSERT de curso (DS07)
-- INSERT INTO dbo.Cursos (Titulo, Descricao, CargaHoraria, Ativo)
-- VALUES ('Programacao em C#', 'Orientacao a objetos na pratica.', 80, 1);

-- UPDATE de curso (DS09) - o WHERE e obrigatorio!
-- UPDATE dbo.Cursos
--    SET Titulo = 'Novo titulo', CargaHoraria = 50
--  WHERE Id = 1;

-- DELETE de curso (DS10) - leva junto as aulas, pelo ON DELETE CASCADE
-- DELETE FROM dbo.Cursos WHERE Id = 1;

-- Marcar aula como concluida (DS05)
-- INSERT INTO dbo.Progresso (UsuarioId, AulaId) VALUES (2, 1);

-- Desmarcar
-- DELETE FROM dbo.Progresso WHERE UsuarioId = 2 AND AulaId = 1;

-- Redefinir senha (DS02)
-- UPDATE dbo.Usuarios SET Senha = 'nova123' WHERE Email = 'maria@escola.com';
