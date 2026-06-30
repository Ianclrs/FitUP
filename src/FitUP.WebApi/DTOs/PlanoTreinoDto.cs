namespace FitUP.WebApi.DTOs;

public class PlanoTreinoDto
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
    public List<DiaTreinoDto> Dias { get; set; } = new();
}

public class DiaTreinoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public List<ExercicioDto> Exercicios { get; set; } = new();
}

public class ExercicioDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int GrupoMuscular { get; set; }
    public int Series { get; set; }
    public int Repeticoes { get; set; }
    public double? Carga { get; set; }
    public string? Observacoes { get; set; }
    public int Ordem { get; set; }
}
