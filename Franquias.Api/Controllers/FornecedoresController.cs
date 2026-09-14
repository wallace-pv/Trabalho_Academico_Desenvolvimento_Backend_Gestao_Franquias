using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Franquias.Api.DTOs.Fornecedores;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FornecedoresController : ControllerBase
{
    private readonly IFornecedorService _fornecedorService;

    public FornecedoresController(IFornecedorService fornecedorService)
    {
        _fornecedorService = fornecedorService;
    }

    /// <summary>
    /// Consulta os fornecedores homologados pela rede.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FornecedorResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] string? termoBusca, [FromQuery] bool? ativo)
    {
        var fornecedores = await _fornecedorService.ListarFornecedoresAsync(termoBusca, ativo);
        return Ok(fornecedores);
    }

    /// <summary>
    /// Consulta um fornecedor por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FornecedorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var fornecedor = await _fornecedorService.ObterPorIdAsync(id);
        if (fornecedor == null)
            return NotFound(new { mensagem = $"Fornecedor com ID {id} não encontrado." });

        return Ok(fornecedor);
    }

    /// <summary>
    /// Cadastra um novo fornecedor homologado.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(FornecedorResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CreateFornecedorDto dto)
    {
        var fornecedor = await _fornecedorService.CriarFornecedorAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = fornecedor.Id }, fornecedor);
    }

    /// <summary>
    /// Atualiza dados cadastrais de um fornecedor.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(FornecedorResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateFornecedorDto dto)
    {
        var fornecedor = await _fornecedorService.AtualizarFornecedorAsync(id, dto);
        return Ok(fornecedor);
    }
}
