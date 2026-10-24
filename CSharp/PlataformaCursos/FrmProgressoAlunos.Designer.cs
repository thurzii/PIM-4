/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: FrmProgressoAlunos.Designer.cs
   ---------------------------------------------------------------------
   Arquivo do DESIGNER da tela de progresso dos alunos.
   ===================================================================== */

namespace PlataformaCursos
{
    partial class FrmProgressoAlunos
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
            this.afr_lblInstrucao = new System.Windows.Forms.Label();
            this.afr_lblBusca = new System.Windows.Forms.Label();
            this.afr_txtBusca = new System.Windows.Forms.TextBox();
            this.afr_btnBuscar = new System.Windows.Forms.Button();
            this.afr_lblAluno = new System.Windows.Forms.Label();
            this.afr_cmbAlunos = new System.Windows.Forms.ComboBox();
            this.afr_btnVer = new System.Windows.Forms.Button();
            this.afr_lblDados = new System.Windows.Forms.Label();
            this.afr_lstProgresso = new System.Windows.Forms.ListView();
            this.afr_colCurso = new System.Windows.Forms.ColumnHeader();
            this.afr_colAulas = new System.Windows.Forms.ColumnHeader();
            this.afr_colPercentual = new System.Windows.Forms.ColumnHeader();
            this.afr_colMinimo = new System.Windows.Forms.ColumnHeader();
            this.afr_colStatus = new System.Windows.Forms.ColumnHeader();
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
            this.afr_lblTitulo.Size = new System.Drawing.Size(250, 25);
            this.afr_lblTitulo.TabIndex = 0;
            this.afr_lblTitulo.Text = "Progresso dos alunos";
            //
            // afr_lblInstrucao
            //
            this.afr_lblInstrucao.ForeColor = System.Drawing.Color.DimGray;
            this.afr_lblInstrucao.Location = new System.Drawing.Point(20, 42);
            this.afr_lblInstrucao.Name = "afr_lblInstrucao";
            this.afr_lblInstrucao.Size = new System.Drawing.Size(700, 34);
            this.afr_lblInstrucao.TabIndex = 1;
            this.afr_lblInstrucao.Text = "Escolha um aluno para acompanhar o andamento dele em cada curso.";
            //
            // afr_lblBusca
            //
            this.afr_lblBusca.AutoSize = true;
            this.afr_lblBusca.Location = new System.Drawing.Point(20, 84);
            this.afr_lblBusca.Name = "afr_lblBusca";
            this.afr_lblBusca.Size = new System.Drawing.Size(85, 15);
            this.afr_lblBusca.TabIndex = 2;
            this.afr_lblBusca.Text = "Buscar aluno:";
            //
            // afr_txtBusca
            //
            this.afr_txtBusca.Location = new System.Drawing.Point(112, 80);
            this.afr_txtBusca.MaxLength = 100;
            this.afr_txtBusca.Name = "afr_txtBusca";
            this.afr_txtBusca.Size = new System.Drawing.Size(228, 23);
            this.afr_txtBusca.TabIndex = 3;
            //
            // afr_btnBuscar
            //
            this.afr_btnBuscar.Location = new System.Drawing.Point(350, 79);
            this.afr_btnBuscar.Name = "afr_btnBuscar";
            this.afr_btnBuscar.Size = new System.Drawing.Size(110, 26);
            this.afr_btnBuscar.TabIndex = 4;
            this.afr_btnBuscar.Text = "Buscar";
            this.afr_btnBuscar.UseVisualStyleBackColor = true;
            this.afr_btnBuscar.Click += new System.EventHandler(this.afr_btnBuscar_Click);
            //
            // afr_lblAluno
            //
            this.afr_lblAluno.AutoSize = true;
            this.afr_lblAluno.Location = new System.Drawing.Point(20, 118);
            this.afr_lblAluno.Name = "afr_lblAluno";
            this.afr_lblAluno.Size = new System.Drawing.Size(45, 15);
            this.afr_lblAluno.TabIndex = 5;
            this.afr_lblAluno.Text = "Aluno:";
            //
            // afr_cmbAlunos
            //
            this.afr_cmbAlunos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.afr_cmbAlunos.Location = new System.Drawing.Point(20, 138);
            this.afr_cmbAlunos.Name = "afr_cmbAlunos";
            this.afr_cmbAlunos.Size = new System.Drawing.Size(420, 23);
            this.afr_cmbAlunos.TabIndex = 6;
            //
            // afr_btnVer
            //
            this.afr_btnVer.BackColor = System.Drawing.Color.AliceBlue;
            this.afr_btnVer.Location = new System.Drawing.Point(450, 137);
            this.afr_btnVer.Name = "afr_btnVer";
            this.afr_btnVer.Size = new System.Drawing.Size(180, 26);
            this.afr_btnVer.TabIndex = 7;
            this.afr_btnVer.Text = "Ver progresso";
            this.afr_btnVer.UseVisualStyleBackColor = false;
            this.afr_btnVer.Click += new System.EventHandler(this.afr_btnVer_Click);
            //
            // afr_lblDados
            //
            this.afr_lblDados.Location = new System.Drawing.Point(20, 174);
            this.afr_lblDados.Name = "afr_lblDados";
            this.afr_lblDados.Size = new System.Drawing.Size(700, 40);
            this.afr_lblDados.TabIndex = 8;
            this.afr_lblDados.Text = "";
            //
            // afr_lstProgresso
            //
            this.afr_lstProgresso.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.afr_colCurso,
            this.afr_colAulas,
            this.afr_colPercentual,
            this.afr_colMinimo,
            this.afr_colStatus});
            this.afr_lstProgresso.FullRowSelect = true;
            this.afr_lstProgresso.GridLines = true;
            this.afr_lstProgresso.Location = new System.Drawing.Point(20, 220);
            this.afr_lstProgresso.MultiSelect = false;
            this.afr_lstProgresso.Name = "afr_lstProgresso";
            this.afr_lstProgresso.Size = new System.Drawing.Size(700, 240);
            this.afr_lstProgresso.TabIndex = 9;
            this.afr_lstProgresso.UseCompatibleStateImageBehavior = false;
            this.afr_lstProgresso.View = System.Windows.Forms.View.Details;
            //
            // afr_colCurso
            //
            this.afr_colCurso.Text = "Curso";
            this.afr_colCurso.Width = 250;
            //
            // afr_colAulas
            //
            this.afr_colAulas.Text = "Aulas";
            this.afr_colAulas.Width = 90;
            //
            // afr_colPercentual
            //
            this.afr_colPercentual.Text = "Percentual";
            this.afr_colPercentual.Width = 90;
            //
            // afr_colMinimo
            //
            this.afr_colMinimo.Text = "Minimo";
            this.afr_colMinimo.Width = 80;
            //
            // afr_colStatus
            //
            this.afr_colStatus.Text = "Status";
            this.afr_colStatus.Width = 170;
            //
            // afr_lblResumo
            //
            this.afr_lblResumo.Location = new System.Drawing.Point(20, 468);
            this.afr_lblResumo.Name = "afr_lblResumo";
            this.afr_lblResumo.Size = new System.Drawing.Size(700, 40);
            this.afr_lblResumo.TabIndex = 10;
            this.afr_lblResumo.Text = "";
            //
            // afr_btnFechar
            //
            this.afr_btnFechar.Location = new System.Drawing.Point(540, 516);
            this.afr_btnFechar.Name = "afr_btnFechar";
            this.afr_btnFechar.Size = new System.Drawing.Size(180, 40);
            this.afr_btnFechar.TabIndex = 11;
            this.afr_btnFechar.Text = "Fechar";
            this.afr_btnFechar.UseVisualStyleBackColor = true;
            this.afr_btnFechar.Click += new System.EventHandler(this.afr_btnFechar_Click);
            //
            // FrmProgressoAlunos
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.afr_btnFechar;
            this.ClientSize = new System.Drawing.Size(740, 572);
            this.Controls.Add(this.afr_lblTitulo);
            this.Controls.Add(this.afr_lblInstrucao);
            this.Controls.Add(this.afr_lblBusca);
            this.Controls.Add(this.afr_txtBusca);
            this.Controls.Add(this.afr_btnBuscar);
            this.Controls.Add(this.afr_lblAluno);
            this.Controls.Add(this.afr_cmbAlunos);
            this.Controls.Add(this.afr_btnVer);
            this.Controls.Add(this.afr_lblDados);
            this.Controls.Add(this.afr_lstProgresso);
            this.Controls.Add(this.afr_lblResumo);
            this.Controls.Add(this.afr_btnFechar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmProgressoAlunos";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Progresso dos alunos";
            this.Load += new System.EventHandler(this.FrmProgressoAlunos_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // =============================================================
        // DECLARACAO DOS CONTROLES DA TELA
        // =============================================================
        private System.Windows.Forms.Label afr_lblTitulo;
        private System.Windows.Forms.Label afr_lblInstrucao;
        private System.Windows.Forms.Label afr_lblBusca;
        private System.Windows.Forms.TextBox afr_txtBusca;
        private System.Windows.Forms.Button afr_btnBuscar;
        private System.Windows.Forms.Label afr_lblAluno;
        private System.Windows.Forms.ComboBox afr_cmbAlunos;
        private System.Windows.Forms.Button afr_btnVer;
        private System.Windows.Forms.Label afr_lblDados;
        private System.Windows.Forms.ListView afr_lstProgresso;
        private System.Windows.Forms.ColumnHeader afr_colCurso;
        private System.Windows.Forms.ColumnHeader afr_colAulas;
        private System.Windows.Forms.ColumnHeader afr_colPercentual;
        private System.Windows.Forms.ColumnHeader afr_colMinimo;
        private System.Windows.Forms.ColumnHeader afr_colStatus;
        private System.Windows.Forms.Label afr_lblResumo;
        private System.Windows.Forms.Button afr_btnFechar;
    }
}
