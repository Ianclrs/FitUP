namespace FitUP.WebApi.DTOs;

public class PlanoAlimentarRequest
{
    public string Nome { get; set; } = string.Empty;
    public int Objetivo { get; set; }
    public string Descricao { get; set; } = string.Empty;
}

public class RefeicaoRequest
{
    public string Nome { get; set; } = string.Empty;
    public TimeSpan? HorarioSugerido { get; set; }
    public int Ordem { get; set; }
}

public class AlimentoRequest
{
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
