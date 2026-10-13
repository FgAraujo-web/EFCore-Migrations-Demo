# Scripts SQL — Entity Framework Core Migrations

Esta pasta contém o script SQL gerado a partir das migrações do Entity Framework Core.

## migrations.sql

Script SQL idempotente, gerado através do comando:

```powershell
Script-Migration -Idempotent -Output Scripts/migrations.sql
```

O script contempla duas migrações:

- **InitialCreate:** criação da tabela `Clientes`, com as colunas `Id`, `Nome` e `Email`.
- **AddTelefoneCliente:** alteração da tabela `Clientes`, acrescentando a coluna `Telefone` do tipo `nvarchar(20)`.

O script utiliza a tabela `__EFMigrationsHistory` para verificar quais as migrações já aplicadas, evitando a sua repetição.

## Validação

As duas migrações foram aplicadas e validadas numa base de dados SQL Server de demonstração.

**Nota:** antes de executar o script noutro ambiente, deve ser verificada a estrutura da base de dados, o histórico de migrações e o impacto das alterações.
