namespace FitUP.WebApi.DTOs;

public class EsqueciSenhaResponse
{
    public string Token { get; set; } = string.Empty;
    public string LinkRedefinicao { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
}