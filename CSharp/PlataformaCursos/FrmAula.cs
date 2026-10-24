/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmAula.cs
   Autor.....: Arthur Fernandes Reis (prefixo afr_)
   ---------------------------------------------------------------------
   TELA DE AULA (TelaAula do modelo)

   Esta tela atende o diagrama DS05 (Marcar Aula como Concluida).
   A sequencia de mensagens do modelo e esta:

       1) assistirAula(aula)        -> o aluno escolhe a aula
       2) abrirAula(aula)           -> o sistema busca a aula
       3) exibirConteudo()          -> a tela mostra o conteudo
       4) concluirAula()            -> o aluno clica no botao
       5) atualizarProgresso(aluno, aula)
       6) salvarProgresso(progresso)
       7) confirmacao()
       8) progressoAtualizado()
       9) aulaConcluida()
      10) exibirConclusao()         -> mostra o resultado do curso

   Antes nao existia tela de aula: o catalogo marcava a aula direto
   num checkbox. O modelo pede o passo "exibirConteudo()", entao agora
   a aula tem conteudo proprio e tela propria.
   ===================================================================== */

using System;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmAula : Form
    {
        // Servicos usados por esta tela
        private AulaService afr_aulaService = new AulaService();
        private ProgressoService afr_progressoService = new ProgressoService();

        // Dados recebidos de quem abriu a tela
        private int afr_aulaId = 0;
        private Curso afr_curso = new Curso();

        // A aula carregada do banco
        private Aula afr_aula = null;

        // Avisa o catalogo se o progresso mudou, para ele recarregar
        public bool afr_HouveMudanca { get; private set; }

        // -------------------------------------------------------------
        // CONSTRUTOR
        // DS05 - assistirAula(aula): recebe qual aula e de qual curso
        // -------------------------------------------------------------
        public FrmAula(int afr_id, Curso afr_cursoDaAula)
        {
            InitializeComponent();

            this.afr_aulaId = afr_id;
            this.afr_curso = afr_cursoDaAula ?? new Curso();
            this.afr_HouveMudanca = false;
        }

        // -------------------------------------------------------------
        // LOAD
        // As verificacoes ficam aqui, e nao no construtor. Chamar
        // Close() dentro do construtor descarta o formulario antes de
        // ele aparecer, e o ShowDialog() depois disso da erro.
        // -------------------------------------------------------------
        private void FrmAula_Load(object sender, EventArgs e)
        {
            if (!Sessao.EstaAberta())
            {
                MessageBox.Show("Sessao encerrada. Entre novamente.",
                                "Aula",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            AbrirAula();
        }

        // -------------------------------------------------------------
        // DS05 - abrirAula(aula) + exibirConteudo()
        // -------------------------------------------------------------
        private void AbrirAula()
        {
            // abrirAula(aula): o servico traz a aula e tambem se ela
            // ja esta concluida por quem esta logado
            afr_aula = afr_aulaService.AbrirAula(afr_aulaId, Sessao.ObterUsuarioId());

            if (afr_aula == null)
            {
                MessageBox.Show("Aula nao encontrada.",
                                "Aula",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            // exibirConteudo(): joga os dados da aula na tela
            this.Text = "Aula - " + afr_aula.afr_Titulo;

            afr_lblCurso.Text = "Curso: " + afr_curso.afr_Titulo;
            afr_lblTituloAula.Text = afr_aula.afr_Ordem + ". " + afr_aula.afr_Titulo;
            afr_lblInfo.Text = "Duracao: " + afr_aula.afr_DuracaoMin + " minutos"
                             + "   |   Aula " + afr_aula.afr_Ordem
                             + " de " + afr_curso.afr_TotalAulas;

            if (afr_aula.afr_Conteudo.Trim() == "")
            {
                afr_txtConteudo.Text = "Esta aula ainda nao tem conteudo cadastrado. "
                                     + "O tutor pode escrever o conteudo na tela de cursos.";
            }
            else
            {
                afr_txtConteudo.Text = afr_aula.afr_Conteudo;
            }

            // Deixa o cursor no inicio do texto, nao no fim
            afr_txtConteudo.SelectionStart = 0;
            afr_txtConteudo.SelectionLength = 0;

            AtualizarBotao();
        }

        // -------------------------------------------------------------
        // Ajusta o rotulo e o botao conforme a aula esta concluida ou nao
        // -------------------------------------------------------------
        private void AtualizarBotao()
        {
            if (afr_aula.afr_Concluida)
            {
                afr_lblStatusAula.ForeColor = System.Drawing.Color.SeaGreen;
                afr_lblStatusAula.Text = "Status: aula concluida";

                afr_btnConcluir.Text = "Desmarcar conclusao";
                afr_btnConcluir.BackColor = System.Drawing.Color.LightYellow;
            }
            else
            {
                afr_lblStatusAula.ForeColor = System.Drawing.Color.Chocolate;
                afr_lblStatusAula.Text = "Status: aula pendente";

                afr_btnConcluir.Text = "Marcar como concluida";
                afr_btnConcluir.BackColor = System.Drawing.Color.Honeydew;
            }
        }

        // -------------------------------------------------------------
        // DS05 - concluirAula() -> atualizarProgresso(aluno, aula)
        //        -> salvarProgresso -> aulaConcluida() -> exibirConclusao()
        // -------------------------------------------------------------
        private void afr_btnConcluir_Click(object sender, EventArgs e)
        {
            if (afr_aula == null)
            {
                return;
            }

            afr_lblStatus.Text = "";

            // O botao alterna: se esta pendente marca, se esta
            // concluida desmarca.
            bool afr_novoEstado = !afr_aula.afr_Concluida;

            // atualizarProgresso(aluno, aula) -> salvarProgresso(progresso)
            string afr_mensagem;
            bool afr_ok = afr_progressoService.AtualizarProgresso(
                              Sessao.ObterUsuarioId(),
                              afr_aula.afr_Id,
                              afr_novoEstado,
                              out afr_mensagem);

            if (!afr_ok)
            {
                afr_lblStatus.ForeColor = System.Drawing.Color.Firebrick;
                afr_lblStatus.Text = afr_mensagem;
                return;
            }

            // progressoAtualizado() / aulaConcluida()
            afr_aula.afr_Concluida = afr_novoEstado;
            afr_HouveMudanca = true;
            AtualizarBotao();

            afr_lblStatus.ForeColor = System.Drawing.Color.SeaGreen;
            afr_lblStatus.Text = afr_mensagem;

            // exibirConclusao(): depois de mexer no progresso o sistema
            // conta como esta o curso inteiro. Se bateu o minimo exigido,
            // avisa o aluno na hora.
            ExibirConclusao();
        }

        // -------------------------------------------------------------
        // DS05 - exibirConclusao()
        // -------------------------------------------------------------
        private void ExibirConclusao()
        {
            ResultadoConclusao afr_resultado =
                afr_progressoService.VerificarConclusao(Sessao.ObterUsuarioId(), afr_curso);

            // Mostra o andamento do curso junto do status da aula
            afr_lblStatusAula.Text = afr_lblStatusAula.Text
                                   + "   |   Curso: " + afr_resultado.afr_Concluidas
                                   + "/" + afr_resultado.afr_Total
                                   + " aulas (" + afr_resultado.afr_Percentual + "%)";

            // Avisa com destaque somente quando o curso foi aprovado
            // e a acao foi de marcar (nao de desmarcar).
            if (afr_resultado.afr_Aprovado && afr_aula.afr_Concluida)
            {
                MessageBox.Show(afr_resultado.ObterResumo(),
                                "Curso concluido",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        // -------------------------------------------------------------
        // Fechar a tela
        // -------------------------------------------------------------
        private void afr_btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
