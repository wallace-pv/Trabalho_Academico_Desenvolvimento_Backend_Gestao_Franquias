using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Franquias.Api.DTOs.Unidades;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnidadesController : ControllerBase
{
    private readonly IUnidadeService _unidadeService;

    public UnidadesController(IUnidadeService unidadeService)
    {
        _unidadeService = unidadeService;
    }

    /// <summary>
    /// Lista as unidades franqueadas com opções de filtros (situação, nome, cidade, CNPJ, responsável).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UnidadeResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] SituacaoUnidade? situacao, [FromQuery] string? termoBusca)
    {
        var unidades = await _unidadeService.ListarUnidadesAsync(situacao, termoBusca);
        return Ok(unidades);
    }

    /// <summary>
    /// Consulta uma unidade franqueada específica por ID com dados do responsável.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(UnidadeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var unidade = await _unidadeService.ObterPorIdAsync(id);
        if (unidade == null)
            return NotFound(new { mensagem = $"Unidade com ID {id} não encontrada." });

        return Ok(unidade);
    }

    /// <summary>
    /// Cadastra uma nova unidade franqueada e seu franqueado responsável (apenas Administrador da Franqueadora).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(UnidadeResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cadastrar([FromBody] CreateUnidadeDto dto)
    {
        var unidade = await _unidadeService.CadastrarUnidadeAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = unidade.Id }, unidade);
    }

    /// <summary>
    /// Atualiza os dados cadastrais e operacionais de uma unidade franqueada.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdminFranqueadora,GestorUnidade")]
    [ProducesResponseType(typeof(UnidadeResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateUnidadeDto dto)
    {
        var unidade = await _unidadeService.AtualizarUnidadeAsync(id, dto);
        return Ok(unidade);
    }

    /// <summary>
    /// Inativa uma unidade franqueada (soft delete garantindo histórico).
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id)
    {
        await _unidadeService.InativarUnidadeAsync(id);
        return NoContent();
    }
}
