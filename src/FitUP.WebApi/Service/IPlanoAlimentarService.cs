using FitUP.WebApi.DTOs;

namespace FitUP.WebApi.Service;

public interface IPlanoAlimentarService
{
    Task<PlanoAlimentarDto?> ObterPorIdAsync(Guid id);
    Task<List<PlanoAlimentarDto>> ListarPorUsuarioAsync(Guid usuarioId);
    Task<PlanoAlimentarDto?> CriarAsync(Guid usuarioId, PlanoAlimentarRequest request);
    Task<bool> AtualizarAsync(Guid id, PlanoAlimentarRequest request);
    Task<bool> RemoverAsync(Guid id);
}
