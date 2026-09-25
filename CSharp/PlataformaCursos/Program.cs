/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: Program.cs  (ponto de entrada da aplicacao)
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   O Main prepara o ambiente grafico e abre a primeira tela: o LOGIN.
   A partir dai quem comanda o programa sao os eventos dos botoes.

   FLUXO DAS TELAS:
     FrmLogin  ->  FrmPrincipal  ->  FrmCatalogo  (aluno)
                                 ->  FrmCursos    (admin)
                                 ->  FrmUsuarios  (admin)
   ===================================================================== */

using System;
using System.Windows.Forms;

namespace PlataformaCursos
{
    internal static class Program
    {
        // [STAThread] e obrigatorio em aplicacoes Windows Forms
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new FrmLogin());
        }
    }
}
