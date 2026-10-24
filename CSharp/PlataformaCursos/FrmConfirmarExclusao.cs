/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmConfirmarExclusao.cs
   Autor.....: Arthur Fernandes Reis (prefixo afr_)
   ---------------------------------------------------------------------
   TELA DE CONFIRMACAO DE EXCLUSAO

   Esta tela atende a parte final dos diagramas DS10 (Excluir Curso) e
   DS14 (Excluir Usuario). Nos dois diagramas a exclusao NAO acontece
   com um clique so. A ordem das mensagens no modelo e esta:

       1) solicitarExclusao(alvo)                 -> tela de gerenciamento
       2) verificarElegibilidadeExclusao(alvo)    -> servico
       3) elegivelParaExclusao(true, dadosDetalhados)
       4) solicitarConfirmacaoExclusao(dadosDetalhados)  <-- ESTA TELA
       5) confirmarExclusaoFinal(alvo)                   <-- ESTA TELA
       6) confirmarIdentidadeEAcao(ator, alvo)           <-- ESTA TELA
       7) removerDefinitivamente(alvo)            -> servico
       8) remocaoSucesso() / exibirMensagemSucesso()

   Por isso a tela mostra os "dadosDetalhados" (o que exatamente vai
   ser apagado) e ainda pede a senha de quem esta logado, que e o
   "confirmarIdentidadeEAcao" do modelo.

   A mesma tela serve para curso e para usuario: quem chama passa o
   titulo e o objeto de elegibilidade ja pronto.
   ===================================================================== */

using System;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmConfirmarExclusao : Form
    {
        // Servico usado so para conferir a senha do operador
        private AutenticacaoService afr_autenticacaoService = new AutenticacaoService();

        // Dados recebidos de quem abriu a tela
        private string afr_tituloOperacao = "";
        private ElegibilidadeExclusao afr_elegibilidade = new ElegibilidadeExclusao();

        // Resultado que a tela devolve para quem a chamou.
        // true = usuario digitou a senha certa e confirmou a exclusao.
        public bool afr_Confirmado { get; private set; }

        // -------------------------------------------------------------
        // CONSTRUTOR
        // Recebe o texto do titulo e o resultado da verificacao de
        // elegibilidade (passo 3 do diagrama).
        // -------------------------------------------------------------
        public FrmConfirmarExclusao(string afr_titulo, ElegibilidadeExclusao afr_dados)
        {
            InitializeComponent();

            this.afr_tituloOperacao = afr_titulo ?? "Confirmar exclusao";
            this.afr_elegibilidade = afr_dados ?? new ElegibilidadeExclusao();
            this.afr_Confirmado = false;

            // Carrega a tela com os dados recebidos.
            // Isso e o "solicitarConfirmacaoExclusao(dadosDetalhados)".
            CarregarInterface();
        }

        // -------------------------------------------------------------
        // DS10 e DS14 - carregarInterface()
        // Mostra na tela o motivo e a lista detalhada do que sera apagado
        // -------------------------------------------------------------
        private void CarregarInterface()
        {
            this.Text = afr_tituloOperacao;
            afr_lblTitulo.Text = afr_tituloOperacao;

            afr_lblMotivo.Text = afr_elegibilidade.afr_Motivo;
            afr_txtDetalhes.Text = afr_elegibilidade.ObterDetalhesTexto();

            // Se o servico disse que NAO esta elegivel, a tela abre
            // somente no modo aviso: nao da para confirmar nada.
            if (!afr_elegibilidade.afr_Elegivel)
            {
                afr_lblDetalhesTitulo.Text = "Motivo do bloqueio:";
                afr_lblInstrucao.Text = "Esta exclusao nao pode ser feita agora. "
                                      + "Resolva o que esta listado acima e tente de novo.";

                afr_lblSenha.Visible = false;
                afr_txtSenha.Visible = false;

                afr_btnConfirmar.Enabled = false;
                afr_btnConfirmar.Text = "Nao permitido";

                afr_btnCancelar.Text = "Fechar";
                afr_btnCancelar.Focus();
                return;
            }

            // Caminho normal: elegivel. Pede a senha do operador.
            afr_lblDetalhesTitulo.Text = "O que sera apagado:";
            afr_txtSenha.Focus();
        }

        // -------------------------------------------------------------
        // DS10 e DS14 - confirmarExclusaoFinal(alvo)
        //               + confirmarIdentidadeEAcao(ator, alvo)
        // -------------------------------------------------------------
        private void afr_btnConfirmar_Click(object sender, EventArgs e)
        {
            afr_lblStatus.Text = "";

            // Trava de seguranca: nunca confirma algo nao elegivel
            if (!afr_elegibilidade.afr_Elegivel)
            {
                afr_lblStatus.ForeColor = System.Drawing.Color.Firebrick;
                afr_lblStatus.Text = "Esta exclusao esta bloqueada pelo sistema.";
                return;
            }

            if (afr_txtSenha.Text.Trim() == "")
            {
                afr_lblStatus.ForeColor = System.Drawing.Color.Firebrick;
                afr_lblStatus.Text = "Digite a sua senha para confirmar.";
                afr_txtSenha.Focus();
                return;
            }

            // confirmarIdentidadeEAcao(ator, alvo)
            string afr_mensagem;
            bool afr_identidadeOk = afr_autenticacaoService.ConfirmarIdentidade(
                                        afr_txtSenha.Text, out afr_mensagem);

            if (!afr_identidadeOk)
            {
                afr_lblStatus.ForeColor = System.Drawing.Color.Firebrick;
                afr_lblStatus.Text = afr_mensagem;
                afr_txtSenha.Clear();
                afr_txtSenha.Focus();
                return;
            }

            // Identidade confirmada: a tela devolve o "sim" para quem
            // a chamou, que e quem chama o removerDefinitivamente().
            afr_Confirmado = true;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // -------------------------------------------------------------
        // Cancelar: nada e apagado
        // -------------------------------------------------------------
        private void afr_btnCancelar_Click(object sender, EventArgs e)
        {
            afr_Confirmado = false;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
