/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmRecuperarSenha.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER da tela de recuperacao de senha (DC02).
   ===================================================================== */

namespace PlataformaCursos
{
    partial class FrmRecuperarSenha
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
            this.afr_lblInfo = new System.Windows.Forms.Label();
            this.afr_lblEmail = new System.Windows.Forms.Label();
            this.afr_txtEmail = new System.Windows.Forms.TextBox();
            this.afr_lblNova = new System.Windows.Forms.Label();
            this.afr_txtNova = new System.Windows.Forms.TextBox();
            this.afr_lblConfirma = new System.Windows.Forms.Label();
            this.afr_txtConfirma = new System.Windows.Forms.TextBox();
            this.afr_btnRedefinir = new System.Windows.Forms.Button();
            this.afr_btnCancelar = new System.Windows.Forms.Button();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // afr_lblTitulo
            //
            this.afr_lblTitulo.AutoSize = true;
            this.afr_lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.afr_lblTitulo.Location = new System.Drawing.Point(30, 22);
            this.afr_lblTitulo.Name = "afr_lblTitulo";
            this.afr_lblTitulo.Size = new System.Drawing.Size(180, 25);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "Recuperar senha";
            //
            // afr_lblInfo
            //
            this.afr_lblInfo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblInfo.Location = new System.Drawing.Point(32, 52);
            this.afr_lblInfo.Name = "afr_lblInfo";
            this.afr_lblInfo.Size = new System.Drawing.Size(336, 34);
            this.afr_lblInfo.TabIndex = 1;
            this.afr_lblInfo.Text = "Informe o email cadastrado e escolha uma nova senha de pelo menos 6 caracteres.";
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
            // afr_lblNova
            //
            this.afr_lblNova.AutoSize = true;
            this.afr_lblNova.Location = new System.Drawing.Point(32, 148);
            this.afr_lblNova.Name = "afr_lblNova";
            this.afr_lblNova.Size = new System.Drawing.Size(70, 15);
            this.afr_lblNova.TabIndex = 4;
            this.afr_lblNova.Text = "Nova senha:";
            //
            // afr_txtNova
            //
            this.afr_txtNova.Location = new System.Drawing.Point(32, 167);
            this.afr_txtNova.MaxLength = 100;
            this.afr_txtNova.Name = "afr_txtNova";
            this.afr_txtNova.PasswordChar = '*';
            this.afr_txtNova.Size = new System.Drawing.Size(336, 23);
            this.afr_txtNova.TabIndex = 5;
            //
            // afr_lblConfirma
            //
            this.afr_lblConfirma.AutoSize = true;
            this.afr_lblConfirma.Location = new System.Drawing.Point(32, 201);
            this.afr_lblConfirma.Name = "afr_lblConfirma";
            this.afr_lblConfirma.Size = new System.Drawing.Size(120, 15);
            this.afr_lblConfirma.TabIndex = 6;
            this.afr_lblConfirma.Text = "Confirmar nova senha:";
            //
            // afr_txtConfirma
            //
            this.afr_txtConfirma.Location = new System.Drawing.Point(32, 220);
            this.afr_txtConfirma.MaxLength = 100;
            this.afr_txtConfirma.Name = "afr_txtConfirma";
            this.afr_txtConfirma.PasswordChar = '*';
            this.afr_txtConfirma.Size = new System.Drawing.Size(336, 23);
            this.afr_txtConfirma.TabIndex = 7;
            //
            // afr_btnRedefinir
            //
            this.afr_btnRedefinir.BackColor = System.Drawing.Color.Honeydew;
            this.afr_btnRedefinir.Location = new System.Drawing.Point(32, 258);
            this.afr_btnRedefinir.Name = "afr_btnRedefinir";
            this.afr_btnRedefinir.Size = new System.Drawing.Size(200, 38);
            this.afr_btnRedefinir.TabIndex = 8;
            this.afr_btnRedefinir.Text = "Redefinir senha";
            this.afr_btnRedefinir.UseVisualStyleBackColor = false;
            this.afr_btnRedefinir.Click += new System.EventHandler(this.afr_btnRedefinir_Click);
            //
            // afr_btnCancelar
            //
            this.afr_btnCancelar.Location = new System.Drawing.Point(242, 258);
            this.afr_btnCancelar.Name = "afr_btnCancelar";
            this.afr_btnCancelar.Size = new System.Drawing.Size(126, 38);
            this.afr_btnCancelar.TabIndex = 9;
            this.afr_btnCancelar.Text = "Cancelar";
            this.afr_btnCancelar.UseVisualStyleBackColor = true;
            this.afr_btnCancelar.Click += new System.EventHandler(this.afr_btnCancelar_Click);
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(32, 306);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(336, 34);
            this.afr_lblStatus.TabIndex = 10;
            this.afr_lblStatus.Text = "";
            //
            // FrmRecuperarSenha
            //
            this.AcceptButton = this.afr_btnRedefinir;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.afr_btnCancelar;
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblInfo);
            this.Controls.Add(this.afr_lblEmail);
            this.Controls.Add(this.afr_txtEmail);
            this.Controls.Add(this.afr_lblNova);
            this.Controls.Add(this.afr_txtNova);
            this.Controls.Add(this.afr_lblConfirma);
            this.Controls.Add(this.afr_txtConfirma);
            this.Controls.Add(this.afr_btnRedefinir);
            this.Controls.Add(this.afr_btnCancelar);
            this.Controls.Add(this.afr_lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmRecuperarSenha";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Recuperar senha";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblTitulo;
        private System.Windows.Forms.Label afr_lblInfo;
        private System.Windows.Forms.Label afr_lblEmail;
        private System.Windows.Forms.TextBox afr_txtEmail;
        private System.Windows.Forms.Label afr_lblNova;
        private System.Windows.Forms.TextBox afr_txtNova;
        private System.Windows.Forms.Label afr_lblConfirma;
        private System.Windows.Forms.TextBox afr_txtConfirma;
        private System.Windows.Forms.Button afr_btnRedefinir;
        private System.Windows.Forms.Button afr_btnCancelar;
        private System.Windows.Forms.Label afr_lblStatus;
    }
}
