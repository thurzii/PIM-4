/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmRecuperarSenha.cs
   Autor.....: Arthur Fernandes Reis (prefixo afr_)
   ---------------------------------------------------------------------
   CASO DE USO DC02 - Recuperar Senha
   DIAGRAMA DE SEQUENCIA DS02

   O modelo do Astah tem QUATRO linhas de vida neste diagrama:
   Aluno, TelaRecuperacao, Sistema e Email. A linha "Email" e o
   motivo da tela ter mudado: antes a pessoa trocava a senha direto,
   sem codigo nenhum. A sequencia do modelo e esta:

       1) informarEmail(email)        -> etapa 1
       2) solicitarRecuperacao(email) -> etapa 1
       3) enviarCodigo(email)         -> vai para a linha Email
       4) codigoEnviado()             -> volta para o Sistema
       5) exibirMensagem()            -> a tela avisa o aluno
       6) recuperacaoConcluida()      -> etapa 3

   Como o trabalho e de faculdade e nao tem servidor de email, o
   "Email" do modelo e um arquivo de texto (dados/emails.txt). O
   botao "Ver o codigo" abre esse arquivo, que faz o papel da caixa
   de entrada. O resto do fluxo e igual ao modelo: sem codigo valido
   a senha nao troca.
   ===================================================================== */

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmRecuperarSenha : Form
    {
        private AutenticacaoService afr_autenticacaoService = new AutenticacaoService();

        // Em qual etapa a tela esta: 1 = email, 2 = codigo, 3 = nova senha
        private int afr_etapaAtual = 1;

        // =============================================================
        // CONSTRUTOR - recebe o email ja digitado na tela de login
        // =============================================================
        public FrmRecuperarSenha(string afr_emailInicial)
        {
            InitializeComponent();

            afr_txtEmail.Text = afr_emailInicial;

            IrParaEtapa(1);
        }

        // =============================================================
        // Controla o que fica habilitado em cada etapa.
        // Assim o aluno nao pula o codigo, que e o que o modelo pede.
        // =============================================================
        private void IrParaEtapa(int afr_etapa)
        {
            afr_etapaAtual = afr_etapa;

            // Etapa 1 - email
            bool afr_e1 = (afr_etapa == 1);
            afr_txtEmail.Enabled = afr_e1;
            afr_btnEnviar.Enabled = afr_e1;

            // Etapa 2 - codigo
            bool afr_e2 = (afr_etapa == 2);
            afr_txtCodigo.Enabled = afr_e2;
            afr_btnConferir.Enabled = afr_e2;

            // Etapa 3 - nova senha
            bool afr_e3 = (afr_etapa == 3);
            afr_txtNova.Enabled = afr_e3;
            afr_txtConfirma.Enabled = afr_e3;
            afr_btnRedefinir.Enabled = afr_e3;

            // O botao padrao do Enter acompanha a etapa
            if (afr_e1)
            {
                this.AcceptButton = afr_btnEnviar;

                afr_lblEtapa.Text = "Etapa 1 de 3 - informar o email";
                afr_lblInfo.Text = "Informe o email cadastrado. "
                                 + "O sistema envia um codigo de 6 digitos.";
                afr_txtEmail.Focus();
            }
            else if (afr_e2)
            {
                this.AcceptButton = afr_btnConferir;

                afr_lblEtapa.Text = "Etapa 2 de 3 - conferir o codigo";
                afr_lblInfo.Text = "Digite o codigo que foi enviado. "
                                 + "Ele vale por " + AutenticacaoService.MINUTOS_VALIDADE_CODIGO
                                 + " minutos e serve uma vez so.";
                afr_txtCodigo.Focus();
            }
            else
            {
                this.AcceptButton = afr_btnRedefinir;

                afr_lblEtapa.Text = "Etapa 3 de 3 - escolher a nova senha";
                afr_lblInfo.Text = "Codigo confirmado. "
                                 + "Escolha uma nova senha de pelo menos 6 caracteres.";
                afr_txtNova.Focus();
            }
        }

        // =============================================================
        // ETAPA 1
        // DS02 - informarEmail(email) -> solicitarRecuperacao(email)
        //        -> enviarCodigo(email) -> codigoEnviado()
        //        -> exibirMensagem()
        // =============================================================
        private void afr_btnEnviar_Click(object sender, EventArgs e)
        {
            string afr_codigoGerado;
            string afr_mensagem;

            bool afr_ok = afr_autenticacaoService.SolicitarRecuperacao(
                              afr_txtEmail.Text,
                              out afr_codigoGerado,
                              out afr_mensagem);

            if (!afr_ok)
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
                afr_lblStatus.Text = afr_mensagem;
                return;
            }

            // exibirMensagem(): o sistema avisa que o codigo foi enviado
            afr_lblStatus.ForeColor = Color.Green;
            afr_lblStatus.Text = afr_mensagem;

            IrParaEtapa(2);
        }

        // =============================================================
        // Abre o arquivo que faz o papel da caixa de entrada.
        // Sem servidor de email, e assim que o aluno le o codigo.
        // =============================================================
        private void afr_btnAbrirCaixa_Click(object sender, EventArgs e)
        {
            // Na etapa 1 ainda nao existe codigo nenhum para ler
            if (afr_etapaAtual == 1)
            {
                afr_lblStatus.ForeColor = Color.Chocolate;
                afr_lblStatus.Text = "Clique primeiro em Enviar codigo. "
                                   + "So depois a caixa de entrada tem o codigo.";
                return;
            }

            string afr_caminho = afr_autenticacaoService.ObterCaminhoCaixaDeEntrada();

            try
            {
                ProcessStartInfo afr_abrir = new ProcessStartInfo(afr_caminho);
                afr_abrir.UseShellExecute = true;

                Process.Start(afr_abrir);
            }
            catch (Exception afr_erro)
            {
                // Se o arquivo ainda nao existe, mostra o caminho para
                // a pessoa achar na mao.
                afr_lblStatus.ForeColor = Color.Chocolate;
                afr_lblStatus.Text = "Nao deu para abrir a caixa de entrada: "
                                   + afr_erro.Message
                                   + "  Caminho: " + afr_caminho;
            }
        }

        // =============================================================
        // ETAPA 2
        // DS02 - conferir o codigo antes de liberar a troca de senha
        // =============================================================
        private void afr_btnConferir_Click(object sender, EventArgs e)
        {
            string afr_mensagem;

            bool afr_ok = afr_autenticacaoService.ConferirCodigo(
                              afr_txtEmail.Text,
                              afr_txtCodigo.Text,
                              out afr_mensagem);

            if (!afr_ok)
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
                afr_lblStatus.Text = afr_mensagem;
                return;
            }

            afr_lblStatus.ForeColor = Color.Green;
            afr_lblStatus.Text = afr_mensagem;

            IrParaEtapa(3);
        }

        // =============================================================
        // ETAPA 3
        // DS02 - recuperacaoConcluida()
        // =============================================================
        private void afr_btnRedefinir_Click(object sender, EventArgs e)
        {
            string afr_mensagem;

            // O codigo vai de novo para o servico: ele confere outra vez
            // e so depois marca o codigo como usado.
            bool afr_ok = afr_autenticacaoService.RedefinirSenha(
                              afr_txtEmail.Text,
                              afr_txtCodigo.Text,
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
