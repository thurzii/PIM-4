/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmRecuperarSenha.cs
   ---------------------------------------------------------------------
   CASO DE USO DC02 - Recuperar Senha (DS02)

   A pessoa informa o email, o CODIGO PESSOAL de recuperacao (aquele
   que aparece quando a conta e criada ou no primeiro acesso) e a nova
   senha. A API confere o codigo, troca a senha e devolve um codigo
   novo, que precisa ser guardado para a proxima vez.
   ===================================================================== */

using System;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmRecuperarSenha : Form
    {
        // Recebe o email que ja estava digitado na tela de login
        public FrmRecuperarSenha(string email)
        {
            InitializeComponent();
            afr_txtEmail.Text = email;
            this.AcceptButton = afr_btnRedefinir;
        }

        private void afr_btnCancelar_Click(object sender, EventArgs e) => Close();

        private void afr_btnRedefinir_Click(object sender, EventArgs e)
        {
            if (afr_txtNova.Text.Length < 8 || afr_txtNova.Text != afr_txtConfirma.Text)
            {
                MessageBox.Show("Informe senhas iguais com pelo menos 8 caracteres.");
                return;
            }

            var r = new PonteApi().RecuperarComCodigo(afr_txtEmail.Text, afr_txtCodigo.Text, afr_txtNova.Text);
            if (!r.afr_Ok) { MessageBox.Show(r.afr_Mensagem); return; }

            MessageBox.Show("Senha redefinida. Guarde o novo código: " + r.CodigoRecuperacao, "Recuperação");
            Close();
        }
    }
}
