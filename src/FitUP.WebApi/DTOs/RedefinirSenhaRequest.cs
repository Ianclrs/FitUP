namespace FitUP.WebApi.DTOs;

public class RedefinirSenhaRequest
{
    public string Token { get; set; } = string.Empty;
    public string NovaSenha { get; set; } = string.Empty;
}