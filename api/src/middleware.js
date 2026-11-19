const jwt = require('jsonwebtoken');
const { sql, getPool } = require('./db');

async function auth(req, res, next) {
  const header = req.headers.authorization || '';
  const token = header.startsWith('Bearer ') ? header.slice(7) : null;
  if (!token) return res.status(401).json({ message: 'Token nao informado.' });

  try {
    const claims = jwt.verify(token, process.env.JWT_SECRET || 'plataforma-cursos-segredo');
    const pool = await getPool();
    const result = await pool.request().input('id', sql.Int, claims.id)
      .query('SELECT TOP 1 Id, Nome, Email, Perfil, Ativo FROM dbo.Usuarios WHERE Id=@id');
    const account = result.recordset[0];
    if (!account || !account.Ativo) return res.status(401).json({ message: 'Conta inexistente ou inativa.' });
    req.user = { id: account.Id, nome: account.Nome, email: account.Email, perfil: account.Perfil };
    next();
  } catch {
    return res.status(401).json({ message: 'Token invalido ou expirado.' });
  }
}

function studentOnly(req, res, next) {
  if (req.user?.perfil !== 'ALUNO') return res.status(403).json({ message: 'Acesso restrito a alunos.' });
  next();
}

function professorOnly(req, res, next) {
  if (req.user?.perfil !== 'PROFESSOR') return res.status(403).json({ message: 'Acesso restrito a professores.' });
  next();
}

function adminOnly(req, res, next) {
  if (req.user?.perfil !== 'ADMIN') {
    return res.status(403).json({ message: 'Acesso permitido somente para administradores.' });
  }
  next();
}

module.exports = { auth, adminOnly, professorOnly, studentOnly };
