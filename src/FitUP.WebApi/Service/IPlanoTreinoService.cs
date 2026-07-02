using FitUP.WebApi.DTOs;

namespace FitUP.WebApi.Service;

public interface IPlanoTreinoService
{
    Task<PlanoTreinoDto?> ObterPorIdAsync(Guid id);
    Task<List<PlanoTreinoDto>> ListarPorUsuarioAsync(Guid usuarioId);
    Task<PlanoTreinoDto?> CriarAsync(Guid usuarioId, PlanoTreinoRequest request);
    Task<bool> AtualizarAsync(Guid id, PlanoTreinoRequest request);
    Task<bool> RemoverAsync(Guid id);
}
