/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: Modelo.cs  (classes de dominio)
   ---------------------------------------------------------------------
   VERSAO 2 - alinhada com a modelagem UML (Projeto_UML.asta)

   O QUE MUDOU EM RELACAO A VERSAO 1:
   1) Perfil PROFESSOR, que aparece nos diagramas DS07 a DS10
   2) Aula ganhou Conteudo (DS05: exibirConteudo)
   3) Curso ganhou PercentualMinimo (DS08: obterRequisitosCurso)
   4) Classes novas: FiltroCurso, LogAcao,
      ResultadoConclusao e ElegibilidadeExclusao

   OS TRES PERFIS, conforme os diagramas de sequencia:
     ALUNO  DS01-DS06 ... catalogo, busca, aulas e proprio progresso
     PROFESSOR  DS07-DS10 ... tudo do aluno + cursos, aulas e progresso
                          dos alunos
     ADMIN  DS11-DS14 ... tudo do professor + gerenciamento de usuarios
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
        public const string PERFIL_PROFESSOR = "PROFESSOR";
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
        // VERSAO 4 - a data de cadastro como texto.
        //
        // A API nao devolve a data de cadastro (o SELECT dela nao traz a
        // coluna). Sem esta conferencia a tela mostraria 01/01/0001, que
        // parece defeito; com ela, aparece um travessao.
        // -------------------------------------------------------------
        public string ObterDataCadastro(string afr_formato)
        {
            if (afr_DataCadastro > DateTime.MinValue)
            {
                return afr_DataCadastro.ToString(afr_formato);
            }

            return "-";
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

        public bool EhProfessor()
        {
            return afr_Perfil == PERFIL_PROFESSOR;
        }

        public bool EhAluno()
        {
            return afr_Perfil == PERFIL_ALUNO;
        }

        // DS09 e DS10: quem edita e exclui curso e o PROFESSOR.
        // O admin tambem pode, porque tem acesso a tudo.
        public bool PodeGerenciarCursos()
        {
            // Operador logico OU (||)
            return EhProfessor() || EhAdministrador();
        }

        // DS07: so o professor (e o admin) consultam o progresso de OUTRO aluno
        public bool PodeVerProgressoDeOutros()
        {
            return EhProfessor() || EhAdministrador();
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
            return new string[] { PERFIL_ALUNO, PERFIL_PROFESSOR, PERFIL_ADMIN };
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

        // Professor responsavel. Sem professor (null) quando o professor
        // foi excluido: o curso fica, e o admin escolhe outro.
        public int? afr_ProfessorId { get; set; }
        public string afr_ProfessorNome { get; set; } = "";

        public string ObterNomeProfessor()
        {
            if (afr_ProfessorId == null || string.IsNullOrWhiteSpace(afr_ProfessorNome))
            {
                return "Sem professor";
            }

            return afr_ProfessorNome;
        }

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
                afr_partes.Add("carga até " + afr_CargaMaxima + "h");
            }

            if (afr_SomenteNaoConcluidos)
            {
                afr_partes.Add("apenas não concluídos");
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
                return "CONCLUÍDO";
            }

            if (afr_Concluidas > 0)
            {
                return "EM ANDAMENTO";
            }

            return "NÃO INICIADO";
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
                return "Nenhum dado relacionado será afetado.";
            }

            return "- " + string.Join(Environment.NewLine + "- ", afr_DadosDetalhados);
        }
    }

    // =================================================================
    // CLASSE 12 - RegistroXP   (VERSAO 3.1)
    //
    // Uma linha de dbo.HistoricoXP: um ganho de XP que aconteceu.
    //
    // ReferenciaTipo + ReferenciaId dizem DE ONDE veio o ponto, e a
    // restricao UNIQUE (UsuarioId, ReferenciaTipo, ReferenciaId) e o
    // que impede a mesma aula de pagar duas vezes. A idempotencia que
    // a analise de integracao pede (item 3.4) mora ali, no banco, e
    // por isso vale igual para o desktop, o site e o celular.
    // =================================================================
    public class RegistroXP
    {
        public const string TIPO_AULA = "AULA";
        public const string TIPO_CONQUISTA = "CONQUISTA";

        public int afr_Id { get; set; }
        public int afr_UsuarioId { get; set; }
        public int afr_Quantidade { get; set; }
        public string afr_Motivo { get; set; } = "";
        public string afr_ReferenciaTipo { get; set; } = TIPO_AULA;
        public int afr_ReferenciaId { get; set; }
        public DateTime afr_DataRegistro { get; set; } = DateTime.Now;

        public string ObterResumo()
        {
            return afr_DataRegistro.ToString("dd/MM/yyyy HH:mm") +
                   "  +" + afr_Quantidade + " XP  " + afr_Motivo;
        }
    }

    // =================================================================
    // CLASSE 13 - Conquista   (VERSAO 3.1)
    //
    // Uma medalha do CATALOGO (dbo.Conquistas). Atencao: aqui a classe
    // representa a medalha que EXISTE, nao a que um aluno ganhou - quem
    // guarda isso e dbo.UsuarioConquistas.
    //
    // afr_Conquistada e afr_DataConquista sao preenchidos quando a
    // consulta traz o dado do aluno junto, para a tela conseguir
    // mostrar numa lista so o que ele tem e o que falta.
    //
    // O texto NAO fica no programa: vem do banco, porque a API tambem
    // le dali. Se ficasse aqui, a mesma medalha teria nomes diferentes
    // no celular e no desktop.
    // =================================================================
    public class Conquista
    {
        public const string PRIMEIRA_AULA = "PRIMEIRA_AULA";
        public const string CINCO_AULAS = "CINCO_AULAS";
        public const string DEZ_AULAS = "DEZ_AULAS";
        public const string PRIMEIRO_CURSO = "PRIMEIRO_CURSO";
        public const string SEQUENCIA_7 = "SEQUENCIA_7";

        public int afr_Id { get; set; }
        public string afr_Codigo { get; set; } = "";
        public string afr_Nome { get; set; } = "";
        public string afr_Descricao { get; set; } = "";
        public string afr_Icone { get; set; } = "";
        public int afr_XpBonus { get; set; }

        // Preenchidos quando a consulta junta os dados do aluno
        public bool afr_Conquistada { get; set; }
        public DateTime? afr_DataConquista { get; set; }

        public string ObterResumo()
        {
            string afr_texto = afr_Nome + " (+" + afr_XpBonus + " XP) - " + afr_Descricao;

            if (afr_Conquistada && afr_DataConquista.HasValue)
            {
                afr_texto = afr_texto + "  conquistada em " +
                            afr_DataConquista.Value.ToString("dd/MM/yyyy");
            }

            return afr_texto;
        }
    }

    // =================================================================
    // CLASSE 14 - Pontuacao   (VERSAO 3.1)
    //
    // O painel do aluno, montado a partir de dbo.Gamificacao.
    //
    // O nivel NAO e guardado: e calculado a partir do XP, com a mesma
    // conta da API (Math.floor(xp / 100) + 1), entao os dois sistemas
    // nunca mostram niveis diferentes para o mesmo aluno.
    // =================================================================
    public class Pontuacao
    {
        public const int PONTOS_POR_AULA = 10;    // igual ao da API
        public const int PONTOS_POR_NIVEL = 100;  // igual ao da API

        public int afr_UsuarioId { get; set; }
        public int afr_XpTotal { get; set; }

        // Dias seguidos estudando, e o recorde
        public int afr_SequenciaAtual { get; set; }
        public int afr_MelhorSequencia { get; set; }

        public int afr_AulasConcluidas { get; set; }
        public int afr_CursosConcluidos { get; set; }

        // TODAS as medalhas do catalogo, com afr_Conquistada marcando
        // quais o aluno ja tem
        public List<Conquista> afr_Conquistas { get; set; } = new List<Conquista>();

        public int ObterNivel()
        {
            return 1 + (afr_XpTotal / PONTOS_POR_NIVEL);
        }

        public int ObterXpNoNivel()
        {
            return afr_XpTotal % PONTOS_POR_NIVEL;
        }

        public int ObterXpParaProximoNivel()
        {
            return PONTOS_POR_NIVEL - ObterXpNoNivel();
        }

        // 0 a 100, para desenhar a barra de progresso
        public int ObterPercentualDoNivel()
        {
            return (ObterXpNoNivel() * 100) / PONTOS_POR_NIVEL;
        }

        public string ObterTituloDoNivel()
        {
            int afr_nivel = ObterNivel();

            if (afr_nivel <= 2)
            {
                return "Iniciante";
            }
            if (afr_nivel <= 4)
            {
                return "Aprendiz";
            }
            if (afr_nivel <= 7)
            {
                return "Dedicado";
            }
            if (afr_nivel <= 10)
            {
                return "Veterano";
            }

            return "Mestre";
        }

        public int ContarConquistadas()
        {
            int afr_total = 0;

            foreach (Conquista afr_item in afr_Conquistas)
            {
                if (afr_item.afr_Conquistada)
                {
                    afr_total = afr_total + 1;
                }
            }

            return afr_total;
        }

        public bool TemConquista(string afr_codigo)
        {
            foreach (Conquista afr_item in afr_Conquistas)
            {
                if (afr_item.afr_Codigo == afr_codigo && afr_item.afr_Conquistada)
                {
                    return true;
                }
            }

            return false;
        }

        public string ObterResumo()
        {
            return "Nível " + ObterNivel() + " (" + ObterTituloDoNivel() + ") - " +
                   afr_XpTotal + " XP - " +
                   ContarConquistadas() + " de " + afr_Conquistas.Count + " conquistas";
        }
    }

    // =================================================================
    // CLASSE 15 - GanhoGamificacao   (VERSAO 3.1)
    //
    // O que o aluno acabou de ganhar. Os campos sao os mesmos que a API
    // devolve em awardLessonCompletion, para o dia em que o programa
    // consumir a API nada precisar ser traduzido:
    //
    //   { xpAula, xpConquistas, xpTotal, sequenciaAtual,
    //     melhorSequencia, novasConquistas, nivel }
    // =================================================================
    public class GanhoGamificacao
    {
        public int afr_XpAula { get; set; }
        public int afr_XpConquistas { get; set; }
        public int afr_XpTotal { get; set; }
        public int afr_SequenciaAtual { get; set; }
        public int afr_MelhorSequencia { get; set; }

        public List<Conquista> afr_NovasConquistas { get; set; } = new List<Conquista>();

        public int ObterXpGanho()
        {
            return afr_XpAula + afr_XpConquistas;
        }

        public int ObterNivel()
        {
            return 1 + (afr_XpTotal / Pontuacao.PONTOS_POR_NIVEL);
        }

        // O nivel ANTES desta acao, para saber se subiu. A API nao
        // devolve isso, entao e calculado: o XP de antes era o total
        // menos o que acabou de entrar.
        public int ObterNivelAnterior()
        {
            int afr_xpAntes = afr_XpTotal - ObterXpGanho();

            if (afr_xpAntes < 0)
            {
                afr_xpAntes = 0;
            }

            return 1 + (afr_xpAntes / Pontuacao.PONTOS_POR_NIVEL);
        }

        public bool SubiuDeNivel()
        {
            return ObterNivel() > ObterNivelAnterior();
        }

        public bool TemNovidade()
        {
            // Operador logico OU (||)
            return ObterXpGanho() > 0 || afr_NovasConquistas.Count > 0;
        }

        // Monta o aviso que a tela mostra. Devolve "" quando nao houve
        // nada de novo, e ai a tela nao mostra aviso nenhum.
        public string ObterMensagem()
        {
            if (!TemNovidade())
            {
                return "";
            }

            List<string> afr_partes = new List<string>();

            if (ObterXpGanho() > 0)
            {
                afr_partes.Add("+" + ObterXpGanho() + " XP");
            }

            if (SubiuDeNivel())
            {
                afr_partes.Add("subiu para o nível " + ObterNivel());
            }

            foreach (Conquista afr_item in afr_NovasConquistas)
            {
                afr_partes.Add("conquista \"" + afr_item.afr_Nome + "\"");
            }

            if (afr_SequenciaAtual >= 2)
            {
                afr_partes.Add(afr_SequenciaAtual + " dias seguidos");
            }

            return string.Join(", ", afr_partes) + ".";
        }
    }

    // =================================================================
    // CLASSE 16 - LinhaRanking   (VERSAO 3.2)
    //
    // Uma linha da classificacao por XP. Nao tem tabela propria: e o
    // resultado de uma consulta que junta Usuarios e Gamificacao.
    //
    // Quem ve isto e professor ou administrador. O aluno ve so o
    // proprio painel, na tela principal - ranking visivel para a turma
    // inteira e uma decisao do grupo, nao um detalhe tecnico.
    //
    // O FORMATO E O DA ROTA /api/gamificacao/turma, por decisao do
    // grupo em 19/11. Por isso sairam daqui a contagem de aulas, a de
    // conquistas e o perfil: a rota nao devolve nenhum dos tres, e
    // manter campos que so existem no modo SQL faria a mesma tela
    // mostrar coisas diferentes conforme a origem dos dados. Tambem por
    // isso a consulta local passou a trazer so quem tem perfil ALUNO.
    // =================================================================
    public class LinhaRanking
    {
        public int afr_Posicao { get; set; }
        public int afr_UsuarioId { get; set; }
        public string afr_Nome { get; set; } = "";
        public string afr_Email { get; set; } = "";

        public int afr_XpTotal { get; set; }
        public int afr_SequenciaAtual { get; set; }
        public int afr_MelhorSequencia { get; set; }

        // Mesma conta da API: XP / 100 + 1
        public int ObterNivel()
        {
            return 1 + (afr_XpTotal / Pontuacao.PONTOS_POR_NIVEL);
        }

        public string ObterNivelTexto()
        {
            return "Nível " + ObterNivel();
        }

        // Uma medalha para os tres primeiros, e so quando ja ha XP:
        // liderar com zero ponto nao e lideranca.
        public string ObterMedalha()
        {
            if (afr_XpTotal <= 0)
            {
                return "";
            }

            if (afr_Posicao == 1)
            {
                return "1o";
            }
            if (afr_Posicao == 2)
            {
                return "2o";
            }
            if (afr_Posicao == 3)
            {
                return "3o";
            }

            return afr_Posicao + "o";
        }

        public string ObterResumo()
        {
            return afr_Posicao + "o  " + afr_Nome + "  -  " +
                   afr_XpTotal + " XP (nível " + ObterNivel() + ")";
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

        public static bool EhProfessor()
        {
            if (afr_usuarioLogado == null)
            {
                return false;
            }

            return afr_usuarioLogado.EhProfessor();
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
