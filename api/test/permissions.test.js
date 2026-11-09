const test = require('node:test');
const assert = require('node:assert/strict');
const {
  adminOnly,
  professorOnly,
  studentOnly,
  canAccessCourse,
} = require('../src/permissions');

function runGuard(guard, user) {
  const req = { user };
  let statusCode = 200;
  let responseBody = null;
  let nextCalled = false;

  const res = {
    status(code) {
      statusCode = code;
      return this;
    },
    json(body) {
      responseBody = body;
      return this;
    },
  };

  guard(req, res, () => {
    nextCalled = true;
  });

  return { statusCode, responseBody, nextCalled };
}

test('ADMIN pode acessar rotas administrativas', () => {
  const result = runGuard(adminOnly, { id: 1, perfil: 'ADMIN' });
  assert.equal(result.nextCalled, true);
  assert.equal(result.statusCode, 200);
});

test('PROFESSOR nao pode acessar rotas administrativas', () => {
  const result = runGuard(adminOnly, { id: 2, perfil: 'PROFESSOR' });
  assert.equal(result.nextCalled, false);
  assert.equal(result.statusCode, 403);
});

test('PROFESSOR pode acessar rotas exclusivas de professor', () => {
  const result = runGuard(professorOnly, { id: 2, perfil: 'PROFESSOR' });
  assert.equal(result.nextCalled, true);
});

test('ALUNO nao pode acessar rotas de professor', () => {
  const result = runGuard(professorOnly, { id: 3, perfil: 'ALUNO' });
  assert.equal(result.nextCalled, false);
  assert.equal(result.statusCode, 403);
});

test('ALUNO pode acessar rotas exclusivas de aluno', () => {
  const result = runGuard(studentOnly, { id: 3, perfil: 'ALUNO' });
  assert.equal(result.nextCalled, true);
});

test('ADMIN nao pode registrar progresso como aluno', () => {
  const result = runGuard(studentOnly, { id: 1, perfil: 'ADMIN' });
  assert.equal(result.nextCalled, false);
  assert.equal(result.statusCode, 403);
});

test('requisicao sem usuario nao passa pela verificacao de perfil', () => {
  const result = runGuard(adminOnly, null);
  assert.equal(result.nextCalled, false);
  assert.equal(result.statusCode, 403);
});

test('ADMIN pode consultar qualquer curso', () => {
  assert.equal(
    canAccessCourse({ id: 1, perfil: 'ADMIN' }, { professorId: 20, ativo: false }),
    true
  );
});

test('PROFESSOR pode consultar somente o proprio curso', () => {
  assert.equal(
    canAccessCourse({ id: 20, perfil: 'PROFESSOR' }, { professorId: 20, ativo: true }),
    true
  );
  assert.equal(
    canAccessCourse({ id: 20, perfil: 'PROFESSOR' }, { professorId: 21, ativo: true }),
    false
  );
});

test('ALUNO pode consultar somente cursos ativos', () => {
  assert.equal(
    canAccessCourse({ id: 30, perfil: 'ALUNO' }, { professorId: 20, ativo: true }),
    true
  );
  assert.equal(
    canAccessCourse({ id: 30, perfil: 'ALUNO' }, { professorId: 20, ativo: false }),
    false
  );
});
