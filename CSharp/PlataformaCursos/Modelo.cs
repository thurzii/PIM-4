/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: Modelo.cs  (classes de dominio)
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   Estas classes representam as ENTIDADES do sistema, exatamente como
   aparecem no diagrama de classes do Astah. Elas nao sabem nada sobre
   banco de dados nem sobre tela: apenas guardam os dados e devolvem
   informacoes calculadas a partir deles.

   CLASSES DESTE ARQUIVO:
   1) Pessoa ....... classe base (nome e email)
   2) Usuario ...... herda de Pessoa; acrescenta senha e perfil
   3) Curso ........ titulo, descricao, carga horaria
   4) Aula ......... pertence a um curso; tem ordem e duracao
   5) Progresso .... liga um aluno a uma aula concluida
   6) Sessao ....... guarda quem esta logado no momento
   ===================================================================== */

using System;

namespace PlataformaCursos
{
    // =================================================================
    // CLASSE 1 - Pessoa (CLASSE BASE)
    // Tudo o que qualquer pessoa do sistema tem.
    // =================================================================
    public class Pessoa
    {
        public int afr_Id { get; set; }
        public string afr_Nome { get; set; } = "";
        public string afr_Email { get; set; } = "";

        // Construtor vazio
        public Pessoa()
        {
        }

        // Construtor com parametros
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

        // Primeiro nome, usado na saudacao da tela principal
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
        // Perfis aceitos pelo sistema
        public const string PERFIL_ADMIN = "ADMIN";
        public const string PERFIL_ALUNO = "ALUNO";

        public string afr_Senha { get; set; } = "";
        public string afr_Perfil { get; set; } = PERFIL_ALUNO;
        public bool afr_Ativo { get; set; } = true;

        // Construtor vazio (usado quando o objeto vem do banco)
        public Usuario()
        {
        }

        // Construtor completo (usado no cadastro pela tela)
        public Usuario(string afr_nome, string afr_email, string afr_senha, string afr_perfil)
            : base(afr_nome, afr_email)
        {
            afr_Senha = afr_senha;
            afr_Perfil = afr_perfil;
        }

        // True quando o usuario tem permissao de administrador
        public bool EhAdministrador()
        {
            return afr_Perfil == PERFIL_ADMIN;
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

        // Preenchido pela consulta, para mostrar na lista
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
    }

    // =================================================================
    // CLASSE 4 - Aula
    // Uma aula sempre pertence a um curso (afr_CursoId).
    // =================================================================
    public class Aula
    {
        public int afr_Id { get; set; }
        public int afr_CursoId { get; set; }
        public string afr_Titulo { get; set; } = "";
        public int afr_Ordem { get; set; }
        public int afr_DuracaoMin { get; set; }

        // Preenchido pela consulta do aluno: se ELE ja concluiu esta aula
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
    // Registra que um aluno concluiu uma aula, e quando.
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
    // CLASSE 6 - Sessao
    // Guarda quem esta logado. E uma classe ESTATICA: existe uma so
    // para o programa inteiro, e qualquer tela consegue consultar.
    // =================================================================
    public static class Sessao
    {
        private static Usuario afr_usuarioLogado = null;

        // Abre a sessao depois que o login deu certo
        public static void Abrir(Usuario afr_usuario)
        {
            afr_usuarioLogado = afr_usuario;
        }

        // Encerra a sessao (caso de uso Fazer Logout)
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

        public static bool EhAdministrador()
        {
            if (afr_usuarioLogado == null)
            {
                return false;
            }

            return afr_usuarioLogado.EhAdministrador();
        }
    }
}
