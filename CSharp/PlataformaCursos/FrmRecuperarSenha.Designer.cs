/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmRecuperarSenha.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER da tela de recuperacao de senha (DC02 / DS02).
   A tela tem tres etapas: enviar o codigo, conferir o codigo e
   redefinir a senha.
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
            this.afr_lblEtapa = new System.Windows.Forms.Label();
            this.afr_lblInfo = new System.Windows.Forms.Label();
            this.afr_lblEmail = new System.Windows.Forms.Label();
            this.afr_txtEmail = new System.Windows.Forms.TextBox();
            this.afr_btnEnviar = new System.Windows.Forms.Button();
            this.afr_btnAbrirCaixa = new System.Windows.Forms.Button();
            this.afr_lblCodigo = new System.Windows.Forms.Label();
            this.afr_txtCodigo = new System.Windows.Forms.TextBox();
            this.afr_btnConferir = new System.Windows.Forms.Button();
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
            this.afr_lblTitulo.Location = new System.Drawing.Point(30, 18);
            this.afr_lblTitulo.Name = "afr_lblTitulo";
            this.afr_lblTitulo.Size = new System.Drawing.Size(180, 25);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "Recuperar senha";
            //
            // afr_lblEtapa
            //
            this.afr_lblEtapa.AutoSize = true;
            this.afr_lblEtapa.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.afr_lblEtapa.ForeColor = System.Drawing.Color.SteelBlue;
            this.afr_lblEtapa.Location = new System.Drawing.Point(32, 48);
            this.afr_lblEtapa.Name = "afr_lblEtapa";
            this.afr_lblEtapa.Size = new System.Drawing.Size(250, 15);
            this.afr_lblEtapa.TabIndex = 1;
            this.afr_lblEtapa.Text = "Etapa 1 de 3 - informar o email";
            //
            // afr_lblInfo
            //
            this.afr_lblInfo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblInfo.Location = new System.Drawing.Point(32, 68);
            this.afr_lblInfo.Name = "afr_lblInfo";
            this.afr_lblInfo.Size = new System.Drawing.Size(376, 34);
            this.afr_lblInfo.TabIndex = 2;
            this.afr_lblInfo.Text = "Informe o email cadastrado. O sistema envia um codigo de 6 digitos.";
            //
            // afr_lblEmail
            //
            this.afr_lblEmail.AutoSize = true;
            this.afr_lblEmail.Location = new System.Drawing.Point(32, 110);
            this.afr_lblEmail.Name = "afr_lblEmail";
            this.afr_lblEmail.Size = new System.Drawing.Size(40, 15);
            this.afr_lblEmail.TabIndex = 3;
            this.afr_lblEmail.Text = "Email:";
            //
            // afr_txtEmail
            //
            this.afr_txtEmail.Location = new System.Drawing.Point(32, 129);
            this.afr_txtEmail.MaxLength = 100;
            this.afr_txtEmail.Name = "afr_txtEmail";
            this.afr_txtEmail.Size = new System.Drawing.Size(376, 23);
            this.afr_txtEmail.TabIndex = 4;
            //
            // afr_btnEnviar
            //
            this.afr_btnEnviar.BackColor = System.Drawing.Color.AliceBlue;
            this.afr_btnEnviar.Location = new System.Drawing.Point(32, 158);
            this.afr_btnEnviar.Name = "afr_btnEnviar";
            this.afr_btnEnviar.Size = new System.Drawing.Size(230, 32);
            this.afr_btnEnviar.TabIndex = 5;
            this.afr_btnEnviar.Text = "1) Enviar codigo";
            this.afr_btnEnviar.UseVisualStyleBackColor = false;
            this.afr_btnEnviar.Click += new System.EventHandler(this.afr_btnEnviar_Click);
            //
            // afr_btnAbrirCaixa
            //
            this.afr_btnAbrirCaixa.Location = new System.Drawing.Point(270, 158);
            this.afr_btnAbrirCaixa.Name = "afr_btnAbrirCaixa";
            this.afr_btnAbrirCaixa.Size = new System.Drawing.Size(138, 32);
            this.afr_btnAbrirCaixa.TabIndex = 6;
            this.afr_btnAbrirCaixa.Text = "Ver o codigo";
            this.afr_btnAbrirCaixa.UseVisualStyleBackColor = true;
            this.afr_btnAbrirCaixa.Click += new System.EventHandler(this.afr_btnAbrirCaixa_Click);
            //
            // afr_lblCodigo
            //
            this.afr_lblCodigo.AutoSize = true;
            this.afr_lblCodigo.Location = new System.Drawing.Point(32, 202);
            this.afr_lblCodigo.Name = "afr_lblCodigo";
            this.afr_lblCodigo.Size = new System.Drawing.Size(140, 15);
            this.afr_lblCodigo.TabIndex = 7;
            this.afr_lblCodigo.Text = "Codigo de 6 digitos:";
            //
            // afr_txtCodigo
            //
            this.afr_txtCodigo.Enabled = false;
            this.afr_txtCodigo.Location = new System.Drawing.Point(32, 221);
            this.afr_txtCodigo.MaxLength = 6;
            this.afr_txtCodigo.Name = "afr_txtCodigo";
            this.afr_txtCodigo.Size = new System.Drawing.Size(180, 23);
            this.afr_txtCodigo.TabIndex = 8;
            //
            // afr_btnConferir
            //
            this.afr_btnConferir.Enabled = false;
            this.afr_btnConferir.Location = new System.Drawing.Point(222, 220);
            this.afr_btnConferir.Name = "afr_btnConferir";
            this.afr_btnConferir.Size = new System.Drawing.Size(186, 25);
            this.afr_btnConferir.TabIndex = 9;
            this.afr_btnConferir.Text = "2) Conferir codigo";
            this.afr_btnConferir.UseVisualStyleBackColor = true;
            this.afr_btnConferir.Click += new System.EventHandler(this.afr_btnConferir_Click);
            //
            // afr_lblNova
            //
            this.afr_lblNova.AutoSize = true;
            this.afr_lblNova.Location = new System.Drawing.Point(32, 258);
            this.afr_lblNova.Name = "afr_lblNova";
            this.afr_lblNova.Size = new System.Drawing.Size(70, 15);
            this.afr_lblNova.TabIndex = 10;
            this.afr_lblNova.Text = "Nova senha:";
            //
            // afr_txtNova
            //
            this.afr_txtNova.Enabled = false;
            this.afr_txtNova.Location = new System.Drawing.Point(32, 277);
            this.afr_txtNova.MaxLength = 100;
            this.afr_txtNova.Name = "afr_txtNova";
            this.afr_txtNova.PasswordChar = '*';
            this.afr_txtNova.Size = new System.Drawing.Size(376, 23);
            this.afr_txtNova.TabIndex = 11;
            //
            // afr_lblConfirma
            //
            this.afr_lblConfirma.AutoSize = true;
            this.afr_lblConfirma.Location = new System.Drawing.Point(32, 308);
            this.afr_lblConfirma.Name = "afr_lblConfirma";
            this.afr_lblConfirma.Size = new System.Drawing.Size(140, 15);
            this.afr_lblConfirma.TabIndex = 12;
            this.afr_lblConfirma.Text = "Confirmar nova senha:";
            //
            // afr_txtConfirma
            //
            this.afr_txtConfirma.Enabled = false;
            this.afr_txtConfirma.Location = new System.Drawing.Point(32, 327);
            this.afr_txtConfirma.MaxLength = 100;
            this.afr_txtConfirma.Name = "afr_txtConfirma";
            this.afr_txtConfirma.PasswordChar = '*';
            this.afr_txtConfirma.Size = new System.Drawing.Size(376, 23);
            this.afr_txtConfirma.TabIndex = 13;
            //
            // afr_btnRedefinir
            //
            this.afr_btnRedefinir.BackColor = System.Drawing.Color.Honeydew;
            this.afr_btnRedefinir.Enabled = false;
            this.afr_btnRedefinir.Location = new System.Drawing.Point(32, 360);
            this.afr_btnRedefinir.Name = "afr_btnRedefinir";
            this.afr_btnRedefinir.Size = new System.Drawing.Size(230, 38);
            this.afr_btnRedefinir.TabIndex = 14;
            this.afr_btnRedefinir.Text = "3) Redefinir senha";
            this.afr_btnRedefinir.UseVisualStyleBackColor = false;
            this.afr_btnRedefinir.Click += new System.EventHandler(this.afr_btnRedefinir_Click);
            //
            // afr_btnCancelar
            //
            this.afr_btnCancelar.Location = new System.Drawing.Point(270, 360);
            this.afr_btnCancelar.Name = "afr_btnCancelar";
            this.afr_btnCancelar.Size = new System.Drawing.Size(138, 38);
            this.afr_btnCancelar.TabIndex = 15;
            this.afr_btnCancelar.Text = "Cancelar";
            this.afr_btnCancelar.UseVisualStyleBackColor = true;
            this.afr_btnCancelar.Click += new System.EventHandler(this.afr_btnCancelar_Click);
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(32, 406);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(376, 44);
            this.afr_lblStatus.TabIndex = 16;
            this.afr_lblStatus.Text = "";
            //
            // FrmRecuperarSenha
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.afr_btnCancelar;
            this.ClientSize = new System.Drawing.Size(440, 464);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblEtapa);
            this.Controls.Add(this.afr_lblInfo);
            this.Controls.Add(this.afr_lblEmail);
            this.Controls.Add(this.afr_txtEmail);
            this.Controls.Add(this.afr_btnEnviar);
            this.Controls.Add(this.afr_btnAbrirCaixa);
            this.Controls.Add(this.afr_lblCodigo);
            this.Controls.Add(this.afr_txtCodigo);
            this.Controls.Add(this.afr_btnConferir);
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
        private System.Windows.Forms.Label afr_lblEtapa;
        private System.Windows.Forms.Label afr_lblInfo;
        private System.Windows.Forms.Label afr_lblEmail;
        private System.Windows.Forms.TextBox afr_txtEmail;
        private System.Windows.Forms.Button afr_btnEnviar;
        private System.Windows.Forms.Button afr_btnAbrirCaixa;
        private System.Windows.Forms.Label afr_lblCodigo;
        private System.Windows.Forms.TextBox afr_txtCodigo;
        private System.Windows.Forms.Button afr_btnConferir;
        private System.Windows.Forms.Label afr_lblNova;
        private System.Windows.Forms.TextBox afr_txtNova;
        private System.Windows.Forms.Label afr_lblConfirma;
        private System.Windows.Forms.TextBox afr_txtConfirma;
        private System.Windows.Forms.Button afr_btnRedefinir;
        private System.Windows.Forms.Button afr_btnCancelar;
        private System.Windows.Forms.Label afr_lblStatus;
    }
}
