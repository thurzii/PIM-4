function allowRoles(...profiles) {
  return (req, res, next) => {
    if (!req.user || !profiles.includes(req.user.perfil)) {
      return res.status(403).json({
        message: 'Voce nao tem permissao para realizar esta operacao.',
      });
    }

    next();
  };
}

function canAccessCourse(user, course) {
  if (!user || !course) {
    return false;
  }

  if (user.perfil === 'ADMIN') {
    return true;
  }

  if (user.perfil === 'PROFESSOR') {
    return Number(course.professorId) === Number(user.id);
  }

  if (user.perfil === 'ALUNO') {
    return course.ativo === true || course.ativo === 1;
  }

  return false;
}

const adminOnly = allowRoles('ADMIN');
const professorOnly = allowRoles('PROFESSOR');
const studentOnly = allowRoles('ALUNO');

module.exports = {
  allowRoles,
  canAccessCourse,
  adminOnly,
  professorOnly,
  studentOnly,
};
