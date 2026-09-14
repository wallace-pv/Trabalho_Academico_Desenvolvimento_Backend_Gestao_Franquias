using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Franquias.Api.DTOs.Produtos;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _produtoService;

    public ProdutosController(IProdutoService produtoService)
    {
        _produtoService = produtoService;
    }

    /// <summary>
    /// Consulta o catálogo de produtos e serviços padronizados da rede.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProdutoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] int? categoriaId,
        [FromQuery] StatusProduto? status,
        [FromQuery] string? termoBusca)
    {
        var produtos = await _produtoService.ListarProdutosAsync(categoriaId, status, termoBusca);
        return Ok(produtos);
    }

    /// <summary>
    /// Consulta um produto ou serviço por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(int id)
    {
        var produto = await _produtoService.ObterPorIdAsync(id);
        if (produto == null)
            return NotFound(new { mensagem = $"Produto com ID {id} não encontrado." });

        return Ok(produto);
    }

    /// <summary>
    /// Cadastra um novo produto ou serviço padronizado (Administrador da Franqueadora).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CreateProdutoDto dto)
    {
        var produto = await _produtoService.CriarProdutoAsync(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
    }

    /// <summary>
    /// Atualiza dados de um produto ou serviço cadastrado.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(ProdutoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateProdutoDto dto)
    {
        var produto = await _produtoService.AtualizarProdutoAsync(id, dto);
        return Ok(produto);
    }

    /// <summary>
    /// Inativa um produto no catálogo.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(int id)
    {
        await _produtoService.InativarProdutoAsync(id);
        return NoContent();
    }
}
