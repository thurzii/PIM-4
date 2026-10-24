/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmCatalogo.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER da tela do aluno.
   Tem a linha de FILTROS da busca (DS04) e o botao que abre a aula
   numa tela propria (DS05).
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
            this.afr_lblCargaMin = new System.Windows.Forms.Label();
            this.afr_numCargaMin = new System.Windows.Forms.NumericUpDown();
            this.afr_lblCargaMax = new System.Windows.Forms.Label();
            this.afr_numCargaMax = new System.Windows.Forms.NumericUpDown();
            this.afr_lblHoras = new System.Windows.Forms.Label();
            this.afr_chkNaoConcluidos = new System.Windows.Forms.CheckBox();
            this.afr_lstCursos = new System.Windows.Forms.ListView();
            this.afr_colId = new System.Windows.Forms.ColumnHeader();
            this.afr_colTitulo = new System.Windows.Forms.ColumnHeader();
            this.afr_colCarga = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulas = new System.Windows.Forms.ColumnHeader();
            this.afr_grpCurso = new System.Windows.Forms.GroupBox();
            this.afr_lblCursoTitulo = new System.Windows.Forms.Label();
            this.afr_txtCursoDesc = new System.Windows.Forms.TextBox();
            this.afr_lblRequisitos = new System.Windows.Forms.Label();
            this.afr_lblProgressoTexto = new System.Windows.Forms.Label();
            this.afr_prgProgresso = new System.Windows.Forms.ProgressBar();
            this.afr_lblConclusao = new System.Windows.Forms.Label();
            this.afr_grpAulas = new System.Windows.Forms.GroupBox();
            this.afr_lstAulas = new System.Windows.Forms.ListView();
            this.afr_colAulaOrdem = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulaTitulo = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulaDuracao = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulaId = new System.Windows.Forms.ColumnHeader();
            this.afr_btnAbrirAula = new System.Windows.Forms.Button();
            this.afr_lblDicaAula = new System.Windows.Forms.Label();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.afr_btnFechar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.afr_numCargaMin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.afr_numCargaMax)).BeginInit();
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
            this.afr_lblAluno.Size = new System.Drawing.Size(848, 20);
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
            // afr_lblCargaMin
            //
            this.afr_lblCargaMin.AutoSize = true;
            this.afr_lblCargaMin.Location = new System.Drawing.Point(25, 120);
            this.afr_lblCargaMin.Name = "afr_lblCargaMin";
            this.afr_lblCargaMin.Size = new System.Drawing.Size(105, 15);
            this.afr_lblCargaMin.TabIndex = 6;
            this.afr_lblCargaMin.Text = "Carga horaria de:";
            //
            // afr_numCargaMin
            //
            this.afr_numCargaMin.Location = new System.Drawing.Point(138, 116);
            this.afr_numCargaMin.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.afr_numCargaMin.Name = "afr_numCargaMin";
            this.afr_numCargaMin.Size = new System.Drawing.Size(70, 23);
            this.afr_numCargaMin.TabIndex = 7;
            //
            // afr_lblCargaMax
            //
            this.afr_lblCargaMax.AutoSize = true;
            this.afr_lblCargaMax.Location = new System.Drawing.Point(214, 120);
            this.afr_lblCargaMax.Name = "afr_lblCargaMax";
            this.afr_lblCargaMax.Size = new System.Drawing.Size(22, 15);
            this.afr_lblCargaMax.TabIndex = 8;
            this.afr_lblCargaMax.Text = "ate";
            //
            // afr_numCargaMax
            //
            this.afr_numCargaMax.Location = new System.Drawing.Point(242, 116);
            this.afr_numCargaMax.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
            this.afr_numCargaMax.Name = "afr_numCargaMax";
            this.afr_numCargaMax.Size = new System.Drawing.Size(70, 23);
            this.afr_numCargaMax.TabIndex = 9;
            //
            // afr_lblHoras
            //
            this.afr_lblHoras.AutoSize = true;
            this.afr_lblHoras.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblHoras.Location = new System.Drawing.Point(318, 120);
            this.afr_lblHoras.Name = "afr_lblHoras";
            this.afr_lblHoras.Size = new System.Drawing.Size(120, 15);
            this.afr_lblHoras.TabIndex = 10;
            this.afr_lblHoras.Text = "h   (0 = sem limite)";
            //
            // afr_chkNaoConcluidos
            //
            this.afr_chkNaoConcluidos.AutoSize = true;
            this.afr_chkNaoConcluidos.Location = new System.Drawing.Point(450, 119);
            this.afr_chkNaoConcluidos.Name = "afr_chkNaoConcluidos";
            this.afr_chkNaoConcluidos.Size = new System.Drawing.Size(260, 19);
            this.afr_chkNaoConcluidos.TabIndex = 11;
            this.afr_chkNaoConcluidos.Text = "Somente os cursos que eu ainda nao conclui";
            this.afr_chkNaoConcluidos.UseVisualStyleBackColor = true;
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
            this.afr_lstCursos.Location = new System.Drawing.Point(25, 152);
            this.afr_lstCursos.MultiSelect = false;
            this.afr_lstCursos.Name = "afr_lstCursos";
            this.afr_lstCursos.Size = new System.Drawing.Size(425, 215);
            this.afr_lstCursos.TabIndex = 12;
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
            this.afr_grpCurso.Controls.Add(this.afr_lblRequisitos);
            this.afr_grpCurso.Controls.Add(this.afr_lblProgressoTexto);
            this.afr_grpCurso.Controls.Add(this.afr_prgProgresso);
            this.afr_grpCurso.Controls.Add(this.afr_lblConclusao);
            this.afr_grpCurso.Location = new System.Drawing.Point(465, 150);
            this.afr_grpCurso.Name = "afr_grpCurso";
            this.afr_grpCurso.Size = new System.Drawing.Size(410, 217);
            this.afr_grpCurso.TabIndex = 13;
            this.afr_grpCurso.TabStop = false;
            this.afr_grpCurso.Text = "Curso selecionado";
            //
            // afr_lblCursoTitulo
            //
            this.afr_lblCursoTitulo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.afr_lblCursoTitulo.Location = new System.Drawing.Point(15, 24);
            this.afr_lblCursoTitulo.Name = "afr_lblCursoTitulo";
            this.afr_lblCursoTitulo.Size = new System.Drawing.Size(380, 22);
            this.afr_lblCursoTitulo.TabIndex = 0;
            this.afr_lblCursoTitulo.Text = "Escolha um curso na lista ao lado.";
            //
            // afr_txtCursoDesc
            //
            this.afr_txtCursoDesc.BackColor = System.Drawing.Color.White;
            this.afr_txtCursoDesc.Location = new System.Drawing.Point(15, 50);
            this.afr_txtCursoDesc.Multiline = true;
            this.afr_txtCursoDesc.Name = "afr_txtCursoDesc";
            this.afr_txtCursoDesc.ReadOnly = true;
            this.afr_txtCursoDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.afr_txtCursoDesc.Size = new System.Drawing.Size(380, 60);
            this.afr_txtCursoDesc.TabIndex = 1;
            //
            // afr_lblRequisitos
            //
            this.afr_lblRequisitos.ForeColor = System.Drawing.Color.SteelBlue;
            this.afr_lblRequisitos.Location = new System.Drawing.Point(15, 116);
            this.afr_lblRequisitos.Name = "afr_lblRequisitos";
            this.afr_lblRequisitos.Size = new System.Drawing.Size(380, 20);
            this.afr_lblRequisitos.TabIndex = 2;
            this.afr_lblRequisitos.Text = "";
            //
            // afr_lblProgressoTexto
            //
            this.afr_lblProgressoTexto.AutoSize = true;
            this.afr_lblProgressoTexto.Location = new System.Drawing.Point(15, 140);
            this.afr_lblProgressoTexto.Name = "afr_lblProgressoTexto";
            this.afr_lblProgressoTexto.Size = new System.Drawing.Size(120, 15);
            this.afr_lblProgressoTexto.TabIndex = 3;
            this.afr_lblProgressoTexto.Text = "Seu progresso: 0%";
            //
            // afr_prgProgresso
            //
            this.afr_prgProgresso.Location = new System.Drawing.Point(15, 159);
            this.afr_prgProgresso.Name = "afr_prgProgresso";
            this.afr_prgProgresso.Size = new System.Drawing.Size(380, 22);
            this.afr_prgProgresso.TabIndex = 4;
            //
            // afr_lblConclusao
            //
            this.afr_lblConclusao.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblConclusao.Location = new System.Drawing.Point(15, 186);
            this.afr_lblConclusao.Name = "afr_lblConclusao";
            this.afr_lblConclusao.Size = new System.Drawing.Size(380, 24);
            this.afr_lblConclusao.TabIndex = 5;
            this.afr_lblConclusao.Text = "";
            //
            // afr_grpAulas
            //
            this.afr_grpAulas.Controls.Add(this.afr_lstAulas);
            this.afr_grpAulas.Controls.Add(this.afr_btnAbrirAula);
            this.afr_grpAulas.Controls.Add(this.afr_lblDicaAula);
            this.afr_grpAulas.Location = new System.Drawing.Point(25, 377);
            this.afr_grpAulas.Name = "afr_grpAulas";
            this.afr_grpAulas.Size = new System.Drawing.Size(850, 205);
            this.afr_grpAulas.TabIndex = 14;
            this.afr_grpAulas.TabStop = false;
            this.afr_grpAulas.Text = "Aulas - abra a aula para ver o conteudo, ou marque a caixa para concluir";
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
            this.afr_lstAulas.Size = new System.Drawing.Size(820, 130);
            this.afr_lstAulas.TabIndex = 0;
            this.afr_lstAulas.UseCompatibleStateImageBehavior = false;
            this.afr_lstAulas.View = System.Windows.Forms.View.Details;
            this.afr_lstAulas.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.afr_lstAulas_ItemChecked);
            this.afr_lstAulas.DoubleClick += new System.EventHandler(this.afr_lstAulas_DoubleClick);
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
            // afr_btnAbrirAula
            //
            this.afr_btnAbrirAula.BackColor = System.Drawing.Color.LightSteelBlue;
            this.afr_btnAbrirAula.Location = new System.Drawing.Point(15, 162);
            this.afr_btnAbrirAula.Name = "afr_btnAbrirAula";
            this.afr_btnAbrirAula.Size = new System.Drawing.Size(250, 32);
            this.afr_btnAbrirAula.TabIndex = 1;
            this.afr_btnAbrirAula.Text = "Abrir a aula selecionada";
            this.afr_btnAbrirAula.UseVisualStyleBackColor = false;
            this.afr_btnAbrirAula.Click += new System.EventHandler(this.afr_btnAbrirAula_Click);
            //
            // afr_lblDicaAula
            //
            this.afr_lblDicaAula.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblDicaAula.Location = new System.Drawing.Point(275, 168);
            this.afr_lblDicaAula.Name = "afr_lblDicaAula";
            this.afr_lblDicaAula.Size = new System.Drawing.Size(560, 30);
            this.afr_lblDicaAula.TabIndex = 2;
            this.afr_lblDicaAula.Text = "Dois cliques na aula abrem o conteudo dela.";
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(25, 592);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(690, 34);
            this.afr_lblStatus.TabIndex = 15;
            this.afr_lblStatus.Text = "Pronto.";
            //
            // afr_btnFechar
            //
            this.afr_btnFechar.Location = new System.Drawing.Point(735, 590);
            this.afr_btnFechar.Name = "afr_btnFechar";
            this.afr_btnFechar.Size = new System.Drawing.Size(140, 32);
            this.afr_btnFechar.TabIndex = 16;
            this.afr_btnFechar.Text = "Fechar";
            this.afr_btnFechar.UseVisualStyleBackColor = true;
            this.afr_btnFechar.Click += new System.EventHandler(this.afr_btnFechar_Click);
            //
            // FrmCatalogo
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 636);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblAluno);
            this.Controls.Add(this.afr_lblBusca);
            this.Controls.Add(this.afr_txtBusca);
            this.Controls.Add(this.afr_btnBuscar);
            this.Controls.Add(this.afr_btnVerTodos);
            this.Controls.Add(this.afr_lblCargaMin);
            this.Controls.Add(this.afr_numCargaMin);
            this.Controls.Add(this.afr_lblCargaMax);
            this.Controls.Add(this.afr_numCargaMax);
            this.Controls.Add(this.afr_lblHoras);
            this.Controls.Add(this.afr_chkNaoConcluidos);
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
            ((System.ComponentModel.ISupportInitialize)(this.afr_numCargaMin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.afr_numCargaMax)).EndInit();
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
        private System.Windows.Forms.Label afr_lblCargaMin;
        private System.Windows.Forms.NumericUpDown afr_numCargaMin;
        private System.Windows.Forms.Label afr_lblCargaMax;
        private System.Windows.Forms.NumericUpDown afr_numCargaMax;
        private System.Windows.Forms.Label afr_lblHoras;
        private System.Windows.Forms.CheckBox afr_chkNaoConcluidos;
        private System.Windows.Forms.ListView afr_lstCursos;
        private System.Windows.Forms.ColumnHeader afr_colId;
        private System.Windows.Forms.ColumnHeader afr_colTitulo;
        private System.Windows.Forms.ColumnHeader afr_colCarga;
        private System.Windows.Forms.ColumnHeader afr_colAulas;
        private System.Windows.Forms.GroupBox afr_grpCurso;
        private System.Windows.Forms.Label afr_lblCursoTitulo;
        private System.Windows.Forms.TextBox afr_txtCursoDesc;
        private System.Windows.Forms.Label afr_lblRequisitos;
        private System.Windows.Forms.Label afr_lblProgressoTexto;
        private System.Windows.Forms.ProgressBar afr_prgProgresso;
        private System.Windows.Forms.Label afr_lblConclusao;
        private System.Windows.Forms.GroupBox afr_grpAulas;
        private System.Windows.Forms.ListView afr_lstAulas;
        private System.Windows.Forms.ColumnHeader afr_colAulaOrdem;
        private System.Windows.Forms.ColumnHeader afr_colAulaTitulo;
        private System.Windows.Forms.ColumnHeader afr_colAulaDuracao;
        private System.Windows.Forms.ColumnHeader afr_colAulaId;
        private System.Windows.Forms.Button afr_btnAbrirAula;
        private System.Windows.Forms.Label afr_lblDicaAula;
        private System.Windows.Forms.Label afr_lblStatus;
        private System.Windows.Forms.Button afr_btnFechar;
    }
}
