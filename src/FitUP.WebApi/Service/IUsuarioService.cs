using FitUP.WebApi.DTOs;

namespace FitUP.WebApi.Service;

public interface IUsuarioService
{
    Task<UsuarioDto?> ObterPorIdAsync(Guid id);
    Task<bool> AtualizarAsync(Guid id, UsuarioUpdateRequest request);
    Task<bool> AlterarSenhaAsync(Guid id, AlterarSenhaRequest request);
    Task<bool> DesativarAsync(Guid id);
}
