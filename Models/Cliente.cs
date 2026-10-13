namespace EFCoreMigrationsDemo.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Email { get; set; }
    // Nova propriedade
    public string? Telefone { get; set; }
}
