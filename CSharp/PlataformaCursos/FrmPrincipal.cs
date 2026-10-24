/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmPrincipal.cs  (menu principal)
   Autor.....: Arthur Fernandes Reis (prefixo afr_)
   ---------------------------------------------------------------------
   Esta tela mostra opcoes DIFERENTES conforme quem entrou. O modelo
   do Astah tem tres atores nos diagramas de sequencia (Aluno, Tutor e
   Admin), entao o menu tem tres areas:

     ALUNO -> Area do Aluno
     TUTOR -> Area do Aluno + Area do Tutor
     ADMIN -> as tres areas (o admin pode tudo)

   Quem decide o que aparece sao os metodos da classe Sessao, que por
   baixo chamam os metodos PodeGerenciarCursos, PodeGerenciarUsuarios
   e PodeVerProgressoDeOutros da classe Usuario. A tela nao fica
   comparando texto de perfil na mao.

   CASO DE USO DC06 - Fazer Logout (botao Sair)
   DIAGRAMA DE SEQUENCIA DS06
   ===================================================================== */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace PlataformaCursos
{
    public partial class FrmPrincipal : Form
    {
        private BancoService afr_bancoService = new BancoService();
        private AutenticacaoService afr_autenticacaoService = new AutenticacaoService();

        // =============================================================
        // CONSTRUTOR - monta o menu conforme o perfil do usuario
        // =============================================================
        public FrmPrincipal()
        {
            InitializeComponent();
        }

        // =============================================================
        // EVENTO LOAD - roda quando a janela ja existe na tela.
        // A checagem de sessao fica aqui, e nao no construtor, porque
        // fechar um formulario que ainda nao apareceu causa erro.
        // =============================================================
        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            AplicarPerfil();
        }

        // =============================================================
        // METODO - mostra ou esconde as opcoes conforme o perfil
        // =============================================================
        private void AplicarPerfil()
        {
            // Protecao: ninguem chega aqui sem estar logado
            if (!Sessao.EstaAberta())
            {
                MessageBox.Show("Sessao nao encontrada. Faca o login novamente.", "Acesso",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            Usuario afr_usuario = Sessao.ObterUsuario();

            afr_lblBoasVindas.Text = "Ola, " + afr_usuario.ObterPrimeiroNome() +
                                     ".  |  " + afr_usuario.ObterResumo();

            afr_lblSessao.Text = "Sessao aberta " + Sessao.ObterTempoSessao() +
                                 ".  |  Cadastro desde " +
                                 afr_usuario.afr_DataCadastro.ToString("dd/MM/yyyy") + ".";

            // Area do aluno: todo mundo tem. O tutor e o admin tambem
            // podem navegar no catalogo como qualquer pessoa.
            afr_grpAluno.Visible = true;

            // Area do tutor: tutor e admin
            afr_grpTutor.Visible = Sessao.PodeGerenciarCursos();

            // Area de administracao: somente admin
            afr_grpAdmin.Visible = Sessao.PodeGerenciarUsuarios();

            afr_lblStatus.ForeColor = Color.DimGray;

            if (Sessao.EhAdministrador())
            {
                afr_lblStatus.Text = "Perfil ADMIN: catalogo, cursos, progresso dos alunos e usuarios.  "
                                   + afr_bancoService.ObterDescricao();
            }
            else if (Sessao.EhTutor())
            {
                afr_lblStatus.Text = "Perfil TUTOR: catalogo, cursos e aulas, e progresso dos alunos.  "
                                   + afr_bancoService.ObterDescricao();
            }
            else
            {
                afr_lblStatus.Text = "Perfil ALUNO: catalogo de cursos e as suas aulas.  "
                                   + afr_bancoService.ObterDescricao();
            }
        }

        // =============================================================
        // EVENTO - CATALOGO DE CURSOS (DC03, DC04, DC05, DC08)
        // =============================================================
        private void afr_btnCatalogo_Click(object sender, EventArgs e)
        {
            FrmCatalogo afr_tela = new FrmCatalogo();
            afr_tela.ShowDialog();
        }

        // =============================================================
        // EVENTO - GERENCIAR CURSOS (DC07 a DC10)
        // =============================================================
        private void afr_btnCursos_Click(object sender, EventArgs e)
        {
            FrmCursos afr_tela = new FrmCursos();
            afr_tela.ShowDialog();
        }

        // =============================================================
        // EVENTO - PROGRESSO DOS ALUNOS (DS07)
        // Tela nova: e o "acessarProgresso()" do diagrama.
        // =============================================================
        private void afr_btnProgresso_Click(object sender, EventArgs e)
        {
            FrmProgressoAlunos afr_tela = new FrmProgressoAlunos();
            afr_tela.ShowDialog();
        }

        // =============================================================
        // EVENTO - GERENCIAR USUARIOS (DC11 a DC14)
        // =============================================================
        private void afr_btnUsuarios_Click(object sender, EventArgs e)
        {
            FrmUsuarios afr_tela = new FrmUsuarios();
            afr_tela.ShowDialog();
        }

        // =============================================================
        // EVENTO - TESTAR CONEXAO
        // =============================================================
        private void afr_btnTestar_Click(object sender, EventArgs e)
        {
            string afr_mensagem;

            bool afr_ok = afr_bancoService.TestarConexao(out afr_mensagem);

            if (afr_ok)
            {
                afr_lblStatus.ForeColor = Color.Green;
            }
            else
            {
                afr_lblStatus.ForeColor = Color.Firebrick;
            }

            afr_lblStatus.Text = afr_mensagem;

            MessageBox.Show(afr_bancoService.ObterDescricao() + Environment.NewLine + afr_mensagem,
                            "Conexao", MessageBoxButtons.OK,
                            afr_ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }

        // =============================================================
        // EVENTO - LOGOUT (DC06 / DS06)
        // DS06: clicarLogout() -> encerrarSessao() -> finalizarSessao()
        //       -> sessaoEncerrada() -> retornarLogin() -> logoutConcluido()
        // =============================================================
        private void afr_btnLogout_Click(object sender, EventArgs e)
        {
            // clicarLogout()
            DialogResult afr_resposta = MessageBox.Show("Deseja encerrar a sessao?", "Logout",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Question);

            if (afr_resposta == DialogResult.Yes)
            {
                // encerrarSessao() -> finalizarSessao() -> sessaoEncerrada()
                afr_autenticacaoService.EncerrarSessao();

                // retornarLogin(): fechar esta tela devolve o controle
                // para o FrmLogin, que volta a aparecer.
                this.Close();
            }
        }
    }
}
