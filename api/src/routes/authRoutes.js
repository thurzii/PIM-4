const express = require('express');
const jwt = require('jsonwebtoken');
const { sql, getPool } = require('../db');

const router = express.Router();

const SECRET = process.env.JWT_SECRET || 'plataforma-cursos-segredo';

function tokenFor(user) {
  return jwt.sign(
    {
      id: user.Id,
      nome: user.Nome,
      email: user.Email,
      perfil: user.Perfil,
    },
    SECRET,
    { expiresIn: '8h' }
  );
}

router.post('/login', async (req, res) => {
  const { email, senha } = req.body;

  if (
    typeof email !== 'string' ||
    !email.trim() ||
    typeof senha !== 'string' ||
    !senha
  ) {
    return res.status(400).json({ message: 'Informe email e senha.' });
  }

  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('email', sql.NVarChar(200), email.trim())
      .input('senha', sql.NVarChar(200), senha)
      .query(`
        SELECT TOP 1 Id, Nome, Email, Perfil, Ativo
        FROM dbo.Usuarios
        WHERE Email = @email AND Senha = @senha
      `);

    const user = result.recordset[0];

    if (!user) {
      return res.status(401).json({ message: 'Email ou senha incorretos.' });
    }

    if (!user.Ativo) {
      return res.status(403).json({ message: 'Usuario inativo.' });
    }

    res.json({
      token: tokenFor(user),
      user: {
        id: user.Id,
        nome: user.Nome,
        email: user.Email,
        perfil: user.Perfil,
      },
    });
  } catch (error) {
    res.status(500).json({
      message: 'Erro ao autenticar.',
      detail: error.message,
    });
  }
});

router.post('/recover', async (req, res) => {
  return res.status(501).json({
    message: 'A recuperacao automatica esta temporariamente indisponivel. Solicite a redefinicao ao administrador da plataforma.',
  });
});

module.exports = router;
