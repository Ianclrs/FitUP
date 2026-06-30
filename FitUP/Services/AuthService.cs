using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

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
/// DTO leve para persistir no localStorage
/// </summary>
internal class StoredUserState
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("usuarioId")]
    public string UsuarioId { get; set; } = string.Empty;

    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Serviço de autenticação que consome a API FitUP
/// Mantém estado de login (em memória + localStorage).
/// </summary>
public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    private const string StorageKey = "fitup_user";

    public bool IsLoggedIn => !string.IsNullOrWhiteSpace(NomeUsuario) && !string.IsNullOrWhiteSpace(Token);
    public string NomeUsuario { get; private set; } = string.Empty;
    public string EmailUsuario { get; private set; } = string.Empty;
    public string UsuarioId { get; private set; } = string.Empty;
    public string Token { get; private set; } = string.Empty;

    /// <summary>
    /// Evento disparado quando o estado de autenticação muda.
    /// </summary>
    public event Action? AuthStateChanged;

    public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Tenta restaurar a sessão salva no localStorage ao iniciar o app.
    /// </summary>
    public async Task RestoreSessionAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }
        catch
        {
            // Ignora erros de JS (ex: prerender)
        }
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

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (authResponse is not null)
        {
            // Atualiza estado em memória
            NomeUsuario = authResponse.Nome;
            EmailUsuario = authResponse.Email;
            UsuarioId = authResponse.UsuarioId;
            Token = authResponse.Token;

            // Configura o header Authorization
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

            // Persiste no localStorage
            await PersistSessionAsync();
            NotifyAuthStateChanged();
        }

        return authResponse;
    }

    /// <summary>
    /// Realiza logout, limpando estado e storage.
    /// </summary>
    public async Task LogoutAsync()
    {
        NomeUsuario = string.Empty;
        EmailUsuario = string.Empty;
        UsuarioId = string.Empty;
        Token = string.Empty;

        _httpClient.DefaultRequestHeaders.Authorization = null;

        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        }
        catch { /* Ignora erros */ }

        NotifyAuthStateChanged();
    }

    private async Task PersistSessionAsync()
    {
        try
        {
            var state = new StoredUserState
            {
                Nome = NomeUsuario,
                Email = EmailUsuario,
                UsuarioId = UsuarioId,
                Token = Token
            };
            var json = JsonSerializer.Serialize(state);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, json);
        }
        catch { /* Ignora erros */ }
    }

    private void NotifyAuthStateChanged()
    {
        AuthStateChanged?.Invoke();
    }
}
