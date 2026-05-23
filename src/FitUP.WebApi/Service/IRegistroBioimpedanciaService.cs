using FitUP.WebApi.DTOs;

namespace FitUP.WebApi.Service;

public interface IRegistroBioimpedanciaService
{
    Task<RegistroBioimpedanciaDto?> ObterPorIdAsync(Guid id);
    Task<List<RegistroBioimpedanciaDto>> ListarPorUsuarioAsync(Guid usuarioId);
    Task<RegistroBioimpedanciaDto?> CriarAsync(Guid usuarioId, RegistroBioimpedanciaRequest request);
    Task<bool> AtualizarAsync(Guid id, RegistroBioimpedanciaRequest request);
    Task<bool> RemoverAsync(Guid id);
}
