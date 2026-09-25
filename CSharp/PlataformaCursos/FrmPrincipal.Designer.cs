/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmPrincipal.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER do menu principal.
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
            this.afr_grpAluno = new System.Windows.Forms.GroupBox();
            this.afr_btnCatalogo = new System.Windows.Forms.Button();
            this.afr_lblAlunoInfo = new System.Windows.Forms.Label();
            this.afr_grpAdmin = new System.Windows.Forms.GroupBox();
            this.afr_btnUsuarios = new System.Windows.Forms.Button();
            this.afr_btnCursos = new System.Windows.Forms.Button();
            this.afr_btnTestar = new System.Windows.Forms.Button();
            this.afr_btnLogout = new System.Windows.Forms.Button();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.afr_grpAluno.SuspendLayout();
            this.afr_grpAdmin.SuspendLayout();
            this.SuspendLayout();
            //
            // afr_lblTitulo
            //
            this.afr_lblTitulo.AutoSize = true;
            this.afr_lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.afr_lblTitulo.Location = new System.Drawing.Point(28, 22);
            this.afr_lblTitulo.Name = "afr_lblTitulo";
            this.afr_lblTitulo.Size = new System.Drawing.Size(290, 28);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "PLATAFORMA DE CURSOS";
            //
            // afr_lblBoasVindas
            //
            this.afr_lblBoasVindas.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblBoasVindas.Location = new System.Drawing.Point(30, 56);
            this.afr_lblBoasVindas.Name = "afr_lblBoasVindas";
            this.afr_lblBoasVindas.Size = new System.Drawing.Size(600, 20);
            this.afr_lblBoasVindas.TabIndex = 1;
            this.afr_lblBoasVindas.Text = "Ola.";
            //
            // afr_grpAluno
            //
            this.afr_grpAluno.Controls.Add(this.afr_btnCatalogo);
            this.afr_grpAluno.Controls.Add(this.afr_lblAlunoInfo);
            this.afr_grpAluno.Location = new System.Drawing.Point(30, 92);
            this.afr_grpAluno.Name = "afr_grpAluno";
            this.afr_grpAluno.Size = new System.Drawing.Size(380, 175);
            this.afr_grpAluno.TabIndex = 2;
            this.afr_grpAluno.TabStop = false;
            this.afr_grpAluno.Text = "Area do Aluno";
            //
            // afr_btnCatalogo
            //
            this.afr_btnCatalogo.BackColor = System.Drawing.Color.LightSteelBlue;
            this.afr_btnCatalogo.Location = new System.Drawing.Point(20, 35);
            this.afr_btnCatalogo.Name = "afr_btnCatalogo";
            this.afr_btnCatalogo.Size = new System.Drawing.Size(340, 55);
            this.afr_btnCatalogo.TabIndex = 0;
            this.afr_btnCatalogo.Text = "Catalogo de Cursos";
            this.afr_btnCatalogo.UseVisualStyleBackColor = false;
            this.afr_btnCatalogo.Click += new System.EventHandler(this.afr_btnCatalogo_Click);
            //
            // afr_lblAlunoInfo
            //
            this.afr_lblAlunoInfo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblAlunoInfo.Location = new System.Drawing.Point(20, 100);
            this.afr_lblAlunoInfo.Name = "afr_lblAlunoInfo";
            this.afr_lblAlunoInfo.Size = new System.Drawing.Size(340, 60);
            this.afr_lblAlunoInfo.TabIndex = 1;
            this.afr_lblAlunoInfo.Text = "Veja os cursos disponiveis, faca buscas, acompanhe as aulas e marque o que ja concluiu.";
            //
            // afr_grpAdmin
            //
            this.afr_grpAdmin.Controls.Add(this.afr_btnUsuarios);
            this.afr_grpAdmin.Controls.Add(this.afr_btnCursos);
            this.afr_grpAdmin.Location = new System.Drawing.Point(430, 92);
            this.afr_grpAdmin.Name = "afr_grpAdmin";
            this.afr_grpAdmin.Size = new System.Drawing.Size(380, 175);
            this.afr_grpAdmin.TabIndex = 3;
            this.afr_grpAdmin.TabStop = false;
            this.afr_grpAdmin.Text = "Administracao";
            //
            // afr_btnCursos
            //
            this.afr_btnCursos.Location = new System.Drawing.Point(20, 35);
            this.afr_btnCursos.Name = "afr_btnCursos";
            this.afr_btnCursos.Size = new System.Drawing.Size(340, 55);
            this.afr_btnCursos.TabIndex = 0;
            this.afr_btnCursos.Text = "Gerenciar Cursos e Aulas";
            this.afr_btnCursos.UseVisualStyleBackColor = true;
            this.afr_btnCursos.Click += new System.EventHandler(this.afr_btnCursos_Click);
            //
            // afr_btnUsuarios
            //
            this.afr_btnUsuarios.Location = new System.Drawing.Point(20, 100);
            this.afr_btnUsuarios.Name = "afr_btnUsuarios";
            this.afr_btnUsuarios.Size = new System.Drawing.Size(340, 55);
            this.afr_btnUsuarios.TabIndex = 1;
            this.afr_btnUsuarios.Text = "Gerenciar Usuarios";
            this.afr_btnUsuarios.UseVisualStyleBackColor = true;
            this.afr_btnUsuarios.Click += new System.EventHandler(this.afr_btnUsuarios_Click);
            //
            // afr_btnTestar
            //
            this.afr_btnTestar.Location = new System.Drawing.Point(30, 288);
            this.afr_btnTestar.Name = "afr_btnTestar";
            this.afr_btnTestar.Size = new System.Drawing.Size(200, 38);
            this.afr_btnTestar.TabIndex = 4;
            this.afr_btnTestar.Text = "Testar conexao";
            this.afr_btnTestar.UseVisualStyleBackColor = true;
            this.afr_btnTestar.Click += new System.EventHandler(this.afr_btnTestar_Click);
            //
            // afr_btnLogout
            //
            this.afr_btnLogout.BackColor = System.Drawing.Color.MistyRose;
            this.afr_btnLogout.Location = new System.Drawing.Point(610, 288);
            this.afr_btnLogout.Name = "afr_btnLogout";
            this.afr_btnLogout.Size = new System.Drawing.Size(200, 38);
            this.afr_btnLogout.TabIndex = 5;
            this.afr_btnLogout.Text = "Sair (Logout)";
            this.afr_btnLogout.UseVisualStyleBackColor = false;
            this.afr_btnLogout.Click += new System.EventHandler(this.afr_btnLogout_Click);
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(30, 340);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(780, 20);
            this.afr_lblStatus.TabIndex = 6;
            this.afr_lblStatus.Text = "Pronto.";
            //
            // FrmPrincipal
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 375);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblBoasVindas);
            this.Controls.Add(this.afr_grpAluno);
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
            this.afr_grpAdmin.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblTitulo;
        private System.Windows.Forms.Label afr_lblBoasVindas;
        private System.Windows.Forms.GroupBox afr_grpAluno;
        private System.Windows.Forms.Button afr_btnCatalogo;
        private System.Windows.Forms.Label afr_lblAlunoInfo;
        private System.Windows.Forms.GroupBox afr_grpAdmin;
        private System.Windows.Forms.Button afr_btnUsuarios;
        private System.Windows.Forms.Button afr_btnCursos;
        private System.Windows.Forms.Button afr_btnTestar;
        private System.Windows.Forms.Button afr_btnLogout;
        private System.Windows.Forms.Label afr_lblStatus;
    }
}
