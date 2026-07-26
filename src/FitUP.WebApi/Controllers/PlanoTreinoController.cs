using System.Security.Claims;
using FitUP.WebApi.DTOs;
using FitUP.WebApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitUP.WebApi.Controllers;

[ApiController]
[Route("api/planos-treino")]
[Authorize]
public class PlanoTreinoController : ControllerBase
{
    private readonly IPlanoTreinoService _planoTreinoService;

    public PlanoTreinoController(IPlanoTreinoService planoTreinoService)
    {
        _planoTreinoService = planoTreinoService;
    }

    private Guid UsuarioIdLogado =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    ///     Lista todos os planos de treino do usuário logado (com paginação).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var planos = await _planoTreinoService.ListarPorUsuarioAsync(UsuarioIdLogado);
        var total = planos.Count();
        var items = planos
            .OrderByDescending(p => p.CriadoEm)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new
        {
            items,
            totalCount = total,
            page,
            pageSize
        });
    }

    /// <summary>
    ///     Obtém um plano de treino pelo ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var plano = await _planoTreinoService.ObterPorIdAsync(id);

        if (plano is null)
            return NotFound(new { mensagem = "Plano de treino não encontrado." });

        return Ok(plano);
    }

    /// <summary>
    ///     Cria um novo plano de treino.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] PlanoTreinoRequest request)
    {
        var plano = await _planoTreinoService.CriarAsync(UsuarioIdLogado, request);
        return CreatedAtAction(nameof(ObterPorId), new { id = plano!.Id }, plano);
    }

    /// <summary>
    ///     Atualiza um plano de treino existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] PlanoTreinoRequest request)
    {
        var resultado = await _planoTreinoService.AtualizarAsync(id, request);

        if (!resultado)
            return NotFound(new { mensagem = "Plano de treino não encontrado." });

        return NoContent();
    }

    /// <summary>
    ///     Remove um plano de treino (em cascata: dias e exercícios).
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        var resultado = await _planoTreinoService.RemoverAsync(id);

        if (!resultado)
            return NotFound(new { mensagem = "Plano de treino não encontrado." });

        return NoContent();
    }
}
