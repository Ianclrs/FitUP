namespace FitUP.Services;

/// <summary>
/// Resposta padronizada para chamadas à API, encapsulando sucesso/erro e status HTTP.
/// </summary>
/// <typeparam name="T">Tipo do dado retornado em caso de sucesso.</typeparam>
public class ApiResponse<T>
{
    /// <summary>Indica se a requisição foi bem-sucedida (2xx).</summary>
    public bool Success { get; set; }

    /// <summary>Dado retornado em caso de sucesso.</summary>
    public T? Data { get; set; }

    /// <summary>Mensagem de erro amigável para exibir ao usuário.</summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>Código de status HTTP retornado pela API.</summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Cria uma resposta de sucesso.
    /// </summary>
    public static ApiResponse<T> Ok(T data, int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Cria uma resposta de erro a partir do status code e da resposta HTTP.
    /// </summary>
    public static ApiResponse<T> Fail(int statusCode, string? responseBody = null)
    {
        var message = statusCode switch
        {
            400 => "Dados inválidos. Verifique as informações e tente novamente.",
            401 => "Sessão expirada. Faça login novamente.",
            403 => "Você não tem permissão para acessar este recurso.",
            404 => "Recurso não encontrado.",
            409 => "Conflito. Este recurso já existe.",
            422 => "Dados inválidos. Verifique as informações e tente novamente.",
            429 => "Muitas requisições. Aguarde um momento e tente novamente.",
            >= 500 and < 600 => "Erro interno do servidor. Tente novamente mais tarde.",
            _ => "Erro inesperado. Verifique sua conexão e tente novamente."
        };

        return new ApiResponse<T>
        {
            Success = false,
            ErrorMessage = message,
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Cria uma resposta de erro para falhas de rede/exceções.
    /// </summary>
    public static ApiResponse<T> NetworkError(string message = "Erro de conexão. Verifique sua internet e tente novamente.")
    {
        return new ApiResponse<T>
        {
            Success = false,
            ErrorMessage = message,
            StatusCode = 0
        };
    }
}