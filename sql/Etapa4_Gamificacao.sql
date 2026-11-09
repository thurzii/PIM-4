/*
  Etapa 4 - Gamificacao
  Execute este script no banco CursosDB ja utilizado pela API.
  O script e idempotente: pode ser executado novamente sem duplicar tabelas/conquistas.
*/

IF OBJECT_ID('dbo.Gamificacao', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Gamificacao (
        UsuarioId INT NOT NULL PRIMARY KEY,
        XP INT NOT NULL CONSTRAINT DF_Gamificacao_XP DEFAULT 0,
        SequenciaAtual INT NOT NULL CONSTRAINT DF_Gamificacao_Sequencia DEFAULT 0,
        MelhorSequencia INT NOT NULL CONSTRAINT DF_Gamificacao_MelhorSequencia DEFAULT 0,
        UltimaAtividade DATE NULL,
        CONSTRAINT FK_Gamificacao_Usuarios FOREIGN KEY (UsuarioId)
            REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID('dbo.Conquistas', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Conquistas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Codigo NVARCHAR(50) NOT NULL UNIQUE,
        Nome NVARCHAR(100) NOT NULL,
        Descricao NVARCHAR(300) NOT NULL,
        Icone NVARCHAR(20) NOT NULL,
        XPBonus INT NOT NULL CONSTRAINT DF_Conquistas_XPBonus DEFAULT 0
    );
END
GO

IF OBJECT_ID('dbo.UsuarioConquistas', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioConquistas (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL,
        ConquistaId INT NOT NULL,
        DataConquista DATETIME NOT NULL CONSTRAINT DF_UsuarioConquistas_Data DEFAULT GETDATE(),
        CONSTRAINT UQ_UsuarioConquistas UNIQUE (UsuarioId, ConquistaId),
        CONSTRAINT FK_UsuarioConquistas_Usuarios FOREIGN KEY (UsuarioId)
            REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE,
        CONSTRAINT FK_UsuarioConquistas_Conquistas FOREIGN KEY (ConquistaId)
            REFERENCES dbo.Conquistas(Id) ON DELETE CASCADE
    );
END
GO

IF OBJECT_ID('dbo.HistoricoXP', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.HistoricoXP (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        UsuarioId INT NOT NULL,
        Quantidade INT NOT NULL,
        Motivo NVARCHAR(200) NOT NULL,
        ReferenciaTipo NVARCHAR(30) NOT NULL,
        ReferenciaId INT NOT NULL,
        DataRegistro DATETIME NOT NULL CONSTRAINT DF_HistoricoXP_Data DEFAULT GETDATE(),
        CONSTRAINT FK_HistoricoXP_Usuarios FOREIGN KEY (UsuarioId)
            REFERENCES dbo.Usuarios(Id) ON DELETE CASCADE,
        CONSTRAINT UQ_HistoricoXP_Evento UNIQUE (UsuarioId, ReferenciaTipo, ReferenciaId)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Conquistas WHERE Codigo = 'PRIMEIRA_AULA')
    INSERT dbo.Conquistas (Codigo, Nome, Descricao, Icone, XPBonus)
    VALUES ('PRIMEIRA_AULA', 'Primeiros passos', 'Conclua sua primeira aula.', N'🎯', 20);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Conquistas WHERE Codigo = 'CINCO_AULAS')
    INSERT dbo.Conquistas (Codigo, Nome, Descricao, Icone, XPBonus)
    VALUES ('CINCO_AULAS', 'Ritmo de estudos', 'Conclua cinco aulas.', N'📚', 30);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Conquistas WHERE Codigo = 'DEZ_AULAS')
    INSERT dbo.Conquistas (Codigo, Nome, Descricao, Icone, XPBonus)
    VALUES ('DEZ_AULAS', 'Estudante dedicado', 'Conclua dez aulas.', N'🏅', 50);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Conquistas WHERE Codigo = 'PRIMEIRO_CURSO')
    INSERT dbo.Conquistas (Codigo, Nome, Descricao, Icone, XPBonus)
    VALUES ('PRIMEIRO_CURSO', 'Curso concluído', 'Conclua todas as aulas de um curso.', N'🎓', 100);
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Conquistas WHERE Codigo = 'SEQUENCIA_7')
    INSERT dbo.Conquistas (Codigo, Nome, Descricao, Icone, XPBonus)
    VALUES ('SEQUENCIA_7', 'Uma semana de foco', 'Estude por sete dias consecutivos.', N'🔥', 75);
GO
