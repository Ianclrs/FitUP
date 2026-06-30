using System.Security.Claims;
using FitUP.WebApi.DTOs;
using FitUP.WebApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitUP.WebApi.Controllers;

[ApiController]
[Route("api/planos-alimentares")]
[Authorize]
public class PlanoAlimentarController : ControllerBase
{
    private readonly IPlanoAlimentarService _planoAlimentarService;

    public PlanoAlimentarController(IPlanoAlimentarService planoAlimentarService)
    {
        _planoAlimentarService = planoAlimentarService;
    }

    private Guid UsuarioIdLogado =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    ///     Lista todos os planos alimentares do usuário logado.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var planos = await _planoAlimentarService.ListarPorUsuarioAsync(UsuarioIdLogado);
        return Ok(planos);
    }

    /// <summary>
    ///     Obtém um plano alimentar pelo ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var plano = await _planoAlimentarService.ObterPorIdAsync(id);

        if (plano is null)
            return NotFound(new { mensagem = "Plano alimentar não encontrado." });

        return Ok(plano);
    }

    /// <summary>
    ///     Cria um novo plano alimentar.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PlanoAlimentarRequest request)
    {
        var plano = await _planoAlimentarService.CriarAsync(UsuarioIdLogado, request);
        return CreatedAtAction(nameof(ObterPorId), new { id = plano!.Id }, plano);
    }

    /// <summary>
    ///     Atualiza um plano alimentar existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] PlanoAlimentarRequest request)
    {
        var resultado = await _planoAlimentarService.AtualizarAsync(id, request);

        if (!resultado)
            return NotFound(new { mensagem = "Plano alimentar não encontrado." });

        return NoContent();
    }

    /// <summary>
    ///     Remove um plano alimentar (em cascata: refeições e alimentos).
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        var resultado = await _planoAlimentarService.RemoverAsync(id);

        if (!resultado)
            return NotFound(new { mensagem = "Plano alimentar não encontrado." });

        return NoContent();
    }
}
