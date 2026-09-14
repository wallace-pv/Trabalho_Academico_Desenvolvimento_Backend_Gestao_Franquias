using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Franquias.Api.DTOs.Auth;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Controllers;

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
    /// Autentica um usuário no sistema e retorna o token JWT.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    /// <summary>
    /// Cadastra um novo usuário no sistema com perfil de acesso.
    /// </summary>
    [HttpPost("registrar")]
    [Authorize(Roles = "AdminFranqueadora,GestorUnidade")]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegisterUsuarioDto request)
    {
        // Se o usuário logado for GestorUnidade, ele só pode criar usuários do tipo Operador vinculados à sua própria unidade
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userUnidadeIdStr = User.FindFirstValue("UnidadeId");

        if (userRole == "GestorUnidade")
        {
            if (request.Perfil != Models.Enums.PerfilUsuario.Operador)
            {
                return Forbid();
            }

            if (int.TryParse(userUnidadeIdStr, out var gestorUnidadeId))
            {
                request.UnidadeId = gestorUnidadeId;
            }
        }

        var response = await _authService.RegistrarUsuarioAsync(request);
        return CreatedAtAction(nameof(Login), new { id = response.Id }, response);
    }

    /// <summary>
    /// Lista usuários cadastrados com opções de filtro.
    /// </summary>
    [HttpGet("usuarios")]
    [Authorize(Roles = "AdminFranqueadora,GestorUnidade")]
    [ProducesResponseType(typeof(IEnumerable<UsuarioResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarUsuarios([FromQuery] int? unidadeId, [FromQuery] bool? ativo)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userUnidadeIdStr = User.FindFirstValue("UnidadeId");

        if (userRole == "GestorUnidade" && int.TryParse(userUnidadeIdStr, out var gestorUnidadeId))
        {
            unidadeId = gestorUnidadeId;
        }

        var usuarios = await _authService.ListarUsuariosAsync(unidadeId, ativo);
        return Ok(usuarios);
    }

    /// <summary>
    /// Ativa ou inativa um usuário do sistema.
    /// </summary>
    [HttpPatch("usuarios/{id:int}/status")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(UsuarioResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarStatus(int id, [FromQuery] bool ativo)
    {
        var response = await _authService.AtualizarStatusUsuarioAsync(id, ativo);
        return Ok(response);
    }
}
