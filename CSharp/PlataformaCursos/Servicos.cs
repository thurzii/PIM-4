/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: Servicos.cs  (regras de negocio)
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   Cada classe deste arquivo cuida de UM assunto. Elas ficam entre as
   telas e o banco: recebem os dados ja digitados, aplicam a regra e
   chamam a BancoService quando precisam gravar ou ler.

   Os nomes dos metodos seguem os DIAGRAMAS DE SEQUENCIA do Astah:
     DS01 Login ...................... ValidarLogin(email, senha)
     DS02 Recuperar Senha ............ RedefinirSenha(email, novaSenha)
     DS03 Visualizar Cursos .......... ExibirCursos()
     DS04 Buscar Cursos .............. BuscarCurso(termo) / LocalizarCurso
     DS05 Marcar Aula como Concluida . AtualizarProgresso(aluno, aula)
     DS06 Fazer Logout ............... EncerrarSessao()
     DS07-10 CRUD de Curso ........... CursoService
     DS11-14 CRUD de Usuario ......... UsuarioService

   CLASSES DESTE ARQUIVO:
   1) AutenticacaoService .. login, logout e redefinicao de senha
   2) UsuarioService ....... CRUD de usuarios e suas validacoes
   3) CursoService ......... CRUD de cursos, listagem e busca
   4) AulaService .......... aulas de um curso
   5) ProgressoService ..... progresso do aluno nas aulas
   ===================================================================== */

using System;
using System.Collections.Generic;

namespace PlataformaCursos
{
    // =================================================================
    // CLASSE 1 - AutenticacaoService
    // Casos de uso DC01 (Login), DC02 (Recuperar Senha), DC06 (Logout)
    // =================================================================
    public class AutenticacaoService
    {
        // ENCAPSULAMENTO: a tela nunca fala com o banco direto
        private BancoService afr_bancoService = new BancoService();

        // -------------------------------------------------------------
        // DS01 - validarLogin(email, senha)
        // Devolve o usuario quando o login e valido, ou null.
        // -------------------------------------------------------------
        public Usuario ValidarLogin(string afr_email, string afr_senha, out string afr_mensagem)
        {
            afr_mensagem = "";

            // Validacao dos campos antes de ir ao banco
            if (afr_email.Trim() == "" || afr_senha.Trim() == "")
            {
                afr_mensagem = "Informe o email e a senha.";
                return null;
            }

            Usuario afr_usuario = afr_bancoService.BuscarUsuarioPorEmail(afr_email.Trim());

            if (afr_usuario == null)
            {
                // Mensagem generica de proposito: nao revela se o email existe
                afr_mensagem = "Email ou senha incorretos.";
                return null;
            }

            if (afr_usuario.afr_Senha != afr_senha)
            {
                afr_mensagem = "Email ou senha incorretos.";
                return null;
            }

            if (!afr_usuario.afr_Ativo)
            {
                afr_mensagem = "Este usuario esta inativo. Procure o administrador.";
                return null;
            }

            // Login aprovado: abre a sessao
            Sessao.Abrir(afr_usuario);

            afr_mensagem = "Bem-vindo, " + afr_usuario.ObterPrimeiroNome() + ".";
            return afr_usuario;
        }

        // -------------------------------------------------------------
        // DS06 - Fazer Logout
        // -------------------------------------------------------------
        public void EncerrarSessao()
        {
            Sessao.Encerrar();
        }

        // -------------------------------------------------------------
        // DS02 - Recuperar Senha
        // Confere se o email existe antes de deixar redefinir.
        // -------------------------------------------------------------
        public bool EmailExiste(string afr_email)
        {
            return afr_bancoService.BuscarUsuarioPorEmail(afr_email.Trim()) != null;
        }

        public bool RedefinirSenha(string afr_email, string afr_novaSenha,
                                   string afr_confirmacao, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (afr_email.Trim() == "")
            {
                afr_mensagem = "Informe o email cadastrado.";
                return false;
            }

            if (!EmailExiste(afr_email))
            {
                afr_mensagem = "Email nao encontrado no sistema.";
                return false;
            }

            if (afr_novaSenha.Length < 6)
            {
                afr_mensagem = "A nova senha precisa ter pelo menos 6 caracteres.";
                return false;
            }

            // Operador logico DIFERENTE (!=)
            if (afr_novaSenha != afr_confirmacao)
            {
                afr_mensagem = "A confirmacao nao confere com a nova senha.";
                return false;
            }

            return afr_bancoService.AtualizarSenha(afr_email.Trim(), afr_novaSenha, out afr_mensagem);
        }
    }

    // =================================================================
    // CLASSE 2 - UsuarioService
    // Casos de uso DC11 a DC14 (CRUD de usuarios)
    // =================================================================
    public class UsuarioService
    {
        private BancoService afr_bancoService = new BancoService();

        // DS12 - Visualizar Usuarios
        public List<Usuario> ListarUsuarios(string afr_termo)
        {
            return afr_bancoService.ListarUsuarios(afr_termo);
        }

        // DS11 - Criar Usuario
        public bool CriarUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            if (!Validar(afr_usuario, true, out afr_mensagem))
            {
                return false;
            }

            return afr_bancoService.InserirUsuario(afr_usuario, out afr_mensagem);
        }

        // DS13 - Editar Usuario
        public bool EditarUsuario(Usuario afr_usuario, out string afr_mensagem)
        {
            if (!Validar(afr_usuario, false, out afr_mensagem))
            {
                return false;
            }

            return afr_bancoService.AtualizarUsuario(afr_usuario, out afr_mensagem);
        }

        // DS14 - Excluir Usuario
        public bool ExcluirUsuario(int afr_id, out string afr_mensagem)
        {
            // REGRA: ninguem pode excluir a propria conta enquanto usa o sistema
            if (Sessao.EstaAberta() && Sessao.ObterUsuario().afr_Id == afr_id)
            {
                afr_mensagem = "Voce nao pode excluir a sua propria conta.";
                return false;
            }

            return afr_bancoService.ExcluirUsuario(afr_id, out afr_mensagem);
        }

        // -------------------------------------------------------------
        // Validacao usada tanto na criacao quanto na edicao
        // -------------------------------------------------------------
        private bool Validar(Usuario afr_usuario, bool afr_ehNovo, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (afr_usuario.afr_Nome.Trim() == "")
            {
                afr_mensagem = "Informe o nome do usuario.";
                return false;
            }

            // Verificacao simples de email: precisa ter @ e ponto depois dele
            string afr_email = afr_usuario.afr_Email.Trim();
            int afr_posArroba = afr_email.IndexOf('@');

            // Operador logico OU (||)
            if (afr_posArroba < 1 || afr_email.IndexOf('.', afr_posArroba) < afr_posArroba + 2)
            {
                afr_mensagem = "Informe um email valido (exemplo: nome@escola.com).";
                return false;
            }

            if (afr_usuario.afr_Senha.Length < 6)
            {
                afr_mensagem = "A senha precisa ter pelo menos 6 caracteres.";
                return false;
            }

            // Operador logico E (&&)
            if (afr_usuario.afr_Perfil != Usuario.PERFIL_ADMIN &&
                afr_usuario.afr_Perfil != Usuario.PERFIL_ALUNO)
            {
                afr_mensagem = "Escolha o perfil do usuario.";
                return false;
            }

            return true;
        }
    }

    // =================================================================
    // CLASSE 3 - CursoService
    // Casos de uso DC03, DC04 e DC07 a DC10
    // =================================================================
    public class CursoService
    {
        private BancoService afr_bancoService = new BancoService();

        // DS03 - exibirCursos(cursos): o que o ALUNO enxerga.
        // Traz apenas os cursos ativos.
        public List<Curso> ExibirCursos()
        {
            return afr_bancoService.ListarCursos("", true);
        }

        // DS04 - buscarCurso(termo): a busca do aluno, so nos ativos
        public List<Curso> BuscarCurso(string afr_termo)
        {
            return afr_bancoService.ListarCursos(afr_termo, true);
        }

        // DS04 - localizarCurso(termo): a busca do ADMIN, que enxerga
        // tambem os cursos inativos
        public List<Curso> LocalizarCurso(string afr_termo)
        {
            return afr_bancoService.ListarCursos(afr_termo, false);
        }

        // DS07 - Criar Curso
        public bool CriarCurso(Curso afr_curso, out string afr_mensagem)
        {
            if (!Validar(afr_curso, out afr_mensagem))
            {
                return false;
            }

            return afr_bancoService.InserirCurso(afr_curso, out afr_mensagem);
        }

        // DS09 - Editar Curso
        public bool EditarCurso(Curso afr_curso, out string afr_mensagem)
        {
            if (!Validar(afr_curso, out afr_mensagem))
            {
                return false;
            }

            return afr_bancoService.AtualizarCurso(afr_curso, out afr_mensagem);
        }

        // DS10 - Excluir Curso
        public bool ExcluirCurso(int afr_id, out string afr_mensagem)
        {
            return afr_bancoService.ExcluirCurso(afr_id, out afr_mensagem);
        }

        private bool Validar(Curso afr_curso, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (afr_curso.afr_Titulo.Trim() == "")
            {
                afr_mensagem = "Informe o titulo do curso.";
                return false;
            }

            if (afr_curso.afr_CargaHoraria <= 0)
            {
                afr_mensagem = "A carga horaria precisa ser maior que zero.";
                return false;
            }

            return true;
        }
    }

    // =================================================================
    // CLASSE 4 - AulaService
    // As aulas que compoem cada curso
    // =================================================================
    public class AulaService
    {
        private BancoService afr_bancoService = new BancoService();

        // Aulas do curso, do ponto de vista de um aluno (marca as concluidas).
        // Para o admin, passe 0 no afr_usuarioId.
        public List<Aula> ListarAulas(int afr_cursoId, int afr_usuarioId)
        {
            return afr_bancoService.ListarAulasDoCurso(afr_cursoId, afr_usuarioId);
        }

        public bool CriarAula(Aula afr_aula, out string afr_mensagem)
        {
            afr_mensagem = "";

            if (afr_aula.afr_Titulo.Trim() == "")
            {
                afr_mensagem = "Informe o titulo da aula.";
                return false;
            }

            if (afr_aula.afr_CursoId <= 0)
            {
                afr_mensagem = "Escolha o curso ao qual a aula pertence.";
                return false;
            }

            if (afr_aula.afr_DuracaoMin <= 0)
            {
                afr_mensagem = "A duracao da aula precisa ser maior que zero.";
                return false;
            }

            return afr_bancoService.InserirAula(afr_aula, out afr_mensagem);
        }

        public bool ExcluirAula(int afr_id, out string afr_mensagem)
        {
            return afr_bancoService.ExcluirAula(afr_id, out afr_mensagem);
        }

        // Sugere o proximo numero de ordem para uma aula nova
        public int ProximaOrdem(int afr_cursoId)
        {
            List<Aula> afr_aulas = ListarAulas(afr_cursoId, 0);

            int afr_maior = 0;

            foreach (Aula afr_aula in afr_aulas)
            {
                if (afr_aula.afr_Ordem > afr_maior)
                {
                    afr_maior = afr_aula.afr_Ordem;
                }
            }

            return afr_maior + 1;
        }
    }

    // =================================================================
    // CLASSE 5 - ProgressoService
    // Caso de uso DC05 (Marcar Aula como Concluida)
    // =================================================================
    public class ProgressoService
    {
        private BancoService afr_bancoService = new BancoService();

        // DS05 - atualizarProgresso(aluno, aula)
        // afr_concluida = true marca, false desmarca.
        public bool AtualizarProgresso(int afr_usuarioId, int afr_aulaId,
                                       bool afr_concluida, out string afr_mensagem)
        {
            if (afr_concluida)
            {
                return afr_bancoService.MarcarAulaConcluida(afr_usuarioId, afr_aulaId, out afr_mensagem);
            }

            return afr_bancoService.DesmarcarAulaConcluida(afr_usuarioId, afr_aulaId, out afr_mensagem);
        }

        // DS08 - obterAulasConcluidas(aluno, curso)
        public int ObterAulasConcluidas(int afr_usuarioId, int afr_cursoId)
        {
            return afr_bancoService.ContarAulasConcluidas(afr_usuarioId, afr_cursoId);
        }

        // DS08 - verificarConclusao(aluno, curso)
        // Devolve a porcentagem concluida, de 0 a 100.
        public int CalcularPercentual(int afr_usuarioId, int afr_cursoId, int afr_totalAulas)
        {
            if (afr_totalAulas <= 0)
            {
                return 0;
            }

            int afr_concluidas = ObterAulasConcluidas(afr_usuarioId, afr_cursoId);

            // Operador matematico: divisao e multiplicacao
            double afr_percentual = (double)afr_concluidas / afr_totalAulas * 100;

            int afr_resultado = (int)Math.Round(afr_percentual);

            // Limita entre 0 e 100: a barra de progresso nao aceita
            // valores fora dessa faixa e lancaria uma excecao.
            if (afr_resultado < 0)
            {
                afr_resultado = 0;
            }

            if (afr_resultado > 100)
            {
                afr_resultado = 100;
            }

            return afr_resultado;
        }

        // DS08 - o curso esta concluido quando todas as aulas foram feitas
        public bool VerificarConclusao(int afr_usuarioId, int afr_cursoId, int afr_totalAulas)
        {
            // Operador logico E (&&)
            return afr_totalAulas > 0 &&
                   ObterAulasConcluidas(afr_usuarioId, afr_cursoId) == afr_totalAulas;
        }
    }
}
