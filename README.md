# Plataforma de Cursos Mobile — PIM IV

Aplicativo mobile educacional desenvolvido como parte do **Projeto Integrado Multidisciplinar (PIM IV)**. A plataforma permite gerenciar cursos e acompanhar a aprendizagem dos alunos, com acesso às funcionalidades de acordo com o perfil de cada usuário.

## Tecnologias

- **Aplicativo:** React Native e Expo (JavaScript).
- **API:** Node.js e Express.
- **Banco de dados:** Microsoft SQL Server, com suporte à conexão ao Azure SQL.
- **Autenticação:** JWT e armazenamento de senhas com `scrypt` para novas contas.

## Funcionalidades

| Perfil | Funcionalidades principais |
|---|---|
| **ADMIN** | Cadastrar e gerenciar usuários (alunos, professores e administradores) e gerenciar cursos. |
| **PROFESSOR** | Gerenciar os próprios cursos e aulas e acompanhar matrículas. |
| **ALUNO** | Consultar o catálogo, acessar cursos, acompanhar aulas concluídas e visualizar perfil, XP e conquistas. |

**O cadastro de usuários é exclusivo do administrador.** Não existe cadastro público pelo aplicativo.

### Progresso e gamificação

A conclusão de aulas é registrada no banco de dados. O sistema concede **10 XP por nova aula concluída**, além de possíveis bônus por conquistas. O histórico evita a concessão duplicada de XP pela mesma conclusão. Ao desmarcar uma aula, o progresso atual é atualizado, mas o XP já concedido não é estornado.

## Arquitetura

```text
Aplicativo React Native (Expo)
             |
             | Requisições HTTP / JWT
             v
      API Node.js / Express
             |
             v
    SQL Server / Azure SQL
```

O aplicativo acessa os dados por meio da API; as credenciais do SQL Server permanecem no backend. Essa arquitetura permite que outras interfaces, como sistemas web e desktop, reutilizem os mesmos serviços.

## Estrutura do projeto

```text
api/       Backend, autenticação e comunicação com o banco
mobile/    Aplicativo React Native / Expo
sql/       Scripts de criação, evolução e diagnóstico do banco
```

## Como executar

**Pré-requisitos:** Node.js, ambiente Expo (Expo Go ou emulador Android) e acesso a um SQL Server configurado.

### 1. Configurar e iniciar a API

Na pasta `api`, copie `.env.example` para `.env` e configure as variáveis de conexão (`DB_SERVER`, `DB_DATABASE`, `DB_USER`, `DB_PASSWORD`) e `JWT_SECRET`, conforme o exemplo do projeto.

```bash
cd api
npm install
npm run dev
```

Verifique a disponibilidade em `http://localhost:3000/api/health`.

### 2. Configurar e iniciar o aplicativo

Na pasta `mobile`, defina `EXPO_PUBLIC_API_URL` no arquivo `.env`:

- **Emulador Android:** `http://10.0.2.2:3000/api`
- **Celular físico:** `http://IP_DO_COMPUTADOR:3000/api` (mesma rede)

```bash
cd mobile
npm install
npx expo start -c
```

Abra o aplicativo pelo Expo Go ou emulador. Para realizar o login, é necessário que exista uma conta ativa no banco. O primeiro administrador deve ser provisionado de forma controlada, sem endpoint público de cadastro.

## Banco de dados

Os scripts da pasta `sql/` documentam a estrutura e as evoluções do banco:

| Arquivo | Finalidade |
|---|---|
| `CursosDB.sql` | Estrutura inicial de usuários, cursos, aulas e progresso. |
| `Etapa1_Professor.sql` | Associação de professores aos cursos e matrículas. |
| `Etapa4_Gamificacao.sql` | Tabelas de XP, conquistas e histórico de pontuação. |
| `DiagnosticoBanco.sql` | Consultas para verificar a estrutura existente. |
| `Desativar_Usuarios_Demo.sql` | Script auxiliar para desativação de contas de demonstração; não faz parte da instalação padrão. |

**Atenção:** antes de executar scripts, confirme o banco selecionado e revise o conteúdo. O script inicial pode criar banco e inserir dados de exemplo; não deve ser reaplicado automaticamente em um banco já existente. Os arquivos usam nomes de banco que podem diferir da configuração atual do Azure SQL.

## Validação funcional

Para demonstrar o funcionamento do projeto, recomenda-se verificar:

1. Login de usuários com os três perfis e restrições de acesso correspondentes.
2. Cadastro de aluno e professor realizado por um administrador.
3. Gerenciamento de cursos e aulas pelo professor responsável.
4. Matrícula e conclusão de aula por um aluno.
5. Atualização do progresso e do XP, sem duplicação ao concluir a mesma aula novamente.
6. Persistência das informações no SQL Server.

## Observações

- A recuperação automática de senha por e-mail ainda não está implementada.
- A integração com Azure SQL e dispositivos Android depende da configuração do ambiente de execução.
- O arquivo `.env`, senhas, tokens e outras credenciais não devem ser publicados no repositório.
- Para uma implantação pública, é necessário configurar HTTPS e revisar as permissões de acesso ao banco.

---

**Projeto acadêmico — PIM IV | Módulo de desenvolvimento mobile**
