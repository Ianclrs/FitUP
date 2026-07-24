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
    public async Task<ApiResponse<List<PlanoAlimentarDto>>> ListarAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/planos-alimentares");
            if (!response.IsSuccessStatusCode)
                return ApiResponse<List<PlanoAlimentarDto>>.Fail((int)response.StatusCode);

            var data = await response.Content.ReadFromJsonAsync<List<PlanoAlimentarDto>>();
            return ApiResponse<List<PlanoAlimentarDto>>.Ok(data ?? new List<PlanoAlimentarDto>());
        }
        catch (HttpRequestException)
        {
            return ApiResponse<List<PlanoAlimentarDto>>.NetworkError();
        }
        catch (TaskCanceledException)
        {
            return ApiResponse<List<PlanoAlimentarDto>>.NetworkError("Tempo limite excedido. Verifique sua conexão.");
        }
    }

    /// <summary>
    /// Cria um novo plano alimentar completo (com refeições e alimentos).
    /// </summary>
    public async Task<ApiResponse<PlanoAlimentarDto>> CriarCompletoAsync(PlanoAlimentarCompletoRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/planos-alimentares", request);
            if (!response.IsSuccessStatusCode)
                return ApiResponse<PlanoAlimentarDto>.Fail((int)response.StatusCode);

            var data = await response.Content.ReadFromJsonAsync<PlanoAlimentarDto>();
            return data is not null
                ? ApiResponse<PlanoAlimentarDto>.Ok(data, (int)response.StatusCode)
                : ApiResponse<PlanoAlimentarDto>.Fail((int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResponse<PlanoAlimentarDto>.NetworkError();
        }
        catch (TaskCanceledException)
        {
            return ApiResponse<PlanoAlimentarDto>.NetworkError("Tempo limite excedido. Verifique sua conexão.");
        }
    }

    /// <summary>
    /// Remove um plano alimentar pelo ID.
    /// </summary>
    public async Task<ApiResponse<bool>> RemoverAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/planos-alimentares/{id}");
            if (!response.IsSuccessStatusCode)
                return ApiResponse<bool>.Fail((int)response.StatusCode);

            return ApiResponse<bool>.Ok(true);
        }
        catch (HttpRequestException)
        {
            return ApiResponse<bool>.NetworkError();
        }
        catch (TaskCanceledException)
        {
            return ApiResponse<bool>.NetworkError("Tempo limite excedido. Verifique sua conexão.");
        }
    }
}