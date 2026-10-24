/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmCursos.cs
   Autor.....: Arthur Fernandes Reis (prefixo afr_)
   ---------------------------------------------------------------------
   CASOS DE USO ATENDIDOS NESTA TELA:
   DC07 / DS07? - Criar Curso ..... botao Salvar em modo NOVO  -> INSERT
   DC08 - Visualizar Curso ........ lista, busca e aulas       -> SELECT
   DC09 / DS09 - Editar Curso ..... botao Salvar em modo EDICAO-> UPDATE
   DC10 / DS10 - Excluir Curso .... botao Excluir em 2 etapas  -> DELETE

   O QUE MUDOU POR CAUSA DO MODELO:

   1) Quem entra aqui agora e o TUTOR (e o admin, que pode tudo). Os
      diagramas DS09 e DS10 tem a linha de vida "Tutor", nao "Admin".
      A tela confere isso com VerificarPermissaoTutor.

   2) DS10 nao exclui com um clique. A ordem do modelo e:
      solicitarExclusao -> verificarElegibilidadeExclusao ->
      elegivelParaExclusao(true, dadosDetalhados) ->
      solicitarConfirmacaoExclusao(dadosDetalhados) ->
      confirmarExclusaoFinal -> confirmarIdentidadeEAcao ->
      removerDefinitivamente. As tres do meio acontecem no
      FrmConfirmarExclusao.

   3) DS09 pede getDadosCurso(curso) e dadosCurso(titulo, desc) antes
      de montar o formulario. A tela usa GetDadosCurso para reler o
      curso do banco em vez de confiar no que esta na lista.

   4) O curso tem percentual minimo de conclusao (DS08) e a aula tem
      conteudo (DS05). Os dois ganharam campo nesta tela.
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

        // Aula que esta aberta no editor de conteudo
        private int afr_aulaSelecionada = 0;

        private List<Curso> afr_listaAtual = new List<Curso>();

        private bool afr_carregandoLista = false;

        // =============================================================
        // CONSTRUTOR
        // =============================================================
        public FrmCursos()
        {
            InitializeComponent();
        }

        // =============================================================
        // EVENTO LOAD
        // A permissao e conferida aqui, e nao no construtor, porque
        // fechar um formulario que ainda nao apareceu causa erro.
        // =============================================================
        private void FrmCursos_Load(object sender, EventArgs e)
        {
            // DS09 e DS10 - quem gerencia curso e o tutor (ou o admin)
            string afr_mensagem;

            if (!afr_cursoService.VerificarPermissaoTutor(out afr_mensagem))
            {
                MessageBox.Show(afr_mensagem, "Acesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

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
        // DS09 - realizarAtualizacao -> validarEAtualizar -> sucesso
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

            // DS08 - o requisito de conclusao do curso
            afr_curso.afr_PercentualMinimo = Convert.ToInt32(afr_numMinimo.Value);

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

            // DS09 - exibirMensagemSucesso() / confirmacaoSucesso()
            MessageBox.Show(afr_comando + " executado." + Environment.NewLine + afr_mensagem,
                            "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

            AtualizarLista(afr_txtBusca.Text);
            EntrarModoNovo();
        }

        // =============================================================
        // EVENTO - BOTAO EXCLUIR (DELETE em duas etapas)
        // DS10 - solicitarExclusao(curso)
        //        -> verificarElegibilidadeExclusao(curso)
        //        -> elegivelParaExclusao(true, dadosDetalhados)
        //        -> solicitarConfirmacaoExclusao(dadosDetalhados)
        //        -> confirmarExclusaoFinal + confirmarIdentidadeEAcao
        //        -> removerDefinitivamente(curso)
        // =============================================================
        private void afr_btnExcluirCurso_Click(object sender, EventArgs e)
        {
            // solicitarExclusao(curso)
            if (afr_idSelecionado == 0)
            {
                MessageBox.Show("Clique primeiro no curso da lista que voce quer excluir.",
                                "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // verificarElegibilidadeExclusao(curso)
            // -> elegivelParaExclusao(true, dadosDetalhados)
            ElegibilidadeExclusao afr_elegibilidade =
                afr_cursoService.VerificarElegibilidadeExclusao(afr_idSelecionado);

            // solicitarConfirmacaoExclusao(dadosDetalhados)
            // A tela de confirmacao mostra a lista detalhada e pede a
            // senha do tutor (confirmarIdentidadeEAcao).
            FrmConfirmarExclusao afr_confirmacao = new FrmConfirmarExclusao(
                "Excluir o curso \"" + afr_txtTitulo.Text + "\" (Id " + afr_idSelecionado + ")",
                afr_elegibilidade);

            afr_confirmacao.ShowDialog();

            // confirmarExclusaoFinal(curso): so segue se a tela voltou com sim
            if (!afr_confirmacao.afr_Confirmado)
            {
                afr_lblStatus.ForeColor = Color.DimGray;
                afr_lblStatus.Text = "Exclusao cancelada. Nada foi apagado.";
                return;
            }

            // removerDefinitivamente(curso)
            string afr_mensagem;

            if (afr_cursoService.RemoverDefinitivamente(afr_idSelecionado, out afr_mensagem))
            {
                // remocaoSucesso() / exibirMensagemSucesso()
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
        // O tutor usa LocalizarCurso, que traz tambem os inativos.
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
        // DS09 - solicitarEdicao(curso) -> buscarCurso -> getDadosCurso
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

            // getDadosCurso(curso) -> dadosCurso(titulo, desc)
            // Vai ao banco em vez de usar o objeto que esta na lista:
            // assim a tela sempre edita o dado mais novo.
            Curso afr_curso = afr_cursoService.GetDadosCurso(afr_id);

            if (afr_curso == null)
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
                afr_lblStatus.Text = "Curso Id " + afr_id + " nao esta mais no banco. "
                                   + "Clique em Buscar para atualizar a lista.";
                return;
            }

            // exibirDadosParaEdicao(dados, curso)
            PreencherCampos(afr_curso);
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

            // DS05 - a aula ja nasce com o conteudo que estiver na caixa
            afr_aula.afr_Conteudo = afr_txtAulaConteudo.Text.Trim();

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
            afr_txtAulaConteudo.Clear();
            afr_aulaSelecionada = 0;

            CarregarAulas(afr_idSelecionado);

            // A quantidade de aulas mudou: recarrega a lista de cursos,
            // mantendo o curso atual selecionado
            AtualizarLista(afr_txtBusca.Text);
            SelecionarCursoNaLista(afr_idSelecionado);

            afr_lblStatus.ForeColor = Color.DimGray;
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
                "Excluir a aula \"" + afr_tituloAula + "\"?" + Environment.NewLine +
                "O progresso dos alunos nesta aula tambem sai.",
                "Confirmacao", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (afr_resposta != DialogResult.Yes)
            {
                return;
            }

            string afr_mensagem;

            if (afr_aulaService.ExcluirAula(afr_aulaId, out afr_mensagem))
            {
                afr_aulaSelecionada = 0;
                afr_txtAulaConteudo.Clear();

                CarregarAulas(afr_idSelecionado);

                AtualizarLista(afr_txtBusca.Text);
                SelecionarCursoNaLista(afr_idSelecionado);

                afr_lblStatus.ForeColor = Color.DimGray;
                afr_lblStatus.Text = "DELETE na tabela dbo.Aulas: " + afr_mensagem;
            }
            else
            {
                MessageBox.Show(afr_mensagem, "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // EVENTO - CLIQUE EM UMA AULA DA LISTA
        // Traz o conteudo dela para a caixa de texto.
        // =============================================================
        private void afr_lstAulas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (afr_lstAulas.SelectedItems.Count == 0)
            {
                return;
            }

            int afr_aulaId = Convert.ToInt32(afr_lstAulas.SelectedItems[0].Text);

            // Passa 0 no usuario: aqui nao interessa o progresso de ninguem
            Aula afr_aula = afr_aulaService.AbrirAula(afr_aulaId, 0);

            if (afr_aula == null)
            {
                return;
            }

            afr_aulaSelecionada = afr_aula.afr_Id;

            afr_txtAulaConteudo.Text = afr_aula.afr_Conteudo;
            afr_txtAulaTitulo.Text = afr_aula.afr_Titulo;
            afr_txtAulaDuracao.Text = afr_aula.afr_DuracaoMin.ToString();

            afr_lblAulaConteudo.Text = "Conteudo da aula " + afr_aula.afr_Ordem +
                                       " (Id " + afr_aula.afr_Id + "):";
        }

        // =============================================================
        // EVENTO - SALVAR O CONTEUDO DA AULA (UPDATE em dbo.Aulas)
        // DS05 - e o texto que o "exibirConteudo()" mostra para o aluno
        // =============================================================
        private void afr_btnSalvarAula_Click(object sender, EventArgs e)
        {
            if (afr_aulaSelecionada == 0)
            {
                MessageBox.Show("Clique primeiro em uma aula da lista.", "Atencao",
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

            // Reaproveita a aula do banco para nao perder a ordem nem o curso
            Aula afr_aula = afr_aulaService.AbrirAula(afr_aulaSelecionada, 0);

            if (afr_aula == null)
            {
                MessageBox.Show("Esta aula nao esta mais no banco.", "Atencao",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            afr_aula.afr_Titulo = afr_txtAulaTitulo.Text.Trim();
            afr_aula.afr_DuracaoMin = afr_duracao;
            afr_aula.afr_Conteudo = afr_txtAulaConteudo.Text.Trim();

            string afr_mensagem;

            if (!afr_aulaService.EditarAula(afr_aula, out afr_mensagem))
            {
                MessageBox.Show(afr_mensagem, "Atencao",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CarregarAulas(afr_idSelecionado);
            SelecionarAulaNaLista(afr_aulaSelecionada);

            afr_lblStatus.ForeColor = Color.Green;
            afr_lblStatus.Text = "UPDATE na tabela dbo.Aulas: " + afr_mensagem;
        }

        // =============================================================
        // METODO - LE OS CURSOS E PREENCHE A LISTA
        // =============================================================
        private void AtualizarLista(string afr_termo)
        {
            afr_carregandoLista = true;

            afr_lstCursos.Items.Clear();

            // O tutor enxerga cursos ativos E inativos. O objeto de
            // filtro e o mesmo do DS04, com SomenteAtivos desligado.
            FiltroCurso afr_filtro = new FiltroCurso();

            afr_filtro.afr_Termo = afr_termo.Trim();
            afr_filtro.afr_SomenteAtivos = false;
            afr_filtro.afr_UsuarioId = Sessao.ObterUsuarioId();

            afr_listaAtual = afr_cursoService.LocalizarCurso(afr_filtro);

            foreach (Curso afr_curso in afr_listaAtual)
            {
                ListViewItem afr_item = new ListViewItem(afr_curso.afr_Id.ToString());

                afr_item.SubItems.Add(afr_curso.afr_Titulo);
                afr_item.SubItems.Add(afr_curso.afr_CargaHoraria + "h");
                afr_item.SubItems.Add(afr_curso.afr_TotalAulas.ToString());
                afr_item.SubItems.Add(afr_curso.afr_PercentualMinimo + "%");

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

            afr_lblStatus.ForeColor = Color.DimGray;
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
                afr_lblAulaConteudo.Text = "Conteudo da aula selecionada:";
                return;
            }

            // Passa 0 no usuario: o tutor nao tem progresso a mostrar
            List<Aula> afr_aulas = afr_aulaService.ListarAulas(afr_cursoId, 0);

            foreach (Aula afr_aula in afr_aulas)
            {
                ListViewItem afr_item = new ListViewItem(afr_aula.afr_Id.ToString());

                afr_item.SubItems.Add(afr_aula.afr_Ordem.ToString());
                afr_item.SubItems.Add(afr_aula.afr_Titulo);
                afr_item.SubItems.Add(afr_aula.afr_DuracaoMin + " min");

                // Aula sem conteudo fica em laranja: assim o tutor ve
                // de longe o que ainda falta escrever.
                if (afr_aula.afr_Conteudo.Trim() == "")
                {
                    afr_item.ForeColor = Color.Chocolate;
                }

                afr_lstAulas.Items.Add(afr_item);
            }

            afr_grpAulas.Text = "Aulas de \"" + afr_txtTitulo.Text + "\"  -  " +
                                afr_aulas.Count + " aula(s)  |  " +
                                "as aulas em laranja ainda estao sem conteudo";
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
        // METODO - DEIXA UMA AULA SELECIONADA NA LISTA PELO Id
        // =============================================================
        private void SelecionarAulaNaLista(int afr_aulaId)
        {
            foreach (ListViewItem afr_item in afr_lstAulas.Items)
            {
                if (afr_item.Text == afr_aulaId.ToString())
                {
                    afr_item.Selected = true;
                    afr_item.EnsureVisible();
                    break;
                }
            }
        }

        // =============================================================
        // METODO - CARREGA O CURSO NOS CAMPOS (modo EDICAO)
        // DS09 - exibirDadosParaEdicao(dados, curso)
        // =============================================================
        private void PreencherCampos(Curso afr_curso)
        {
            afr_idSelecionado = afr_curso.afr_Id;
            afr_aulaSelecionada = 0;

            afr_txtTitulo.Text = afr_curso.afr_Titulo;
            afr_txtDescricao.Text = afr_curso.afr_Descricao;
            afr_txtCarga.Text = afr_curso.afr_CargaHoraria.ToString();
            afr_chkAtivoCurso.Checked = afr_curso.afr_Ativo;

            // O NumericUpDown tem minimo 1 e maximo 100: se vier algo
            // fora disso do banco, ajusta antes de atribuir.
            int afr_minimo = afr_curso.afr_PercentualMinimo;

            if (afr_minimo < 1)
            {
                afr_minimo = 1;
            }

            if (afr_minimo > 100)
            {
                afr_minimo = 100;
            }

            afr_numMinimo.Value = afr_minimo;

            afr_lblModo.ForeColor = Color.DarkOrange;
            afr_lblModo.Text = "EDITANDO O Id " + afr_curso.afr_Id;

            afr_txtAulaTitulo.Clear();
            afr_txtAulaDuracao.Text = "45";
            afr_txtAulaConteudo.Clear();

            CarregarAulas(afr_curso.afr_Id);
        }

        // =============================================================
        // METODO - VOLTA PARA O MODO DE CADASTRO
        // =============================================================
        private void EntrarModoNovo()
        {
            afr_idSelecionado = 0;
            afr_aulaSelecionada = 0;

            afr_carregandoLista = true;
            afr_lstCursos.SelectedItems.Clear();
            afr_carregandoLista = false;

            afr_txtTitulo.Clear();
            afr_txtDescricao.Clear();
            afr_txtCarga.Text = "20";
            afr_chkAtivoCurso.Checked = true;
            afr_numMinimo.Value = 100;

            afr_lblModo.ForeColor = Color.Black;
            afr_lblModo.Text = "NOVO CURSO";

            afr_txtAulaTitulo.Clear();
            afr_txtAulaDuracao.Text = "45";
            afr_txtAulaConteudo.Clear();

            CarregarAulas(0);

            afr_txtTitulo.Focus();
        }
    }
}
