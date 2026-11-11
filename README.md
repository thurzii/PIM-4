# Plataforma de Cursos Online - Versão Mobile

Versão mobile do projeto original, desenvolvida em JavaScript.

## Arquitetura

- Mobile: React Native + Expo + React Navigation
- API: Node.js + Express
- Banco: SQL Server (configurado para Azure SQL; também pode usar SQL Server local)
- Comunicação: HTTP + JSON
- Autenticação: JWT
- Persistência local: AsyncStorage

Fluxo:

`Aplicativo Mobile -> REST API -> SQL Server`

## Funcionalidades

### Aluno
- Login
- Recuperação de senha demonstrativa
- Catálogo e busca de cursos
- Detalhes do curso e lista de aulas
- Marcar/desmarcar aula concluída
- Percentual de progresso sincronizado com o banco
- Perfil e logout

### Administrador
- Login por perfil
- Cadastro, edição e exclusão de cursos
- Cadastro e exclusão de aulas
- Ativar/desativar cursos
- Listagem e busca de usuários
- Ativação/desativação de usuários

## Usuários de demonstração

Os usuários abaixo só funcionarão se já estiverem cadastrados no banco conectado à API:

- ADMIN: `admin@escola.com` / `admin123`
- ALUNO: `maria@escola.com` / `aluno123`
- ALUNO: `joao@escola.com` / `aluno123`

## 1. Configurar o banco de dados na nuvem (Azure SQL)

O projeto está preparado com estes valores iniciais:

- Servidor: `database-pim-iv.database.windows.net`
- Porta: `1433`
- Banco provisório: `banco_pim`
- Usuário: `maria`

**Importante:** `banco_pim` é apenas um nome provisório. Quando você confirmar o nome real do banco no portal Azure, altere `DB_DATABASE` no arquivo `api/.env`. O banco precisa existir no servidor e conter as tabelas esperadas pela API: `Usuarios`, `Cursos`, `Aulas` e `Progresso`.

No portal Azure, confira também se o firewall do servidor SQL permite conexões a partir do IP público da máquina onde a API será executada. Não libere acesso irrestrito (`0.0.0.0` a `255.255.255.255`) apenas para testar.

## 2. Configurar e iniciar a API

Abra um terminal na pasta `api` e instale as dependências:

```bash
npm install
```

Copie `api/.env.example` para `api/.env`. No Windows PowerShell, estando na pasta `api`, você pode usar:

```powershell
Copy-Item .env.example .env
```

Abra `api/.env` e confira os valores. Substitua `DB_PASSWORD` pela senha do usuário SQL. Não envie o arquivo `.env` ao GitHub nem compartilhe a senha.

```env
PORT=3000
JWT_SECRET=troque-por-uma-chave-longa-e-aleatoria
DB_SERVER=database-pim-iv.database.windows.net
DB_PORT=1433
DB_DATABASE=banco_pim
DB_USER=maria
DB_PASSWORD=COLOQUE_SUA_SENHA_AQUI
DB_ENCRYPT=true
DB_TRUST_SERVER_CERTIFICATE=false
DB_INSTANCE=
```

Depois inicie a API:

```bash
npm run dev
```

Para verificar a conexão, abra `http://localhost:3000/api/health` no navegador. Se a API responder com `database: "ok"`, a conexão com o banco foi estabelecida. Se houver erro, confira o nome real do banco, as credenciais e a regra de firewall do Azure.

### Usar SQL Server local (alternativa)

Se precisar voltar para um SQL Server local, ajuste o `api/.env`, por exemplo:

```env
DB_SERVER=localhost
DB_DATABASE=CursosDB
DB_USER=
DB_PASSWORD=
DB_ENCRYPT=false
DB_TRUST_SERVER_CERTIFICATE=true
DB_INSTANCE=MSSQLLocalDB
```

A configuração local depende da instalação e do método de autenticação disponível na máquina.

## 3. Iniciar o aplicativo mobile

Entre na pasta `mobile` e instale as dependências:

```bash
npm install
npx expo start
```

Confira `mobile/src/services/api.js` e configure o endereço da API conforme o dispositivo:

- Emulador Android: `http://10.0.2.2:3000/api`
- Celular físico: `http://IP-DA-MAQUINA:3000/api`, com o celular e o computador na mesma rede.

O aplicativo conversa com a API; as credenciais do banco ficam exclusivamente no backend.

## 4. Banco e estrutura esperada

O arquivo `sql/CursosDB.sql` contém o script de criação do banco usado originalmente pelo projeto. Use-o apenas se precisar criar a estrutura em um banco novo e tiver autorização para executar esse script. Antes de rodá-lo no Azure, confirme o banco de destino para evitar criar objetos no lugar errado.

## 5. Apresentação da etapa mobile

Fluxo sugerido:

`Login -> Catálogo -> Curso -> Aulas -> Concluir aula -> Progresso`

Demonstre também:

`Login Admin -> Administração -> Cursos/Usuários -> API -> Banco`

## Observação de segurança

O sistema original armazena senhas de usuários em texto simples para fins de demonstração. Em um sistema real, use hash seguro de senha, HTTPS, uma chave JWT forte e fluxo de recuperação de senha com verificação. O usuário SQL deve ter apenas as permissões necessárias para a aplicação.
