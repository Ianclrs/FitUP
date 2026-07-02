namespace FitUP.WebApi.DTOs;

public class UsuarioUpdateRequest
{
    public string? Nome { get; set; }
    public string? Sobrenome { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? CPF { get; set; }
    public DateTime? DataNascimento { get; set; }
}
