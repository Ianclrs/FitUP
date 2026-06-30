namespace FitUP.WebApi.DTOs;

public class UsuarioUpdateRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Sobrenome { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public string? CPF { get; set; }
    public DateTime? DataNascimento { get; set; }
}
