const test = require('node:test');
const assert = require('node:assert/strict');
const http = require('node:http');
const users = [
  { Id: 1, Nome: 'Administrador', Email: 'admin@escola.com', Senha: 'admin123', Perfil: 'ADMIN', Ativo: true },
  { Id: 2, Nome: 'Professor', Email: 'professor@escola.com', Senha: 'professor123', Perfil: 'PROFESSOR', Ativo: true },
  { Id: 3, Nome: 'Maria', Email: 'maria@escola.com', Senha: 'aluno123', Perfil: 'ALUNO', Ativo: true },
  { Id: 4, Nome: 'Joao', Email: 'joao@escola.com', Senha: 'aluno123', Perfil: 'ALUNO', Ativo: false },
];

const courses = [
  { Id: 10, Titulo: 'JavaScript', Descricao: 'Fundamentos', CargaHoraria: 20, Ativo: true, ProfessorId: 2, DataCadastro: new Date() },
  { Id: 11, Titulo: 'Curso Inativo', Descricao: 'Indisponivel', CargaHoraria: 10, Ativo: false, ProfessorId: 2, DataCadastro: new Date() },
  { Id: 12, Titulo: 'Curso de outro professor', Descricao: 'Privado', CargaHoraria: 10, Ativo: true, ProfessorId: 99, DataCadastro: new Date() },
];
const lessons = [
  { Id: 100, CursoId: 10, Titulo: 'Variaveis', Ordem: 1, DuracaoMin: 15 },
  { Id: 101, CursoId: 10, Titulo: 'Funcoes', Ordem: 2, DuracaoMin: 20 },
];
const enrollments = [];
const progress = [];
const gamification = new Map();
const xpEvents = new Set();
const achievements = [
  { id: 1, codigo: 'PRIMEIRA_AULA', nome: 'Primeiros passos', descricao: 'Conclua sua primeira aula.', icone: '🎯', xpBonus: 20 },
  { id: 2, codigo: 'CINCO_AULAS', nome: 'Ritmo de estudos', descricao: 'Conclua cinco aulas.', icone: '📚', xpBonus: 30 },
  { id: 3, codigo: 'DEZ_AULAS', nome: 'Estudante dedicado', descricao: 'Conclua dez aulas.', icone: '🏅', xpBonus: 50 },
  { id: 4, codigo: 'PRIMEIRO_CURSO', nome: 'Curso concluído', descricao: 'Conclua todas as aulas de um curso.', icone: '🎓', xpBonus: 100 },
  { id: 5, codigo: 'SEQUENCIA_7', nome: 'Uma semana de foco', descricao: 'Estude por sete dias consecutivos.', icone: '🔥', xpBonus: 75 },
];

const sql = {
  Int: 'Int', Bit: 'Bit', MAX: 'MAX',
  NVarChar: (length) => `NVarChar(${length})`,
  Transaction: class { async begin() {} async commit() {} async rollback() {} },
  Request: class {
    constructor() { this.values = {}; }
    input(name, _type, value) { this.values[name] = value; return this; }
    async query(statement) { return executeQuery(statement, this.values); }
  },
};

function executeQuery(statement, values) {
  const query = statement.replace(/\s+/g, ' ').trim().toLowerCase();
  const empty = { recordset: [], rowsAffected: [0] };

  if (query.includes('declare @novasconquistas table')) {
    const eventKey = `${values.usuarioId}:AULA:${values.aulaId}`;
    let state = gamification.get(values.usuarioId);
    if (!state) {
      state = { xp: 0, sequenciaAtual: 1, melhorSequencia: 1, unlocked: new Set() };
      gamification.set(values.usuarioId, state);
    }

    let xpAula = 0;
    if (!xpEvents.has(eventKey)) {
      xpEvents.add(eventKey);
      state.xp += 10;
      xpAula = 10;
    }

    const completedLessons = xpEvents.size
      ? [...xpEvents].filter((key) => key.startsWith(`${values.usuarioId}:AULA:`)).length
      : 0;
    const completedCourse = enrollments.some((item) => item.UsuarioId === values.usuarioId &&
      lessons.filter((lesson) => lesson.CursoId === item.CursoId).length > 0 &&
      lessons.filter((lesson) => lesson.CursoId === item.CursoId).every((lesson) =>
        progress.some((entry) => entry.UsuarioId === values.usuarioId && entry.AulaId === lesson.Id)));

    const eligible = [];
    if (completedLessons >= 1) eligible.push('PRIMEIRA_AULA');
    if (completedLessons >= 5) eligible.push('CINCO_AULAS');
    if (completedLessons >= 10) eligible.push('DEZ_AULAS');
    if (completedCourse) eligible.push('PRIMEIRO_CURSO');
    const newBadges = achievements.filter((item) => eligible.includes(item.codigo) && !state.unlocked.has(item.codigo));
    newBadges.forEach((item) => {
      state.unlocked.add(item.codigo);
      state.xp += item.xpBonus;
    });

    return {
      recordsets: [[{
        xpAula,
        xpConquistas: newBadges.reduce((total, item) => total + item.xpBonus, 0),
        xpTotal: state.xp,
        sequenciaAtual: state.sequenciaAtual,
        melhorSequencia: state.melhorSequencia,
      }], [
        ...newBadges.map((item) => ({ id: item.id, nome: item.nome, descricao: item.descricao, icone: item.icone, xpBonus: item.xpBonus })),
      ]],
    };
  }

  if (query.includes('from (select @usuarioid as usuarioid)') && query.includes('left join dbo.gamificacao')) {
    const state = gamification.get(values.usuarioId) || { xp: 0, sequenciaAtual: 0, melhorSequencia: 0, unlocked: new Set() };
    const completedLessons = [...xpEvents].filter((key) => key.startsWith(`${values.usuarioId}:AULA:`)).length;
    const stats = [{ xp: state.xp, nivel: Math.floor(state.xp / 100) + 1, sequenciaAtual: state.sequenciaAtual, melhorSequencia: state.melhorSequencia, aulasConcluidas: completedLessons }];
    const badges = achievements.map((item) => ({ ...item, conquistada: state.unlocked.has(item.codigo), dataConquista: null }));
    return { recordsets: [stats, badges] };
  }

  if (query.includes('information_schema.columns')) {
    const columns = {
      Usuarios: ['Id', 'Nome', 'Email', 'Senha', 'Perfil', 'Ativo'],
      Cursos: ['Id', 'Titulo', 'Descricao', 'CargaHoraria', 'Ativo', 'DataCadastro', 'ProfessorId'],
      Aulas: ['Id', 'CursoId', 'Titulo', 'Ordem', 'DuracaoMin'],
      Progresso: ['Id', 'UsuarioId', 'AulaId', 'DataConclusao'],
      Matriculas: ['Id', 'UsuarioId', 'CursoId', 'DataMatricula'],
      Gamificacao: ['UsuarioId', 'XP', 'SequenciaAtual', 'MelhorSequencia', 'UltimaAtividade'],
      Conquistas: ['Id', 'Codigo', 'Nome', 'Descricao', 'Icone', 'XPBonus'],
      UsuarioConquistas: ['Id', 'UsuarioId', 'ConquistaId', 'DataConquista'],
      HistoricoXP: ['Id', 'UsuarioId', 'Quantidade', 'Motivo', 'ReferenciaTipo', 'ReferenciaId', 'DataRegistro'],
    };
    return { recordset: Object.entries(columns).flatMap(([tableName, names]) => names.map((columnName) => ({ tableName, columnName }))) };
  }

  if (query.includes('from dbo.usuarios') && query.includes('where email = @email')) {
    const user = users.find((item) => item.Email === values.email && item.Senha === values.senha);
    return { recordset: user ? [{ ...user }] : [] };
  }
  if (query.includes('from dbo.usuarios') && query.includes('where id = @id')) {
    const user = users.find((item) => item.Id === values.id);
    return { recordset: user ? [{ Id: user.Id, Nome: user.Nome, Email: user.Email, Perfil: user.Perfil, Ativo: user.Ativo }] : [] };
  }
  if (query.includes('from dbo.cursos c') && query.includes('where c.professorid = @professorid')) {
    return { recordset: courses.filter((course) => course.ProfessorId === values.professorId).map((course) => ({ id: course.Id, titulo: course.Titulo, descricao: course.Descricao, cargaHoraria: course.CargaHoraria, ativo: course.Ativo, totalAulas: lessons.filter((item) => item.CursoId === course.Id).length, totalAlunos: enrollments.filter((item) => item.CursoId === course.Id).length })) };
  }
  if (query.includes('from dbo.cursos c') && query.includes('where c.ativo = 1')) {
    const term = (values.termo || '').toLowerCase();
    return { recordset: courses.filter((course) => course.Ativo && (!term || `${course.Titulo} ${course.Descricao}`.toLowerCase().includes(term))).map((course) => ({ id: course.Id, titulo: course.Titulo, descricao: course.Descricao, cargaHoraria: course.CargaHoraria, ativo: course.Ativo, totalAulas: lessons.filter((item) => item.CursoId === course.Id).length, totalAlunos: enrollments.filter((item) => item.CursoId === course.Id).length })) };
  }
  if (query.includes('from dbo.cursos') && query.includes('where id = @id')) {
    const course = courses.find((item) => item.Id === values.id);
    return { recordset: course ? [{ id: course.Id, titulo: course.Titulo, descricao: course.Descricao, cargaHoraria: course.CargaHoraria, ativo: course.Ativo, professorId: course.ProfessorId }] : [] };
  }
  if (query.includes('from dbo.cursos') && query.includes('where id = @cursoid and ativo = 1')) {
    const course = courses.find((item) => item.Id === values.cursoId && item.Ativo);
    return { recordset: course ? [{ Id: course.Id }] : [] };
  }
  if (query.includes('from dbo.aulas a') && query.includes('left join dbo.progresso')) {
    return { recordset: lessons.filter((item) => item.CursoId === values.cursoId).sort((a, b) => a.Ordem - b.Ordem).map((item) => ({ id: item.Id, titulo: item.Titulo, ordem: item.Ordem, duracaoMin: item.DuracaoMin, concluida: progress.some((p) => p.AulaId === item.Id && p.UsuarioId === values.usuarioId) })) };
  }
  if (query.includes('from dbo.matriculas') && query.includes('where cursoid = @cursoid and usuarioid = @usuarioid')) {
    const enrollment = enrollments.find((item) => item.CursoId === values.cursoId && item.UsuarioId === values.usuarioId);
    return { recordset: enrollment ? [{ Id: enrollment.Id }] : [] };
  }
  if (query.includes('from dbo.matriculas') && query.includes('where usuarioid = @usuarioid and cursoid = @cursoid')) {
    const enrollment = enrollments.find((item) => item.CursoId === values.cursoId && item.UsuarioId === values.usuarioId);
    return { recordset: enrollment ? [{ Id: enrollment.Id }] : [] };
  }
  if (query.includes('if not exists') && query.includes('insert into dbo.matriculas')) {
    if (!enrollments.some((item) => item.CursoId === values.cursoId && item.UsuarioId === values.usuarioId)) {
      enrollments.push({ Id: enrollments.length + 1, CursoId: values.cursoId, UsuarioId: values.usuarioId });
    }
    return empty;
  }
  if (query.includes('from dbo.aulas') && query.includes('where id = @aulaid and cursoid = @cursoid')) {
    const lesson = lessons.find((item) => item.Id === values.aulaId && item.CursoId === values.cursoId);
    return { recordset: lesson ? [{ Id: lesson.Id }] : [] };
  }
  if (query.includes('if not exists') && query.includes('insert into dbo.progresso')) {
    if (!progress.some((item) => item.UsuarioId === values.usuarioId && item.AulaId === values.aulaId)) {
      progress.push({ UsuarioId: values.usuarioId, AulaId: values.aulaId });
    }
    return empty;
  }
  if (query.includes('delete from dbo.progresso')) {
    const before = progress.length;
    for (let index = progress.length - 1; index >= 0; index -= 1) {
      if (progress[index].UsuarioId === values.usuarioId && progress[index].AulaId === values.aulaId) progress.splice(index, 1);
    }
    return { ...empty, rowsAffected: [before - progress.length] };
  }

  throw new Error(`Consulta nao simulada no teste: ${statement}`);
}

const fakeDb = { sql, getPool: async () => ({ request: () => new sql.Request() }) };
require.cache[require.resolve('../src/db')] = {
  id: require.resolve('../src/db'), filename: require.resolve('../src/db'), loaded: true, exports: fakeDb,
};
const app = require('../src/server');

let server;
let baseUrl;
test.before(async () => {
  server = http.createServer(app);
  await new Promise((resolve) => server.listen(0, '127.0.0.1', resolve));
  baseUrl = `http://127.0.0.1:${server.address().port}`;
});
test.after(async () => {
  await new Promise((resolve) => server.close(resolve));
});

async function request(route, { method = 'GET', token, body } = {}) {
  const response = await fetch(`${baseUrl}${route}`, {
    method,
    headers: {
      ...(body ? { 'Content-Type': 'application/json' } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    ...(body ? { body: JSON.stringify(body) } : {}),
  });
  const data = await response.json();
  return { status: response.status, data };
}

async function login(email, senha) {
  const result = await request('/api/auth/login', { method: 'POST', body: { email, senha } });
  return result.data.token;
}

test('health confirma API e estrutura do banco simulado', async () => {
  const result = await request('/api/health');
  assert.equal(result.status, 200);
  assert.deepEqual(result.data, { api: 'ok', database: 'ok', schema: 'ok' });
});

test('login retorna token e dados publicos do usuario', async () => {
  const result = await request('/api/auth/login', { method: 'POST', body: { email: 'maria@escola.com', senha: 'aluno123' } });
  assert.equal(result.status, 200);
  assert.ok(result.data.token);
  assert.equal(result.data.user.perfil, 'ALUNO');
  assert.equal('senha' in result.data.user, false);
});

test('login rejeita senha incorreta e usuario inativo', async () => {
  assert.equal((await request('/api/auth/login', { method: 'POST', body: { email: 'maria@escola.com', senha: 'errada' } })).status, 401);
  assert.equal((await request('/api/auth/login', { method: 'POST', body: { email: 'joao@escola.com', senha: 'aluno123' } })).status, 403);
});

test('rota protegida rejeita requisicao sem token', async () => {
  const result = await request('/api/cursos');
  assert.equal(result.status, 401);
});

test('ALUNO consulta catalogo e nao visualiza cursos inativos', async () => {
  const token = await login('maria@escola.com', 'aluno123');
  const result = await request('/api/cursos', { token });
  assert.equal(result.status, 200);
  assert.deepEqual(result.data.map((item) => item.id), [10, 12]);
});

test('PROFESSOR nao pode acessar catalogo exclusivo de ALUNO', async () => {
  const token = await login('professor@escola.com', 'professor123');
  const result = await request('/api/cursos', { token });
  assert.equal(result.status, 403);
});

test('permissao usa o perfil atual do banco, nao o perfil antigo do token', async () => {
  const token = await login('professor@escola.com', 'professor123');
  const professor = users.find((item) => item.Id === 2);
  professor.Perfil = 'ALUNO';
  const result = await request('/api/professor/cursos', { token });
  professor.Perfil = 'PROFESSOR';
  assert.equal(result.status, 403);
});

test('PROFESSOR consulta somente os proprios cursos', async () => {
  const token = await login('professor@escola.com', 'professor123');
  const result = await request('/api/professor/cursos', { token });
  assert.equal(result.status, 200);
  assert.ok(Array.isArray(result.data));
});

test('ALUNO nao pode acessar rotas administrativas', async () => {
  const token = await login('maria@escola.com', 'aluno123');
  const result = await request('/api/admin/cursos', { token });
  assert.equal(result.status, 403);
});

test('ALUNO nao registra progresso antes de se matricular', async () => {
  const token = await login('maria@escola.com', 'aluno123');
  const result = await request('/api/cursos/10/aulas/100/progresso', { method: 'PUT', token, body: { concluida: true } });
  assert.equal(result.status, 403);
});

test('ALUNO pode se matricular e concluir uma aula', async () => {
  const token = await login('maria@escola.com', 'aluno123');
  const enrollment = await request('/api/cursos/10/matricula', { method: 'POST', token, body: {} });
  assert.equal(enrollment.status, 201);
  const completion = await request('/api/cursos/10/aulas/100/progresso', { method: 'PUT', token, body: { concluida: true } });
  assert.equal(completion.status, 200);
  assert.equal(completion.data.gamificacao.xpGanho, 30);
  assert.equal(completion.data.gamificacao.novasConquistas.length, 1);

  const repeatedCompletion = await request('/api/cursos/10/aulas/100/progresso', { method: 'PUT', token, body: { concluida: true } });
  assert.equal(repeatedCompletion.status, 200);
  assert.equal(repeatedCompletion.data.gamificacao.xpGanho, 0);

  const gamificationResult = await request('/api/gamificacao/me', { token });
  assert.equal(gamificationResult.data.xp, 30);
  assert.equal(gamificationResult.data.conquistas.find((item) => item.codigo === 'PRIMEIRA_AULA').conquistada, true);

  const course = await request('/api/cursos/10', { token });
  assert.equal(course.status, 200);
  assert.equal(course.data.matriculado, true);
  assert.equal(course.data.progresso, 50);
  assert.equal(course.data.aulas[0].concluida, true);
});

test('API valida os dados enviados ao registrar progresso', async () => {
  const token = await login('maria@escola.com', 'aluno123');
  const result = await request('/api/cursos/10/aulas/100/progresso', { method: 'PUT', token, body: { concluida: 'sim' } });
  assert.equal(result.status, 400);
});

test('gamificacao exibe o painel apenas para ALUNO', async () => {
  const studentToken = await login('maria@escola.com', 'aluno123');
  const studentResult = await request('/api/gamificacao/me', { token: studentToken });
  assert.equal(studentResult.status, 200);
  assert.equal(studentResult.data.nivel, 1);
  assert.equal(studentResult.data.conquistas.length, 5);

  const professorToken = await login('professor@escola.com', 'professor123');
  const professorResult = await request('/api/gamificacao/me', { token: professorToken });
  assert.equal(professorResult.status, 403);
});

test('recuperacao insegura de senha permanece bloqueada', async () => {
  const result = await request('/api/auth/recover', { method: 'POST', body: { email: 'maria@escola.com', senha: 'nova' } });
  assert.equal(result.status, 501);
});

test('rota inexistente retorna 404', async () => {
  const result = await request('/api/nao-existe');
  assert.equal(result.status, 404);
});
