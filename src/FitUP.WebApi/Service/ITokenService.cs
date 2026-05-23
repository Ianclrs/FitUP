using FitUP.WebApi.Models;

namespace FitUP.WebApi.Service;

public interface ITokenService
{
    (string token, DateTime expiraEm) GerarTokenJwt(Usuario usuario);
    string GerarRefreshToken();
}
