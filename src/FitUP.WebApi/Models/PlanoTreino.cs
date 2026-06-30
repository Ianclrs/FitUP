namespace FitUP.WebApi.Models;

public class PlanoTreino
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Divisao { get; set; }
    public int Nivel { get; set; }
    public int FrequenciaSemanal { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
}
