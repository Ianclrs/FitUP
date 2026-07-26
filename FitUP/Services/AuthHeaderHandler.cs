using System.Net.Http.Headers;

namespace FitUP.Services;

/// <summary>
/// DelegatingHandler que injeta automaticamente o token JWT
/// em todas as requisições feitas pelo HttpClient nomeado "Api".
/// Usa ITokenProvider (singleton) como fonte única de verdade do token.
/// </summary>
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;

    public AuthHeaderHandler(ITokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_tokenProvider.Token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenProvider.Token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
