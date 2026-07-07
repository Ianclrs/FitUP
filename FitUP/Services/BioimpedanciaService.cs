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

    public async Task<List<RegistroBioimpedanciaDto>> ListarAsync()
    {
        var response = await _httpClient.GetAsync("api/bioimpedancia");
        if (!response.IsSuccessStatusCode)
            return new List<RegistroBioimpedanciaDto>();

        return await response.Content.ReadFromJsonAsync<List<RegistroBioimpedanciaDto>>() ?? new();
    }

    public async Task<RegistroBioimpedanciaDto?> CriarAsync(RegistroBioimpedanciaRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/bioimpedancia", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<RegistroBioimpedanciaDto>();
    }

    public async Task<bool> RemoverAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/bioimpedancia/{id}");
        return response.IsSuccessStatusCode;
    }
}