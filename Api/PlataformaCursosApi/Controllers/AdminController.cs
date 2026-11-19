/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Controllers/AdminController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   Rotas do ADMINISTRADOR. Porte das rotas /api/admin do server.js da
   API do Gui:

   GET    /api/admin/cursos            POST /api/admin/cursos
   PUT    /api/admin/cursos/:id        DELETE /api/admin/cursos/:id
   POST   /api/admin/cursos/:id/aulas  DELETE /api/admin/aulas/:id
   GET    /api/admin/usuarios          POST /api/admin/usuarios
   PUT    /api/admin/usuarios/:id/status

   A MAIS (as telas do C# precisam; so tabelas que ja existem):
   GET    /api/admin/usuarios/:id      um usuario, com a data de cadastro
   PUT    /api/admin/usuarios/:id      editar nome, email, perfil, senha
   DELETE /api/admin/usuarios/:id      excluir de vez
   PUT    /api/admin/aulas/:id         editar titulo e duracao

   A lista de usuarios tambem devolve "dataCadastro" (campo a mais: quem
   nao usa, ignora).
   ===================================================================== */

using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using PlataformaCursosApi.Infra;

namespace PlataformaCursosApi.Controllers
{
    [Route("api/admin")]
    [Autenticado("ADMIN")]
    public class AdminController : ControladorBase
    {
        public AdminController(Banco afr_b) : base(afr_b) { }

        // =============================================================
        // CURSOS
        // =============================================================

        [HttpGet("cursos")]
        public IActionResult ListarCursos([FromQuery] string busca)
        {
            try
            {
                return Ok(afr_banco.Consultar(@"
                    SELECT c.Id AS id, c.Titulo AS titulo, c.Descricao AS descricao,
                           c.CargaHoraria AS cargaHoraria, c.Ativo AS ativo,
                           (SELECT COUNT(*) FROM dbo.Aulas a WHERE a.CursoId = c.Id) AS totalAulas
                    FROM dbo.Cursos c
                    WHERE @termo = '' OR c.Titulo LIKE '%' + @termo + '%' OR c.Descricao LIKE '%' + @termo + '%'
                    ORDER BY c.Titulo", Parametros("@termo", (busca ?? "").Trim())));
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao listar cursos.", afr_erro);
            }
        }

        // A API do Gui so exige o titulo aqui; carga sem numero vira 0
        // e "ativo" ausente vira true
        [HttpPost("cursos")]
        public async Task<IActionResult> CriarCurso()
        {
            JsonElement afr_corpo = await LerCorpo();
            string afr_titulo = Texto(afr_corpo, "titulo");

            if (string.IsNullOrEmpty(afr_titulo))
            {
                return Mensagem(400, "Titulo obrigatorio.");
            }

            try
            {
                object afr_id = afr_banco.Escalar(@"
                    INSERT INTO dbo.Cursos (Titulo, Descricao, CargaHoraria, Ativo, DataCadastro)
                    OUTPUT INSERTED.Id
                    VALUES (@titulo, @descricao, @carga, @ativo, GETDATE())",
                    Parametros("@titulo", afr_titulo, "@descricao", Texto(afr_corpo, "descricao") ?? "",
                               "@carga", Inteiro(afr_corpo, "cargaHoraria") ?? 0,
                               "@ativo", Booleano(afr_corpo, "ativo") ?? true));

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
            JsonElement afr_corpo = await LerCorpo();

            try
            {
                int afr_linhas = afr_banco.Executar(@"
                    UPDATE dbo.Cursos
                    SET Titulo = @titulo, Descricao = @descricao, CargaHoraria = @carga, Ativo = @ativo
                    WHERE Id = @id",
                    Parametros("@id", IdValido(id), "@titulo", Texto(afr_corpo, "titulo") ?? "",
                               "@descricao", Texto(afr_corpo, "descricao") ?? "",
                               "@carga", Inteiro(afr_corpo, "cargaHoraria") ?? 0,
                               "@ativo", Booleano(afr_corpo, "ativo") ?? true));

                if (afr_linhas == 0)
                {
                    return Mensagem(404, "Curso nao encontrado.");
                }

                return Mensagem(200, "Curso atualizado.");
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao atualizar curso.", afr_erro);
            }
        }

        // Progresso, aulas e curso numa transacao. As matriculas saem
        // sozinhas (FK com ON DELETE CASCADE no esquema do Gui).
        [HttpDelete("cursos/{id}")]
        public IActionResult ExcluirCurso(string id)
        {
            int afr_id = IdValido(id);

            try
            {
                int afr_linhas;

                using (SqlConnection afr_cn = afr_banco.Abrir())
                using (SqlTransaction afr_tx = afr_cn.BeginTransaction())
                {
                    Dictionary<string, object> afr_p = Parametros("@id", afr_id);

                    afr_banco.Executar(@"DELETE p FROM dbo.Progresso p
                                         INNER JOIN dbo.Aulas a ON a.Id = p.AulaId
                                         WHERE a.CursoId = @id", afr_p, afr_cn, afr_tx);
                    afr_banco.Executar("DELETE FROM dbo.Aulas WHERE CursoId = @id", afr_p, afr_cn, afr_tx);
                    afr_linhas = afr_banco.Executar("DELETE FROM dbo.Cursos WHERE Id = @id", afr_p, afr_cn, afr_tx);
                    afr_tx.Commit();
                }

                if (afr_linhas == 0)
                {
                    return Mensagem(404, "Curso nao encontrado.");
                }

                return Mensagem(200, "Curso excluido.");
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao excluir curso.", afr_erro);
            }
        }

        // =============================================================
        // AULAS
        // =============================================================

        [HttpPost("cursos/{cursoId}/aulas")]
        public async Task<IActionResult> CriarAula(string cursoId)
        {
            JsonElement afr_corpo = await LerCorpo();
            string afr_titulo = Texto(afr_corpo, "titulo");

            if (string.IsNullOrEmpty(afr_titulo))
            {
                return Mensagem(400, "Titulo obrigatorio.");
            }

            try
            {
                object afr_id = afr_banco.Escalar(@"
                    DECLARE @ordem INT = (SELECT ISNULL(MAX(Ordem), 0) + 1 FROM dbo.Aulas WHERE CursoId = @cursoId);
                    INSERT INTO dbo.Aulas (CursoId, Titulo, Ordem, DuracaoMin)
                    OUTPUT INSERTED.Id
                    VALUES (@cursoId, @titulo, @ordem, @duracao)",
                    Parametros("@cursoId", IdValido(cursoId), "@titulo", afr_titulo,
                               "@duracao", Inteiro(afr_corpo, "duracaoMin") ?? 0));

                return StatusCode(201, new { id = Convert.ToInt32(afr_id), message = "Aula criada." });
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao criar aula.", afr_erro);
            }
        }

        // A MAIS
        [HttpPut("aulas/{id}")]
        public async Task<IActionResult> AlterarAula(string id)
        {
            int afr_aulaId = IdValido(id);
            string afr_erroValidacao;
            ProfessorController.ValidarAula(await LerCorpo(), out string afr_titulo, out int afr_duracao,
                                            out afr_erroValidacao);

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
                int afr_linhas = afr_banco.Executar(
                    "UPDATE dbo.Aulas SET Titulo = @titulo, DuracaoMin = @duracaoMin WHERE Id = @aulaId",
                    Parametros("@aulaId", afr_aulaId, "@titulo", afr_titulo, "@duracaoMin", afr_duracao));

                if (afr_linhas == 0)
                {
                    return Mensagem(404, "Aula nao encontrada.");
                }

                return Mensagem(200, "Aula atualizada.");
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao atualizar aula.", afr_erro);
            }
        }

        // -------------------------------------------------------------
        // DIFERENCA PROPOSITAL: o progresso da aula sai antes.
        //
        // Na API do Gui, esta rota apaga so a aula. Se algum aluno ja
        // concluiu a aula, a FK de Progresso recusa e a rota da erro
        // 500. A rota do professor dele ja apaga o progresso antes - aqui
        // o admin faz o mesmo.
        // -------------------------------------------------------------
        [HttpDelete("aulas/{id}")]
        public IActionResult ExcluirAula(string id)
        {
            int afr_aulaId = IdValido(id);

            try
            {
                int afr_linhas;

                using (SqlConnection afr_cn = afr_banco.Abrir())
                using (SqlTransaction afr_tx = afr_cn.BeginTransaction())
                {
                    Dictionary<string, object> afr_p = Parametros("@id", afr_aulaId);

                    afr_banco.Executar("DELETE FROM dbo.Progresso WHERE AulaId = @id", afr_p, afr_cn, afr_tx);
                    afr_linhas = afr_banco.Executar("DELETE FROM dbo.Aulas WHERE Id = @id", afr_p, afr_cn, afr_tx);
                    afr_tx.Commit();
                }

                if (afr_linhas == 0)
                {
                    return Mensagem(404, "Aula nao encontrada.");
                }

                return Mensagem(200, "Aula excluida.");
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao excluir aula.", afr_erro);
            }
        }

        // =============================================================
        // USUARIOS
        // =============================================================

        private const string afr_SELECT_USUARIO =
            "SELECT Id AS id, Nome AS nome, Email AS email, Perfil AS perfil, Ativo AS ativo, " +
            "DataCadastro AS dataCadastro FROM dbo.Usuarios";

        [HttpGet("usuarios")]
        public IActionResult ListarUsuarios([FromQuery] string busca)
        {
            try
            {
                return Ok(afr_banco.Consultar(afr_SELECT_USUARIO +
                    " WHERE @termo = '' OR Nome LIKE '%' + @termo + '%' OR Email LIKE '%' + @termo + '%' ORDER BY Nome",
                    Parametros("@termo", (busca ?? "").Trim())));
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao listar usuarios.", afr_erro);
            }
        }

        // A MAIS
        [HttpGet("usuarios/{id}")]
        public IActionResult ObterUsuario(string id)
        {
            try
            {
                List<Dictionary<string, object>> afr_linhas = afr_banco.Consultar(afr_SELECT_USUARIO +
                    " WHERE Id = @id", Parametros("@id", IdValido(id)));

                if (afr_linhas.Count == 0)
                {
                    return Mensagem(404, "Usuario nao encontrado.");
                }

                return Ok(afr_linhas[0]);
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao carregar usuario.", afr_erro);
            }
        }

        // createAccount da API do Gui: mesma validacao, mesmo hash, 409
        // para email repetido
        [HttpPost("usuarios")]
        public async Task<IActionResult> CriarUsuario()
        {
            JsonElement afr_corpo = await LerCorpo();
            string afr_erro = ValidarConta(afr_corpo, true, out string afr_nome, out string afr_email,
                                           out string afr_senha, out string afr_perfil);

            if (afr_erro != null)
            {
                return Mensagem(400, afr_erro);
            }

            try
            {
                object afr_id = afr_banco.Escalar(@"
                    INSERT INTO dbo.Usuarios (Nome, Email, Senha, Perfil, Ativo, DataCadastro)
                    OUTPUT INSERTED.Id VALUES (@nome, @email, @senha, @perfil, 1, GETDATE())",
                    Parametros("@nome", afr_nome, "@email", afr_email,
                               "@senha", SenhaHash.Gerar(afr_senha), "@perfil", afr_perfil));

                return StatusCode(201, new { id = Convert.ToInt32(afr_id), message = "Usuario criado com sucesso." });
            }
            catch (SqlException afr_sql) when (afr_sql.Number == 2627 || afr_sql.Number == 2601)
            {
                return Mensagem(409, "Este email ja esta cadastrado.");
            }
            catch (Exception)
            {
                return Mensagem(500, "Erro ao criar usuario.");
            }
        }

        [HttpPut("usuarios/{id}/status")]
        public async Task<IActionResult> AlterarStatus(string id)
        {
            JsonElement afr_corpo = await LerCorpo();
            int afr_id = IdValido(id);
            bool afr_ativo = Booleano(afr_corpo, "ativo") ?? false;

            // A MAIS: o admin nao se desativa nem desativa o ultimo admin.
            // Sem isso o sistema podia ficar sem ninguem para administrar.
            string afr_bloqueio = BloqueioDoUltimoAdmin(afr_id, afr_ativo ? "ADMIN" : null, afr_ativo);

            if (afr_bloqueio != null)
            {
                return Mensagem(400, afr_bloqueio);
            }

            try
            {
                int afr_linhas = afr_banco.Executar("UPDATE dbo.Usuarios SET Ativo = @ativo WHERE Id = @id",
                                                    Parametros("@id", afr_id, "@ativo", afr_ativo));

                if (afr_linhas == 0)
                {
                    return Mensagem(404, "Usuario nao encontrado.");
                }

                return Mensagem(200, "Status atualizado.");
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao atualizar usuario.", afr_erro);
            }
        }

        // -------------------------------------------------------------
        // A MAIS: editar o usuario inteiro.
        //
        // Mesma validacao da criacao, com uma diferenca: a senha e
        // opcional. Vazia = manter a atual (o banco guarda o hash, nao
        // ha como mostrar a senha para a pessoa redigitar).
        // -------------------------------------------------------------
        [HttpPut("usuarios/{id}")]
        public async Task<IActionResult> AlterarUsuario(string id)
        {
            int afr_id = IdValido(id);
            JsonElement afr_corpo = await LerCorpo();
            string afr_erro = ValidarConta(afr_corpo, false, out string afr_nome, out string afr_email,
                                           out string afr_senha, out string afr_perfil);

            if (afr_id == 0)
            {
                return Mensagem(400, "Identificador de usuario invalido.");
            }

            if (afr_erro != null)
            {
                return Mensagem(400, afr_erro);
            }

            bool afr_ativo = Booleano(afr_corpo, "ativo") ?? true;
            string afr_bloqueio = BloqueioDoUltimoAdmin(afr_id, afr_perfil, afr_ativo);

            if (afr_bloqueio != null)
            {
                return Mensagem(400, afr_bloqueio);
            }

            try
            {
                string afr_sql = "UPDATE dbo.Usuarios SET Nome = @nome, Email = @email, Perfil = @perfil, " +
                                 "Ativo = @ativo" + (afr_senha != "" ? ", Senha = @senha" : "") +
                                 " WHERE Id = @id";

                Dictionary<string, object> afr_p = Parametros("@id", afr_id, "@nome", afr_nome,
                    "@email", afr_email, "@perfil", afr_perfil, "@ativo", afr_ativo);

                if (afr_senha != "")
                {
                    afr_p["@senha"] = SenhaHash.Gerar(afr_senha);
                }

                if (afr_banco.Executar(afr_sql, afr_p) == 0)
                {
                    return Mensagem(404, "Usuario nao encontrado.");
                }

                return Mensagem(200, "Usuario atualizado.");
            }
            catch (SqlException afr_sql) when (afr_sql.Number == 2627 || afr_sql.Number == 2601)
            {
                return Mensagem(409, "Este email ja esta cadastrado.");
            }
            catch (Exception afr_erroBanco)
            {
                return Falha("Erro ao atualizar usuario.", afr_erroBanco);
            }
        }

        // -------------------------------------------------------------
        // A MAIS: excluir de vez.
        //
        // No esquema do banco, Progresso e Cursos.ProfessorId apontam
        // para o usuario SEM apagar em cascata: excluir direto daria
        // erro. Por isso, numa transacao:
        //   - o progresso, o historico de XP e as medalhas saem
        //   - os cursos de que era professor ficam sem professor (nao
        //     se apaga o curso de ninguem por tabela)
        //   - matriculas e gamificacao saem pela cascata do esquema,
        //     mas sao apagadas aqui tambem para nao depender disso
        // -------------------------------------------------------------
        [HttpDelete("usuarios/{id}")]
        public IActionResult ExcluirUsuario(string id)
        {
            int afr_id = IdValido(id);

            if (afr_id == Usuario.afr_Id)
            {
                return Mensagem(400, "Voce nao pode excluir a sua propria conta.");
            }

            string afr_bloqueio = BloqueioDoUltimoAdmin(afr_id, null, false);

            if (afr_bloqueio != null)
            {
                return Mensagem(400, afr_bloqueio);
            }

            try
            {
                int afr_linhas;

                using (SqlConnection afr_cn = afr_banco.Abrir())
                using (SqlTransaction afr_tx = afr_cn.BeginTransaction())
                {
                    Dictionary<string, object> afr_p = Parametros("@id", afr_id);

                    afr_banco.Executar("UPDATE dbo.Cursos SET ProfessorId = NULL WHERE ProfessorId = @id", afr_p, afr_cn, afr_tx);
                    afr_banco.Executar("DELETE FROM dbo.Progresso WHERE UsuarioId = @id", afr_p, afr_cn, afr_tx);
                    afr_banco.Executar("DELETE FROM dbo.HistoricoXP WHERE UsuarioId = @id", afr_p, afr_cn, afr_tx);
                    afr_banco.Executar("DELETE FROM dbo.UsuarioConquistas WHERE UsuarioId = @id", afr_p, afr_cn, afr_tx);
                    afr_banco.Executar("DELETE FROM dbo.Gamificacao WHERE UsuarioId = @id", afr_p, afr_cn, afr_tx);
                    afr_banco.Executar("DELETE FROM dbo.Matriculas WHERE UsuarioId = @id", afr_p, afr_cn, afr_tx);
                    afr_linhas = afr_banco.Executar("DELETE FROM dbo.Usuarios WHERE Id = @id", afr_p, afr_cn, afr_tx);
                    afr_tx.Commit();
                }

                if (afr_linhas == 0)
                {
                    return Mensagem(404, "Usuario nao encontrado.");
                }

                return Mensagem(200, "Usuario excluido.");
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao excluir usuario.", afr_erro);
            }
        }

        // -------------------------------------------------------------
        // O sistema nunca fica sem administrador ativo. Devolve a
        // mensagem de bloqueio, ou null se pode seguir.
        //   afr_perfilNovo: o perfil que o usuario vai ter (null = sai)
        // -------------------------------------------------------------
        private string BloqueioDoUltimoAdmin(int afr_id, string afr_perfilNovo, bool afr_ativoNovo)
        {
            bool afr_continuaAdmin = afr_perfilNovo == "ADMIN" && afr_ativoNovo;

            if (afr_continuaAdmin)
            {
                return null;
            }

            object afr_perfilAtual = afr_banco.Escalar(
                "SELECT Perfil FROM dbo.Usuarios WHERE Id = @id AND Ativo = 1", Parametros("@id", afr_id));

            if (!"ADMIN".Equals(Convert.ToString(afr_perfilAtual), StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (afr_id == Usuario.afr_Id)
            {
                return "Voce nao pode tirar o seu proprio acesso de administrador.";
            }

            int afr_admins = Convert.ToInt32(afr_banco.Escalar(
                "SELECT COUNT(*) FROM dbo.Usuarios WHERE Perfil = 'ADMIN' AND Ativo = 1"));

            return afr_admins <= 1
                ? "Este e o unico administrador ativo do sistema. Promova outro usuario antes."
                : null;
        }

        // -------------------------------------------------------------
        // validateAccount da API do Gui, com as mesmas mensagens.
        // Na edicao (afr_senhaObrigatoria = false) a senha pode vir vazia.
        // -------------------------------------------------------------
        private static string ValidarConta(JsonElement afr_corpo, bool afr_senhaObrigatoria,
                                           out string afr_nome, out string afr_email,
                                           out string afr_senha, out string afr_perfil)
        {
            afr_nome = (Texto(afr_corpo, "nome") ?? "").Trim();
            afr_email = (Texto(afr_corpo, "email") ?? "").Trim().ToLowerInvariant();
            afr_senha = Texto(afr_corpo, "senha") ?? "";
            afr_perfil = (Texto(afr_corpo, "perfil") ?? "").ToUpperInvariant();

            if (afr_nome.Length < 3 || afr_nome.Length > 200)
            {
                return "Informe um nome entre 3 e 200 caracteres.";
            }

            if (afr_email.Length > 200 || !Regex.IsMatch(afr_email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$"))
            {
                return "Informe um email valido.";
            }

            bool afr_temSenha = afr_senha != "";

            if ((afr_senhaObrigatoria || afr_temSenha) && (afr_senha.Length < 8 || afr_senha.Length > 128))
            {
                return "A senha deve ter entre 8 e 128 caracteres.";
            }

            if (afr_perfil != "ALUNO" && afr_perfil != "PROFESSOR" && afr_perfil != "ADMIN")
            {
                return "Perfil invalido.";
            }

            return null;
        }
    }
}
