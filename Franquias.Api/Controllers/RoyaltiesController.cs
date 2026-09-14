using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Franquias.Api.DTOs.Royalties;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoyaltiesController : ControllerBase
{
    private readonly IRoyaltyService _royaltyService;

    public RoyaltiesController(IRoyaltyService royaltyService)
    {
        _royaltyService = royaltyService;
    }

    /// <summary>
    /// Calcula e apura o royalty de uma unidade com base no faturamento do mês/ano informado (Admin Franqueadora).
    /// </summary>
    [HttpPost("apurar")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(RoyaltyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApurarRoyalty([FromBody] GerarRoyaltyDto dto)
    {
        var royalty = await _royaltyService.ApurarRoyaltyAsync(dto);
        return Ok(royalty);
    }

    /// <summary>
    /// Registra a liquidação/pagamento de uma cobrança de royalty.
    /// </summary>
    [HttpPost("{id:int}/pagar")]
    [Authorize(Roles = "AdminFranqueadora,GestorUnidade")]
    [ProducesResponseType(typeof(RoyaltyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PagarRoyalty(int id, [FromBody] PagarRoyaltyDto dto)
    {
        var royalty = await _royaltyService.RegistrarPagamentoAsync(id, dto);
        return Ok(royalty);
    }

    /// <summary>
    /// Consulta os lançamentos de royalties, valores devidos e pagos por unidade.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RoyaltyResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarRoyalties(
        [FromQuery] int? unidadeId,
        [FromQuery] int? ano,
        [FromQuery] StatusRoyalty? status)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var userUnidadeIdStr = User.FindFirstValue("UnidadeId");

        if (userRole != "AdminFranqueadora" && int.TryParse(userUnidadeIdStr, out var uId))
        {
            unidadeId = uId;
        }

        var royalties = await _royaltyService.ListarRoyaltiesAsync(unidadeId, ano, status);
        return Ok(royalties);
    }
}
