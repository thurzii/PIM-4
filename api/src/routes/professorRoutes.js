const express = require('express');
const { sql, getPool } = require('../db');
const { auth, professorOnly } = require('../middleware');

const router = express.Router();

router.use(auth, professorOnly);

function validId(value) {
  const id = Number(value);
  return Number.isInteger(id) && id > 0 ? id : null;
}

function validateCourse(body) {
  const titulo = typeof body.titulo === 'string' ? body.titulo.trim() : '';
  const descricao = typeof body.descricao === 'string' ? body.descricao.trim() : '';
  const cargaHoraria = Number(body.cargaHoraria);

  if (!titulo || titulo.length > 200) {
    return { error: 'Informe um titulo com no maximo 200 caracteres.' };
  }

  if (!Number.isInteger(cargaHoraria) || cargaHoraria < 0) {
    return { error: 'A carga horaria deve ser um numero inteiro igual ou maior que zero.' };
  }

  return { data: { titulo, descricao, cargaHoraria } };
}

router.get('/cursos', async (req, res) => {
  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('professorId', sql.Int, req.user.id)
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
        WHERE c.ProfessorId = @professorId
        ORDER BY c.Titulo
      `);

    res.json(result.recordset);
  } catch (error) {
    res.status(500).json({ message: 'Erro ao listar seus cursos.', detail: error.message });
  }
});

router.post('/cursos', async (req, res) => {
  const validation = validateCourse(req.body);
  if (validation.error) return res.status(400).json({ message: validation.error });

  const { titulo, descricao, cargaHoraria } = validation.data;

  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('titulo', sql.NVarChar(200), titulo)
      .input('descricao', sql.NVarChar(sql.MAX), descricao)
      .input('cargaHoraria', sql.Int, cargaHoraria)
      .input('professorId', sql.Int, req.user.id)
      .query(`
        INSERT INTO dbo.Cursos (Titulo, Descricao, CargaHoraria, Ativo, DataCadastro, ProfessorId)
        OUTPUT INSERTED.Id
        VALUES (@titulo, @descricao, @cargaHoraria, 1, GETDATE(), @professorId)
      `);

    res.status(201).json({ id: result.recordset[0].Id, message: 'Curso criado.' });
  } catch (error) {
    res.status(500).json({ message: 'Erro ao criar curso.', detail: error.message });
  }
});

router.put('/cursos/:id', async (req, res) => {
  const id = validId(req.params.id);
  const validation = validateCourse(req.body);
  if (!id) return res.status(400).json({ message: 'Identificador de curso invalido.' });
  if (validation.error) return res.status(400).json({ message: validation.error });

  const { titulo, descricao, cargaHoraria } = validation.data;

  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('id', sql.Int, id)
      .input('professorId', sql.Int, req.user.id)
      .input('titulo', sql.NVarChar(200), titulo)
      .input('descricao', sql.NVarChar(sql.MAX), descricao)
      .input('cargaHoraria', sql.Int, cargaHoraria)
      .query(`
        UPDATE dbo.Cursos
        SET Titulo = @titulo, Descricao = @descricao, CargaHoraria = @cargaHoraria
        WHERE Id = @id AND ProfessorId = @professorId
      `);

    if (!result.rowsAffected[0]) {
      return res.status(404).json({ message: 'Curso nao encontrado ou voce nao e o responsavel.' });
    }

    res.json({ message: 'Curso atualizado.' });
  } catch (error) {
    res.status(500).json({ message: 'Erro ao atualizar curso.', detail: error.message });
  }
});

router.get('/cursos/:id/aulas', async (req, res) => {
  const cursoId = validId(req.params.id);
  if (!cursoId) return res.status(400).json({ message: 'Identificador de curso invalido.' });

  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('cursoId', sql.Int, cursoId)
      .input('professorId', sql.Int, req.user.id)
      .query(`
        SELECT a.Id AS id, a.Titulo AS titulo, a.Ordem AS ordem, a.DuracaoMin AS duracaoMin
        FROM dbo.Aulas a
        INNER JOIN dbo.Cursos c ON c.Id = a.CursoId
        WHERE c.Id = @cursoId AND c.ProfessorId = @professorId
        ORDER BY a.Ordem
      `);

    const ownership = await pool.request()
      .input('cursoId', sql.Int, cursoId)
      .input('professorId', sql.Int, req.user.id)
      .query('SELECT Id FROM dbo.Cursos WHERE Id = @cursoId AND ProfessorId = @professorId');

    if (!ownership.recordset[0]) {
      return res.status(404).json({ message: 'Curso nao encontrado ou voce nao e o responsavel.' });
    }

    res.json(result.recordset);
  } catch (error) {
    res.status(500).json({ message: 'Erro ao listar aulas.', detail: error.message });
  }
});

router.post('/cursos/:id/aulas', async (req, res) => {
  const cursoId = validId(req.params.id);
  const titulo = typeof req.body.titulo === 'string' ? req.body.titulo.trim() : '';
  const duracaoMin = Number(req.body.duracaoMin);

  if (!cursoId) return res.status(400).json({ message: 'Identificador de curso invalido.' });
  if (!titulo || titulo.length > 300) return res.status(400).json({ message: 'Informe um titulo de aula com no maximo 300 caracteres.' });
  if (!Number.isInteger(duracaoMin) || duracaoMin < 0) return res.status(400).json({ message: 'A duracao deve ser um numero inteiro igual ou maior que zero.' });

  try {
    const pool = await getPool();
    const ownership = await pool.request()
      .input('cursoId', sql.Int, cursoId)
      .input('professorId', sql.Int, req.user.id)
      .query('SELECT Id FROM dbo.Cursos WHERE Id = @cursoId AND ProfessorId = @professorId');

    if (!ownership.recordset[0]) {
      return res.status(404).json({ message: 'Curso nao encontrado ou voce nao e o responsavel.' });
    }

    const result = await pool.request()
      .input('cursoId', sql.Int, cursoId)
      .input('titulo', sql.NVarChar(300), titulo)
      .input('duracaoMin', sql.Int, duracaoMin)
      .query(`
        DECLARE @ordem INT = (SELECT ISNULL(MAX(Ordem), 0) + 1 FROM dbo.Aulas WHERE CursoId = @cursoId);
        INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin)
        OUTPUT INSERTED.Id
        VALUES (@cursoId, @titulo, @ordem, @duracaoMin);
      `);

    res.status(201).json({ id: result.recordset[0].Id, message: 'Aula criada.' });
  } catch (error) {
    res.status(500).json({ message: 'Erro ao criar aula.', detail: error.message });
  }
});

router.delete('/aulas/:id', async (req, res) => {
  const aulaId = validId(req.params.id);
  if (!aulaId) return res.status(400).json({ message: 'Identificador de aula invalido.' });

  let transaction;

  try {
    const pool = await getPool();
    transaction = new sql.Transaction(pool);
    await transaction.begin();

    const ownership = await new sql.Request(transaction)
      .input('aulaId', sql.Int, aulaId)
      .input('professorId', sql.Int, req.user.id)
      .query(`
        SELECT a.Id
        FROM dbo.Aulas a
        INNER JOIN dbo.Cursos c ON c.Id = a.CursoId
        WHERE a.Id = @aulaId AND c.ProfessorId = @professorId
      `);

    if (!ownership.recordset[0]) {
      await transaction.rollback();
      return res.status(404).json({ message: 'Aula nao encontrada ou voce nao e o responsavel.' });
    }

    await new sql.Request(transaction)
      .input('aulaId', sql.Int, aulaId)
      .query('DELETE FROM dbo.Progresso WHERE AulaId = @aulaId');

    await new sql.Request(transaction)
      .input('aulaId', sql.Int, aulaId)
      .query('DELETE FROM dbo.Aulas WHERE Id = @aulaId');

    await transaction.commit();
    res.json({ message: 'Aula excluida.' });
  } catch (error) {
    if (transaction) {
      try {
        await transaction.rollback();
      } catch (rollbackError) {
        // A transacao pode ja ter sido encerrada.
      }
    }

    res.status(500).json({ message: 'Erro ao excluir aula.', detail: error.message });
  }
});

router.get('/cursos/:id/alunos', async (req, res) => {
  const cursoId = validId(req.params.id);
  if (!cursoId) return res.status(400).json({ message: 'Identificador de curso invalido.' });

  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('cursoId', sql.Int, cursoId)
      .input('professorId', sql.Int, req.user.id)
      .query(`
        SELECT
          u.Id AS id,
          u.Nome AS nome,
          u.Email AS email,
          m.DataMatricula AS dataMatricula,
          COUNT(a.Id) AS totalAulas,
          COUNT(p.Id) AS aulasConcluidas,
          CASE WHEN COUNT(a.Id) = 0 THEN 0
               ELSE CAST(ROUND(COUNT(p.Id) * 100.0 / COUNT(a.Id), 0) AS INT)
          END AS progresso
        FROM dbo.Cursos c
        INNER JOIN dbo.Matriculas m ON m.CursoId = c.Id
        INNER JOIN dbo.Usuarios u ON u.Id = m.UsuarioId
        LEFT JOIN dbo.Aulas a ON a.CursoId = c.Id
        LEFT JOIN dbo.Progresso p ON p.AulaId = a.Id AND p.UsuarioId = u.Id
        WHERE c.Id = @cursoId AND c.ProfessorId = @professorId
        GROUP BY u.Id, u.Nome, u.Email, m.DataMatricula
        ORDER BY u.Nome
      `);

    const ownership = await pool.request()
      .input('cursoId', sql.Int, cursoId)
      .input('professorId', sql.Int, req.user.id)
      .query('SELECT Id FROM dbo.Cursos WHERE Id = @cursoId AND ProfessorId = @professorId');

    if (!ownership.recordset[0]) {
      return res.status(404).json({ message: 'Curso nao encontrado ou voce nao e o responsavel.' });
    }

    res.json(result.recordset);
  } catch (error) {
    res.status(500).json({ message: 'Erro ao consultar alunos do curso.', detail: error.message });
  }
});

module.exports = router;
