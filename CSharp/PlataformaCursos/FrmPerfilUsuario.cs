/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmPerfilUsuario.cs
   Autor.....: Arthur Fernandes Reis (prefixo afr_)
   ---------------------------------------------------------------------
   TELA DE PERFIL DO USUARIO (parte do TelaAdminDashboard do modelo)

   Esta tela atende o diagrama DS12 (Visualizar Usuario). A sequencia
   de mensagens do modelo e esta:

       1) realizarBuscarPerfil(admin, usuario)
       2) verificarPermissaoAdmin(admin)
       3) solicitarVisualizacaoPerfil(usuario)
       4) elegivelParaVisualizacao(true)
       5) buscarDadosUsuario(usuario)
       6) dadosUsuario(nome, email, papel, dataCadastro)
       7) buscarHistoricoAcoes(usuario)
       8) historicoAcoes(loginLog, acoesCrud, cursosRelacionados)
       9) perfilCompleto(dadosEHistorico)
      10) exibirPerfilUsuario(dadosEHistorico)

   Repare que o modelo pede TRES listas separadas de historico, e nao
   uma lista unica. Por isso a tela tem as tres: acessos, cadastros e
   acoes em cursos. Quem grava esse historico e o LogService.
   ===================================================================== */

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmPerfilUsuario : Form
    {
        // Servicos usados por esta tela
        private UsuarioService afr_usuarioService = new UsuarioService();
        private LogService afr_logService = new LogService();

        // Id de quem esta sendo consultado
        private int afr_usuarioId = 0;

        // -------------------------------------------------------------
        // CONSTRUTOR
        // DS12 - realizarBuscarPerfil(admin, usuario)
        // -------------------------------------------------------------
        public FrmPerfilUsuario(int afr_id)
        {
            InitializeComponent();

            this.afr_usuarioId = afr_id;
        }

        // -------------------------------------------------------------
        // LOAD
        // As checagens ficam aqui porque Close() dentro do construtor
        // descarta o formulario antes de ele aparecer.
        // -------------------------------------------------------------
        private void FrmPerfilUsuario_Load(object sender, EventArgs e)
        {
            if (!Sessao.EstaAberta())
            {
                MessageBox.Show("Sessao encerrada. Entre novamente.",
                                "Perfil",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            // DS12 - verificarPermissaoAdmin(admin)
            string afr_mensagem;

            if (!afr_usuarioService.VerificarPermissaoAdmin(out afr_mensagem))
            {
                MessageBox.Show(afr_mensagem,
                                "Perfil",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            CarregarPerfil();
        }

        // -------------------------------------------------------------
        // DS12 - solicitarVisualizacaoPerfil(usuario)
        //        -> buscarDadosUsuario -> buscarHistoricoAcoes
        //        -> exibirPerfilUsuario(dadosEHistorico)
        // -------------------------------------------------------------
        private void CarregarPerfil()
        {
            // buscarDadosUsuario(usuario) -> dadosUsuario(...)
            Usuario afr_usuario = afr_usuarioService.BuscarDadosUsuario(afr_usuarioId);

            if (afr_usuario == null)
            {
                MessageBox.Show("Usuario nao encontrado.",
                                "Perfil",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            // dadosUsuario(nome, email, papel, dataCadastro)
            this.Text = "Perfil - " + afr_usuario.afr_Nome;

            afr_lblTitulo.Text = "Perfil de " + afr_usuario.ObterPrimeiroNome();
            afr_lblNome.Text = afr_usuario.afr_Nome;
            afr_lblEmail.Text = "Email: " + afr_usuario.afr_Email;
            afr_lblPapel.Text = "Papel: " + afr_usuario.afr_Perfil
                              + (afr_usuario.afr_Ativo ? "   (ativo)" : "   (inativo)");
            afr_lblCadastro.Text = "Cadastrado em: "
                                 + afr_usuario.afr_DataCadastro.ToString("dd/MM/yyyy HH:mm");

            // buscarHistoricoAcoes(usuario)
            // -> historicoAcoes(loginLog, acoesCrud, cursosRelacionados)
            List<LogAcao> afr_loginLog = afr_logService.BuscarLoginLog(afr_usuarioId);
            List<LogAcao> afr_acoesCrud = afr_logService.BuscarAcoesCrud(afr_usuarioId);
            List<LogAcao> afr_cursosRelacionados = afr_logService.BuscarCursosRelacionados(afr_usuarioId);

            // exibirPerfilUsuario(dadosEHistorico)
            PreencherLista(afr_lstLogin, afr_loginLog);
            PreencherLista(afr_lstCrud, afr_acoesCrud);
            PreencherLista(afr_lstCursos, afr_cursosRelacionados);

            // perfilCompleto(dadosEHistorico): o resumo final
            int afr_total = afr_loginLog.Count + afr_acoesCrud.Count + afr_cursosRelacionados.Count;

            if (afr_total == 0)
            {
                afr_lblResumo.ForeColor = System.Drawing.Color.Chocolate;
                afr_lblResumo.Text = "Este usuario ainda nao tem nenhuma acao registrada. "
                                   + "O historico comeca a encher a partir do primeiro login dele.";
            }
            else
            {
                afr_lblResumo.ForeColor = System.Drawing.Color.DimGray;
                afr_lblResumo.Text = "Acessos: " + afr_loginLog.Count
                                   + "   |   Cadastros: " + afr_acoesCrud.Count
                                   + "   |   Acoes em cursos: " + afr_cursosRelacionados.Count
                                   + "   |   Total de registros: " + afr_total;
            }
        }

        // -------------------------------------------------------------
        // Preenche uma das tres listas de historico.
        // Uso o mesmo metodo nas tres para nao repetir codigo.
        // -------------------------------------------------------------
        private void PreencherLista(ListView afr_lista, List<LogAcao> afr_registros)
        {
            afr_lista.Items.Clear();

            for (int afr_i = 0; afr_i < afr_registros.Count; afr_i++)
            {
                LogAcao afr_registro = afr_registros[afr_i];

                ListViewItem afr_item = new ListViewItem(
                    afr_registro.afr_DataHora.ToString("dd/MM/yyyy HH:mm"));

                afr_item.SubItems.Add(afr_registro.afr_Descricao);

                afr_lista.Items.Add(afr_item);
            }

            if (afr_registros.Count == 0)
            {
                ListViewItem afr_vazio = new ListViewItem("-");
                afr_vazio.SubItems.Add("sem registros");
                afr_vazio.ForeColor = System.Drawing.Color.DimGray;

                afr_lista.Items.Add(afr_vazio);
            }
        }

        private void afr_btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
