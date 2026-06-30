namespace FitUP.WebApi.Models;

public class Usuario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? CPF { get; set; }
    public DateTime? DataNascimento { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? UltimoLoginEm { get; set; }
    public bool Ativo { get; set; } = true;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiraEm { get; set; }
}
