USE PlataformaCursosDB;
GO

-- Confirme os registros antes de desativar as contas de teste.
SELECT Id, Nome, Email, Perfil, Ativo
FROM dbo.Usuarios
WHERE Email IN (
    'maria@escola.com',
    'joao@escola.com',
    'professor@escola.com'
)
ORDER BY Email;
GO

-- Desativa somente as contas demonstrativas.
-- O administrador admin@escola.com é preservado.
UPDATE dbo.Usuarios
SET Ativo = 0
OUTPUT INSERTED.Id, INSERTED.Nome, INSERTED.Email, INSERTED.Perfil, INSERTED.Ativo
WHERE Email IN (
    'maria@escola.com',
    'joao@escola.com',
    'professor@escola.com'
);
GO
