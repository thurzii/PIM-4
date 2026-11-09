const express = require('express');
const { sql, getPool } = require('../db');
const { auth, adminOnly } = require('../middleware');

const router = express.Router();

router.use(auth, adminOnly);

function validId(value) {
  const id = Number(value);
  return Number.isInteger(id) && id > 0 ? id : null;
}

function validCourseData(body) {
  const titulo = typeof body.titulo === 'string' ? body.titulo.trim() : '';
  const descricao = typeof body.descricao === 'string' ? body.descricao.trim() : '';
  const cargaHoraria = Number(body.cargaHoraria);

  if (!titulo) {
    return { error: 'Informe o titulo do curso.' };
  }

  if (titulo.length > 200) {
    return { error: 'O titulo deve ter no maximo 200 caracteres.' };
  }

  if (!Number.isInteger(cargaHoraria) || cargaHoraria < 0) {
    return { error: 'A carga horaria deve ser um numero inteiro igual ou maior que zero.' };
  }

  return {
    data: {
      titulo,
      descricao,
      cargaHoraria,
      ativo: body.ativo === undefined ? true : Boolean(body.ativo),
    },
  };
}

router.get('/cursos', async (req, res) => {
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
          (SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId = c.Id) AS totalAulas
        FROM dbo.Cursos c
        WHERE @termo = ''
          OR c.Titulo LIKE '%' + @termo + '%'
          OR c.Descricao LIKE '%' + @termo + '%'
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

router.post('/cursos', async (req, res) => {
  const validation = validCourseData(req.body);

  if (validation.error) {
    return res.status(400).json({ message: validation.error });
  }

  const { titulo, descricao, cargaHoraria, ativo } = validation.data;

  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('titulo', sql.NVarChar(200), titulo)
      .input('descricao', sql.NVarChar(sql.MAX), descricao)
      .input('carga', sql.Int, cargaHoraria)
      .input('ativo', sql.Bit, ativo)
      .query(`
        INSERT INTO dbo.Cursos (
          Titulo, Descricao, CargaHoraria, Ativo, DataCadastro
        )
        OUTPUT INSERTED.Id
        VALUES (@titulo, @descricao, @carga, @ativo, GETDATE())
      `);

    res.status(201).json({
      id: result.recordset[0].Id,
      message: 'Curso criado.',
    });
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao criar curso.',
      detail: error.message,
    });
  }
});

router.put('/cursos/:id', async (req, res) => {
  const id = validId(req.params.id);
  const validation = validCourseData(req.body);

  if (!id) {
    return res.status(400).json({ message: 'Identificador de curso invalido.' });
  }

  if (validation.error) {
    return res.status(400).json({ message: validation.error });
  }

  const { titulo, descricao, cargaHoraria, ativo } = validation.data;

  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('id', sql.Int, id)
      .input('titulo', sql.NVarChar(200), titulo)
      .input('descricao', sql.NVarChar(sql.MAX), descricao)
      .input('carga', sql.Int, cargaHoraria)
      .input('ativo', sql.Bit, ativo)
      .query(`
        UPDATE dbo.Cursos
        SET
          Titulo = @titulo,
          Descricao = @descricao,
          CargaHoraria = @carga,
          Ativo = @ativo
        WHERE Id = @id
      `);

    if (!result.rowsAffected[0]) {
      return res.status(404).json({ message: 'Curso nao encontrado.' });
    }

    res.json({ message: 'Curso atualizado.' });
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao atualizar curso.',
      detail: error.message,
    });
  }
});

router.delete('/cursos/:id', async (req, res) => {
  const id = validId(req.params.id);

  if (!id) {
    return res.status(400).json({ message: 'Identificador de curso invalido.' });
  }

  let transaction;

  try {
    const pool = await getPool();
    transaction = new sql.Transaction(pool);
    await transaction.begin();

    await new sql.Request(transaction)
      .input('id', sql.Int, id)
      .query(`
        DELETE p
        FROM dbo.Progresso p
        INNER JOIN dbo.Aulas a ON a.Id = p.AulaId
        WHERE a.CursoId = @id
      `);

    await new sql.Request(transaction)
      .input('id', sql.Int, id)
      .query('DELETE FROM dbo.Aulas WHERE CursoId = @id');

    const result = await new sql.Request(transaction)
      .input('id', sql.Int, id)
      .query('DELETE FROM dbo.Cursos WHERE Id = @id');

    if (!result.rowsAffected[0]) {
      await transaction.rollback();
      return res.status(404).json({ message: 'Curso nao encontrado.' });
    }

    await transaction.commit();
    res.json({ message: 'Curso excluido.' });
  } catch (error) {
    if (transaction) {
      try {
        await transaction.rollback();
      } catch (rollbackError) {
        // A transacao pode ja ter sido encerrada.
      }
    }

    res.status(500).json({
      message: 'Erro ao excluir curso.',
      detail: error.message,
    });
  }
});

router.post('/cursos/:cursoId/aulas', async (req, res) => {
  const cursoId = validId(req.params.cursoId);
  const titulo = typeof req.body.titulo === 'string'
    ? req.body.titulo.trim()
    : '';
  const duracaoMin = Number(req.body.duracaoMin);

  if (!cursoId) {
    return res.status(400).json({ message: 'Identificador de curso invalido.' });
  }

  if (!titulo) {
    return res.status(400).json({ message: 'Informe o titulo da aula.' });
  }

  if (titulo.length > 300) {
    return res.status(400).json({ message: 'O titulo da aula deve ter no maximo 300 caracteres.' });
  }

  if (!Number.isInteger(duracaoMin) || duracaoMin < 0) {
    return res.status(400).json({ message: 'A duracao deve ser um numero inteiro igual ou maior que zero.' });
  }

  try {
    const pool = await getPool();
    const course = await pool.request()
      .input('cursoId', sql.Int, cursoId)
      .query('SELECT Id FROM dbo.Cursos WHERE Id = @cursoId');

    if (!course.recordset[0]) {
      return res.status(404).json({ message: 'Curso nao encontrado.' });
    }

    const result = await pool.request()
      .input('cursoId', sql.Int, cursoId)
      .input('titulo', sql.NVarChar(300), titulo)
      .input('duracao', sql.Int, duracaoMin)
      .query(`
        DECLARE @ordem INT = (
          SELECT ISNULL(MAX(Ordem), 0) + 1
          FROM dbo.Aulas
          WHERE CursoId = @cursoId
        );

        INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin)
        OUTPUT INSERTED.Id
        VALUES (@cursoId, @titulo, @ordem, @duracao)
      `);

    res.status(201).json({
      id: result.recordset[0].Id,
      message: 'Aula criada.',
    });
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao criar aula.',
      detail: error.message,
    });
  }
});

router.delete('/aulas/:id', async (req, res) => {
  const id = validId(req.params.id);

  if (!id) {
    return res.status(400).json({ message: 'Identificador de aula invalido.' });
  }

  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('id', sql.Int, id)
      .query('DELETE FROM dbo.Aulas WHERE Id = @id');

    if (!result.rowsAffected[0]) {
      return res.status(404).json({ message: 'Aula nao encontrada.' });
    }

    res.json({ message: 'Aula excluida.' });
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao excluir aula.',
      detail: error.message,
    });
  }
});

router.post('/usuarios', async (req, res) => {
  const nome = typeof req.body.nome === 'string' ? req.body.nome.trim() : '';
  const email = typeof req.body.email === 'string' ? req.body.email.trim().toLowerCase() : '';
  const senha = typeof req.body.senha === 'string' ? req.body.senha : '';
  const perfil = typeof req.body.perfil === 'string' ? req.body.perfil.toUpperCase() : '';

  if (!nome || nome.length > 200) {
    return res.status(400).json({ message: 'Informe um nome com no maximo 200 caracteres.' });
  }

  if (!email || email.length > 200 || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
    return res.status(400).json({ message: 'Informe um email valido.' });
  }

  if (!senha || senha.length < 6 || senha.length > 200) {
    return res.status(400).json({ message: 'A senha deve ter entre 6 e 200 caracteres.' });
  }

  if (!['ADMIN', 'PROFESSOR', 'ALUNO'].includes(perfil)) {
    return res.status(400).json({ message: 'Perfil invalido.' });
  }

  try {
    const pool = await getPool();
    const existing = await pool.request()
      .input('email', sql.NVarChar(200), email)
      .query('SELECT Id FROM dbo.Usuarios WHERE Email = @email');

    if (existing.recordset[0]) {
      return res.status(409).json({ message: 'Ja existe um usuario com este email.' });
    }

    const result = await pool.request()
      .input('nome', sql.NVarChar(200), nome)
      .input('email', sql.NVarChar(200), email)
      .input('senha', sql.NVarChar(200), senha)
      .input('perfil', sql.NVarChar(20), perfil)
      .query(`
        INSERT INTO dbo.Usuarios (Nome, Email, Senha, Perfil, Ativo, DataCadastro)
        OUTPUT INSERTED.Id
        VALUES (@nome, @email, @senha, @perfil, 1, GETDATE())
      `);

    res.status(201).json({ id: result.recordset[0].Id, message: 'Usuario criado.' });
  } catch (error) {
    res.status(500).json({ message: 'Erro ao criar usuario.', detail: error.message });
  }
});

router.get('/usuarios', async (req, res) => {
  const termo = typeof req.query.busca === 'string'
    ? req.query.busca.trim()
    : '';

  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('termo', sql.NVarChar(200), termo)
      .query(`
        SELECT
          Id AS id,
          Nome AS nome,
          Email AS email,
          Perfil AS perfil,
          Ativo AS ativo
        FROM dbo.Usuarios
        WHERE @termo = ''
          OR Nome LIKE '%' + @termo + '%'
          OR Email LIKE '%' + @termo + '%'
        ORDER BY Nome
      `);

    res.json(result.recordset);
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao listar usuarios.',
      detail: error.message,
    });
  }
});

router.put('/usuarios/:id/status', async (req, res) => {
  const id = validId(req.params.id);

  if (!id) {
    return res.status(400).json({ message: 'Identificador de usuario invalido.' });
  }

  if (typeof req.body.ativo !== 'boolean') {
    return res.status(400).json({ message: 'Informe o novo status do usuario.' });
  }

  if (id === req.user.id && req.body.ativo === false) {
    return res.status(400).json({ message: 'Voce nao pode desativar sua propria conta durante a sessao.' });
  }

  try {
    const pool = await getPool();
    const target = await pool.request()
      .input('id', sql.Int, id)
      .query('SELECT Id, Perfil, Ativo FROM dbo.Usuarios WHERE Id = @id');

    if (!target.recordset[0]) {
      return res.status(404).json({ message: 'Usuario nao encontrado.' });
    }

    if (target.recordset[0].Perfil === 'ADMIN' && req.body.ativo === false) {
      const activeAdmins = await pool.request().query(`
        SELECT COUNT(*) AS total
        FROM dbo.Usuarios
        WHERE Perfil = 'ADMIN' AND Ativo = 1
      `);

      if (activeAdmins.recordset[0].total <= 1) {
        return res.status(400).json({ message: 'Nao e possivel desativar o ultimo administrador ativo.' });
      }
    }

    const result = await pool.request()
      .input('id', sql.Int, id)
      .input('ativo', sql.Bit, req.body.ativo)
      .query('UPDATE dbo.Usuarios SET Ativo = @ativo WHERE Id = @id');

    if (!result.rowsAffected[0]) {
      return res.status(404).json({ message: 'Usuario nao encontrado.' });
    }

    res.json({ message: 'Status atualizado.' });
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao atualizar usuario.',
      detail: error.message,
    });
  }
});

module.exports = router;
