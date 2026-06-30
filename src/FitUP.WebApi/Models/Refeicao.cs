namespace FitUP.WebApi.Models;

public class Refeicao
{
    public Guid Id { get; set; }
    public Guid PlanoAlimentarId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TimeSpan? HorarioSugerido { get; set; }
    public int Ordem { get; set; }
    public double TotalProteina { get; set; }
    public double TotalCarboidrato { get; set; }
    public double TotalGordura { get; set; }
    public double TotalFibra { get; set; }
    public double TotalCalorias { get; set; }
}
