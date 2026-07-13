namespace FitUP.WebApi.DTOs;

public class PlanoAlimentarRequest
{
    public string Nome { get; set; } = string.Empty;
    public int Objetivo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public List<RefeicaoCompletaRequest> Refeicoes { get; set; } = new();
}

public class RefeicaoRequest
{
    public string Nome { get; set; } = string.Empty;
    public TimeSpan? HorarioSugerido { get; set; }
    public int Ordem { get; set; }
}

public class RefeicaoCompletaRequest
{
    public string Nome { get; set; } = string.Empty;
    public TimeSpan? HorarioSugerido { get; set; }
    public int Ordem { get; set; }
    public double TotalProteina { get; set; }
    public double TotalCarboidrato { get; set; }
    public double TotalGordura { get; set; }
    public double TotalFibra { get; set; }
    public double TotalCalorias { get; set; }
    public List<AlimentoRequest> Alimentos { get; set; } = new();
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
