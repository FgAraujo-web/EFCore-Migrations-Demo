using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EFCoreMigrationsDemo.Data;

public class DemoDbContextFactory : IDesignTimeDbContextFactory<DemoDbContext>
{
    public DemoDbContext CreateDbContext(string[] args)
    {
        var connection = Environment.GetEnvironmentVariable("EFCORE_DEMO_CONNECTION")
            ?? "Server=localhost;Database=EFCoreMigrationsDemo;Trusted_Connection=True;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<DemoDbContext>()
            .UseSqlServer(connection)
            .Options;

        return new DemoDbContext(options);
    }
}
