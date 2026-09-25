/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmPrincipal.cs  (menu principal)
   ---------------------------------------------------------------------
   Esta tela mostra opcoes DIFERENTES conforme quem entrou:
     ADMIN -> ve o grupo Administracao (cursos e usuarios)
     ALUNO -> ve apenas a Area do Aluno

   CASO DE USO DC06 - Fazer Logout (botao Sair)
   ===================================================================== */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmPrincipal : Form
    {
        private BancoService afr_bancoService = new BancoService();
        private AutenticacaoService afr_autenticacaoService = new AutenticacaoService();

        // =============================================================
        // CONSTRUTOR - monta o menu conforme o perfil do usuario
        // =============================================================
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        // =============================================================
        // EVENTO LOAD - roda quando a janela ja existe na tela.
        // A checagem de sessao fica aqui, e nao no construtor, porque
        // fechar um formulario que ainda nao apareceu causa erro.
        // =============================================================
        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            AplicarPerfil();
        }

        // =============================================================
        // METODO - mostra ou esconde as opcoes conforme o perfil
        // =============================================================
        private void AplicarPerfil()
        {
            // Protecao: ninguem chega aqui sem estar logado
            if (!Sessao.EstaAberta())
            {
                MessageBox.Show("Sessao nao encontrada. Faca o login novamente.", "Acesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            Usuario afr_usuario = Sessao.ObterUsuario();

            afr_lblBoasVindas.Text = "Ola, " + afr_usuario.ObterPrimeiroNome() +
                                     ".  |  " + afr_usuario.ObterResumo();

            // O grupo de administracao so aparece para o ADMIN
            afr_grpAdmin.Visible = Sessao.EhAdministrador();

            if (Sessao.EhAdministrador())
            {
                afr_lblStatus.Text = "Perfil administrador: acesso ao catalogo e a administracao.  " +
                                     afr_bancoService.ObterDescricao();
            }
            else
            {
                afr_lblStatus.Text = "Perfil aluno: acesso ao catalogo de cursos.  " +
                                     afr_bancoService.ObterDescricao();
            }
        }

        // =============================================================
        // EVENTO - CATALOGO DE CURSOS (DC03, DC04, DC05, DC08)
        // =============================================================
        private void afr_btnCatalogo_Click(object sender, EventArgs e)
        {
            FrmCatalogo afr_tela = new FrmCatalogo();
            afr_tela.ShowDialog();
        }

        // =============================================================
        // EVENTO - GERENCIAR CURSOS (DC07 a DC10)
        // =============================================================
        private void afr_btnCursos_Click(object sender, EventArgs e)
        {
            FrmCursos afr_tela = new FrmCursos();
            afr_tela.ShowDialog();
        }

        // =============================================================
        // EVENTO - GERENCIAR USUARIOS (DC11 a DC14)
        // =============================================================
        private void afr_btnUsuarios_Click(object sender, EventArgs e)
        {
            FrmUsuarios afr_tela = new FrmUsuarios();
            afr_tela.ShowDialog();
        }

        // =============================================================
        // EVENTO - TESTAR CONEXAO
        // =============================================================
        private void afr_btnTestar_Click(object sender, EventArgs e)
        {
            string afr_mensagem;

            bool afr_ok = afr_bancoService.TestarConexao(out afr_mensagem);

            if (afr_ok)
            {
                afr_lblStatus.ForeColor = Color.Green;
            }
            else
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
            }

            afr_lblStatus.Text = afr_mensagem;

            MessageBox.Show(afr_bancoService.ObterDescricao() + Environment.NewLine + afr_mensagem,
                            "Conexao", MessageBoxButtons.OK,
                            afr_ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        // =============================================================
        // EVENTO - LOGOUT (DC06)
        // =============================================================
        private void afr_btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult afr_resposta = MessageBox.Show("Deseja encerrar a sessao?", "Logout",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question);

            if (afr_resposta == DialogResult.Yes)
            {
                afr_autenticacaoService.EncerrarSessao();
                this.Close();
            }
        }
    }
}
