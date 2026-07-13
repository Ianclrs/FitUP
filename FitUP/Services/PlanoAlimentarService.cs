using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FitUP.Services;

/// <summary>
/// DTOs que espelham o backend FitUP.WebApi.DTOs para planos alimentares
/// </summary>
public class PlanoAlimentarDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("objetivo")]
    public int Objetivo { get; set; }

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [JsonPropertyName("usuarioId")]
    public Guid UsuarioId { get; set; }

    [JsonPropertyName("criadoEm")]
    public DateTime CriadoEm { get; set; }

    [JsonPropertyName("atualizadoEm")]
    public DateTime? AtualizadoEm { get; set; }

    [JsonPropertyName("refeicoes")]
    public List<RefeicaoDto> Refeicoes { get; set; } = new();
}

public class RefeicaoDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("horarioSugerido")]
    public TimeSpan? HorarioSugerido { get; set; }

    [JsonPropertyName("ordem")]
    public int Ordem { get; set; }

    [JsonPropertyName("totalProteina")]
    public double TotalProteina { get; set; }

    [JsonPropertyName("totalCarboidrato")]
    public double TotalCarboidrato { get; set; }

    [JsonPropertyName("totalGordura")]
    public double TotalGordura { get; set; }

    [JsonPropertyName("totalFibra")]
    public double TotalFibra { get; set; }

    [JsonPropertyName("totalCalorias")]
    public double TotalCalorias { get; set; }

    [JsonPropertyName("alimentos")]
    public List<AlimentoDto> Alimentos { get; set; } = new();
}

public class AlimentoDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("quantidade")]
    public double Quantidade { get; set; }

    [JsonPropertyName("unidadeMedida")]
    public string UnidadeMedida { get; set; } = string.Empty;

    [JsonPropertyName("proteina")]
    public double Proteina { get; set; }

    [JsonPropertyName("carboidrato")]
    public double Carboidrato { get; set; }

    [JsonPropertyName("gordura")]
    public double Gordura { get; set; }

    [JsonPropertyName("fibra")]
    public double Fibra { get; set; }

    [JsonPropertyName("calorias")]
    public double Calorias { get; set; }

    [JsonPropertyName("observacoes")]
    public string? Observacoes { get; set; }
}

public class PlanoAlimentarCompletoRequest
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("objetivo")]
    public int Objetivo { get; set; }

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [JsonPropertyName("refeicoes")]
    public List<RefeicaoCompletaRequest> Refeicoes { get; set; } = new();
}

public class RefeicaoCompletaRequest
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("horarioSugerido")]
    public TimeSpan? HorarioSugerido { get; set; }

    [JsonPropertyName("ordem")]
    public int Ordem { get; set; }

    [JsonPropertyName("totalProteina")]
    public double TotalProteina { get; set; }

    [JsonPropertyName("totalCarboidrato")]
    public double TotalCarboidrato { get; set; }

    [JsonPropertyName("totalGordura")]
    public double TotalGordura { get; set; }

    [JsonPropertyName("totalFibra")]
    public double TotalFibra { get; set; }

    [JsonPropertyName("totalCalorias")]
    public double TotalCalorias { get; set; }

    [JsonPropertyName("alimentos")]
    public List<AlimentoRequestDto> Alimentos { get; set; } = new();
}

public class AlimentoRequestDto
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("quantidade")]
    public double Quantidade { get; set; }

    [JsonPropertyName("unidadeMedida")]
    public string UnidadeMedida { get; set; } = string.Empty;

    [JsonPropertyName("proteina")]
    public double Proteina { get; set; }

    [JsonPropertyName("carboidrato")]
    public double Carboidrato { get; set; }

    [JsonPropertyName("gordura")]
    public double Gordura { get; set; }

    [JsonPropertyName("fibra")]
    public double Fibra { get; set; }

    [JsonPropertyName("calorias")]
    public double Calorias { get; set; }

    [JsonPropertyName("observacoes")]
    public string? Observacoes { get; set; }
}

/// <summary>
/// Serviço para consumir a API de planos alimentares.
/// </summary>
public class PlanoAlimentarService
{
    private readonly HttpClient _httpClient;

    public PlanoAlimentarService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Lista todos os planos alimentares do usuário logado.
    /// </summary>
    public async Task<List<PlanoAlimentarDto>> ListarAsync()
    {
        var response = await _httpClient.GetAsync("api/planos-alimentares");
        if (!response.IsSuccessStatusCode)
            return new List<PlanoAlimentarDto>();

        return await response.Content.ReadFromJsonAsync<List<PlanoAlimentarDto>>() ?? new();
    }

    /// <summary>
    /// Cria um novo plano alimentar completo (com refeições e alimentos).
    /// </summary>
    public async Task<PlanoAlimentarDto?> CriarCompletoAsync(PlanoAlimentarCompletoRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/planos-alimentares", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<PlanoAlimentarDto>();
    }

    /// <summary>
    /// Remove um plano alimentar pelo ID.
    /// </summary>
    public async Task<bool> RemoverAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/planos-alimentares/{id}");
        return response.IsSuccessStatusCode;
    }
}