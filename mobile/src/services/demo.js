const users = [
  { id: 1, nome: 'Maria Silva', email: 'maria@escola.com', perfil: 'ALUNO', ativo: true },
  { id: 2, nome: 'Professor Demonstração', email: 'professor@escola.com', perfil: 'PROFESSOR', ativo: true },
  { id: 3, nome: 'Administrador', email: 'admin@escola.com', perfil: 'ADMIN', ativo: true },
];

let nextCourseId = 3;
let courses = [
  { id: 1, titulo: 'Introdução à Programação', descricao: 'Aprenda os fundamentos da lógica de programação e dê seus primeiros passos no desenvolvimento.', cargaHoraria: 12, totalAulas: 4, totalAlunos: 18, ativo: true, matriculado: true, professorId: 2,
    aulas: [{ id: 1, ordem: 1, titulo: 'O que é programação?', duracaoMin: 20, concluida: true }, { id: 2, ordem: 2, titulo: 'Lógica e algoritmos', duracaoMin: 35, concluida: false }, { id: 3, ordem: 3, titulo: 'Variáveis e tipos', duracaoMin: 30, concluida: false }, { id: 4, ordem: 4, titulo: 'Praticando', duracaoMin: 40, concluida: false }] },
  { id: 2, titulo: 'Banco de Dados SQL', descricao: 'Conheça tabelas, consultas e relacionamentos com SQL.', cargaHoraria: 8, totalAulas: 3, totalAlunos: 12, ativo: true, matriculado: false, professorId: 2,
    aulas: [{ id: 5, ordem: 1, titulo: 'Conceitos de banco de dados', duracaoMin: 25, concluida: false }, { id: 6, ordem: 2, titulo: 'SELECT e filtros', duracaoMin: 35, concluida: false }, { id: 7, ordem: 3, titulo: 'Relacionamentos', duracaoMin: 40, concluida: false }] },
];
let xp = 10;
let streak = 2;
let history = [];
let demoEnabled = false;
export function setDemoMode(value) { demoEnabled = !!value; }
export function isDemoMode() { return demoEnabled; }
export function demoLogin(email, password) {
  const credentials = {
    'maria@escola.com': ['aluno123', users[0]],
    'professor@escola.com': ['professor123', users[1]],
    'admin@escola.com': ['admin123', users[2]],
  };
  const entry = credentials[email.trim().toLowerCase()];
  if (!entry || entry[0] !== password) throw new Error('E-mail ou senha de demonstração inválidos.');
  demoEnabled = true;
  return { ...entry[1], modoDemo: true };
}
function response(data) { return { data, status: 200 }; }
function fail(message, status = 404) { const e = new Error(message); e.response = { data: { message }, status }; throw e; }
function courseView(course) {
  const aulas = course.aulas || [];
  const concluidas = aulas.filter(a => a.concluida).length;
  return { ...course, aulas: aulas.map(a => ({ ...a })), totalAulas: aulas.length, concluidas, progresso: aulas.length ? Math.round(concluidas * 100 / aulas.length) : 0, matriculado: !!course.matriculado };
}
export async function demoRequest(method, url, config = {}) {
  if (!demoEnabled) return null;
  const path = url.replace(/^https?:\/\/[^/]+\/api/, '').split('?')[0];
  const body = config.data || {};
  const params = config.params || {};
  if (method === 'post' && path === '/auth/login') fail('Use as credenciais de demonstração na tela de login.', 401);
  if (method === 'get' && path === '/cursos') return response(courses.filter(c => c.ativo && (!params.busca || c.titulo.toLowerCase().includes(params.busca.toLowerCase()))).map(c => ({ ...courseView(c), aulas: undefined })));
  let m = path.match(/^\/cursos\/(\d+)$/);
  if (method === 'get' && m) { const c = courses.find(x => x.id === +m[1]); if (!c) fail('Curso não encontrado.'); return response(courseView(c)); }
  m = path.match(/^\/cursos\/(\d+)\/matricula$/);
  if (method === 'post' && m) { const c = courses.find(x => x.id === +m[1]); if (!c) fail('Curso não encontrado.'); c.matriculado = true; return response({ message: 'Matrícula realizada.' }); }
  m = path.match(/^\/cursos\/(\d+)\/aulas\/(\d+)\/progresso$/);
  if (method === 'put' && m) {
    const c = courses.find(x => x.id === +m[1]); const a = c?.aulas.find(x => x.id === +m[2]);
    if (!c || !a) fail('Aula não encontrada.'); if (!c.matriculado) fail('Você precisa estar matriculado.', 403);
    let xpGanho = 0; if (body.concluida && !a.concluida) { xpGanho = 10; xp += xpGanho; history.push(new Date().toISOString().slice(0,10)); }
    a.concluida = !!body.concluida;
    return response({ message: 'Progresso atualizado.', gamificacao: { xpGanho, xpTotal: xp, novasConquistas: [] } });
  }
  if (method === 'get' && path === '/gamificacao/me') {
    const total = courses.reduce((sum,c) => sum + c.aulas.filter(a => a.concluida).length, 0);
    const conquistas = [
      ['primeira_aula','Primeiros passos','Conclua sua primeira aula','🌱',20,total >= 1],
      ['cinco_aulas','Ritmo de estudos','Conclua cinco aulas','📚',30,total >= 5],
      ['dez_aulas','Estudante dedicado','Conclua dez aulas','⭐',50,total >= 10],
      ['curso_concluido','Curso concluído','Conclua todas as aulas de um curso','🎓',100,courses.some(c => c.aulas.length && c.aulas.every(a => a.concluida))],
      ['sete_dias','Uma semana de foco','Estude por sete dias consecutivos','🔥',75,false],
    ].map((a,i) => ({ id:i+1,codigo:a[0],nome:a[1],descricao:a[2],icone:a[3],xpBonus:a[4],conquistada:a[5] }));
    return response({ xp, nivel: Math.floor(xp/100)+1, xpNoNivel: xp%100, progressoNivel: xp%100, sequenciaAtual: streak, melhorSequencia: streak, conquistas });
  }
  if (method === 'get' && path === '/professor/cursos') return response(courses.filter(c => c.professorId === 2).map(courseView));
  if (method === 'get' && path === '/admin/cursos') return response(courses.map(courseView));
  if (method === 'get' && path === '/admin/usuarios') return response(users);
  if (method === 'post' && path === '/admin/usuarios') { const u = { id: users.length+1, ...body, ativo:true }; users.push(u); return response(u); }
  m = path.match(/^\/admin\/usuarios\/(\d+)\/status$/);
  if (method === 'put' && m) { const u=users.find(x=>x.id===+m[1]); if(!u) fail('Usuário não encontrado.'); u.ativo=!!body.ativo; return response(u); }
  m = path.match(/^\/professor\/cursos\/(\d+)\/alunos$/);
  if (method === 'get' && m) return response([{ id:1,nome:'Maria Silva',email:'maria@escola.com',dataMatricula:new Date().toISOString(),progresso:courseView(courses.find(c=>c.id===+m[1])||courses[0]).progresso,aulasConcluidas:1,totalAulas:4 }]);
  return fail('Esta operação ainda não está disponível no modo demonstração.', 501);
}
