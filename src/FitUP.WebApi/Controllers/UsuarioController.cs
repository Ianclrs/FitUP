using System.Security.Claims;
using FitUP.WebApi.DTOs;
using FitUP.WebApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitUP.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuarioController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    private Guid UsuarioIdLogado =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    ///     Obtém os dados do usuário logado.
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> ObterMeusDados()
    {
        var usuario = await _usuarioService.ObterPorIdAsync(UsuarioIdLogado);

        if (usuario is null)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        return Ok(usuario);
    }

    /// <summary>
    ///     Atualiza os dados do usuário logado.
    /// </summary>
    [HttpPut("me")]
    public async Task<IActionResult> AtualizarMeusDados([FromBody] UsuarioUpdateRequest request)
    {
        var resultado = await _usuarioService.AtualizarAsync(UsuarioIdLogado, request);

        if (!resultado)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        return NoContent();
    }

    /// <summary>
    ///     Altera a senha do usuário logado.
    /// </summary>
    [HttpPut("me/alterar-senha")]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaRequest request)
    {
        var resultado = await _usuarioService.AlterarSenhaAsync(UsuarioIdLogado, request);

        if (!resultado)
            return BadRequest(new { mensagem = "Senha atual incorreta." });

        return NoContent();
    }

    /// <summary>
    ///     Desativa (soft delete) a conta do usuário logado.
    /// </summary>
    [HttpDelete("me")]
    public async Task<IActionResult> DesativarConta()
    {
        var resultado = await _usuarioService.DesativarAsync(UsuarioIdLogado);

        if (!resultado)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        return NoContent();
    }
}
