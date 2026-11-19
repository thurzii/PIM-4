/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - API DO DESKTOP E DA WEB
   Arquivo...: Infra/RegraXp.cs
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   A REGRA DE XP - copia LITERAL da API do Gui
   (api/src/gamificationService.js, funcao awardLessonCompletion).

   POR QUE COPIA LITERAL
   As duas APIs gravam no mesmo banco do Azure. Se cada uma tivesse a
   sua versao da regra, concluir uma aula pelo celular e pelo desktop
   daria XP diferente. O texto SQL abaixo foi extraido do arquivo dele
   por programa, sem redigitar, e esta verificado no teste da API.

   O QUE A REGRA FAZ
   - 10 XP por aula, uma vez so por aula: HistoricoXP guarda o evento e
     o IF NOT EXISTS impede pagar de novo. DESMARCAR A AULA NAO DEVOLVE
     O XP, e concluir de novo nao paga outra vez.
   - Sequencia de dias seguidos (atividade hoje, ontem, ou recomeca).
   - Conquistas do catalogo dbo.Conquistas, cada uma com o seu bonus.
   - Tudo numa transacao: ou grava tudo, ou nada.

   SE A REGRA MUDAR NA API DO GUI, ESTE TEXTO TEM QUE MUDAR JUNTO.
   ===================================================================== */

namespace PlataformaCursosApi.Infra
{
    public static class RegraXp
    {
        public const string SQL = @"
      SET NOCOUNT ON;
      SET XACT_ABORT ON;

      DECLARE @NovasConquistas TABLE (ConquistaId INT);
      DECLARE @XPConquistas INT = 0;
      DECLARE @XPConcedido INT = 0;

      BEGIN TRY
        BEGIN TRANSACTION;

        IF NOT EXISTS (
          SELECT 1 FROM dbo.Gamificacao WITH (UPDLOCK, HOLDLOCK)
          WHERE UsuarioId = @usuarioId
        )
        BEGIN
          INSERT INTO dbo.Gamificacao (UsuarioId, XP)
          VALUES (@usuarioId, 0);
        END;

        IF NOT EXISTS (
          SELECT 1 FROM dbo.HistoricoXP WITH (UPDLOCK, HOLDLOCK)
          WHERE UsuarioId = @usuarioId
            AND ReferenciaTipo = 'AULA'
            AND ReferenciaId = @aulaId
        )
        BEGIN
          INSERT INTO dbo.HistoricoXP
            (UsuarioId, Quantidade, Motivo, ReferenciaTipo, ReferenciaId)
          VALUES
            (@usuarioId, 10, N'Conclusão de aula', 'AULA', @aulaId);

          DECLARE @Hoje DATE = CAST(GETDATE() AS DATE);

          UPDATE dbo.Gamificacao
          SET
            XP = XP + 10,
            SequenciaAtual = CASE
              WHEN UltimaAtividade = @Hoje THEN SequenciaAtual
              WHEN UltimaAtividade = DATEADD(DAY, -1, @Hoje) THEN SequenciaAtual + 1
              ELSE 1
            END,
            MelhorSequencia = CASE
              WHEN UltimaAtividade = @Hoje THEN MelhorSequencia
              WHEN UltimaAtividade = DATEADD(DAY, -1, @Hoje)
                THEN CASE WHEN SequenciaAtual + 1 > MelhorSequencia
                          THEN SequenciaAtual + 1 ELSE MelhorSequencia END
              WHEN MelhorSequencia < 1 THEN 1
              ELSE MelhorSequencia
            END,
            UltimaAtividade = @Hoje
          WHERE UsuarioId = @usuarioId;

          SET @XPConcedido = 10;
        END;

        DECLARE @AulasConcluidas INT = (
          SELECT COUNT(*)
          FROM dbo.HistoricoXP
          WHERE UsuarioId = @usuarioId AND ReferenciaTipo = 'AULA'
        );

        DECLARE @CursosConcluidos INT = (
          SELECT COUNT(*)
          FROM dbo.Matriculas m
          WHERE m.UsuarioId = @usuarioId
            AND EXISTS (SELECT 1 FROM dbo.Aulas a WHERE a.CursoId = m.CursoId)
            AND NOT EXISTS (
              SELECT 1
              FROM dbo.Aulas a
              WHERE a.CursoId = m.CursoId
                AND NOT EXISTS (
                  SELECT 1 FROM dbo.Progresso p
                  WHERE p.UsuarioId = @usuarioId AND p.AulaId = a.Id
                )
            )
        );

        INSERT INTO dbo.UsuarioConquistas (UsuarioId, ConquistaId)
        OUTPUT INSERTED.ConquistaId INTO @NovasConquistas (ConquistaId)
        SELECT @usuarioId, c.Id
        FROM dbo.Conquistas c
        WHERE NOT EXISTS (
          SELECT 1 FROM dbo.UsuarioConquistas uc
          WHERE uc.UsuarioId = @usuarioId AND uc.ConquistaId = c.Id
        )
        AND (
          (c.Codigo = 'PRIMEIRA_AULA' AND @AulasConcluidas >= 1)
          OR (c.Codigo = 'CINCO_AULAS' AND @AulasConcluidas >= 5)
          OR (c.Codigo = 'DEZ_AULAS' AND @AulasConcluidas >= 10)
          OR (c.Codigo = 'PRIMEIRO_CURSO' AND @CursosConcluidos >= 1)
          OR (c.Codigo = 'SEQUENCIA_7' AND EXISTS (
            SELECT 1 FROM dbo.Gamificacao
            WHERE UsuarioId = @usuarioId AND SequenciaAtual >= 7
          ))
        );

        SELECT @XPConquistas = ISNULL(SUM(c.XPBonus), 0)
        FROM dbo.Conquistas c
        INNER JOIN @NovasConquistas n ON n.ConquistaId = c.Id;

        IF @XPConquistas > 0
        BEGIN
          UPDATE dbo.Gamificacao
          SET XP = XP + @XPConquistas
          WHERE UsuarioId = @usuarioId;

          INSERT INTO dbo.HistoricoXP
            (UsuarioId, Quantidade, Motivo, ReferenciaTipo, ReferenciaId)
          SELECT @usuarioId, c.XPBonus, CONCAT(N'Conquista: ', c.Nome), 'CONQUISTA', c.Id
          FROM dbo.Conquistas c
          INNER JOIN @NovasConquistas n ON n.ConquistaId = c.Id;
        END;

        COMMIT TRANSACTION;

        SELECT
          @XPConcedido AS xpAula,
          @XPConquistas AS xpConquistas,
          g.XP AS xpTotal,
          g.SequenciaAtual AS sequenciaAtual,
          g.MelhorSequencia AS melhorSequencia
        FROM dbo.Gamificacao g
        WHERE g.UsuarioId = @usuarioId;

        SELECT c.Id AS id, c.Nome AS nome, c.Descricao AS descricao,
               c.Icone AS icone, c.XPBonus AS xpBonus
        FROM dbo.Conquistas c
        INNER JOIN @NovasConquistas n ON n.ConquistaId = c.Id;
      END TRY
      BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
      END CATCH;
    ";

        // -------------------------------------------------------------
        // Executa a regra e monta a mesma resposta da API do Gui:
        // { xpAula, xpConquistas, xpTotal, sequenciaAtual,
        //   melhorSequencia, xpGanho, novasConquistas, nivel }
        // -------------------------------------------------------------
        public static Dictionary<string, object> ConcederPorAula(Banco afr_banco, int afr_usuarioId, int afr_aulaId)
        {
            List<List<Dictionary<string, object>>> afr_resultados = afr_banco.ConsultarVarios(SQL,
                new Dictionary<string, object> { { "@usuarioId", afr_usuarioId }, { "@aulaId", afr_aulaId } });

            Dictionary<string, object> afr_resumo =
                afr_resultados.Count > 0 && afr_resultados[0].Count > 0
                    ? afr_resultados[0][0]
                    : new Dictionary<string, object>
                      {
                          { "xpAula", 0 }, { "xpConquistas", 0 }, { "xpTotal", 0 },
                          { "sequenciaAtual", 0 }, { "melhorSequencia", 0 }
                      };

            int afr_xpAula = Convert.ToInt32(afr_resumo["xpAula"] ?? 0);
            int afr_xpConquistas = Convert.ToInt32(afr_resumo["xpConquistas"] ?? 0);
            int afr_xpTotal = Convert.ToInt32(afr_resumo["xpTotal"] ?? 0);

            Dictionary<string, object> afr_resposta = new Dictionary<string, object>(afr_resumo);
            afr_resposta["xpGanho"] = afr_xpAula + afr_xpConquistas;
            afr_resposta["novasConquistas"] = afr_resultados.Count > 1
                ? afr_resultados[1]
                : new List<Dictionary<string, object>>();
            afr_resposta["nivel"] = afr_xpTotal / 100 + 1;

            return afr_resposta;
        }
    }
}
