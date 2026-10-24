/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmPrincipal.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER do menu principal.
   O menu tem TRES areas, uma para cada perfil do modelo:
   Aluno, Tutor e Administrador.
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
            this.afr_grpTutor = new System.Windows.Forms.GroupBox();
            this.afr_btnCursos = new System.Windows.Forms.Button();
            this.afr_btnProgresso = new System.Windows.Forms.Button();
            this.afr_lblTutorInfo = new System.Windows.Forms.Label();
            this.afr_grpAdmin = new System.Windows.Forms.GroupBox();
            this.afr_btnUsuarios = new System.Windows.Forms.Button();
            this.afr_lblAdminInfo = new System.Windows.Forms.Label();
            this.afr_btnTestar = new System.Windows.Forms.Button();
            this.afr_btnLogout = new System.Windows.Forms.Button();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.afr_grpAluno.SuspendLayout();
            this.afr_grpTutor.SuspendLayout();
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
            // afr_grpTutor
            //
            this.afr_grpTutor.Controls.Add(this.afr_btnCursos);
            this.afr_grpTutor.Controls.Add(this.afr_btnProgresso);
            this.afr_grpTutor.Controls.Add(this.afr_lblTutorInfo);
            this.afr_grpTutor.Location = new System.Drawing.Point(306, 104);
            this.afr_grpTutor.Name = "afr_grpTutor";
            this.afr_grpTutor.Size = new System.Drawing.Size(266, 200);
            this.afr_grpTutor.TabIndex = 4;
            this.afr_grpTutor.TabStop = false;
            this.afr_grpTutor.Text = "Area do Tutor";
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
            // afr_lblTutorInfo
            //
            this.afr_lblTutorInfo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblTutorInfo.Location = new System.Drawing.Point(18, 156);
            this.afr_lblTutorInfo.Name = "afr_lblTutorInfo";
            this.afr_lblTutorInfo.Size = new System.Drawing.Size(230, 36);
            this.afr_lblTutorInfo.TabIndex = 2;
            this.afr_lblTutorInfo.Text = "Cadastre cursos e aulas e acompanhe os alunos.";
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
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(30, 368);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(818, 36);
            this.afr_lblStatus.TabIndex = 8;
            this.afr_lblStatus.Text = "Pronto.";
            //
            // FrmPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 418);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblBoasVindas);
            this.Controls.Add(this.afr_lblSessao);
            this.Controls.Add(this.afr_grpAluno);
            this.Controls.Add(this.afr_grpTutor);
            this.Controls.Add(this.afr_grpAdmin);
            this.Controls.Add(this.afr_btnTestar);
            this.Controls.Add(this.afr_btnLogout);
            this.Controls.Add(this.afr_lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Plataforma de Cursos - Menu Principal";
            this.Load += new System.EventHandler(this.FrmPrincipal_Load);
            this.afr_grpAluno.ResumeLayout(false);
            this.afr_grpTutor.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox afr_grpTutor;
        private System.Windows.Forms.Button afr_btnCursos;
        private System.Windows.Forms.Button afr_btnProgresso;
        private System.Windows.Forms.Label afr_lblTutorInfo;
        private System.Windows.Forms.GroupBox afr_grpAdmin;
        private System.Windows.Forms.Button afr_btnUsuarios;
        private System.Windows.Forms.Label afr_lblAdminInfo;
        private System.Windows.Forms.Button afr_btnTestar;
        private System.Windows.Forms.Button afr_btnLogout;
        private System.Windows.Forms.Label afr_lblStatus;
    }
}
