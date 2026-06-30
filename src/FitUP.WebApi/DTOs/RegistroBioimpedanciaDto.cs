namespace FitUP.WebApi.DTOs;

public class RegistroBioimpedanciaDto
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public DateTime DataRegistro { get; set; }
    public double Peso { get; set; }
    public double Altura { get; set; }
    public double? MassaMagra { get; set; }
    public double? MassaGorda { get; set; }
    public double? PercentualGordura { get; set; }
    public double? MassaMuscular { get; set; }
    public double? AguaCorporal { get; set; }
    public double? TaxaMetabolicaBasal { get; set; }
    public double? IdadeMetabolica { get; set; }
    public double? CircunferenciaCintura { get; set; }
    public double? CircunferenciaQuadril { get; set; }
    public double? RelacaoCinturaQuadril { get; set; }
    public string? Observacoes { get; set; }
}
