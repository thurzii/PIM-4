/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmLogin.cs  (eventos da tela de login)
   ---------------------------------------------------------------------
   CASOS DE USO ATENDIDOS NESTA TELA:
   DC01 - Fazer Login          DC02 - Recuperar Senha (abre outra tela)

   A tela nao conhece a API diretamente: ela valida o que foi digitado e
   entrega para a classe AutenticacaoService decidir.
   ===================================================================== */

using System;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmLogin : Form
    {

        // VERSAO 4 - no modo API a conexao testada e descrita e a da API
        private PonteApi afr_ponte = new PonteApi();
        private AutenticacaoService afr_autenticacaoService = new AutenticacaoService();

        // =============================================================
        // CONSTRUTOR - confere se a API responde
        // =============================================================
        public FrmLogin()
        {
            InitializeComponent();
            {
                var cadastro = new LinkLabel { Text = "Criar conta", AutoSize = true, Location = new Point(270, 285) };
                cadastro.LinkClicked += (sender, args) => Cadastrar();
                Controls.Add(cadastro);
            }

            string afr_mensagem;

            // O desktop nao tem banco proprio: aqui so se confere se a API
            // do grupo responde (esperando alguns segundos, caso ela esteja
            // acordando - o servidor gratuito do Render dorme quando parado)
            bool afr_pronto = afr_ponte.AguardarApi(15, out afr_mensagem);

            if (afr_pronto)
            {
                afr_lblStatus.ForeColor = Color.DimGray;
                afr_lblStatus.Text = afr_mensagem + "  " +
                                     afr_ponte.ObterDescricao();

                // A tela abre vazia: como no aplicativo, quem entra ja sabe
                // a propria conta. Nenhuma conta fica sugerida na tela.
                afr_txtEmail.Focus();
            }
            else
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
                afr_lblStatus.Text = afr_mensagem;

                MessageBox.Show(afr_mensagem, "Conexão",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // EVENTO - BOTAO ENTRAR (DC01)
        // =============================================================
        private void afr_btnEntrar_Click(object sender, EventArgs e)
        {
            string afr_mensagem;

            {
                var cliente = ConfiguracaoApi.ObterCliente();
                var resposta = cliente.Entrar(afr_txtEmail.Text, afr_txtSenha.Text);
                if (resposta.afr_Ok)
                {
                    using var doc = JsonDocument.Parse(resposta.afr_CorpoJson);
                    if (doc.RootElement.TryGetProperty("tokenPrimeiroAcesso", out var token))
                    {
                        var campos = Solicitar("Primeiro acesso", new[] { "Nova senha", "Confirmar senha" }, new[] { true, true });
                        if (campos == null) return;
                        if (campos[0] != campos[1])
                        {
                            MessageBox.Show("As senhas não conferem.");
                            return;
                        }
                        var troca = cliente.PrimeiroAcesso(token.GetString(), afr_txtSenha.Text, campos[0]);
                        if (!troca.afr_Ok)
                        {
                            MessageBox.Show(troca.afr_Mensagem);
                            return;
                        }
                        MessageBox.Show("Guarde seu código de recuperação: " + troca.CodigoRecuperacao);
                        afr_txtSenha.Text = campos[0];
                    }
                } else { MessageBox.Show(resposta.afr_Mensagem); return; }
            }
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
            afr_lblStatus.Text = "Sessão encerrada. Entre novamente para continuar.";

            this.Show();
            afr_txtEmail.Focus();
        }

        private void Cadastrar()
        {
            var campos = Solicitar("Criar conta de aluno", new[] { "Nome", "Email", "Senha", "Confirmar senha" }, new[] { false, false, true, true });
            if (campos == null) return;
            var resposta = ConfiguracaoApi.ObterCliente().CadastrarAluno(campos[0], campos[1], campos[2], campos[3]);
            MessageBox.Show(resposta.afr_Ok ? "Conta criada. Guarde seu código de recuperação: " + resposta.CodigoRecuperacao : resposta.afr_Mensagem);
            if (resposta.afr_Ok) afr_txtEmail.Text = campos[1];
        }

        private string[] Solicitar(string titulo, string[] rotulos, bool[] secretos)
        {
            using var janela = new Form { Text = titulo, ClientSize = new Size(380, rotulos.Length * 65 + 55), StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
            var entradas = new TextBox[rotulos.Length];
            for (int i = 0; i < rotulos.Length; i++)
            {
                janela.Controls.Add(new Label { Text = rotulos[i], Location = new Point(20, 10 + i*65), AutoSize = true });
                entradas[i] = new TextBox { Location = new Point(20, 32 + i*65), Width = 340, UseSystemPasswordChar = secretos[i] };
                janela.Controls.Add(entradas[i]);
            }
            var ok = new Button { Text = "Confirmar", DialogResult = DialogResult.OK, Location = new Point(180, rotulos.Length*65 + 10), Width = 90 };
            var cancelar = new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, Location = new Point(275, rotulos.Length*65 + 10), Width = 85 };
            janela.Controls.Add(ok); janela.Controls.Add(cancelar); janela.AcceptButton = ok; janela.CancelButton = cancelar;
            if (janela.ShowDialog(this) != DialogResult.OK) return null;
            var valores = new string[entradas.Length];
            for (int i = 0; i < entradas.Length; i++) valores[i] = entradas[i].Text;
            return valores;
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
