
const express = require('express');
const { sql, getPool } = require('../db');
const { auth } = require('../middleware');

const router = express.Router();

// Middleware de acesso exclusivo para alunos
function studentOnly(req, res, next) {
  const perfil = String(
    req.user?.perfil || req.user?.Perfil || ''
  ).toUpperCase();

  if (perfil !== 'ALUNO') {
    return res.status(403).json({
      message: 'Acesso permitido apenas para alunos.'
    });
  }

  next();
}

// Ranking global de alunos, visivel somente para professores e administradores.
// Para restringir a uma turma/curso especifico, sera necessario definir a regra
// de escopo junto a equipe web/desktop.
router.get('/turma', auth, async (req, res) => {
  const perfil = String(req.user?.perfil || '').toUpperCase();
  if (!['ADMIN', 'PROFESSOR'].includes(perfil)) {
    return res.status(403).json({ message: 'Acesso permitido apenas a professores e administradores.' });
  }
  try {
    const pool = await getPool();
    const result = await pool.request().query(`
      SELECT u.Id AS id, u.Nome AS nome, u.Email AS email,
             ISNULL(g.XP, 0) AS xp,
             CAST(FLOOR(ISNULL(g.XP, 0) / 100.0) AS INT) + 1 AS nivel,
             ISNULL(g.SequenciaAtual, 0) AS sequenciaAtual,
             ISNULL(g.MelhorSequencia, 0) AS melhorSequencia
      FROM dbo.Usuarios u
      LEFT JOIN dbo.Gamificacao g ON g.UsuarioId = u.Id
      WHERE u.Perfil = 'ALUNO' AND u.Ativo = 1
      ORDER BY ISNULL(g.XP, 0) DESC, u.Nome ASC, u.Id ASC
    `);
    return res.json(result.recordset.map((aluno, index) => ({ posicao: index + 1, ...aluno })));
  } catch (error) {
    console.error('Erro ao consultar ranking:', error);
    return res.status(500).json({ message: 'Erro ao consultar ranking de XP.' });
  }
});

// GET /api/gamificacao/me
// Retorna XP, nível, sequência e conquistas do aluno
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
          (
            SELECT COUNT(*)
            FROM dbo.HistoricoXP h
            WHERE h.UsuarioId = @usuarioId
              AND h.ReferenciaTipo = 'AULA'
          ) AS aulasConcluidas
        FROM (SELECT @usuarioId AS UsuarioId) u
        LEFT JOIN dbo.Gamificacao g
          ON g.UsuarioId = u.UsuarioId;

        SELECT
          c.Id AS id,
          c.Codigo AS codigo,
          c.Nome AS nome,
          c.Descricao AS descricao,
          c.Icone AS icone,
          c.XPBonus AS xpBonus,
          CASE
            WHEN uc.Id IS NULL THEN CAST(0 AS BIT)
            ELSE CAST(1 AS BIT)
          END AS conquistada,
          uc.DataConquista AS dataConquista
        FROM dbo.Conquistas c
        LEFT JOIN dbo.UsuarioConquistas uc
          ON uc.ConquistaId = c.Id
          AND uc.UsuarioId = @usuarioId
        ORDER BY conquistada DESC, c.Id;
      `);

    const stats = result.recordsets?.[0]?.[0] || {
      xp: 0,
      nivel: 1,
      sequenciaAtual: 0,
      melhorSequencia: 0,
      aulasConcluidas: 0
    };

    const nivel = Number(stats.nivel || 1);
    const xp = Number(stats.xp || 0);
    const xpBaseNivel = (nivel - 1) * 100;

    return res.json({
      ...stats,
      xp,
      nivel,
      xpProximoNivel: nivel * 100,
      xpNoNivel: xp - xpBaseNivel,
      progressoNivel: xp - xpBaseNivel,
      conquistas: result.recordsets?.[1] || []
    });

  } catch (error) {
    console.error('Erro na gamificação:', error);

    return res.status(500).json({
      message: 'Erro ao carregar gamificação.',
      detail: error.message
    });
  }
});

module.exports = router;
