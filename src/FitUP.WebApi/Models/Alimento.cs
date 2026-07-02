namespace FitUP.WebApi.Models;

public class Alimento
{
    public Guid Id { get; set; }
    public Guid RefeicaoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public double Quantidade { get; set; }
    public string UnidadeMedida { get; set; } = string.Empty;
    public double Proteina { get; set; }
    public double Carboidrato { get; set; }
    public double Gordura { get; set; }
    public double Fibra { get; set; }
    public double Calorias { get; set; }
    public string? Observacoes { get; set; }
}
