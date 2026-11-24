using System;
using System.Windows.Forms;
namespace PlataformaCursos {
 public partial class FrmRecuperarSenha : Form {
  public FrmRecuperarSenha(string email) {
   InitializeComponent(); afr_txtEmail.Text=email; afr_txtEmail.MaxLength=200;
   afr_txtCodigo.MaxLength=19; afr_txtNova.MaxLength=128; afr_txtConfirma.MaxLength=128;
   afr_lblEtapa.Text="Codigo pessoal de recuperacao";
   afr_lblInfo.Text="Use o codigo recebido no cadastro. Guarde o novo codigo.";
   afr_lblCodigo.Text="Codigo pessoal:";
   afr_btnEnviar.Visible=false; afr_btnAbrirCaixa.Visible=false; afr_btnConferir.Visible=false;
   afr_txtEmail.Enabled=afr_txtCodigo.Enabled=afr_txtNova.Enabled=afr_txtConfirma.Enabled=afr_btnRedefinir.Enabled=true;
   afr_btnRedefinir.Text="Redefinir senha";
  }
  private void afr_btnEnviar_Click(object sender, EventArgs e) {}
  private void afr_btnAbrirCaixa_Click(object sender, EventArgs e) {}
  private void afr_btnConferir_Click(object sender, EventArgs e) {}
  private void afr_btnCancelar_Click(object sender, EventArgs e) => Close();
  private void afr_btnRedefinir_Click(object sender, EventArgs e) {
   if(afr_txtNova.Text.Length<8 || afr_txtNova.Text!=afr_txtConfirma.Text) { MessageBox.Show("Informe senhas iguais com pelo menos 8 caracteres."); return; }
   var r=new PonteApi().RecuperarComCodigo(afr_txtEmail.Text,afr_txtCodigo.Text,afr_txtNova.Text);
   if(!r.afr_Ok) { MessageBox.Show(r.afr_Mensagem); return; }
   MessageBox.Show("Senha redefinida. Guarde o novo codigo: " + r.CodigoRecuperacao,"Recuperacao"); Close();
  }
 }
}
