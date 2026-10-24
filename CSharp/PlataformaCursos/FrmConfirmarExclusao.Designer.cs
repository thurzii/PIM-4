/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmConfirmarExclusao.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER da tela de confirmacao de exclusao.
   ===================================================================== */

namespace PlataformaCursos
{
    partial class FrmConfirmarExclusao
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
            this.afr_lblMotivo = new System.Windows.Forms.Label();
            this.afr_lblDetalhesTitulo = new System.Windows.Forms.Label();
            this.afr_txtDetalhes = new System.Windows.Forms.TextBox();
            this.afr_lblInstrucao = new System.Windows.Forms.Label();
            this.afr_lblSenha = new System.Windows.Forms.Label();
            this.afr_txtSenha = new System.Windows.Forms.TextBox();
            this.afr_btnConfirmar = new System.Windows.Forms.Button();
            this.afr_btnCancelar = new System.Windows.Forms.Button();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // afr_lblTitulo
            //
            this.afr_lblTitulo.AutoSize = true;
            this.afr_lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.afr_lblTitulo.Location = new System.Drawing.Point(20, 16);
            this.afr_lblTitulo.Name = "afr_lblTitulo";
            this.afr_lblTitulo.Size = new System.Drawing.Size(220, 25);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "Confirmar exclusao";
            //
            // afr_lblMotivo
            //
            this.afr_lblMotivo.ForeColor = System.Drawing.Color.Firebrick;
            this.afr_lblMotivo.Location = new System.Drawing.Point(22, 48);
            this.afr_lblMotivo.Name = "afr_lblMotivo";
            this.afr_lblMotivo.Size = new System.Drawing.Size(436, 46);
            this.afr_lblMotivo.TabIndex = 1;
            this.afr_lblMotivo.Text = "";
            //
            // afr_lblDetalhesTitulo
            //
            this.afr_lblDetalhesTitulo.AutoSize = true;
            this.afr_lblDetalhesTitulo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.afr_lblDetalhesTitulo.Location = new System.Drawing.Point(22, 100);
            this.afr_lblDetalhesTitulo.Name = "afr_lblDetalhesTitulo";
            this.afr_lblDetalhesTitulo.Size = new System.Drawing.Size(150, 15);
            this.afr_lblDetalhesTitulo.TabIndex = 2;
            this.afr_lblDetalhesTitulo.Text = "O que sera apagado:";
            //
            // afr_txtDetalhes
            //
            this.afr_txtDetalhes.BackColor = System.Drawing.Color.White;
            this.afr_txtDetalhes.Font = new System.Drawing.Font("Consolas", 9F);
            this.afr_txtDetalhes.Location = new System.Drawing.Point(22, 120);
            this.afr_txtDetalhes.Multiline = true;
            this.afr_txtDetalhes.Name = "afr_txtDetalhes";
            this.afr_txtDetalhes.ReadOnly = true;
            this.afr_txtDetalhes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.afr_txtDetalhes.Size = new System.Drawing.Size(436, 118);
            this.afr_txtDetalhes.TabIndex = 3;
            //
            // afr_lblInstrucao
            //
            this.afr_lblInstrucao.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblInstrucao.Location = new System.Drawing.Point(22, 248);
            this.afr_lblInstrucao.Name = "afr_lblInstrucao";
            this.afr_lblInstrucao.Size = new System.Drawing.Size(436, 34);
            this.afr_lblInstrucao.TabIndex = 4;
            this.afr_lblInstrucao.Text = "Esta operacao nao tem volta. Para confirmar, digite a sua senha de acesso.";
            //
            // afr_lblSenha
            //
            this.afr_lblSenha.AutoSize = true;
            this.afr_lblSenha.Location = new System.Drawing.Point(22, 288);
            this.afr_lblSenha.Name = "afr_lblSenha";
            this.afr_lblSenha.Size = new System.Drawing.Size(100, 15);
            this.afr_lblSenha.TabIndex = 5;
            this.afr_lblSenha.Text = "Sua senha:";
            //
            // afr_txtSenha
            //
            this.afr_txtSenha.Location = new System.Drawing.Point(22, 308);
            this.afr_txtSenha.MaxLength = 100;
            this.afr_txtSenha.Name = "afr_txtSenha";
            this.afr_txtSenha.PasswordChar = '*';
            this.afr_txtSenha.Size = new System.Drawing.Size(436, 23);
            this.afr_txtSenha.TabIndex = 6;
            //
            // afr_btnConfirmar
            //
            this.afr_btnConfirmar.BackColor = System.Drawing.Color.MistyRose;
            this.afr_btnConfirmar.Location = new System.Drawing.Point(22, 346);
            this.afr_btnConfirmar.Name = "afr_btnConfirmar";
            this.afr_btnConfirmar.Size = new System.Drawing.Size(240, 40);
            this.afr_btnConfirmar.TabIndex = 7;
            this.afr_btnConfirmar.Text = "Confirmar e excluir";
            this.afr_btnConfirmar.UseVisualStyleBackColor = false;
            this.afr_btnConfirmar.Click += new System.EventHandler(this.afr_btnConfirmar_Click);
            //
            // afr_btnCancelar
            //
            this.afr_btnCancelar.Location = new System.Drawing.Point(272, 346);
            this.afr_btnCancelar.Name = "afr_btnCancelar";
            this.afr_btnCancelar.Size = new System.Drawing.Size(186, 40);
            this.afr_btnCancelar.TabIndex = 8;
            this.afr_btnCancelar.Text = "Cancelar";
            this.afr_btnCancelar.UseVisualStyleBackColor = true;
            this.afr_btnCancelar.Click += new System.EventHandler(this.afr_btnCancelar_Click);
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(22, 394);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(436, 20);
            this.afr_lblStatus.TabIndex = 9;
            this.afr_lblStatus.Text = "";
            //
            // FrmConfirmarExclusao
            //
            this.AcceptButton = this.afr_btnConfirmar;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.afr_btnCancelar;
            this.ClientSize = new System.Drawing.Size(480, 424);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblMotivo);
            this.Controls.Add(this.afr_lblDetalhesTitulo);
            this.Controls.Add(this.afr_txtDetalhes);
            this.Controls.Add(this.afr_lblInstrucao);
            this.Controls.Add(this.afr_lblSenha);
            this.Controls.Add(this.afr_txtSenha);
            this.Controls.Add(this.afr_btnConfirmar);
            this.Controls.Add(this.afr_btnCancelar);
            this.Controls.Add(this.afr_lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmConfirmarExclusao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Confirmar exclusao";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblTitulo;
        private System.Windows.Forms.Label afr_lblMotivo;
        private System.Windows.Forms.Label afr_lblDetalhesTitulo;
        private System.Windows.Forms.TextBox afr_txtDetalhes;
        private System.Windows.Forms.Label afr_lblInstrucao;
        private System.Windows.Forms.Label afr_lblSenha;
        private System.Windows.Forms.TextBox afr_txtSenha;
        private System.Windows.Forms.Button afr_btnConfirmar;
        private System.Windows.Forms.Button afr_btnCancelar;
        private System.Windows.Forms.Label afr_lblStatus;
    }
}
