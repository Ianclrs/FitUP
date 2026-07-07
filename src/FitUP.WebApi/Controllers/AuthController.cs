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

    /// <summary>
    ///     Solicita redefinição de senha. Retorna o link com token (modo dev).
    /// </summary>
    [HttpPost("esqueci-senha")]
    [AllowAnonymous]
    public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaRequest request)
    {
        try
        {
            var response = await _authService.SolicitarRedefinicaoSenhaAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    ///     Redefine a senha usando o token enviado.
    /// </summary>
    [HttpPost("redefinir-senha")]
    [AllowAnonymous]
    public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaRequest request)
    {
        var resultado = await _authService.RedefinirSenhaAsync(request);

        if (!resultado)
            return BadRequest(new { mensagem = "Token inválido ou expirado." });

        return Ok(new { mensagem = "Senha redefinida com sucesso." });
    }
}