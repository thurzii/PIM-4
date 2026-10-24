/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmPerfilUsuario.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER da tela de perfil do usuario.
   ===================================================================== */

namespace PlataformaCursos
{
    partial class FrmPerfilUsuario
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
            this.afr_lblNome = new System.Windows.Forms.Label();
            this.afr_lblEmail = new System.Windows.Forms.Label();
            this.afr_lblPapel = new System.Windows.Forms.Label();
            this.afr_lblCadastro = new System.Windows.Forms.Label();
            this.afr_lblHistoricoTitulo = new System.Windows.Forms.Label();
            this.afr_lblLoginTitulo = new System.Windows.Forms.Label();
            this.afr_lstLogin = new System.Windows.Forms.ListView();
            this.afr_colLoginData = new System.Windows.Forms.ColumnHeader();
            this.afr_colLoginDesc = new System.Windows.Forms.ColumnHeader();
            this.afr_lblCrudTitulo = new System.Windows.Forms.Label();
            this.afr_lstCrud = new System.Windows.Forms.ListView();
            this.afr_colCrudData = new System.Windows.Forms.ColumnHeader();
            this.afr_colCrudDesc = new System.Windows.Forms.ColumnHeader();
            this.afr_lblCursosTitulo = new System.Windows.Forms.Label();
            this.afr_lstCursos = new System.Windows.Forms.ListView();
            this.afr_colCursoData = new System.Windows.Forms.ColumnHeader();
            this.afr_colCursoDesc = new System.Windows.Forms.ColumnHeader();
            this.afr_lblResumo = new System.Windows.Forms.Label();
            this.afr_btnFechar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // afr_lblTitulo
            //
            this.afr_lblTitulo.AutoSize = true;
            this.afr_lblTitulo.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
            this.afr_lblTitulo.Location = new System.Drawing.Point(20, 14);
            this.afr_lblTitulo.Name = "afr_lblTitulo";
            this.afr_lblTitulo.Size = new System.Drawing.Size(220, 25);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "Perfil do usuario";
            //
            // afr_lblNome
            //
            this.afr_lblNome.AutoSize = true;
            this.afr_lblNome.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.afr_lblNome.Location = new System.Drawing.Point(20, 48);
            this.afr_lblNome.Name = "afr_lblNome";
            this.afr_lblNome.Size = new System.Drawing.Size(120, 19);
            this.afr_lblNome.TabIndex = 1;
            this.afr_lblNome.Text = "Nome";
            //
            // afr_lblEmail
            //
            this.afr_lblEmail.AutoSize = true;
            this.afr_lblEmail.Location = new System.Drawing.Point(20, 74);
            this.afr_lblEmail.Name = "afr_lblEmail";
            this.afr_lblEmail.Size = new System.Drawing.Size(120, 15);
            this.afr_lblEmail.TabIndex = 2;
            this.afr_lblEmail.Text = "Email";
            //
            // afr_lblPapel
            //
            this.afr_lblPapel.AutoSize = true;
            this.afr_lblPapel.Location = new System.Drawing.Point(20, 96);
            this.afr_lblPapel.Name = "afr_lblPapel";
            this.afr_lblPapel.Size = new System.Drawing.Size(120, 15);
            this.afr_lblPapel.TabIndex = 3;
            this.afr_lblPapel.Text = "Papel";
            //
            // afr_lblCadastro
            //
            this.afr_lblCadastro.AutoSize = true;
            this.afr_lblCadastro.Location = new System.Drawing.Point(20, 118);
            this.afr_lblCadastro.Name = "afr_lblCadastro";
            this.afr_lblCadastro.Size = new System.Drawing.Size(120, 15);
            this.afr_lblCadastro.TabIndex = 4;
            this.afr_lblCadastro.Text = "Data de cadastro";
            //
            // afr_lblHistoricoTitulo
            //
            this.afr_lblHistoricoTitulo.AutoSize = true;
            this.afr_lblHistoricoTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.afr_lblHistoricoTitulo.Location = new System.Drawing.Point(20, 150);
            this.afr_lblHistoricoTitulo.Name = "afr_lblHistoricoTitulo";
            this.afr_lblHistoricoTitulo.Size = new System.Drawing.Size(180, 19);
            this.afr_lblHistoricoTitulo.TabIndex = 5;
            this.afr_lblHistoricoTitulo.Text = "Historico de acoes";
            //
            // afr_lblLoginTitulo
            //
            this.afr_lblLoginTitulo.AutoSize = true;
            this.afr_lblLoginTitulo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblLoginTitulo.Location = new System.Drawing.Point(20, 176);
            this.afr_lblLoginTitulo.Name = "afr_lblLoginTitulo";
            this.afr_lblLoginTitulo.Size = new System.Drawing.Size(150, 15);
            this.afr_lblLoginTitulo.TabIndex = 6;
            this.afr_lblLoginTitulo.Text = "Acessos (loginLog)";
            //
            // afr_lstLogin
            //
            this.afr_lstLogin.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.afr_colLoginData,
            this.afr_colLoginDesc});
            this.afr_lstLogin.FullRowSelect = true;
            this.afr_lstLogin.GridLines = true;
            this.afr_lstLogin.Location = new System.Drawing.Point(20, 196);
            this.afr_lstLogin.MultiSelect = false;
            this.afr_lstLogin.Name = "afr_lstLogin";
            this.afr_lstLogin.Size = new System.Drawing.Size(320, 150);
            this.afr_lstLogin.TabIndex = 7;
            this.afr_lstLogin.UseCompatibleStateImageBehavior = false;
            this.afr_lstLogin.View = System.Windows.Forms.View.Details;
            //
            // afr_colLoginData
            //
            this.afr_colLoginData.Text = "Data e hora";
            this.afr_colLoginData.Width = 125;
            //
            // afr_colLoginDesc
            //
            this.afr_colLoginDesc.Text = "Acao";
            this.afr_colLoginDesc.Width = 170;
            //
            // afr_lblCrudTitulo
            //
            this.afr_lblCrudTitulo.AutoSize = true;
            this.afr_lblCrudTitulo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblCrudTitulo.Location = new System.Drawing.Point(360, 176);
            this.afr_lblCrudTitulo.Name = "afr_lblCrudTitulo";
            this.afr_lblCrudTitulo.Size = new System.Drawing.Size(170, 15);
            this.afr_lblCrudTitulo.TabIndex = 8;
            this.afr_lblCrudTitulo.Text = "Cadastros (acoesCrud)";
            //
            // afr_lstCrud
            //
            this.afr_lstCrud.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.afr_colCrudData,
            this.afr_colCrudDesc});
            this.afr_lstCrud.FullRowSelect = true;
            this.afr_lstCrud.GridLines = true;
            this.afr_lstCrud.Location = new System.Drawing.Point(360, 196);
            this.afr_lstCrud.MultiSelect = false;
            this.afr_lstCrud.Name = "afr_lstCrud";
            this.afr_lstCrud.Size = new System.Drawing.Size(320, 150);
            this.afr_lstCrud.TabIndex = 9;
            this.afr_lstCrud.UseCompatibleStateImageBehavior = false;
            this.afr_lstCrud.View = System.Windows.Forms.View.Details;
            //
            // afr_colCrudData
            //
            this.afr_colCrudData.Text = "Data e hora";
            this.afr_colCrudData.Width = 125;
            //
            // afr_colCrudDesc
            //
            this.afr_colCrudDesc.Text = "Acao";
            this.afr_colCrudDesc.Width = 170;
            //
            // afr_lblCursosTitulo
            //
            this.afr_lblCursosTitulo.AutoSize = true;
            this.afr_lblCursosTitulo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblCursosTitulo.Location = new System.Drawing.Point(20, 358);
            this.afr_lblCursosTitulo.Name = "afr_lblCursosTitulo";
            this.afr_lblCursosTitulo.Size = new System.Drawing.Size(220, 15);
            this.afr_lblCursosTitulo.TabIndex = 10;
            this.afr_lblCursosTitulo.Text = "Cursos relacionados (cursosRelacionados)";
            //
            // afr_lstCursos
            //
            this.afr_lstCursos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.afr_colCursoData,
            this.afr_colCursoDesc});
            this.afr_lstCursos.FullRowSelect = true;
            this.afr_lstCursos.GridLines = true;
            this.afr_lstCursos.Location = new System.Drawing.Point(20, 378);
            this.afr_lstCursos.MultiSelect = false;
            this.afr_lstCursos.Name = "afr_lstCursos";
            this.afr_lstCursos.Size = new System.Drawing.Size(660, 150);
            this.afr_lstCursos.TabIndex = 11;
            this.afr_lstCursos.UseCompatibleStateImageBehavior = false;
            this.afr_lstCursos.View = System.Windows.Forms.View.Details;
            //
            // afr_colCursoData
            //
            this.afr_colCursoData.Text = "Data e hora";
            this.afr_colCursoData.Width = 140;
            //
            // afr_colCursoDesc
            //
            this.afr_colCursoDesc.Text = "Acao no curso";
            this.afr_colCursoDesc.Width = 490;
            //
            // afr_lblResumo
            //
            this.afr_lblResumo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblResumo.Location = new System.Drawing.Point(20, 538);
            this.afr_lblResumo.Name = "afr_lblResumo";
            this.afr_lblResumo.Size = new System.Drawing.Size(660, 36);
            this.afr_lblResumo.TabIndex = 12;
            this.afr_lblResumo.Text = "";
            //
            // afr_btnFechar
            //
            this.afr_btnFechar.Location = new System.Drawing.Point(500, 578);
            this.afr_btnFechar.Name = "afr_btnFechar";
            this.afr_btnFechar.Size = new System.Drawing.Size(180, 40);
            this.afr_btnFechar.TabIndex = 13;
            this.afr_btnFechar.Text = "Fechar";
            this.afr_btnFechar.UseVisualStyleBackColor = true;
            this.afr_btnFechar.Click += new System.EventHandler(this.afr_btnFechar_Click);
            //
            // FrmPerfilUsuario
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.afr_btnFechar;
            this.ClientSize = new System.Drawing.Size(700, 634);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblNome);
            this.Controls.Add(this.afr_lblEmail);
            this.Controls.Add(this.afr_lblPapel);
            this.Controls.Add(this.afr_lblCadastro);
            this.Controls.Add(this.afr_lblHistoricoTitulo);
            this.Controls.Add(this.afr_lblLoginTitulo);
            this.Controls.Add(this.afr_lstLogin);
            this.Controls.Add(this.afr_lblCrudTitulo);
            this.Controls.Add(this.afr_lstCrud);
            this.Controls.Add(this.afr_lblCursosTitulo);
            this.Controls.Add(this.afr_lstCursos);
            this.Controls.Add(this.afr_lblResumo);
            this.Controls.Add(this.afr_btnFechar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmPerfilUsuario";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Perfil do usuario";
            this.Load += new System.EventHandler(this.FrmPerfilUsuario_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblTitulo;
        private System.Windows.Forms.Label afr_lblNome;
        private System.Windows.Forms.Label afr_lblEmail;
        private System.Windows.Forms.Label afr_lblPapel;
        private System.Windows.Forms.Label afr_lblCadastro;
        private System.Windows.Forms.Label afr_lblHistoricoTitulo;
        private System.Windows.Forms.Label afr_lblLoginTitulo;
        private System.Windows.Forms.ListView afr_lstLogin;
        private System.Windows.Forms.ColumnHeader afr_colLoginData;
        private System.Windows.Forms.ColumnHeader afr_colLoginDesc;
        private System.Windows.Forms.Label afr_lblCrudTitulo;
        private System.Windows.Forms.ListView afr_lstCrud;
        private System.Windows.Forms.ColumnHeader afr_colCrudData;
        private System.Windows.Forms.ColumnHeader afr_colCrudDesc;
        private System.Windows.Forms.Label afr_lblCursosTitulo;
        private System.Windows.Forms.ListView afr_lstCursos;
        private System.Windows.Forms.ColumnHeader afr_colCursoData;
        private System.Windows.Forms.ColumnHeader afr_colCursoDesc;
        private System.Windows.Forms.Label afr_lblResumo;
        private System.Windows.Forms.Button afr_btnFechar;
    }
}
