require('dotenv').config();
const express = require('express');
const cors = require('cors');
const jwt = require('jsonwebtoken');
const { sql, getPool } = require('./db');
const { auth, adminOnly } = require('./middleware');
const { awardLessonCompletion } = require('./gamificationService');


const crypto = require('crypto');
const { promisify } = require('util');
const scrypt = promisify(crypto.scrypt);
async function hashPassword(password) {
  const salt = crypto.randomBytes(16).toString('hex');
  const hash = await scrypt(password, salt, 64);
  return `scrypt:${salt}:${hash.toString('hex')}`;
}
async function verifyPassword(password, stored) {
  if (typeof stored !== 'string') return false;
  if (!stored.startsWith('scrypt:')) return stored === password; // compatibilidade com contas antigas
  const parts = stored.split(':');
  if (parts.length !== 3 || !/^[a-f0-9]{32}$/.test(parts[1]) || !/^[a-f0-9]{128}$/.test(parts[2])) return false;
  const expected = Buffer.from(parts[2], 'hex');
  const actual = await scrypt(password, parts[1], expected.length);
  return crypto.timingSafeEqual(expected, actual);
}
function validateAccount(body, publicSignup = false) {
  const nome = typeof body?.nome === 'string' ? body.nome.trim() : '';
  const email = typeof body?.email === 'string' ? body.email.trim().toLowerCase() : '';
  const senha = typeof body?.senha === 'string' ? body.senha : '';
  const perfil = publicSignup ? 'ALUNO' : (typeof body?.perfil === 'string' ? body.perfil.toUpperCase() : '');
  if (nome.length < 3 || nome.length > 200) return { error: 'Informe um nome entre 3 e 200 caracteres.' };
  if (email.length > 200 || !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) return { error: 'Informe um email valido.' };
  if (senha.length < 8 || senha.length > 128) return { error: 'A senha deve ter entre 8 e 128 caracteres.' };
  if (publicSignup && senha !== body.confirmarSenha) return { error: 'As senhas nao conferem.' };
  if (!['ALUNO', 'PROFESSOR', 'ADMIN'].includes(perfil)) return { error: 'Perfil invalido.' };
  return { nome, email, senha, perfil };
}
async function createAccount(body, publicSignup, res) {
  const data = validateAccount(body, publicSignup);
  if (data.error) return res.status(400).json({ message: data.error });
  try {
    const pool = await getPool();
    const senhaHash = await hashPassword(data.senha);
    const result = await pool.request()
      .input('nome', sql.NVarChar(200), data.nome)
      .input('email', sql.NVarChar(200), data.email)
      .input('senha', sql.NVarChar(200), senhaHash)
      .input('perfil', sql.NVarChar(20), data.perfil)
      .query(`INSERT INTO dbo.Usuarios (Nome, Email, Senha, Perfil, Ativo, DataCadastro)
              OUTPUT INSERTED.Id VALUES (@nome, @email, @senha, @perfil, 1, GETDATE())`);
    return res.status(201).json({ id: result.recordset[0].Id, message: 'Usuario criado com sucesso.' });
  } catch (error) {
    if (error.number === 2627 || error.number === 2601 || /duplicate|unique|duplicad/i.test(error.message))
      return res.status(409).json({ message: 'Este email ja esta cadastrado.' });
    return res.status(500).json({ message: 'Erro ao criar usuario.' });
  }
}

const app = express();
app.use(cors());
app.use(express.json());

const PORT = process.env.PORT || 3000;
const SECRET = process.env.JWT_SECRET || 'plataforma-cursos-segredo';

function tokenFor(user) {
  return jwt.sign({ id: user.Id, nome: user.Nome, email: user.Email, perfil: user.Perfil }, SECRET, { expiresIn: '8h' });
}

app.get('/api/health', async (req, res) => {
  try {
    const pool = await getPool();
    await pool.request().query('SELECT 1 AS ok');
    res.json({ api: 'ok', database: 'ok' });
  } catch (e) {
    res.status(500).json({ api: 'ok', database: 'error', message: e.message });
  }
});

// Cadastro publico desabilitado. Apenas /api/admin/usuarios pode criar usuarios.
app.post('/api/auth/login', async (req, res) => {
  const { email, senha } = req.body || {};
  if (typeof email !== 'string' || typeof senha !== 'string' || !email.trim() || !senha)
    return res.status(400).json({ message: 'Informe email e senha.' });
  try {
    const pool = await getPool();
    const result = await pool.request().input('email', sql.NVarChar(200), email.trim().toLowerCase())
      .query('SELECT TOP 1 Id, Nome, Email, Senha, Perfil, Ativo FROM dbo.Usuarios WHERE Email=@email');
    const user = result.recordset[0];
    if (!user || !(await verifyPassword(senha, user.Senha)))
      return res.status(401).json({ message: 'Email ou senha incorretos.' });
    if (!user.Ativo) return res.status(403).json({ message: 'Usuario inativo.' });
    return res.json({ token: tokenFor(user), user: { id: user.Id, nome: user.Nome, email: user.Email, perfil: user.Perfil } });
  } catch (e) { return res.status(500).json({ message: 'Erro ao autenticar.' }); }
});

app.post('/api/auth/recover', (req, res) => res.status(501).json({
  message: 'Recuperacao automatica indisponivel. Solicite a redefinicao ao administrador.'
}));

// Cursos e matriculas: implementacao unica no roteador dedicado.
app.use('/api/cursos', require('./routes/courseRoutes'));

// ---------------- ADMIN: CURSOS ----------------
app.get('/api/admin/cursos', auth, adminOnly, async (req, res) => {
  const termo = (req.query.busca || '').trim();
  try {
    const pool = await getPool();
    const result = await pool.request().input('termo', sql.NVarChar(200), termo).query(`SELECT c.Id AS id,c.Titulo AS titulo,c.Descricao AS descricao,c.CargaHoraria AS cargaHoraria,c.Ativo AS ativo,(SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId=c.Id) AS totalAulas FROM dbo.Cursos c WHERE @termo='' OR c.Titulo LIKE '%'+@termo+'%' OR c.Descricao LIKE '%'+@termo+'%' ORDER BY c.Titulo`);
    res.json(result.recordset);
  } catch (e) { res.status(500).json({ message: 'Erro ao listar cursos.', detail: e.message }); }
});

app.post('/api/admin/cursos', auth, adminOnly, async (req, res) => {
  const { titulo, descricao, cargaHoraria, ativo = true } = req.body;
  if (!titulo) return res.status(400).json({ message: 'Titulo obrigatorio.' });
  try {
    const pool = await getPool();
    const r = await pool.request().input('titulo',sql.NVarChar(200),titulo).input('descricao',sql.NVarChar(sql.MAX),descricao||'').input('carga',sql.Int,Number(cargaHoraria)||0).input('ativo',sql.Bit,Boolean(ativo)).query(`INSERT INTO dbo.Cursos(Titulo,Descricao,CargaHoraria,Ativo,DataCadastro) OUTPUT INSERTED.Id VALUES(@titulo,@descricao,@carga,@ativo,GETDATE())`);
    res.status(201).json({ id:r.recordset[0].Id, message:'Curso criado.' });
  } catch(e){ res.status(500).json({message:'Erro ao criar curso.',detail:e.message}); }
});

app.put('/api/admin/cursos/:id', auth, adminOnly, async (req,res)=>{
  const {titulo,descricao,cargaHoraria,ativo=true}=req.body;
  try{ const pool=await getPool(); const r=await pool.request().input('id',sql.Int,Number(req.params.id)).input('titulo',sql.NVarChar(200),titulo).input('descricao',sql.NVarChar(sql.MAX),descricao||'').input('carga',sql.Int,Number(cargaHoraria)||0).input('ativo',sql.Bit,Boolean(ativo)).query('UPDATE dbo.Cursos SET Titulo=@titulo,Descricao=@descricao,CargaHoraria=@carga,Ativo=@ativo WHERE Id=@id'); if(!r.rowsAffected[0])return res.status(404).json({message:'Curso nao encontrado.'}); res.json({message:'Curso atualizado.'}); }catch(e){res.status(500).json({message:'Erro ao atualizar curso.',detail:e.message});}
});

app.delete('/api/admin/cursos/:id', auth, adminOnly, async (req,res)=>{
  try{const pool=await getPool();const id=Number(req.params.id);const tx=new sql.Transaction(pool);await tx.begin();try{await new sql.Request(tx).input('id',sql.Int,id).query('DELETE p FROM dbo.Progresso p INNER JOIN dbo.Aulas a ON a.Id=p.AulaId WHERE a.CursoId=@id');await new sql.Request(tx).input('id',sql.Int,id).query('DELETE FROM dbo.Aulas WHERE CursoId=@id');const r=await new sql.Request(tx).input('id',sql.Int,id).query('DELETE FROM dbo.Cursos WHERE Id=@id');await tx.commit();if(!r.rowsAffected[0])return res.status(404).json({message:'Curso nao encontrado.'});res.json({message:'Curso excluido.'});}catch(e){await tx.rollback();throw e;}}catch(e){res.status(500).json({message:'Erro ao excluir curso.',detail:e.message});}
});

// ---------------- ADMIN: AULAS ----------------
app.post('/api/admin/cursos/:cursoId/aulas', auth, adminOnly, async (req,res)=>{
  const cursoId=Number(req.params.cursoId); const {titulo,duracaoMin}=req.body;
  if(!titulo)return res.status(400).json({message:'Titulo obrigatorio.'});
  try{const pool=await getPool();const r=await pool.request().input('cursoId',sql.Int,cursoId).input('titulo',sql.NVarChar(300),titulo).input('duracao',sql.Int,Number(duracaoMin)||0).query(`DECLARE @ordem INT=(SELECT ISNULL(MAX(Ordem),0)+1 FROM dbo.Aulas WHERE CursoId=@cursoId); INSERT INTO dbo.Aulas(CursoId,Titulo,Ordem,DuracaoMin) OUTPUT INSERTED.Id VALUES(@cursoId,@titulo,@ordem,@duracao)`);res.status(201).json({id:r.recordset[0].Id,message:'Aula criada.'});}catch(e){res.status(500).json({message:'Erro ao criar aula.',detail:e.message});}
});

app.delete('/api/admin/aulas/:id', auth, adminOnly, async (req,res)=>{try{const pool=await getPool();const r=await pool.request().input('id',sql.Int,Number(req.params.id)).query('DELETE FROM dbo.Aulas WHERE Id=@id');if(!r.rowsAffected[0])return res.status(404).json({message:'Aula nao encontrada.'});res.json({message:'Aula excluida.'});}catch(e){res.status(500).json({message:'Erro ao excluir aula.',detail:e.message});}});

app.use('/api/professor', require('./routes/professorRoutes'));
app.use('/api/gamificacao', require('./routes/gamificationRoutes'));

// ---------------- ADMIN: USUARIOS ----------------
app.post('/api/admin/usuarios', auth, adminOnly, (req, res) => createAccount(req.body, false, res));

app.get('/api/admin/usuarios', auth, adminOnly, async (req,res)=>{const termo=(req.query.busca||'').trim();try{const pool=await getPool();const r=await pool.request().input('termo',sql.NVarChar(200),termo).query(`SELECT Id AS id,Nome AS nome,Email AS email,Perfil AS perfil,Ativo AS ativo FROM dbo.Usuarios WHERE @termo='' OR Nome LIKE '%'+@termo+'%' OR Email LIKE '%'+@termo+'%' ORDER BY Nome`);res.json(r.recordset);}catch(e){res.status(500).json({message:'Erro ao listar usuarios.',detail:e.message});}});

app.put('/api/admin/usuarios/:id/status', auth, adminOnly, async (req,res)=>{try{const pool=await getPool();const r=await pool.request().input('id',sql.Int,Number(req.params.id)).input('ativo',sql.Bit,Boolean(req.body.ativo)).query('UPDATE dbo.Usuarios SET Ativo=@ativo WHERE Id=@id');if(!r.rowsAffected[0])return res.status(404).json({message:'Usuario nao encontrado.'});res.json({message:'Status atualizado.'});}catch(e){res.status(500).json({message:'Erro ao atualizar usuario.',detail:e.message});}});

app.use((req,res)=>res.status(404).json({message:'Rota nao encontrada.'}));
app.listen(PORT,()=>console.log(`API Plataforma Cursos rodando em http://localhost:${PORT}`));
