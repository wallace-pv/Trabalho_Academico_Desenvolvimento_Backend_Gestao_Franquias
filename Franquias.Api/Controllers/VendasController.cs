using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Franquias.Api.DTOs.Vendas;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VendasController : ControllerBase
{
    private readonly IVendaService _vendaService;

    public VendasController(IVendaService vendaService)
    {
        _vendaService = vendaService;
    }

    /// <summary>
    /// Registra uma nova venda para uma unidade franqueada ativa, com itens, cálculo automático e baixa de estoque.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(VendaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarVenda([FromBody] CreateVendaDto dto)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var usuarioId))
        {
            return Unauthorized(new { mensagem = "Usuário não autenticado." });
        }

        var venda = await _vendaService.RegistrarVendaAsync(dto, usuarioId);
        return CreatedAtAction(nameof(ObterPorId), new { id = venda.Id }, venda);
    }

    /// <summary>
    /// Consulta os detalhes completos de uma venda pelo ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(VendaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var venda = await _vendaService.ObterPorIdAsync(id);
        if (venda == null)
            return NotFound(new { mensagem = $"Venda com ID {id} não encontrada." });

        return Ok(venda);
    }

    /// <summary>
    /// Consulta vendas realizadas por unidade e intervalo de datas.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VendaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarVendas(
        [FromQuery] int? unidadeId,
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userUnidadeIdStr = User.FindFirstValue("UnidadeId");

        // Usuários de unidade só podem visualizar vendas da sua unidade
        if (userRole != "AdminFranqueadora" && int.TryParse(userUnidadeIdStr, out var uId))
        {
            unidadeId = uId;
        }

        var vendas = await _vendaService.ListarVendasAsync(unidadeId, dataInicio, dataFim);
        return Ok(vendas);
    }
}
