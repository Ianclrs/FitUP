using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FitUP.Services;

/// <summary>
/// DTOs que espelham o backend FitUP.WebApi.DTOs
/// </summary>
public class PlanoTreinoDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [JsonPropertyName("divisao")]
    public int Divisao { get; set; }

    [JsonPropertyName("nivel")]
    public int Nivel { get; set; }

    [JsonPropertyName("frequenciaSemanal")]
    public int FrequenciaSemanal { get; set; }

    [JsonPropertyName("usuarioId")]
    public Guid UsuarioId { get; set; }

    [JsonPropertyName("criadoEm")]
    public DateTime CriadoEm { get; set; }

    [JsonPropertyName("atualizadoEm")]
    public DateTime? AtualizadoEm { get; set; }

    [JsonPropertyName("dias")]
    public List<DiaTreinoDto> Dias { get; set; } = new();
}

public class DiaTreinoDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("ordem")]
    public int Ordem { get; set; }

    [JsonPropertyName("exercicios")]
    public List<ExercicioDto> Exercicios { get; set; } = new();
}

public class ExercicioDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("grupoMuscular")]
    public int GrupoMuscular { get; set; }

    [JsonPropertyName("series")]
    public int Series { get; set; }

    [JsonPropertyName("repeticoes")]
    public int Repeticoes { get; set; }

    [JsonPropertyName("carga")]
    public double? Carga { get; set; }

    [JsonPropertyName("observacoes")]
    public string? Observacoes { get; set; }

    [JsonPropertyName("ordem")]
    public int Ordem { get; set; }
}

public class PlanoTreinoRequest
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [JsonPropertyName("divisao")]
    public int Divisao { get; set; }

    [JsonPropertyName("nivel")]
    public int Nivel { get; set; }

    [JsonPropertyName("frequenciaSemanal")]
    public int FrequenciaSemanal { get; set; }

    [JsonPropertyName("dias")]
    public List<DiaTreinoRequest> Dias { get; set; } = new();
}

public class DiaTreinoRequest
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("ordem")]
    public int Ordem { get; set; }

    [JsonPropertyName("exercicios")]
    public List<ExercicioRequest> Exercicios { get; set; } = new();
}

public class ExercicioRequest
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("descricao")]
    public string? Descricao { get; set; }

    [JsonPropertyName("grupoMuscular")]
    public int GrupoMuscular { get; set; }

    [JsonPropertyName("series")]
    public int Series { get; set; }

    [JsonPropertyName("repeticoes")]
    public int Repeticoes { get; set; }

    [JsonPropertyName("carga")]
    public double? Carga { get; set; }

    [JsonPropertyName("observacoes")]
    public string? Observacoes { get; set; }

    [JsonPropertyName("ordem")]
    public int Ordem { get; set; }
}

/// <summary>
/// Serviço para consumir a API de planos de treino.
/// </summary>
public class PlanoTreinoService
{
    private readonly HttpClient _httpClient;

    public PlanoTreinoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Lista todos os planos de treino do usuário logado.
    /// </summary>
    public async Task<List<PlanoTreinoDto>> ListarAsync()
    {
        var response = await _httpClient.GetAsync("api/planos-treino");
        if (!response.IsSuccessStatusCode)
            return new List<PlanoTreinoDto>();

        return await response.Content.ReadFromJsonAsync<List<PlanoTreinoDto>>() ?? new();
    }

    /// <summary>
    /// Cria um novo plano de treino para o usuário logado.
    /// </summary>
    public async Task<PlanoTreinoDto?> CriarAsync(PlanoTreinoRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/planos-treino", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<PlanoTreinoDto>();
    }

    /// <summary>
    /// Remove um plano de treino pelo ID.
    /// </summary>
    public async Task<bool> RemoverAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/planos-treino/{id}");
        return response.IsSuccessStatusCode;
    }
}