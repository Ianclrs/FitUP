namespace FitUP.Services;

/// <summary>
/// Fonte única de verdade para o token JWT.
/// Singleton compartilhado entre AuthService e AuthHeaderHandler,
/// evitando dependências circulares e problemas de escopo no Blazor WASM.
/// </summary>
public interface ITokenProvider
{
    string Token { get; }
    void SetToken(string token);
    void Clear();
}

public class TokenProvider : ITokenProvider
{
    public string Token { get; private set; } = string.Empty;

    public void SetToken(string token) => Token = token;

    public void Clear() => Token = string.Empty;
}