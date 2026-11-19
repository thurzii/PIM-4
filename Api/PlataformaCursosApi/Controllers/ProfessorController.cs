/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Controllers/ProfessorController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   Porte do professorRoutes.js da API do Gui. Todas as rotas sao so do
   PROFESSOR e so enxergam os cursos dele (Cursos.ProfessorId).

   GET    /api/professor/cursos
   POST   /api/professor/cursos
   PUT    /api/professor/cursos/:id
   GET    /api/professor/cursos/:id/aulas
   POST   /api/professor/cursos/:id/aulas
   DELETE /api/professor/aulas/:id
   GET    /api/professor/cursos/:id/alunos

   A MAIS (as telas do C# precisam):
   PUT    /api/professor/aulas/:id      editar titulo e duracao
   ===================================================================== */

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PlataformaCursosApi.Infra;

namespace PlataformaCursosApi.Controllers
{
    [Route("api/professor")]
    [Autenticado("PROFESSOR")]
    public class ProfessorController : ControladorBase
    {
        public ProfessorController(Banco afr_b) : base(afr_b) { }

        [HttpGet("cursos")]
        public IActionResult ListarCursos()
        {
            try
            {
                return Ok(afr_banco.Consultar(@"
                    SELECT
                      c.Id AS id,
                      c.Titulo AS titulo,
                      c.Descricao AS descricao,
                      c.CargaHoraria AS cargaHoraria,
                      c.Ativo AS ativo,
                      (SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId = c.Id) AS totalAulas,
                      (SELECT COUNT(*) FROM dbo.Matriculas m WHERE m.CursoId = c.Id) AS totalAlunos
                    FROM dbo.Cursos c
                    WHERE c.ProfessorId = @professorId
                    ORDER BY c.Titulo", Parametros("@professorId", Usuario.afr_Id)));
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao listar cursos.", afr_erro);
            }
        }

        [HttpPost("cursos")]
        public async Task<IActionResult> CriarCurso()
        {
            string afr_erroValidacao;
            ValidarCurso(await LerCorpo(), out string afr_titulo, out string afr_descricao,
                         out int afr_carga, out afr_erroValidacao);

            if (afr_erroValidacao != null)
            {
                return Mensagem(400, afr_erroValidacao);
            }

            try
            {
                object afr_id = afr_banco.Escalar(@"
                    INSERT INTO dbo.Cursos (Titulo, Descricao, CargaHoraria, Ativo, DataCadastro, ProfessorId)
                    OUTPUT INSERTED.Id
                    VALUES (@titulo, @descricao, @cargaHoraria, 1, GETDATE(), @professorId)",
                    Parametros("@titulo", afr_titulo, "@descricao", afr_descricao,
                               "@cargaHoraria", afr_carga, "@professorId", Usuario.afr_Id));

                return StatusCode(201, new { id = Convert.ToInt32(afr_id), message = "Curso criado." });
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao criar curso.", afr_erro);
            }
        }

        [HttpPut("cursos/{id}")]
        public async Task<IActionResult> AlterarCurso(string id)
        {
            int afr_id = IdValido(id);
            string afr_erroValidacao;
            ValidarCurso(await LerCorpo(), out string afr_titulo, out string afr_descricao,
                         out int afr_carga, out afr_erroValidacao);

            if (afr_id == 0)
            {
                return Mensagem(400, "Identificador de curso invalido.");
            }

            if (afr_erroValidacao != null)
            {
                return Mensagem(400, afr_erroValidacao);
            }

            try
            {
                int afr_linhas = afr_banco.Executar(@"
                    UPDATE dbo.Cursos
                    SET Titulo = @titulo, Descricao = @descricao, CargaHoraria = @cargaHoraria
                    WHERE Id = @id AND ProfessorId = @professorId",
                    Parametros("@id", afr_id, "@professorId", Usuario.afr_Id, "@titulo", afr_titulo,
                               "@descricao", afr_descricao, "@cargaHoraria", afr_carga));

                if (afr_linhas == 0)
                {
                    return Mensagem(404, "Curso nao encontrado ou voce nao e o responsavel.");
                }

                return Mensagem(200, "Curso atualizado.");
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao atualizar curso.", afr_erro);
            }
        }

        [HttpGet("cursos/{id}/aulas")]
        public IActionResult ListarAulas(string id)
        {
            int afr_cursoId = IdValido(id);

            if (afr_cursoId == 0)
            {
                return Mensagem(400, "Identificador de curso invalido.");
            }

            try
            {
                if (!EhDono(afr_cursoId))
                {
                    return Mensagem(404, "Curso nao encontrado ou voce nao e o responsavel.");
                }

                return Ok(afr_banco.Consultar(@"
                    SELECT a.Id AS id, a.Titulo AS titulo, a.Ordem AS ordem, a.DuracaoMin AS duracaoMin
                    FROM dbo.Aulas a
                    WHERE a.CursoId = @cursoId
                    ORDER BY a.Ordem", Parametros("@cursoId", afr_cursoId)));
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao listar aulas.", afr_erro);
            }
        }

        [HttpPost("cursos/{id}/aulas")]
        public async Task<IActionResult> CriarAula(string id)
        {
            int afr_cursoId = IdValido(id);
            JsonElement afr_corpo = await LerCorpo();
            string afr_erroValidacao;
            ValidarAula(afr_corpo, out string afr_titulo, out int afr_duracao, out afr_erroValidacao);

            if (afr_cursoId == 0)
            {
                return Mensagem(400, "Identificador de curso invalido.");
            }

            if (afr_erroValidacao != null)
            {
                return Mensagem(400, afr_erroValidacao);
            }

            try
            {
                if (!EhDono(afr_cursoId))
                {
                    return Mensagem(404, "Curso nao encontrado ou voce nao e o responsavel.");
                }

                // A ordem nao vem no corpo: a proxima livre do curso
                object afr_id = afr_banco.Escalar(@"
                    DECLARE @ordem INT = (SELECT ISNULL(MAX(Ordem), 0) + 1 FROM dbo.Aulas WHERE CursoId = @cursoId);
                    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin)
                    OUTPUT INSERTED.Id
                    VALUES (@cursoId, @titulo, @ordem, @duracaoMin);",
                    Parametros("@cursoId", afr_cursoId, "@titulo", afr_titulo, "@duracaoMin", afr_duracao));

                return StatusCode(201, new { id = Convert.ToInt32(afr_id), message = "Aula criada." });
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao criar aula.", afr_erro);
            }
        }

        // A MAIS: editar titulo e duracao (o banco nao tem coluna de
        // conteudo da aula, entao o texto nao entra aqui)
        [HttpPut("aulas/{id}")]
        public async Task<IActionResult> AlterarAula(string id)
        {
            int afr_aulaId = IdValido(id);
            string afr_erroValidacao;
            ValidarAula(await LerCorpo(), out string afr_titulo, out int afr_duracao, out afr_erroValidacao);

            if (afr_aulaId == 0)
            {
                return Mensagem(400, "Identificador de aula invalido.");
            }

            if (afr_erroValidacao != null)
            {
                return Mensagem(400, afr_erroValidacao);
            }

            try
            {
                int afr_linhas = afr_banco.Executar(@"
                    UPDATE a SET a.Titulo = @titulo, a.DuracaoMin = @duracaoMin
                    FROM dbo.Aulas a
                    INNER JOIN dbo.Cursos c ON c.Id = a.CursoId
                    WHERE a.Id = @aulaId AND c.ProfessorId = @professorId",
                    Parametros("@aulaId", afr_aulaId, "@professorId", Usuario.afr_Id,
                               "@titulo", afr_titulo, "@duracaoMin", afr_duracao));

                if (afr_linhas == 0)
                {
                    return Mensagem(404, "Aula nao encontrada ou voce nao e o responsavel.");
                }

                return Mensagem(200, "Aula atualizada.");
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao atualizar aula.", afr_erro);
            }
        }

        [HttpDelete("aulas/{id}")]
        public IActionResult ExcluirAula(string id)
        {
            int afr_aulaId = IdValido(id);

            if (afr_aulaId == 0)
            {
                return Mensagem(400, "Identificador de aula invalido.");
            }

            try
            {
                // Progresso primeiro: a FK de Progresso para Aulas nao
                // apaga em cascata
                using (var afr_cn = afr_banco.Abrir())
                using (var afr_tx = afr_cn.BeginTransaction())
                {
                    List<Dictionary<string, object>> afr_dono = afr_banco.Consultar(@"
                        SELECT a.Id FROM dbo.Aulas a
                        INNER JOIN dbo.Cursos c ON c.Id = a.CursoId
                        WHERE a.Id = @aulaId AND c.ProfessorId = @professorId",
                        Parametros("@aulaId", afr_aulaId, "@professorId", Usuario.afr_Id), afr_cn, afr_tx);

                    if (afr_dono.Count == 0)
                    {
                        afr_tx.Rollback();
                        return Mensagem(404, "Aula nao encontrada ou voce nao e o responsavel.");
                    }

                    afr_banco.Executar("DELETE FROM dbo.Progresso WHERE AulaId = @aulaId",
                                       Parametros("@aulaId", afr_aulaId), afr_cn, afr_tx);
                    afr_banco.Executar("DELETE FROM dbo.Aulas WHERE Id = @aulaId",
                                       Parametros("@aulaId", afr_aulaId), afr_cn, afr_tx);
                    afr_tx.Commit();
                }

                return Mensagem(200, "Aula excluida.");
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao excluir aula.", afr_erro);
            }
        }

        [HttpGet("cursos/{id}/alunos")]
        public IActionResult ListarAlunos(string id)
        {
            int afr_cursoId = IdValido(id);

            if (afr_cursoId == 0)
            {
                return Mensagem(400, "Identificador de curso invalido.");
            }

            try
            {
                if (!EhDono(afr_cursoId))
                {
                    return Mensagem(404, "Curso nao encontrado ou voce nao e o responsavel.");
                }

                return Ok(afr_banco.Consultar(@"
                    SELECT
                      u.Id AS id,
                      u.Nome AS nome,
                      u.Email AS email,
                      m.DataMatricula AS dataMatricula,
                      COUNT(a.Id) AS totalAulas,
                      COUNT(p.Id) AS aulasConcluidas,
                      CASE WHEN COUNT(a.Id) = 0 THEN 0
                           ELSE CAST(ROUND(COUNT(p.Id) * 100.0 / COUNT(a.Id), 0) AS INT)
                      END AS progresso
                    FROM dbo.Cursos c
                    INNER JOIN dbo.Matriculas m ON m.CursoId = c.Id
                    INNER JOIN dbo.Usuarios u ON u.Id = m.UsuarioId
                    LEFT JOIN dbo.Aulas a ON a.CursoId = c.Id
                    LEFT JOIN dbo.Progresso p ON p.AulaId = a.Id AND p.UsuarioId = u.Id
                    WHERE c.Id = @cursoId
                    GROUP BY u.Id, u.Nome, u.Email, m.DataMatricula
                    ORDER BY u.Nome", Parametros("@cursoId", afr_cursoId)));
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao listar alunos.", afr_erro);
            }
        }

        private bool EhDono(int afr_cursoId)
        {
            return afr_banco.Escalar("SELECT Id FROM dbo.Cursos WHERE Id = @cursoId AND ProfessorId = @professorId",
                                     Parametros("@cursoId", afr_cursoId, "@professorId", Usuario.afr_Id)) != null;
        }

        // validateCourse da API do Gui
        public static void ValidarCurso(JsonElement afr_corpo, out string afr_titulo, out string afr_descricao,
                                        out int afr_carga, out string afr_erro)
        {
            afr_titulo = (Texto(afr_corpo, "titulo") ?? "").Trim();
            afr_descricao = (Texto(afr_corpo, "descricao") ?? "").Trim();
            int? afr_cargaLida = Inteiro(afr_corpo, "cargaHoraria");
            afr_carga = afr_cargaLida ?? 0;
            afr_erro = null;

            if (afr_titulo == "" || afr_titulo.Length > 200)
            {
                afr_erro = "Informe um titulo com no maximo 200 caracteres.";
            }
            else if (afr_cargaLida == null || afr_cargaLida < 0)
            {
                afr_erro = "A carga horaria deve ser um numero inteiro igual ou maior que zero.";
            }
        }

        // A validacao de aula da API do Gui (professorRoutes.js)
        public static void ValidarAula(JsonElement afr_corpo, out string afr_titulo, out int afr_duracao,
                                       out string afr_erro)
        {
            afr_titulo = (Texto(afr_corpo, "titulo") ?? "").Trim();
            int? afr_duracaoLida = Inteiro(afr_corpo, "duracaoMin");
            afr_duracao = afr_duracaoLida ?? 0;
            afr_erro = null;

            if (afr_titulo == "" || afr_titulo.Length > 300)
            {
                afr_erro = "Informe um titulo de aula com no maximo 300 caracteres.";
            }
            else if (afr_duracaoLida == null || afr_duracaoLida < 0)
            {
                afr_erro = "A duracao deve ser um numero inteiro igual ou maior que zero.";
            }
        }
    }
}
