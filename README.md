# Entity Framework Core Migrations — Demonstração prática

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4)
![C%23](https://img.shields.io/badge/C%23-ASP.NET%20ecosystem-239120)
![EF Core](https://img.shields.io/badge/EF%20Core-8.0.11-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927)

## Visão geral

Este repositório apresenta um **exemplo técnico independente** de utilização de *Migrations* do Entity Framework Core (EF Core) com C# e SQL Server.

O objetivo é demonstrar como gerir a evolução do esquema de uma base de dados a partir de um modelo de entidades, mantendo um histórico das alterações efetuadas.

> **Âmbito e privacidade:** exemplo didático com entidades fictícias. Não contém código, dados, regras de negócio ou o esquema da aplicação privada *Consultório Privado*. Apesar de ilustrar conceitos relevantes para ASP.NET Core, o projeto de demonstração é uma **aplicação de consola .NET 8**, e não uma aplicação ASP.NET Core MVC.

## Tecnologias

- C# e .NET 8
- Entity Framework Core 8.0.11
- Provider `Microsoft.EntityFrameworkCore.SqlServer`
- Microsoft SQL Server
- Visual Studio e Consola do Gestor de Pacotes NuGet

## Organização do repositório

```text
EFCore-Migrations-Demo/
├── Data/
│   ├── DemoDbContext.cs
│   └── DemoDbContextFactory.cs
├── Models/
│   └── Cliente.cs
├── Migrations/
│   ├── <timestamp>_InitialCreate.cs
│   ├── <timestamp>_InitialCreate.Designer.cs
│   ├── <timestamp>_AddTelefoneCliente.cs
│   ├── <timestamp>_AddTelefoneCliente.Designer.cs
│   └── DemoDbContextModelSnapshot.cs
├── Scripts/
│   └── migrations.sql
├── EFCoreMigrationsDemo.csproj
├── Program.cs
└── README.md
```

A pasta `Migrations/` é gerada automaticamente pelo EF Core. Os nomes incluem timestamps e podem variar entre execuções.

## Modelo de dados

A entidade fictícia `Cliente` contém:

| Propriedade | Tipo C# | SQL Server | Observações |
|---|---|---|---|
| `Id` | `int` | `int IDENTITY` | Chave primária |
| `Nome` | `string` | `nvarchar(150)` | Obrigatório |
| `Email` | `string?` | `nvarchar(200)` | Opcional |
| `Telefone` | `string?` | `nvarchar(20)` | Opcional, adicionado na segunda migração |

O `DemoDbContext` configura o mapeamento e as restrições das propriedades.

## Processo demonstrado

### 1. Migração inicial — `InitialCreate`

Depois de definir a entidade inicial com `Id`, `Nome` e `Email`, foi executado na Consola do Gestor de Pacotes NuGet:

```powershell
Add-Migration InitialCreate
Update-Database
```

A migração criou a tabela `Clientes` e foi registada na tabela `__EFMigrationsHistory`.

### 2. Evolução do modelo — `AddTelefoneCliente`

Foi acrescentada à entidade `Cliente` a propriedade:

```csharp
public string? Telefone { get; set; }
```

E configurado o comprimento máximo no `DemoDbContext`:

```csharp
entity.Property(x => x.Telefone).HasMaxLength(20);
```

Foram depois executados:

```powershell
Add-Migration AddTelefoneCliente
Update-Database
```

A segunda migração adicionou a coluna `Telefone` à tabela existente, sem recriar a tabela.

### 3. Geração de script SQL idempotente

```powershell
Script-Migration -Idempotent -Output Scripts/migrations.sql
```

O script gerado contém verificações ao histórico de migrações e as operações principais:

```sql
CREATE TABLE [Clientes] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(150) NOT NULL,
    [Email] nvarchar(200) NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
);
```

```sql
ALTER TABLE [Clientes] ADD [Telefone] nvarchar(20) NULL;
```

O script inclui ainda a criação, se necessária, da tabela `__EFMigrationsHistory`, o registo de cada migração e blocos transacionais. As operações são condicionadas à ausência dos respetivos identificadores no histórico.

> **Nota:** a idempotência depende da consistência do histórico de migrações com o esquema real. Antes de executar scripts noutros ambientes, rever as alterações, confirmar a base de dados de destino e garantir cópias de segurança adequadas.

## Migrações validadas

As seguintes migrações foram confirmadas na base de dados de demonstração:

| MigrationId | EF Core |
|---|---|
| `20261013143648_InitialCreate` | `8.0.11` |
| `20261013144027_AddTelefoneCliente` | `8.0.11` |

A tabela `Clientes` foi validada com as colunas `Id`, `Nome`, `Email` e `Telefone`.

## Como executar localmente

### Pré-requisitos

- SDK .NET 8
- SQL Server de desenvolvimento
- Visual Studio com suporte para C# e NuGet

### Passos

1. Clonar o repositório e abrir `EFCoreMigrationsDemo.csproj`.
2. Confirmar a ligação à instância SQL Server de testes. Por predefinição, a fábrica de contexto usa:

   `Server=localhost;Database=EFCoreMigrationsDemo;Trusted_Connection=True;TrustServerCertificate=True`

   Se necessário, definir a variável de ambiente `EFCORE_DEMO_CONNECTION`. Não guardar credenciais no repositório.
3. Restaurar os pacotes NuGet.
4. Na Consola do Gestor de Pacotes NuGet, com `EFCoreMigrationsDemo` selecionado como projeto predefinido, executar:

```powershell
Update-Database
```

As migrações já incluídas no repositório serão aplicadas à base de dados de demonstração.

**Alternativa com .NET CLI:**

```powershell
dotnet restore
dotnet tool install --global dotnet-ef --version 8.0.11
dotnet ef database update
```

Se `dotnet-ef` já estiver instalado, usar `dotnet tool update --global dotnet-ef --version 8.0.11`.

> Não é necessário executar `Add-Migration` ao clonar um repositório que já contém as migrações. Esse comando é utilizado quando o modelo é alterado e se pretende criar uma nova migração.

## Consultas de validação

```sql
USE EFCoreMigrationsDemo;
GO

SELECT MigrationId, ProductVersion
FROM dbo.__EFMigrationsHistory
ORDER BY MigrationId;
GO

SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME = 'Clientes'
ORDER BY ORDINAL_POSITION;
GO
```

## Considerações técnicas

- As migrações permitem versionar a evolução do esquema juntamente com o código.
- A tabela `__EFMigrationsHistory` regista quais as migrações aplicadas.
- O método `Up()` descreve a evolução; o método `Down()` descreve a reversão prevista.
- Alterações potencialmente destrutivas exigem revisão manual e uma estratégia de recuperação.
- **Bases de dados preexistentes:** não aplicar diretamente uma migração `InitialCreate` que tente criar tabelas já existentes. A adoção de migrações nesses cenários requer preparação específica de uma baseline.

## Contexto

Este exemplo foi preparado para acompanhar um artigo técnico no LinkedIn sobre **Entity Framework Core Migrations: gestão da evolução de bases de dados em C# e SQL Server**.

O código disponibilizado é exclusivamente demonstrativo e não corresponde à aplicação privada que motivou a exploração do tema.

## Direitos e utilização

Este repositório não inclui uma licença de software de código aberto. A ausência de licença não impede tecnicamente a cópia ou visualização do código, mas não concede automaticamente permissões de reutilização, modificação ou distribuição.
