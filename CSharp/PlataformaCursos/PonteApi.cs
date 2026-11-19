/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: PonteApi.cs
   Professor.: Antonio Graça
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

   QUANDO E USADA
   So com a API ligada (ConfiguracaoApi.ATIVA = true). Com ela
   desligada o programa usa o banco local, como antes - o modo de
   testes combinado pelo grupo enquanto a API nao estiver no Azure.

   O QUE AINDA NAO TEM ROTA
   Algumas telas usam operacoes que a API ainda nao oferece (editar e
   excluir usuario, editar aula, historico de acoes, progresso de outro
   aluno, recuperacao de senha). Nesses casos o metodo devolve false com
   a mensagem SEM_ROTA, em vez de cair no banco local escondido: metade
   no banco e metade na API e justamente o que o grupo quis evitar.
   ===================================================================== */

using System;
using System.Collections.Generic;

namespace PlataformaCursos
{
    public class PonteApi
    {
        public const string SEM_ROTA =
            "Esta operacao ainda nao existe na API. Ela volta a funcionar " +
            "quando a rota for criada.";

        // DIFERENCA EM RELACAO A WEB: no desktop o cliente e um so para o
        // programa inteiro (ConfiguracaoApi.ObterCliente) e guarda o token
        // do login. Na web cada requisicao cria o seu e o token vem do
        // cookie. O resto desta classe e igual nas duas.
        public bool Ativa()
        {
            return ConfiguracaoApi.ATIVA;
        }

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

            afr_mensagem = "A API nao respondeu: " + afr_resposta.afr_Mensagem;
            return false;
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

        // POST /api/auth/recover - hoje sempre 501
        public string RecuperarSenha(string afr_email)
        {
            RespostaApi afr_resposta = Cliente().RecuperarSenha(afr_email);

            if (afr_resposta.afr_CodigoHttp == 501)
            {
                return "A recuperacao de senha ainda nao existe na API. " +
                       "Procure o administrador para trocar a senha.";
            }

            return afr_resposta.afr_Mensagem;
        }

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

        // Nao ha rota de "um usuario": procura na lista que o perfil
        // logado pode ver
        public Usuario BuscarUsuario(int afr_id)
        {
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

            afr_mensagem = afr_resposta.afr_Ok ? "Usuario cadastrado." : afr_resposta.afr_Mensagem;
            return afr_resposta.afr_Ok;
        }

        // PUT /api/admin/usuarios/:id/status - a unica edicao que existe
        public bool AlterarStatusUsuario(int afr_id, bool afr_ativo, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Cliente().AlterarStatusUsuario(afr_id, afr_ativo);

            afr_mensagem = afr_resposta.afr_Ok
                ? (afr_ativo ? "Usuario ativado." : "Usuario desativado.")
                : afr_resposta.afr_Mensagem;

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
                afr_mensagem = "Na API, so o administrador exclui curso.";
                return false;
            }

            RespostaApi afr_resposta = Cliente().ExcluirCurso(afr_cursoId);

            afr_mensagem = afr_resposta.afr_Ok ? "Curso excluido." : afr_resposta.afr_Mensagem;
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
                afr_lista.Add(afr_item.ParaAula(afr_cursoId));
            }

            return afr_lista;
        }

        // -------------------------------------------------------------
        // Nao ha rota de "uma aula". A aula e procurada dentro dos
        // cursos que o perfil enxerga. Sao poucos cursos, entao o custo
        // e pequeno - mas a rota GET /api/aulas/:id resolveria melhor.
        //
        // A API tambem nao guarda o TEXTO da aula (nao existe a coluna
        // Conteudo no banco dela), entao a aula chega sem conteudo.
        // -------------------------------------------------------------
        public Aula AbrirAula(int afr_aulaId)
        {
            foreach (Curso afr_curso in ListarCursos(""))
            {
                foreach (Aula afr_aula in ListarAulas(afr_curso.afr_Id))
                {
                    if (afr_aula.afr_Id == afr_aulaId)
                    {
                        return afr_aula;
                    }
                }
            }

            return null;
        }

        public bool CriarAula(Aula afr_aula, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Sessao.EhAdministrador()
                ? Cliente().CriarAula(afr_aula.afr_CursoId, afr_aula.afr_Titulo, afr_aula.afr_DuracaoMin)
                : Cliente().CriarAulaProfessor(afr_aula.afr_CursoId, afr_aula.afr_Titulo,
                                               afr_aula.afr_DuracaoMin);

            afr_mensagem = afr_resposta.afr_Ok ? "Aula incluida." : afr_resposta.afr_Mensagem;
            return afr_resposta.afr_Ok;
        }

        public bool ExcluirAula(int afr_aulaId, out string afr_mensagem)
        {
            RespostaApi afr_resposta = Sessao.EhAdministrador()
                ? Cliente().ExcluirAula(afr_aulaId)
                : Cliente().ExcluirAulaProfessor(afr_aulaId);

            afr_mensagem = afr_resposta.afr_Ok ? "Aula excluida." : afr_resposta.afr_Mensagem;
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

            afr_mensagem = afr_concluida ? "Aula concluida." : "Aula desmarcada.";
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

        // -------------------------------------------------------------
        // O XP de OUTRO aluno, para a tela do professor. Sai da linha
        // dele no ranking: XP e sequencia vem, as medalhas nao (a rota
        // nao devolve conquistas).
        // -------------------------------------------------------------
        public Pontuacao PontuacaoDoAluno(int afr_alunoId)
        {
            Pontuacao afr_pontuacao = new Pontuacao();
            afr_pontuacao.afr_UsuarioId = afr_alunoId;

            foreach (LinhaRanking afr_linha in Ranking())
            {
                if (afr_linha.afr_UsuarioId == afr_alunoId)
                {
                    afr_pontuacao.afr_XpTotal = afr_linha.afr_XpTotal;
                    afr_pontuacao.afr_SequenciaAtual = afr_linha.afr_SequenciaAtual;
                    afr_pontuacao.afr_MelhorSequencia = afr_linha.afr_MelhorSequencia;
                }
            }

            return afr_pontuacao;
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

        // A mesma conta do BancoService.MontarResultado, com o minimo
        // fixo em 100 (ver ListarCursos)
        private ResultadoConclusao MontarResultado(int afr_concluidas, int afr_total)
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
