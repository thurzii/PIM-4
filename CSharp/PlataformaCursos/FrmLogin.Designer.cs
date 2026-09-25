/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmLogin.Designer.cs  (desenho da tela de login)
   ---------------------------------------------------------------------
   Arquivo do DESIGNER: criacao e posicionamento dos controles.
   Para editar a tela visualmente, clique com o botao direito em
   FrmLogin.cs no Gerenciador de Solucoes e escolha "Exibir Designer".
   ===================================================================== */

namespace PlataformaCursos
{
    partial class FrmLogin
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

        // =============================================================
        // Codigo gerado pelo Designer de Formularios do Windows
        // =============================================================
        private void InitializeComponent()
        {
            this.afr_lblTitulo = new System.Windows.Forms.Label();
            this.afr_lblSubtitulo = new System.Windows.Forms.Label();
            this.afr_lblEmail = new System.Windows.Forms.Label();
            this.afr_txtEmail = new System.Windows.Forms.TextBox();
            this.afr_lblSenha = new System.Windows.Forms.Label();
            this.afr_txtSenha = new System.Windows.Forms.TextBox();
            this.afr_chkMostrarSenha = new System.Windows.Forms.CheckBox();
            this.afr_btnEntrar = new System.Windows.Forms.Button();
            this.afr_lnkEsqueci = new System.Windows.Forms.LinkLabel();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // afr_lblTitulo
            //
            this.afr_lblTitulo.AutoSize = true;
            this.afr_lblTitulo.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.afr_lblTitulo.Location = new System.Drawing.Point(30, 25);
            this.afr_lblTitulo.Name = "afr_lblTitulo";
            this.afr_lblTitulo.Size = new System.Drawing.Size(290, 28);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "PLATAFORMA DE CURSOS";
            //
            // afr_lblSubtitulo
            //
            this.afr_lblSubtitulo.AutoSize = true;
            this.afr_lblSubtitulo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblSubtitulo.Location = new System.Drawing.Point(32, 58);
            this.afr_lblSubtitulo.Name = "afr_lblSubtitulo";
            this.afr_lblSubtitulo.Size = new System.Drawing.Size(200, 15);
            this.afr_lblSubtitulo.TabIndex = 1;
            this.afr_lblSubtitulo.Text = "Acesse com seu email e sua senha";
            //
            // afr_lblEmail
            //
            this.afr_lblEmail.AutoSize = true;
            this.afr_lblEmail.Location = new System.Drawing.Point(32, 95);
            this.afr_lblEmail.Name = "afr_lblEmail";
            this.afr_lblEmail.Size = new System.Drawing.Size(40, 15);
            this.afr_lblEmail.TabIndex = 2;
            this.afr_lblEmail.Text = "Email:";
            //
            // afr_txtEmail
            //
            this.afr_txtEmail.Location = new System.Drawing.Point(32, 114);
            this.afr_txtEmail.MaxLength = 100;
            this.afr_txtEmail.Name = "afr_txtEmail";
            this.afr_txtEmail.Size = new System.Drawing.Size(336, 23);
            this.afr_txtEmail.TabIndex = 3;
            //
            // afr_lblSenha
            //
            this.afr_lblSenha.AutoSize = true;
            this.afr_lblSenha.Location = new System.Drawing.Point(32, 150);
            this.afr_lblSenha.Name = "afr_lblSenha";
            this.afr_lblSenha.Size = new System.Drawing.Size(44, 15);
            this.afr_lblSenha.TabIndex = 4;
            this.afr_lblSenha.Text = "Senha:";
            //
            // afr_txtSenha
            //
            this.afr_txtSenha.Location = new System.Drawing.Point(32, 169);
            this.afr_txtSenha.MaxLength = 100;
            this.afr_txtSenha.Name = "afr_txtSenha";
            this.afr_txtSenha.PasswordChar = '*';
            this.afr_txtSenha.Size = new System.Drawing.Size(336, 23);
            this.afr_txtSenha.TabIndex = 5;
            //
            // afr_chkMostrarSenha
            //
            this.afr_chkMostrarSenha.AutoSize = true;
            this.afr_chkMostrarSenha.Location = new System.Drawing.Point(32, 201);
            this.afr_chkMostrarSenha.Name = "afr_chkMostrarSenha";
            this.afr_chkMostrarSenha.Size = new System.Drawing.Size(100, 19);
            this.afr_chkMostrarSenha.TabIndex = 6;
            this.afr_chkMostrarSenha.Text = "Mostrar senha";
            this.afr_chkMostrarSenha.UseVisualStyleBackColor = true;
            this.afr_chkMostrarSenha.CheckedChanged += new System.EventHandler(this.afr_chkMostrarSenha_CheckedChanged);
            //
            // afr_btnEntrar
            //
            this.afr_btnEntrar.BackColor = System.Drawing.Color.LightSteelBlue;
            this.afr_btnEntrar.Location = new System.Drawing.Point(32, 232);
            this.afr_btnEntrar.Name = "afr_btnEntrar";
            this.afr_btnEntrar.Size = new System.Drawing.Size(336, 42);
            this.afr_btnEntrar.TabIndex = 7;
            this.afr_btnEntrar.Text = "Entrar";
            this.afr_btnEntrar.UseVisualStyleBackColor = false;
            this.afr_btnEntrar.Click += new System.EventHandler(this.afr_btnEntrar_Click);
            //
            // afr_lnkEsqueci
            //
            this.afr_lnkEsqueci.AutoSize = true;
            this.afr_lnkEsqueci.Location = new System.Drawing.Point(32, 285);
            this.afr_lnkEsqueci.Name = "afr_lnkEsqueci";
            this.afr_lnkEsqueci.Size = new System.Drawing.Size(120, 15);
            this.afr_lnkEsqueci.TabIndex = 8;
            this.afr_lnkEsqueci.TabStop = true;
            this.afr_lnkEsqueci.Text = "Esqueci minha senha";
            this.afr_lnkEsqueci.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.afr_lnkEsqueci_LinkClicked);
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(32, 315);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(336, 34);
            this.afr_lblStatus.TabIndex = 9;
            this.afr_lblStatus.Text = "Pronto.";
            //
            // FrmLogin
            //
            this.AcceptButton = this.afr_btnEntrar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 360);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblSubtitulo);
            this.Controls.Add(this.afr_lblEmail);
            this.Controls.Add(this.afr_txtEmail);
            this.Controls.Add(this.afr_lblSenha);
            this.Controls.Add(this.afr_txtSenha);
            this.Controls.Add(this.afr_chkMostrarSenha);
            this.Controls.Add(this.afr_btnEntrar);
            this.Controls.Add(this.afr_lnkEsqueci);
            this.Controls.Add(this.afr_lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Plataforma de Cursos - Login";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblTitulo;
        private System.Windows.Forms.Label afr_lblSubtitulo;
        private System.Windows.Forms.Label afr_lblEmail;
        private System.Windows.Forms.TextBox afr_txtEmail;
        private System.Windows.Forms.Label afr_lblSenha;
        private System.Windows.Forms.TextBox afr_txtSenha;
        private System.Windows.Forms.CheckBox afr_chkMostrarSenha;
        private System.Windows.Forms.Button afr_btnEntrar;
        private System.Windows.Forms.LinkLabel afr_lnkEsqueci;
        private System.Windows.Forms.Label afr_lblStatus;
    }
}
