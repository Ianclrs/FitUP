using FitUP.WebApi.DTOs;
using FitUP.WebApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitUP.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    ///     Realiza o login do usuário.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response is null)
            return Unauthorized(new { mensagem = "Email ou senha inválidos." });

        return Ok(response);
    }

    /// <summary>
    ///     Registra um novo usuário.
    /// </summary>
    [HttpPost("registrar")]
    [AllowAnonymous]
    public async Task<IActionResult> Registrar([FromBody] RegistroRequest request)
    {
        var response = await _authService.RegistrarAsync(request);

        if (response is null)
            return Conflict(new { mensagem = "Este email já está cadastrado." });

        return Created(string.Empty, response);
    }

    /// <summary>
    ///     Renova o token JWT usando o refresh token.
    /// </summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request);

        if (response is null)
            return Unauthorized(new { mensagem = "Refresh token inválido ou expirado." });

        return Ok(response);
    }
}
