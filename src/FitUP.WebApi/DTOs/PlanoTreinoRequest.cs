namespace FitUP.WebApi.DTOs;

public class PlanoTreinoRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public int Divisao { get; set; }
    public int Nivel { get; set; }
    public int FrequenciaSemanal { get; set; }
}

public class DiaTreinoRequest
{
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
}

public class ExercicioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int GrupoMuscular { get; set; }
    public int Series { get; set; }
    public int Repeticoes { get; set; }
    public double? Carga { get; set; }
    public string? Observacoes { get; set; }
    public int Ordem { get; set; }
}
