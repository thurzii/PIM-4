/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Controllers/ConsultasController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   Rotas SO DE LEITURA que as telas do desktop e da web precisam e a API
   do Gui nao tem. Nenhuma grava nada; todas usam tabelas que ja existem.

   GET /api/aulas/:id
       Uma aula, com o curso a que pertence e se quem pede ja a
       concluiu. A pagina da aula abre pelo id da aula; sem esta rota,
       seria preciso abrir curso por curso ate achar.

   GET /api/progresso/alunos/:id
       O progresso de UM aluno em cada curso (tela de progresso do
       professor e do admin). Professor ve so os cursos dele, como em
       todas as rotas de professor da API do Gui; admin ve todos.
   ===================================================================== */

using Microsoft.AspNetCore.Mvc;
using PlataformaCursosApi.Infra;

namespace PlataformaCursosApi.Controllers
{
    [Route("api")]
    public class ConsultasController : ControladorBase
    {
        public ConsultasController(Banco afr_b) : base(afr_b) { }

        [HttpGet("aulas/{id}")]
        [Autenticado]
        public IActionResult Aula(string id)
        {
            int afr_aulaId = IdValido(id);

            if (afr_aulaId == 0)
            {
                return Mensagem(400, "Identificador de aula invalido.");
            }

            try
            {
                List<Dictionary<string, object>> afr_linhas = afr_banco.Consultar(@"
                    SELECT a.Id AS id, a.CursoId AS cursoId, a.Titulo AS titulo, a.Ordem AS ordem,
                           a.DuracaoMin AS duracaoMin,
                           CASE WHEN p.Id IS NULL THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END AS concluida,
                           c.Ativo AS ativo, c.ProfessorId AS professorId
                    FROM dbo.Aulas a
                    INNER JOIN dbo.Cursos c ON c.Id = a.CursoId
                    LEFT JOIN dbo.Progresso p ON p.AulaId = a.Id AND p.UsuarioId = @usuarioId
                    WHERE a.Id = @aulaId",
                    Parametros("@aulaId", afr_aulaId, "@usuarioId", Usuario.afr_Id));

                // A mesma regra de acesso do curso: quem nao pode abrir o
                // curso tambem nao abre a aula dele
                if (afr_linhas.Count == 0 || !CursosController.PodeAcessar(Usuario, afr_linhas[0]))
                {
                    return Mensagem(404, "Aula nao encontrada.");
                }

                Dictionary<string, object> afr_aula = afr_linhas[0];
                afr_aula.Remove("ativo");
                afr_aula.Remove("professorId");

                return Ok(afr_aula);
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao carregar aula.", afr_erro);
            }
        }

        [HttpGet("progresso/alunos/{id}")]
        [Autenticado("ADMIN", "PROFESSOR", Mensagem = "Acesso permitido apenas a professores e administradores.")]
        public IActionResult ProgressoDoAluno(string id)
        {
            int afr_alunoId = IdValido(id);

            if (afr_alunoId == 0)
            {
                return Mensagem(400, "Identificador de aluno invalido.");
            }

            bool afr_ehProfessor = Usuario.afr_Perfil == "PROFESSOR";

            try
            {
                // Mesmo calculo de progresso da API do Gui (concluidas
                // sobre o total de aulas, arredondado)
                return Ok(afr_banco.Consultar(@"
                    SELECT
                      c.Id AS id,
                      c.Titulo AS titulo,
                      c.Descricao AS descricao,
                      c.CargaHoraria AS cargaHoraria,
                      c.Ativo AS ativo,
                      COUNT(a.Id) AS totalAulas,
                      COUNT(p.Id) AS concluidas,
                      CASE WHEN COUNT(a.Id) = 0 THEN 0
                           ELSE CAST(ROUND(COUNT(p.Id) * 100.0 / COUNT(a.Id), 0) AS INT)
                      END AS progresso,
                      CASE WHEN EXISTS (SELECT 1 FROM dbo.Matriculas m
                                        WHERE m.CursoId = c.Id AND m.UsuarioId = @alunoId)
                           THEN CAST(1 AS bit) ELSE CAST(0 AS bit) END AS matriculado
                    FROM dbo.Cursos c
                    LEFT JOIN dbo.Aulas a ON a.CursoId = c.Id
                    LEFT JOIN dbo.Progresso p ON p.AulaId = a.Id AND p.UsuarioId = @alunoId
                    WHERE c.Ativo = 1
                      AND (@professorId = 0 OR c.ProfessorId = @professorId)
                    GROUP BY c.Id, c.Titulo, c.Descricao, c.CargaHoraria, c.Ativo
                    ORDER BY c.Titulo",
                    Parametros("@alunoId", afr_alunoId, "@professorId", afr_ehProfessor ? Usuario.afr_Id : 0)));
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao carregar progresso.", afr_erro);
            }
        }
    }
}
