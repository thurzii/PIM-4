/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmCatalogo.cs  (tela do aluno)
   ---------------------------------------------------------------------
   CASOS DE USO ATENDIDOS NESTA TELA:
   DC03 - Visualizar Cursos ......... lista dos cursos ativos
   DC04 - Buscar Cursos ............. campo de busca
   DC08 - Visualizar Curso .......... descricao, aulas e progresso
   DC05 - Marcar Aula como Concluida  caixa de selecao de cada aula

   Quando o aluno marca ou desmarca uma aula, o programa grava no banco
   na hora e recalcula a barra de progresso.
   ===================================================================== */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmCatalogo : Form
    {
        private CursoService afr_cursoService = new CursoService();
        private AulaService afr_aulaService = new AulaService();
        private ProgressoService afr_progressoService = new ProgressoService();

        private List<Curso> afr_listaAtual = new List<Curso>();

        private Curso afr_cursoSelecionado = null;

        // Evita que o evento da caixa dispare durante o carregamento
        private bool afr_carregando = false;

        // =============================================================
        // CONSTRUTOR
        // =============================================================
        public FrmCatalogo()
        {
            InitializeComponent();
        }

        // =============================================================
        // EVENTO LOAD - roda quando a janela ja existe na tela.
        // A checagem de sessao fica aqui, e nao no construtor, porque
        // fechar um formulario que ainda nao apareceu causa erro.
        // =============================================================
        private void FrmCatalogo_Load(object sender, EventArgs e)
        {
            if (!Sessao.EstaAberta())
            {
                MessageBox.Show("Sessao nao encontrada. Faca o login novamente.", "Acesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            Usuario afr_aluno = Sessao.ObterUsuario();

            afr_lblAluno.Text = "Aluno: " + afr_aluno.afr_Nome + "  |  " + afr_aluno.afr_Email;

            CarregarCursos("");
        }

        // =============================================================
        // EVENTO - BOTAO BUSCAR (DC04)
        // =============================================================
        private void afr_btnBuscar_Click(object sender, EventArgs e)
        {
            CarregarCursos(afr_txtBusca.Text);

            afr_lblStatus.Text = "Busca executada para o termo \"" + afr_txtBusca.Text.Trim() +
                                 "\": " + afr_listaAtual.Count + " curso(s) encontrado(s).";
        }

        // =============================================================
        // EVENTO - BOTAO VER TODOS (DC03)
        // =============================================================
        private void afr_btnVerTodos_Click(object sender, EventArgs e)
        {
            afr_txtBusca.Clear();

            CarregarCursos("");

            afr_lblStatus.Text = "Todos os cursos disponiveis: " + afr_listaAtual.Count + " curso(s).";
        }

        // =============================================================
        // EVENTO - BOTAO FECHAR
        // =============================================================
        private void afr_btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // =============================================================
        // EVENTO - CLIQUE EM UM CURSO (DC08)
        // =============================================================
        private void afr_lstCursos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (afr_lstCursos.SelectedItems.Count == 0)
            {
                return;
            }

            int afr_id = Convert.ToInt32(afr_lstCursos.SelectedItems[0].Text);

            foreach (Curso afr_curso in afr_listaAtual)
            {
                if (afr_curso.afr_Id == afr_id)
                {
                    afr_cursoSelecionado = afr_curso;

                    MostrarCurso(afr_curso);
                    return;
                }
            }
        }

        // =============================================================
        // EVENTO - MARCOU OU DESMARCOU UMA AULA (DC05)
        // =============================================================
        private void afr_lstAulas_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // Ignora enquanto a lista esta sendo preenchida pelo programa
            if (afr_carregando)
            {
                return;
            }

            if (afr_cursoSelecionado == null)
            {
                return;
            }

            // O Id da aula fica guardado na propriedade Tag do item
            int afr_aulaId = Convert.ToInt32(e.Item.Tag);
            int afr_alunoId = Sessao.ObterUsuario().afr_Id;

            string afr_mensagem;

            bool afr_ok = afr_progressoService.AtualizarProgresso(
                              afr_alunoId, afr_aulaId, e.Item.Checked, out afr_mensagem);

            if (!afr_ok)
            {
                MessageBox.Show(afr_mensagem, "Progresso",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Recalcula a barra de progresso com o numero novo
            AtualizarProgresso();

            if (e.Item.Checked)
            {
                afr_lblStatus.Text = "INSERT em dbo.Progresso: aula " + afr_aulaId +
                                     " marcada como concluida.";
            }
            else
            {
                afr_lblStatus.Text = "DELETE em dbo.Progresso: aula " + afr_aulaId +
                                     " voltou para nao concluida.";
            }
        }

        // =============================================================
        // METODO - LE OS CURSOS ATIVOS (DC03 e DC04)
        // =============================================================
        private void CarregarCursos(string afr_termo)
        {
            afr_lstCursos.Items.Clear();

            // ExibirCursos traz todos; BuscarCurso filtra pelo termo.
            // Os dois metodos mostram apenas os cursos ATIVOS.
            if (afr_termo.Trim() == "")
            {
                afr_listaAtual = afr_cursoService.ExibirCursos();
            }
            else
            {
                afr_listaAtual = afr_cursoService.BuscarCurso(afr_termo.Trim());
            }

            foreach (Curso afr_curso in afr_listaAtual)
            {
                ListViewItem afr_item = new ListViewItem(afr_curso.afr_Id.ToString());

                afr_item.SubItems.Add(afr_curso.afr_Titulo);
                afr_item.SubItems.Add(afr_curso.afr_CargaHoraria + "h");
                afr_item.SubItems.Add(afr_curso.afr_TotalAulas.ToString());

                afr_lstCursos.Items.Add(afr_item);
            }

            // Nenhum curso encontrado: limpa o painel da direita
            if (afr_listaAtual.Count == 0)
            {
                afr_cursoSelecionado = null;
                LimparPainel();

                afr_lblStatus.Text = "Nenhum curso encontrado para esse termo.";
            }
        }

        // =============================================================
        // METODO - MOSTRA O CURSO ESCOLHIDO E SUAS AULAS (DC08)
        // =============================================================
        private void MostrarCurso(Curso afr_curso)
        {
            afr_lblCursoTitulo.Text = afr_curso.afr_Titulo;
            afr_txtCursoDesc.Text = afr_curso.afr_Descricao;

            afr_grpAulas.Text = "Aulas de \"" + afr_curso.afr_Titulo +
                                "\" - marque a caixa para registrar a aula como concluida";

            CarregarAulas(afr_curso);
            AtualizarProgresso();
        }

        // =============================================================
        // METODO - LE AS AULAS JA COM AS CONCLUIDAS MARCADAS
        // =============================================================
        private void CarregarAulas(Curso afr_curso)
        {
            // Liga a trava: preencher as caixas nao deve gravar nada
            afr_carregando = true;

            afr_lstAulas.Items.Clear();

            int afr_alunoId = Sessao.ObterUsuario().afr_Id;

            List<Aula> afr_aulas = afr_aulaService.ListarAulas(afr_curso.afr_Id, afr_alunoId);

            foreach (Aula afr_aula in afr_aulas)
            {
                ListViewItem afr_item = new ListViewItem(afr_aula.afr_Ordem.ToString());

                afr_item.SubItems.Add(afr_aula.afr_Titulo);
                afr_item.SubItems.Add(afr_aula.afr_DuracaoMin + " min");
                afr_item.SubItems.Add(afr_aula.afr_Id.ToString());

                // Guarda o Id da aula para o evento da caixa usar depois
                afr_item.Tag = afr_aula.afr_Id;

                // Vem do banco: o aluno ja concluiu esta aula?
                afr_item.Checked = afr_aula.afr_Concluida;

                if (afr_aula.afr_Concluida)
                {
                    afr_item.ForeColor = Color.Green;
                }

                afr_lstAulas.Items.Add(afr_item);
            }

            afr_carregando = false;
        }

        // =============================================================
        // METODO - RECALCULA A BARRA DE PROGRESSO (DS08)
        // =============================================================
        private void AtualizarProgresso()
        {
            if (afr_cursoSelecionado == null)
            {
                return;
            }

            int afr_alunoId = Sessao.ObterUsuario().afr_Id;
            int afr_total = afr_lstAulas.Items.Count;

            int afr_concluidas = afr_progressoService.ObterAulasConcluidas(
                                     afr_alunoId, afr_cursoSelecionado.afr_Id);

            int afr_percentual = afr_progressoService.CalcularPercentual(
                                     afr_alunoId, afr_cursoSelecionado.afr_Id, afr_total);

            afr_prgProgresso.Value = afr_percentual;

            afr_lblProgressoTexto.Text = "Seu progresso: " + afr_percentual + "%  (" +
                                         afr_concluidas + " de " + afr_total + " aulas)";

            // CONDICAO: o curso so esta concluido quando todas as aulas foram feitas
            if (afr_progressoService.VerificarConclusao(afr_alunoId,
                                                        afr_cursoSelecionado.afr_Id,
                                                        afr_total))
            {
                afr_lblConclusao.ForeColor = Color.Green;
                afr_lblConclusao.Text = "Parabens! Voce concluiu todas as aulas deste curso.";
            }
            else if (afr_concluidas > 0)
            {
                afr_lblConclusao.ForeColor = Color.DarkOrange;
                afr_lblConclusao.Text = "Curso em andamento. Faltam " +
                                        (afr_total - afr_concluidas) + " aula(s).";
            }
            else
            {
                afr_lblConclusao.ForeColor = Color.DimGray;
                afr_lblConclusao.Text = "Voce ainda nao concluiu nenhuma aula deste curso.";
            }

            // Repinta de verde as aulas concluidas
            afr_carregando = true;

            foreach (ListViewItem afr_item in afr_lstAulas.Items)
            {
                if (afr_item.Checked)
                {
                    afr_item.ForeColor = Color.Green;
                }
                else
                {
                    afr_item.ForeColor = Color.Black;
                }
            }

            afr_carregando = false;
        }

        // =============================================================
        // METODO - LIMPA O PAINEL DA DIREITA
        // =============================================================
        private void LimparPainel()
        {
            afr_carregando = true;
            afr_lstAulas.Items.Clear();
            afr_carregando = false;

            afr_lblCursoTitulo.Text = "Escolha um curso na lista ao lado.";
            afr_txtCursoDesc.Clear();
            afr_prgProgresso.Value = 0;
            afr_lblProgressoTexto.Text = "Seu progresso: 0%";

            afr_lblConclusao.ForeColor = Color.DimGray;
            afr_lblConclusao.Text = "";

            afr_grpAulas.Text = "Aulas - marque a caixa para registrar a aula como concluida";
        }
    }
}
