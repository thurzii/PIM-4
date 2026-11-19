# Plataforma de Cursos Mobile — PIM IV

Aplicativo educacional em **React Native + Expo**, com API **Node.js/Express**, autenticação **JWT** e persistência no **SQL Server**. O aplicativo consome a API HTTP; credenciais de banco ficam somente no backend.

## Perfis e permissões

- **ALUNO:** cadastro público, login, catálogo, progresso de aulas, perfil e conquistas.
- **PROFESSOR:** criado por administrador; gerencia os próprios cursos, aulas e acompanha matrículas.
- **ADMIN:** criado somente por outro administrador; gerencia usuários e cursos. Não existe cadastro público de administradores.

A API define o perfil no cadastro público, independentemente de valores enviados pelo aplicativo. Senhas novas usam `scrypt`; há compatibilidade legada com senhas antigas, que devem ser migradas. Recuperação automática de senha ainda não foi implementada.

## Instalação e execução

Requisitos: Node.js compatível com o Expo do projeto, Android Emulator/Expo Go, SQL Server acessível.

1. Copie `api/.env.example` para `api/.env` e preencha `DB_SERVER`, `DB_DATABASE`, `DB_USER`, `DB_PASSWORD`, `JWT_SECRET` e configurações de criptografia. **Nunca publique `.env`**.
2. Na pasta `api`, execute `npm install` e `npm run dev`.
3. Verifique `http://localhost:3000/api/health` (API e banco devem indicar `ok`).
4. Na pasta `mobile`, configure `EXPO_PUBLIC_API_URL` em `.env`: emulador Android `http://10.0.2.2:3000/api`; celular físico `http://IP_DO_PC:3000/api` na mesma rede.
5. Execute `npm install`, `npx expo install @expo/vector-icons` e `npx expo start -c` na pasta `mobile`.

## Banco de dados e scripts SQL

**Não execute scripts diretamente no banco de produção sem backup e conferência do banco selecionado.**

| Script | Finalidade | Recomendação |
|---|---|---|
| `sql/CursosDB.sql` | Estrutura base de Usuarios, Cursos, Aulas e Progresso; inclui cursos de exemplo | Manter como bootstrap histórico; revisar antes de executar |
| `sql/Etapa1_Professor.sql` | Adiciona Cursos.ProfessorId e Matriculas | Manter; executar após a estrutura base |
| `sql/Etapa4_Gamificacao.sql` | Cria Gamificacao, HistoricoXP, Conquistas e UsuarioConquistas | Manter; executar após a estrutura base |
| `sql/DiagnosticoBanco.sql` | Consulta colunas e relacionamentos sem modificar dados | Manter para suporte; diagnóstico ampliado pode incluir gamificação |
| `sql/Desativar_Usuarios_Demo.sql` | Desativa contas de teste com e-mails fixos | Arquivar; não faz parte da instalação normal |

### Atenção aos nomes de banco

`CursosDB.sql` cria/seleciona **CursosDB**; `Desativar_Usuarios_Demo.sql` seleciona **PlataformaCursosDB**. Esses nomes não devem ser presumidos como o banco Azure atual. Confirme `DB_DATABASE` em `api/.env` e a conexão no SSMS/Azure Data Studio. Para banco existente, **não rode novamente o script inicial sem revisar**. O script base inclui inserts de demonstração e `CREATE DATABASE`.

### Tabelas e relacionamentos

- `Usuarios`: identidade, e-mail, hash de senha, perfil e status.
- `Cursos`: catálogo, carga horária, estado e `ProfessorId` após a migração.
- `Aulas`: aulas pertencentes aos cursos.
- `Progresso`: conclusão atual por aluno e aula (par único).
- `Matriculas`: vínculo aluno-curso.
- `Gamificacao`: XP total e sequências de estudo.
- `HistoricoXP`: eventos de XP concedidos uma única vez por referência.
- `Conquistas`: catálogo de conquistas e bônus.
- `UsuarioConquistas`: conquistas desbloqueadas pelos alunos.

**Ordem para banco novo (revisar nomes e permissões antes):** estrutura base → professor/matrículas → gamificação → diagnóstico. O script de desativação de demo é opcional e não deve ser usado automaticamente.

## Progresso e gamificação

A tela de curso usa **checkboxes** para conclusão. O aplicativo atualiza a interface imediatamente e reverte se a API retornar erro. A API registra em `Progresso` e chama `awardLessonCompletion` para conceder **10 XP por aula nova**, além dos bônus de conquistas. O `HistoricoXP` impede conceder XP duplicado ao marcar a mesma aula novamente. Desmarcar a aula remove a conclusão atual, **mas não estorna XP já concedido**; o perfil conta aulas atualmente concluídas em `Progresso`, enquanto o XP é histórico. O perfil recarrega os dados ao receber foco.

O backend principal usado por `npm run dev` é `api/src/server.js`; existem módulos em `api/src/routes/` que não estão todos montados no servidor. Evite editar uma rota isolada supondo que esteja ativa. Para futura manutenção, recomenda-se consolidar as rotas em módulos únicos.

## Testes manuais recomendados

1. Criar conta pública e confirmar perfil ALUNO; tentar enviar ADMIN pelo cadastro público e verificar que não ganha privilégios.
2. Fazer login como ADMIN, cadastrar professor e validar permissões.
3. Matricular aluno, marcar aula uma vez e conferir `Progresso`, `Gamificacao` e `HistoricoXP` no SQL Server.
4. Abrir perfil e verificar XP, nível e número de aulas; desmarcar/remarcar e conferir que XP não duplica.
5. Conferir home e gerenciamento de cursos em ADMIN e PROFESSOR.

## Limitações e segurança

- Integração real com SQL Server/Android precisa ser validada no ambiente da equipe.
- Recuperação de senha via e-mail não está implementada.
- A contagem de matrículas no painel do professor é a soma das matrículas por curso, não usuários únicos.
- Use HTTPS e segredos fortes em produção; restrinja permissões do usuário SQL.
- Não envie `node_modules`, `.expo` ou `.env` ao Git.


## Atualização visual — painéis de professor e administrador

As telas `AdminScreen.js` e `ProfessorCoursesScreen.js` utilizam agora cabeçalhos com SafeAreaView, cards de indicadores, área de destaque e ações rápidas. O administrador consulta `/admin/cursos` e o professor consulta `/professor/cursos`. Os indicadores são calculados a partir das listas retornadas por essas rotas, sem números fictícios. O total de matrículas do professor é a soma das matrículas por curso (um aluno em dois cursos conta duas vezes). Para instalar as dependências, execute `npx expo install react-native-safe-area-context @expo/vector-icons` na pasta mobile.


## Cadastro de usuarios — somente ADMIN

O cadastro publico foi removido. Somente usuarios autenticados com perfil `ADMIN` podem cadastrar novos usuarios (`ALUNO`, `PROFESSOR` ou `ADMIN`) pelo painel administrativo, usando `POST /api/admin/usuarios`. A rota publica `POST /api/auth/register` nao existe mais. A interface de login nao oferece `Criar conta`. Professores e alunos nao podem criar usuarios. O primeiro administrador deve ser provisionado de forma controlada diretamente no ambiente administrativo/banco de dados, nunca por endpoint publico.
