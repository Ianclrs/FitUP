using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FitUP.Services;

public class RegistroBioimpedanciaDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("usuarioId")]
    public Guid UsuarioId { get; set; }

    [JsonPropertyName("dataRegistro")]
    public DateTime DataRegistro { get; set; }

    [JsonPropertyName("peso")]
    public double Peso { get; set; }

    [JsonPropertyName("altura")]
    public double Altura { get; set; }

    [JsonPropertyName("massaMagra")]
    public double? MassaMagra { get; set; }

    [JsonPropertyName("massaGorda")]
    public double? MassaGorda { get; set; }

    [JsonPropertyName("percentualGordura")]
    public double? PercentualGordura { get; set; }

    [JsonPropertyName("massaMuscular")]
    public double? MassaMuscular { get; set; }

    [JsonPropertyName("aguaCorporal")]
    public double? AguaCorporal { get; set; }

    [JsonPropertyName("taxaMetabolicaBasal")]
    public double? TaxaMetabolicaBasal { get; set; }

    [JsonPropertyName("idadeMetabolica")]
    public double? IdadeMetabolica { get; set; }

    [JsonPropertyName("circunferenciaCintura")]
    public double? CircunferenciaCintura { get; set; }

    [JsonPropertyName("circunferenciaQuadril")]
    public double? CircunferenciaQuadril { get; set; }

    [JsonPropertyName("relacaoCinturaQuadril")]
    public double? RelacaoCinturaQuadril { get; set; }

    [JsonPropertyName("observacoes")]
    public string? Observacoes { get; set; }
}

public class RegistroBioimpedanciaRequest
{
    [JsonPropertyName("dataRegistro")]
    public DateTime DataRegistro { get; set; }

    [JsonPropertyName("peso")]
    public double Peso { get; set; }

    [JsonPropertyName("altura")]
    public double Altura { get; set; }

    [JsonPropertyName("massaMagra")]
    public double? MassaMagra { get; set; }

    [JsonPropertyName("massaGorda")]
    public double? MassaGorda { get; set; }

    [JsonPropertyName("percentualGordura")]
    public double? PercentualGordura { get; set; }

    [JsonPropertyName("massaMuscular")]
    public double? MassaMuscular { get; set; }

    [JsonPropertyName("aguaCorporal")]
    public double? AguaCorporal { get; set; }

    [JsonPropertyName("taxaMetabolicaBasal")]
    public double? TaxaMetabolicaBasal { get; set; }

    [JsonPropertyName("idadeMetabolica")]
    public double? IdadeMetabolica { get; set; }

    [JsonPropertyName("circunferenciaCintura")]
    public double? CircunferenciaCintura { get; set; }

    [JsonPropertyName("circunferenciaQuadril")]
    public double? CircunferenciaQuadril { get; set; }

    [JsonPropertyName("relacaoCinturaQuadril")]
    public double? RelacaoCinturaQuadril { get; set; }

    [JsonPropertyName("observacoes")]
    public string? Observacoes { get; set; }
}

public class BioimpedanciaService
{
    private readonly HttpClient _httpClient;

    public BioimpedanciaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<List<RegistroBioimpedanciaDto>>> ListarAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("api/bioimpedancia");
            if (!response.IsSuccessStatusCode)
                return ApiResponse<List<RegistroBioimpedanciaDto>>.Fail((int)response.StatusCode);

            var data = await response.Content.ReadFromJsonAsync<List<RegistroBioimpedanciaDto>>();
            return ApiResponse<List<RegistroBioimpedanciaDto>>.Ok(data ?? new List<RegistroBioimpedanciaDto>());
        }
        catch (HttpRequestException)
        {
            return ApiResponse<List<RegistroBioimpedanciaDto>>.NetworkError();
        }
        catch (TaskCanceledException)
        {
            return ApiResponse<List<RegistroBioimpedanciaDto>>.NetworkError("Tempo limite excedido. Verifique sua conexão.");
        }
    }

    public async Task<ApiResponse<RegistroBioimpedanciaDto>> CriarAsync(RegistroBioimpedanciaRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/bioimpedancia", request);
            if (!response.IsSuccessStatusCode)
                return ApiResponse<RegistroBioimpedanciaDto>.Fail((int)response.StatusCode);

            var data = await response.Content.ReadFromJsonAsync<RegistroBioimpedanciaDto>();
            return data is not null
                ? ApiResponse<RegistroBioimpedanciaDto>.Ok(data, (int)response.StatusCode)
                : ApiResponse<RegistroBioimpedanciaDto>.Fail((int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResponse<RegistroBioimpedanciaDto>.NetworkError();
        }
        catch (TaskCanceledException)
        {
            return ApiResponse<RegistroBioimpedanciaDto>.NetworkError("Tempo limite excedido. Verifique sua conexão.");
        }
    }

    public async Task<ApiResponse<bool>> RemoverAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"api/bioimpedancia/{id}");
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