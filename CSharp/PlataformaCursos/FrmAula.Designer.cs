/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmAula.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER da tela de aula.
   ===================================================================== */

namespace PlataformaCursos
{
    partial class FrmAula
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
            this.afr_lblCurso = new System.Windows.Forms.Label();
            this.afr_lblTituloAula = new System.Windows.Forms.Label();
            this.afr_lblInfo = new System.Windows.Forms.Label();
            this.afr_lblConteudoTitulo = new System.Windows.Forms.Label();
            this.afr_txtConteudo = new System.Windows.Forms.TextBox();
            this.afr_lblStatusAula = new System.Windows.Forms.Label();
            this.afr_btnConcluir = new System.Windows.Forms.Button();
            this.afr_btnFechar = new System.Windows.Forms.Button();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // afr_lblCurso
            //
            this.afr_lblCurso.AutoSize = true;
            this.afr_lblCurso.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblCurso.Location = new System.Drawing.Point(20, 14);
            this.afr_lblCurso.Name = "afr_lblCurso";
            this.afr_lblCurso.Size = new System.Drawing.Size(120, 15);
            this.afr_lblCurso.TabIndex = 0;
            this.afr_lblCurso.Text = "Curso";
            //
            // afr_lblTituloAula
            //
            this.afr_lblTituloAula.AutoSize = true;
            this.afr_lblTituloAula.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.afr_lblTituloAula.Location = new System.Drawing.Point(20, 34);
            this.afr_lblTituloAula.Name = "afr_lblTituloAula";
            this.afr_lblTituloAula.Size = new System.Drawing.Size(160, 25);
            this.afr_lblTituloAula.TabIndex = 1;
            this.afr_lblTituloAula.Text = "Titulo da aula";
            //
            // afr_lblInfo
            //
            this.afr_lblInfo.AutoSize = true;
            this.afr_lblInfo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblInfo.Location = new System.Drawing.Point(20, 64);
            this.afr_lblInfo.Name = "afr_lblInfo";
            this.afr_lblInfo.Size = new System.Drawing.Size(200, 15);
            this.afr_lblInfo.TabIndex = 2;
            this.afr_lblInfo.Text = "";
            //
            // afr_lblConteudoTitulo
            //
            this.afr_lblConteudoTitulo.AutoSize = true;
            this.afr_lblConteudoTitulo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.afr_lblConteudoTitulo.Location = new System.Drawing.Point(20, 94);
            this.afr_lblConteudoTitulo.Name = "afr_lblConteudoTitulo";
            this.afr_lblConteudoTitulo.Size = new System.Drawing.Size(140, 15);
            this.afr_lblConteudoTitulo.TabIndex = 3;
            this.afr_lblConteudoTitulo.Text = "Conteudo da aula:";
            //
            // afr_txtConteudo
            //
            this.afr_txtConteudo.BackColor = System.Drawing.Color.White;
            this.afr_txtConteudo.Location = new System.Drawing.Point(20, 114);
            this.afr_txtConteudo.Multiline = true;
            this.afr_txtConteudo.Name = "afr_txtConteudo";
            this.afr_txtConteudo.ReadOnly = true;
            this.afr_txtConteudo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.afr_txtConteudo.Size = new System.Drawing.Size(560, 250);
            this.afr_txtConteudo.TabIndex = 4;
            //
            // afr_lblStatusAula
            //
            this.afr_lblStatusAula.AutoSize = true;
            this.afr_lblStatusAula.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.afr_lblStatusAula.Location = new System.Drawing.Point(20, 376);
            this.afr_lblStatusAula.Name = "afr_lblStatusAula";
            this.afr_lblStatusAula.Size = new System.Drawing.Size(150, 19);
            this.afr_lblStatusAula.TabIndex = 5;
            this.afr_lblStatusAula.Text = "";
            //
            // afr_btnConcluir
            //
            this.afr_btnConcluir.BackColor = System.Drawing.Color.Honeydew;
            this.afr_btnConcluir.Location = new System.Drawing.Point(20, 402);
            this.afr_btnConcluir.Name = "afr_btnConcluir";
            this.afr_btnConcluir.Size = new System.Drawing.Size(280, 44);
            this.afr_btnConcluir.TabIndex = 6;
            this.afr_btnConcluir.Text = "Marcar como concluida";
            this.afr_btnConcluir.UseVisualStyleBackColor = false;
            this.afr_btnConcluir.Click += new System.EventHandler(this.afr_btnConcluir_Click);
            //
            // afr_btnFechar
            //
            this.afr_btnFechar.Location = new System.Drawing.Point(320, 402);
            this.afr_btnFechar.Name = "afr_btnFechar";
            this.afr_btnFechar.Size = new System.Drawing.Size(260, 44);
            this.afr_btnFechar.TabIndex = 7;
            this.afr_btnFechar.Text = "Fechar";
            this.afr_btnFechar.UseVisualStyleBackColor = true;
            this.afr_btnFechar.Click += new System.EventHandler(this.afr_btnFechar_Click);
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(20, 454);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(560, 20);
            this.afr_lblStatus.TabIndex = 8;
            this.afr_lblStatus.Text = "";
            //
            // FrmAula
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.afr_btnFechar;
            this.ClientSize = new System.Drawing.Size(600, 486);
            this.Controls.Add(this.afr_lblCurso);
            this.Controls.Add(this.afr_lblTituloAula);
            this.Controls.Add(this.afr_lblInfo);
            this.Controls.Add(this.afr_lblConteudoTitulo);
            this.Controls.Add(this.afr_txtConteudo);
            this.Controls.Add(this.afr_lblStatusAula);
            this.Controls.Add(this.afr_btnConcluir);
            this.Controls.Add(this.afr_btnFechar);
            this.Controls.Add(this.afr_lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAula";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Aula";
            this.Load += new System.EventHandler(this.FrmAula_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblCurso;
        private System.Windows.Forms.Label afr_lblTituloAula;
        private System.Windows.Forms.Label afr_lblInfo;
        private System.Windows.Forms.Label afr_lblConteudoTitulo;
        private System.Windows.Forms.TextBox afr_txtConteudo;
        private System.Windows.Forms.Label afr_lblStatusAula;
        private System.Windows.Forms.Button afr_btnConcluir;
        private System.Windows.Forms.Button afr_btnFechar;
        private System.Windows.Forms.Label afr_lblStatus;
    }
}
