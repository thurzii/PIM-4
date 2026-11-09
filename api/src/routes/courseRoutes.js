const express = require('express');
const { sql, getPool } = require('../db');
const { auth, studentOnly } = require('../middleware');
const { canAccessCourse } = require('../permissions');
const { awardLessonCompletion } = require('../gamificationService');

const router = express.Router();

function validId(value) {
  const id = Number(value);
  return Number.isInteger(id) && id > 0 ? id : null;
}

router.get('/', auth, studentOnly, async (req, res) => {
  const termo = typeof req.query.busca === 'string'
    ? req.query.busca.trim()
    : '';

  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('termo', sql.NVarChar(200), termo)
      .query(`
        SELECT
          c.Id AS id,
          c.Titulo AS titulo,
          c.Descricao AS descricao,
          c.CargaHoraria AS cargaHoraria,
          c.Ativo AS ativo,
          (SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId = c.Id) AS totalAulas,
          (SELECT COUNT(*) FROM dbo.Matriculas m WHERE m.CursoId = c.Id) AS totalAlunos
        FROM dbo.Cursos c
        WHERE c.Ativo = 1
          AND (
            @termo = ''
            OR c.Titulo LIKE '%' + @termo + '%'
            OR c.Descricao LIKE '%' + @termo + '%'
          )
        ORDER BY c.Titulo
      `);

    res.json(result.recordset);
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao listar cursos.',
      detail: error.message,
    });
  }
});

router.get('/:id', auth, async (req, res) => {
  const id = validId(req.params.id);

  if (!id) {
    return res.status(400).json({ message: 'Identificador de curso invalido.' });
  }

  try {
    const pool = await getPool();
    const course = await pool.request()
      .input('id', sql.Int, id)
      .query(`
        SELECT
          Id AS id,
          Titulo AS titulo,
          Descricao AS descricao,
          CargaHoraria AS cargaHoraria,
          Ativo AS ativo,
          ProfessorId AS professorId
        FROM dbo.Cursos
        WHERE Id = @id
      `);

    if (!course.recordset[0]) {
      return res.status(404).json({ message: 'Curso nao encontrado.' });
    }

    const courseData = course.recordset[0];

    if (!canAccessCourse(req.user, courseData)) {
      if (req.user.perfil === 'PROFESSOR') {
        return res.status(403).json({ message: 'Voce nao e o responsavel por este curso.' });
      }

      return res.status(404).json({ message: 'Curso nao encontrado ou indisponivel.' });
    }

    const lessons = await pool.request()
      .input('cursoId', sql.Int, id)
      .input('usuarioId', sql.Int, req.user.id)
      .query(`
        SELECT
          a.Id AS id,
          a.Titulo AS titulo,
          a.Ordem AS ordem,
          a.DuracaoMin AS duracaoMin,
          CASE
            WHEN p.Id IS NULL THEN CAST(0 AS bit)
            ELSE CAST(1 AS bit)
          END AS concluida
        FROM dbo.Aulas a
        LEFT JOIN dbo.Progresso p
          ON p.AulaId = a.Id AND p.UsuarioId = @usuarioId
        WHERE a.CursoId = @cursoId
        ORDER BY a.Ordem
      `);

    const enrollment = await pool.request()
      .input('cursoId', sql.Int, id)
      .input('usuarioId', sql.Int, req.user.id)
      .query(`SELECT Id FROM dbo.Matriculas WHERE CursoId = @cursoId AND UsuarioId = @usuarioId`);

    const total = lessons.recordset.length;
    const done = lessons.recordset.filter((lesson) => lesson.concluida).length;

    res.json({
      ...courseData,
      matriculado: enrollment.recordset.length > 0,
      aulas: lessons.recordset,
      progresso: total ? Math.round((done * 100) / total) : 0,
      concluidas: done,
      totalAulas: total,
    });
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao carregar curso.',
      detail: error.message,
    });
  }
});

router.post('/:id/matricula', auth, studentOnly, async (req, res) => {
  const cursoId = validId(req.params.id);

  if (!cursoId) {
    return res.status(400).json({ message: 'Identificador de curso invalido.' });
  }

  try {
    const pool = await getPool();
    const course = await pool.request()
      .input('cursoId', sql.Int, cursoId)
      .query('SELECT Id FROM dbo.Cursos WHERE Id = @cursoId AND Ativo = 1');

    if (!course.recordset[0]) {
      return res.status(404).json({ message: 'Curso nao encontrado ou inativo.' });
    }

    await pool.request()
      .input('cursoId', sql.Int, cursoId)
      .input('usuarioId', sql.Int, req.user.id)
      .query(`
        IF NOT EXISTS (
          SELECT 1 FROM dbo.Matriculas
          WHERE CursoId = @cursoId AND UsuarioId = @usuarioId
        )
        INSERT INTO dbo.Matriculas (CursoId, UsuarioId, DataMatricula)
        VALUES (@cursoId, @usuarioId, GETDATE())
      `);

    res.status(201).json({ message: 'Matricula realizada com sucesso.' });
  } catch (error) {
    res.status(500).json({ message: 'Erro ao realizar matricula.', detail: error.message });
  }
});

router.put('/:cursoId/aulas/:aulaId/progresso', auth, studentOnly, async (req, res) => {
  const cursoId = validId(req.params.cursoId);
  const aulaId = validId(req.params.aulaId);
  const { concluida } = req.body;

  if (!cursoId || !aulaId) {
    return res.status(400).json({ message: 'Identificador de curso ou aula invalido.' });
  }

  if (typeof concluida !== 'boolean') {
    return res.status(400).json({ message: 'Informe se a aula foi concluida.' });
  }

  try {
    const pool = await getPool();
    const check = await pool.request()
      .input('aulaId', sql.Int, aulaId)
      .input('cursoId', sql.Int, cursoId)
      .query('SELECT Id FROM dbo.Aulas WHERE Id = @aulaId AND CursoId = @cursoId');

    if (!check.recordset[0]) {
      return res.status(404).json({ message: 'Aula nao encontrada.' });
    }

    const enrollment = await pool.request()
      .input('usuarioId', sql.Int, req.user.id)
      .input('cursoId', sql.Int, cursoId)
      .query('SELECT Id FROM dbo.Matriculas WHERE UsuarioId = @usuarioId AND CursoId = @cursoId');

    if (!enrollment.recordset[0]) {
      return res.status(403).json({ message: 'Matricule-se no curso antes de registrar o progresso.' });
    }

    let gamificacao = null;

    if (concluida) {
      await pool.request()
        .input('usuarioId', sql.Int, req.user.id)
        .input('aulaId', sql.Int, aulaId)
        .query(`
          IF NOT EXISTS (
            SELECT 1
            FROM dbo.Progresso
            WHERE UsuarioId = @usuarioId AND AulaId = @aulaId
          )
          INSERT INTO dbo.Progresso (UsuarioId, AulaId, DataConclusao)
          VALUES (@usuarioId, @aulaId, GETDATE())
        `);

      gamificacao = await awardLessonCompletion(req.user.id, aulaId);
    } else {
      await pool.request()
        .input('usuarioId', sql.Int, req.user.id)
        .input('aulaId', sql.Int, aulaId)
        .query('DELETE FROM dbo.Progresso WHERE UsuarioId = @usuarioId AND AulaId = @aulaId');
    }

    res.json({
      message: concluida ? 'Aula concluida.' : 'Conclusao removida.',
      gamificacao,
    });
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao atualizar progresso.',
      detail: error.message,
    });
  }
});

module.exports = router;
