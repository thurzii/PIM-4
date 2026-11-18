/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmPrincipal.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER do menu principal.
   O menu tem TRES areas, uma para cada perfil do modelo:
   Aluno, Professor e Administrador.
   ===================================================================== */

namespace PlataformaCursos
{
    partial class FrmPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.afr_lblTitulo = new System.Windows.Forms.Label();
            this.afr_lblBoasVindas = new System.Windows.Forms.Label();
            this.afr_lblSessao = new System.Windows.Forms.Label();
            this.afr_grpAluno = new System.Windows.Forms.GroupBox();
            this.afr_btnCatalogo = new System.Windows.Forms.Button();
            this.afr_lblAlunoInfo = new System.Windows.Forms.Label();
            this.afr_grpProfessor = new System.Windows.Forms.GroupBox();
            this.afr_btnCursos = new System.Windows.Forms.Button();
            this.afr_btnProgresso = new System.Windows.Forms.Button();
            this.afr_lblProfessorInfo = new System.Windows.Forms.Label();
            this.afr_grpAdmin = new System.Windows.Forms.GroupBox();
            this.afr_btnUsuarios = new System.Windows.Forms.Button();
            this.afr_lblAdminInfo = new System.Windows.Forms.Label();
            this.afr_btnTestar = new System.Windows.Forms.Button();
            this.afr_btnLogout = new System.Windows.Forms.Button();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.afr_lblNivel = new System.Windows.Forms.Label();
            this.afr_lblXp = new System.Windows.Forms.Label();
            this.afr_lblConquistas = new System.Windows.Forms.Label();
            this.afr_barraNivel = new System.Windows.Forms.ProgressBar();
            this.afr_grpAluno.SuspendLayout();
            this.afr_grpProfessor.SuspendLayout();
            this.afr_grpAdmin.SuspendLayout();
            this.SuspendLayout();
            //
            // afr_lblTitulo
            //
            this.afr_lblTitulo.AutoSize = true;
            this.afr_lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.afr_lblTitulo.Location = new System.Drawing.Point(28, 20);
            this.afr_lblTitulo.Name = "afr_lblTitulo";
            this.afr_lblTitulo.Size = new System.Drawing.Size(290, 28);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "PLATAFORMA DE CURSOS";
            //
            // afr_lblBoasVindas
            //
            this.afr_lblBoasVindas.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblBoasVindas.Location = new System.Drawing.Point(30, 54);
            this.afr_lblBoasVindas.Name = "afr_lblBoasVindas";
            this.afr_lblBoasVindas.Size = new System.Drawing.Size(818, 20);
            this.afr_lblBoasVindas.TabIndex = 1;
            this.afr_lblBoasVindas.Text = "Ola.";
            //
            // afr_lblSessao
            //
            this.afr_lblSessao.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblSessao.Location = new System.Drawing.Point(30, 76);
            this.afr_lblSessao.Name = "afr_lblSessao";
            this.afr_lblSessao.Size = new System.Drawing.Size(818, 20);
            this.afr_lblSessao.TabIndex = 2;
            this.afr_lblSessao.Text = "";
            //
            // afr_grpAluno
            //
            this.afr_grpAluno.Controls.Add(this.afr_btnCatalogo);
            this.afr_grpAluno.Controls.Add(this.afr_lblAlunoInfo);
            this.afr_grpAluno.Location = new System.Drawing.Point(30, 104);
            this.afr_grpAluno.Name = "afr_grpAluno";
            this.afr_grpAluno.Size = new System.Drawing.Size(266, 200);
            this.afr_grpAluno.TabIndex = 3;
            this.afr_grpAluno.TabStop = false;
            this.afr_grpAluno.Text = "Area do Aluno";
            //
            // afr_btnCatalogo
            //
            this.afr_btnCatalogo.BackColor = System.Drawing.Color.LightSteelBlue;
            this.afr_btnCatalogo.Location = new System.Drawing.Point(18, 32);
            this.afr_btnCatalogo.Name = "afr_btnCatalogo";
            this.afr_btnCatalogo.Size = new System.Drawing.Size(230, 55);
            this.afr_btnCatalogo.TabIndex = 0;
            this.afr_btnCatalogo.Text = "Catalogo de Cursos";
            this.afr_btnCatalogo.UseVisualStyleBackColor = false;
            this.afr_btnCatalogo.Click += new System.EventHandler(this.afr_btnCatalogo_Click);
            //
            // afr_lblAlunoInfo
            //
            this.afr_lblAlunoInfo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblAlunoInfo.Location = new System.Drawing.Point(18, 95);
            this.afr_lblAlunoInfo.Name = "afr_lblAlunoInfo";
            this.afr_lblAlunoInfo.Size = new System.Drawing.Size(230, 95);
            this.afr_lblAlunoInfo.TabIndex = 1;
            this.afr_lblAlunoInfo.Text = "Veja os cursos, use os filtros da busca, abra o conteudo das aulas e marque o que ja concluiu.";
            //
            // afr_grpProfessor
            //
            this.afr_grpProfessor.Controls.Add(this.afr_btnCursos);
            this.afr_grpProfessor.Controls.Add(this.afr_btnProgresso);
            this.afr_grpProfessor.Controls.Add(this.afr_lblProfessorInfo);
            this.afr_grpProfessor.Location = new System.Drawing.Point(306, 104);
            this.afr_grpProfessor.Name = "afr_grpProfessor";
            this.afr_grpProfessor.Size = new System.Drawing.Size(266, 200);
            this.afr_grpProfessor.TabIndex = 4;
            this.afr_grpProfessor.TabStop = false;
            this.afr_grpProfessor.Text = "Area do Professor";
            //
            // afr_btnCursos
            //
            this.afr_btnCursos.BackColor = System.Drawing.Color.Lavender;
            this.afr_btnCursos.Location = new System.Drawing.Point(18, 32);
            this.afr_btnCursos.Name = "afr_btnCursos";
            this.afr_btnCursos.Size = new System.Drawing.Size(230, 55);
            this.afr_btnCursos.TabIndex = 0;
            this.afr_btnCursos.Text = "Gerenciar Cursos e Aulas";
            this.afr_btnCursos.UseVisualStyleBackColor = false;
            this.afr_btnCursos.Click += new System.EventHandler(this.afr_btnCursos_Click);
            //
            // afr_btnProgresso
            //
            this.afr_btnProgresso.BackColor = System.Drawing.Color.Lavender;
            this.afr_btnProgresso.Location = new System.Drawing.Point(18, 95);
            this.afr_btnProgresso.Name = "afr_btnProgresso";
            this.afr_btnProgresso.Size = new System.Drawing.Size(230, 55);
            this.afr_btnProgresso.TabIndex = 1;
            this.afr_btnProgresso.Text = "Progresso dos Alunos";
            this.afr_btnProgresso.UseVisualStyleBackColor = false;
            this.afr_btnProgresso.Click += new System.EventHandler(this.afr_btnProgresso_Click);
            //
            // afr_lblProfessorInfo
            //
            this.afr_lblProfessorInfo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblProfessorInfo.Location = new System.Drawing.Point(18, 156);
            this.afr_lblProfessorInfo.Name = "afr_lblProfessorInfo";
            this.afr_lblProfessorInfo.Size = new System.Drawing.Size(230, 36);
            this.afr_lblProfessorInfo.TabIndex = 2;
            this.afr_lblProfessorInfo.Text = "Cadastre cursos e aulas e acompanhe os alunos.";
            //
            // afr_grpAdmin
            //
            this.afr_grpAdmin.Controls.Add(this.afr_btnUsuarios);
            this.afr_grpAdmin.Controls.Add(this.afr_lblAdminInfo);
            this.afr_grpAdmin.Location = new System.Drawing.Point(582, 104);
            this.afr_grpAdmin.Name = "afr_grpAdmin";
            this.afr_grpAdmin.Size = new System.Drawing.Size(266, 200);
            this.afr_grpAdmin.TabIndex = 5;
            this.afr_grpAdmin.TabStop = false;
            this.afr_grpAdmin.Text = "Administracao";
            //
            // afr_btnUsuarios
            //
            this.afr_btnUsuarios.BackColor = System.Drawing.Color.OldLace;
            this.afr_btnUsuarios.Location = new System.Drawing.Point(18, 32);
            this.afr_btnUsuarios.Name = "afr_btnUsuarios";
            this.afr_btnUsuarios.Size = new System.Drawing.Size(230, 55);
            this.afr_btnUsuarios.TabIndex = 0;
            this.afr_btnUsuarios.Text = "Gerenciar Usuarios";
            this.afr_btnUsuarios.UseVisualStyleBackColor = false;
            this.afr_btnUsuarios.Click += new System.EventHandler(this.afr_btnUsuarios_Click);
            //
            // afr_lblAdminInfo
            //
            this.afr_lblAdminInfo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblAdminInfo.Location = new System.Drawing.Point(18, 95);
            this.afr_lblAdminInfo.Name = "afr_lblAdminInfo";
            this.afr_lblAdminInfo.Size = new System.Drawing.Size(230, 95);
            this.afr_lblAdminInfo.TabIndex = 1;
            this.afr_lblAdminInfo.Text = "Cadastre, edite e exclua usuarios, e consulte o perfil de cada um com o historico de acoes.";
            //
            // afr_btnTestar
            //
            this.afr_btnTestar.Location = new System.Drawing.Point(30, 320);
            this.afr_btnTestar.Name = "afr_btnTestar";
            this.afr_btnTestar.Size = new System.Drawing.Size(200, 38);
            this.afr_btnTestar.TabIndex = 6;
            this.afr_btnTestar.Text = "Testar conexao";
            this.afr_btnTestar.UseVisualStyleBackColor = true;
            this.afr_btnTestar.Click += new System.EventHandler(this.afr_btnTestar_Click);
            //
            // afr_btnLogout
            //
            this.afr_btnLogout.BackColor = System.Drawing.Color.MistyRose;
            this.afr_btnLogout.Location = new System.Drawing.Point(648, 320);
            this.afr_btnLogout.Name = "afr_btnLogout";
            this.afr_btnLogout.Size = new System.Drawing.Size(200, 38);
            this.afr_btnLogout.TabIndex = 7;
            this.afr_btnLogout.Text = "Sair (Logout)";
            this.afr_btnLogout.UseVisualStyleBackColor = false;
            this.afr_btnLogout.Click += new System.EventHandler(this.afr_btnLogout_Click);
            //
            // VERSAO 3 - PAINEL DE GAMIFICACAO
            //
            // Fica ABAIXO dos botoes Testar e Logout, que ocupam a faixa
            // y=320..358. A janela cresceu 78 pixels para abrir espaco;
            // nenhum controle que ja existia mudou de posicao, so a
            // barra de status desceu junto.
            //
            // afr_lblNivel
            //
            this.afr_lblNivel.Font = new System.Drawing.Font("Segoe UI", 10F,
                System.Drawing.FontStyle.Bold);
            this.afr_lblNivel.ForeColor = System.Drawing.Color.DarkViolet;
            this.afr_lblNivel.Location = new System.Drawing.Point(30, 368);
            this.afr_lblNivel.Name = "afr_lblNivel";
            this.afr_lblNivel.Size = new System.Drawing.Size(300, 20);
            this.afr_lblNivel.TabIndex = 20;
            this.afr_lblNivel.Text = "Nivel 1";
            //
            // afr_lblXp
            //
            this.afr_lblXp.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblXp.Location = new System.Drawing.Point(340, 370);
            this.afr_lblXp.Name = "afr_lblXp";
            this.afr_lblXp.Size = new System.Drawing.Size(320, 20);
            this.afr_lblXp.TabIndex = 21;
            this.afr_lblXp.Text = "0 XP";
            //
            // afr_lblConquistas
            //
            // Linha propria, largura inteira: aqui entram os NOMES das
            // medalhas, nao so a contagem, e em 180px nao caberiam.
            this.afr_lblConquistas.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblConquistas.Location = new System.Drawing.Point(30, 414);
            this.afr_lblConquistas.Name = "afr_lblConquistas";
            this.afr_lblConquistas.Size = new System.Drawing.Size(820, 20);
            this.afr_lblConquistas.TabIndex = 22;
            this.afr_lblConquistas.Text = "Conquistas: 0 de 5";
            //
            // afr_barraNivel
            //
            // Minimo 0 e maximo 100 porque ObterPercentualDoNivel()
            // devolve exatamente nessa escala.
            //
            this.afr_barraNivel.Location = new System.Drawing.Point(30, 394);
            this.afr_barraNivel.Maximum = 100;
            this.afr_barraNivel.Minimum = 0;
            this.afr_barraNivel.Name = "afr_barraNivel";
            this.afr_barraNivel.Size = new System.Drawing.Size(820, 14);
            this.afr_barraNivel.TabIndex = 23;
            this.afr_barraNivel.Value = 0;
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(30, 442);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(818, 36);
            this.afr_lblStatus.TabIndex = 8;
            this.afr_lblStatus.Text = "Pronto.";
            //
            // FrmPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            // VERSAO 3 - 26 pixels a mais, para o painel de XP caber sem
            // espremer a barra de status que ja existia.
            this.ClientSize = new System.Drawing.Size(880, 496);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblBoasVindas);
            this.Controls.Add(this.afr_lblSessao);
            this.Controls.Add(this.afr_grpAluno);
            this.Controls.Add(this.afr_grpProfessor);
            this.Controls.Add(this.afr_grpAdmin);
            this.Controls.Add(this.afr_btnTestar);
            this.Controls.Add(this.afr_btnLogout);
            this.Controls.Add(this.afr_lblStatus);
            this.Controls.Add(this.afr_lblNivel);
            this.Controls.Add(this.afr_lblXp);
            this.Controls.Add(this.afr_lblConquistas);
            this.Controls.Add(this.afr_barraNivel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Plataforma de Cursos - Menu Principal";
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.afr_grpAluno.ResumeLayout(false);
            this.afr_grpProfessor.ResumeLayout(false);
            this.afr_grpAdmin.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblTitulo;
        private System.Windows.Forms.Label afr_lblBoasVindas;
        private System.Windows.Forms.Label afr_lblSessao;
        private System.Windows.Forms.GroupBox afr_grpAluno;
        private System.Windows.Forms.Button afr_btnCatalogo;
        private System.Windows.Forms.Label afr_lblAlunoInfo;
        private System.Windows.Forms.GroupBox afr_grpProfessor;
        private System.Windows.Forms.Button afr_btnCursos;
        private System.Windows.Forms.Button afr_btnProgresso;
        private System.Windows.Forms.Label afr_lblProfessorInfo;
        private System.Windows.Forms.GroupBox afr_grpAdmin;
        private System.Windows.Forms.Button afr_btnUsuarios;
        private System.Windows.Forms.Label afr_lblAdminInfo;
        private System.Windows.Forms.Button afr_btnTestar;
        private System.Windows.Forms.Button afr_btnLogout;
        private System.Windows.Forms.Label afr_lblStatus;
        private System.Windows.Forms.Label afr_lblNivel;
        private System.Windows.Forms.Label afr_lblXp;
        private System.Windows.Forms.Label afr_lblConquistas;
        private System.Windows.Forms.ProgressBar afr_barraNivel;
    }
}
