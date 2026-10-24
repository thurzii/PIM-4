/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: Modelo.cs  (classes de dominio)
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   VERSAO 2 - alinhada com a modelagem UML (Projeto_UML.asta)

   O QUE MUDOU EM RELACAO A VERSAO 1:
   1) Perfil TUTOR, que aparece nos diagramas DS07 a DS10
   2) Aula ganhou Conteudo (DS05: exibirConteudo)
   3) Curso ganhou PercentualMinimo (DS08: obterRequisitosCurso)
   4) Classes novas: FiltroCurso, CodigoRecuperacao, LogAcao,
      ResultadoConclusao e ElegibilidadeExclusao

   OS TRES PERFIS, conforme os diagramas de sequencia:
     ALUNO  DS01-DS06 ... catalogo, busca, aulas e proprio progresso
     TUTOR  DS07-DS10 ... tudo do aluno + cursos, aulas e progresso
                          dos alunos
     ADMIN  DS11-DS14 ... tudo do tutor + gerenciamento de usuarios
   ===================================================================== */

using System;
using System.Collections.Generic;

namespace PlataformaCursos
{
    // =================================================================
    // CLASSE 1 - Pessoa (CLASSE BASE)
    // =================================================================
    public class Pessoa
    {
        public int afr_Id { get; set; }
        public string afr_Nome { get; set; } = "";
        public string afr_Email { get; set; } = "";

        public Pessoa()
        {
        }

        public Pessoa(string afr_nome, string afr_email)
        {
            afr_Nome = afr_nome;
            afr_Email = afr_email;
        }

        // Metodo VIRTUAL: as classes filhas podem reescrever
        public virtual string ObterResumo()
        {
            return afr_Nome + " (" + afr_Email + ")";
        }

        public string ObterPrimeiroNome()
        {
            string[] afr_partes = afr_Nome.Trim().Split(' ');

            if (afr_partes.Length > 0 && afr_partes[0] != "")
            {
                return afr_partes[0];
            }

            return afr_Nome;
        }
    }

    // =================================================================
    // CLASSE 2 - Usuario (HERANCA: Usuario E UMA Pessoa)
    // =================================================================
    public class Usuario : Pessoa
    {
        // Os tres perfis dos diagramas de sequencia
        public const string PERFIL_ADMIN = "ADMIN";
        public const string PERFIL_TUTOR = "TUTOR";
        public const string PERFIL_ALUNO = "ALUNO";

        public string afr_Senha { get; set; } = "";
        public string afr_Perfil { get; set; } = PERFIL_ALUNO;
        public bool afr_Ativo { get; set; } = true;
        public DateTime afr_DataCadastro { get; set; } = DateTime.Now;

        public Usuario()
        {
        }

        public Usuario(string afr_nome, string afr_email, string afr_senha, string afr_perfil)
            : base(afr_nome, afr_email)
        {
            afr_Senha = afr_senha;
            afr_Perfil = afr_perfil;
        }

        // -------------------------------------------------------------
        // PERMISSOES
        // Em vez de espalhar "if perfil == ..." pelas telas, cada
        // permissao vira um metodo com nome proprio.
        // -------------------------------------------------------------
        public bool EhAdministrador()
        {
            return afr_Perfil == PERFIL_ADMIN;
        }

        public bool EhTutor()
        {
            return afr_Perfil == PERFIL_TUTOR;
        }

        public bool EhAluno()
        {
            return afr_Perfil == PERFIL_ALUNO;
        }

        // DS09 e DS10: quem edita e exclui curso e o TUTOR.
        // O admin tambem pode, porque tem acesso a tudo.
        public bool PodeGerenciarCursos()
        {
            // Operador logico OU (||)
            return EhTutor() || EhAdministrador();
        }

        // DS07: so o tutor (e o admin) consultam o progresso de OUTRO aluno
        public bool PodeVerProgressoDeOutros()
        {
            return EhTutor() || EhAdministrador();
        }

        // DS11 a DS14: gerenciar usuarios e exclusivo do administrador
        public bool PodeGerenciarUsuarios()
        {
            return EhAdministrador();
        }

        // POLIMORFISMO - reescreve o metodo da classe Pessoa
        public override string ObterResumo()
        {
            string afr_situacao;

            if (afr_Ativo)
            {
                afr_situacao = "ativo";
            }
            else
            {
                afr_situacao = "inativo";
            }

            return base.ObterResumo() + " - " + afr_Perfil + " - " + afr_situacao;
        }

        // Lista usada pelas caixas de selecao de perfil
        public static string[] ObterPerfis()
        {
            return new string[] { PERFIL_ALUNO, PERFIL_TUTOR, PERFIL_ADMIN };
        }
    }

    // =================================================================
    // CLASSE 3 - Curso
    // =================================================================
    public class Curso
    {
        public int afr_Id { get; set; }
        public string afr_Titulo { get; set; } = "";
        public string afr_Descricao { get; set; } = "";
        public int afr_CargaHoraria { get; set; }
        public bool afr_Ativo { get; set; } = true;

        // DS08 - obterRequisitosCurso: percentual minimo de aulas
        // concluidas para o curso contar como concluido.
        public int afr_PercentualMinimo { get; set; } = 100;

        public int afr_TotalAulas { get; set; }

        public Curso()
        {
        }

        public Curso(string afr_titulo, string afr_descricao, int afr_cargaHoraria)
        {
            afr_Titulo = afr_titulo;
            afr_Descricao = afr_descricao;
            afr_CargaHoraria = afr_cargaHoraria;
        }

        public string ObterResumo()
        {
            return afr_Titulo + " - " + afr_CargaHoraria + "h - " +
                   afr_TotalAulas + " aula(s)";
        }

        // DS08 - requisitosCurso(dados): o texto dos requisitos
        public string ObterRequisitos()
        {
            return "Concluir pelo menos " + afr_PercentualMinimo + "% das " +
                   afr_TotalAulas + " aula(s) do curso.";
        }
    }

    // =================================================================
    // CLASSE 4 - Aula
    // =================================================================
    public class Aula
    {
        public int afr_Id { get; set; }
        public int afr_CursoId { get; set; }
        public string afr_Titulo { get; set; } = "";
        public int afr_Ordem { get; set; }
        public int afr_DuracaoMin { get; set; }

        // DS05 - exibirConteudo(): o que o aluno le ao abrir a aula
        public string afr_Conteudo { get; set; } = "";

        // Preenchido pela consulta do aluno
        public bool afr_Concluida { get; set; }

        public Aula()
        {
        }

        public Aula(int afr_cursoId, string afr_titulo, int afr_ordem, int afr_duracaoMin)
        {
            afr_CursoId = afr_cursoId;
            afr_Titulo = afr_titulo;
            afr_Ordem = afr_ordem;
            afr_DuracaoMin = afr_duracaoMin;
        }

        public string ObterResumo()
        {
            return afr_Ordem + ". " + afr_Titulo + " (" + afr_DuracaoMin + " min)";
        }
    }

    // =================================================================
    // CLASSE 5 - Progresso
    // =================================================================
    public class Progresso
    {
        public int afr_Id { get; set; }
        public int afr_UsuarioId { get; set; }
        public int afr_AulaId { get; set; }
        public DateTime afr_DataConclusao { get; set; }

        public Progresso()
        {
        }

        public Progresso(int afr_usuarioId, int afr_aulaId)
        {
            afr_UsuarioId = afr_usuarioId;
            afr_AulaId = afr_aulaId;
            afr_DataConclusao = DateTime.Now;
        }
    }

    // =================================================================
    // CLASSE 6 - FiltroCurso
    // DS04 - buscarCurso(termo, FILTROS)
    // O modelo passa "filtros" como um parametro so. Esta classe e
    // esse parametro: junta tudo o que restringe a busca.
    // =================================================================
    public class FiltroCurso
    {
        public string afr_Termo { get; set; } = "";
        public int afr_CargaMinima { get; set; } = 0;
        public int afr_CargaMaxima { get; set; } = 0;     // 0 = sem limite
        public bool afr_SomenteAtivos { get; set; } = true;
        public bool afr_SomenteNaoConcluidos { get; set; } = false;

        // Id do aluno, usado quando o filtro de nao concluidos esta ligado
        public int afr_UsuarioId { get; set; }

        public bool TemFiltroAlemDoTermo()
        {
            // Operador logico OU (||)
            return afr_CargaMinima > 0 ||
                   afr_CargaMaxima > 0 ||
                   afr_SomenteNaoConcluidos;
        }

        public string ObterResumo()
        {
            List<string> afr_partes = new List<string>();

            if (afr_Termo.Trim() != "")
            {
                afr_partes.Add("termo \"" + afr_Termo.Trim() + "\"");
            }

            if (afr_CargaMinima > 0)
            {
                afr_partes.Add("carga a partir de " + afr_CargaMinima + "h");
            }

            if (afr_CargaMaxima > 0)
            {
                afr_partes.Add("carga ate " + afr_CargaMaxima + "h");
            }

            if (afr_SomenteNaoConcluidos)
            {
                afr_partes.Add("apenas nao concluidos");
            }

            if (afr_partes.Count == 0)
            {
                return "sem filtro";
            }

            return string.Join(", ", afr_partes);
        }
    }

    // =================================================================
    // CLASSE 7 - ResultadoConclusao
    // DS08 - resultadoConclusao(status, percentual)
    // =================================================================
    public class ResultadoConclusao
    {
        public int afr_Concluidas { get; set; }
        public int afr_Total { get; set; }
        public int afr_Percentual { get; set; }
        public int afr_PercentualMinimo { get; set; }
        public bool afr_Aprovado { get; set; }
        public string afr_Requisitos { get; set; } = "";

        // DS08 - status: o texto que vai para a tela
        public string ObterStatus()
        {
            if (afr_Total == 0)
            {
                return "SEM AULAS";
            }

            if (afr_Aprovado)
            {
                return "CONCLUIDO";
            }

            if (afr_Concluidas > 0)
            {
                return "EM ANDAMENTO";
            }

            return "NAO INICIADO";
        }

        public string ObterResumo()
        {
            return ObterStatus() + " - " + afr_Percentual + "% (" +
                   afr_Concluidas + " de " + afr_Total + " aulas)";
        }
    }

    // =================================================================
    // CLASSE 7b - ProgressoCurso
    // DS07 - progresso(dados) / exibirProgresso(dados)
    // Junta o curso com o quanto um aluno ja andou nele. E o "dados"
    // que o Sistema devolve para a TelaProgresso no diagrama.
    // =================================================================
    public class ProgressoCurso
    {
        public Curso afr_Curso { get; set; } = new Curso();
        public ResultadoConclusao afr_Resultado { get; set; } = new ResultadoConclusao();

        public string ObterResumo()
        {
            return afr_Curso.afr_Titulo + ": " + afr_Resultado.ObterResumo();
        }
    }

    // =================================================================
    // CLASSE 8 - CodigoRecuperacao
    // DS02 - enviarCodigo(email) / codigoEnviado()
    // =================================================================
    public class CodigoRecuperacao
    {
        public int afr_Id { get; set; }
        public int afr_UsuarioId { get; set; }
        public string afr_Codigo { get; set; } = "";
        public DateTime afr_Expiracao { get; set; }
        public bool afr_Usado { get; set; }

        public bool EstaValido()
        {
            // Operador logico E (&&)
            return !afr_Usado && DateTime.Now <= afr_Expiracao;
        }

        public int MinutosRestantes()
        {
            double afr_minutos = (afr_Expiracao - DateTime.Now).TotalMinutes;

            if (afr_minutos < 0)
            {
                return 0;
            }

            return (int)Math.Ceiling(afr_minutos);
        }
    }

    // =================================================================
    // CLASSE 9 - LogAcao
    // DS12 - historicoAcoes(loginLog, acoesCrud, cursosRelacionados)
    // =================================================================
    public class LogAcao
    {
        public const string TIPO_LOGIN = "LOGIN";
        public const string TIPO_CRUD = "CRUD";
        public const string TIPO_CURSO = "CURSO";

        public int afr_Id { get; set; }
        public int afr_UsuarioId { get; set; }
        public string afr_Tipo { get; set; } = TIPO_CRUD;
        public string afr_Descricao { get; set; } = "";
        public DateTime afr_DataHora { get; set; } = DateTime.Now;

        public LogAcao()
        {
        }

        public LogAcao(int afr_usuarioId, string afr_tipo, string afr_descricao)
        {
            afr_UsuarioId = afr_usuarioId;
            afr_Tipo = afr_tipo;
            afr_Descricao = afr_descricao;
            afr_DataHora = DateTime.Now;
        }

        public string ObterResumo()
        {
            return afr_DataHora.ToString("dd/MM/yyyy HH:mm") + "  [" +
                   afr_Tipo + "]  " + afr_Descricao;
        }
    }

    // =================================================================
    // CLASSE 10 - ElegibilidadeExclusao
    // DS10 e DS14 - verificarElegibilidadeExclusao /
    //               elegivelParaExclusao(true, dadosDetalhados)
    // Antes de apagar qualquer coisa, o sistema explica O QUE sera
    // apagado junto. E o "dadosDetalhados" do modelo.
    // =================================================================
    public class ElegibilidadeExclusao
    {
        public bool afr_Elegivel { get; set; }
        public string afr_Motivo { get; set; } = "";
        public List<string> afr_DadosDetalhados { get; set; } = new List<string>();

        public string ObterDetalhesTexto()
        {
            if (afr_DadosDetalhados.Count == 0)
            {
                return "Nenhum dado relacionado sera afetado.";
            }

            return "- " + string.Join(Environment.NewLine + "- ", afr_DadosDetalhados);
        }
    }

    // =================================================================
    // CLASSE 11 - Sessao
    // DS06 - encerrarSessao / finalizarSessao / sessaoEncerrada
    // =================================================================
    public static class Sessao
    {
        private static Usuario afr_usuarioLogado = null;
        private static DateTime afr_inicio;

        public static void Abrir(Usuario afr_usuario)
        {
            afr_usuarioLogado = afr_usuario;
            afr_inicio = DateTime.Now;
        }

        // DS06 - finalizarSessao()
        public static void Encerrar()
        {
            afr_usuarioLogado = null;
        }

        public static bool EstaAberta()
        {
            return afr_usuarioLogado != null;
        }

        public static Usuario ObterUsuario()
        {
            return afr_usuarioLogado;
        }

        public static int ObterUsuarioId()
        {
            if (afr_usuarioLogado == null)
            {
                return 0;
            }

            return afr_usuarioLogado.afr_Id;
        }

        public static bool EhAdministrador()
        {
            if (afr_usuarioLogado == null)
            {
                return false;
            }

            return afr_usuarioLogado.EhAdministrador();
        }

        public static bool EhTutor()
        {
            if (afr_usuarioLogado == null)
            {
                return false;
            }

            return afr_usuarioLogado.EhTutor();
        }

        public static bool PodeGerenciarCursos()
        {
            if (afr_usuarioLogado == null)
            {
                return false;
            }

            return afr_usuarioLogado.PodeGerenciarCursos();
        }

        public static bool PodeGerenciarUsuarios()
        {
            if (afr_usuarioLogado == null)
            {
                return false;
            }

            return afr_usuarioLogado.PodeGerenciarUsuarios();
        }

        public static bool PodeVerProgressoDeOutros()
        {
            if (afr_usuarioLogado == null)
            {
                return false;
            }

            return afr_usuarioLogado.PodeVerProgressoDeOutros();
        }

        public static string ObterTempoSessao()
        {
            if (afr_usuarioLogado == null)
            {
                return "";
            }

            TimeSpan afr_tempo = DateTime.Now - afr_inicio;

            return (int)afr_tempo.TotalMinutes + " min";
        }
    }
}
