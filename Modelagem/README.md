# Modelagem UML

Arquivos da modelagem do sistema, feita no **Astah** (versão 11).

## O que tem aqui

| Arquivo | O que é |
|---|---|
| `Projeto_UML.asta` | O projeto completo do Astah — abra com o Astah para editar |

## O que foi modelado

O sistema é uma plataforma de cursos online, com dois perfis: **Aluno** e **Admin**.

São **14 casos de uso**, cada um com três artefatos: o diagrama de comunicação (DC), a narrativa (NG) e o diagrama de sequência (DS).

| Grupo | Casos de uso |
|---|---|
| Acesso | DC01 Login · DC02 Recuperar Senha · DC06 Logout |
| Aluno | DC03 Visualizar Cursos · DC04 Buscar Cursos · DC05 Marcar Aula como Concluída · DC08 Visualizar Curso |
| Cursos (Admin) | DC07 Criar · DC09 Editar · DC10 Excluir |
| Usuários (Admin) | DC11 Criar · DC12 Visualizar · DC13 Editar · DC14 Excluir |

O arquivo também traz o diagrama de classes e o diagrama de casos de uso.

A tabela que liga cada caso de uso à tela e ao método correspondente no sistema está em [../CSharp/README.md](../CSharp/README.md).

## Exportar os diagramas como imagem

Para quem não tem o Astah instalado conseguir ver, e para usar nos slides:

No Astah: **File → Export Image** → escolha PNG → salve nesta mesma pasta.

Vale exportar pelo menos o diagrama de casos de uso e o de classes, que são os dois que costumam ir para a apresentação.

## Antes de editar

O `.asta` é um arquivo binário. O Git **não consegue** juntar duas versões dele: se duas pessoas editarem ao mesmo tempo, uma das versões se perde.

Combine no grupo antes de abrir o arquivo para mexer.
