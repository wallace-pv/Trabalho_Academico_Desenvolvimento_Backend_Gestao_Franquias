using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Franquias.Api.DTOs.Chamados;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChamadosController : ControllerBase
{
    private readonly IChamadoService _chamadoService;

    public ChamadosController(IChamadoService chamadoService)
    {
        _chamadoService = chamadoService;
    }

    /// <summary>
    /// Abre um novo chamado de suporte ou solicitação entre unidade e franqueadora.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ChamadoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AbrirChamado([FromBody] CreateChamadoDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var usuarioId))
        {
            return Unauthorized(new { mensagem = "Usuário não autenticado." });
        }

        var chamado = await _chamadoService.AbrirChamadoAsync(dto, usuarioId);
        return CreatedAtAction(nameof(ObterPorId), new { id = chamado.Id }, chamado);
    }

    /// <summary>
    /// Consulta os dados detalhados de um chamado por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ChamadoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var chamado = await _chamadoService.ObterPorIdAsync(id);
        if (chamado == null)
            return NotFound(new { mensagem = $"Chamado com ID {id} não encontrado." });

        return Ok(chamado);
    }

    /// <summary>
    /// Atualiza a situação de um chamado (resolução, encerramento ou resposta).
    /// </summary>
    [HttpPut("{id:int}/status")]
    [ProducesResponseType(typeof(ChamadoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] UpdateChamadoDto dto)
    {
        var chamado = await _chamadoService.AtualizarStatusChamadoAsync(id, dto);
        return Ok(chamado);
    }

    /// <summary>
    /// Lista os chamados com opções de filtro por unidade, status e prioridade.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ChamadoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarChamados(
        [FromQuery] int? unidadeId,
        [FromQuery] StatusChamado? status,
        [FromQuery] PrioridadeChamado? prioridade)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userUnidadeIdStr = User.FindFirstValue("UnidadeId");

        if (userRole != "AdminFranqueadora" && int.TryParse(userUnidadeIdStr, out var uId))
        {
            unidadeId = uId;
        }

        var chamados = await _chamadoService.ListarChamadosAsync(unidadeId, status, prioridade);
        return Ok(chamados);
    }
}
