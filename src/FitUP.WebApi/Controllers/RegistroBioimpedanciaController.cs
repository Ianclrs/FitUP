using System.Security.Claims;
using FitUP.WebApi.DTOs;
using FitUP.WebApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitUP.WebApi.Controllers;

[ApiController]
[Route("api/bioimpedancia")]
[Authorize]
public class RegistroBioimpedanciaController : ControllerBase
{
    private readonly IRegistroBioimpedanciaService _bioimpedanciaService;

    public RegistroBioimpedanciaController(IRegistroBioimpedanciaService bioimpedanciaService)
    {
        _bioimpedanciaService = bioimpedanciaService;
    }

    private Guid UsuarioIdLogado =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>
    ///     Lista todos os registros de bioimpedância do usuário logado (com paginação).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
    {
        var registros = await _bioimpedanciaService.ListarPorUsuarioAsync(UsuarioIdLogado);
        var total = registros.Count();
        var items = registros
            .OrderByDescending(r => r.DataRegistro)
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
    ///     Obtém um registro de bioimpedância pelo ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id)
    {
        var registro = await _bioimpedanciaService.ObterPorIdAsync(id);

        if (registro is null)
            return NotFound(new { mensagem = "Registro de bioimpedância não encontrado." });

        return Ok(registro);
    }

    /// <summary>
    ///     Cria um novo registro de bioimpedância.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] RegistroBioimpedanciaRequest request)
    {
        var registro = await _bioimpedanciaService.CriarAsync(UsuarioIdLogado, request);
        return CreatedAtAction(nameof(ObterPorId), new { id = registro!.Id }, registro);
    }

    /// <summary>
    ///     Atualiza um registro de bioimpedância existente.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] RegistroBioimpedanciaRequest request)
    {
        var resultado = await _bioimpedanciaService.AtualizarAsync(id, request);

        if (!resultado)
            return NotFound(new { mensagem = "Registro de bioimpedância não encontrado." });

        return NoContent();
    }

    /// <summary>
    ///     Remove um registro de bioimpedância.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        var resultado = await _bioimpedanciaService.RemoverAsync(id);

        if (!resultado)
            return NotFound(new { mensagem = "Registro de bioimpedância não encontrado." });

        return NoContent();
    }
}
