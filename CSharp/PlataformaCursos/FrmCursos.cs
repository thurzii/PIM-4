/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmCursos.cs
   ---------------------------------------------------------------------
   CASOS DE USO ATENDIDOS NESTA TELA:
   DC07 - Criar Curso ........ botao Salvar em modo NOVO   -> INSERT
   DC08 - Visualizar Curso ... lista, busca e aulas        -> SELECT
   DC09 - Editar Curso ....... botao Salvar em modo EDICAO -> UPDATE
   DC10 - Excluir Curso ...... botao Excluir               -> DELETE

   A tela tambem cadastra e exclui AULAS do curso selecionado.
   ===================================================================== */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmCursos : Form
    {
        private CursoService afr_cursoService = new CursoService();
        private AulaService afr_aulaService = new AulaService();
        private BancoService afr_bancoService = new BancoService();

        // afr_idSelecionado = 0 -> modo NOVO;  > 0 -> modo EDICAO
        private int afr_idSelecionado = 0;

        private List<Curso> afr_listaAtual = new List<Curso>();

        private bool afr_carregandoLista = false;

        // =============================================================
        // CONSTRUTOR
        // =============================================================
        public FrmCursos()
        {
            InitializeComponent();

            AtualizarLista("");
            EntrarModoNovo();
        }

        // =============================================================
        // EVENTO - BOTAO NOVO
        // =============================================================
        private void afr_btnNovoCurso_Click(object sender, EventArgs e)
        {
            EntrarModoNovo();
        }

        // =============================================================
        // EVENTO - BOTAO SALVAR (INSERT ou UPDATE)
        // =============================================================
        private void afr_btnSalvarCurso_Click(object sender, EventArgs e)
        {
            // Validacao da carga horaria antes de montar o objeto
            int afr_carga;

            if (!int.TryParse(afr_txtCarga.Text.Trim(), out afr_carga))
            {
                MessageBox.Show("A carga horaria precisa ser um numero inteiro.", "Atencao",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                afr_txtCarga.Focus();
                return;
            }

            Curso afr_curso = new Curso();

            afr_curso.afr_Id = afr_idSelecionado;
            afr_curso.afr_Titulo = afr_txtTitulo.Text.Trim();
            afr_curso.afr_Descricao = afr_txtDescricao.Text.Trim();
            afr_curso.afr_CargaHoraria = afr_carga;
            afr_curso.afr_Ativo = afr_chkAtivoCurso.Checked;

            string afr_mensagem;
            bool afr_ok;
            string afr_comando;

            if (afr_idSelecionado == 0)
            {
                afr_comando = "INSERT";
                afr_ok = afr_cursoService.CriarCurso(afr_curso, out afr_mensagem);
            }
            else
            {
                afr_comando = "UPDATE";
                afr_ok = afr_cursoService.EditarCurso(afr_curso, out afr_mensagem);
            }

            if (!afr_ok)
            {
                MessageBox.Show(afr_mensagem, "Atencao",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show(afr_comando + " executado." + Environment.NewLine + afr_mensagem,
                            "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            AtualizarLista(afr_txtBusca.Text);
            EntrarModoNovo();
        }

        // =============================================================
        // EVENTO - BOTAO EXCLUIR (DELETE)
        // =============================================================
        private void afr_btnExcluirCurso_Click(object sender, EventArgs e)
        {
            if (afr_idSelecionado == 0)
            {
                MessageBox.Show("Clique primeiro no curso da lista que voce quer excluir.",
                                "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult afr_resposta = MessageBox.Show(
                "Excluir o curso \"" + afr_txtTitulo.Text + "\" (Id " + afr_idSelecionado + ")?" +
                Environment.NewLine + Environment.NewLine +
                "As aulas do curso e o progresso dos alunos nelas tambem serao apagados.",
                "Confirmacao", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (afr_resposta != DialogResult.Yes)
            {
                return;
            }

            string afr_mensagem;

            if (afr_cursoService.ExcluirCurso(afr_idSelecionado, out afr_mensagem))
            {
                MessageBox.Show("DELETE executado." + Environment.NewLine + afr_mensagem,
                                "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                AtualizarLista(afr_txtBusca.Text);
                EntrarModoNovo();
            }
            else
            {
                MessageBox.Show(afr_mensagem, "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // EVENTO - BOTAO BUSCAR
        // O admin usa LocalizarCurso, que traz tambem os inativos.
        // =============================================================
        private void afr_btnBuscar_Click(object sender, EventArgs e)
        {
            AtualizarLista(afr_txtBusca.Text);
            EntrarModoNovo();
        }

        // =============================================================
        // EVENTO - BOTAO FECHAR
        // =============================================================
        private void afr_btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // =============================================================
        // EVENTO - CLIQUE EM UM CURSO DA LISTA
        // =============================================================
        private void afr_lstCursos_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (afr_carregandoLista)
            {
                return;
            }

            if (afr_lstCursos.SelectedItems.Count == 0)
            {
                return;
            }

            int afr_id = Convert.ToInt32(afr_lstCursos.SelectedItems[0].Text);

            foreach (Curso afr_curso in afr_listaAtual)
            {
                if (afr_curso.afr_Id == afr_id)
                {
                    PreencherCampos(afr_curso);
                    return;
                }
            }
        }

        // =============================================================
        // EVENTO - INCLUIR AULA no curso selecionado
        // =============================================================
        private void afr_btnAddAula_Click(object sender, EventArgs e)
        {
            if (afr_idSelecionado == 0)
            {
                MessageBox.Show("Escolha primeiro um curso na lista.", "Atencao",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int afr_duracao;

            if (!int.TryParse(afr_txtAulaDuracao.Text.Trim(), out afr_duracao))
            {
                MessageBox.Show("A duracao da aula precisa ser um numero inteiro de minutos.",
                                "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                afr_txtAulaDuracao.Focus();
                return;
            }

            Aula afr_aula = new Aula();

            afr_aula.afr_CursoId = afr_idSelecionado;
            afr_aula.afr_Titulo = afr_txtAulaTitulo.Text.Trim();
            afr_aula.afr_DuracaoMin = afr_duracao;

            // A ordem e calculada: entra sempre como a ultima aula do curso
            afr_aula.afr_Ordem = afr_aulaService.ProximaOrdem(afr_idSelecionado);

            string afr_mensagem;

            if (!afr_aulaService.CriarAula(afr_aula, out afr_mensagem))
            {
                MessageBox.Show(afr_mensagem, "Atencao",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            afr_txtAulaTitulo.Clear();
            afr_txtAulaDuracao.Text = "45";

            CarregarAulas(afr_idSelecionado);

            // A quantidade de aulas mudou: recarrega a lista de cursos,
            // mantendo o curso atual selecionado
            AtualizarLista(afr_txtBusca.Text);
            SelecionarCursoNaLista(afr_idSelecionado);

            afr_lblStatus.Text = "INSERT na tabela dbo.Aulas: " + afr_mensagem;
            afr_txtAulaTitulo.Focus();
        }

        // =============================================================
        // EVENTO - EXCLUIR AULA
        // =============================================================
        private void afr_btnDelAula_Click(object sender, EventArgs e)
        {
            if (afr_lstAulas.SelectedItems.Count == 0)
            {
                MessageBox.Show("Selecione a aula que voce quer excluir.", "Atencao",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int afr_aulaId = Convert.ToInt32(afr_lstAulas.SelectedItems[0].Text);
            string afr_tituloAula = afr_lstAulas.SelectedItems[0].SubItems[2].Text;

            DialogResult afr_resposta = MessageBox.Show(
                "Excluir a aula \"" + afr_tituloAula + "\"?",
                "Confirmacao", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (afr_resposta != DialogResult.Yes)
            {
                return;
            }

            string afr_mensagem;

            if (afr_aulaService.ExcluirAula(afr_aulaId, out afr_mensagem))
            {
                CarregarAulas(afr_idSelecionado);

                AtualizarLista(afr_txtBusca.Text);
                SelecionarCursoNaLista(afr_idSelecionado);

                afr_lblStatus.Text = "DELETE na tabela dbo.Aulas: " + afr_mensagem;
            }
            else
            {
                MessageBox.Show(afr_mensagem, "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // METODO - LE OS CURSOS E PREENCHE A LISTA
        // =============================================================
        private void AtualizarLista(string afr_termo)
        {
            afr_carregandoLista = true;

            afr_lstCursos.Items.Clear();

            // O admin enxerga cursos ativos E inativos
            afr_listaAtual = afr_cursoService.LocalizarCurso(afr_termo.Trim());

            foreach (Curso afr_curso in afr_listaAtual)
            {
                ListViewItem afr_item = new ListViewItem(afr_curso.afr_Id.ToString());

                afr_item.SubItems.Add(afr_curso.afr_Titulo);
                afr_item.SubItems.Add(afr_curso.afr_CargaHoraria + "h");
                afr_item.SubItems.Add(afr_curso.afr_TotalAulas.ToString());

                if (afr_curso.afr_Ativo)
                {
                    afr_item.SubItems.Add("Sim");
                }
                else
                {
                    afr_item.SubItems.Add("Nao");
                    afr_item.ForeColor = Color.Gray;
                }

                afr_lstCursos.Items.Add(afr_item);
            }

            afr_lblStatus.Text = afr_listaAtual.Count + " curso(s) em dbo.Cursos  |  " +
                                 afr_bancoService.ObterDescricao();

            afr_carregandoLista = false;
        }

        // =============================================================
        // METODO - LE AS AULAS DO CURSO E PREENCHE A SEGUNDA LISTA
        // =============================================================
        private void CarregarAulas(int afr_cursoId)
        {
            afr_lstAulas.Items.Clear();

            if (afr_cursoId == 0)
            {
                afr_grpAulas.Text = "Aulas do curso selecionado";
                return;
            }

            // Passa 0 no usuario: o admin nao tem progresso a mostrar
            List<Aula> afr_aulas = afr_aulaService.ListarAulas(afr_cursoId, 0);

            foreach (Aula afr_aula in afr_aulas)
            {
                ListViewItem afr_item = new ListViewItem(afr_aula.afr_Id.ToString());

                afr_item.SubItems.Add(afr_aula.afr_Ordem.ToString());
                afr_item.SubItems.Add(afr_aula.afr_Titulo);
                afr_item.SubItems.Add(afr_aula.afr_DuracaoMin + " min");

                afr_lstAulas.Items.Add(afr_item);
            }

            afr_grpAulas.Text = "Aulas de \"" + afr_txtTitulo.Text + "\"  -  " +
                                afr_aulas.Count + " aula(s)";
        }

        // =============================================================
        // METODO - DEIXA UM CURSO SELECIONADO NA LISTA PELO Id
        // =============================================================
        private void SelecionarCursoNaLista(int afr_cursoId)
        {
            afr_carregandoLista = true;

            foreach (ListViewItem afr_item in afr_lstCursos.Items)
            {
                if (afr_item.Text == afr_cursoId.ToString())
                {
                    afr_item.Selected = true;
                    afr_item.EnsureVisible();
                    break;
                }
            }

            afr_carregandoLista = false;
        }

        // =============================================================
        // METODO - CARREGA O CURSO NOS CAMPOS (modo EDICAO)
        // =============================================================
        private void PreencherCampos(Curso afr_curso)
        {
            afr_idSelecionado = afr_curso.afr_Id;

            afr_txtTitulo.Text = afr_curso.afr_Titulo;
            afr_txtDescricao.Text = afr_curso.afr_Descricao;
            afr_txtCarga.Text = afr_curso.afr_CargaHoraria.ToString();
            afr_chkAtivoCurso.Checked = afr_curso.afr_Ativo;

            afr_lblModo.ForeColor = Color.DarkOrange;
            afr_lblModo.Text = "EDITANDO O Id " + afr_curso.afr_Id;

            CarregarAulas(afr_curso.afr_Id);
        }

        // =============================================================
        // METODO - VOLTA PARA O MODO DE CADASTRO
        // =============================================================
        private void EntrarModoNovo()
        {
            afr_idSelecionado = 0;

            afr_carregandoLista = true;
            afr_lstCursos.SelectedItems.Clear();
            afr_carregandoLista = false;

            afr_txtTitulo.Clear();
            afr_txtDescricao.Clear();
            afr_txtCarga.Text = "20";
            afr_chkAtivoCurso.Checked = true;

            afr_lblModo.ForeColor = Color.Black;
            afr_lblModo.Text = "NOVO CURSO";

            CarregarAulas(0);

            afr_txtTitulo.Focus();
        }
    }
}
