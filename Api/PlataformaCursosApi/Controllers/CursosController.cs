/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Controllers/CursosController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   GET  /api/cursos                                  catalogo
   GET  /api/cursos/:id                              curso, aulas e progresso
   POST /api/cursos/:id/matricula                    (so aluno)
   PUT  /api/cursos/:cursoId/aulas/:aulaId/progresso (so aluno)

   Porte do courseRoutes.js da API do Gui: mesmas consultas, mesmas
   regras de acesso, mesmas mensagens.

   UNICA DIFERENCA: o catalogo (GET /api/cursos)
   Na API do Gui ele e so de aluno - o aplicativo e do aluno. As telas
   do desktop e da web tambem tem catalogo para professor e admin. Aqui:
     aluno ..... os cursos ativos (igual a dele)
     professor . os cursos DELE (os unicos que ele pode abrir: a regra
                 de acesso ao curso, abaixo, e a mesma da API do Gui)
     admin ..... todos os ativos
   E so leitura: nao muda nenhum dado do banco.
   ===================================================================== */

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PlataformaCursosApi.Infra;

namespace PlataformaCursosApi.Controllers
{
    [Route("api/cursos")]
    public class CursosController : ControladorBase
    {
        public CursosController(Banco afr_b) : base(afr_b) { }

        [HttpGet("")]
        [Autenticado]
        public IActionResult Listar([FromQuery] string busca)
        {
            string afr_termo = (busca ?? "").Trim();
            bool afr_ehProfessor = Usuario.afr_Perfil == "PROFESSOR";

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
                    WHERE c.Ativo = 1
                      AND (@professorId = 0 OR c.ProfessorId = @professorId)
                      AND (
                        @termo = ''
                        OR c.Titulo LIKE '%' + @termo + '%'
                        OR c.Descricao LIKE '%' + @termo + '%'
                      )
                    ORDER BY c.Titulo",
                    Parametros("@termo", afr_termo, "@professorId", afr_ehProfessor ? Usuario.afr_Id : 0)));
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao listar cursos.", afr_erro);
            }
        }

        [HttpGet("{id}")]
        [Autenticado]
        public IActionResult Obter(string id)
        {
            int afr_id = IdValido(id);

            if (afr_id == 0)
            {
                return Mensagem(400, "Identificador de curso invalido.");
            }

            try
            {
                List<Dictionary<string, object>> afr_cursos = afr_banco.Consultar(@"
                    SELECT Id AS id, Titulo AS titulo, Descricao AS descricao,
                           CargaHoraria AS cargaHoraria, Ativo AS ativo, ProfessorId AS professorId
                    FROM dbo.Cursos WHERE Id = @id", Parametros("@id", afr_id));

                if (afr_cursos.Count == 0)
                {
                    return Mensagem(404, "Curso nao encontrado.");
                }

                Dictionary<string, object> afr_curso = afr_cursos[0];

                if (!PodeAcessar(Usuario, afr_curso))
                {
                    return Usuario.afr_Perfil == "PROFESSOR"
                        ? Mensagem(403, "Voce nao e o responsavel por este curso.")
                        : Mensagem(404, "Curso nao encontrado ou indisponivel.");
                }

                List<Dictionary<string, object>> afr_aulas = afr_banco.Consultar(@"
                    SELECT
                      a.Id AS id,
                      a.Titulo AS titulo,
                      a.Ordem AS ordem,
                      a.DuracaoMin AS duracaoMin,
                      CASE WHEN p.Id IS NULL THEN CAST(0 AS bit) ELSE CAST(1 AS bit) END AS concluida
                    FROM dbo.Aulas a
                    LEFT JOIN dbo.Progresso p ON p.AulaId = a.Id AND p.UsuarioId = @usuarioId
                    WHERE a.CursoId = @cursoId
                    ORDER BY a.Ordem",
                    Parametros("@cursoId", afr_id, "@usuarioId", Usuario.afr_Id));

                object afr_matricula = afr_banco.Escalar(
                    "SELECT Id FROM dbo.Matriculas WHERE CursoId = @cursoId AND UsuarioId = @usuarioId",
                    Parametros("@cursoId", afr_id, "@usuarioId", Usuario.afr_Id));

                int afr_total = afr_aulas.Count;
                int afr_feitas = afr_aulas.Count(a => Convert.ToBoolean(a["concluida"]));

                Dictionary<string, object> afr_resposta = new Dictionary<string, object>(afr_curso);
                afr_resposta["matriculado"] = afr_matricula != null;
                afr_resposta["aulas"] = afr_aulas;
                afr_resposta["progresso"] = afr_total > 0
                    ? (int)Math.Round(afr_feitas * 100.0 / afr_total, MidpointRounding.AwayFromZero)
                    : 0;
                afr_resposta["concluidas"] = afr_feitas;
                afr_resposta["totalAulas"] = afr_total;

                return Ok(afr_resposta);
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao carregar curso.", afr_erro);
            }
        }

        [HttpPost("{id}/matricula")]
        [Autenticado("ALUNO")]
        public IActionResult Matricular(string id)
        {
            int afr_cursoId = IdValido(id);

            if (afr_cursoId == 0)
            {
                return Mensagem(400, "Identificador de curso invalido.");
            }

            try
            {
                if (afr_banco.Escalar("SELECT Id FROM dbo.Cursos WHERE Id = @cursoId AND Ativo = 1",
                                      Parametros("@cursoId", afr_cursoId)) == null)
                {
                    return Mensagem(404, "Curso nao encontrado ou inativo.");
                }

                // Matricular de novo nao e erro: quem ja esta matriculado
                // continua matriculado (UQ_Matriculas_Usuario_Curso)
                afr_banco.Executar(@"
                    IF NOT EXISTS (SELECT 1 FROM dbo.Matriculas WHERE CursoId = @cursoId AND UsuarioId = @usuarioId)
                    INSERT INTO dbo.Matriculas (CursoId, UsuarioId, DataMatricula)
                    VALUES (@cursoId, @usuarioId, GETDATE())",
                    Parametros("@cursoId", afr_cursoId, "@usuarioId", Usuario.afr_Id));

                return Mensagem(201, "Matricula realizada com sucesso.");
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao realizar matricula.", afr_erro);
            }
        }

        [HttpPut("{cursoId}/aulas/{aulaId}/progresso")]
        [Autenticado("ALUNO")]
        public async Task<IActionResult> MarcarProgresso(string cursoId, string aulaId)
        {
            int afr_cursoId = IdValido(cursoId);
            int afr_aulaId = IdValido(aulaId);
            JsonElement afr_corpo = await LerCorpo();
            bool? afr_concluida = Booleano(afr_corpo, "concluida");

            if (afr_cursoId == 0 || afr_aulaId == 0)
            {
                return Mensagem(400, "Identificador de curso ou aula invalido.");
            }

            if (afr_concluida == null)
            {
                return Mensagem(400, "Informe se a aula foi concluida.");
            }

            try
            {
                if (afr_banco.Escalar("SELECT Id FROM dbo.Aulas WHERE Id = @aulaId AND CursoId = @cursoId",
                                      Parametros("@aulaId", afr_aulaId, "@cursoId", afr_cursoId)) == null)
                {
                    return Mensagem(404, "Aula nao encontrada.");
                }

                // A matricula e o porteiro do progresso, igual a API do Gui
                if (afr_banco.Escalar("SELECT Id FROM dbo.Matriculas WHERE UsuarioId = @usuarioId AND CursoId = @cursoId",
                                      Parametros("@usuarioId", Usuario.afr_Id, "@cursoId", afr_cursoId)) == null)
                {
                    return Mensagem(403, "Matricule-se no curso antes de registrar o progresso.");
                }

                Dictionary<string, object> afr_gamificacao = null;

                if (afr_concluida.Value)
                {
                    afr_banco.Executar(@"
                        IF NOT EXISTS (SELECT 1 FROM dbo.Progresso WHERE UsuarioId = @usuarioId AND AulaId = @aulaId)
                        INSERT INTO dbo.Progresso (UsuarioId, AulaId, DataConclusao)
                        VALUES (@usuarioId, @aulaId, GETDATE())",
                        Parametros("@usuarioId", Usuario.afr_Id, "@aulaId", afr_aulaId));

                    afr_gamificacao = RegraXp.ConcederPorAula(afr_banco, Usuario.afr_Id, afr_aulaId);
                }
                else
                {
                    // Desmarcar tira so o progresso. O XP fica: a regra
                    // do Gui paga cada aula uma vez e nao devolve.
                    afr_banco.Executar("DELETE FROM dbo.Progresso WHERE UsuarioId = @usuarioId AND AulaId = @aulaId",
                                       Parametros("@usuarioId", Usuario.afr_Id, "@aulaId", afr_aulaId));
                }

                return Ok(new
                {
                    message = afr_concluida.Value ? "Aula concluida." : "Conclusao removida.",
                    gamificacao = afr_gamificacao
                });
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao atualizar progresso.", afr_erro);
            }
        }

        // -------------------------------------------------------------
        // canAccessCourse da API do Gui (permissions.js):
        //   admin ve todos; professor so os dele; aluno so os ativos
        // -------------------------------------------------------------
        public static bool PodeAcessar(UsuarioLogado afr_usuario, Dictionary<string, object> afr_curso)
        {
            if (afr_usuario.afr_Perfil == "ADMIN")
            {
                return true;
            }

            if (afr_usuario.afr_Perfil == "PROFESSOR")
            {
                return afr_curso["professorId"] != null &&
                       Convert.ToInt32(afr_curso["professorId"]) == afr_usuario.afr_Id;
            }

            if (afr_usuario.afr_Perfil == "ALUNO")
            {
                return Convert.ToBoolean(afr_curso["ativo"]);
            }

            return false;
        }
    }
}
