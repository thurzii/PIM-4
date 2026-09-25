/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmUsuarios.cs
   ---------------------------------------------------------------------
   CASOS DE USO ATENDIDOS NESTA TELA (CRUD completo de usuarios):
   DC11 - Criar Usuario ........ botao Salvar em modo NOVO  -> INSERT
   DC12 - Visualizar Usuarios .. lista e busca              -> SELECT
   DC13 - Editar Usuario ....... botao Salvar em modo EDICAO-> UPDATE
   DC14 - Excluir Usuario ...... botao Excluir              -> DELETE
   ===================================================================== */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmUsuarios : Form
    {
        private UsuarioService afr_usuarioService = new UsuarioService();
        private BancoService afr_bancoService = new BancoService();

        // afr_idSelecionado = 0 -> modo NOVO;  > 0 -> modo EDICAO
        private int afr_idSelecionado = 0;

        // Guarda os usuarios da tela para recuperar os dados no clique
        private List<Usuario> afr_listaAtual = new List<Usuario>();

        // Evita que o evento da lista dispare durante o recarregamento
        private bool afr_carregandoLista = false;

        // =============================================================
        // CONSTRUTOR
        // =============================================================
        public FrmUsuarios()
        {
            InitializeComponent();

            afr_cmbPerfil.SelectedIndex = 0;

            AtualizarLista("");
            EntrarModoNovo();
        }

        // =============================================================
        // EVENTO - BOTAO NOVO
        // =============================================================
        private void afr_btnNovo_Click(object sender, EventArgs e)
        {
            EntrarModoNovo();

            afr_txtSaida.Text = "Modo de cadastro. Preencha os campos e clique em Salvar.";
        }

        // =============================================================
        // EVENTO - BOTAO SALVAR (INSERT ou UPDATE)
        // =============================================================
        private void afr_btnSalvar_Click(object sender, EventArgs e)
        {
            // Monta o objeto com o que esta na tela
            Usuario afr_usuario = new Usuario();

            afr_usuario.afr_Id = afr_idSelecionado;
            afr_usuario.afr_Nome = afr_txtNome.Text.Trim();
            afr_usuario.afr_Email = afr_txtEmail.Text.Trim();
            afr_usuario.afr_Senha = afr_txtSenha.Text;
            afr_usuario.afr_Perfil = afr_cmbPerfil.Text;
            afr_usuario.afr_Ativo = afr_chkAtivo.Checked;

            string afr_mensagem;
            bool afr_ok;
            string afr_comando;

            // A MESMA tela faz os dois comandos: o que decide e o afr_idSelecionado
            if (afr_idSelecionado == 0)
            {
                afr_comando = "INSERT";
                afr_ok = afr_usuarioService.CriarUsuario(afr_usuario, out afr_mensagem);
            }
            else
            {
                afr_comando = "UPDATE";
                afr_ok = afr_usuarioService.EditarUsuario(afr_usuario, out afr_mensagem);
            }

            if (!afr_ok)
            {
                MessageBox.Show(afr_mensagem, "Atencao",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            afr_txtSaida.Text = afr_comando + " executado." + Environment.NewLine +
                                afr_mensagem + Environment.NewLine +
                                afr_usuario.ObterResumo();

            MessageBox.Show(afr_mensagem, "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

            AtualizarLista(afr_txtBusca.Text);
            EntrarModoNovo();
        }

        // =============================================================
        // EVENTO - BOTAO EXCLUIR (DELETE)
        // =============================================================
        private void afr_btnExcluir_Click(object sender, EventArgs e)
        {
            if (afr_idSelecionado == 0)
            {
                MessageBox.Show("Clique primeiro no usuario da lista que voce quer excluir.",
                                "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult afr_resposta = MessageBox.Show(
                "Excluir o usuario " + afr_txtNome.Text + " (Id " + afr_idSelecionado + ")?" +
                Environment.NewLine + Environment.NewLine +
                "O progresso dele nas aulas tambem sera apagado.",
                "Confirmacao", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (afr_resposta != DialogResult.Yes)
            {
                return;
            }

            string afr_mensagem;

            if (afr_usuarioService.ExcluirUsuario(afr_idSelecionado, out afr_mensagem))
            {
                afr_txtSaida.Text = "DELETE executado." + Environment.NewLine + afr_mensagem;

                AtualizarLista(afr_txtBusca.Text);
                EntrarModoNovo();
            }
            else
            {
                MessageBox.Show(afr_mensagem, "Erro",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =============================================================
        // EVENTO - BOTAO BUSCAR
        // =============================================================
        private void afr_btnBuscar_Click(object sender, EventArgs e)
        {
            AtualizarLista(afr_txtBusca.Text);
            EntrarModoNovo();

            afr_txtSaida.Text = "SELECT com filtro executado para o termo: \"" +
                                afr_txtBusca.Text.Trim() + "\"";
        }

        // =============================================================
        // EVENTO - BOTAO RECARREGAR
        // =============================================================
        private void afr_btnRecarregar_Click(object sender, EventArgs e)
        {
            afr_txtBusca.Clear();

            AtualizarLista("");
            EntrarModoNovo();

            afr_txtSaida.Text = "SELECT executado: lista recarregada da tabela dbo.Usuarios.";
        }

        // =============================================================
        // EVENTO - BOTAO FECHAR
        // =============================================================
        private void afr_btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // =============================================================
        // EVENTO - CLIQUE EM UM USUARIO DA LISTA
        // =============================================================
        private void afr_lstUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (afr_carregandoLista)
            {
                return;
            }

            if (afr_lstUsuarios.SelectedItems.Count == 0)
            {
                return;
            }

            int afr_id = Convert.ToInt32(afr_lstUsuarios.SelectedItems[0].Text);

            foreach (Usuario afr_usuario in afr_listaAtual)
            {
                if (afr_usuario.afr_Id == afr_id)
                {
                    PreencherCampos(afr_usuario);
                    return;
                }
            }
        }

        // =============================================================
        // METODO - LE O BANCO E PREENCHE A LISTA
        // =============================================================
        private void AtualizarLista(string afr_termo)
        {
            afr_carregandoLista = true;

            afr_lstUsuarios.Items.Clear();

            afr_listaAtual = afr_usuarioService.ListarUsuarios(afr_termo.Trim());

            foreach (Usuario afr_usuario in afr_listaAtual)
            {
                ListViewItem afr_item = new ListViewItem(afr_usuario.afr_Id.ToString());

                afr_item.SubItems.Add(afr_usuario.afr_Nome);
                afr_item.SubItems.Add(afr_usuario.afr_Email);
                afr_item.SubItems.Add(afr_usuario.afr_Perfil);

                if (afr_usuario.afr_Ativo)
                {
                    afr_item.SubItems.Add("Sim");
                }
                else
                {
                    afr_item.SubItems.Add("Nao");
                    afr_item.ForeColor = Color.Gray;
                }

                // Destaca os administradores
                if (afr_usuario.EhAdministrador() && afr_usuario.afr_Ativo)
                {
                    afr_item.ForeColor = Color.DarkBlue;
                }

                afr_lstUsuarios.Items.Add(afr_item);
            }

            afr_lblStatus.Text = afr_listaAtual.Count + " usuario(s) em dbo.Usuarios  |  " +
                                 afr_bancoService.ObterDescricao();

            afr_carregandoLista = false;
        }

        // =============================================================
        // METODO - CARREGA O USUARIO NOS CAMPOS (modo EDICAO)
        // =============================================================
        private void PreencherCampos(Usuario afr_usuario)
        {
            afr_idSelecionado = afr_usuario.afr_Id;

            afr_txtNome.Text = afr_usuario.afr_Nome;
            afr_txtEmail.Text = afr_usuario.afr_Email;
            afr_txtSenha.Text = afr_usuario.afr_Senha;
            afr_chkAtivo.Checked = afr_usuario.afr_Ativo;

            int afr_posicao = afr_cmbPerfil.FindStringExact(afr_usuario.afr_Perfil);

            if (afr_posicao >= 0)
            {
                afr_cmbPerfil.SelectedIndex = afr_posicao;
            }

            afr_lblModo.ForeColor = Color.DarkOrange;
            afr_lblModo.Text = "EDITANDO O Id " + afr_usuario.afr_Id;

            afr_txtSaida.Text = "Usuario carregado para edicao." + Environment.NewLine +
                                "Altere os campos e clique em Salvar para executar o UPDATE." +
                                Environment.NewLine +
                                "O botao Novo cancela a edicao." + Environment.NewLine +
                                afr_usuario.ObterResumo();
        }

        // =============================================================
        // METODO - VOLTA PARA O MODO DE CADASTRO
        // =============================================================
        private void EntrarModoNovo()
        {
            afr_idSelecionado = 0;

            afr_lstUsuarios.SelectedItems.Clear();

            afr_txtNome.Clear();
            afr_txtEmail.Clear();
            afr_txtSenha.Clear();
            afr_cmbPerfil.SelectedIndex = 0;
            afr_chkAtivo.Checked = true;

            afr_lblModo.ForeColor = Color.Black;
            afr_lblModo.Text = "NOVO USUARIO";

            afr_txtNome.Focus();
        }
    }
}
