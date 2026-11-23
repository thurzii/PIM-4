/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmLogin.cs  (eventos da tela de login)
   ---------------------------------------------------------------------
   CASOS DE USO ATENDIDOS NESTA TELA:
   DC01 - Fazer Login          DC02 - Recuperar Senha (abre outra tela)

   A tela nao conhece banco de dados: ela valida o que foi digitado e
   entrega para a classe AutenticacaoService decidir.
   ===================================================================== */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmLogin : Form
    {
        private BancoService afr_bancoService = new BancoService();

        // VERSAO 4 - no modo API a conexao testada e descrita e a da API
        private PonteApi afr_ponte = new PonteApi();
        private AutenticacaoService afr_autenticacaoService = new AutenticacaoService();

        // =============================================================
        // CONSTRUTOR - prepara o banco na primeira execucao
        // =============================================================
        public FrmLogin()
        {
            InitializeComponent();

            string afr_mensagem;

            // VERSAO 4 - COM A API LIGADA, O PROGRAMA NAO TOCA EM BANCO
            // NENHUM. PrepararBanco cria tabelas, roda ALTER TABLE/UPDATE
            // de migracao e cadastra usuarios de teste: no banco do Azure,
            // que o aplicativo ja usa, isso estragaria o esquema dele. No
            // modo API o que se testa aqui e se a API responde.
            bool afr_pronto = afr_ponte.Ativa()
                ? afr_ponte.AguardarApi(15, out afr_mensagem)
                : afr_bancoService.PrepararBanco(out afr_mensagem);

            if (afr_pronto)
            {
                afr_lblStatus.ForeColor = Color.DimGray;
                afr_lblStatus.Text = afr_mensagem + "  " +
                                     (afr_ponte.Ativa() ? afr_ponte.ObterDescricao() : afr_bancoService.ObterDescricao());

                // A tela abre vazia: como no aplicativo, quem entra ja sabe
                // a propria conta. Nenhuma conta fica sugerida na tela.
                afr_txtEmail.Focus();
            }
            else
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
                afr_lblStatus.Text = afr_mensagem;

                MessageBox.Show(afr_mensagem, "Banco de dados",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // EVENTO - BOTAO ENTRAR (DC01)
        // =============================================================
        private void afr_btnEntrar_Click(object sender, EventArgs e)
        {
            string afr_mensagem;

            Usuario afr_usuario = afr_autenticacaoService.ValidarLogin(
                                      afr_txtEmail.Text,
                                      afr_txtSenha.Text,
                                      out afr_mensagem);

            // Login recusado: mostra o motivo e para por aqui
            if (afr_usuario == null)
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
                afr_lblStatus.Text = afr_mensagem;

                MessageBox.Show(afr_mensagem, "Login",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

                afr_txtSenha.Clear();
                afr_txtSenha.Focus();
                return;
            }

            // Login aprovado: esconde esta tela e abre a principal
            this.Hide();

            FrmPrincipal afr_principal = new FrmPrincipal();
            afr_principal.ShowDialog();

            // Quando a tela principal fecha (logout), volta para o login limpo
            afr_txtSenha.Clear();
            afr_lblStatus.ForeColor = Color.DimGray;
            afr_lblStatus.Text = "Sessao encerrada. Entre novamente para continuar.";

            this.Show();
            afr_txtEmail.Focus();
        }

        // =============================================================
        // EVENTO - CAIXA "MOSTRAR SENHA"
        // =============================================================
        private void afr_chkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            if (afr_chkMostrarSenha.Checked)
            {
                afr_txtSenha.PasswordChar = '\0';   // mostra o texto
            }
            else
            {
                afr_txtSenha.PasswordChar = '*';    // volta a esconder
            }
        }

        // =============================================================
        // EVENTO - LINK "ESQUECI MINHA SENHA" (DC02)
        // =============================================================
        private void afr_lnkEsqueci_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            FrmRecuperarSenha afr_tela = new FrmRecuperarSenha(afr_txtEmail.Text);

            afr_tela.ShowDialog();

            afr_txtSenha.Clear();
            afr_txtSenha.Focus();
        }
    }
}
