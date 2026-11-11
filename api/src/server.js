require('dotenv').config();
const express = require('express');
const cors = require('cors');
const jwt = require('jsonwebtoken');
const { sql, getPool } = require('./db');
const { auth, adminOnly } = require('./middleware');

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

app.post('/api/auth/login', async (req, res) => {
  const { email, senha } = req.body;
  if (!email || !senha) return res.status(400).json({ message: 'Informe email e senha.' });
  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('email', sql.NVarChar(200), email.trim())
      .input('senha', sql.NVarChar(200), senha)
      .query(`SELECT TOP 1 Id, Nome, Email, Perfil, Ativo FROM dbo.Usuarios WHERE Email=@email AND Senha=@senha`);
    const user = result.recordset[0];
    if (!user) return res.status(401).json({ message: 'Email ou senha incorretos.' });
    if (!user.Ativo) return res.status(403).json({ message: 'Usuario inativo.' });
    res.json({ token: tokenFor(user), user: { id: user.Id, nome: user.Nome, email: user.Email, perfil: user.Perfil } });
  } catch (e) {
    res.status(500).json({ message: 'Erro ao autenticar.', detail: e.message });
  }
});

app.post('/api/auth/recover', async (req, res) => {
  const { email, novaSenha } = req.body;
  if (!email || !novaSenha) return res.status(400).json({ message: 'Informe email e nova senha.' });
  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('email', sql.NVarChar(200), email.trim())
      .input('senha', sql.NVarChar(200), novaSenha)
      .query('UPDATE dbo.Usuarios SET Senha=@senha WHERE Email=@email');
    if (!result.rowsAffected[0]) return res.status(404).json({ message: 'Email nao encontrado.' });
    res.json({ message: 'Senha redefinida com sucesso.' });
  } catch (e) { res.status(500).json({ message: 'Erro ao redefinir senha.', detail: e.message }); }
});

app.get('/api/cursos', auth, async (req, res) => {
  const termo = (req.query.busca || '').trim();
  try {
    const pool = await getPool();
    const result = await pool.request()
      .input('termo', sql.NVarChar(200), termo)
      .query(`SELECT c.Id AS id, c.Titulo AS titulo, c.Descricao AS descricao, c.CargaHoraria AS cargaHoraria, c.Ativo AS ativo,
        (SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId=c.Id) AS totalAulas
        FROM dbo.Cursos c WHERE c.Ativo=1 AND (@termo='' OR c.Titulo LIKE '%'+@termo+'%' OR c.Descricao LIKE '%'+@termo+'%') ORDER BY c.Titulo`);
    res.json(result.recordset);
  } catch (e) { res.status(500).json({ message: 'Erro ao listar cursos.', detail: e.message }); }
});

app.get('/api/cursos/:id', auth, async (req, res) => {
  const id = Number(req.params.id);
  try {
    const pool = await getPool();
    const course = await pool.request().input('id', sql.Int, id).query(`SELECT Id AS id, Titulo AS titulo, Descricao AS descricao, CargaHoraria AS cargaHoraria, Ativo AS ativo FROM dbo.Cursos WHERE Id=@id`);
    if (!course.recordset[0]) return res.status(404).json({ message: 'Curso nao encontrado.' });
    const lessons = await pool.request().input('cursoId', sql.Int, id).input('usuarioId', sql.Int, req.user.id).query(`
      SELECT a.Id AS id, a.Titulo AS titulo, a.Ordem AS ordem, a.DuracaoMin AS duracaoMin,
      CASE WHEN p.Id IS NULL THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END AS concluida
      FROM dbo.Aulas a LEFT JOIN dbo.Progresso p ON p.AulaId=a.Id AND p.UsuarioId=@usuarioId
      WHERE a.CursoId=@cursoId ORDER BY a.Ordem`);
    const total = lessons.recordset.length;
    const done = lessons.recordset.filter(x => x.concluida).length;
    res.json({ ...course.recordset[0], aulas: lessons.recordset, progresso: total ? Math.round(done * 100 / total) : 0, concluidas: done, totalAulas: total });
  } catch (e) { res.status(500).json({ message: 'Erro ao carregar curso.', detail: e.message }); }
});

app.put('/api/cursos/:cursoId/aulas/:aulaId/progresso', auth, async (req, res) => {
  const cursoId = Number(req.params.cursoId); const aulaId = Number(req.params.aulaId);
  const concluida = Boolean(req.body.concluida);
  try {
    const pool = await getPool();
    const check = await pool.request().input('aulaId', sql.Int, aulaId).input('cursoId', sql.Int, cursoId).query('SELECT Id FROM dbo.Aulas WHERE Id=@aulaId AND CursoId=@cursoId');
    if (!check.recordset[0]) return res.status(404).json({ message: 'Aula nao encontrada.' });
    if (concluida) {
      await pool.request().input('usuarioId', sql.Int, req.user.id).input('aulaId', sql.Int, aulaId).query(`IF NOT EXISTS (SELECT 1 FROM dbo.Progresso WHERE UsuarioId=@usuarioId AND AulaId=@aulaId) INSERT INTO dbo.Progresso (UsuarioId,AulaId,DataConclusao) VALUES (@usuarioId,@aulaId,GETDATE())`);
    } else {
      await pool.request().input('usuarioId', sql.Int, req.user.id).input('aulaId', sql.Int, aulaId).query('DELETE FROM dbo.Progresso WHERE UsuarioId=@usuarioId AND AulaId=@aulaId');
    }
    res.json({ message: concluida ? 'Aula concluida.' : 'Conclusao removida.' });
  } catch (e) { res.status(500).json({ message: 'Erro ao atualizar progresso.', detail: e.message }); }
});

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

// ---------------- ADMIN: USUARIOS ----------------
app.get('/api/admin/usuarios', auth, adminOnly, async (req,res)=>{const termo=(req.query.busca||'').trim();try{const pool=await getPool();const r=await pool.request().input('termo',sql.NVarChar(200),termo).query(`SELECT Id AS id,Nome AS nome,Email AS email,Perfil AS perfil,Ativo AS ativo FROM dbo.Usuarios WHERE @termo='' OR Nome LIKE '%'+@termo+'%' OR Email LIKE '%'+@termo+'%' ORDER BY Nome`);res.json(r.recordset);}catch(e){res.status(500).json({message:'Erro ao listar usuarios.',detail:e.message});}});

app.put('/api/admin/usuarios/:id/status', auth, adminOnly, async (req,res)=>{try{const pool=await getPool();const r=await pool.request().input('id',sql.Int,Number(req.params.id)).input('ativo',sql.Bit,Boolean(req.body.ativo)).query('UPDATE dbo.Usuarios SET Ativo=@ativo WHERE Id=@id');if(!r.rowsAffected[0])return res.status(404).json({message:'Usuario nao encontrado.'});res.json({message:'Status atualizado.'});}catch(e){res.status(500).json({message:'Erro ao atualizar usuario.',detail:e.message});}});

app.use((req,res)=>res.status(404).json({message:'Rota nao encontrada.'}));
app.listen(PORT,()=>console.log(`API Plataforma Cursos rodando em http://localhost:${PORT}`));
