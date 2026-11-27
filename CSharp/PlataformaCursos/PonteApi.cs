/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: PonteApi.cs
   ---------------------------------------------------------------------
   VERSAO 4 - a ponte entre os servicos e a API Node.js

   POR QUE ESTE ARQUIVO EXISTE
   O grupo decidiu que os tres programas falam com o banco do Azure
   ATRAVES DA API, e so ela mexe no banco. Assim a regra de XP, de
   matricula e de permissao mora em um lugar so, em vez de uma copia em
   C# e outra em JavaScript que podiam divergir.

   O ClienteApi.cs sabe CHAMAR cada rota. Esta classe sabe QUAL rota
   cada tela precisa e converte a resposta para as classes que o
   programa ja usa (Usuario, Curso, Aula, Pontuacao...). Com isso as
   telas e os servicos continuam iguais: so o lugar de onde vem o dado
   muda.

   O DESKTOP SO FALA COM A API
   Nao existe mais banco local. A API e a do grupo, unificada pelo
   Guilherme e hospedada no Render (ver ConfiguracaoApi); o aplicativo
   e a web usam a mesma.

   O QUE AINDA NAO DA
   Historico de acoes: a API nao tem a rota. A tela de perfil mostra as
   listas vazias ate a rota existir.
   ===================================================================== */

using System;
using System.Collections.Generic;

namespace PlataformaCursos
{
    public class PonteApi
    {
        // DIFERENCA EM RELACAO A WEB: no desktop o cliente e um so para o
        // programa inteiro (ConfiguracaoApi.ObterCliente) e guarda o token
        // do login. Na web cada requisicao cria o seu e o token vem do
        // cookie. O resto desta classe e igual nas duas.
        public string ObterDescricao()
        {
            return ConfiguracaoApi.ObterDescricao();
        }

        private ClienteApi Cliente()
        {
            return ConfiguracaoApi.ObterCliente();
        }

        // A sessao do desktop guarda o usuario inteiro na memoria
        private Usuario Logado()
        {
            Usuario afr_usuario = Sessao.ObterUsuario();

            return afr_usuario ?? new Usuario();
        }

        // =============================================================
        // CONEXAO E LOGIN
        // =============================================================

        // GET /api/health - a API esta no ar e conseguiu falar com o banco?
        public bool TestarConexao(out string afr_mensagem)
        {
            RespostaApi afr_resposta = Cliente().Verificar();

            if (afr_resposta.afr_Ok)
            {
                afr_mensagem = "API no ar e conectada ao banco (" +
                               ConfiguracaoApi.URL_BASE + ").";
                return true;
            }

            // Sem resposta nenhuma, a mensagem do ClienteApi comeca com
            // "Nao foi possivel falar com a API" - o AguardarApi usa isso.
            // Com resposta, e a API dizendo que o banco nao respondeu.
            afr_mensagem = afr_resposta.afr_CodigoHttp == 0
                ? afr_resposta.afr_Mensagem
                : "A API respondeu, mas o banco não: " + afr_resposta.afr_Mensagem;
            return false;
        }

        // -------------------------------------------------------------
        // VERSAO 6 - espera a API ficar pronta.
        //
        // Pelo perfil "API + ..." o Visual Studio sobe a API e o programa
        // ao mesmo tempo, e o programa costuma ficar pronto primeiro.
        // Em vez de acusar "a API nao respondeu", tenta de novo por alguns
        // segundos. Quando a API ja esta no ar, responde na primeira.
        // -------------------------------------------------------------
        public bool AguardarApi(int afr_segundos, out string afr_mensagem)
        {
            DateTime afr_limite = DateTime.Now.AddSeconds(afr_segundos);

            while (true)
            {
                if (TestarConexao(out afr_mensagem))
                {
                    return true;
                }

                // Se a API respondeu mas o banco nao (firewall do Azure,
                // por exemplo), esperar nao adianta
                if (!afr_mensagem.Contains("Não foi possível falar com a API") ||
                    DateTime.Now >= afr_limite)
                {
                    return false;
                }

                System.Threading.Thread.Sleep(500);
            }
        }

        // POST /api/auth/login. Devolve o usuario e, por fora, o token.
        public Usuario Entrar(string afr_email, string afr_senha,
                              out string afr_token, out string afr_mensagem)
        {
            return Entrar(Cliente(), afr_email, afr_senha, out afr_token, out afr_mensagem);
        }

        // O login de verdade guarda o token no cliente do programa. A
        // confirmacao de senha (abaixo) usa um cliente descartavel, para
        // nao trocar o token de quem ja esta logado.
        private Usuario Entrar(ClienteApi afr_novo, string afr_email, string afr_senha,
                               out string afr_token, out string afr_mensagem)
        {
            afr_token = "";

            RespostaApi afr_resposta = afr_novo.Entrar(afr_email, afr_senha);

            if (!afr_resposta.afr_Ok)
            {
                // 401 vira "Sessao expirada" no ClienteApi; no login o
                // certo e dizer que a senha nao confere
                afr_mensagem = afr_resposta.afr_CodigoHttp == 401
                    ? "Email ou senha incorretos."
                    : afr_resposta.afr_Mensagem;
                return null;
            }

            LoginApi afr_login = afr_resposta.Converter<LoginApi>();

            if (afr_login == null || afr_login.afr_Usuario == null)
            {
                afr_mensagem = "A API respondeu em um formato inesperado.";
                return null;
            }

            afr_token = afr_novo.ObterToken();
            afr_mensagem = "";

            Usuario afr_usuario = afr_login.afr_Usuario.ParaUsuario();

            // A API so devolve o login de quem esta ativo
            afr_usuario.afr_Ativo = true;

            return afr_usuario;
        }

        // DS10 e DS14 - confirmar a senha antes de excluir. Sem rota
        // propria: refaz o login com a senha digitada. Se a API aceitar,
        // a senha confere.
        public bool ConfirmarSenha(string afr_senha, out string afr_mensagem)
        {
            string afr_tokenIgnorado;

            Usuario afr_usuario = Entrar(new ClienteApi(ConfiguracaoApi.URL_BASE),
                                         Logado().afr_Email, afr_senha,
                                         out afr_tokenIgnorado, out afr_mensagem);

            return afr_usuario != null;
        }

        // Os mesmos atalhos da PonteApi da web (cadastro de aluno, primeiro
        // acesso). Cada chamada usa um cliente novo, sem token: sao rotas
        // de quem ainda nao entrou no sistema.
        public RespostaApi LoginPublico(string afr_email, string afr_senha)
        {
            return new ClienteApi(ConfiguracaoApi.URL_BASE).Entrar(afr_email, afr_senha);
        }

        public RespostaApi Registrar(string afr_nome, string afr_email, string afr_senha, string afr_confirmarSenha)
        {
            return new ClienteApi(ConfiguracaoApi.URL_BASE).CadastrarAluno(afr_nome, afr_email, afr_senha, afr_confirmarSenha);
        }

        public RespostaApi PrimeiroAcesso(string afr_token, string afr_temporaria, string afr_nova)
        {
            return new ClienteApi(ConfiguracaoApi.URL_BASE).PrimeiroAcesso(afr_token, afr_temporaria, afr_nova);
        }

        public RespostaApi RecuperarComCodigo(string email, string codigo, string senha) => Cliente().RecuperarSenha(email, codigo, senha);


        // =============================================================
        // USUARIOS
        // =============================================================

        // GET /api/admin/usuarios (so admin)
        public List<Usuario> ListarUsuarios(string afr_termo)
        {
            List<Usuario> afr_lista = new List<Usuario>();

            RespostaApi afr_resposta = Cliente().ListarUsuarios(afr_termo);
            List<UsuarioApi> afr_itens = afr_resposta.afr_Ok
                ? afr_resposta.Converter<List<UsuarioApi>>()
                : null;

            if (afr_itens != null)
            {
                foreach (UsuarioApi afr_item in afr_itens)
                {
                    afr_lista.Add(afr_item.ParaUsuario());
                }
            }

            return afr_lista;
        }

        // -------------------------------------------------------------
        // Os alunos que o professor acompanha.
        //
        // A lista de usuarios e adminOnly, entao professor nao pode usa-
        // la. A rota do ranking (/gamificacao/turma) aceita professor e
        // admin e ja traz todos os alunos ativos com id, nome e email -
        // e dela que a lista sai.
        // -------------------------------------------------------------
        public List<Usuario> ListarAlunos(string afr_termo)
        {
            List<Usuario> afr_lista = new List<Usuario>();
            string afr_busca = (afr_termo ?? "").Trim().ToLowerInvariant();

            foreach (LinhaRanking afr_linha in Ranking())
            {
                bool afr_combina = afr_busca == "" ||
                                   afr_linha.afr_Nome.ToLowerInvariant().Contains(afr_busca) ||
                                   afr_linha.afr_Email.ToLowerInvariant().Contains(afr_busca);

                if (afr_combina)
                {
                    Usuario afr_aluno = new Usuario();
                    afr_aluno.afr_Id = afr_linha.afr_UsuarioId;
                    afr_aluno.afr_Nome = afr_linha.afr_Nome;
                    afr_aluno.afr_Email = afr_linha.afr_Email;
                    afr_aluno.afr_Perfil = Usuario.PERFIL_ALUNO;
                    afr_aluno.afr_Ativo = true;

                    afr_lista.Add(afr_aluno);
                }
            }

            afr_lista.Sort((a, b) => string.Compare(a.afr_Nome, b.afr_Nome, StringComparison.OrdinalIgnoreCase));

            return afr_lista;
        }

        // Admin: GET /api/admin/usuarios/:id (rota da nossa API, traz a
        // data de cadastro). Professor: procura na lista de alunos.
        public Usuario BuscarUsuario(int afr_id)
        {
            if (Sessao.EhAdministrador())
            {
                RespostaApi afr_resposta = Cliente().ObterUsuario(afr_id);
                UsuarioApi afr_daApi = afr_resposta.afr_Ok ? afr_resposta.Converter<UsuarioApi>() : null;

                if (afr_daApi != null)
                {
                    return afr_daApi.ParaUsuario();
                }
            }

            List<Usuario> afr_lista = Sessao.EhAdministrador()
                ? ListarUsuarios("")
                : ListarAlunos("");

            foreach (Usuario afr_usuario in afr_lista)
            {
                if (afr_usuario.afr_Id == afr_id)
                {
                    return afr_usuario;
                }
            }

            // O proprio usuario logado sempre existe, mesmo que a lista
            // dele nao o inclua (aluno nao lista ninguem)
            if (afr_id == Sessao.ObterUsuarioId())
            {
                return Logado();
            }

            return null;
        }

        // POST /api/admin/usuarios. A API grava a senha em scrypt.
        public bool CriarUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Cliente().CriarUsuario(
                afr_usuario.afr_Nome, afr_usuario.afr_Email,
                afr_usuario.afr_Senha, afr_usuario.afr_Perfil);

            afr_mensagem = afr_resposta.afr_Ok ? "Usuário cadastrado. Entregue a senha temporária ao usuário." : afr_resposta.afr_Mensagem;
            return afr_resposta.afr_Ok;
        }

        // VERSAO 5 - PUT /api/admin/usuarios/:id (rota da nossa API).
        // Senha vazia = manter a atual. Quem impede o admin de tirar o
        // proprio acesso ou o do ultimo admin e a API.
        public bool EditarUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Cliente().AlterarUsuario(
                afr_usuario.afr_Id, afr_usuario.afr_Nome, afr_usuario.afr_Email,
                afr_usuario.afr_Senha, afr_usuario.afr_Perfil, afr_usuario.afr_Ativo);

            afr_mensagem = afr_resposta.afr_Ok ? "Usuário atualizado." : afr_resposta.afr_Mensagem;
            return afr_resposta.afr_Ok;
        }

        // VERSAO 5 - DELETE /api/admin/usuarios/:id (rota da nossa API)
        public bool ExcluirUsuario(int afr_id, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Cliente().ExcluirUsuario(afr_id);

            afr_mensagem = afr_resposta.afr_Ok ? "Usuário excluído." : afr_resposta.afr_Mensagem;
            return afr_resposta.afr_Ok;
        }

        // =============================================================
        // CURSOS
        // =============================================================

        // -------------------------------------------------------------
        // Cada perfil tem a sua rota de listagem:
        //   ALUNO ...... GET /api/cursos            (so os ativos)
        //   PROFESSOR .. GET /api/professor/cursos  (so os dele)
        //   ADMIN ...... GET /api/admin/cursos      (todos)
        //
        // A API nao tem "percentual minimo": para ela, curso concluido e
        // curso com todas as aulas feitas. Por isso o minimo fica 100.
        // -------------------------------------------------------------
        public List<Curso> ListarCursos(string afr_termo)
        {
            RespostaApi afr_resposta;

            if (Sessao.EhAdministrador())
            {
                afr_resposta = Cliente().ListarCursosAdmin(afr_termo);
            }
            else if (Sessao.EhProfessor())
            {
                afr_resposta = Cliente().ListarCursosProfessor();
            }
            else
            {
                afr_resposta = Cliente().ListarCursos(afr_termo);
            }

            List<Curso> afr_lista = new List<Curso>();
            List<CursoApi> afr_itens = afr_resposta.afr_Ok
                ? afr_resposta.Converter<List<CursoApi>>()
                : null;

            if (afr_itens == null)
            {
                return afr_lista;
            }

            // A rota do professor nao recebe busca: o termo e aplicado aqui
            string afr_busca = (afr_termo ?? "").Trim().ToLowerInvariant();

            foreach (CursoApi afr_item in afr_itens)
            {
                Curso afr_curso = afr_item.ParaCurso();
                afr_curso.afr_PercentualMinimo = 100;

                bool afr_combina = afr_busca == "" ||
                                   afr_curso.afr_Titulo.ToLowerInvariant().Contains(afr_busca) ||
                                   afr_curso.afr_Descricao.ToLowerInvariant().Contains(afr_busca);

                if (afr_combina)
                {
                    afr_lista.Add(afr_curso);
                }
            }

            return afr_lista;
        }

        // GET /api/cursos/:id - o curso com as aulas e o progresso de
        // quem esta logado. Devolve null se nao existir ou se o perfil
        // nao puder ver (professor so ve os cursos dele).
        public CursoDetalheApi ObterDetalhe(int afr_cursoId)
        {
            RespostaApi afr_resposta = Cliente().ObterCurso(afr_cursoId);

            if (!afr_resposta.afr_Ok)
            {
                return null;
            }

            return afr_resposta.Converter<CursoDetalheApi>();
        }

        public Curso ObterCurso(int afr_cursoId)
        {
            CursoDetalheApi afr_detalhe = ObterDetalhe(afr_cursoId);

            if (afr_detalhe == null)
            {
                return null;
            }

            Curso afr_curso = afr_detalhe.ParaCurso();
            afr_curso.afr_PercentualMinimo = 100;
            afr_curso.afr_TotalAulas = afr_detalhe.afr_Aulas.Count;

            return afr_curso;
        }

        public bool CriarCurso(Curso afr_curso, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Sessao.EhAdministrador()
                ? Cliente().CriarCurso(afr_curso.afr_Titulo, afr_curso.afr_Descricao,
                                       afr_curso.afr_CargaHoraria, afr_curso.afr_Ativo)
                : Cliente().CriarCursoProfessor(afr_curso.afr_Titulo, afr_curso.afr_Descricao,
                                                afr_curso.afr_CargaHoraria);

            afr_mensagem = afr_resposta.afr_Ok ? "Curso cadastrado." : afr_resposta.afr_Mensagem;
            return afr_resposta.afr_Ok;
        }

        public bool EditarCurso(Curso afr_curso, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Sessao.EhAdministrador()
                ? Cliente().AlterarCurso(afr_curso.afr_Id, afr_curso.afr_Titulo,
                                         afr_curso.afr_Descricao, afr_curso.afr_CargaHoraria,
                                         afr_curso.afr_Ativo)
                : Cliente().AlterarCursoProfessor(afr_curso.afr_Id, afr_curso.afr_Titulo,
                                                  afr_curso.afr_Descricao,
                                                  afr_curso.afr_CargaHoraria);

            afr_mensagem = afr_resposta.afr_Ok ? "Curso atualizado." : afr_resposta.afr_Mensagem;
            return afr_resposta.afr_Ok;
        }

        // DELETE /api/admin/cursos/:id - so o admin tem essa rota
        public bool ExcluirCurso(int afr_cursoId, out string afr_mensagem)
        {
            if (!Sessao.EhAdministrador())
            {
                afr_mensagem = "Na API, só o administrador exclui curso.";
                return false;
            }

            RespostaApi afr_resposta = Cliente().ExcluirCurso(afr_cursoId);

            afr_mensagem = afr_resposta.afr_Ok ? "Curso excluído." : afr_resposta.afr_Mensagem;
            return afr_resposta.afr_Ok;
        }

        // POST /api/cursos/:id/matricula (so aluno)
        public bool Matricular(int afr_cursoId, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Cliente().MatricularEmCurso(afr_cursoId);

            afr_mensagem = afr_resposta.afr_Ok
                ? "Matricula realizada. Bons estudos!"
                : afr_resposta.afr_Mensagem;

            return afr_resposta.afr_Ok;
        }

        public bool EstaMatriculado(int afr_cursoId)
        {
            CursoDetalheApi afr_detalhe = ObterDetalhe(afr_cursoId);

            return afr_detalhe != null && afr_detalhe.afr_Matriculado;
        }

        // =============================================================
        // AULAS
        // =============================================================

        public List<Aula> ListarAulas(int afr_cursoId)
        {
            List<Aula> afr_lista = new List<Aula>();
            CursoDetalheApi afr_detalhe = ObterDetalhe(afr_cursoId);

            if (afr_detalhe == null)
            {
                return afr_lista;
            }

            foreach (AulaApi afr_item in afr_detalhe.afr_Aulas)
            {
                Aula afr_aula = afr_item.ParaAula(afr_cursoId);

                // A rota do curso nao manda o TEXTO das aulas. Para quem
                // gerencia (professor e admin), o texto vem da rota de cada
                // aula - sem isso toda aula pareceria "sem conteudo".
                if (afr_item.Conteudo == null && !(!Sessao.EhAdministrador() && !Sessao.EhProfessor()))
                {
                    Aula afr_completa = AbrirAula(afr_aula.afr_Id);

                    if (afr_completa != null)
                    {
                        afr_aula.afr_Conteudo = afr_completa.afr_Conteudo;
                    }
                }

                afr_lista.Add(afr_aula);
            }

            return afr_lista;
        }

        // -------------------------------------------------------------
        // VERSAO 5 - GET /api/aulas/:id (rota da nossa API)
        //
        // O banco do Azure nao tem a coluna do TEXTO da aula, entao a
        // aula chega sem conteudo ate o grupo combinar essa coluna.
        // -------------------------------------------------------------
        public Aula AbrirAula(int afr_aulaId)
        {
            RespostaApi afr_resposta = Cliente().AbrirAula(afr_aulaId);
            AulaApi afr_daApi = afr_resposta.afr_Ok ? afr_resposta.Converter<AulaApi>() : null;

            return afr_daApi != null ? afr_daApi.ParaAula(afr_daApi.afr_CursoId) : null;
        }

        // VERSAO 5 - titulo e duracao; o texto da aula nao tem coluna
        public bool EditarAula(Aula afr_aula, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Sessao.EhAdministrador()
                ? Cliente().AlterarAula(afr_aula.afr_Id, afr_aula.afr_Titulo, afr_aula.afr_DuracaoMin, afr_aula.afr_Conteudo)
                : Cliente().AlterarAulaProfessor(afr_aula.afr_Id, afr_aula.afr_Titulo, afr_aula.afr_DuracaoMin, afr_aula.afr_Conteudo);

            afr_mensagem = afr_resposta.afr_Ok
                ? "Aula atualizada."
                : afr_resposta.afr_Mensagem;

            return afr_resposta.afr_Ok;
        }

        public bool CriarAula(Aula afr_aula, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Sessao.EhAdministrador()
                ? Cliente().CriarAula(afr_aula.afr_CursoId, afr_aula.afr_Titulo, afr_aula.afr_DuracaoMin, afr_aula.afr_Conteudo)
                : Cliente().CriarAulaProfessor(afr_aula.afr_CursoId, afr_aula.afr_Titulo,
                                               afr_aula.afr_DuracaoMin, afr_aula.afr_Conteudo);

            afr_mensagem = afr_resposta.afr_Ok ? "Aula incluída." : afr_resposta.afr_Mensagem;
            return afr_resposta.afr_Ok;
        }

        public bool ExcluirAula(int afr_aulaId, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Sessao.EhAdministrador()
                ? Cliente().ExcluirAula(afr_aulaId)
                : Cliente().ExcluirAulaProfessor(afr_aulaId);

            afr_mensagem = afr_resposta.afr_Ok ? "Aula excluída." : afr_resposta.afr_Mensagem;
            return afr_resposta.afr_Ok;
        }

        // =============================================================
        // PROGRESSO E XP
        // =============================================================

        // -------------------------------------------------------------
        // PUT /api/cursos/:c/aulas/:a/progresso
        //
        // A API confere a matricula, grava o progresso e paga o XP na
        // mesma chamada - e por isso o C# nao tem mais regra de XP no
        // modo API.
        // -------------------------------------------------------------
        public bool MarcarProgresso(int afr_cursoId, int afr_aulaId, bool afr_concluida,
                                    out GanhoGamificacao afr_ganho, out string afr_mensagem)
        {
            afr_ganho = new GanhoGamificacao();

            RespostaApi afr_resposta = Cliente().MarcarProgresso(afr_cursoId, afr_aulaId, afr_concluida);

            if (!afr_resposta.afr_Ok)
            {
                afr_mensagem = afr_resposta.afr_Mensagem;
                return false;
            }

            RespostaProgressoApi afr_corpo = afr_resposta.Converter<RespostaProgressoApi>();

            if (afr_corpo != null && afr_corpo.afr_Gamificacao != null)
            {
                afr_ganho = afr_corpo.afr_Gamificacao.ParaGanho();
            }

            afr_mensagem = afr_concluida ? "Aula concluída." : "Aula desmarcada.";
            return true;
        }

        // O progresso de quem esta logado, curso a curso
        public List<ProgressoCurso> MeuProgresso()
        {
            List<ProgressoCurso> afr_lista = new List<ProgressoCurso>();

            foreach (Curso afr_curso in ListarCursos(""))
            {
                CursoDetalheApi afr_detalhe = ObterDetalhe(afr_curso.afr_Id);

                if (afr_detalhe == null)
                {
                    continue;
                }

                afr_curso.afr_TotalAulas = afr_detalhe.afr_Aulas.Count;

                ProgressoCurso afr_item = new ProgressoCurso();
                afr_item.afr_Curso = afr_curso;
                afr_item.afr_Resultado = MontarResultado(afr_detalhe.afr_Concluidas,
                                                         afr_detalhe.afr_Aulas.Count);

                afr_lista.Add(afr_item);
            }

            return afr_lista;
        }

        public int ContarConcluidas(int afr_cursoId)
        {
            CursoDetalheApi afr_detalhe = ObterDetalhe(afr_cursoId);

            return afr_detalhe != null ? afr_detalhe.afr_Concluidas : 0;
        }

        // GET /api/gamificacao/me - so do proprio aluno
        public Pontuacao MinhaPontuacao()
        {
            RespostaApi afr_resposta = Cliente().ObterGamificacao();
            PontuacaoApi afr_corpo = afr_resposta.afr_Ok
                ? afr_resposta.Converter<PontuacaoApi>()
                : null;

            if (afr_corpo == null)
            {
                Pontuacao afr_vazia = new Pontuacao();
                afr_vazia.afr_UsuarioId = Sessao.ObterUsuarioId();
                return afr_vazia;
            }

            return afr_corpo.ParaPontuacao(Sessao.ObterUsuarioId());
        }

        // VERSAO 5 - o XP e as medalhas de OUTRO aluno, para a tela do
        // professor: GET /api/gamificacao/alunos/:id (rota da nossa API)
        public Pontuacao PontuacaoDoAluno(int afr_alunoId)
        {
            RespostaApi afr_resposta = Cliente().ObterGamificacaoDoAluno(afr_alunoId);
            PontuacaoApi afr_corpo = afr_resposta.afr_Ok ? afr_resposta.Converter<PontuacaoApi>() : null;

            if (afr_corpo == null)
            {
                Pontuacao afr_vazia = new Pontuacao();
                afr_vazia.afr_UsuarioId = afr_alunoId;
                return afr_vazia;
            }

            return afr_corpo.ParaPontuacao(afr_alunoId);
        }

        // VERSAO 5 - o progresso de OUTRO aluno, curso a curso:
        // GET /api/progresso/alunos/:id (rota da nossa API)
        public List<ProgressoCurso> ProgressoDoAluno(int afr_alunoId)
        {
            List<ProgressoCurso> afr_lista = new List<ProgressoCurso>();

            RespostaApi afr_resposta = Cliente().ObterProgressoDoAluno(afr_alunoId);
            List<ProgressoAlunoApi> afr_itens = afr_resposta.afr_Ok
                ? afr_resposta.Converter<List<ProgressoAlunoApi>>()
                : null;

            if (afr_itens == null)
            {
                return afr_lista;
            }

            foreach (ProgressoAlunoApi afr_item in afr_itens)
            {
                Curso afr_curso = afr_item.ParaCurso();
                afr_curso.afr_PercentualMinimo = 100;

                ProgressoCurso afr_linha = new ProgressoCurso();
                afr_linha.afr_Curso = afr_curso;
                afr_linha.afr_Resultado = MontarResultado(afr_item.afr_Concluidas, afr_item.afr_TotalAulas);

                afr_lista.Add(afr_linha);
            }

            return afr_lista;
        }

        // GET /api/gamificacao/turma (professor e admin)
        public List<LinhaRanking> Ranking()
        {
            List<LinhaRanking> afr_lista = new List<LinhaRanking>();

            RespostaApi afr_resposta = Cliente().ObterRankingTurma();
            List<RankingApi> afr_itens = afr_resposta.afr_Ok
                ? afr_resposta.Converter<List<RankingApi>>()
                : null;

            if (afr_itens != null)
            {
                foreach (RankingApi afr_item in afr_itens)
                {
                    afr_lista.Add(afr_item.ParaLinhaRanking());
                }
            }

            return afr_lista;
        }

        // Percentual e aprovacao de um curso, com o minimo fixo em 100
        // (para a API, curso concluido e curso com todas as aulas feitas)
        public static ResultadoConclusao MontarResultado(int afr_concluidas, int afr_total)
        {
            ResultadoConclusao afr_resultado = new ResultadoConclusao();

            afr_resultado.afr_Concluidas = afr_concluidas;
            afr_resultado.afr_Total = afr_total;
            afr_resultado.afr_PercentualMinimo = 100;
            afr_resultado.afr_Percentual = afr_total > 0
                ? (int)Math.Round((double)afr_concluidas / afr_total * 100)
                : 0;
            afr_resultado.afr_Aprovado = afr_total > 0 && afr_concluidas >= afr_total;
            afr_resultado.afr_Requisitos = "Concluir todas as aulas do curso.";

            return afr_resultado;
        }
    }

    // A resposta de PUT .../progresso: { message, gamificacao }
    public class RespostaProgressoApi
    {
        [System.Text.Json.Serialization.JsonPropertyName("gamificacao")]
        public GamificacaoApi afr_Gamificacao { get; set; }
    }
}
