IF DB_ID('CursosDB') IS NULL CREATE DATABASE CursosDB;
GO
USE CursosDB;
GO
IF OBJECT_ID('dbo.Progresso','U') IS NULL
CREATE TABLE dbo.Progresso(Id INT IDENTITY(1,1) PRIMARY KEY,UsuarioId INT NOT NULL,AulaId INT NOT NULL,DataConclusao DATETIME NOT NULL DEFAULT GETDATE(),CONSTRAINT UQ_Progresso UNIQUE(UsuarioId,AulaId));
GO
IF OBJECT_ID('dbo.Aulas','U') IS NULL
CREATE TABLE dbo.Aulas(Id INT IDENTITY(1,1) PRIMARY KEY,CursoId INT NOT NULL,Titulo NVARCHAR(300) NOT NULL,Ordem INT NOT NULL,DuracaoMin INT NOT NULL DEFAULT 0);
GO
IF OBJECT_ID('dbo.Cursos','U') IS NULL
CREATE TABLE dbo.Cursos(Id INT IDENTITY(1,1) PRIMARY KEY,Titulo NVARCHAR(200) NOT NULL,Descricao NVARCHAR(MAX) NULL,CargaHoraria INT NOT NULL DEFAULT 0,Ativo BIT NOT NULL DEFAULT 1,DataCadastro DATETIME NOT NULL DEFAULT GETDATE());
GO
IF OBJECT_ID('dbo.Usuarios','U') IS NULL
CREATE TABLE dbo.Usuarios(Id INT IDENTITY(1,1) PRIMARY KEY,Nome NVARCHAR(200) NOT NULL,Email NVARCHAR(200) NOT NULL UNIQUE,Senha NVARCHAR(200) NOT NULL,Perfil NVARCHAR(20) NOT NULL,Ativo BIT NOT NULL DEFAULT 1,DataCadastro DATETIME NOT NULL DEFAULT GETDATE());
GO
IF NOT EXISTS(SELECT 1 FROM dbo.Usuarios WHERE Email='admin@escola.com') INSERT dbo.Usuarios(Nome,Email,Senha,Perfil,Ativo) VALUES('Administrador','admin@escola.com','admin123','ADMIN',1);
IF NOT EXISTS(SELECT 1 FROM dbo.Usuarios WHERE Email='maria@escola.com') INSERT dbo.Usuarios(Nome,Email,Senha,Perfil,Ativo) VALUES('Maria','maria@escola.com','aluno123','ALUNO',1);
IF NOT EXISTS(SELECT 1 FROM dbo.Usuarios WHERE Email='joao@escola.com') INSERT dbo.Usuarios(Nome,Email,Senha,Perfil,Ativo) VALUES('João','joao@escola.com','aluno123','ALUNO',1);
GO
IF NOT EXISTS(SELECT 1 FROM dbo.Cursos)
BEGIN
INSERT dbo.Cursos(Titulo,Descricao,CargaHoraria,Ativo) VALUES
('Introdução à Programação','Fundamentos de lógica, algoritmos e primeiros programas.',20,1),
('Banco de Dados com SQL Server','Conceitos de banco de dados e comandos SQL.',30,1);
INSERT dbo.Aulas(CursoId,Titulo,Ordem,DuracaoMin) SELECT Id,'Lógica de programação e algoritmos',1,60 FROM dbo.Cursos WHERE Titulo='Introdução à Programação';
INSERT dbo.Aulas(CursoId,Titulo,Ordem,DuracaoMin) SELECT Id,'Variáveis, condições e repetição',2,60 FROM dbo.Cursos WHERE Titulo='Introdução à Programação';
INSERT dbo.Aulas(CursoId,Titulo,Ordem,DuracaoMin) SELECT Id,'SELECT, INSERT, UPDATE e DELETE',1,70 FROM dbo.Cursos WHERE Titulo='Banco de Dados com SQL Server';
END
GO
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name='FK_Aulas_Cursos') ALTER TABLE dbo.Aulas ADD CONSTRAINT FK_Aulas_Cursos FOREIGN KEY(CursoId) REFERENCES dbo.Cursos(Id) ON DELETE CASCADE;
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name='FK_Progresso_Usuarios') ALTER TABLE dbo.Progresso ADD CONSTRAINT FK_Progresso_Usuarios FOREIGN KEY(UsuarioId) REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE;
IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE name='FK_Progresso_Aulas') ALTER TABLE dbo.Progresso ADD CONSTRAINT FK_Progresso_Aulas FOREIGN KEY(AulaId) REFERENCES dbo.Aulas(Id) ON DELETE CASCADE;
GO
