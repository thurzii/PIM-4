/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmProgressoAlunos.cs
   Autor.....: Arthur Fernandes Reis (prefixo afr_)
   ---------------------------------------------------------------------
   TELA DE PROGRESSO DOS ALUNOS (TelaProgresso do modelo)

   Esta tela atende o diagrama DS07. No Astah esse diagrama esta
   salvo com o nome "Criar Curso", mas as mensagens dentro dele sao
   todas de progresso e a linha de vida do Tutor aparece nele. A
   sequencia e esta:

       1) acessarProgresso()              -> o tutor abre a tela
       2) solicitarProgresso(aluno)       -> escolhe o aluno
       3) buscarProgresso(aluno)          -> servico vai ao banco
       4) aulasConcluidas()               -> conta as aulas feitas
       5) progresso(dados)                -> devolve os dados
       6) exibirProgresso(dados)          -> a tela monta a lista
       7) mostrarProgresso()              -> o tutor ve o resultado

   Quem pode abrir: tutor e administrador (PodeVerProgressoDeOutros).
   O aluno comum nao entra aqui: ele ve o proprio progresso no
   catalogo de cursos.
   ===================================================================== */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmProgressoAlunos : Form
    {
        // Servicos usados por esta tela
        private UsuarioService afr_usuarioService = new UsuarioService();
        private ProgressoService afr_progressoService = new ProgressoService();

        // Lista de alunos que esta no combo.
        // Guardo a lista separada para pegar o Id pelo indice do combo,
        // sem depender do texto que aparece na tela.
        private List<Usuario> afr_listaAlunos = new List<Usuario>();

        public FrmProgressoAlunos()
        {
            InitializeComponent();
        }

        // -------------------------------------------------------------
        // LOAD
        // DS07 - acessarProgresso()
        // A checagem de permissao fica aqui, e nao no construtor,
        // porque Close() no construtor descarta o formulario e o
        // ShowDialog() depois disso da erro.
        // -------------------------------------------------------------
        private void FrmProgressoAlunos_Load(object sender, EventArgs e)
        {
            if (!Sessao.EstaAberta())
            {
                MessageBox.Show("Sessao encerrada. Entre novamente.",
                                "Progresso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            if (!Sessao.PodeVerProgressoDeOutros())
            {
                MessageBox.Show("Somente tutor ou administrador acompanha o progresso de outros alunos.",
                                "Progresso",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            afr_lblInstrucao.Text = "Perfil " + Sessao.ObterUsuario().afr_Perfil
                                  + ". Escolha um aluno para acompanhar o andamento dele em cada curso.";

            CarregarAlunos("");
        }

        // -------------------------------------------------------------
        // Preenche o combo com os alunos cadastrados
        // -------------------------------------------------------------
        private void CarregarAlunos(string afr_termo)
        {
            afr_cmbAlunos.Items.Clear();
            afr_listaAlunos = afr_usuarioService.ListarAlunos(afr_termo);

            for (int afr_i = 0; afr_i < afr_listaAlunos.Count; afr_i++)
            {
                Usuario afr_aluno = afr_listaAlunos[afr_i];

                afr_cmbAlunos.Items.Add(afr_aluno.afr_Nome + "  (" + afr_aluno.afr_Email + ")");
            }

            afr_lstProgresso.Items.Clear();
            afr_lblDados.Text = "";

            if (afr_listaAlunos.Count == 0)
            {
                afr_lblResumo.ForeColor = System.Drawing.Color.Chocolate;
                afr_lblResumo.Text = "Nenhum aluno encontrado para esta busca.";
                return;
            }

            afr_cmbAlunos.SelectedIndex = 0;

            afr_lblResumo.ForeColor = System.Drawing.Color.DimGray;
            afr_lblResumo.Text = afr_listaAlunos.Count + " aluno(s) na lista. "
                               + "Clique em Ver progresso.";
        }

        // -------------------------------------------------------------
        // Buscar aluno pelo nome ou pelo email
        // -------------------------------------------------------------
        private void afr_btnBuscar_Click(object sender, EventArgs e)
        {
            CarregarAlunos(afr_txtBusca.Text.Trim());
        }

        // -------------------------------------------------------------
        // DS07 - solicitarProgresso(aluno) -> buscarProgresso(aluno)
        //        -> progresso(dados) -> exibirProgresso(dados)
        // -------------------------------------------------------------
        private void afr_btnVer_Click(object sender, EventArgs e)
        {
            if (afr_cmbAlunos.SelectedIndex < 0
                || afr_cmbAlunos.SelectedIndex >= afr_listaAlunos.Count)
            {
                afr_lblResumo.ForeColor = System.Drawing.Color.Firebrick;
                afr_lblResumo.Text = "Escolha um aluno na lista.";
                return;
            }

            Usuario afr_aluno = afr_listaAlunos[afr_cmbAlunos.SelectedIndex];

            // elegivelParaVisualizacao(true): o servico confere de novo
            // a permissao, nao fico confiando so no botao da tela.
            string afr_mensagem;
            bool afr_elegivel = afr_progressoService.ElegivelParaVisualizacao(
                                    afr_aluno.afr_Id, out afr_mensagem);

            if (!afr_elegivel)
            {
                afr_lblResumo.ForeColor = System.Drawing.Color.Firebrick;
                afr_lblResumo.Text = afr_mensagem;
                afr_lstProgresso.Items.Clear();
                return;
            }

            // buscarProgresso(aluno) -> progresso(dados)
            List<ProgressoCurso> afr_dados = afr_progressoService.BuscarProgresso(afr_aluno.afr_Id);

            // exibirProgresso(dados)
            ExibirProgresso(afr_aluno, afr_dados);
        }

        // -------------------------------------------------------------
        // DS07 - exibirProgresso(dados) / mostrarProgresso()
        // -------------------------------------------------------------
        private void ExibirProgresso(Usuario afr_aluno, List<ProgressoCurso> afr_dados)
        {
            // Cabecalho com os dados de quem esta sendo consultado
            afr_lblDados.ForeColor = System.Drawing.Color.Black;
            afr_lblDados.Text = "Aluno: " + afr_aluno.afr_Nome
                              + "   |   Email: " + afr_aluno.afr_Email
                              + "   |   Cadastro: "
                              + afr_aluno.afr_DataCadastro.ToString("dd/MM/yyyy");

            afr_lstProgresso.Items.Clear();

            int afr_aprovados = 0;
            int afr_somaPercentual = 0;

            for (int afr_i = 0; afr_i < afr_dados.Count; afr_i++)
            {
                ProgressoCurso afr_linha = afr_dados[afr_i];
                ResultadoConclusao afr_r = afr_linha.afr_Resultado;

                ListViewItem afr_item = new ListViewItem(afr_linha.afr_Curso.afr_Titulo);

                afr_item.SubItems.Add(afr_r.afr_Concluidas + " de " + afr_r.afr_Total);
                afr_item.SubItems.Add(afr_r.afr_Percentual + "%");
                afr_item.SubItems.Add(afr_r.afr_PercentualMinimo + "%");
                afr_item.SubItems.Add(afr_r.ObterStatus());

                // Pinta a linha de verde quando o curso foi aprovado
                if (afr_r.afr_Aprovado)
                {
                    afr_item.ForeColor = System.Drawing.Color.SeaGreen;
                    afr_aprovados = afr_aprovados + 1;
                }

                afr_somaPercentual = afr_somaPercentual + afr_r.afr_Percentual;

                afr_lstProgresso.Items.Add(afr_item);
            }

            // mostrarProgresso(): resumo geral embaixo da lista
            if (afr_dados.Count == 0)
            {
                afr_lblResumo.ForeColor = System.Drawing.Color.Chocolate;
                afr_lblResumo.Text = "Nao existe nenhum curso ativo cadastrado no sistema.";
                return;
            }

            int afr_media = afr_somaPercentual / afr_dados.Count;

            afr_lblResumo.ForeColor = System.Drawing.Color.DimGray;
            afr_lblResumo.Text = "Cursos acompanhados: " + afr_dados.Count
                               + "   |   Concluidos: " + afr_aprovados
                               + "   |   Media geral de conclusao: " + afr_media + "%";
        }

        private void afr_btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
