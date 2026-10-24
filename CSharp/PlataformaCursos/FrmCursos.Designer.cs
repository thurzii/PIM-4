/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmCursos.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER da tela de cursos e aulas.
   Ganhou o campo do percentual minimo de conclusao (DS08) e o editor
   do conteudo da aula (DS05).
   ===================================================================== */

namespace PlataformaCursos
{
    partial class FrmCursos
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
            this.afr_grpCurso = new System.Windows.Forms.GroupBox();
            this.afr_lblModo = new System.Windows.Forms.Label();
            this.afr_lblTituloCurso = new System.Windows.Forms.Label();
            this.afr_txtTitulo = new System.Windows.Forms.TextBox();
            this.afr_lblDescricao = new System.Windows.Forms.Label();
            this.afr_txtDescricao = new System.Windows.Forms.TextBox();
            this.afr_lblCarga = new System.Windows.Forms.Label();
            this.afr_txtCarga = new System.Windows.Forms.TextBox();
            this.afr_chkAtivoCurso = new System.Windows.Forms.CheckBox();
            this.afr_lblMinimo = new System.Windows.Forms.Label();
            this.afr_numMinimo = new System.Windows.Forms.NumericUpDown();
            this.afr_lblMinimoDica = new System.Windows.Forms.Label();
            this.afr_btnNovoCurso = new System.Windows.Forms.Button();
            this.afr_btnSalvarCurso = new System.Windows.Forms.Button();
            this.afr_btnExcluirCurso = new System.Windows.Forms.Button();
            this.afr_lblAvisoCurso = new System.Windows.Forms.Label();
            this.afr_lblBusca = new System.Windows.Forms.Label();
            this.afr_txtBusca = new System.Windows.Forms.TextBox();
            this.afr_btnBuscar = new System.Windows.Forms.Button();
            this.afr_lstCursos = new System.Windows.Forms.ListView();
            this.afr_colId = new System.Windows.Forms.ColumnHeader();
            this.afr_colTitulo = new System.Windows.Forms.ColumnHeader();
            this.afr_colCarga = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulas = new System.Windows.Forms.ColumnHeader();
            this.afr_colMinimo = new System.Windows.Forms.ColumnHeader();
            this.afr_colAtivo = new System.Windows.Forms.ColumnHeader();
            this.afr_grpAulas = new System.Windows.Forms.GroupBox();
            this.afr_lblAulaTitulo = new System.Windows.Forms.Label();
            this.afr_txtAulaTitulo = new System.Windows.Forms.TextBox();
            this.afr_lblAulaDuracao = new System.Windows.Forms.Label();
            this.afr_txtAulaDuracao = new System.Windows.Forms.TextBox();
            this.afr_btnAddAula = new System.Windows.Forms.Button();
            this.afr_btnDelAula = new System.Windows.Forms.Button();
            this.afr_lstAulas = new System.Windows.Forms.ListView();
            this.afr_colAulaId = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulaOrdem = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulaTitulo = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulaDuracao = new System.Windows.Forms.ColumnHeader();
            this.afr_lblAulaConteudo = new System.Windows.Forms.Label();
            this.afr_txtAulaConteudo = new System.Windows.Forms.TextBox();
            this.afr_btnSalvarAula = new System.Windows.Forms.Button();
            this.afr_lblDicaConteudo = new System.Windows.Forms.Label();
            this.afr_lblStatus = new System.Windows.Forms.Label();
            this.afr_btnFechar = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.afr_numMinimo)).BeginInit();
            this.afr_grpCurso.SuspendLayout();
            this.afr_grpAulas.SuspendLayout();
            this.SuspendLayout();
            //
            // afr_lblTitulo
            //
            this.afr_lblTitulo.AutoSize = true;
            this.afr_lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.afr_lblTitulo.Location = new System.Drawing.Point(25, 18);
            this.afr_lblTitulo.Name = "afr_lblTitulo";
            this.afr_lblTitulo.Size = new System.Drawing.Size(330, 25);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "GERENCIAR CURSOS E AULAS";
            //
            // afr_grpCurso
            //
            this.afr_grpCurso.Controls.Add(this.afr_lblModo);
            this.afr_grpCurso.Controls.Add(this.afr_lblTituloCurso);
            this.afr_grpCurso.Controls.Add(this.afr_txtTitulo);
            this.afr_grpCurso.Controls.Add(this.afr_lblDescricao);
            this.afr_grpCurso.Controls.Add(this.afr_txtDescricao);
            this.afr_grpCurso.Controls.Add(this.afr_lblCarga);
            this.afr_grpCurso.Controls.Add(this.afr_txtCarga);
            this.afr_grpCurso.Controls.Add(this.afr_chkAtivoCurso);
            this.afr_grpCurso.Controls.Add(this.afr_lblMinimo);
            this.afr_grpCurso.Controls.Add(this.afr_numMinimo);
            this.afr_grpCurso.Controls.Add(this.afr_lblMinimoDica);
            this.afr_grpCurso.Controls.Add(this.afr_btnNovoCurso);
            this.afr_grpCurso.Controls.Add(this.afr_btnSalvarCurso);
            this.afr_grpCurso.Controls.Add(this.afr_btnExcluirCurso);
            this.afr_grpCurso.Controls.Add(this.afr_lblAvisoCurso);
            this.afr_grpCurso.Location = new System.Drawing.Point(25, 56);
            this.afr_grpCurso.Name = "afr_grpCurso";
            this.afr_grpCurso.Size = new System.Drawing.Size(400, 318);
            this.afr_grpCurso.TabIndex = 1;
            this.afr_grpCurso.TabStop = false;
            this.afr_grpCurso.Text = "Dados do Curso";
            //
            // afr_lblModo
            //
            this.afr_lblModo.AutoSize = true;
            this.afr_lblModo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.afr_lblModo.Location = new System.Drawing.Point(20, 26);
            this.afr_lblModo.Name = "afr_lblModo";
            this.afr_lblModo.Size = new System.Drawing.Size(100, 15);
            this.afr_lblModo.TabIndex = 0;
            this.afr_lblModo.Text = "NOVO CURSO";
            //
            // afr_lblTituloCurso
            //
            this.afr_lblTituloCurso.AutoSize = true;
            this.afr_lblTituloCurso.Location = new System.Drawing.Point(20, 58);
            this.afr_lblTituloCurso.Name = "afr_lblTituloCurso";
            this.afr_lblTituloCurso.Size = new System.Drawing.Size(40, 15);
            this.afr_lblTituloCurso.TabIndex = 1;
            this.afr_lblTituloCurso.Text = "Titulo:";
            //
            // afr_txtTitulo
            //
            this.afr_txtTitulo.Location = new System.Drawing.Point(110, 55);
            this.afr_txtTitulo.MaxLength = 120;
            this.afr_txtTitulo.Name = "afr_txtTitulo";
            this.afr_txtTitulo.Size = new System.Drawing.Size(270, 23);
            this.afr_txtTitulo.TabIndex = 2;
            //
            // afr_lblDescricao
            //
            this.afr_lblDescricao.AutoSize = true;
            this.afr_lblDescricao.Location = new System.Drawing.Point(20, 93);
            this.afr_lblDescricao.Name = "afr_lblDescricao";
            this.afr_lblDescricao.Size = new System.Drawing.Size(62, 15);
            this.afr_lblDescricao.TabIndex = 3;
            this.afr_lblDescricao.Text = "Descricao:";
            //
            // afr_txtDescricao
            //
            this.afr_txtDescricao.Location = new System.Drawing.Point(110, 90);
            this.afr_txtDescricao.MaxLength = 500;
            this.afr_txtDescricao.Multiline = true;
            this.afr_txtDescricao.Name = "afr_txtDescricao";
            this.afr_txtDescricao.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.afr_txtDescricao.Size = new System.Drawing.Size(270, 60);
            this.afr_txtDescricao.TabIndex = 4;
            //
            // afr_lblCarga
            //
            this.afr_lblCarga.AutoSize = true;
            this.afr_lblCarga.Location = new System.Drawing.Point(20, 165);
            this.afr_lblCarga.Name = "afr_lblCarga";
            this.afr_lblCarga.Size = new System.Drawing.Size(80, 15);
            this.afr_lblCarga.TabIndex = 5;
            this.afr_lblCarga.Text = "Carga (horas):";
            //
            // afr_txtCarga
            //
            this.afr_txtCarga.Location = new System.Drawing.Point(110, 162);
            this.afr_txtCarga.MaxLength = 5;
            this.afr_txtCarga.Name = "afr_txtCarga";
            this.afr_txtCarga.Size = new System.Drawing.Size(80, 23);
            this.afr_txtCarga.TabIndex = 6;
            this.afr_txtCarga.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // afr_chkAtivoCurso
            //
            this.afr_chkAtivoCurso.AutoSize = true;
            this.afr_chkAtivoCurso.Checked = true;
            this.afr_chkAtivoCurso.CheckState = System.Windows.Forms.CheckState.Checked;
            this.afr_chkAtivoCurso.Location = new System.Drawing.Point(210, 164);
            this.afr_chkAtivoCurso.Name = "afr_chkAtivoCurso";
            this.afr_chkAtivoCurso.Size = new System.Drawing.Size(170, 19);
            this.afr_chkAtivoCurso.TabIndex = 7;
            this.afr_chkAtivoCurso.Text = "Ativo (visivel para o aluno)";
            this.afr_chkAtivoCurso.UseVisualStyleBackColor = true;
            //
            // afr_lblMinimo
            //
            this.afr_lblMinimo.AutoSize = true;
            this.afr_lblMinimo.Location = new System.Drawing.Point(20, 196);
            this.afr_lblMinimo.Name = "afr_lblMinimo";
            this.afr_lblMinimo.Size = new System.Drawing.Size(85, 15);
            this.afr_lblMinimo.TabIndex = 8;
            this.afr_lblMinimo.Text = "Minimo (%):";
            //
            // afr_numMinimo
            //
            this.afr_numMinimo.Location = new System.Drawing.Point(110, 193);
            this.afr_numMinimo.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.afr_numMinimo.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.afr_numMinimo.Name = "afr_numMinimo";
            this.afr_numMinimo.Size = new System.Drawing.Size(80, 23);
            this.afr_numMinimo.TabIndex = 9;
            this.afr_numMinimo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.afr_numMinimo.Value = new decimal(new int[] { 100, 0, 0, 0 });
            //
            // afr_lblMinimoDica
            //
            this.afr_lblMinimoDica.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblMinimoDica.Location = new System.Drawing.Point(207, 194);
            this.afr_lblMinimoDica.Name = "afr_lblMinimoDica";
            this.afr_lblMinimoDica.Size = new System.Drawing.Size(175, 32);
            this.afr_lblMinimoDica.TabIndex = 10;
            this.afr_lblMinimoDica.Text = "das aulas para concluir o curso";
            //
            // afr_btnNovoCurso
            //
            this.afr_btnNovoCurso.Location = new System.Drawing.Point(20, 232);
            this.afr_btnNovoCurso.Name = "afr_btnNovoCurso";
            this.afr_btnNovoCurso.Size = new System.Drawing.Size(118, 40);
            this.afr_btnNovoCurso.TabIndex = 11;
            this.afr_btnNovoCurso.Text = "Novo";
            this.afr_btnNovoCurso.UseVisualStyleBackColor = true;
            this.afr_btnNovoCurso.Click += new System.EventHandler(this.afr_btnNovoCurso_Click);
            //
            // afr_btnSalvarCurso
            //
            this.afr_btnSalvarCurso.BackColor = System.Drawing.Color.LightSteelBlue;
            this.afr_btnSalvarCurso.Location = new System.Drawing.Point(148, 232);
            this.afr_btnSalvarCurso.Name = "afr_btnSalvarCurso";
            this.afr_btnSalvarCurso.Size = new System.Drawing.Size(118, 40);
            this.afr_btnSalvarCurso.TabIndex = 12;
            this.afr_btnSalvarCurso.Text = "Salvar";
            this.afr_btnSalvarCurso.UseVisualStyleBackColor = false;
            this.afr_btnSalvarCurso.Click += new System.EventHandler(this.afr_btnSalvarCurso_Click);
            //
            // afr_btnExcluirCurso
            //
            this.afr_btnExcluirCurso.BackColor = System.Drawing.Color.MistyRose;
            this.afr_btnExcluirCurso.Location = new System.Drawing.Point(276, 232);
            this.afr_btnExcluirCurso.Name = "afr_btnExcluirCurso";
            this.afr_btnExcluirCurso.Size = new System.Drawing.Size(104, 40);
            this.afr_btnExcluirCurso.TabIndex = 13;
            this.afr_btnExcluirCurso.Text = "Excluir";
            this.afr_btnExcluirCurso.UseVisualStyleBackColor = false;
            this.afr_btnExcluirCurso.Click += new System.EventHandler(this.afr_btnExcluirCurso_Click);
            //
            // afr_lblAvisoCurso
            //
            this.afr_lblAvisoCurso.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblAvisoCurso.Location = new System.Drawing.Point(20, 278);
            this.afr_lblAvisoCurso.Name = "afr_lblAvisoCurso";
            this.afr_lblAvisoCurso.Size = new System.Drawing.Size(360, 34);
            this.afr_lblAvisoCurso.TabIndex = 14;
            this.afr_lblAvisoCurso.Text = "Clique em um curso da lista para editar e para ver ou cadastrar as aulas dele.";
            //
            // afr_lblBusca
            //
            this.afr_lblBusca.AutoSize = true;
            this.afr_lblBusca.Location = new System.Drawing.Point(445, 64);
            this.afr_lblBusca.Name = "afr_lblBusca";
            this.afr_lblBusca.Size = new System.Drawing.Size(45, 15);
            this.afr_lblBusca.TabIndex = 2;
            this.afr_lblBusca.Text = "Buscar:";
            //
            // afr_txtBusca
            //
            this.afr_txtBusca.Location = new System.Drawing.Point(500, 60);
            this.afr_txtBusca.MaxLength = 100;
            this.afr_txtBusca.Name = "afr_txtBusca";
            this.afr_txtBusca.Size = new System.Drawing.Size(240, 23);
            this.afr_txtBusca.TabIndex = 3;
            //
            // afr_btnBuscar
            //
            this.afr_btnBuscar.Location = new System.Drawing.Point(750, 59);
            this.afr_btnBuscar.Name = "afr_btnBuscar";
            this.afr_btnBuscar.Size = new System.Drawing.Size(125, 26);
            this.afr_btnBuscar.TabIndex = 4;
            this.afr_btnBuscar.Text = "Buscar (SELECT)";
            this.afr_btnBuscar.UseVisualStyleBackColor = true;
            this.afr_btnBuscar.Click += new System.EventHandler(this.afr_btnBuscar_Click);
            //
            // afr_lstCursos
            //
            this.afr_lstCursos.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.afr_colId,
            this.afr_colTitulo,
            this.afr_colCarga,
            this.afr_colAulas,
            this.afr_colMinimo,
            this.afr_colAtivo});
            this.afr_lstCursos.FullRowSelect = true;
            this.afr_lstCursos.GridLines = true;
            this.afr_lstCursos.Location = new System.Drawing.Point(445, 96);
            this.afr_lstCursos.MultiSelect = false;
            this.afr_lstCursos.Name = "afr_lstCursos";
            this.afr_lstCursos.Size = new System.Drawing.Size(430, 278);
            this.afr_lstCursos.TabIndex = 5;
            this.afr_lstCursos.UseCompatibleStateImageBehavior = false;
            this.afr_lstCursos.View = System.Windows.Forms.View.Details;
            this.afr_lstCursos.SelectedIndexChanged += new System.EventHandler(this.afr_lstCursos_SelectedIndexChanged);
            //
            // afr_colId
            //
            this.afr_colId.Text = "Id";
            this.afr_colId.Width = 35;
            //
            // afr_colTitulo
            //
            this.afr_colTitulo.Text = "Titulo";
            this.afr_colTitulo.Width = 170;
            //
            // afr_colCarga
            //
            this.afr_colCarga.Text = "Carga";
            this.afr_colCarga.Width = 55;
            //
            // afr_colAulas
            //
            this.afr_colAulas.Text = "Aulas";
            this.afr_colAulas.Width = 50;
            //
            // afr_colMinimo
            //
            this.afr_colMinimo.Text = "Minimo";
            this.afr_colMinimo.Width = 60;
            //
            // afr_colAtivo
            //
            this.afr_colAtivo.Text = "Ativo";
            this.afr_colAtivo.Width = 50;
            //
            // afr_grpAulas
            //
            this.afr_grpAulas.Controls.Add(this.afr_lblAulaTitulo);
            this.afr_grpAulas.Controls.Add(this.afr_txtAulaTitulo);
            this.afr_grpAulas.Controls.Add(this.afr_lblAulaDuracao);
            this.afr_grpAulas.Controls.Add(this.afr_txtAulaDuracao);
            this.afr_grpAulas.Controls.Add(this.afr_btnAddAula);
            this.afr_grpAulas.Controls.Add(this.afr_btnDelAula);
            this.afr_grpAulas.Controls.Add(this.afr_lstAulas);
            this.afr_grpAulas.Controls.Add(this.afr_lblAulaConteudo);
            this.afr_grpAulas.Controls.Add(this.afr_txtAulaConteudo);
            this.afr_grpAulas.Controls.Add(this.afr_btnSalvarAula);
            this.afr_grpAulas.Controls.Add(this.afr_lblDicaConteudo);
            this.afr_grpAulas.Location = new System.Drawing.Point(25, 384);
            this.afr_grpAulas.Name = "afr_grpAulas";
            this.afr_grpAulas.Size = new System.Drawing.Size(850, 230);
            this.afr_grpAulas.TabIndex = 6;
            this.afr_grpAulas.TabStop = false;
            this.afr_grpAulas.Text = "Aulas do curso selecionado";
            //
            // afr_lblAulaTitulo
            //
            this.afr_lblAulaTitulo.AutoSize = true;
            this.afr_lblAulaTitulo.Location = new System.Drawing.Point(20, 28);
            this.afr_lblAulaTitulo.Name = "afr_lblAulaTitulo";
            this.afr_lblAulaTitulo.Size = new System.Drawing.Size(70, 15);
            this.afr_lblAulaTitulo.TabIndex = 0;
            this.afr_lblAulaTitulo.Text = "Nova aula:";
            //
            // afr_txtAulaTitulo
            //
            this.afr_txtAulaTitulo.Location = new System.Drawing.Point(100, 25);
            this.afr_txtAulaTitulo.MaxLength = 120;
            this.afr_txtAulaTitulo.Name = "afr_txtAulaTitulo";
            this.afr_txtAulaTitulo.Size = new System.Drawing.Size(320, 23);
            this.afr_txtAulaTitulo.TabIndex = 1;
            //
            // afr_lblAulaDuracao
            //
            this.afr_lblAulaDuracao.AutoSize = true;
            this.afr_lblAulaDuracao.Location = new System.Drawing.Point(435, 28);
            this.afr_lblAulaDuracao.Name = "afr_lblAulaDuracao";
            this.afr_lblAulaDuracao.Size = new System.Drawing.Size(80, 15);
            this.afr_lblAulaDuracao.TabIndex = 2;
            this.afr_lblAulaDuracao.Text = "Duracao (min):";
            //
            // afr_txtAulaDuracao
            //
            this.afr_txtAulaDuracao.Location = new System.Drawing.Point(525, 25);
            this.afr_txtAulaDuracao.MaxLength = 4;
            this.afr_txtAulaDuracao.Name = "afr_txtAulaDuracao";
            this.afr_txtAulaDuracao.Size = new System.Drawing.Size(70, 23);
            this.afr_txtAulaDuracao.TabIndex = 3;
            this.afr_txtAulaDuracao.Text = "45";
            this.afr_txtAulaDuracao.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            //
            // afr_btnAddAula
            //
            this.afr_btnAddAula.BackColor = System.Drawing.Color.Honeydew;
            this.afr_btnAddAula.Location = new System.Drawing.Point(610, 24);
            this.afr_btnAddAula.Name = "afr_btnAddAula";
            this.afr_btnAddAula.Size = new System.Drawing.Size(115, 26);
            this.afr_btnAddAula.TabIndex = 4;
            this.afr_btnAddAula.Text = "Incluir aula";
            this.afr_btnAddAula.UseVisualStyleBackColor = false;
            this.afr_btnAddAula.Click += new System.EventHandler(this.afr_btnAddAula_Click);
            //
            // afr_btnDelAula
            //
            this.afr_btnDelAula.Location = new System.Drawing.Point(735, 24);
            this.afr_btnDelAula.Name = "afr_btnDelAula";
            this.afr_btnDelAula.Size = new System.Drawing.Size(95, 26);
            this.afr_btnDelAula.TabIndex = 5;
            this.afr_btnDelAula.Text = "Excluir aula";
            this.afr_btnDelAula.UseVisualStyleBackColor = true;
            this.afr_btnDelAula.Click += new System.EventHandler(this.afr_btnDelAula_Click);
            //
            // afr_lstAulas
            //
            this.afr_lstAulas.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.afr_colAulaId,
            this.afr_colAulaOrdem,
            this.afr_colAulaTitulo,
            this.afr_colAulaDuracao});
            this.afr_lstAulas.FullRowSelect = true;
            this.afr_lstAulas.GridLines = true;
            this.afr_lstAulas.Location = new System.Drawing.Point(20, 58);
            this.afr_lstAulas.MultiSelect = false;
            this.afr_lstAulas.Name = "afr_lstAulas";
            this.afr_lstAulas.Size = new System.Drawing.Size(400, 160);
            this.afr_lstAulas.TabIndex = 6;
            this.afr_lstAulas.UseCompatibleStateImageBehavior = false;
            this.afr_lstAulas.View = System.Windows.Forms.View.Details;
            this.afr_lstAulas.SelectedIndexChanged += new System.EventHandler(this.afr_lstAulas_SelectedIndexChanged);
            //
            // afr_colAulaId
            //
            this.afr_colAulaId.Text = "Id";
            this.afr_colAulaId.Width = 40;
            //
            // afr_colAulaOrdem
            //
            this.afr_colAulaOrdem.Text = "Ordem";
            this.afr_colAulaOrdem.Width = 60;
            //
            // afr_colAulaTitulo
            //
            this.afr_colAulaTitulo.Text = "Titulo da aula";
            this.afr_colAulaTitulo.Width = 210;
            //
            // afr_colAulaDuracao
            //
            this.afr_colAulaDuracao.Text = "Duracao";
            this.afr_colAulaDuracao.Width = 70;
            //
            // afr_lblAulaConteudo
            //
            this.afr_lblAulaConteudo.AutoSize = true;
            this.afr_lblAulaConteudo.Location = new System.Drawing.Point(440, 58);
            this.afr_lblAulaConteudo.Name = "afr_lblAulaConteudo";
            this.afr_lblAulaConteudo.Size = new System.Drawing.Size(230, 15);
            this.afr_lblAulaConteudo.TabIndex = 7;
            this.afr_lblAulaConteudo.Text = "Conteudo da aula selecionada:";
            //
            // afr_txtAulaConteudo
            //
            this.afr_txtAulaConteudo.Location = new System.Drawing.Point(440, 78);
            this.afr_txtAulaConteudo.Multiline = true;
            this.afr_txtAulaConteudo.Name = "afr_txtAulaConteudo";
            this.afr_txtAulaConteudo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.afr_txtAulaConteudo.Size = new System.Drawing.Size(390, 112);
            this.afr_txtAulaConteudo.TabIndex = 8;
            //
            // afr_btnSalvarAula
            //
            this.afr_btnSalvarAula.BackColor = System.Drawing.Color.LightSteelBlue;
            this.afr_btnSalvarAula.Location = new System.Drawing.Point(440, 196);
            this.afr_btnSalvarAula.Name = "afr_btnSalvarAula";
            this.afr_btnSalvarAula.Size = new System.Drawing.Size(200, 26);
            this.afr_btnSalvarAula.TabIndex = 9;
            this.afr_btnSalvarAula.Text = "Salvar conteudo da aula";
            this.afr_btnSalvarAula.UseVisualStyleBackColor = false;
            this.afr_btnSalvarAula.Click += new System.EventHandler(this.afr_btnSalvarAula_Click);
            //
            // afr_lblDicaConteudo
            //
            this.afr_lblDicaConteudo.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblDicaConteudo.Location = new System.Drawing.Point(650, 200);
            this.afr_lblDicaConteudo.Name = "afr_lblDicaConteudo";
            this.afr_lblDicaConteudo.Size = new System.Drawing.Size(180, 20);
            this.afr_lblDicaConteudo.TabIndex = 10;
            this.afr_lblDicaConteudo.Text = "E o que o aluno le na aula.";
            //
            // afr_lblStatus
            //
            this.afr_lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblStatus.Location = new System.Drawing.Point(25, 624);
            this.afr_lblStatus.Name = "afr_lblStatus";
            this.afr_lblStatus.Size = new System.Drawing.Size(690, 34);
            this.afr_lblStatus.TabIndex = 7;
            this.afr_lblStatus.Text = "Pronto.";
            //
            // afr_btnFechar
            //
            this.afr_btnFechar.Location = new System.Drawing.Point(735, 622);
            this.afr_btnFechar.Name = "afr_btnFechar";
            this.afr_btnFechar.Size = new System.Drawing.Size(140, 32);
            this.afr_btnFechar.TabIndex = 8;
            this.afr_btnFechar.Text = "Fechar";
            this.afr_btnFechar.UseVisualStyleBackColor = true;
            this.afr_btnFechar.Click += new System.EventHandler(this.afr_btnFechar_Click);
            //
            // FrmCursos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 668);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_grpCurso);
            this.Controls.Add(this.afr_lblBusca);
            this.Controls.Add(this.afr_txtBusca);
            this.Controls.Add(this.afr_btnBuscar);
            this.Controls.Add(this.afr_lstCursos);
            this.Controls.Add(this.afr_grpAulas);
            this.Controls.Add(this.afr_lblStatus);
            this.Controls.Add(this.afr_btnFechar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmCursos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Gerenciar Cursos e Aulas";
            this.Load += new System.EventHandler(this.FrmCursos_Load);
            ((System.ComponentModel.ISupportInitialize)(this.afr_numMinimo)).EndInit();
            this.afr_grpCurso.ResumeLayout(false);
            this.afr_grpCurso.PerformLayout();
            this.afr_grpAulas.ResumeLayout(false);
            this.afr_grpAulas.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblTitulo;
        private System.Windows.Forms.GroupBox afr_grpCurso;
        private System.Windows.Forms.Label afr_lblModo;
        private System.Windows.Forms.Label afr_lblTituloCurso;
        private System.Windows.Forms.TextBox afr_txtTitulo;
        private System.Windows.Forms.Label afr_lblDescricao;
        private System.Windows.Forms.TextBox afr_txtDescricao;
        private System.Windows.Forms.Label afr_lblCarga;
        private System.Windows.Forms.TextBox afr_txtCarga;
        private System.Windows.Forms.CheckBox afr_chkAtivoCurso;
        private System.Windows.Forms.Label afr_lblMinimo;
        private System.Windows.Forms.NumericUpDown afr_numMinimo;
        private System.Windows.Forms.Label afr_lblMinimoDica;
        private System.Windows.Forms.Button afr_btnNovoCurso;
        private System.Windows.Forms.Button afr_btnSalvarCurso;
        private System.Windows.Forms.Button afr_btnExcluirCurso;
        private System.Windows.Forms.Label afr_lblAvisoCurso;
        private System.Windows.Forms.Label afr_lblBusca;
        private System.Windows.Forms.TextBox afr_txtBusca;
        private System.Windows.Forms.Button afr_btnBuscar;
        private System.Windows.Forms.ListView afr_lstCursos;
        private System.Windows.Forms.ColumnHeader afr_colId;
        private System.Windows.Forms.ColumnHeader afr_colTitulo;
        private System.Windows.Forms.ColumnHeader afr_colCarga;
        private System.Windows.Forms.ColumnHeader afr_colAulas;
        private System.Windows.Forms.ColumnHeader afr_colMinimo;
        private System.Windows.Forms.ColumnHeader afr_colAtivo;
        private System.Windows.Forms.GroupBox afr_grpAulas;
        private System.Windows.Forms.Label afr_lblAulaTitulo;
        private System.Windows.Forms.TextBox afr_txtAulaTitulo;
        private System.Windows.Forms.Label afr_lblAulaDuracao;
        private System.Windows.Forms.TextBox afr_txtAulaDuracao;
        private System.Windows.Forms.Button afr_btnAddAula;
        private System.Windows.Forms.Button afr_btnDelAula;
        private System.Windows.Forms.ListView afr_lstAulas;
        private System.Windows.Forms.ColumnHeader afr_colAulaId;
        private System.Windows.Forms.ColumnHeader afr_colAulaOrdem;
        private System.Windows.Forms.ColumnHeader afr_colAulaTitulo;
        private System.Windows.Forms.ColumnHeader afr_colAulaDuracao;
        private System.Windows.Forms.Label afr_lblAulaConteudo;
        private System.Windows.Forms.TextBox afr_txtAulaConteudo;
        private System.Windows.Forms.Button afr_btnSalvarAula;
        private System.Windows.Forms.Label afr_lblDicaConteudo;
        private System.Windows.Forms.Label afr_lblStatus;
        private System.Windows.Forms.Button afr_btnFechar;
    }
}
