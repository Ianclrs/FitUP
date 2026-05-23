namespace FitUP.WebApi.Models;

public class Exercicio
{
    public Guid Id { get; set; }
    public Guid DiaTreinoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int GrupoMuscular { get; set; }
    public int Series { get; set; }
    public int Repeticoes { get; set; }
    public double? Carga { get; set; }
    public string? Observacoes { get; set; }
    public int Ordem { get; set; }
}
