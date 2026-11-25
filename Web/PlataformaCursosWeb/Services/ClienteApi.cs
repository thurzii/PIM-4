/* =====================================================================
   PLATAFORMA DE CURSOS ONLINE - Trabalho em grupo
   Arquivo...: ClienteApi.cs
   ---------------------------------------------------------------------
   VERSAO 3.1 - reescrito a partir do CODIGO da API, nao de suposicao

   O QUE MUDOU DA 3.0 PARA A 3.1
   A versao 3.0 foi escrita antes de o branch main estar disponivel, com
   base so na tabela de endpoints do documento de analise. Ela supunha um
   envelope { ok, mensagem, dados }, combinado no briefing Api/README.md.

   ESSE ENVELOPE NAO EXISTE. Lendo api/src/server.js, a API responde:

     sucesso de login .: { token, user: { id, nome, email, perfil } }
     erro .............: { message: "..." }          <- message, em ingles
     listas ...........: [ {...}, {...} ]            <- array puro, sem embrulho
     criacao ..........: { id, message }
     progresso ........: { message, gamificacao: {...} }

   Quem diz se deu certo e o CODIGO HTTP, nao um campo no corpo.

   ROTAS CONFERIDAS UMA A UMA no server.js. Importante: dos cinco
   arquivos em api/src/routes, so professorRoutes e gamificationRoutes
   estao montados. adminRoutes.js, authRoutes.js e courseRoutes.js
   existem no repositorio mas NAO estao no ar - nenhuma rota deles
   responde. As rotas de curso e de administracao que funcionam sao as
   escritas direto no server.js. Nada aqui chama rota nao montada.

   VERSAO 6 - A API UNICA DO GRUPO
   O desktop, a web e o aplicativo usam a mesma API, unificada pelo
   Guilherme e hospedada no Render. Nao ha mais banco local: este
   cliente e o unico caminho para os dados.
   ===================================================================== */

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using PlataformaCursosWeb.Models;

namespace PlataformaCursosWeb.Services
{
    // =================================================================
    // ConfiguracaoApi - a CHAVE DE LIGA/DESLIGA da integracao
    //
    // Na web a configuracao vem do appsettings.json (secao "Api"), que
    // e onde a configuracao do banco ja mora. A vantagem sobre uma
    // constante no codigo: da para ligar a API sem recompilar, e cada
    // integrante pode apontar para a sua propria maquina sem gerar
    // conflito no Git - mesma ideia da lista "Servidores".
    // =================================================================
    public class ConfiguracaoApi
    {
        private readonly IConfiguration afr_configuracao;

        public ConfiguracaoApi(IConfiguration afr_config)
        {
            afr_configuracao = afr_config;
        }

        public string ObterUrlBase()
        {
            string afr_url = afr_configuracao.GetValue<string>("Api:UrlBase");

            if (string.IsNullOrWhiteSpace(afr_url))
            {
                return "https://pim-iv-api.onrender.com";
            }

            return afr_url;
        }

        public ClienteApi CriarCliente()
        {
            return new ClienteApi(ObterUrlBase());
        }

        public string ObterDescricao()
        {
            return "API: " + ObterUrlBase();
        }
    }

    // =================================================================
    // RespostaApi - o resultado de uma chamada
    //
    // Nao ha envelope, entao esta classe guarda o que importa: se deu
    // certo (pelo codigo HTTP), a mensagem (campo "message", quando
    // vem) e o corpo inteiro, para quem quiser converter.
    // =================================================================
    public class RespostaApi
    {
        public string CodigoRecuperacao {
            get { try { using var doc = JsonDocument.Parse(afr_CorpoJson); return doc.RootElement.TryGetProperty("codigoRecuperacao", out var code) ? code.GetString() ?? "" : ""; } catch (JsonException) { return ""; } }
        }
        public bool afr_Ok { get; set; }
        public int afr_CodigoHttp { get; set; }
        public string afr_Mensagem { get; set; } = "";
        public string afr_CorpoJson { get; set; } = "";

        // Converte o corpo inteiro no tipo pedido. Devolve default (null,
        // para classes e listas) se o corpo nao casar com o formato.
        public T Converter<T>()
        {
            if (string.IsNullOrWhiteSpace(afr_CorpoJson))
            {
                return default(T);
            }

            try
            {
                return JsonSerializer.Deserialize<T>(afr_CorpoJson);
            }
            catch (JsonException)
            {
                return default(T);
            }
        }
    }

    // =================================================================
    // ClienteApi - a conversa HTTP com a API Node.js
    // =================================================================
    public class ClienteApi
    {
        // Um HttpClient so para o programa inteiro. Criar um por
        // chamada esgota as portas da maquina depois de um tempo - e
        // um erro classico com esta classe.
        private static readonly HttpClient afr_http = new HttpClient();

        private readonly string afr_urlBase;

        // O token JWT devolvido pelo login. Vale 8 horas (expiresIn no
        // server.js); depois disso a API devolve 401 e o programa
        // precisa mandar o usuario entrar de novo.
        private string afr_token = "";

        public ClienteApi(string afr_url)
        {
            afr_urlBase = (afr_url ?? "").TrimEnd('/');
        }

        public bool TemToken()
        {
            return afr_token != "";
        }

        public void EsquecerToken()
        {
            afr_token = "";
        }

        // -------------------------------------------------------------
        // VERSAO 4 - o token entra e sai do cliente.
        //
        // Na web cada requisicao do navegador cria o seu ClienteApi, e o
        // objeto morre no fim dela. O token obtido no login precisa
        // sobreviver de um clique para o outro, entao ele vai para o
        // cookie da pessoa (ver ContaController) e volta para ca a cada
        // requisicao. No desktop o cliente e um so para o programa
        // inteiro e o token fica dentro dele.
        // -------------------------------------------------------------
        public void UsarToken(string afr_novoToken)
        {
            afr_token = afr_novoToken ?? "";
        }

        public string ObterToken()
        {
            return afr_token;
        }

        // -------------------------------------------------------------
        // GET /api/health
        // Serve para a tela "testar conexao": responde se a API esta no
        // ar E se ela conseguiu falar com o banco.
        // -------------------------------------------------------------
        public RespostaApi Verificar()
        {
            return Chamar(HttpMethod.Get, "/api/health", null);
        }

        // -------------------------------------------------------------
        // POST /api/auth/login   ->   { token, user: {...} }
        // -------------------------------------------------------------
        public RespostaApi Entrar(string afr_email, string afr_senha)
        {
            Dictionary<string, string> afr_corpo = new Dictionary<string, string>();

            // A API grava e procura email em minusculas
            // (email.trim().toLowerCase() no server.js). Mandar com
            // maiuscula faria o login falhar sem explicacao.
            afr_corpo["email"] = (afr_email ?? "").Trim().ToLowerInvariant();
            afr_corpo["senha"] = afr_senha ?? "";

            RespostaApi afr_resposta = Chamar(HttpMethod.Post, "/api/auth/login", afr_corpo);

            if (afr_resposta.afr_Ok)
            {
                LoginApi afr_login = afr_resposta.Converter<LoginApi>();

                if (afr_login != null && !string.IsNullOrEmpty(afr_login.afr_Token))
                {
                    afr_token = afr_login.afr_Token;
                }
            }

            return afr_resposta;
        }

        // -------------------------------------------------------------
        // POST /api/auth/recover
        //
        // Hoje responde SEMPRE 501, dizendo para procurar o
        // administrador. Esta aqui para o programa dar a mensagem certa
        // em vez de parecer quebrado.
        // -------------------------------------------------------------
        public RespostaApi Registrar(string nome, string email, string senha, string confirmarSenha)
            => Chamar(HttpMethod.Post, "/api/auth/register", new { nome, email, senha, confirmarSenha });
        public RespostaApi PrimeiroAcesso(string tokenPrimeiroAcesso, string senhaTemporaria, string novaSenha)
            => Chamar(HttpMethod.Post, "/api/auth/primeiro-acesso", new { tokenPrimeiroAcesso, senhaTemporaria, novaSenha });

        public RespostaApi RecuperarSenha(string afr_email, string codigo, string novaSenha)
        {
            Dictionary<string, string> afr_corpo = new Dictionary<string, string>();
            afr_corpo["email"] = (afr_email ?? "").Trim().ToLowerInvariant();
            afr_corpo["codigoRecuperacao"] = codigo;
            afr_corpo["novaSenha"] = novaSenha;
            return Chamar(HttpMethod.Post, "/api/auth/recover", afr_corpo);
        }

        // -------------------------------------------------------------
        // GET /api/cursos?busca=...   ->   ARRAY de cursos (sem embrulho)
        //
        // Desde que o courseRoutes foi montado (19/11), esta rota e
        // studentOnly: professor e admin levam 403. O catalogo das telas
        // deles so funciona no modo SQL ate isso mudar do lado da API.
        // -------------------------------------------------------------
        public RespostaApi ListarCursos(string afr_busca)
        {
            return Chamar(HttpMethod.Get,
                          "/api/cursos?busca=" + Uri.EscapeDataString(afr_busca ?? ""),
                          null);
        }

        // -------------------------------------------------------------
        // GET /api/cursos/:id  ->  o curso COM as aulas e o progresso
        // -------------------------------------------------------------
        public RespostaApi ObterCurso(int afr_cursoId)
        {
            return Chamar(HttpMethod.Get, "/api/cursos/" + afr_cursoId, null);
        }

        // -------------------------------------------------------------
        // PUT /api/cursos/:cursoId/aulas/:aulaId/progresso
        //      corpo: { concluida: true|false }
        //      resposta: { message, gamificacao: {...} }
        //
        // A gamificacao so vem preenchida quando concluida = true; ao
        // desmarcar, a API manda null nesse campo.
        //
        // Desde 19/11 a rota exige matricula: sem linha em dbo.Matriculas
        // ela responde 403 "Matricule-se no curso antes de registrar o
        // progresso". Chame MatricularEmCurso antes da primeira aula.
        // -------------------------------------------------------------
        public RespostaApi MarcarProgresso(int afr_cursoId, int afr_aulaId, bool afr_concluida)
        {
            Dictionary<string, object> afr_corpo = new Dictionary<string, object>();
            afr_corpo["concluida"] = afr_concluida;

            string afr_rota = "/api/cursos/" + afr_cursoId +
                              "/aulas/" + afr_aulaId + "/progresso";

            return Chamar(HttpMethod.Put, afr_rota, afr_corpo);
        }

        // -------------------------------------------------------------
        // GET /api/gamificacao/me  ->  { xp, nivel, sequenciaAtual,
        //                                melhorSequencia, aulasConcluidas,
        //                                conquistas: [...] }
        //
        // Atencao: esta rota e marcada studentOnly no router dela.
        // Administrador e professor levam 403, mesmo logados.
        // -------------------------------------------------------------
        public RespostaApi ObterGamificacao()
        {
            return Chamar(HttpMethod.Get, "/api/gamificacao/me", null);
        }

        // -------------------------------------------------------------
        // GET /api/gamificacao/turma  ->  ARRAY de alunos, ja ordenado
        //      por XP e com a posicao calculada pela API
        //
        // Criada a nosso pedido: /me e studentOnly, entao professor e
        // admin nao tinham como ler o XP de ninguem. Esta aceita so
        // PROFESSOR e ADMIN; aluno leva 403.
        //
        // Atencao: ela NAO devolve aulas concluidas nem quantidade de
        // conquistas. Essas duas colunas da tela de ranking ficam em
        // zero no modo API ate a rota passar a devolve-las.
        // -------------------------------------------------------------
        public RespostaApi ObterRankingTurma()
        {
            return Chamar(HttpMethod.Get, "/api/gamificacao/turma", null);
        }

        // -------------------------------------------------------------
        // POST /api/cursos/:id/matricula  ->  201 { message }
        //
        // Passou a existir quando o courseRoutes foi montado. E o
        // equivalente, pela API, de CursoService.Matricular, que no
        // modo SQL grava direto em dbo.Matriculas.
        // -------------------------------------------------------------
        public RespostaApi MatricularEmCurso(int afr_cursoId)
        {
            return Chamar(HttpMethod.Post,
                          "/api/cursos/" + afr_cursoId + "/matricula",
                          null);
        }

        // -------------------------------------------------------------
        // Rotas de administrador (todas exigem perfil ADMIN)
        // -------------------------------------------------------------
        public RespostaApi ListarUsuarios(string afr_busca)
        {
            return Chamar(HttpMethod.Get,
                          "/api/admin/usuarios?busca=" + Uri.EscapeDataString(afr_busca ?? ""),
                          null);
        }

        public RespostaApi CriarUsuario(string afr_nome, string afr_email,
                                        string afr_senha, string afr_perfil)
        {
            Dictionary<string, string> afr_corpo = new Dictionary<string, string>();

            afr_corpo["nome"] = afr_nome ?? "";
            afr_corpo["email"] = (afr_email ?? "").Trim().ToLowerInvariant();
            afr_corpo["senha"] = afr_senha ?? "";

            // A API aceita so ALUNO, PROFESSOR ou ADMIN, em maiusculas
            afr_corpo["perfil"] = (afr_perfil ?? "").ToUpperInvariant();

            return Chamar(HttpMethod.Post, "/api/admin/usuarios", afr_corpo);
        }

        public RespostaApi AlterarStatusUsuario(int afr_usuarioId, bool afr_ativo)
        {
            Dictionary<string, object> afr_corpo = new Dictionary<string, object>();
            afr_corpo["ativo"] = afr_ativo;

            return Chamar(HttpMethod.Put,
                          "/api/admin/usuarios/" + afr_usuarioId + "/status",
                          afr_corpo);
        }

        public RespostaApi ListarCursosAdmin(string afr_busca)
        {
            return Chamar(HttpMethod.Get,
                          "/api/admin/cursos?busca=" + Uri.EscapeDataString(afr_busca ?? ""),
                          null);
        }

        public RespostaApi CriarCurso(string afr_titulo, string afr_descricao,
                                      int afr_cargaHoraria, bool afr_ativo)
        {
            Dictionary<string, object> afr_corpo = new Dictionary<string, object>();

            afr_corpo["titulo"] = afr_titulo ?? "";
            afr_corpo["descricao"] = afr_descricao ?? "";
            afr_corpo["cargaHoraria"] = afr_cargaHoraria;
            afr_corpo["ativo"] = afr_ativo;

            return Chamar(HttpMethod.Post, "/api/admin/cursos", afr_corpo);
        }

        public RespostaApi AlterarCurso(int afr_cursoId, string afr_titulo,
                                        string afr_descricao, int afr_cargaHoraria,
                                        bool afr_ativo)
        {
            Dictionary<string, object> afr_corpo = new Dictionary<string, object>();

            afr_corpo["titulo"] = afr_titulo ?? "";
            afr_corpo["descricao"] = afr_descricao ?? "";
            afr_corpo["cargaHoraria"] = afr_cargaHoraria;
            afr_corpo["ativo"] = afr_ativo;

            return Chamar(HttpMethod.Put, "/api/admin/cursos/" + afr_cursoId, afr_corpo);
        }

        public RespostaApi ExcluirCurso(int afr_cursoId)
        {
            return Chamar(HttpMethod.Delete, "/api/admin/cursos/" + afr_cursoId, null);
        }

        public RespostaApi CriarAula(int afr_cursoId, string afr_titulo, int afr_duracaoMin, string conteudo = null)
        {
            Dictionary<string, object> afr_corpo = new Dictionary<string, object>();

            afr_corpo["titulo"] = afr_titulo ?? "";
            afr_corpo["duracaoMin"] = afr_duracaoMin;
            if (conteudo != null) afr_corpo["conteudo"] = conteudo;

            // A ordem da aula NAO vai no corpo: a API calcula sozinha
            // (MAX(Ordem)+1 dentro do curso).
            return Chamar(HttpMethod.Post,
                          "/api/admin/cursos/" + afr_cursoId + "/aulas",
                          afr_corpo);
        }

        public RespostaApi ExcluirAula(int afr_aulaId)
        {
            return Chamar(HttpMethod.Delete, "/api/admin/aulas/" + afr_aulaId, null);
        }

        // -------------------------------------------------------------
        // VERSAO 4 - rotas do PROFESSOR (professorRoutes.js)
        //
        // Todas sao professorOnly e so enxergam os cursos em que
        // Cursos.ProfessorId e o professor logado. As de admin acima sao
        // adminOnly. Por isso o mesmo botao da tela chama uma ou outra,
        // conforme o perfil (quem escolhe e a PonteApi).
        //
        // Diferencas que importam: o professor NAO tem rota para excluir
        // curso, e criar/editar curso nao recebe "ativo".
        // -------------------------------------------------------------
        public RespostaApi ListarCursosProfessor()
        {
            return Chamar(HttpMethod.Get, "/api/professor/cursos", null);
        }

        public RespostaApi CriarCursoProfessor(string afr_titulo, string afr_descricao,
                                               int afr_cargaHoraria)
        {
            Dictionary<string, object> afr_corpo = new Dictionary<string, object>();

            afr_corpo["titulo"] = afr_titulo ?? "";
            afr_corpo["descricao"] = afr_descricao ?? "";
            afr_corpo["cargaHoraria"] = afr_cargaHoraria;

            return Chamar(HttpMethod.Post, "/api/professor/cursos", afr_corpo);
        }

        public RespostaApi AlterarCursoProfessor(int afr_cursoId, string afr_titulo,
                                                 string afr_descricao, int afr_cargaHoraria)
        {
            Dictionary<string, object> afr_corpo = new Dictionary<string, object>();

            afr_corpo["titulo"] = afr_titulo ?? "";
            afr_corpo["descricao"] = afr_descricao ?? "";
            afr_corpo["cargaHoraria"] = afr_cargaHoraria;

            return Chamar(HttpMethod.Put, "/api/professor/cursos/" + afr_cursoId, afr_corpo);
        }

        public RespostaApi CriarAulaProfessor(int afr_cursoId, string afr_titulo, int afr_duracaoMin, string conteudo = null)
        {
            Dictionary<string, object> afr_corpo = new Dictionary<string, object>();

            afr_corpo["titulo"] = afr_titulo ?? "";
            afr_corpo["duracaoMin"] = afr_duracaoMin;
            if (conteudo != null) afr_corpo["conteudo"] = conteudo;

            return Chamar(HttpMethod.Post,
                          "/api/professor/cursos/" + afr_cursoId + "/aulas",
                          afr_corpo);
        }

        public RespostaApi ExcluirAulaProfessor(int afr_aulaId)
        {
            return Chamar(HttpMethod.Delete, "/api/professor/aulas/" + afr_aulaId, null);
        }

        // -------------------------------------------------------------
        // VERSAO 5 - rotas a mais da API do grupo
        //
        // A API do grupo e propria do desktop e da web, com as mesmas
        // rotas e regras da API do Gui, mais estas, que as telas daqui
        // precisam. Todas usam tabelas que ja existem no banco do Azure.
        // -------------------------------------------------------------
        public RespostaApi ObterUsuario(int afr_usuarioId)
        {
            return Chamar(HttpMethod.Get, "/api/admin/usuarios/" + afr_usuarioId, null);
        }

        // Senha vazia = manter a atual
        public RespostaApi AlterarUsuario(int afr_usuarioId, string afr_nome, string afr_email,
                                          string afr_senha, string afr_perfil, bool afr_ativo)
        {
            Dictionary<string, object> afr_corpo = new Dictionary<string, object>();

            afr_corpo["nome"] = afr_nome ?? "";
            afr_corpo["email"] = (afr_email ?? "").Trim().ToLowerInvariant();
            afr_corpo["senha"] = afr_senha ?? "";
            afr_corpo["perfil"] = (afr_perfil ?? "").ToUpperInvariant();
            afr_corpo["ativo"] = afr_ativo;

            return Chamar(HttpMethod.Put, "/api/admin/usuarios/" + afr_usuarioId, afr_corpo);
        }

        public RespostaApi ExcluirUsuario(int afr_usuarioId)
        {
            return Chamar(HttpMethod.Delete, "/api/admin/usuarios/" + afr_usuarioId, null);
        }

        // Uma aula pelo id, ja com o curso dela e se quem pede concluiu
        public RespostaApi AbrirAula(int afr_aulaId)
        {
            return Chamar(HttpMethod.Get, "/api/aulas/" + afr_aulaId, null);
        }

        public RespostaApi AlterarAula(int afr_aulaId, string afr_titulo, int afr_duracaoMin, string conteudo = null)
        {
            return Chamar(HttpMethod.Put, "/api/admin/aulas/" + afr_aulaId,
                          CorpoAula(afr_titulo, afr_duracaoMin, conteudo));
        }

        public RespostaApi AlterarAulaProfessor(int afr_aulaId, string afr_titulo, int afr_duracaoMin, string conteudo = null)
        {
            return Chamar(HttpMethod.Put, "/api/professor/aulas/" + afr_aulaId,
                          CorpoAula(afr_titulo, afr_duracaoMin, conteudo));
        }

        private static Dictionary<string, object> CorpoAula(string afr_titulo, int afr_duracaoMin, string conteudo = null)
        {
            Dictionary<string, object> afr_corpo = new Dictionary<string, object>();

            afr_corpo["titulo"] = afr_titulo ?? "";
            afr_corpo["duracaoMin"] = afr_duracaoMin;
            if (conteudo != null) afr_corpo["conteudo"] = conteudo;

            return afr_corpo;
        }

        // O progresso de UM aluno em cada curso (professor ve os dele)
        public RespostaApi ObterProgressoDoAluno(int afr_alunoId)
        {
            return Chamar(HttpMethod.Get, "/api/progresso/alunos/" + afr_alunoId, null);
        }

        // O mesmo que /gamificacao/me, de outro aluno (professor e admin)
        public RespostaApi ObterGamificacaoDoAluno(int afr_alunoId)
        {
            return Chamar(HttpMethod.Get, "/api/gamificacao/alunos/" + afr_alunoId, null);
        }

        // -------------------------------------------------------------
        // O metodo que faz a chamada de verdade.
        //
        // Sincrono de proposito (.GetAwaiter().GetResult()): o resto do
        // programa e todo sincrono, e misturar async pela metade daria
        // mais problema do que resolve num trabalho deste tamanho.
        // -------------------------------------------------------------
        private RespostaApi Chamar(HttpMethod afr_metodo, string afr_rota, object afr_corpo)
        {
            RespostaApi afr_resposta = new RespostaApi();

            try
            {
                HttpRequestMessage afr_requisicao =
                    new HttpRequestMessage(afr_metodo, afr_urlBase + afr_rota);

                if (afr_corpo != null)
                {
                    string afr_json = JsonSerializer.Serialize(afr_corpo);
                    afr_requisicao.Content =
                        new StringContent(afr_json, Encoding.UTF8, "application/json");
                }

                // Authorization: Bearer <token> nas rotas protegidas
                if (afr_token != "")
                {
                    afr_requisicao.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", afr_token);
                }

                HttpResponseMessage afr_retorno =
                    afr_http.SendAsync(afr_requisicao).GetAwaiter().GetResult();

                string afr_texto =
                    afr_retorno.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                afr_resposta.afr_CodigoHttp = (int)afr_retorno.StatusCode;
                afr_resposta.afr_Ok = afr_retorno.IsSuccessStatusCode;
                afr_resposta.afr_CorpoJson = afr_texto;

                // O token venceu (8 horas) ou nao foi mandado
                if (afr_resposta.afr_CodigoHttp == 401)
                {
                    EsquecerToken();
                    afr_resposta.afr_Mensagem = "Sessão expirada. Entre novamente.";
                    return afr_resposta;
                }

                LerMensagem(afr_resposta);

                // Mensagens padrao quando a API nao mandou nenhuma
                if (afr_resposta.afr_Mensagem == "")
                {
                    if (afr_resposta.afr_CodigoHttp == 403)
                    {
                        afr_resposta.afr_Mensagem =
                            "Seu perfil não tem permissão para esta operação.";
                    }
                    else if (afr_resposta.afr_CodigoHttp == 501)
                    {
                        afr_resposta.afr_Mensagem =
                            "Esta operação ainda não existe na API.";
                    }
                    else if (!afr_resposta.afr_Ok)
                    {
                        afr_resposta.afr_Mensagem =
                            "A API respondeu com o código " + afr_resposta.afr_CodigoHttp + ".";
                    }
                }

                return afr_resposta;
            }
            catch (Exception afr_erro)
            {
                // API fora do ar, URL errada, sem rede. Nunca deixa a
                // excecao subir ate a tela: vira mensagem.
                afr_resposta.afr_Ok = false;
                afr_resposta.afr_Mensagem = "Não foi possível falar com a API: " +
                                            afr_erro.Message;
                return afr_resposta;
            }
        }

        // -------------------------------------------------------------
        // Procura o campo "message" no corpo.
        //
        // E "message", em ingles - foi assim que a API foi escrita. Eu
        // tinha suposto "mensagem" na versao 3.0, e com isso toda
        // mensagem de erro dela se perderia pelo caminho.
        // -------------------------------------------------------------
        private void LerMensagem(RespostaApi afr_resposta)
        {
            if (string.IsNullOrWhiteSpace(afr_resposta.afr_CorpoJson))
            {
                return;
            }

            try
            {
                using (JsonDocument afr_documento =
                           JsonDocument.Parse(afr_resposta.afr_CorpoJson))
                {
                    // Lista nao tem campo nenhum para ler
                    if (afr_documento.RootElement.ValueKind != JsonValueKind.Object)
                    {
                        return;
                    }

                    JsonElement afr_mensagem;

                    if (afr_documento.RootElement.TryGetProperty("message", out afr_mensagem) &&
                        afr_mensagem.ValueKind == JsonValueKind.String)
                    {
                        afr_resposta.afr_Mensagem = afr_mensagem.GetString();
                    }
                }
            }
            catch (JsonException)
            {
                // Corpo que nao e JSON (uma pagina de erro do servidor,
                // por exemplo). Nao e motivo para quebrar.
                afr_resposta.afr_Mensagem = "A API respondeu em um formato inesperado.";
            }
        }
    }

    // =================================================================
    // OS FORMATOS DA API
    //
    // Cada [JsonPropertyName] abaixo foi copiado dos SELECT e dos
    // res.json do server.js, nao suposto. Os apelidos em SQL
    // ("c.Titulo AS titulo") sao o que define o nome do campo.
    // =================================================================

    public class LoginApi
    {
        [JsonPropertyName("primeiroAcesso")] public bool PrimeiroAcesso { get; set; }
        [JsonPropertyName("tokenPrimeiroAcesso")] public string TokenPrimeiroAcesso { get; set; }
        [JsonPropertyName("token")]
        public string afr_Token { get; set; } = "";

        // "user", em ingles - nao "usuario"
        [JsonPropertyName("user")]
        public UsuarioApi afr_Usuario { get; set; }
    }

    public class UsuarioApi
    {
        [JsonPropertyName("id")]
        public int afr_Id { get; set; }

        [JsonPropertyName("nome")]
        public string afr_Nome { get; set; } = "";

        [JsonPropertyName("email")]
        public string afr_Email { get; set; } = "";

        [JsonPropertyName("perfil")]
        public string afr_Perfil { get; set; } = "";

        [JsonPropertyName("ativo")]
        public bool afr_Ativo { get; set; }

        // VERSAO 5 - a nossa API devolve; a do Gui nao (fica vazio)
        [JsonPropertyName("dataCadastro")]
        public DateTime? afr_DataCadastro { get; set; }

        // Converte para a classe que o resto do programa ja usa, para
        // nenhuma tela precisar saber que o dado veio da API.
        public Usuario ParaUsuario()
        {
            Usuario afr_usuario = new Usuario();

            if (afr_DataCadastro.HasValue)
            {
                afr_usuario.afr_DataCadastro = afr_DataCadastro.Value;
            }

            afr_usuario.afr_Id = afr_Id;
            afr_usuario.afr_Nome = afr_Nome ?? "";
            afr_usuario.afr_Email = afr_Email ?? "";
            afr_usuario.afr_Perfil = (afr_Perfil ?? "").ToUpperInvariant();
            afr_usuario.afr_Ativo = afr_Ativo;

            return afr_usuario;
        }
    }

    public class CursoApi
    {
        [JsonPropertyName("id")]
        public int afr_Id { get; set; }

        [JsonPropertyName("titulo")]
        public string afr_Titulo { get; set; } = "";

        [JsonPropertyName("descricao")]
        public string afr_Descricao { get; set; } = "";

        [JsonPropertyName("cargaHoraria")]
        public int afr_CargaHoraria { get; set; }

        [JsonPropertyName("ativo")]
        public bool afr_Ativo { get; set; }

        [JsonPropertyName("totalAulas")]
        public int afr_TotalAulas { get; set; }

        public Curso ParaCurso()
        {
            Curso afr_curso = new Curso();

            afr_curso.afr_Id = afr_Id;
            afr_curso.afr_Titulo = afr_Titulo ?? "";
            afr_curso.afr_Descricao = afr_Descricao ?? "";
            afr_curso.afr_CargaHoraria = afr_CargaHoraria;
            afr_curso.afr_Ativo = afr_Ativo;
            afr_curso.afr_TotalAulas = afr_TotalAulas;

            return afr_curso;
        }
    }

    // GET /api/cursos/:id devolve o curso COM as aulas dentro
    public class CursoDetalheApi : CursoApi
    {
        [JsonPropertyName("aulas")]
        public List<AulaApi> afr_Aulas { get; set; } = new List<AulaApi>();

        [JsonPropertyName("progresso")]
        public int afr_Progresso { get; set; }

        [JsonPropertyName("concluidas")]
        public int afr_Concluidas { get; set; }

        // Passou a vir no corpo quando a matricula virou explicita.
        // Diz se o aluno logado ja está matriculado neste curso.
        [JsonPropertyName("matriculado")]
        public bool afr_Matriculado { get; set; }
    }

    // GET /api/gamificacao/turma devolve um array destes.
    // A API ja manda ordenado e com a posicao pronta, entao nao
    // reordenamos aqui: duas ordens diferentes para o mesmo ranking
    // seria exatamente o tipo de divergencia que a rota veio evitar.
    public class RankingApi
    {
        [JsonPropertyName("posicao")]
        public int afr_Posicao { get; set; }

        [JsonPropertyName("id")]
        public int afr_Id { get; set; }

        [JsonPropertyName("nome")]
        public string afr_Nome { get; set; } = "";

        [JsonPropertyName("email")]
        public string afr_Email { get; set; } = "";

        [JsonPropertyName("xp")]
        public int afr_Xp { get; set; }

        [JsonPropertyName("nivel")]
        public int afr_Nivel { get; set; }

        [JsonPropertyName("sequenciaAtual")]
        public int afr_SequenciaAtual { get; set; }

        [JsonPropertyName("melhorSequencia")]
        public int afr_MelhorSequencia { get; set; }

        public LinhaRanking ParaLinhaRanking()
        {
            LinhaRanking afr_linha = new LinhaRanking();

            afr_linha.afr_Posicao = afr_Posicao;
            afr_linha.afr_UsuarioId = afr_Id;
            afr_linha.afr_Nome = afr_Nome ?? "";
            afr_linha.afr_Email = afr_Email ?? "";
            afr_linha.afr_XpTotal = afr_Xp;
            afr_linha.afr_SequenciaAtual = afr_SequenciaAtual;
            afr_linha.afr_MelhorSequencia = afr_MelhorSequencia;

            // afr_Nivel vem calculado pela rota e nao e copiado: o
            // LinhaRanking calcula o nivel com a mesma formula
            // (XP / 100 + 1), e guardar os dois abriria espaco para
            // divergirem. O campo existe no DTO so para o JSON bater.
            return afr_linha;
        }
    }

    public class AulaApi
    {
        [JsonPropertyName("conteudo")] public string Conteudo { get; set; }
        [JsonPropertyName("id")]
        public int afr_Id { get; set; }

        // VERSAO 5 - so vem em GET /api/aulas/:id
        [JsonPropertyName("cursoId")]
        public int afr_CursoId { get; set; }

        [JsonPropertyName("titulo")]
        public string afr_Titulo { get; set; } = "";

        [JsonPropertyName("ordem")]
        public int afr_Ordem { get; set; }

        [JsonPropertyName("duracaoMin")]
        public int afr_DuracaoMin { get; set; }

        [JsonPropertyName("concluida")]
        public bool afr_Concluida { get; set; }

        public Aula ParaAula(int afr_cursoId)
        {
            Aula afr_aula = new Aula();

            afr_aula.afr_Id = afr_Id;
            afr_aula.afr_CursoId = afr_cursoId;
            afr_aula.afr_Titulo = afr_Titulo ?? "";
            afr_aula.afr_Ordem = afr_Ordem;
            afr_aula.afr_DuracaoMin = afr_DuracaoMin;
            afr_aula.afr_Concluida = afr_Concluida;
            afr_aula.afr_Conteudo = Conteudo ?? "";

            return afr_aula;
        }
    }

    // O que vem dentro de "gamificacao" ao concluir uma aula
    public class GamificacaoApi
    {
        [JsonPropertyName("xpAula")]
        public int afr_XpAula { get; set; }

        [JsonPropertyName("xpConquistas")]
        public int afr_XpConquistas { get; set; }

        [JsonPropertyName("xpTotal")]
        public int afr_XpTotal { get; set; }

        [JsonPropertyName("sequenciaAtual")]
        public int afr_SequenciaAtual { get; set; }

        [JsonPropertyName("melhorSequencia")]
        public int afr_MelhorSequencia { get; set; }

        [JsonPropertyName("nivel")]
        public int afr_Nivel { get; set; }

        [JsonPropertyName("novasConquistas")]
        public List<ConquistaApi> afr_NovasConquistas { get; set; } = new List<ConquistaApi>();

        // Converte para a classe que as telas ja usam
        public GanhoGamificacao ParaGanho()
        {
            GanhoGamificacao afr_ganho = new GanhoGamificacao();

            afr_ganho.afr_XpAula = afr_XpAula;
            afr_ganho.afr_XpConquistas = afr_XpConquistas;
            afr_ganho.afr_XpTotal = afr_XpTotal;
            afr_ganho.afr_SequenciaAtual = afr_SequenciaAtual;
            afr_ganho.afr_MelhorSequencia = afr_MelhorSequencia;

            foreach (ConquistaApi afr_item in afr_NovasConquistas)
            {
                afr_ganho.afr_NovasConquistas.Add(afr_item.ParaConquista());
            }

            return afr_ganho;
        }
    }

    // VERSAO 5 - GET /api/progresso/alunos/:id (uma linha por curso)
    public class ProgressoAlunoApi : CursoApi
    {
        [JsonPropertyName("concluidas")]
        public int afr_Concluidas { get; set; }
    }

    // VERSAO 4 - GET /api/gamificacao/me (so aluno)
    public class PontuacaoApi
    {
        [JsonPropertyName("xp")]
        public int afr_Xp { get; set; }

        [JsonPropertyName("sequenciaAtual")]
        public int afr_SequenciaAtual { get; set; }

        [JsonPropertyName("melhorSequencia")]
        public int afr_MelhorSequencia { get; set; }

        [JsonPropertyName("aulasConcluidas")]
        public int afr_AulasConcluidas { get; set; }

        [JsonPropertyName("conquistas")]
        public List<ConquistaApi> afr_Conquistas { get; set; } = new List<ConquistaApi>();

        // "nivel" tambem vem no JSON e nao e lido: a Pontuacao calcula o
        // nivel pela mesma formula (XP / 100 + 1). Guardar os dois daria
        // espaco para divergirem - mesmo motivo do RankingApi.
        public Pontuacao ParaPontuacao(int afr_usuarioId)
        {
            Pontuacao afr_pontuacao = new Pontuacao();

            afr_pontuacao.afr_UsuarioId = afr_usuarioId;
            afr_pontuacao.afr_XpTotal = afr_Xp;
            afr_pontuacao.afr_SequenciaAtual = afr_SequenciaAtual;
            afr_pontuacao.afr_MelhorSequencia = afr_MelhorSequencia;
            afr_pontuacao.afr_AulasConcluidas = afr_AulasConcluidas;

            foreach (ConquistaApi afr_item in afr_Conquistas)
            {
                afr_pontuacao.afr_Conquistas.Add(afr_item.ParaConquista());
            }

            return afr_pontuacao;
        }
    }

    public class ConquistaApi
    {
        [JsonPropertyName("id")]
        public int afr_Id { get; set; }

        [JsonPropertyName("codigo")]
        public string afr_Codigo { get; set; } = "";

        [JsonPropertyName("nome")]
        public string afr_Nome { get; set; } = "";

        [JsonPropertyName("descricao")]
        public string afr_Descricao { get; set; } = "";

        [JsonPropertyName("icone")]
        public string afr_Icone { get; set; } = "";

        [JsonPropertyName("xpBonus")]
        public int afr_XpBonus { get; set; }

        [JsonPropertyName("conquistada")]
        public bool afr_Conquistada { get; set; }

        public Conquista ParaConquista()
        {
            Conquista afr_conquista = new Conquista();

            afr_conquista.afr_Id = afr_Id;
            afr_conquista.afr_Codigo = afr_Codigo ?? "";
            afr_conquista.afr_Nome = afr_Nome ?? "";
            afr_conquista.afr_Descricao = afr_Descricao ?? "";
            afr_conquista.afr_Icone = afr_Icone ?? "";
            afr_conquista.afr_XpBonus = afr_XpBonus;
            afr_conquista.afr_Conquistada = afr_Conquistada;

            return afr_conquista;
        }
    }
}
