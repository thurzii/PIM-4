/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmRecuperarSenha.cs
   ---------------------------------------------------------------------
   CASO DE USO DC02 - Recuperar Senha
   Fluxo modelado no Astah (NG02):
     1. Usuario solicita a recuperacao
     2. Sistema pede o email cadastrado
     3. Sistema confere se o email existe
     4. Usuario redefine a senha
   ===================================================================== */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmRecuperarSenha : Form
    {
        private AutenticacaoService afr_autenticacaoService = new AutenticacaoService();

        // =============================================================
        // CONSTRUTOR - recebe o email ja digitado na tela de login
        // =============================================================
        public FrmRecuperarSenha(string afr_emailInicial)
        {
            InitializeComponent();

            afr_txtEmail.Text = afr_emailInicial;

            if (afr_txtEmail.Text.Trim() == "")
            {
                afr_txtEmail.Focus();
            }
            else
            {
                afr_txtNova.Focus();
            }
        }

        // =============================================================
        // EVENTO - BOTAO REDEFINIR
        // =============================================================
        private void afr_btnRedefinir_Click(object sender, EventArgs e)
        {
            string afr_mensagem;

            bool afr_ok = afr_autenticacaoService.RedefinirSenha(
                              afr_txtEmail.Text,
                              afr_txtNova.Text,
                              afr_txtConfirma.Text,
                              out afr_mensagem);

            if (!afr_ok)
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
                afr_lblStatus.Text = afr_mensagem;

                MessageBox.Show(afr_mensagem, "Recuperar senha",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            afr_lblStatus.ForeColor = Color.Green;
            afr_lblStatus.Text = afr_mensagem;

            MessageBox.Show(afr_mensagem + Environment.NewLine +
                            "Use a nova senha para entrar.", "Recuperar senha",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }

        // =============================================================
        // EVENTO - BOTAO CANCELAR
        // =============================================================
        private void afr_btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
