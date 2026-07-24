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
/// DTO de requisição para atualizar perfil
/// </summary>
public class AtualizarPerfilRequest
{
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;

    [JsonPropertyName("sobrenome")]
    public string Sobrenome { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("telefone")]
    public string? Telefone { get; set; }

    [JsonPropertyName("cpf")]
    public string? Cpf { get; set; }

    [JsonPropertyName("dataNascimento")]
    public DateTime? DataNascimento { get; set; }
}

/// <summary>
/// DTO de requisição para alterar senha
/// </summary>
public class AlterarSenhaRequest
{
    [JsonPropertyName("senhaAtual")]
    public string SenhaAtual { get; set; } = string.Empty;

    [JsonPropertyName("novaSenha")]
    public string NovaSenha { get; set; } = string.Empty;

    [JsonPropertyName("confirmarNovaSenha")]
    public string ConfirmarNovaSenha { get; set; } = string.Empty;
}

/// <summary>
/// DTO de requisição para esqueci minha senha
/// </summary>
public class EsqueciSenhaRequestDto
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// DTO de resposta do esqueci minha senha
/// </summary>
public class EsqueciSenhaResponseDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("linkRedefinicao")]
    public string LinkRedefinicao { get; set; } = string.Empty;

    [JsonPropertyName("expiraEm")]
    public DateTime ExpiraEm { get; set; }
}

/// <summary>
/// DTO de requisição para redefinir senha
/// </summary>
public class RedefinirSenhaRequestDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("novaSenha")]
    public string NovaSenha { get; set; } = string.Empty;
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
            var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", StorageKey);
            if (string.IsNullOrWhiteSpace(json))
                return;

            var state = JsonSerializer.Deserialize<StoredUserState>(json);
            if (state is null)
                return;

            NomeUsuario = state.Nome;
            EmailUsuario = state.Email;
            UsuarioId = state.UsuarioId;
            Token = state.Token;

            // Reconfigura o header Authorization
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);

            NotifyAuthStateChanged();
        }
        catch
        {
            // Ignora erros de JS (ex: prerender)
        }
    }

    public async Task<ApiResponse<AuthResponse>> RegistrarAsync(RegistroRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/registrar", request);
            if (!response.IsSuccessStatusCode)
                return ApiResponse<AuthResponse>.Fail((int)response.StatusCode);

            var data = await response.Content.ReadFromJsonAsync<AuthResponse>();
            return data is not null
                ? ApiResponse<AuthResponse>.Ok(data, (int)response.StatusCode)
                : ApiResponse<AuthResponse>.Fail((int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResponse<AuthResponse>.NetworkError();
        }
        catch (TaskCanceledException)
        {
            return ApiResponse<AuthResponse>.NetworkError("Tempo limite excedido. Verifique sua conexão.");
        }
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
            if (!response.IsSuccessStatusCode)
                return ApiResponse<AuthResponse>.Fail((int)response.StatusCode);

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

                return ApiResponse<AuthResponse>.Ok(authResponse, (int)response.StatusCode);
            }

            return ApiResponse<AuthResponse>.Fail((int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResponse<AuthResponse>.NetworkError();
        }
        catch (TaskCanceledException)
        {
            return ApiResponse<AuthResponse>.NetworkError("Tempo limite excedido. Verifique sua conexão.");
        }
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

    /// <summary>
    /// Atualiza nome e/ou e-mail do usuário logado.
    /// </summary>
    public async Task<ApiResponse<bool>> AtualizarPerfilAsync(AtualizarPerfilRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/usuario/me", request);
            if (!response.IsSuccessStatusCode)
                return ApiResponse<bool>.Fail((int)response.StatusCode);

            // Atualiza estado em memória
            if (!string.IsNullOrWhiteSpace(request.Nome))
                NomeUsuario = request.Nome;
            if (!string.IsNullOrWhiteSpace(request.Email))
                EmailUsuario = request.Email;

            await PersistSessionAsync();
            NotifyAuthStateChanged();
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

    /// <summary>
    /// Altera a senha do usuário logado.
    /// </summary>
    public async Task<ApiResponse<bool>> AlterarSenhaAsync(AlterarSenhaRequest request)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync("api/usuario/me/alterar-senha", request);
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

    /// <summary>
    /// Solicita redefinição de senha (esqueci minha senha).
    /// </summary>
    public async Task<ApiResponse<EsqueciSenhaResponseDto>> EsqueciSenhaAsync(EsqueciSenhaRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/esqueci-senha", request);
            if (!response.IsSuccessStatusCode)
                return ApiResponse<EsqueciSenhaResponseDto>.Fail((int)response.StatusCode);

            var data = await response.Content.ReadFromJsonAsync<EsqueciSenhaResponseDto>();
            return data is not null
                ? ApiResponse<EsqueciSenhaResponseDto>.Ok(data, (int)response.StatusCode)
                : ApiResponse<EsqueciSenhaResponseDto>.Fail((int)response.StatusCode);
        }
        catch (HttpRequestException)
        {
            return ApiResponse<EsqueciSenhaResponseDto>.NetworkError();
        }
        catch (TaskCanceledException)
        {
            return ApiResponse<EsqueciSenhaResponseDto>.NetworkError("Tempo limite excedido. Verifique sua conexão.");
        }
    }

    /// <summary>
    /// Redefine a senha usando o token recebido.
    /// </summary>
    public async Task<ApiResponse<bool>> RedefinirSenhaAsync(RedefinirSenhaRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/redefinir-senha", request);
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