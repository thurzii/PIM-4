/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmCatalogo.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER da tela do aluno.
   ===================================================================== */

namespace PlataformaCursos
{
    partial class FrmCatalogo
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
            this.afr_lblAluno = new System.Windows.Forms.Label();
            this.afr_lblBusca = new System.Windows.Forms.Label();
            this.afr_txtBusca = new System.Windows.Forms.TextBox();
            this.afr_btnBuscar = new System.Windows.Forms.Button();
            this.afr_btnVerTodos = new System.Windows.Forms.Button();
            this.afr_lstCursos = new System.Windows.Forms.ListView();
            this.afr_colId = new System.Windows.Forms.ColumnHeader();
            this.afr_colTitulo = new System.Windows.Forms.ColumnHeader();
            this.afr_colCarga = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulas = new System.Windows.Forms.ColumnHeader();
            this.afr_grpCurso = new System.Windows.Forms.GroupBox();
            this.afr_lblCursoTitulo = new System.Windows.Forms.Label();
            this.afr_txtCursoDesc = new System.Windows.Forms.TextBox();
            this.afr_lblProgressoTexto = new System.Windows.Forms.Label();
            this.afr_prgProgresso = new System.Windows.Forms.ProgressBar();
            this.afr_lblConclusao = new System.Windows.Forms.Label();
            this.afr_grpAulas = new System.Windows.Forms.GroupBox();
            this.afr_lstAulas = new System.Windows.Forms.ListView();
            this.afr_colAulaOrdem = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulaTitulo = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulaDuracao = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulaId = new System.Windows.Forms.ColumnHeader();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.afr_btnFechar = new System.Windows.Forms.Button();
            this.afr_grpCurso.SuspendLayout();
            this.afr_grpAulas.SuspendLayout();
            this.SuspendLayout();
            //
            // afr_lblTitulo
            //
            this.afr_lblTitulo.AutoSize = true;
            this.afr_lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.afr_lblTitulo.Location = new System.Drawing.Point(25, 16);
            this.afr_lblTitulo.Name = "afr_lblTitulo";
            this.afr_lblTitulo.Size = new System.Drawing.Size(270, 25);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "CATALOGO DE CURSOS";
            //
            // afr_lblAluno
            //
            this.afr_lblAluno.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblAluno.Location = new System.Drawing.Point(27, 48);
            this.afr_lblAluno.Name = "afr_lblAluno";
            this.afr_lblAluno.Size = new System.Drawing.Size(700, 20);
            this.afr_lblAluno.TabIndex = 1;
            this.afr_lblAluno.Text = "Aluno.";
            //
            // afr_lblBusca
            //
            this.afr_lblBusca.AutoSize = true;
            this.afr_lblBusca.Location = new System.Drawing.Point(25, 88);
            this.afr_lblBusca.Name = "afr_lblBusca";
            this.afr_lblBusca.Size = new System.Drawing.Size(45, 15);
            this.afr_lblBusca.TabIndex = 2;
            this.afr_lblBusca.Text = "Buscar:";
            //
            // afr_txtBusca
            //
            this.afr_txtBusca.Location = new System.Drawing.Point(80, 84);
            this.afr_txtBusca.MaxLength = 100;
            this.afr_txtBusca.Name = "afr_txtBusca";
            this.afr_txtBusca.Size = new System.Drawing.Size(290, 23);
            this.afr_txtBusca.TabIndex = 3;
            //
            // afr_btnBuscar
            //
            this.afr_btnBuscar.BackColor = System.Drawing.Color.LightSteelBlue;
            this.afr_btnBuscar.Location = new System.Drawing.Point(380, 83);
            this.afr_btnBuscar.Name = "afr_btnBuscar";
            this.afr_btnBuscar.Size = new System.Drawing.Size(130, 26);
            this.afr_btnBuscar.TabIndex = 4;
            this.afr_btnBuscar.Text = "Buscar cursos";
            this.afr_btnBuscar.UseVisualStyleBackColor = false;
            this.afr_btnBuscar.Click += new System.EventHandler(this.afr_btnBuscar_Click);
            //
            // afr_btnVerTodos
            //
            this.afr_btnVerTodos.Location = new System.Drawing.Point(520, 83);
            this.afr_btnVerTodos.Name = "afr_btnVerTodos";
            this.afr_btnVerTodos.Size = new System.Drawing.Size(150, 26);
            this.afr_btnVerTodos.TabIndex = 5;
            this.afr_btnVerTodos.Text = "Ver todos os cursos";
            this.afr_btnVerTodos.UseVisualStyleBackColor = true;
            this.afr_btnVerTodos.Click += new System.EventHandler(this.afr_btnVerTodos_Click);
            //
            // afr_lstCursos
            //
            this.afr_lstCursos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.afr_colId,
            this.afr_colTitulo,
            this.afr_colCarga,
            this.afr_colAulas});
            this.afr_lstCursos.FullRowSelect = true;
            this.afr_lstCursos.GridLines = true;
            this.afr_lstCursos.Location = new System.Drawing.Point(25, 120);
            this.afr_lstCursos.MultiSelect = false;
            this.afr_lstCursos.Name = "afr_lstCursos";
            this.afr_lstCursos.Size = new System.Drawing.Size(425, 245);
            this.afr_lstCursos.TabIndex = 6;
            this.afr_lstCursos.UseCompatibleStateImageBehavior = false;
            this.afr_lstCursos.View = System.Windows.Forms.View.Details;
            this.afr_lstCursos.SelectedIndexChanged += new System.EventHandler(this.afr_lstCursos_SelectedIndexChanged);
            //
            // afr_colId
            //
            this.afr_colId.Text = "Id";
            this.afr_colId.Width = 40;
            //
            // afr_colTitulo
            //
            this.afr_colTitulo.Text = "Curso";
            this.afr_colTitulo.Width = 225;
            //
            // afr_colCarga
            //
            this.afr_colCarga.Text = "Carga";
            this.afr_colCarga.Width = 60;
            //
            // afr_colAulas
            //
            this.afr_colAulas.Text = "Aulas";
            this.afr_colAulas.Width = 60;
            //
            // afr_grpCurso
            //
            this.afr_grpCurso.Controls.Add(this.afr_lblCursoTitulo);
            this.afr_grpCurso.Controls.Add(this.afr_txtCursoDesc);
            this.afr_grpCurso.Controls.Add(this.afr_lblProgressoTexto);
            this.afr_grpCurso.Controls.Add(this.afr_prgProgresso);
            this.afr_grpCurso.Controls.Add(this.afr_lblConclusao);
            this.afr_grpCurso.Location = new System.Drawing.Point(465, 118);
            this.afr_grpCurso.Name = "afr_grpCurso";
            this.afr_grpCurso.Size = new System.Drawing.Size(410, 247);
            this.afr_grpCurso.TabIndex = 7;
            this.afr_grpCurso.TabStop = false;
            this.afr_grpCurso.Text = "Curso selecionado";
            //
            // afr_lblCursoTitulo
            //
            this.afr_lblCursoTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.afr_lblCursoTitulo.Location = new System.Drawing.Point(15, 28);
            this.afr_lblCursoTitulo.Name = "afr_lblCursoTitulo";
            this.afr_lblCursoTitulo.Size = new System.Drawing.Size(380, 40);
            this.afr_lblCursoTitulo.TabIndex = 0;
            this.afr_lblCursoTitulo.Text = "Escolha um curso na lista ao lado.";
            //
            // afr_txtCursoDesc
            //
            this.afr_txtCursoDesc.BackColor = System.Drawing.Color.White;
            this.afr_txtCursoDesc.Location = new System.Drawing.Point(15, 72);
            this.afr_txtCursoDesc.Multiline = true;
            this.afr_txtCursoDesc.Name = "afr_txtCursoDesc";
            this.afr_txtCursoDesc.ReadOnly = true;
            this.afr_txtCursoDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.afr_txtCursoDesc.Size = new System.Drawing.Size(380, 70);
            this.afr_txtCursoDesc.TabIndex = 1;
            //
            // afr_lblProgressoTexto
            //
            this.afr_lblProgressoTexto.AutoSize = true;
            this.afr_lblProgressoTexto.Location = new System.Drawing.Point(15, 152);
            this.afr_lblProgressoTexto.Name = "afr_lblProgressoTexto";
            this.afr_lblProgressoTexto.Size = new System.Drawing.Size(120, 15);
            this.afr_lblProgressoTexto.TabIndex = 2;
            this.afr_lblProgressoTexto.Text = "Seu progresso: 0%";
            //
            // afr_prgProgresso
            //
            this.afr_prgProgresso.Location = new System.Drawing.Point(15, 172);
            this.afr_prgProgresso.Name = "afr_prgProgresso";
            this.afr_prgProgresso.Size = new System.Drawing.Size(380, 24);
            this.afr_prgProgresso.TabIndex = 3;
            //
            // afr_lblConclusao
            //
            this.afr_lblConclusao.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblConclusao.Location = new System.Drawing.Point(15, 204);
            this.afr_lblConclusao.Name = "afr_lblConclusao";
            this.afr_lblConclusao.Size = new System.Drawing.Size(380, 35);
            this.afr_lblConclusao.TabIndex = 4;
            this.afr_lblConclusao.Text = "";
            //
            // afr_grpAulas
            //
            this.afr_grpAulas.Controls.Add(this.afr_lstAulas);
            this.afr_grpAulas.Location = new System.Drawing.Point(25, 375);
            this.afr_grpAulas.Name = "afr_grpAulas";
            this.afr_grpAulas.Size = new System.Drawing.Size(850, 175);
            this.afr_grpAulas.TabIndex = 8;
            this.afr_grpAulas.TabStop = false;
            this.afr_grpAulas.Text = "Aulas - marque a caixa para registrar a aula como concluida";
            //
            // afr_lstAulas
            //
            this.afr_lstAulas.CheckBoxes = true;
            this.afr_lstAulas.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.afr_colAulaOrdem,
            this.afr_colAulaTitulo,
            this.afr_colAulaDuracao,
            this.afr_colAulaId});
            this.afr_lstAulas.FullRowSelect = true;
            this.afr_lstAulas.GridLines = true;
            this.afr_lstAulas.Location = new System.Drawing.Point(15, 25);
            this.afr_lstAulas.MultiSelect = false;
            this.afr_lstAulas.Name = "afr_lstAulas";
            this.afr_lstAulas.Size = new System.Drawing.Size(820, 140);
            this.afr_lstAulas.TabIndex = 0;
            this.afr_lstAulas.UseCompatibleStateImageBehavior = false;
            this.afr_lstAulas.View = System.Windows.Forms.View.Details;
            this.afr_lstAulas.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.afr_lstAulas_ItemChecked);
            //
            // afr_colAulaOrdem
            //
            this.afr_colAulaOrdem.Text = "Aula";
            this.afr_colAulaOrdem.Width = 80;
            //
            // afr_colAulaTitulo
            //
            this.afr_colAulaTitulo.Text = "Titulo";
            this.afr_colAulaTitulo.Width = 540;
            //
            // afr_colAulaDuracao
            //
            this.afr_colAulaDuracao.Text = "Duracao";
            this.afr_colAulaDuracao.Width = 100;
            //
            // afr_colAulaId
            //
            this.afr_colAulaId.Text = "Id";
            this.afr_colAulaId.Width = 60;
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(25, 560);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(690, 34);
            this.afr_lblStatus.TabIndex = 9;
            this.afr_lblStatus.Text = "Pronto.";
            //
            // afr_btnFechar
            //
            this.afr_btnFechar.Location = new System.Drawing.Point(735, 558);
            this.afr_btnFechar.Name = "afr_btnFechar";
            this.afr_btnFechar.Size = new System.Drawing.Size(140, 32);
            this.afr_btnFechar.TabIndex = 10;
            this.afr_btnFechar.Text = "Fechar";
            this.afr_btnFechar.UseVisualStyleBackColor = true;
            this.afr_btnFechar.Click += new System.EventHandler(this.afr_btnFechar_Click);
            //
            // FrmCatalogo
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 605);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblAluno);
            this.Controls.Add(this.afr_lblBusca);
            this.Controls.Add(this.afr_txtBusca);
            this.Controls.Add(this.afr_btnBuscar);
            this.Controls.Add(this.afr_btnVerTodos);
            this.Controls.Add(this.afr_lstCursos);
            this.Controls.Add(this.afr_grpCurso);
            this.Controls.Add(this.afr_grpAulas);
            this.Controls.Add(this.afr_lblStatus);
            this.Controls.Add(this.afr_btnFechar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmCatalogo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Catalogo de Cursos";
            this.Load += new System.EventHandler(this.FrmCatalogo_Load);
            this.afr_grpCurso.ResumeLayout(false);
            this.afr_grpCurso.PerformLayout();
            this.afr_grpAulas.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblTitulo;
        private System.Windows.Forms.Label afr_lblAluno;
        private System.Windows.Forms.Label afr_lblBusca;
        private System.Windows.Forms.TextBox afr_txtBusca;
        private System.Windows.Forms.Button afr_btnBuscar;
        private System.Windows.Forms.Button afr_btnVerTodos;
        private System.Windows.Forms.ListView afr_lstCursos;
        private System.Windows.Forms.ColumnHeader afr_colId;
        private System.Windows.Forms.ColumnHeader afr_colTitulo;
        private System.Windows.Forms.ColumnHeader afr_colCarga;
        private System.Windows.Forms.ColumnHeader afr_colAulas;
        private System.Windows.Forms.GroupBox afr_grpCurso;
        private System.Windows.Forms.Label afr_lblCursoTitulo;
        private System.Windows.Forms.TextBox afr_txtCursoDesc;
        private System.Windows.Forms.Label afr_lblProgressoTexto;
        private System.Windows.Forms.ProgressBar afr_prgProgresso;
        private System.Windows.Forms.Label afr_lblConclusao;
        private System.Windows.Forms.GroupBox afr_grpAulas;
        private System.Windows.Forms.ListView afr_lstAulas;
        private System.Windows.Forms.ColumnHeader afr_colAulaOrdem;
        private System.Windows.Forms.ColumnHeader afr_colAulaTitulo;
        private System.Windows.Forms.ColumnHeader afr_colAulaDuracao;
        private System.Windows.Forms.ColumnHeader afr_colAulaId;
        private System.Windows.Forms.Label afr_lblStatus;
        private System.Windows.Forms.Button afr_btnFechar;
    }
}
