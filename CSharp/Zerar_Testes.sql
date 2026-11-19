/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: Zerar_Testes.sql
   Professor.: Antonio Graça
   ---------------------------------------------------------------------
   PARA QUE SERVE
   Apaga o que foi gerado TESTANDO - aulas concluidas, XP, conquistas e
   matriculas - e deixa o banco como estava antes da primeira conclusao.
   Serve para ensaiar a apresentacao quantas vezes quiser.

   O QUE ESTE SCRIPT NAO APAGA
     - usuarios, cursos e aulas    (o conteudo do sistema)
     - o catalogo dbo.Conquistas   (as 5 medalhas que EXISTEM)

   A diferenca entre as duas tabelas de conquista confunde no comeco:

     dbo.Conquistas .......... o CATALOGO: quais medalhas existem.
                               5 linhas fixas. NAO apagar - sem elas,
                               nenhuma conquista pode ser concedida.

     dbo.UsuarioConquistas ... quem GANHOU o que. E esta que se zera.

   COMO USAR
     SSMS -> conectar em (localdb)\MSSQLLocalDB -> Nova Consulta ->
     colar este arquivo -> escolher UM dos dois blocos abaixo -> F5.

   Depois de rodar, FECHE E ABRA o programa desktop (ou atualize a
   pagina do site). As telas guardam o painel de XP em memoria e so
   releem o banco ao abrir.
   ===================================================================== */

USE CursosDB;
GO

-- =====================================================================
-- BLOCO 1 - ZERAR UM ALUNO SO
-- =====================================================================
-- O mais util no dia a dia: ensaiar com a Maria sem mexer no resto.
-- Troque o email na primeira linha para escolher quem zerar.
-- =====================================================================

DECLARE @Email VARCHAR(100) = 'maria@escola.com';

DECLARE @UsuarioId INT = (
    SELECT Id FROM dbo.Usuarios WHERE Email = @Email
);

IF @UsuarioId IS NULL
BEGIN
    PRINT 'Nao achei usuario com esse email. Nada foi apagado.';
END
ELSE
BEGIN
    -- As aulas que ele marcou como concluidas
    DELETE FROM dbo.Progresso WHERE UsuarioId = @UsuarioId;

    -- O extrato de XP (cada ganho, um por linha)
    DELETE FROM dbo.HistoricoXP WHERE UsuarioId = @UsuarioId;

    -- As medalhas que ele ganhou (o catalogo continua intacto)
    DELETE FROM dbo.UsuarioConquistas WHERE UsuarioId = @UsuarioId;

    -- Os cursos em que ele se matriculou.
    -- ATENCAO: desde a versao 3.3 a matricula e explicita (o aluno
    -- aperta o botao, como no aplicativo). Depois de rodar isto, ele
    -- precisa se matricular de novo antes de concluir qualquer aula -
    -- nao e defeito, e o fluxo igual ao do primeiro acesso.
    DELETE FROM dbo.Matriculas WHERE UsuarioId = @UsuarioId;

    -- O XP total e a sequencia de dias.
    -- Apaga a linha inteira em vez de zerar os campos: o programa
    -- recria com XP = 0 na proxima aula concluida.
    DELETE FROM dbo.Gamificacao WHERE UsuarioId = @UsuarioId;

    PRINT 'Zerado: ' + @Email;
END
GO


-- =====================================================================
-- BLOCO 2 - ZERAR TODO MUNDO
-- =====================================================================
-- Para comecar do zero antes da apresentacao.
-- Remova os dois tracos do inicio das linhas para usar.
-- =====================================================================

-- DELETE FROM dbo.Progresso;
-- DELETE FROM dbo.HistoricoXP;
-- DELETE FROM dbo.UsuarioConquistas;
-- DELETE FROM dbo.Matriculas;
-- DELETE FROM dbo.Gamificacao;
-- PRINT 'Progresso e gamificacao zerados para todos os usuarios.';
-- GO


-- =====================================================================
-- OPCIONAL - VOLTAR AS SENHAS PARA TEXTO PURO
-- =====================================================================
-- So se quiser DEMONSTRAR a conversao de senha acontecendo: depois
-- disto, o primeiro login certo de cada um grava o hash de novo.
--
-- Serve tambem para destravar uma conta cuja senha voce esqueceu.
--
-- NAO use em banco de verdade - isto grava senha legivel na tabela.
-- Remova os dois tracos para usar.
-- =====================================================================

-- UPDATE dbo.Usuarios SET Senha = 'admin123'     WHERE Email = 'admin@escola.com';
-- UPDATE dbo.Usuarios SET Senha = 'tutor123'     WHERE Email = 'carla@escola.com';
-- UPDATE dbo.Usuarios SET Senha = 'aluno123'     WHERE Email = 'maria@escola.com';
-- UPDATE dbo.Usuarios SET Senha = 'aluno123'     WHERE Email = 'joao@escola.com';
-- GO


-- =====================================================================
-- CONFERENCIA - rode depois para ver como ficou
-- =====================================================================

SELECT
    u.Nome,
    u.Email,
    u.Perfil,
    ISNULL(g.XP, 0)                                              AS XP,
    ISNULL(g.SequenciaAtual, 0)                                  AS Sequencia,
    (SELECT COUNT(*) FROM dbo.Progresso p
      WHERE p.UsuarioId = u.Id)                                  AS AulasConcluidas,
    (SELECT COUNT(*) FROM dbo.UsuarioConquistas uc
      WHERE uc.UsuarioId = u.Id)                                 AS Conquistas,
    (SELECT COUNT(*) FROM dbo.Matriculas m
      WHERE m.UsuarioId = u.Id)                                  AS Matriculas,
    CASE WHEN u.Senha LIKE 'scrypt:%' THEN 'hash' ELSE 'texto puro' END AS FormatoSenha
FROM dbo.Usuarios u
ORDER BY u.Perfil, u.Nome;
GO

-- O catalogo tem que continuar com as 5 medalhas.
-- Se vier vazio, alguma coisa apagou o que nao devia: abra o programa
-- desktop uma vez que ele recria.
SELECT Codigo, Nome, XPBonus FROM dbo.Conquistas ORDER BY XPBonus;
GO
