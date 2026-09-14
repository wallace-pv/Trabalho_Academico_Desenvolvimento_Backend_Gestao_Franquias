using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Franquias.Api.DTOs.Estoque;
using Franquias.Api.DTOs.Relatorios;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EstoquesController : ControllerBase
{
    private readonly IEstoqueService _estoqueService;

    public EstoquesController(IEstoqueService estoqueService)
    {
        _estoqueService = estoqueService;
    }

    /// <summary>
    /// Consulta o saldo de estoque de uma unidade franqueada.
    /// </summary>
    [HttpGet("unidade/{unidadeId:int}")]
    [ProducesResponseType(typeof(IEnumerable<EstoqueResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterPorUnidade(int unidadeId, [FromQuery] bool apenasAbaixoDoMinimo = false)
    {
        var estoques = await _estoqueService.ObterEstoquePorUnidadeAsync(unidadeId, apenasAbaixoDoMinimo);
        return Ok(estoques);
    }

    /// <summary>
    /// Registra movimentações manuais de estoque (Entrada, Ajuste Positivo ou Ajuste Negativo) com validação de saldo não-negativo.
    /// </summary>
    [HttpPost("movimentar")]
    [ProducesResponseType(typeof(EstoqueResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MovimentarEstoque([FromBody] MovimentarEstoqueDto dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int? usuarioId = int.TryParse(userIdClaim, out var uid) ? uid : null;

        var resultado = await _estoqueService.MovimentarEstoqueAsync(dto, usuarioId);
        return Ok(resultado);
    }

    /// <summary>
    /// Lista itens com estoque crítico (abaixo do estoque mínimo) em todas as unidades ativas.
    /// </summary>
    [HttpGet("criticos")]
    [ProducesResponseType(typeof(IEnumerable<ItemEstoqueCriticoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterEstoqueCritico()
    {
        var criticos = await _estoqueService.ObterEstoqueCriticoGlobalAsync();
        return Ok(criticos);
    }

    /// <summary>
    /// Consulta o histórico de movimentações de estoque registradas.
    /// </summary>
    [HttpGet("movimentacoes")]
    [ProducesResponseType(typeof(IEnumerable<MovimentacaoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterHistorico([FromQuery] int? unidadeId, [FromQuery] int? produtoId)
    {
        var historico = await _estoqueService.ObterHistoricoMovimentacoesAsync(unidadeId, produtoId);
        return Ok(historico);
    }
}
