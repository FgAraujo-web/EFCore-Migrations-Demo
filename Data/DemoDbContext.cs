using EFCoreMigrationsDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreMigrationsDemo.Data;

public class DemoDbContext(DbContextOptions<DemoDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Nome).HasMaxLength(150).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(200);
            entity.Property(x => x.Telefone).HasMaxLength(20);
        });
    }
}
