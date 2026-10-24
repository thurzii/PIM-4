/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - VERSAO WEB
   Arquivo...: Models/ViewModels.cs
   Professor.: Antonio Graça
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
     RecuperarSenhaViewModel .. tres etapas (DS02)
     AulaViewModel ............ conteudo da aula (DS05)
     ProgressoAlunosViewModel . progresso dos alunos (DS07)
     PerfilUsuarioViewModel ... perfil com historico (DS12)
     ConfirmarExclusaoViewModel exclusao em 2 etapas (DS10 e DS14)
   ===================================================================== */

using System.ComponentModel.DataAnnotations;

namespace PlataformaCursosWeb.Models
{
    // =================================================================
    // LOGIN (DS01)
    // =================================================================
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Informe o email.")]
        [EmailAddress(ErrorMessage = "Email invalido.")]
        [Display(Name = "Email")]
        public string afr_Email { get; set; } = "";

        [Required(ErrorMessage = "Informe a senha.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string afr_Senha { get; set; } = "";
    }

    // =================================================================
    // RECUPERAR SENHA (DS02) - TRES ETAPAS
    //
    // O diagrama tem a linha de vida "Email" e a mensagem
    // enviarCodigo(email). Por isso a recuperacao nao e mais um unico
    // formulario: sao tres etapas, e a propriedade afr_Etapa diz em
    // qual delas a pagina esta.
    //
    //   Etapa 1 - informar o email        -> solicitarRecuperacao
    //   Etapa 2 - conferir o codigo       -> conferirCodigo
    //   Etapa 3 - escolher a nova senha   -> recuperacaoConcluida
    //
    // As validacoes sao condicionais, entao elas ficam no controller,
    // nao em atributos: na etapa 1 ainda nao existe codigo nem senha.
    // =================================================================
    public class RecuperarSenhaViewModel
    {
        public int afr_Etapa { get; set; } = 1;

        [Display(Name = "Email cadastrado")]
        public string afr_Email { get; set; } = "";

        [Display(Name = "Codigo de 6 digitos")]
        public string afr_Codigo { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        public string afr_NovaSenha { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar a nova senha")]
        public string afr_Confirmacao { get; set; } = "";

        // Quantos minutos o codigo vale, para a pagina avisar
        public int afr_MinutosValidade { get; set; }

        public string ObterTituloEtapa()
        {
            if (afr_Etapa == 1)
            {
                return "Etapa 1 de 3 - informar o email";
            }

            if (afr_Etapa == 2)
            {
                return "Etapa 2 de 3 - conferir o codigo";
            }

            return "Etapa 3 de 3 - escolher a nova senha";
        }

        public string ObterExplicacaoEtapa()
        {
            if (afr_Etapa == 1)
            {
                return "Informe o email cadastrado. O sistema envia um codigo de 6 digitos.";
            }

            if (afr_Etapa == 2)
            {
                return "Digite o codigo que foi enviado. Ele vale por "
                     + afr_MinutosValidade + " minutos e serve uma vez so.";
            }

            return "Codigo confirmado. Escolha uma nova senha de pelo menos 6 caracteres.";
        }
    }

    // =================================================================
    // USUARIO (DS11 e DS13)
    // =================================================================
    public class UsuarioViewModel
    {
        public int afr_Id { get; set; }

        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "O nome precisa ter entre 3 e 100 caracteres.")]
        [Display(Name = "Nome completo")]
        public string afr_Nome { get; set; } = "";

        [Required(ErrorMessage = "Informe o email.")]
        [EmailAddress(ErrorMessage = "Email invalido.")]
        [Display(Name = "Email")]
        public string afr_Email { get; set; } = "";

        [Required(ErrorMessage = "Informe a senha.")]
        [StringLength(100, MinimumLength = 6,
            ErrorMessage = "A senha precisa ter pelo menos 6 caracteres.")]
        [Display(Name = "Senha")]
        public string afr_Senha { get; set; } = "";

        [Required(ErrorMessage = "Escolha o perfil.")]
        [Display(Name = "Perfil")]
        public string afr_Perfil { get; set; } = Usuario.PERFIL_ALUNO;

        [Display(Name = "Usuario ativo")]
        public bool afr_Ativo { get; set; } = true;

        // Mostrado na edicao; nao e editavel
        public DateTime afr_DataCadastro { get; set; }

        public bool EhEdicao()
        {
            return afr_Id > 0;
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
                return "Acessa tudo, inclusive o cadastro de usuarios.";
            }

            if (afr_perfil == Usuario.PERFIL_TUTOR)
            {
                return "Cadastra cursos e aulas e acompanha o progresso dos alunos.";
            }

            return "Ve o catalogo e marca as proprias aulas como concluidas.";
        }
    }

    // =================================================================
    // CURSO (DS09)
    // Ganhou o percentual minimo de conclusao (DS08).
    // =================================================================
    public class CursoViewModel
    {
        public int afr_Id { get; set; }

        [Required(ErrorMessage = "Informe o titulo do curso.")]
        [StringLength(120, MinimumLength = 3,
            ErrorMessage = "O titulo precisa ter entre 3 e 120 caracteres.")]
        [Display(Name = "Titulo")]
        public string afr_Titulo { get; set; } = "";

        [StringLength(500, ErrorMessage = "A descricao cabe em 500 caracteres.")]
        [Display(Name = "Descricao")]
        public string afr_Descricao { get; set; } = "";

        [Range(1, 1000, ErrorMessage = "A carga horaria vai de 1 a 1000 horas.")]
        [Display(Name = "Carga horaria (horas)")]
        public int afr_CargaHoraria { get; set; } = 20;

        [Range(1, 100, ErrorMessage = "O minimo para concluir vai de 1% a 100%.")]
        [Display(Name = "Minimo para concluir (%)")]
        public int afr_PercentualMinimo { get; set; } = 100;

        [Display(Name = "Curso ativo (visivel para o aluno)")]
        public bool afr_Ativo { get; set; } = true;

        public bool EhEdicao()
        {
            return afr_Id > 0;
        }
    }

    // =================================================================
    // AULAS DO CURSO - visao do tutor (DS05)
    // O conteudo da aula entra aqui.
    // =================================================================
    public class AulasAdminViewModel
    {
        public Curso afr_Curso { get; set; } = new Curso();
        public List<Aula> afr_Aulas { get; set; } = new List<Aula>();

        // Formulario de nova aula
        [Display(Name = "Titulo da aula")]
        public string afr_NovaAulaTitulo { get; set; } = "";

        [Range(1, 600, ErrorMessage = "A duracao vai de 1 a 600 minutos.")]
        [Display(Name = "Duracao (minutos)")]
        public int afr_NovaAulaDuracao { get; set; } = 45;

        [Display(Name = "Conteudo da aula")]
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
    // EDITAR UMA AULA - visao do tutor
    // =================================================================
    public class AulaFormViewModel
    {
        public int afr_Id { get; set; }
        public int afr_CursoId { get; set; }
        public string afr_CursoTitulo { get; set; } = "";
        public int afr_Ordem { get; set; }

        [Required(ErrorMessage = "Informe o titulo da aula.")]
        [StringLength(120, MinimumLength = 3,
            ErrorMessage = "O titulo precisa ter entre 3 e 120 caracteres.")]
        [Display(Name = "Titulo da aula")]
        public string afr_Titulo { get; set; } = "";

        [Range(1, 600, ErrorMessage = "A duracao vai de 1 a 600 minutos.")]
        [Display(Name = "Duracao (minutos)")]
        public int afr_DuracaoMin { get; set; } = 45;

        [Display(Name = "Conteudo da aula")]
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

        [Display(Name = "Buscar por titulo ou descricao")]
        public string afr_Termo { get; set; } = "";

        [Range(0, 1000, ErrorMessage = "A carga minima vai de 0 a 1000 horas.")]
        [Display(Name = "Carga horaria de")]
        public int afr_CargaMinima { get; set; } = 0;

        [Range(0, 1000, ErrorMessage = "A carga maxima vai de 0 a 1000 horas.")]
        [Display(Name = "ate")]
        public int afr_CargaMaxima { get; set; } = 0;

        [Display(Name = "Somente os cursos que eu ainda nao conclui")]
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

        // Mensagem da faixa de progresso
        public string ObterMensagemProgresso()
        {
            if (afr_Resultado.afr_Total == 0)
            {
                return "Este curso ainda nao tem aulas cadastradas.";
            }

            if (afr_Resultado.afr_Aprovado)
            {
                return "CONCLUIDO: voce bateu o minimo de "
                     + afr_Resultado.afr_PercentualMinimo + "% deste curso.";
            }

            if (afr_Resultado.afr_Concluidas > 0)
            {
                return "EM ANDAMENTO: faltam "
                     + (afr_Resultado.afr_Total - afr_Resultado.afr_Concluidas)
                     + " aula(s) para o minimo de "
                     + afr_Resultado.afr_PercentualMinimo + "%.";
            }

            return "NAO INICIADO: voce ainda nao concluiu nenhuma aula.";
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
                return "Aula concluida";
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
    // PROGRESSO DOS ALUNOS - visao do tutor (DS07)
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

            return "Excluir o usuario " + afr_Titulo;
        }

        // Para onde voltar se a pessoa cancelar
        public string ObterControllerDeVolta()
        {
            if (EhCurso())
            {
                return "Cursos";
            }

            return "Usuarios";
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
        public bool afr_EhTutor { get; set; }
        public bool afr_PodeGerenciarCursos { get; set; }
        public bool afr_PodeGerenciarUsuarios { get; set; }
        public bool afr_PodeVerProgresso { get; set; }

        public string afr_StatusBanco { get; set; } = "";

        public int afr_TotalCursos { get; set; }
        public int afr_TotalUsuarios { get; set; }

        // Para o aluno: quantos cursos ele ja concluiu
        public int afr_MeusCursos { get; set; }
        public int afr_MeusConcluidos { get; set; }

        public string DescreverPerfil()
        {
            if (afr_EhAdmin)
            {
                return "Perfil ADMIN: catalogo, cursos, progresso dos alunos e usuarios.";
            }

            if (afr_EhTutor)
            {
                return "Perfil TUTOR: catalogo, cursos e aulas, e progresso dos alunos.";
            }

            return "Perfil ALUNO: catalogo de cursos e as suas aulas.";
        }
    }
}
