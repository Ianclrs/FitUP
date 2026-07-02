namespace FitUP.WebApi.Models;

public class PlanoAlimentar
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Objetivo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}
