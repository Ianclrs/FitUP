namespace FitUP.WebApi.DTOs;

public class PlanoAlimentarDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Objetivo { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public DateTime CriadoEm { get; set; }
    public DateTime? AtualizadoEm { get; set; }
    public List<RefeicaoDto> Refeicoes { get; set; } = new();
}

public class RefeicaoDto
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TimeSpan? HorarioSugerido { get; set; }
    public int Ordem { get; set; }
    public double TotalProteina { get; set; }
    public double TotalCarboidrato { get; set; }
    public double TotalGordura { get; set; }
    public double TotalFibra { get; set; }
    public double TotalCalorias { get; set; }
    public List<AlimentoDto> Alimentos { get; set; } = new();
}

public class AlimentoDto
{
    public Guid Id { get; set; }
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
