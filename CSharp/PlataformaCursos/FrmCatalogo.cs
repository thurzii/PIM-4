/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmCatalogo.cs  (tela do aluno)
   Autor.....: Arthur Fernandes Reis (prefixo afr_)
   ---------------------------------------------------------------------
   CASOS DE USO ATENDIDOS NESTA TELA:
   DC03 / DS03 - Visualizar Cursos ..... lista dos cursos ativos
   DC04 / DS04 - Buscar Cursos ......... termo MAIS filtros
   DC08 / DS08 - Visualizar Curso ...... requisitos e resultado
   DC05 / DS05 - Marcar Aula ........... abre a aula numa tela propria

   O QUE MUDOU POR CAUSA DO MODELO:

   1) DS04 manda "buscarCurso(termo, filtros)", com a palavra filtros
      no plural. Antes a busca so tinha o termo. Agora tem tambem
      carga horaria minima, carga maxima e a opcao de ver somente os
      cursos que o aluno ainda nao concluiu. Tudo isso vai junto num
      objeto FiltroCurso, que e o "filtros" do diagrama.

   2) DS05 manda "abrirAula(aula)" e "exibirConteudo()". So a caixa de
      selecao nao da conta disso, entao a aula agora abre no FrmAula.
      A caixa de selecao continua funcionando como atalho.

   3) DS08 manda "obterRequisitosCurso(curso)" e
      "resultadoConclusao(status, percentual)". Por isso o painel do
      curso mostra o requisito do curso e o status vem do objeto
      ResultadoConclusao, nao de um true/false.
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

            afr_lblAluno.Text = "Usuario: " + afr_aluno.afr_Nome + "  |  " + afr_aluno.afr_Email +
                                "  |  Perfil: " + afr_aluno.afr_Perfil;

            // DS03 - visualizarCursos() / mostrarCursos()
            ExibirTodosOsCursos();
        }

        // =============================================================
        // DS04 - monta o objeto "filtros" que vai junto com o termo
        // =============================================================
        private FiltroCurso MontarFiltro()
        {
            FiltroCurso afr_filtro = new FiltroCurso();

            afr_filtro.afr_Termo = afr_txtBusca.Text.Trim();
            afr_filtro.afr_CargaMinima = Convert.ToInt32(afr_numCargaMin.Value);
            afr_filtro.afr_CargaMaxima = Convert.ToInt32(afr_numCargaMax.Value);
            afr_filtro.afr_SomenteAtivos = true;
            afr_filtro.afr_SomenteNaoConcluidos = afr_chkNaoConcluidos.Checked;
            afr_filtro.afr_UsuarioId = Sessao.ObterUsuarioId();

            return afr_filtro;
        }

        // =============================================================
        // EVENTO - BOTAO BUSCAR
        // DS04 - processarBuscar(termo, filtros) -> buscarCurso(...)
        //        -> cursosEncontrados(listaCursos)
        //        -> exibirResultados(listaCursos) -> mostrarResultados()
        // =============================================================
        private void afr_btnBuscar_Click(object sender, EventArgs e)
        {
            FiltroCurso afr_filtro = MontarFiltro();

            // Confere a faixa antes de ir ao banco
            if (afr_filtro.afr_CargaMaxima > 0
                && afr_filtro.afr_CargaMaxima < afr_filtro.afr_CargaMinima)
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
                afr_lblStatus.Text = "A carga maxima nao pode ser menor que a minima. "
                                   + "Use 0 na maxima para nao ter limite.";
                return;
            }

            // buscarCurso(termo, filtros) -> cursosEncontrados(listaCursos)
            afr_listaAtual = afr_cursoService.BuscarCurso(afr_filtro);

            // exibirResultados(listaCursos)
            PreencherListaDeCursos();

            afr_lblStatus.ForeColor = Color.DimGray;
            afr_lblStatus.Text = "Busca executada. " + afr_listaAtual.Count
                               + " curso(s) encontrado(s).  Filtros: " + afr_filtro.ObterResumo();
        }

        // =============================================================
        // EVENTO - BOTAO VER TODOS
        // DS03 - listarCursos() -> cursosEncontrados() -> exibirCursos()
        // =============================================================
        private void afr_btnVerTodos_Click(object sender, EventArgs e)
        {
            afr_txtBusca.Clear();
            afr_numCargaMin.Value = 0;
            afr_numCargaMax.Value = 0;
            afr_chkNaoConcluidos.Checked = false;

            ExibirTodosOsCursos();
        }

        // =============================================================
        // DS03 - exibirCursos(cursos)
        // =============================================================
        private void ExibirTodosOsCursos()
        {
            afr_listaAtual = afr_cursoService.ExibirCursos();

            PreencherListaDeCursos();

            afr_lblStatus.ForeColor = Color.DimGray;
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
        // Atalho rapido: o caminho completo do DS05 esta no FrmAula.
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
            int afr_alunoId = Sessao.ObterUsuarioId();

            string afr_mensagem;

            // DS05 - atualizarProgresso(aluno, aula)
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

            afr_lblStatus.ForeColor = Color.DimGray;

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
        // EVENTO - DOIS CLIQUES NA AULA
        // DS05 - assistirAula(aula)
        // =============================================================
        private void afr_lstAulas_DoubleClick(object sender, EventArgs e)
        {
            AbrirAulaSelecionada();
        }

        // =============================================================
        // EVENTO - BOTAO ABRIR AULA
        // DS05 - assistirAula(aula)
        // =============================================================
        private void afr_btnAbrirAula_Click(object sender, EventArgs e)
        {
            AbrirAulaSelecionada();
        }

        // =============================================================
        // DS05 - assistirAula(aula): manda o Id da aula para o FrmAula,
        // que faz o abrirAula / exibirConteudo / concluirAula.
        // =============================================================
        private void AbrirAulaSelecionada()
        {
            if (afr_cursoSelecionado == null)
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
                afr_lblStatus.Text = "Escolha primeiro um curso na lista.";
                return;
            }

            if (afr_lstAulas.SelectedItems.Count == 0)
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
                afr_lblStatus.Text = "Escolha uma aula na lista de baixo para abrir.";
                return;
            }

            int afr_aulaId = Convert.ToInt32(afr_lstAulas.SelectedItems[0].Tag);

            FrmAula afr_tela = new FrmAula(afr_aulaId, afr_cursoSelecionado);
            afr_tela.ShowDialog();

            // Se o aluno mexeu no progresso dentro da tela da aula,
            // recarrega a lista aqui para as duas telas combinarem.
            if (afr_tela.afr_HouveMudanca)
            {
                CarregarAulas(afr_cursoSelecionado);
                AtualizarProgresso();

                afr_lblStatus.ForeColor = Color.Green;
                afr_lblStatus.Text = "Progresso atualizado pela tela da aula.";
            }
        }

        // =============================================================
        // METODO - joga a lista de cursos na tela
        // =============================================================
        private void PreencherListaDeCursos()
        {
            afr_lstCursos.Items.Clear();

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
            }
        }

        // =============================================================
        // METODO - MOSTRA O CURSO ESCOLHIDO E SUAS AULAS (DC08)
        // =============================================================
        private void MostrarCurso(Curso afr_curso)
        {
            afr_lblCursoTitulo.Text = afr_curso.afr_Titulo;
            afr_txtCursoDesc.Text = afr_curso.afr_Descricao;

            // DS08 - obterRequisitosCurso(curso) / requisitosCurso(dados)
            afr_lblRequisitos.Text = "Requisito: "
                                   + afr_progressoService.ObterRequisitosCurso(afr_curso);

            afr_grpAulas.Text = "Aulas de \"" + afr_curso.afr_Titulo +
                                "\" - abra a aula para ver o conteudo, ou marque a caixa para concluir";

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

            int afr_alunoId = Sessao.ObterUsuarioId();

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
        // METODO - RECALCULA A BARRA DE PROGRESSO
        // DS08 - verificarConclusao(aluno, curso)
        //        -> resultadoConclusao(status, percentual)
        //        -> exibirResultado(status, percentual)
        // =============================================================
        private void AtualizarProgresso()
        {
            if (afr_cursoSelecionado == null)
            {
                return;
            }

            int afr_alunoId = Sessao.ObterUsuarioId();

            // Um unico objeto traz concluidas, total, percentual,
            // minimo exigido e se passou. E o "resultadoConclusao".
            ResultadoConclusao afr_resultado =
                afr_progressoService.VerificarConclusao(afr_alunoId, afr_cursoSelecionado);

            // A barra nunca recebe valor fora de 0 a 100
            int afr_valor = afr_resultado.afr_Percentual;

            if (afr_valor < 0)
            {
                afr_valor = 0;
            }

            if (afr_valor > 100)
            {
                afr_valor = 100;
            }

            afr_prgProgresso.Value = afr_valor;

            afr_lblProgressoTexto.Text = "Seu progresso: " + afr_resultado.afr_Percentual + "%  (" +
                                         afr_resultado.afr_Concluidas + " de " +
                                         afr_resultado.afr_Total + " aulas)";

            // exibirResultado(status, percentual)
            if (afr_resultado.afr_Total == 0)
            {
                afr_lblConclusao.ForeColor = Color.DimGray;
                afr_lblConclusao.Text = "Este curso ainda nao tem aulas cadastradas.";
            }
            else if (afr_resultado.afr_Aprovado)
            {
                afr_lblConclusao.ForeColor = Color.Green;
                afr_lblConclusao.Text = "CONCLUIDO: voce bateu o minimo de "
                                      + afr_resultado.afr_PercentualMinimo + "% deste curso.";
            }
            else if (afr_resultado.afr_Concluidas > 0)
            {
                afr_lblConclusao.ForeColor = Color.DarkOrange;
                afr_lblConclusao.Text = "EM ANDAMENTO: faltam "
                                      + (afr_resultado.afr_Total - afr_resultado.afr_Concluidas)
                                      + " aula(s) para o minimo de "
                                      + afr_resultado.afr_PercentualMinimo + "%.";
            }
            else
            {
                afr_lblConclusao.ForeColor = Color.DimGray;
                afr_lblConclusao.Text = "NAO INICIADO: voce ainda nao concluiu nenhuma aula.";
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
            afr_lblRequisitos.Text = "";
            afr_prgProgresso.Value = 0;
            afr_lblProgressoTexto.Text = "Seu progresso: 0%";

            afr_lblConclusao.ForeColor = Color.DimGray;
            afr_lblConclusao.Text = "";

            afr_grpAulas.Text = "Aulas - abra a aula para ver o conteudo, ou marque a caixa para concluir";
        }
    }
}
