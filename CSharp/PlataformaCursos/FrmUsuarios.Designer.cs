/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmUsuarios.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER da tela de gerenciamento de usuarios.
   ===================================================================== */

namespace PlataformaCursos
{
    partial class FrmUsuarios
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
            this.afr_grpDados = new System.Windows.Forms.GroupBox();
            this.afr_lblModo = new System.Windows.Forms.Label();
            this.afr_lblNome = new System.Windows.Forms.Label();
            this.afr_txtNome = new System.Windows.Forms.TextBox();
            this.afr_lblEmail = new System.Windows.Forms.Label();
            this.afr_txtEmail = new System.Windows.Forms.TextBox();
            this.afr_lblSenha = new System.Windows.Forms.Label();
            this.afr_txtSenha = new System.Windows.Forms.TextBox();
            this.afr_lblPerfil = new System.Windows.Forms.Label();
            this.afr_cmbPerfil = new System.Windows.Forms.ComboBox();
            this.afr_chkAtivo = new System.Windows.Forms.CheckBox();
            this.afr_btnNovo = new System.Windows.Forms.Button();
            this.afr_btnSalvar = new System.Windows.Forms.Button();
            this.afr_btnExcluir = new System.Windows.Forms.Button();
            this.afr_lblAviso = new System.Windows.Forms.Label();
            this.afr_lblBusca = new System.Windows.Forms.Label();
            this.afr_txtBusca = new System.Windows.Forms.TextBox();
            this.afr_btnBuscar = new System.Windows.Forms.Button();
            this.afr_lstUsuarios = new System.Windows.Forms.ListView();
            this.afr_colId = new System.Windows.Forms.ColumnHeader();
            this.afr_colNome = new System.Windows.Forms.ColumnHeader();
            this.afr_colEmail = new System.Windows.Forms.ColumnHeader();
            this.afr_colPerfil = new System.Windows.Forms.ColumnHeader();
            this.afr_colAtivo = new System.Windows.Forms.ColumnHeader();
            this.afr_colCadastro = new System.Windows.Forms.ColumnHeader();
            this.afr_btnRecarregar = new System.Windows.Forms.Button();
            this.afr_btnVerPerfil = new System.Windows.Forms.Button();
            this.afr_btnFechar = new System.Windows.Forms.Button();
            this.afr_txtSaida = new System.Windows.Forms.TextBox();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.afr_grpDados.SuspendLayout();
            this.SuspendLayout();
            //
            // afr_lblTitulo
            //
            this.afr_lblTitulo.AutoSize = true;
            this.afr_lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.afr_lblTitulo.Location = new System.Drawing.Point(25, 18);
            this.afr_lblTitulo.Name = "afr_lblTitulo";
            this.afr_lblTitulo.Size = new System.Drawing.Size(260, 25);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "GERENCIAR USUARIOS";
            //
            // afr_grpDados
            //
            this.afr_grpDados.Controls.Add(this.afr_lblModo);
            this.afr_grpDados.Controls.Add(this.afr_lblNome);
            this.afr_grpDados.Controls.Add(this.afr_txtNome);
            this.afr_grpDados.Controls.Add(this.afr_lblEmail);
            this.afr_grpDados.Controls.Add(this.afr_txtEmail);
            this.afr_grpDados.Controls.Add(this.afr_lblSenha);
            this.afr_grpDados.Controls.Add(this.afr_txtSenha);
            this.afr_grpDados.Controls.Add(this.afr_lblPerfil);
            this.afr_grpDados.Controls.Add(this.afr_cmbPerfil);
            this.afr_grpDados.Controls.Add(this.afr_chkAtivo);
            this.afr_grpDados.Controls.Add(this.afr_btnNovo);
            this.afr_grpDados.Controls.Add(this.afr_btnSalvar);
            this.afr_grpDados.Controls.Add(this.afr_btnExcluir);
            this.afr_grpDados.Controls.Add(this.afr_lblAviso);
            this.afr_grpDados.Location = new System.Drawing.Point(25, 58);
            this.afr_grpDados.Name = "afr_grpDados";
            this.afr_grpDados.Size = new System.Drawing.Size(400, 312);
            this.afr_grpDados.TabIndex = 1;
            this.afr_grpDados.TabStop = false;
            this.afr_grpDados.Text = "Dados do Usuario";
            //
            // afr_lblModo
            //
            this.afr_lblModo.AutoSize = true;
            this.afr_lblModo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.afr_lblModo.Location = new System.Drawing.Point(20, 28);
            this.afr_lblModo.Name = "afr_lblModo";
            this.afr_lblModo.Size = new System.Drawing.Size(120, 15);
            this.afr_lblModo.TabIndex = 0;
            this.afr_lblModo.Text = "NOVO USUARIO";
            //
            // afr_lblNome
            //
            this.afr_lblNome.AutoSize = true;
            this.afr_lblNome.Location = new System.Drawing.Point(20, 62);
            this.afr_lblNome.Name = "afr_lblNome";
            this.afr_lblNome.Size = new System.Drawing.Size(45, 15);
            this.afr_lblNome.TabIndex = 1;
            this.afr_lblNome.Text = "Nome:";
            //
            // afr_txtNome
            //
            this.afr_txtNome.Location = new System.Drawing.Point(110, 59);
            this.afr_txtNome.MaxLength = 100;
            this.afr_txtNome.Name = "afr_txtNome";
            this.afr_txtNome.Size = new System.Drawing.Size(270, 23);
            this.afr_txtNome.TabIndex = 2;
            //
            // afr_lblEmail
            //
            this.afr_lblEmail.AutoSize = true;
            this.afr_lblEmail.Location = new System.Drawing.Point(20, 97);
            this.afr_lblEmail.Name = "afr_lblEmail";
            this.afr_lblEmail.Size = new System.Drawing.Size(40, 15);
            this.afr_lblEmail.TabIndex = 3;
            this.afr_lblEmail.Text = "Email:";
            //
            // afr_txtEmail
            //
            this.afr_txtEmail.Location = new System.Drawing.Point(110, 94);
            this.afr_txtEmail.MaxLength = 100;
            this.afr_txtEmail.Name = "afr_txtEmail";
            this.afr_txtEmail.Size = new System.Drawing.Size(270, 23);
            this.afr_txtEmail.TabIndex = 4;
            //
            // afr_lblSenha
            //
            this.afr_lblSenha.AutoSize = true;
            this.afr_lblSenha.Location = new System.Drawing.Point(20, 132);
            this.afr_lblSenha.Name = "afr_lblSenha";
            this.afr_lblSenha.Size = new System.Drawing.Size(44, 15);
            this.afr_lblSenha.TabIndex = 5;
            this.afr_lblSenha.Text = "Senha:";
            //
            // afr_txtSenha
            //
            this.afr_txtSenha.Location = new System.Drawing.Point(110, 129);
            this.afr_txtSenha.MaxLength = 100;
            this.afr_txtSenha.Name = "afr_txtSenha";
            this.afr_txtSenha.Size = new System.Drawing.Size(270, 23);
            this.afr_txtSenha.TabIndex = 6;
            //
            // afr_lblPerfil
            //
            this.afr_lblPerfil.AutoSize = true;
            this.afr_lblPerfil.Location = new System.Drawing.Point(20, 167);
            this.afr_lblPerfil.Name = "afr_lblPerfil";
            this.afr_lblPerfil.Size = new System.Drawing.Size(37, 15);
            this.afr_lblPerfil.TabIndex = 7;
            this.afr_lblPerfil.Text = "Perfil:";
            //
            // afr_cmbPerfil
            //
            this.afr_cmbPerfil.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            // Os tres perfis do modelo: Aluno, Tutor e Admin
            this.afr_cmbPerfil.Items.AddRange(new object[] {
            "ALUNO",
            "TUTOR",
            "ADMIN"});
            this.afr_cmbPerfil.Location = new System.Drawing.Point(110, 164);
            this.afr_cmbPerfil.Name = "afr_cmbPerfil";
            this.afr_cmbPerfil.Size = new System.Drawing.Size(140, 23);
            this.afr_cmbPerfil.TabIndex = 8;
            //
            // afr_chkAtivo
            //
            this.afr_chkAtivo.AutoSize = true;
            this.afr_chkAtivo.Checked = true;
            this.afr_chkAtivo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.afr_chkAtivo.Location = new System.Drawing.Point(270, 166);
            this.afr_chkAtivo.Name = "afr_chkAtivo";
            this.afr_chkAtivo.Size = new System.Drawing.Size(55, 19);
            this.afr_chkAtivo.TabIndex = 9;
            this.afr_chkAtivo.Text = "Ativo";
            this.afr_chkAtivo.UseVisualStyleBackColor = true;
            //
            // afr_btnNovo
            //
            this.afr_btnNovo.Location = new System.Drawing.Point(20, 205);
            this.afr_btnNovo.Name = "afr_btnNovo";
            this.afr_btnNovo.Size = new System.Drawing.Size(118, 40);
            this.afr_btnNovo.TabIndex = 10;
            this.afr_btnNovo.Text = "Novo";
            this.afr_btnNovo.UseVisualStyleBackColor = true;
            this.afr_btnNovo.Click += new System.EventHandler(this.afr_btnNovo_Click);
            //
            // afr_btnSalvar
            //
            this.afr_btnSalvar.BackColor = System.Drawing.Color.LightSteelBlue;
            this.afr_btnSalvar.Location = new System.Drawing.Point(148, 205);
            this.afr_btnSalvar.Name = "afr_btnSalvar";
            this.afr_btnSalvar.Size = new System.Drawing.Size(118, 40);
            this.afr_btnSalvar.TabIndex = 11;
            this.afr_btnSalvar.Text = "Salvar";
            this.afr_btnSalvar.UseVisualStyleBackColor = false;
            this.afr_btnSalvar.Click += new System.EventHandler(this.afr_btnSalvar_Click);
            //
            // afr_btnExcluir
            //
            this.afr_btnExcluir.BackColor = System.Drawing.Color.MistyRose;
            this.afr_btnExcluir.Location = new System.Drawing.Point(276, 205);
            this.afr_btnExcluir.Name = "afr_btnExcluir";
            this.afr_btnExcluir.Size = new System.Drawing.Size(104, 40);
            this.afr_btnExcluir.TabIndex = 12;
            this.afr_btnExcluir.Text = "Excluir";
            this.afr_btnExcluir.UseVisualStyleBackColor = false;
            this.afr_btnExcluir.Click += new System.EventHandler(this.afr_btnExcluir_Click);
            //
            // afr_lblAviso
            //
            this.afr_lblAviso.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblAviso.Location = new System.Drawing.Point(20, 255);
            this.afr_lblAviso.Name = "afr_lblAviso";
            this.afr_lblAviso.Size = new System.Drawing.Size(360, 45);
            this.afr_lblAviso.TabIndex = 13;
            this.afr_lblAviso.Text = "Clique em um usuario da lista para carregar os dados e poder alterar ou excluir.";
            //
            // afr_lblBusca
            //
            this.afr_lblBusca.AutoSize = true;
            this.afr_lblBusca.Location = new System.Drawing.Point(445, 66);
            this.afr_lblBusca.Name = "afr_lblBusca";
            this.afr_lblBusca.Size = new System.Drawing.Size(45, 15);
            this.afr_lblBusca.TabIndex = 2;
            this.afr_lblBusca.Text = "Buscar:";
            //
            // afr_txtBusca
            //
            this.afr_txtBusca.Location = new System.Drawing.Point(500, 62);
            this.afr_txtBusca.MaxLength = 100;
            this.afr_txtBusca.Name = "afr_txtBusca";
            this.afr_txtBusca.Size = new System.Drawing.Size(240, 23);
            this.afr_txtBusca.TabIndex = 3;
            //
            // afr_btnBuscar
            //
            this.afr_btnBuscar.Location = new System.Drawing.Point(750, 61);
            this.afr_btnBuscar.Name = "afr_btnBuscar";
            this.afr_btnBuscar.Size = new System.Drawing.Size(125, 26);
            this.afr_btnBuscar.TabIndex = 4;
            this.afr_btnBuscar.Text = "Buscar (SELECT)";
            this.afr_btnBuscar.UseVisualStyleBackColor = true;
            this.afr_btnBuscar.Click += new System.EventHandler(this.afr_btnBuscar_Click);
            //
            // afr_lstUsuarios
            //
            this.afr_lstUsuarios.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.afr_colId,
            this.afr_colNome,
            this.afr_colEmail,
            this.afr_colPerfil,
            this.afr_colAtivo,
            this.afr_colCadastro});
            this.afr_lstUsuarios.FullRowSelect = true;
            this.afr_lstUsuarios.GridLines = true;
            this.afr_lstUsuarios.Location = new System.Drawing.Point(445, 98);
            this.afr_lstUsuarios.MultiSelect = false;
            this.afr_lstUsuarios.Name = "afr_lstUsuarios";
            this.afr_lstUsuarios.Size = new System.Drawing.Size(430, 272);
            this.afr_lstUsuarios.TabIndex = 5;
            this.afr_lstUsuarios.UseCompatibleStateImageBehavior = false;
            this.afr_lstUsuarios.View = System.Windows.Forms.View.Details;
            this.afr_lstUsuarios.SelectedIndexChanged += new System.EventHandler(this.afr_lstUsuarios_SelectedIndexChanged);
            //
            // afr_colId
            //
            this.afr_colId.Text = "Id";
            this.afr_colId.Width = 40;
            //
            // afr_colNome
            //
            this.afr_colNome.Text = "Nome";
            this.afr_colNome.Width = 105;
            //
            // afr_colEmail
            //
            this.afr_colEmail.Text = "Email";
            this.afr_colEmail.Width = 110;
            //
            // afr_colPerfil
            //
            this.afr_colPerfil.Text = "Perfil";
            this.afr_colPerfil.Width = 50;
            //
            // afr_colAtivo
            //
            this.afr_colAtivo.Text = "Ativo";
            this.afr_colAtivo.Width = 45;
            //
            // afr_colCadastro
            //
            this.afr_colCadastro.Text = "Cadastro";
            this.afr_colCadastro.Width = 80;
            //
            // afr_btnRecarregar
            //
            this.afr_btnRecarregar.Location = new System.Drawing.Point(445, 380);
            this.afr_btnRecarregar.Name = "afr_btnRecarregar";
            this.afr_btnRecarregar.Size = new System.Drawing.Size(140, 35);
            this.afr_btnRecarregar.TabIndex = 6;
            this.afr_btnRecarregar.Text = "Recarregar lista";
            this.afr_btnRecarregar.UseVisualStyleBackColor = true;
            this.afr_btnRecarregar.Click += new System.EventHandler(this.afr_btnRecarregar_Click);
            //
            // afr_btnVerPerfil
            //
            this.afr_btnVerPerfil.BackColor = System.Drawing.Color.AliceBlue;
            this.afr_btnVerPerfil.Location = new System.Drawing.Point(595, 380);
            this.afr_btnVerPerfil.Name = "afr_btnVerPerfil";
            this.afr_btnVerPerfil.Size = new System.Drawing.Size(180, 35);
            this.afr_btnVerPerfil.TabIndex = 7;
            this.afr_btnVerPerfil.Text = "Ver perfil e historico";
            this.afr_btnVerPerfil.UseVisualStyleBackColor = false;
            this.afr_btnVerPerfil.Click += new System.EventHandler(this.afr_btnVerPerfil_Click);
            //
            // afr_btnFechar
            //
            this.afr_btnFechar.Location = new System.Drawing.Point(785, 380);
            this.afr_btnFechar.Name = "afr_btnFechar";
            this.afr_btnFechar.Size = new System.Drawing.Size(90, 35);
            this.afr_btnFechar.TabIndex = 8;
            this.afr_btnFechar.Text = "Fechar";
            this.afr_btnFechar.UseVisualStyleBackColor = true;
            this.afr_btnFechar.Click += new System.EventHandler(this.afr_btnFechar_Click);
            //
            // afr_txtSaida
            //
            this.afr_txtSaida.BackColor = System.Drawing.Color.White;
            this.afr_txtSaida.Font = new System.Drawing.Font("Consolas", 9F);
            this.afr_txtSaida.Location = new System.Drawing.Point(25, 428);
            this.afr_txtSaida.Multiline = true;
            this.afr_txtSaida.Name = "afr_txtSaida";
            this.afr_txtSaida.ReadOnly = true;
            this.afr_txtSaida.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.afr_txtSaida.Size = new System.Drawing.Size(850, 92);
            this.afr_txtSaida.TabIndex = 8;
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(25, 528);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(850, 20);
            this.afr_lblStatus.TabIndex = 9;
            this.afr_lblStatus.Text = "Pronto.";
            //
            // FrmUsuarios
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 558);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_grpDados);
            this.Controls.Add(this.afr_lblBusca);
            this.Controls.Add(this.afr_txtBusca);
            this.Controls.Add(this.afr_btnBuscar);
            this.Controls.Add(this.afr_lstUsuarios);
            this.Controls.Add(this.afr_btnRecarregar);
            this.Controls.Add(this.afr_btnVerPerfil);
            this.Controls.Add(this.afr_btnFechar);
            this.Controls.Add(this.afr_txtSaida);
            this.Controls.Add(this.afr_lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmUsuarios";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gerenciar Usuarios";
            this.Load += new System.EventHandler(this.FrmUsuarios_Load);
            this.afr_grpDados.ResumeLayout(false);
            this.afr_grpDados.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblTitulo;
        private System.Windows.Forms.GroupBox afr_grpDados;
        private System.Windows.Forms.Label afr_lblModo;
        private System.Windows.Forms.Label afr_lblNome;
        private System.Windows.Forms.TextBox afr_txtNome;
        private System.Windows.Forms.Label afr_lblEmail;
        private System.Windows.Forms.TextBox afr_txtEmail;
        private System.Windows.Forms.Label afr_lblSenha;
        private System.Windows.Forms.TextBox afr_txtSenha;
        private System.Windows.Forms.Label afr_lblPerfil;
        private System.Windows.Forms.ComboBox afr_cmbPerfil;
        private System.Windows.Forms.CheckBox afr_chkAtivo;
        private System.Windows.Forms.Button afr_btnNovo;
        private System.Windows.Forms.Button afr_btnSalvar;
        private System.Windows.Forms.Button afr_btnExcluir;
        private System.Windows.Forms.Label afr_lblAviso;
        private System.Windows.Forms.Label afr_lblBusca;
        private System.Windows.Forms.TextBox afr_txtBusca;
        private System.Windows.Forms.Button afr_btnBuscar;
        private System.Windows.Forms.ListView afr_lstUsuarios;
        private System.Windows.Forms.ColumnHeader afr_colId;
        private System.Windows.Forms.ColumnHeader afr_colNome;
        private System.Windows.Forms.ColumnHeader afr_colEmail;
        private System.Windows.Forms.ColumnHeader afr_colPerfil;
        private System.Windows.Forms.ColumnHeader afr_colAtivo;
        private System.Windows.Forms.ColumnHeader afr_colCadastro;
        private System.Windows.Forms.Button afr_btnVerPerfil;
        private System.Windows.Forms.Button afr_btnRecarregar;
        private System.Windows.Forms.Button afr_btnFechar;
        private System.Windows.Forms.TextBox afr_txtSaida;
        private System.Windows.Forms.Label afr_lblStatus;
    }
}
