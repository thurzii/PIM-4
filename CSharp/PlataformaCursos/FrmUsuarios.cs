/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmUsuarios.cs
   Autor.....: Arthur Fernandes Reis (prefixo afr_)
   ---------------------------------------------------------------------
   CASOS DE USO ATENDIDOS NESTA TELA (CRUD completo de usuarios):
   DC11 / DS11 - Criar Usuario ..... Salvar em modo NOVO   -> INSERT
   DC12 / DS12 - Visualizar Usuario  botao Ver perfil      -> SELECT
   DC13 / DS13 - Editar Usuario .... Salvar em modo EDICAO -> UPDATE
   DC14 / DS14 - Excluir Usuario ... Excluir em 2 etapas   -> DELETE

   O QUE MUDOU POR CAUSA DO MODELO:

   1) DS11 pede verificarDuplicidade(email) -> emailDisponivel(true)
      ANTES de salvar. A tela avisa na hora se o email ja existe.

   2) DS12 nao e so a lista: pede o perfil completo com o historico de
      acoes (loginLog, acoesCrud, cursosRelacionados). Isso virou o
      botao "Ver perfil e historico", que abre o FrmPerfilUsuario.

   3) DS14 exclui em duas etapas, igual ao DS10 dos cursos:
      verificarElegibilidadeExclusao -> dadosDetalhados ->
      confirmarIdentidadeEAcao -> removerDefinitivamente.

   4) Existem TRES perfis agora: ALUNO, TUTOR e ADMIN.
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
        }

        // =============================================================
        // EVENTO LOAD
        // DS12 - verificarPermissaoAdmin(admin)
        // A checagem fica aqui, e nao no construtor, porque fechar um
        // formulario que ainda nao apareceu causa erro.
        // =============================================================
        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            string afr_mensagem;

            if (!afr_usuarioService.VerificarPermissaoAdmin(out afr_mensagem))
            {
                MessageBox.Show(afr_mensagem, "Acesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

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
        // DS11 - solicitarCadastro(dados) -> validaECria(dados)
        // DS13 - confirmarEdicao(novosDados) -> validaEAtualiza(...)
        // =============================================================
        private void afr_btnSalvar_Click(object sender, EventArgs e)
        {
            // DS11 - verificarDuplicidade(email) / emailDisponivel(true)
            // Confere ANTES de mandar para o servico, para dar o aviso
            // direto no campo do email.
            if (afr_usuarioService.VerificarDuplicidade(afr_txtEmail.Text, afr_idSelecionado))
            {
                MessageBox.Show("Este email ja esta cadastrado para outro usuario. "
                              + "Escolha outro email.",
                                "Email em uso", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                afr_txtEmail.Focus();
                afr_txtEmail.SelectAll();
                return;
            }

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

            // DS11 - exibirMensagemSucesso() / sucessoConfirmado()
            MessageBox.Show(afr_mensagem, "Sucesso",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

            AtualizarLista(afr_txtBusca.Text);
            EntrarModoNovo();
        }

        // =============================================================
        // EVENTO - BOTAO EXCLUIR (DELETE em duas etapas)
        // DS14 - solicitarExclusaoUsuario(usuario)
        //        -> verificarElegibilidadeExclusao(usuario)
        //        -> elegivelParaExclusao(true, dadosDetalhados)
        //        -> solicitarConfirmacaoExclusao(dadosDetalhados)
        //        -> confirmarExclusaoFinal + confirmarIdentidadeEAcao
        //        -> removerDefinitivamente(usuario)
        // =============================================================
        private void afr_btnExcluir_Click(object sender, EventArgs e)
        {
            // solicitarExclusaoUsuario(usuario)
            if (afr_idSelecionado == 0)
            {
                MessageBox.Show("Clique primeiro no usuario da lista que voce quer excluir.",
                                "Atencao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // verificarElegibilidadeExclusao(usuario)
            // -> elegivelParaExclusao(true, dadosDetalhados)
            // O servico ja barra, por exemplo, o admin tentando apagar
            // a propria conta ou o ultimo administrador do sistema.
            ElegibilidadeExclusao afr_elegibilidade =
                afr_usuarioService.VerificarElegibilidadeExclusao(afr_idSelecionado);

            // solicitarConfirmacaoExclusao(dadosDetalhados)
            FrmConfirmarExclusao afr_confirmacao = new FrmConfirmarExclusao(
                "Excluir o usuario " + afr_txtNome.Text + " (Id " + afr_idSelecionado + ")",
                afr_elegibilidade);

            afr_confirmacao.ShowDialog();

            // confirmarExclusaoFinal(usuario)
            if (!afr_confirmacao.afr_Confirmado)
            {
                afr_txtSaida.Text = "Exclusao cancelada. Nenhum DELETE foi executado.";
                return;
            }

            // removerDefinitivamente(usuario)
            string afr_mensagem;

            if (afr_usuarioService.RemoverDefinitivamente(afr_idSelecionado, out afr_mensagem))
            {
                // remocaoSucesso() / exibirMensagemSucesso()
                afr_txtSaida.Text = "DELETE executado." + Environment.NewLine + afr_mensagem;

                MessageBox.Show(afr_mensagem, "Sucesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

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
        // EVENTO - BOTAO VER PERFIL E HISTORICO
        // DS12 - realizarBuscarPerfil(admin, usuario)
        //        -> solicitarVisualizacaoPerfil(usuario)
        // =============================================================
        private void afr_btnVerPerfil_Click(object sender, EventArgs e)
        {
            if (afr_idSelecionado == 0)
            {
                MessageBox.Show("Clique primeiro em um usuario da lista.", "Atencao",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FrmPerfilUsuario afr_tela = new FrmPerfilUsuario(afr_idSelecionado);
            afr_tela.ShowDialog();

            afr_txtSaida.Text = "Perfil do Id " + afr_idSelecionado + " consultado: "
                              + "dados do usuario e historico de acoes (login, CRUD e cursos).";
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
        // DS13 - solicitarEdicaoUsuario(usuario) -> getDadosUsuario
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

            // getDadosUsuario(usuario) -> dadosUsuario(...)
            // Vai ao banco em vez de usar o objeto da lista: assim a
            // tela edita sempre o dado mais novo.
            Usuario afr_usuario = afr_usuarioService.BuscarDadosUsuario(afr_id);

            if (afr_usuario == null)
            {
                afr_txtSaida.Text = "O usuario Id " + afr_id + " nao esta mais no banco. "
                                  + "Clique em Recarregar lista.";
                return;
            }

            // exibirFormularioEdicao(dadosUsuario)
            PreencherCampos(afr_usuario);
        }

        // =============================================================
        // METODO - LE O BANCO E PREENCHE A LISTA
        // =============================================================
        private void AtualizarLista(string afr_termo)
        {
            afr_carregandoLista = true;

            afr_lstUsuarios.Items.Clear();

            afr_listaAtual = afr_usuarioService.ListarUsuarios(afr_termo.Trim());

            int afr_admins = 0;
            int afr_tutores = 0;
            int afr_alunos = 0;

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

                // DS12 - dataCadastro faz parte dos dados do usuario
                afr_item.SubItems.Add(afr_usuario.afr_DataCadastro.ToString("dd/MM/yyyy"));

                // Cada perfil ganha a sua cor, so para ficar facil de ler
                if (afr_usuario.afr_Ativo)
                {
                    if (afr_usuario.EhAdministrador())
                    {
                        afr_item.ForeColor = Color.DarkBlue;
                    }
                    else if (afr_usuario.EhTutor())
                    {
                        afr_item.ForeColor = Color.DarkMagenta;
                    }
                }

                // Conta quantos tem de cada perfil
                if (afr_usuario.EhAdministrador())
                {
                    afr_admins = afr_admins + 1;
                }
                else if (afr_usuario.EhTutor())
                {
                    afr_tutores = afr_tutores + 1;
                }
                else
                {
                    afr_alunos = afr_alunos + 1;
                }

                afr_lstUsuarios.Items.Add(afr_item);
            }

            afr_lblStatus.Text = afr_listaAtual.Count + " usuario(s) em dbo.Usuarios  |  "
                               + afr_admins + " admin, " + afr_tutores + " tutor, "
                               + afr_alunos + " aluno  |  "
                               + afr_bancoService.ObterDescricao();

            afr_carregandoLista = false;
        }

        // =============================================================
        // METODO - CARREGA O USUARIO NOS CAMPOS (modo EDICAO)
        // DS13 - exibirFormularioEdicao(dadosUsuario)
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
                                "Cadastro em " +
                                afr_usuario.afr_DataCadastro.ToString("dd/MM/yyyy HH:mm") +
                                Environment.NewLine +
                                afr_usuario.ObterResumo();
        }

        // =============================================================
        // METODO - VOLTA PARA O MODO DE CADASTRO
        // =============================================================
        private void EntrarModoNovo()
        {
            afr_idSelecionado = 0;

            afr_carregandoLista = true;
            afr_lstUsuarios.SelectedItems.Clear();
            afr_carregandoLista = false;

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
