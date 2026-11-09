-- Etapa 1: suporte ao perfil PROFESSOR e matriculas.
-- Execute no banco CursosDB ja existente. O script e idempotente.

IF COL_LENGTH('dbo.Cursos', 'ProfessorId') IS NULL
BEGIN
    ALTER TABLE dbo.Cursos ADD ProfessorId INT NULL;
END
GO

IF OBJECT_ID('dbo.Matriculas', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Matriculas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL,
        CursoId INT NOT NULL,
        DataMatricula DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT UQ_Matriculas_Usuario_Curso UNIQUE (UsuarioId, CursoId)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Cursos_Professor')
BEGIN
    ALTER TABLE dbo.Cursos
    ADD CONSTRAINT FK_Cursos_Professor
        FOREIGN KEY (ProfessorId) REFERENCES dbo.Usuarios(Id);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Matriculas_Usuarios')
BEGIN
    ALTER TABLE dbo.Matriculas
    ADD CONSTRAINT FK_Matriculas_Usuarios
        FOREIGN KEY (UsuarioId) REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE;
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Matriculas_Cursos')
BEGIN
    ALTER TABLE dbo.Matriculas
    ADD CONSTRAINT FK_Matriculas_Cursos
        FOREIGN KEY (CursoId) REFERENCES dbo.Cursos(Id) ON DELETE CASCADE;
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Email = 'professor@escola.com')
BEGIN
    INSERT dbo.Usuarios (Nome, Email, Senha, Perfil, Ativo)
    VALUES ('Professor Demo', 'professor@escola.com', 'professor123', 'PROFESSOR', 1);
END
GO
