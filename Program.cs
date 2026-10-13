using EFCoreMigrationsDemo.Data;

Console.WriteLine("Demonstração de EF Core Migrations.");
Console.WriteLine("Consulta o README.md para gerar e aplicar migrações.");
using var db = new DemoDbContextFactory().CreateDbContext(args);
Console.WriteLine($"Contexto configurado: {db.Database.ProviderName}");
