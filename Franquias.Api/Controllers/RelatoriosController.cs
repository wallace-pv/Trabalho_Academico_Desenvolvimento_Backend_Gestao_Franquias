using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Franquias.Api.DTOs.Relatorios;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RelatoriosController : ControllerBase
{
    private readonly IRelatorioService _relatorioService;

    public RelatoriosController(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    /// <summary>
    /// Consulta o faturamento detalhado de uma unidade em um período.
    /// </summary>
    [HttpGet("faturamento")]
    [ProducesResponseType(typeof(FaturamentoUnidadeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObterFaturamento(
        [FromQuery] int unidadeId,
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim)
    {
        if (dataInicio > dataFim)
        {
            return BadRequest(new { mensagem = "A data de início não pode ser superior à data de término." });
        }

        var relatorio = await _relatorioService.ObterFaturamentoUnidadeAsync(unidadeId, dataInicio, dataFim);
        return Ok(relatorio);
    }

    /// <summary>
    /// Retorna o ranking de unidades franqueadas ordenadas pelo maior faturamento (Admin Franqueadora).
    /// </summary>
    [HttpGet("ranking-unidades")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(IEnumerable<RankingUnidadeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterRankingUnidades([FromQuery] DateTime? dataInicio, [FromQuery] DateTime? dataFim)
    {
        var ranking = await _relatorioService.ObterRankingUnidadesAsync(dataInicio, dataFim);
        return Ok(ranking);
    }

    /// <summary>
    /// Retorna o total consolidado de royalties gerados, pagos, pendentes e atrasados.
    /// </summary>
    [HttpGet("royalties")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(TotalRoyaltiesDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterTotalRoyalties([FromQuery] int? ano)
    {
        var resultado = await _relatorioService.ObterTotalRoyaltiesAsync(ano);
        return Ok(resultado);
    }

    /// <summary>
    /// Retorna os produtos e serviços mais comercializados pela rede de franquias.
    /// </summary>
    [HttpGet("produtos-mais-vendidos")]
    [ProducesResponseType(typeof(IEnumerable<ProdutoMaisVendidoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterProdutosMaisVendidos([FromQuery] int top = 10)
    {
        var produtos = await _relatorioService.ObterProdutosMaisVendidosAsync(top);
        return Ok(produtos);
    }

    /// <summary>
    /// Retorna todos os itens cujo saldo em estoque está abaixo ou no limite mínimo configurado.
    /// </summary>
    [HttpGet("estoque-critico")]
    [ProducesResponseType(typeof(IEnumerable<ItemEstoqueCriticoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterEstoqueCritico()
    {
        var criticos = await _relatorioService.ObterEstoqueCriticoAsync();
        return Ok(criticos);
    }

    /// <summary>
    /// Retorna a contagem agrupada de chamados de suporte por status.
    /// </summary>
    [HttpGet("chamados-status")]
    [ProducesResponseType(typeof(IEnumerable<ChamadosStatusDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterChamadosPorStatus()
    {
        var status = await _relatorioService.ObterChamadosPorStatusAsync();
        return Ok(status);
    }

    /// <summary>
    /// Retorna os indicadores do dashboard corporativo geral da rede.
    /// </summary>
    [HttpGet("dashboard")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(DashboardGeralDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterDashboardGeral()
    {
        var dashboard = await _relatorioService.ObterDashboardGeralAsync();
        return Ok(dashboard);
    }
}
