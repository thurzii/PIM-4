/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Services/Servicos.cs  (regras de negocio)
   ---------------------------------------------------------------------
   Cada metodo publico corresponde a uma mensagem dos diagramas de
   sequencia do Astah; o nome foi mantido igual ao da mensagem sempre
   que possivel.

   VERSAO 5 - SO PELA API
   O site nao tem mais banco proprio. Todos os dados vem da API do grupo
   (a mesma do aplicativo e do desktop), por meio da PonteApi, que sabe
   qual rota chamar e converte a resposta nas classes do Modelo.

   O que ficou AQUI e o que e regra de tela: validar o formulario antes
   de mandar, conferir o perfil antes de mostrar uma area, montar a
   mensagem certa. O que mexe em dados - e as regras de XP, matricula e
   permissao - e da API: assim as tres interfaces seguem uma regra so.

   CLASSES DESTE ARQUIVO:
   1) LogService ........... DS12 - buscarHistoricoAcoes
   2) AutenticacaoService .. DS01, DS06, DS10/DS14 (confirmar senha)
   3) UsuarioService ....... DS11, DS12, DS13, DS14
   4) CursoService ......... DS03, DS04, DS09, DS10
   5) AulaService .......... DS05 - abrirAula / assistirAula
   6) ProgressoService ..... DS05, DS07, DS08
   7) GamificacaoService ... XP, nivel, medalhas e ranking
   ===================================================================== */

using System;
using System.Collections.Generic;
using PlataformaCursosWeb.Models;

namespace PlataformaCursosWeb.Services
{
    // =================================================================
    // CLASSE 1 - LogService
    // DS12 - buscarHistoricoAcoes(usuario)
    //
    // A API do grupo ainda nao guarda historico de acoes. As listas
    // voltam vazias e a tela de perfil mostra "nenhum registro". Quando
    // a API ganhar a rota, so este servico muda.
    // =================================================================
    public class LogService
    {
        public List<LogAcao> BuscarLoginLog(int afr_usuarioId)
        {
            return new List<LogAcao>();
        }

        public List<LogAcao> BuscarAcoesCrud(int afr_usuarioId)
        {
            return new List<LogAcao>();
        }

        public List<LogAcao> BuscarCursosRelacionados(int afr_usuarioId)
        {
            return new List<LogAcao>();
        }
    }

    // =================================================================
    // CLASSE 2 - AutenticacaoService
    // DS01 Login, DS06 Logout, e a confirmacao de senha do DS10/DS14
    // =================================================================
    public class AutenticacaoService
    {
        private readonly SessaoAtual afr_sessao;
        private readonly PonteApi afr_ponte;

        // O token que o ultimo login pela API devolveu
        private string afr_ultimoTokenApi = "";

        public AutenticacaoService(SessaoAtual afr_sessaoAtual, PonteApi afr_ponteApi)
        {
            this.afr_sessao = afr_sessaoAtual;
            this.afr_ponte = afr_ponteApi;
        }

        // -------------------------------------------------------------
        // DS01 - validarLogin(email, senha)
        // Quem confere a senha e a API. Na web, quem abre a sessao (o
        // cookie) e o ContaController.
        // -------------------------------------------------------------
        public Usuario ValidarLogin(string afr_email, string afr_senha, out string afr_mensagem)
        {
            if ((afr_email ?? "").Trim() == "" || (afr_senha ?? "").Trim() == "")
            {
                afr_mensagem = "Informe o email e a senha.";
                return null;
            }

            Usuario afr_usuario = afr_ponte.Entrar(NormalizarEmail(afr_email), afr_senha,
                                                   out afr_ultimoTokenApi, out afr_mensagem);

            if (afr_usuario != null)
            {
                afr_mensagem = "Bem-vindo, " + afr_usuario.ObterPrimeiroNome() + ".";
            }

            return afr_usuario;
        }

        public string ObterUltimoTokenApi()
        {
            return afr_ultimoTokenApi;
        }

        // DS06 - encerrarSessao(): na web quem apaga o cookie e o
        // ContaController (SignOutAsync). Nao ha mais nada a registrar.
        public void EncerrarSessao()
        {
        }

        // A API grava e procura o email com trim().toLowerCase(). Sem
        // normalizar, o mesmo endereco com maiuscula nao seria achado.
        public static string NormalizarEmail(string afr_email)
        {
            if (afr_email == null)
            {
                return "";
            }

            return afr_email.Trim().ToLowerInvariant();
        }

        // -------------------------------------------------------------
        // DS10 e DS14 - confirmarIdentidadeEAcao(ator, alvo)
        // Antes de apagar, o sistema pede a senha de quem esta logado. A
        // senha nao fica no site: quem confere e a API, refazendo o login.
        // -------------------------------------------------------------
        public bool ConfirmarIdentidade(string afr_senhaDigitada, out string afr_mensagem)
        {
            if (!afr_sessao.EstaAberta())
            {
                afr_mensagem = "Sessão encerrada. Entre novamente.";
                return false;
            }

            if (!afr_ponte.ConfirmarSenha(afr_senhaDigitada, out afr_mensagem))
            {
                afr_mensagem = "Senha incorreta. A exclusão foi cancelada.";
                return false;
            }

            afr_mensagem = "Identidade confirmada.";
            return true;
        }
    }

    // =================================================================
    // CLASSE 3 - UsuarioService
    // DS11 Criar, DS12 Visualizar, DS13 Editar, DS14 Excluir
    // =================================================================
    public class UsuarioService
    {
        private readonly SessaoAtual afr_sessao;
        private readonly PonteApi afr_ponte;

        public UsuarioService(SessaoAtual afr_sessaoAtual, PonteApi afr_ponteApi)
        {
            this.afr_sessao = afr_sessaoAtual;
            this.afr_ponte = afr_ponteApi;
        }

        // DS12 - verificarPermissaoAdmin(admin)
        public bool VerificarPermissaoAdmin(out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!afr_sessao.EstaAberta())
            {
                afr_mensagem = "Sessão encerrada. Entre novamente.";
                return false;
            }

            if (!afr_sessao.PodeGerenciarUsuarios())
            {
                afr_mensagem = "Apenas o administrador pode gerenciar usuários.";
                return false;
            }

            return true;
        }

        public List<Usuario> ListarUsuarios(string afr_termo)
        {
            return afr_ponte.ListarUsuarios(afr_termo);
        }

        // DS07 - a lista de alunos que o professor pode acompanhar
        public List<Usuario> ListarAlunos(string afr_termo)
        {
            return afr_ponte.ListarAlunos(afr_termo);
        }

        // DS12 - buscarDadosUsuario(usuario)
        public Usuario BuscarDadosUsuario(int afr_id)
        {
            return afr_ponte.BuscarUsuario(afr_id);
        }

        // DS11 - verificarDuplicidade(email). A API tambem recusa (409),
        // mas conferir antes da a mensagem ao lado do campo certo.
        public bool VerificarDuplicidade(string afr_email, int afr_idIgnorar)
        {
            string afr_procurado = AutenticacaoService.NormalizarEmail(afr_email);

            foreach (Usuario afr_existente in afr_ponte.ListarUsuarios(""))
            {
                if (afr_existente.afr_Id != afr_idIgnorar &&
                    AutenticacaoService.NormalizarEmail(afr_existente.afr_Email) == afr_procurado)
                {
                    return true;
                }
            }

            return false;
        }

        // DS11 - validaECria(dados). A senha vai em texto pela conexao e
        // quem gera o hash e a API.
        public bool CriarUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            if (!Validar(afr_usuario, false, out afr_mensagem))
            {
                return false;
            }

            if (VerificarDuplicidade(afr_usuario.afr_Email, 0))
            {
                afr_mensagem = "Já existe um usuário com este email.";
                return false;
            }

            afr_usuario.afr_Email = AutenticacaoService.NormalizarEmail(afr_usuario.afr_Email);

            return afr_ponte.CriarUsuario(afr_usuario, out afr_mensagem);
        }

        // DS13 - validaEAtualiza(novosDados). Senha em branco = manter a
        // atual (a API so troca quando vem preenchida).
        public bool EditarUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            afr_usuario.afr_Senha = afr_usuario.afr_Senha ?? "";

            if (!Validar(afr_usuario, true, out afr_mensagem))
            {
                return false;
            }

            if (VerificarDuplicidade(afr_usuario.afr_Email, afr_usuario.afr_Id))
            {
                afr_mensagem = "Já existe outro usuário com este email.";
                return false;
            }

            afr_usuario.afr_Email = AutenticacaoService.NormalizarEmail(afr_usuario.afr_Email);

            return afr_ponte.EditarUsuario(afr_usuario, out afr_mensagem);
        }

        // -------------------------------------------------------------
        // DS14 - verificarElegibilidadeExclusao(usuario)
        // A API recusa a propria conta e o ultimo admin; a regra se
        // repete aqui so para a tela de confirmacao ja chegar com o motivo.
        // -------------------------------------------------------------
        public ElegibilidadeExclusao VerificarElegibilidadeExclusao(int afr_id)
        {
            ElegibilidadeExclusao afr_resultado = new ElegibilidadeExclusao();
            Usuario afr_alvo = afr_ponte.BuscarUsuario(afr_id);

            if (afr_alvo == null)
            {
                afr_resultado.afr_Elegivel = false;
                afr_resultado.afr_Motivo = "Usuário não encontrado.";
                return afr_resultado;
            }

            if (afr_id == afr_sessao.ObterUsuarioId())
            {
                afr_resultado.afr_Elegivel = false;
                afr_resultado.afr_Motivo = "Você não pode excluir a sua própria conta.";
                return afr_resultado;
            }

            afr_resultado.afr_Elegivel = true;
            afr_resultado.afr_Motivo = "O usuário pode ser excluído.";
            afr_resultado.afr_DadosDetalhados.Add(
                "Usuário: " + afr_alvo.afr_Nome + " (" + afr_alvo.afr_Email + ")");
            afr_resultado.afr_DadosDetalhados.Add("Perfil: " + afr_alvo.afr_Perfil);
            afr_resultado.afr_DadosDetalhados.Add(
                "O progresso, o XP e as medalhas dele serão apagados");

            // O curso do professor NAO e apagado: fica sem professor ate
            // o administrador escolher outro na tela de cursos.
            if (afr_alvo.afr_Perfil == Usuario.PERFIL_PROFESSOR)
            {
                int afr_cursos = 0;

                foreach (Curso afr_curso in afr_ponte.ListarCursos(""))
                {
                    if (afr_curso.afr_ProfessorId == afr_id)
                    {
                        afr_cursos++;
                    }
                }

                afr_resultado.afr_DadosDetalhados.Add(
                    "Cursos dele: " + afr_cursos + ". Eles NÃO são apagados: ficam sem " +
                    "professor até o administrador escolher outro.");
            }

            return afr_resultado;
        }

        // DS14 - removerDefinitivamente(usuario)
        public bool RemoverDefinitivamente(int afr_id, out string afr_mensagem)
        {
            return afr_ponte.ExcluirUsuario(afr_id, out afr_mensagem);
        }

        // afr_senhaOpcional: na edicao, senha vazia = manter a atual
        private bool Validar(Usuario afr_usuario, bool afr_senhaOpcional, out string afr_mensagem)
        {
            afr_mensagem = "";

            if ((afr_usuario.afr_Nome ?? "").Trim() == "")
            {
                afr_mensagem = "Informe o nome do usuário.";
                return false;
            }

            string afr_email = (afr_usuario.afr_Email ?? "").Trim();
            int afr_posArroba = afr_email.IndexOf('@');

            // Operador logico OU (||)
            if (afr_posArroba < 1 || afr_email.IndexOf('.', afr_posArroba) < afr_posArroba + 2)
            {
                afr_mensagem = "Informe um email válido (exemplo: nome@escola.com).";
                return false;
            }

            string afr_senha = afr_usuario.afr_Senha ?? "";
            bool afr_mantemSenha = afr_senhaOpcional && afr_senha == "";

            // A mesma regra da API: pelo menos 8 caracteres
            if (!afr_mantemSenha && afr_senha.Length < 8)
            {
                afr_mensagem = "A senha precisa ter pelo menos 8 caracteres.";
                return false;
            }

            // Operador logico E (&&): o perfil precisa ser um dos tres
            if (afr_usuario.afr_Perfil != Usuario.PERFIL_ADMIN &&
                afr_usuario.afr_Perfil != Usuario.PERFIL_PROFESSOR &&
                afr_usuario.afr_Perfil != Usuario.PERFIL_ALUNO)
            {
                afr_mensagem = "Escolha o perfil do usuário.";
                return false;
            }

            return true;
        }
    }

    // =================================================================
    // CLASSE 4 - CursoService
    // DS03 Visualizar, DS04 Buscar, DS09 Editar, DS10 Excluir
    // =================================================================
    public class CursoService
    {
        private readonly SessaoAtual afr_sessao;
        private readonly PonteApi afr_ponte;

        public CursoService(SessaoAtual afr_sessaoAtual, PonteApi afr_ponteApi)
        {
            this.afr_sessao = afr_sessaoAtual;
            this.afr_ponte = afr_ponteApi;
        }

        // O admin escolhe o professor responsavel por cada curso
        public List<Usuario> ListarProfessores()
        {
            return afr_ponte.ListarProfessores();
        }

        // DS09 e DS10 - quem pode mexer em curso e o PROFESSOR (e o admin)
        public bool VerificarPermissaoProfessor(out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!afr_sessao.EstaAberta())
            {
                afr_mensagem = "Sessão encerrada. Entre novamente.";
                return false;
            }

            if (!afr_sessao.PodeGerenciarCursos())
            {
                afr_mensagem = "Apenas professor ou administrador pode gerenciar cursos.";
                return false;
            }

            return true;
        }

        // DS03 - exibirCursos(cursos): o catalogo
        public List<Curso> ExibirCursos()
        {
            FiltroCurso afr_filtro = new FiltroCurso();
            afr_filtro.afr_SomenteAtivos = true;

            return Filtrar(afr_filtro);
        }

        // DS04 - buscarCurso(termo, filtros): a busca do catalogo
        public List<Curso> BuscarCurso(FiltroCurso afr_filtro)
        {
            afr_filtro = afr_filtro ?? new FiltroCurso();
            afr_filtro.afr_SomenteAtivos = true;

            return Filtrar(afr_filtro);
        }

        // DS04 - localizarCurso: a busca do PROFESSOR, com os inativos
        public List<Curso> LocalizarCurso(FiltroCurso afr_filtro)
        {
            afr_filtro = afr_filtro ?? new FiltroCurso();
            afr_filtro.afr_SomenteAtivos = false;

            return Filtrar(afr_filtro);
        }

        // -------------------------------------------------------------
        // Os filtros da busca (DS04). A API filtra so por texto; carga
        // horaria, situacao e "so os que nao conclui" sao aplicados aqui,
        // sobre a lista que ela devolve.
        // -------------------------------------------------------------
        private List<Curso> Filtrar(FiltroCurso afr_filtro)
        {
            List<Curso> afr_resultado = new List<Curso>();
            List<int> afr_concluidos = new List<int>();

            if (afr_filtro.afr_SomenteNaoConcluidos)
            {
                foreach (ProgressoCurso afr_linha in afr_ponte.MeuProgresso())
                {
                    if (afr_linha.afr_Resultado.afr_Aprovado)
                    {
                        afr_concluidos.Add(afr_linha.afr_Curso.afr_Id);
                    }
                }
            }

            foreach (Curso afr_curso in afr_ponte.ListarCursos(afr_filtro.afr_Termo))
            {
                if (afr_filtro.afr_SomenteAtivos && !afr_curso.afr_Ativo) continue;
                if (afr_filtro.afr_CargaMinima > 0 && afr_curso.afr_CargaHoraria < afr_filtro.afr_CargaMinima) continue;
                if (afr_filtro.afr_CargaMaxima > 0 && afr_curso.afr_CargaHoraria > afr_filtro.afr_CargaMaxima) continue;
                if (afr_concluidos.Contains(afr_curso.afr_Id)) continue;

                afr_resultado.Add(afr_curso);
            }

            return afr_resultado;
        }

        // DS09 - getDadosCurso(curso)
        public Curso GetDadosCurso(int afr_id)
        {
            return afr_ponte.ObterCurso(afr_id);
        }

        public bool CriarCurso(Curso afr_curso, out string afr_mensagem)
        {
            if (!Validar(afr_curso, out afr_mensagem))
            {
                return false;
            }

            return afr_ponte.CriarCurso(afr_curso, out afr_mensagem);
        }

        // DS09 - validarEAtualizar(novosDados)
        public bool EditarCurso(Curso afr_curso, out string afr_mensagem)
        {
            if (!Validar(afr_curso, out afr_mensagem))
            {
                return false;
            }

            return afr_ponte.EditarCurso(afr_curso, out afr_mensagem);
        }

        // -------------------------------------------------------------
        // DS10 - verificarElegibilidadeExclusao(curso). Na API so o admin
        // tem rota de excluir curso.
        // -------------------------------------------------------------
        public ElegibilidadeExclusao VerificarElegibilidadeExclusao(int afr_cursoId)
        {
            ElegibilidadeExclusao afr_resultado = new ElegibilidadeExclusao();
            Curso afr_curso = afr_ponte.ObterCurso(afr_cursoId);

            if (afr_curso == null)
            {
                afr_resultado.afr_Elegivel = false;
                afr_resultado.afr_Motivo = "Curso não encontrado.";
                return afr_resultado;
            }

            if (!afr_sessao.EhAdministrador())
            {
                afr_resultado.afr_Elegivel = false;
                afr_resultado.afr_Motivo = "Só o administrador exclui curso. " +
                                           "Você pode desativar o curso em Editar.";
                return afr_resultado;
            }

            afr_resultado.afr_Elegivel = true;
            afr_resultado.afr_Motivo = "O curso pode ser excluído. Os alunos matriculados " +
                                       "perdem o progresso nele.";
            afr_resultado.afr_DadosDetalhados.Add(
                "Curso: " + afr_curso.afr_Titulo + " (" + afr_curso.afr_CargaHoraria + "h)");
            afr_resultado.afr_DadosDetalhados.Add(
                afr_curso.afr_TotalAulas + " aula(s) serão apagadas junto");

            return afr_resultado;
        }

        // DS10 - removerDefinitivamente(curso)
        public bool RemoverDefinitivamente(int afr_cursoId, out string afr_mensagem)
        {
            return afr_ponte.ExcluirCurso(afr_cursoId, out afr_mensagem);
        }

        private bool Validar(Curso afr_curso, out string afr_mensagem)
        {
            afr_mensagem = "";

            if ((afr_curso.afr_Titulo ?? "").Trim() == "")
            {
                afr_mensagem = "Informe o título do curso.";
                return false;
            }

            if (afr_curso.afr_CargaHoraria <= 0)
            {
                afr_mensagem = "A carga horária precisa ser maior que zero.";
                return false;
            }

            // Operador logico OU (||)
            if (afr_curso.afr_PercentualMinimo < 1 || afr_curso.afr_PercentualMinimo > 100)
            {
                afr_mensagem = "O percentual mínimo precisa ficar entre 1 e 100.";
                return false;
            }

            return true;
        }

        // -------------------------------------------------------------
        // MATRICULA EXPLICITA - so aluno se matricula, como na API.
        // Professor e administrador acompanham cursos, nao cursam.
        // -------------------------------------------------------------
        public bool Matricular(int afr_usuarioId, int afr_cursoId, out string afr_mensagem)
        {
            if (!afr_sessao.EstaAberta())
            {
                afr_mensagem = "Entre no sistema para se matricular.";
                return false;
            }

            if (afr_sessao.EhAdministrador() || afr_sessao.EhProfessor())
            {
                afr_mensagem = "Apenas aluno pode se matricular em um curso.";
                return false;
            }

            return afr_ponte.Matricular(afr_cursoId, out afr_mensagem);
        }

        // Pela API so da para saber da matricula de quem esta logado
        public bool EstaMatriculado(int afr_usuarioId, int afr_cursoId)
        {
            return afr_ponte.EstaMatriculado(afr_cursoId);
        }
    }

    // =================================================================
    // CLASSE 5 - AulaService
    // DS05 - abrirAula(aula) / exibirConteudo() / assistirAula(aula)
    // =================================================================
    public class AulaService
    {
        private readonly PonteApi afr_ponte;

        public AulaService(PonteApi afr_ponteApi)
        {
            this.afr_ponte = afr_ponteApi;
        }

        // O "concluida" de cada aula e sempre o de quem esta logado:
        // o token diz quem e
        public List<Aula> ListarAulas(int afr_cursoId, int afr_usuarioId)
        {
            return afr_ponte.ListarAulas(afr_cursoId);
        }

        // DS05 - abrirAula(aula): traz a aula com o conteudo
        public Aula AbrirAula(int afr_aulaId, int afr_usuarioId)
        {
            return afr_ponte.AbrirAula(afr_aulaId);
        }

        public bool CriarAula(Aula afr_aula, out string afr_mensagem)
        {
            if (!Validar(afr_aula, out afr_mensagem))
            {
                return false;
            }

            return afr_ponte.CriarAula(afr_aula, out afr_mensagem);
        }

        public bool EditarAula(Aula afr_aula, out string afr_mensagem)
        {
            if (!Validar(afr_aula, out afr_mensagem))
            {
                return false;
            }

            return afr_ponte.EditarAula(afr_aula, out afr_mensagem);
        }

        public bool ExcluirAula(int afr_id, out string afr_mensagem)
        {
            return afr_ponte.ExcluirAula(afr_id, out afr_mensagem);
        }

        // So para mostrar na tela: quem decide a ordem de verdade e a API
        public int ProximaOrdem(int afr_cursoId)
        {
            int afr_maior = 0;

            foreach (Aula afr_aula in ListarAulas(afr_cursoId, 0))
            {
                if (afr_aula.afr_Ordem > afr_maior)
                {
                    afr_maior = afr_aula.afr_Ordem;
                }
            }

            return afr_maior + 1;
        }

        private bool Validar(Aula afr_aula, out string afr_mensagem)
        {
            afr_mensagem = "";

            if ((afr_aula.afr_Titulo ?? "").Trim() == "")
            {
                afr_mensagem = "Informe o título da aula.";
                return false;
            }

            if (afr_aula.afr_CursoId <= 0)
            {
                afr_mensagem = "Escolha o curso ao qual a aula pertence.";
                return false;
            }

            if (afr_aula.afr_DuracaoMin <= 0)
            {
                afr_mensagem = "A duração da aula precisa ser maior que zero.";
                return false;
            }

            return true;
        }
    }

    // =================================================================
    // CLASSE 6 - ProgressoService
    // DS05 Marcar aula, DS07 Progresso do aluno, DS08 Conclusao
    // =================================================================
    public class ProgressoService
    {
        private readonly SessaoAtual afr_sessao;
        private readonly PonteApi afr_ponte;

        public ProgressoService(SessaoAtual afr_sessaoAtual, PonteApi afr_ponteApi)
        {
            this.afr_sessao = afr_sessaoAtual;
            this.afr_ponte = afr_ponteApi;
        }

        // DS05 - atualizarProgresso(aluno, aula). Versao curta, para quem
        // nao se importa com o XP.
        public bool AtualizarProgresso(int afr_usuarioId, int afr_aulaId,
                                       bool afr_concluida, out string afr_mensagem)
        {
            GanhoGamificacao afr_ignorado;

            return AtualizarProgresso(afr_usuarioId, afr_aulaId, afr_concluida,
                                      out afr_mensagem, out afr_ignorado);
        }

        // -------------------------------------------------------------
        // A mesma operacao, devolvendo TAMBEM o que o aluno ganhou, para a
        // tela avisar "+10 XP". A API confere a matricula, grava o
        // progresso e paga o XP na mesma chamada.
        // -------------------------------------------------------------
        public bool AtualizarProgresso(int afr_usuarioId, int afr_aulaId,
                                       bool afr_concluida, out string afr_mensagem,
                                       out GanhoGamificacao afr_ganho)
        {
            afr_ganho = new GanhoGamificacao();

            // So aluno faz curso e ganha XP (decisao do grupo). A tela ja
            // esconde o botao, mas a regra fica aqui - e na API.
            if (afr_sessao.EhAdministrador() || afr_sessao.EhProfessor())
            {
                afr_mensagem = "Apenas aluno registra progresso nas aulas.";
                return false;
            }

            Aula afr_aula = afr_ponte.AbrirAula(afr_aulaId);

            if (afr_aula == null)
            {
                afr_mensagem = "Aula não encontrada.";
                return false;
            }

            return afr_ponte.MarcarProgresso(afr_aula.afr_CursoId, afr_aulaId, afr_concluida,
                                             out afr_ganho, out afr_mensagem);
        }

        // Pela API so se sabe o progresso de quem esta logado
        public int ObterAulasConcluidas(int afr_usuarioId, int afr_cursoId)
        {
            return afr_usuarioId == afr_sessao.ObterUsuarioId()
                ? afr_ponte.ContarConcluidas(afr_cursoId)
                : 0;
        }

        // DS08 - obterRequisitosCurso(curso)
        public string ObterRequisitosCurso(Curso afr_curso)
        {
            if (afr_curso == null)
            {
                return "Curso não encontrado.";
            }

            return afr_curso.ObterRequisitos();
        }

        // DS08 - verificarConclusao(aluno, curso)
        public ResultadoConclusao VerificarConclusao(int afr_usuarioId, Curso afr_curso)
        {
            if (afr_curso == null)
            {
                return new ResultadoConclusao();
            }

            int afr_concluidas = ObterAulasConcluidas(afr_usuarioId, afr_curso.afr_Id);

            return PonteApi.MontarResultado(afr_concluidas, afr_curso.afr_TotalAulas);
        }

        // DS07 - buscarProgresso(aluno): o proprio pelos cursos de quem
        // esta logado, o de outro aluno pela rota de progresso da API
        public List<ProgressoCurso> BuscarProgresso(int afr_alunoId)
        {
            return afr_alunoId == afr_sessao.ObterUsuarioId()
                ? afr_ponte.MeuProgresso()
                : afr_ponte.ProgressoDoAluno(afr_alunoId);
        }

        // -------------------------------------------------------------
        // DS07 - elegivelParaVisualizacao(true)
        // So professor e admin consultam o progresso de OUTRA pessoa.
        // Qualquer um pode ver o proprio.
        // -------------------------------------------------------------
        public bool ElegivelParaVisualizacao(int afr_alunoId, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (!afr_sessao.EstaAberta())
            {
                afr_mensagem = "Sessão encerrada. Entre novamente.";
                return false;
            }

            if (afr_alunoId == afr_sessao.ObterUsuarioId())
            {
                return true;
            }

            if (!afr_sessao.PodeVerProgressoDeOutros())
            {
                afr_mensagem = "Apenas professor ou administrador pode consultar " +
                               "o progresso de outro aluno.";
                return false;
            }

            return true;
        }
    }

    // =================================================================
    // CLASSE 7 - GamificacaoService
    // XP, nivel, sequencia de dias, medalhas e a classificacao. A regra
    // de quanto vale cada coisa mora na API: aqui so se le o resultado.
    // =================================================================
    public class GamificacaoService
    {
        private readonly SessaoAtual afr_sessao;
        private readonly PonteApi afr_ponte;

        public GamificacaoService(SessaoAtual afr_sessaoAtual, PonteApi afr_ponteApi)
        {
            this.afr_sessao = afr_sessaoAtual;
            this.afr_ponte = afr_ponteApi;
        }

        // O proprio aluno pela rota /me (com as medalhas); outro aluno,
        // pela rota do professor. Professor e admin nao tem XP proprio.
        public Pontuacao ObterPontuacao(int afr_usuarioId)
        {
            if (afr_usuarioId == afr_sessao.ObterUsuarioId())
            {
                if (afr_sessao.EhAdministrador() || afr_sessao.EhProfessor())
                {
                    Pontuacao afr_vazia = new Pontuacao();
                    afr_vazia.afr_UsuarioId = afr_usuarioId;
                    return afr_vazia;
                }

                return afr_ponte.MinhaPontuacao();
            }

            return afr_ponte.PontuacaoDoAluno(afr_usuarioId);
        }

        // -------------------------------------------------------------
        // A classificacao por XP. A PERMISSAO E CONFERIDA AQUI, nao na
        // tela: aluno recebe lista vazia.
        // -------------------------------------------------------------
        public List<LinhaRanking> ListarRanking(int afr_limite)
        {
            if (!afr_sessao.PodeVerProgressoDeOutros())
            {
                return new List<LinhaRanking>();
            }

            List<LinhaRanking> afr_lista = afr_ponte.Ranking();

            if (afr_lista.Count > afr_limite)
            {
                afr_lista = afr_lista.GetRange(0, afr_limite);
            }

            return afr_lista;
        }
    }
}
