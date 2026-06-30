namespace FitUP.WebApi.Models;

public class DiaTreino
{
    public Guid Id { get; set; }
    public Guid PlanoTreinoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
}
