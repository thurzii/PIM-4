/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Controllers/GamificacaoController.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   Porte do gamificationRoutes.js da API do Gui:
   GET /api/gamificacao/me      XP, nivel, sequencia e medalhas do aluno
   GET /api/gamificacao/turma   ranking dos alunos (professor e admin)

   A MAIS:
   GET /api/gamificacao/alunos/:id   o mesmo que /me, de UM aluno, para
                                     a tela de progresso do professor
   O ranking tambem devolve aulasConcluidas e conquistas (quantidade),
   que a tela de classificacao mostra. Campos a mais: quem nao usa,
   ignora.

   O nivel e calculado do mesmo jeito da API do Gui: XP / 100 + 1.
   ===================================================================== */

using Microsoft.AspNetCore.Mvc;
using PlataformaCursosApi.Infra;

namespace PlataformaCursosApi.Controllers
{
    [Route("api/gamificacao")]
    public class GamificacaoController : ControladorBase
    {
        public GamificacaoController(Banco afr_b) : base(afr_b) { }

        [HttpGet("turma")]
        [Autenticado("ADMIN", "PROFESSOR", Mensagem = "Acesso permitido apenas a professores e administradores.")]
        public IActionResult Turma()
        {
            try
            {
                List<Dictionary<string, object>> afr_linhas = afr_banco.Consultar(@"
                    SELECT u.Id AS id, u.Nome AS nome, u.Email AS email,
                           ISNULL(g.XP, 0) AS xp,
                           CAST(FLOOR(ISNULL(g.XP, 0) / 100.0) AS INT) + 1 AS nivel,
                           ISNULL(g.SequenciaAtual, 0) AS sequenciaAtual,
                           ISNULL(g.MelhorSequencia, 0) AS melhorSequencia,
                           (SELECT COUNT(*) FROM dbo.HistoricoXP h
                            WHERE h.UsuarioId = u.Id AND h.ReferenciaTipo = 'AULA') AS aulasConcluidas,
                           (SELECT COUNT(*) FROM dbo.UsuarioConquistas uc
                            WHERE uc.UsuarioId = u.Id) AS conquistas
                    FROM dbo.Usuarios u
                    LEFT JOIN dbo.Gamificacao g ON g.UsuarioId = u.Id
                    WHERE u.Perfil = 'ALUNO' AND u.Ativo = 1
                    ORDER BY ISNULL(g.XP, 0) DESC, u.Nome ASC, u.Id ASC");

                // A posicao vem pronta, como na API do Gui: as telas nao
                // reordenam, entao o ranking e o mesmo em todo lugar
                List<Dictionary<string, object>> afr_resposta = new List<Dictionary<string, object>>();

                for (int i = 0; i < afr_linhas.Count; i++)
                {
                    Dictionary<string, object> afr_item = new Dictionary<string, object> { { "posicao", i + 1 } };

                    foreach (KeyValuePair<string, object> afr_campo in afr_linhas[i])
                    {
                        afr_item[afr_campo.Key] = afr_campo.Value;
                    }

                    afr_resposta.Add(afr_item);
                }

                return Ok(afr_resposta);
            }
            catch (Exception)
            {
                return Mensagem(500, "Erro ao consultar ranking de XP.");
            }
        }

        // O roteador de gamificacao do Gui tem a sua propria mensagem
        // para "so aluno" (diferente da do courseRoutes) - copiada igual
        [HttpGet("me")]
        [Autenticado("ALUNO", Mensagem = "Acesso permitido apenas para alunos.")]
        public IActionResult Minha()
        {
            try
            {
                return Ok(Pontuacao(Usuario.afr_Id));
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao carregar gamificação.", afr_erro);
            }
        }

        // A MAIS
        [HttpGet("alunos/{id}")]
        [Autenticado("ADMIN", "PROFESSOR", Mensagem = "Acesso permitido apenas a professores e administradores.")]
        public IActionResult DoAluno(string id)
        {
            int afr_id = IdValido(id);

            if (afr_id == 0)
            {
                return Mensagem(400, "Identificador de aluno invalido.");
            }

            try
            {
                return Ok(Pontuacao(afr_id));
            }
            catch (Exception afr_erro)
            {
                return Falha("Erro ao carregar gamificação.", afr_erro);
            }
        }

        // As duas consultas do /me da API do Gui, e o mesmo calculo dos
        // campos derivados (xpProximoNivel, xpNoNivel, progressoNivel)
        private Dictionary<string, object> Pontuacao(int afr_usuarioId)
        {
            Dictionary<string, object> afr_p = Parametros("@usuarioId", afr_usuarioId);

            List<Dictionary<string, object>> afr_numeros = afr_banco.Consultar(@"
                SELECT
                  ISNULL(g.XP, 0) AS xp,
                  CAST(FLOOR(ISNULL(g.XP, 0) / 100.0) AS INT) + 1 AS nivel,
                  ISNULL(g.SequenciaAtual, 0) AS sequenciaAtual,
                  ISNULL(g.MelhorSequencia, 0) AS melhorSequencia,
                  (SELECT COUNT(*) FROM dbo.HistoricoXP h
                   WHERE h.UsuarioId = @usuarioId AND h.ReferenciaTipo = 'AULA') AS aulasConcluidas
                FROM (SELECT @usuarioId AS UsuarioId) u
                LEFT JOIN dbo.Gamificacao g ON g.UsuarioId = u.UsuarioId", afr_p);

            List<Dictionary<string, object>> afr_conquistas = afr_banco.Consultar(@"
                SELECT
                  c.Id AS id, c.Codigo AS codigo, c.Nome AS nome, c.Descricao AS descricao,
                  c.Icone AS icone, c.XPBonus AS xpBonus,
                  CASE WHEN uc.Id IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS conquistada,
                  uc.DataConquista AS dataConquista
                FROM dbo.Conquistas c
                LEFT JOIN dbo.UsuarioConquistas uc ON uc.ConquistaId = c.Id AND uc.UsuarioId = @usuarioId
                ORDER BY conquistada DESC, c.Id", afr_p);

            Dictionary<string, object> afr_resposta = afr_numeros.Count > 0
                ? new Dictionary<string, object>(afr_numeros[0])
                : new Dictionary<string, object>
                  {
                      { "xp", 0 }, { "nivel", 1 }, { "sequenciaAtual", 0 },
                      { "melhorSequencia", 0 }, { "aulasConcluidas", 0 }
                  };

            int afr_xp = Convert.ToInt32(afr_resposta["xp"]);
            int afr_nivel = Convert.ToInt32(afr_resposta["nivel"]);
            int afr_base = (afr_nivel - 1) * 100;

            afr_resposta["xpProximoNivel"] = afr_nivel * 100;
            afr_resposta["xpNoNivel"] = afr_xp - afr_base;
            afr_resposta["progressoNivel"] = afr_xp - afr_base;
            afr_resposta["conquistas"] = afr_conquistas;

            return afr_resposta;
        }
    }
}
