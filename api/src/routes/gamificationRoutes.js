const express = require('express');
const { sql, getPool } = require('../db');
const { auth, studentOnly } = require('../middleware');

const router = express.Router();

router.get('/me', auth, studentOnly, async (req, res) => {
  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('usuarioId', sql.Int, req.user.id)
      .query(`
        SELECT
          ISNULL(g.XP, 0) AS xp,
          CAST(FLOOR(ISNULL(g.XP, 0) / 100.0) AS INT) + 1 AS nivel,
          ISNULL(g.SequenciaAtual, 0) AS sequenciaAtual,
          ISNULL(g.MelhorSequencia, 0) AS melhorSequencia,
          (SELECT COUNT(*) FROM dbo.HistoricoXP h
           WHERE h.UsuarioId = @usuarioId AND h.ReferenciaTipo = 'AULA') AS aulasConcluidas
        FROM (SELECT @usuarioId AS UsuarioId) u
        LEFT JOIN dbo.Gamificacao g ON g.UsuarioId = u.UsuarioId;

        SELECT
          c.Id AS id,
          c.Codigo AS codigo,
          c.Nome AS nome,
          c.Descricao AS descricao,
          c.Icone AS icone,
          c.XPBonus AS xpBonus,
          CASE WHEN uc.Id IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS conquistada,
          uc.DataConquista AS dataConquista
        FROM dbo.Conquistas c
        LEFT JOIN dbo.UsuarioConquistas uc
          ON uc.ConquistaId = c.Id AND uc.UsuarioId = @usuarioId
        ORDER BY conquistada DESC, c.Id;
      `);

    const stats = result.recordsets?.[0]?.[0] || {
      xp: 0,
      nivel: 1,
      sequenciaAtual: 0,
      melhorSequencia: 0,
      aulasConcluidas: 0,
    };
    const nivel = Number(stats.nivel || 1);
    const xp = Number(stats.xp || 0);
    const xpBaseNivel = (nivel - 1) * 100;

    res.json({
      ...stats,
      xp,
      nivel,
      xpProximoNivel: nivel * 100,
      xpNoNivel: xp - xpBaseNivel,
      progressoNivel: xp - xpBaseNivel,
      conquistas: result.recordsets?.[1] || [],
    });
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao carregar gamificação.',
      detail: error.message,
    });
  }
});

module.exports = router;
