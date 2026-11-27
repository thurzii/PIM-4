/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Models/ViewModels.cs
   ---------------------------------------------------------------------
   VERSAO 2 - alinhada com a modelagem UML (Projeto_UML.asta)

   O QUE E UM ViewModel

   As classes de Modelo.cs representam o DOMINIO: o que o sistema e.
   As classes deste arquivo representam a TELA: o que a pagina mostra
   e o que o formulario devolve.

   No desktop isso nao era necessario: o formulario lia direto dos
   controles (afr_txtTitulo.Text). Na web o navegador manda um monte
   de texto solto, e precisa existir uma classe para receber esse
   texto, validar e so depois virar um objeto de dominio. Os atributos
   [Required], [EmailAddress] e [Range] fazem essa validacao sozinhos:
   e o equivalente aos ifs de validacao que ficavam no Click do botao.

   NOVOS NESTA VERSAO, por causa dos diagramas de sequencia:
     RecuperarSenhaViewModel .. recuperar senha (DS02)
     AulaViewModel ............ conteudo da aula (DS05)
     ProgressoAlunosViewModel . progresso dos alunos (DS07)
     PerfilUsuarioViewModel ... perfil com historico (DS12)
     ConfirmarExclusaoViewModel exclusao em 2 etapas (DS10 e DS14)
   ===================================================================== */

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlataformaCursosWeb.Models
{
    // =================================================================
    // LOGIN (DS01)
    // =================================================================
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o email.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [Display(Name = "Email")]
        public string afr_Email { get; set; } = "";

        [Required(ErrorMessage = "Informe a senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string afr_Senha { get; set; } = "";
    }

    // =================================================================
    // RECUPERAR SENHA (DS02)
    // Email, codigo pessoal de recuperacao (recebido no cadastro) e a
    // nova senha, num formulario so. Quem confere o codigo e a API.
    // =================================================================
    public class RecuperarSenhaViewModel
    {
        [Display(Name = "Email cadastrado")]
        public string afr_Email { get; set; } = "";

        [Display(Name = "Código pessoal")]
        public string afr_Codigo { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string afr_NovaSenha { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar a nova senha")]
        public string afr_Confirmacao { get; set; } = "";
    }

    // =================================================================
    // USUARIO (DS11 e DS13)
    // =================================================================
    // IValidatableObject permite uma regra que olha o objeto INTEIRO, e nao
    // um campo isolado. E o que a senha precisa: se e obrigatoria ou nao
    // depende de ser cadastro ou edicao, e nenhum atributo sozinho sabe isso.
    public class UsuarioViewModel : IValidatableObject
    {
        public int afr_Id { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "O nome precisa ter entre 3 e 100 caracteres.")]
        [Display(Name = "Nome completo")]
        public string afr_Nome { get; set; } = "";

        [Required(ErrorMessage = "Informe o email.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [Display(Name = "Email")]
        public string afr_Email { get; set; } = "";

        // VERSAO 3 - a senha deixou de ser obrigatoria POR ATRIBUTO.
        // Na edicao o campo chega vazio quando o administrador nao quis
        // troca-la (a tela nao mostra mais a senha gravada, que agora e
        // hash). A regra passou para Validate(), mais abaixo, que sabe
        // distinguir cadastro de edicao.
        [StringLength(100, ErrorMessage = "A senha pode ter no máximo 100 caracteres.")]
        [Display(Name = "Senha")]
        public string afr_Senha { get; set; } = "";

        [Required(ErrorMessage = "Escolha o perfil.")]
        [Display(Name = "Perfil")]
        public string afr_Perfil { get; set; } = Usuario.PERFIL_ALUNO;

        [Display(Name = "Usuário ativo")]
        public bool afr_Ativo { get; set; } = true;

        // Mostrado na edicao; nao e editavel
        public DateTime afr_DataCadastro { get; set; }

        public bool EhEdicao()
        {
            return afr_Id > 0;
        }

        // -------------------------------------------------------------
        // VERSAO 3 - a regra da senha, que depende do objeto inteiro
        //
        // CADASTRO: a senha e obrigatoria e precisa de 8 caracteres.
        // EDICAO..: campo vazio quer dizer "manter a senha atual", entao
        //           so vale a regra dos 8 caracteres se algo foi digitado.
        // -------------------------------------------------------------
        public IEnumerable<ValidationResult> Validate(ValidationContext afr_contexto)
        {
            bool afr_informouSenha = !string.IsNullOrWhiteSpace(afr_Senha);

            if (!EhEdicao() && !afr_informouSenha)
            {
                yield return new ValidationResult(
                    "Informe a senha.",
                    new string[] { nameof(afr_Senha) });
            }
            else if (afr_informouSenha && afr_Senha.Trim().Length < 8)
            {
                yield return new ValidationResult(
                    "A senha precisa ter pelo menos 8 caracteres.",
                    new string[] { nameof(afr_Senha) });
            }
        }

        // Os tres perfis do modelo, para montar o <select> da pagina
        public string[] ObterPerfis()
        {
            return Usuario.ObterPerfis();
        }

        // Explica na tela o que cada perfil alcanca
        public static string DescreverPerfil(string afr_perfil)
        {
            if (afr_perfil == Usuario.PERFIL_ADMIN)
            {
                return "Acessa tudo, inclusive o cadastro de usuários.";
            }

            if (afr_perfil == Usuario.PERFIL_PROFESSOR)
            {
                return "Cadastra cursos e aulas e acompanha o progresso dos alunos.";
            }

            return "Vê o catálogo e marca as próprias aulas como concluídas.";
        }
    }

    // =================================================================
    // CURSO (DS09)
    // Ganhou o percentual minimo de conclusao (DS08).
    // =================================================================
    public class CursoViewModel
    {
        public int afr_Id { get; set; }

        [Required(ErrorMessage = "Informe o título do curso.")]
        [StringLength(120, MinimumLength = 3,
            ErrorMessage = "O título precisa ter entre 3 e 120 caracteres.")]
        [Display(Name = "Título")]
        public string afr_Titulo { get; set; } = "";

        [StringLength(500, ErrorMessage = "A descrição cabe em 500 caracteres.")]
        [Display(Name = "Descrição")]
        public string afr_Descricao { get; set; } = "";

        [Range(1, 1000, ErrorMessage = "A carga horária vai de 1 a 1000 horas.")]
        [Display(Name = "Carga horária (horas)")]
        public int afr_CargaHoraria { get; set; } = 20;

        // A API considera o curso concluido com todas as aulas
        public int afr_PercentualMinimo { get; set; } = 100;

        // Professor responsavel. O admin escolhe; vazio = sem professor.
        [Display(Name = "Professor")]
        public int? afr_ProfessorId { get; set; }

        [Display(Name = "Curso ativo (visível para o aluno)")]
        public bool afr_Ativo { get; set; } = true;

        public bool EhEdicao()
        {
            return afr_Id > 0;
        }
    }

    // =================================================================
    // AULAS DO CURSO - visao do professor (DS05)
    // O conteudo da aula entra aqui.
    // =================================================================
    public class AulasAdminViewModel
    {
        public Curso afr_Curso { get; set; } = new Curso();
        public List<Aula> afr_Aulas { get; set; } = new List<Aula>();

        // Formulario de nova aula
        [Display(Name = "Título da aula")]
        public string afr_NovaAulaTitulo { get; set; } = "";

        [Range(1, 600, ErrorMessage = "A duração vai de 1 a 600 minutos.")]
        [Display(Name = "Duração (minutos)")]
        public int afr_NovaAulaDuracao { get; set; } = 45;

        [Display(Name = "Conteúdo da aula")]
        public string afr_NovaAulaConteudo { get; set; } = "";

        // Quantas aulas ainda estao sem conteudo escrito
        public int ContarSemConteudo()
        {
            int afr_total = 0;

            foreach (Aula afr_aula in afr_Aulas)
            {
                if (afr_aula.afr_Conteudo.Trim() == "")
                {
                    afr_total = afr_total + 1;
                }
            }

            return afr_total;
        }
    }

    // =================================================================
    // EDITAR UMA AULA - visao do professor
    // =================================================================
    public class AulaFormViewModel
    {
        public int afr_Id { get; set; }
        public int afr_CursoId { get; set; }
        public string afr_CursoTitulo { get; set; } = "";
        public int afr_Ordem { get; set; }

        [Required(ErrorMessage = "Informe o título da aula.")]
        [StringLength(120, MinimumLength = 3,
            ErrorMessage = "O título precisa ter entre 3 e 120 caracteres.")]
        [Display(Name = "Título da aula")]
        public string afr_Titulo { get; set; } = "";

        [Range(1, 600, ErrorMessage = "A duração vai de 1 a 600 minutos.")]
        [Display(Name = "Duração (minutos)")]
        public int afr_DuracaoMin { get; set; } = 45;

        [Display(Name = "Conteúdo da aula")]
        public string afr_Conteudo { get; set; } = "";
    }

    // =================================================================
    // CATALOGO (DS03 e DS04)
    //
    // O diagrama DS04 manda buscarCurso(termo, FILTROS). Os campos de
    // filtro da pagina ficam aqui e viram um objeto FiltroCurso no
    // controller.
    // =================================================================
    public class CatalogoViewModel
    {
        public List<Curso> afr_Cursos { get; set; } = new List<Curso>();

        [Display(Name = "Buscar por título ou descrição")]
        public string afr_Termo { get; set; } = "";

        [Range(0, 1000, ErrorMessage = "A carga mínima vai de 0 a 1000 horas.")]
        [Display(Name = "Carga horária de")]
        public int afr_CargaMinima { get; set; } = 0;

        [Range(0, 1000, ErrorMessage = "A carga máxima vai de 0 a 1000 horas.")]
        [Display(Name = "ate")]
        public int afr_CargaMaxima { get; set; } = 0;

        [Display(Name = "Somente os cursos que eu ainda não conclui")]
        public bool afr_SomenteNaoConcluidos { get; set; } = false;

        // Preenchido pelo controller: o progresso do aluno em cada curso
        public Dictionary<int, ResultadoConclusao> afr_Progressos { get; set; }
            = new Dictionary<int, ResultadoConclusao>();

        public bool TemFiltro()
        {
            return afr_Termo.Trim() != ""
                || afr_CargaMinima > 0
                || afr_CargaMaxima > 0
                || afr_SomenteNaoConcluidos;
        }

        // Devolve o resultado de um curso, ou um vazio se nao tiver
        public ResultadoConclusao ObterProgresso(int afr_cursoId)
        {
            if (afr_Progressos.ContainsKey(afr_cursoId))
            {
                return afr_Progressos[afr_cursoId];
            }

            return new ResultadoConclusao();
        }

        // Monta o objeto de filtro que o CursoService espera
        public FiltroCurso MontarFiltro(int afr_usuarioId)
        {
            FiltroCurso afr_filtro = new FiltroCurso();

            afr_filtro.afr_Termo = (afr_Termo ?? "").Trim();
            afr_filtro.afr_CargaMinima = afr_CargaMinima;
            afr_filtro.afr_CargaMaxima = afr_CargaMaxima;
            afr_filtro.afr_SomenteAtivos = true;
            afr_filtro.afr_SomenteNaoConcluidos = afr_SomenteNaoConcluidos;
            afr_filtro.afr_UsuarioId = afr_usuarioId;

            return afr_filtro;
        }
    }

    // =================================================================
    // DETALHES DO CURSO - visao do aluno (DS05 e DS08)
    //
    // O status vem do objeto ResultadoConclusao, nao de um true/false:
    // o DS08 fala em "resultadoConclusao(status, percentual)".
    // =================================================================
    public class CursoAlunoViewModel
    {
        public Curso afr_Curso { get; set; } = new Curso();
        public List<Aula> afr_Aulas { get; set; } = new List<Aula>();

        public ResultadoConclusao afr_Resultado { get; set; } = new ResultadoConclusao();

        // DS08 - requisitosCurso(dados)
        public string afr_Requisitos { get; set; } = "";

        // VERSAO 3.3 - a matricula virou explicita. A pagina precisa
        // saber de duas coisas diferentes: se ESTE usuario ja esta
        // matriculado, e se ele e alguem que PODE se matricular -
        // professor e administrador abrem o curso para conferir o
        // conteudo, e para eles o botao nem aparece.
        public bool afr_Matriculado { get; set; }
        public bool afr_PodeMatricular { get; set; }

        // Mensagem da faixa de progresso
        public string ObterMensagemProgresso()
        {
            if (afr_Resultado.afr_Total == 0)
            {
                return "Este curso ainda não tem aulas cadastradas.";
            }

            if (afr_Resultado.afr_Aprovado)
            {
                return "CONCLUÍDO: você bateu o mínimo de "
                     + afr_Resultado.afr_PercentualMinimo + "% deste curso.";
            }

            if (afr_Resultado.afr_Concluidas > 0)
            {
                return "EM ANDAMENTO: faltam "
                     + (afr_Resultado.afr_Total - afr_Resultado.afr_Concluidas)
                     + " aula(s) para o mínimo de "
                     + afr_Resultado.afr_PercentualMinimo + "%.";
            }

            return "NÃO INICIADO: você ainda não concluiu nenhuma aula.";
        }

        public string ObterClasseProgresso()
        {
            if (afr_Resultado.afr_Aprovado)
            {
                return "situacao-aprovado";
            }

            if (afr_Resultado.afr_Concluidas > 0)
            {
                return "situacao-andamento";
            }

            return "situacao-neutro";
        }
    }

    // =================================================================
    // UMA AULA - visao do aluno (DS05)
    //
    // E a tela FrmAula do desktop. O diagrama pede abrirAula(aula) e
    // exibirConteudo(), entao a aula tem pagina propria.
    // =================================================================
    public class AulaViewModel
    {
        public Aula afr_Aula { get; set; } = new Aula();
        public Curso afr_Curso { get; set; } = new Curso();

        // DS05 - exibirConclusao(): como esta o curso inteiro
        public ResultadoConclusao afr_Resultado { get; set; } = new ResultadoConclusao();

        // Para os links de navegacao entre aulas
        public int afr_AulaAnterior { get; set; }
        public int afr_AulaSeguinte { get; set; }

        public string ObterStatusAula()
        {
            if (afr_Aula.afr_Concluida)
            {
                return "Aula concluída";
            }

            return "Aula pendente";
        }

        public string ObterClasseStatus()
        {
            if (afr_Aula.afr_Concluida)
            {
                return "situacao-aprovado";
            }

            return "situacao-andamento";
        }

        public bool TemConteudo()
        {
            return afr_Aula.afr_Conteudo.Trim() != "";
        }
    }

    // =================================================================
    // PROGRESSO DOS ALUNOS - visao do professor (DS07)
    // =================================================================
    public class ProgressoAlunosViewModel
    {
        public List<Usuario> afr_Alunos { get; set; } = new List<Usuario>();

        [Display(Name = "Buscar aluno")]
        public string afr_Termo { get; set; } = "";

        // Qual aluno esta sendo consultado (0 = nenhum ainda)
        public int afr_AlunoId { get; set; }

        public Usuario afr_Aluno { get; set; } = null;

        // DS07 - progresso(dados)
        public List<ProgressoCurso> afr_Progresso { get; set; } = new List<ProgressoCurso>();

        // VERSAO 3.2 - XP, nivel e conquistas do aluno consultado.
        // Comeca com objeto vazio para a pagina nunca quebrar com null.
        public Pontuacao afr_Pontuacao { get; set; } = new Pontuacao();

        // VERSAO 3.2 - a classificacao por XP, ja filtrada pelo
        // servico: quem nao pode ver recebe lista vazia.
        public List<LinhaRanking> afr_Ranking { get; set; } = new List<LinhaRanking>();

        public bool TemResultado()
        {
            return afr_Aluno != null;
        }

        public int ContarConcluidos()
        {
            int afr_total = 0;

            foreach (ProgressoCurso afr_linha in afr_Progresso)
            {
                if (afr_linha.afr_Resultado.afr_Aprovado)
                {
                    afr_total = afr_total + 1;
                }
            }

            return afr_total;
        }

        public int CalcularMedia()
        {
            if (afr_Progresso.Count == 0)
            {
                return 0;
            }

            int afr_soma = 0;

            foreach (ProgressoCurso afr_linha in afr_Progresso)
            {
                afr_soma = afr_soma + afr_linha.afr_Resultado.afr_Percentual;
            }

            return afr_soma / afr_Progresso.Count;
        }
    }

    // =================================================================
    // PERFIL DO USUARIO - visao do admin (DS12)
    //
    // O diagrama pede historicoAcoes(loginLog, acoesCrud,
    // cursosRelacionados): TRES listas separadas, nao uma so.
    // =================================================================
    public class PerfilUsuarioViewModel
    {
        public Usuario afr_Usuario { get; set; } = new Usuario();

        public List<LogAcao> afr_LoginLog { get; set; } = new List<LogAcao>();
        public List<LogAcao> afr_AcoesCrud { get; set; } = new List<LogAcao>();
        public List<LogAcao> afr_CursosRelacionados { get; set; } = new List<LogAcao>();

        public int ContarTudo()
        {
            return afr_LoginLog.Count + afr_AcoesCrud.Count + afr_CursosRelacionados.Count;
        }
    }

    // =================================================================
    // CONFIRMAR EXCLUSAO (DS10 e DS14)
    //
    // Serve para curso e para usuario: afr_Tipo diz qual dos dois.
    // A pagina mostra os dadosDetalhados e pede a senha de quem esta
    // logado - o confirmarIdentidadeEAcao do modelo.
    // =================================================================
    public class ConfirmarExclusaoViewModel
    {
        public const string TIPO_CURSO = "CURSO";
        public const string TIPO_USUARIO = "USUARIO";

        public string afr_Tipo { get; set; } = TIPO_CURSO;
        public int afr_Id { get; set; }
        public string afr_Titulo { get; set; } = "";

        // DS10 e DS14 - elegivelParaExclusao(true, dadosDetalhados)
        public ElegibilidadeExclusao afr_Elegibilidade { get; set; }
            = new ElegibilidadeExclusao();

        [DataType(DataType.Password)]
        [Display(Name = "Sua senha")]
        public string afr_Senha { get; set; } = "";

        public bool EhCurso()
        {
            return afr_Tipo == TIPO_CURSO;
        }

        public string ObterTitulo()
        {
            if (EhCurso())
            {
                return "Excluir o curso \"" + afr_Titulo + "\"";
            }

            return "Excluir o usuário " + afr_Titulo;
        }

        // Para onde voltar se a pessoa cancelar
        public string ObterControllerDeVolta()
        {
            if (EhCurso())
            {
                return "Cursos";
            }

            return "Usuários";
        }
    }

    // =================================================================
    // PAINEL INICIAL
    // E o FrmPrincipal do desktop: mostra as areas conforme o perfil.
    // =================================================================
    public class HomeViewModel
    {
        public string afr_Nome { get; set; } = "";
        public string afr_Email { get; set; } = "";
        public string afr_Perfil { get; set; } = "";
        public DateTime afr_DataCadastro { get; set; }

        public bool afr_EhAdmin { get; set; }
        public bool afr_EhProfessor { get; set; }
        public bool afr_PodeGerenciarCursos { get; set; }
        public bool afr_PodeGerenciarUsuarios { get; set; }
        public bool afr_PodeVerProgresso { get; set; }

        public string afr_StatusBanco { get; set; } = "";

        public int afr_TotalCursos { get; set; }
        public int afr_TotalUsuarios { get; set; }

        // Para o aluno: quantos cursos ele ja concluiu
        public int afr_MeusCursos { get; set; }
        public int afr_MeusConcluidos { get; set; }

        // VERSAO 3 - XP, nivel e conquistas de quem esta logado.
        // Comeca com um objeto vazio (nivel 1, zero XP) para a pagina
        // nunca quebrar com null, mesmo se o painel for aberto antes de
        // o controller preencher.
        public Pontuacao afr_Pontuacao { get; set; } = new Pontuacao();

        public string DescreverPerfil()
        {
            if (afr_EhAdmin)
            {
                return "Perfil ADMIN: catálogo, cursos, progresso dos alunos e usuários.";
            }

            if (afr_EhProfessor)
            {
                return "Perfil PROFESSOR: catálogo, cursos e aulas, e progresso dos alunos.";
            }

            return "Perfil ALUNO: catálogo de cursos e as suas aulas.";
        }
    }
}
