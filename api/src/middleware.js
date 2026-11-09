const jwt = require('jsonwebtoken');
<<<<<<< HEAD
=======
const { sql, getPool } = require('./db');
const { allowRoles, adminOnly, professorOnly, studentOnly } = require('./permissions');
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)

function auth(req, res, next) {
  const header = req.headers.authorization || '';
  const token = header.startsWith('Bearer ') ? header.slice(7) : null;
<<<<<<< HEAD
  if (!token) return res.status(401).json({ message: 'Token nao informado.' });

  try {
    req.user = jwt.verify(token, process.env.JWT_SECRET || 'plataforma-cursos-segredo');
    next();
  } catch {
    return res.status(401).json({ message: 'Token invalido ou expirado.' });
  }
}

function adminOnly(req, res, next) {
  if (req.user?.perfil !== 'ADMIN') {
    return res.status(403).json({ message: 'Acesso permitido somente para administradores.' });
  }
  next();
}

module.exports = { auth, adminOnly };
=======

  if (!token) {
    return res.status(401).json({ message: 'Token nao informado.' });
  }

  let payload;

  try {
    payload = jwt.verify(
      token,
      process.env.JWT_SECRET || 'plataforma-cursos-segredo'
    );
  } catch (error) {
    return res.status(401).json({ message: 'Token invalido ou expirado.' });
  }

  if (!Number.isInteger(Number(payload.id)) || Number(payload.id) <= 0) {
    return res.status(401).json({ message: 'Token invalido.' });
  }

  getPool()
    .then((pool) => pool.request()
      .input('id', sql.Int, Number(payload.id))
      .query(`
        SELECT Id, Nome, Email, Perfil, Ativo
        FROM dbo.Usuarios
        WHERE Id = @id
      `))
    .then((result) => {
      const user = result.recordset[0];

      if (!user || !user.Ativo) {
        return res.status(401).json({
          message: 'Sessao encerrada. Usuario inexistente ou inativo.',
        });
      }

      req.user = {
        id: user.Id,
        nome: user.Nome,
        email: user.Email,
        perfil: user.Perfil,
      };

      next();
    })
    .catch((error) => {
      console.error('Falha ao validar usuario autenticado:', error.message);
      res.status(503).json({
        message: 'Nao foi possivel validar sua sessao no banco de dados.',
      });
    });
}

module.exports = {
  auth,
  allowRoles,
  adminOnly,
  professorOnly,
  studentOnly,
};
>>>>>>> b3854cb (melhorias funções dos usuários / gamificação / melhorias api)
