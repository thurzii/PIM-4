/*
  DIAGNOSTICO DO BANCO - PLATAFORMA DE CURSOS

  Este script e somente de consulta. Ele NAO cria, altera ou exclui dados,
  tabelas, indices ou relacionamentos.

  Execute conectado diretamente ao banco que a equipe pretende utilizar.
*/

SELECT
    DB_NAME() AS BancoConectado,
    SCHEMA_NAME() AS SchemaPadrao;
GO

WITH ColunasEsperadas AS (
    SELECT tabela, coluna
    FROM (VALUES
        ('Usuarios', 'Id'),
        ('Usuarios', 'Nome'),
        ('Usuarios', 'Email'),
        ('Usuarios', 'Senha'),
        ('Usuarios', 'Perfil'),
        ('Usuarios', 'Ativo'),
        ('Cursos', 'Id'),
        ('Cursos', 'Titulo'),
        ('Cursos', 'Descricao'),
        ('Cursos', 'CargaHoraria'),
        ('Cursos', 'Ativo'),
        ('Cursos', 'DataCadastro'),
        ('Cursos', 'ProfessorId'),
        ('Aulas', 'Id'),
        ('Aulas', 'CursoId'),
        ('Aulas', 'Titulo'),
        ('Aulas', 'Ordem'),
        ('Aulas', 'DuracaoMin'),
        ('Progresso', 'Id'),
        ('Progresso', 'UsuarioId'),
        ('Progresso', 'AulaId'),
        ('Progresso', 'DataConclusao'),
        ('Matriculas', 'Id'),
        ('Matriculas', 'UsuarioId'),
        ('Matriculas', 'CursoId'),
        ('Matriculas', 'DataMatricula')
    ) AS esperado(tabela, coluna)
)
SELECT
    esperadas.tabela AS Tabela,
    esperadas.coluna AS Coluna,
    CASE
        WHEN existentes.COLUMN_NAME IS NULL THEN 'AUSENTE'
        ELSE 'OK'
    END AS Situacao
FROM ColunasEsperadas AS esperadas
LEFT JOIN INFORMATION_SCHEMA.COLUMNS AS existentes
    ON existentes.TABLE_SCHEMA = 'dbo'
    AND existentes.TABLE_NAME = esperadas.tabela
    AND existentes.COLUMN_NAME = esperadas.coluna
ORDER BY esperadas.tabela, esperadas.coluna;
GO

WITH RelacionamentosEsperados AS (
    SELECT nome
    FROM (VALUES
        ('FK_Aulas_Cursos'),
        ('FK_Progresso_Usuarios'),
        ('FK_Progresso_Aulas'),
        ('FK_Cursos_Professor'),
        ('FK_Matriculas_Usuarios'),
        ('FK_Matriculas_Cursos')
    ) AS esperado(nome)
)
SELECT
    esperados.nome AS Relacionamento,
    CASE
        WHEN existentes.name IS NULL THEN 'AUSENTE'
        ELSE 'OK'
    END AS Situacao
FROM RelacionamentosEsperados AS esperados
LEFT JOIN sys.foreign_keys AS existentes
    ON existentes.name = esperados.nome
ORDER BY esperados.nome;
GO

SELECT
    tabelas.name AS Tabela,
    COALESCE(SUM(particoes.row_count), 0) AS QuantidadeRegistros
FROM sys.tables AS tabelas
LEFT JOIN sys.dm_db_partition_stats AS particoes
    ON particoes.object_id = tabelas.object_id
    AND particoes.index_id IN (0, 1)
WHERE tabelas.schema_id = SCHEMA_ID('dbo')
    AND tabelas.name IN ('Usuarios', 'Cursos', 'Aulas', 'Progresso', 'Matriculas')
GROUP BY tabelas.name
ORDER BY tabelas.name;
GO
