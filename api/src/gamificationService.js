const { sql, getPool } = require('./db');

/**
 * Registra XP uma unica vez por aula. Desmarcar e concluir novamente a mesma
 * aula nao concede XP duplicado. Todo o calculo e feito no servidor.
 */
async function awardLessonCompletion(usuarioId, aulaId) {
  const pool = await getPool();
  const result = await pool.request()
    .input('usuarioId', sql.Int, usuarioId)
    .input('aulaId', sql.Int, aulaId)
    .query(`
      SET NOCOUNT ON;
      SET XACT_ABORT ON;

      DECLARE @NovasConquistas TABLE (ConquistaId INT);
      DECLARE @XPConquistas INT = 0;
      DECLARE @XPConcedido INT = 0;

      BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
          SELECT 1 FROM dbo.Gamificacao WITH (UPDLOCK, HOLDLOCK)
          WHERE UsuarioId = @usuarioId
        )
        BEGIN
          INSERT INTO dbo.Gamificacao (UsuarioId, XP)
          VALUES (@usuarioId, 0);
        END;

        IF NOT EXISTS (
          SELECT 1 FROM dbo.HistoricoXP WITH (UPDLOCK, HOLDLOCK)
          WHERE UsuarioId = @usuarioId
            AND ReferenciaTipo = 'AULA'
            AND ReferenciaId = @aulaId
        )
        BEGIN
          INSERT INTO dbo.HistoricoXP
            (UsuarioId, Quantidade, Motivo, ReferenciaTipo, ReferenciaId)
          VALUES
            (@usuarioId, 10, N'Conclusão de aula', 'AULA', @aulaId);

          DECLARE @Hoje DATE = CAST(GETDATE() AS DATE);

          UPDATE dbo.Gamificacao
          SET
            XP = XP + 10,
            SequenciaAtual = CASE
              WHEN UltimaAtividade = @Hoje THEN SequenciaAtual
              WHEN UltimaAtividade = DATEADD(DAY, -1, @Hoje) THEN SequenciaAtual + 1
              ELSE 1
            END,
            MelhorSequencia = CASE
              WHEN UltimaAtividade = @Hoje THEN MelhorSequencia
              WHEN UltimaAtividade = DATEADD(DAY, -1, @Hoje)
                THEN CASE WHEN SequenciaAtual + 1 > MelhorSequencia
                          THEN SequenciaAtual + 1 ELSE MelhorSequencia END
              WHEN MelhorSequencia < 1 THEN 1
              ELSE MelhorSequencia
            END,
            UltimaAtividade = @Hoje
          WHERE UsuarioId = @usuarioId;

          SET @XPConcedido = 10;
        END;

        DECLARE @AulasConcluidas INT = (
          SELECT COUNT(*)
          FROM dbo.HistoricoXP
          WHERE UsuarioId = @usuarioId AND ReferenciaTipo = 'AULA'
        );

        DECLARE @CursosConcluidos INT = (
          SELECT COUNT(*)
          FROM dbo.Matriculas m
          WHERE m.UsuarioId = @usuarioId
            AND EXISTS (SELECT 1 FROM dbo.Aulas a WHERE a.CursoId = m.CursoId)
            AND NOT EXISTS (
              SELECT 1
              FROM dbo.Aulas a
              WHERE a.CursoId = m.CursoId
                AND NOT EXISTS (
                  SELECT 1 FROM dbo.Progresso p
                  WHERE p.UsuarioId = @usuarioId AND p.AulaId = a.Id
                )
            )
        );

        INSERT INTO dbo.UsuarioConquistas (UsuarioId, ConquistaId)
        OUTPUT INSERTED.ConquistaId INTO @NovasConquistas (ConquistaId)
        SELECT @usuarioId, c.Id
        FROM dbo.Conquistas c
        WHERE NOT EXISTS (
          SELECT 1 FROM dbo.UsuarioConquistas uc
          WHERE uc.UsuarioId = @usuarioId AND uc.ConquistaId = c.Id
        )
        AND (
          (c.Codigo = 'PRIMEIRA_AULA' AND @AulasConcluidas >= 1)
          OR (c.Codigo = 'CINCO_AULAS' AND @AulasConcluidas >= 5)
          OR (c.Codigo = 'DEZ_AULAS' AND @AulasConcluidas >= 10)
          OR (c.Codigo = 'PRIMEIRO_CURSO' AND @CursosConcluidos >= 1)
          OR (c.Codigo = 'SEQUENCIA_7' AND EXISTS (
            SELECT 1 FROM dbo.Gamificacao
            WHERE UsuarioId = @usuarioId AND SequenciaAtual >= 7
          ))
        );

        SELECT @XPConquistas = ISNULL(SUM(c.XPBonus), 0)
        FROM dbo.Conquistas c
        INNER JOIN @NovasConquistas n ON n.ConquistaId = c.Id;

        IF @XPConquistas > 0
        BEGIN
          UPDATE dbo.Gamificacao
          SET XP = XP + @XPConquistas
          WHERE UsuarioId = @usuarioId;

          INSERT INTO dbo.HistoricoXP
            (UsuarioId, Quantidade, Motivo, ReferenciaTipo, ReferenciaId)
          SELECT @usuarioId, c.XPBonus, CONCAT(N'Conquista: ', c.Nome), 'CONQUISTA', c.Id
          FROM dbo.Conquistas c
          INNER JOIN @NovasConquistas n ON n.ConquistaId = c.Id;
        END;

        COMMIT TRANSACTION;

        SELECT
          @XPConcedido AS xpAula,
          @XPConquistas AS xpConquistas,
          g.XP AS xpTotal,
          g.SequenciaAtual AS sequenciaAtual,
          g.MelhorSequencia AS melhorSequencia
        FROM dbo.Gamificacao g
        WHERE g.UsuarioId = @usuarioId;

        SELECT c.Id AS id, c.Nome AS nome, c.Descricao AS descricao,
               c.Icone AS icone, c.XPBonus AS xpBonus
        FROM dbo.Conquistas c
        INNER JOIN @NovasConquistas n ON n.ConquistaId = c.Id;
      END TRY
      BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
      END CATCH;
    `);

  const summary = result.recordsets?.[0]?.[0] || {
    xpAula: 0,
    xpConquistas: 0,
    xpTotal: 0,
    sequenciaAtual: 0,
    melhorSequencia: 0,
  };

  return {
    ...summary,
    xpGanho: Number(summary.xpAula || 0) + Number(summary.xpConquistas || 0),
    novasConquistas: result.recordsets?.[1] || [],
    nivel: Math.floor(Number(summary.xpTotal || 0) / 100) + 1,
  };
}

module.exports = { awardLessonCompletion };
