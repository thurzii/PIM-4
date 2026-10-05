# Plataforma de Cursos Mobile

Aplicativo mobile da Plataforma de Cursos Online, desenvolvido com **React Native + Expo** e uma API em **Node.js/Express**, utilizando o banco **CursosDB (SQL Server)**.

## Requisitos

- Node.js
- Android Studio + Android SDK (para emulador) ou celular com Expo Go
- SQL Server Express LocalDB com a instância `MSSQLLocalDB`
- O banco `CursosDB` deve estar criado na máquina

## Como rodar

### 1. API

Abra um terminal na pasta `api`:

```powershell
npm install
npm install msnodesqlv8
npm run dev
```

Teste no navegador:

```text
http://localhost:3000/api/health
```

Deve aparecer `"api":"ok"` e `"database":"ok"`.

### 2. Aplicativo

Abra outro terminal na pasta `mobile`:

```powershell
npm install
npx expo install --fix
npx expo-doctor
npx expo start --lan -c
```

Depois pressione `a` para abrir no emulador Android.

Para usar celular físico, abra o **Expo Go** e leia o QR Code. O computador e o celular precisam estar na mesma rede.

## Banco de dados

A API utiliza:

```text
(localdb)\MSSQLLocalDB
CursosDB
```

Não é necessário criar outro banco se o `CursosDB` já existir.

## Usuários de teste

```text
Aluno:
maria@escola.com
aluno123

Administrador:
admin@escola.com
admin123
```

## Estrutura

```text
api/     → API Node.js + Express
mobile/  → Aplicativo React Native + Expo
sql/     → Script do banco
```
