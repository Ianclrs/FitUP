using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace FitUP.Services;

/// <summary>
/// DTO de requisição para login
/// </summary>
public class LoginRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("senha")]
    public string Senha { get; set; } = string.Empty;
}

/// <summary>
/// DTO de requisição para registro
/// </summary>
public class RegistroRequest
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("sobrenome")]
    public string Sobrenome { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("senha")]
    public string Senha { get; set; } = string.Empty;

    [JsonPropertyName("telefone")]
    public string? Telefone { get; set; }

    [JsonPropertyName("cpf")]
    public string? Cpf { get; set; }

    [JsonPropertyName("dataNascimento")]
    public string? DataNascimento { get; set; }
}

/// <summary>
/// DTO de resposta da autenticação
/// </summary>
public class AuthResponse
{
    [JsonPropertyName("usuarioId")]
    public string UsuarioId { get; set; } = string.Empty;

    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;

    [JsonPropertyName("expiraEm")]
    public DateTime ExpiraEm { get; set; }
}

/// <summary>
/// Serviço de autenticação que consome a API FitUP
/// </summary>
public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse?> RegistrarAsync(RegistroRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/registrar", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AuthResponse>();
    }
}
