const jwt = require('jsonwebtoken');

function auth(req, res, next) {
  const header = req.headers.authorization || '';
  const token = header.startsWith('Bearer ') ? header.slice(7) : null;
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
