using FitUP.WebApi.DTOs;

namespace FitUP.WebApi.Service;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegistrarAsync(RegistroRequest request);
    Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request);
}
