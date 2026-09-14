using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.DTOs.Categorias;
using Franquias.Api.Models;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista as categorias de produtos e serviços.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoriaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] bool? apenasAtivas = true)
    {
        var query = _context.Categorias.Include(c => c.Produtos).AsQueryable();
        if (apenasAtivas.HasValue && apenasAtivas.Value)
        {
            query = query.Where(c => c.Ativo);
        }

        var categorias = await query.OrderBy(c => c.Nome).ToListAsync();
        var dtos = categorias.Select(c => new CategoriaResponseDto
        {
            Id = c.Id,
            Nome = c.Nome,
            Descricao = c.Descricao,
            Ativo = c.Ativo,
            QuantidadeProdutos = c.Produtos.Count
        });

        return Ok(dtos);
    }

    /// <summary>
    /// Cadastra uma nova categoria no catálogo (Administrador da Franqueadora).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Criar([FromBody] CreateCategoriaDto dto)
    {
        var categoria = new CategoriaProduto
        {
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao?.Trim(),
            Ativo = true
        };

        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();

        var response = new CategoriaResponseDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Descricao = categoria.Descricao,
            Ativo = categoria.Ativo,
            QuantidadeProdutos = 0
        };

        return CreatedAtAction(nameof(Listar), new { id = categoria.Id }, response);
    }

    /// <summary>
    /// Atualiza uma categoria existente.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(CategoriaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(int id, [FromBody] UpdateCategoriaDto dto)
    {
        var categoria = await _context.Categorias.Include(c => c.Produtos).FirstOrDefaultAsync(c => c.Id == id);
        if (categoria == null)
            return NotFound(new { mensagem = "Categoria não encontrada." });

        categoria.Nome = dto.Nome.Trim();
        categoria.Descricao = dto.Descricao?.Trim();
        categoria.Ativo = dto.Ativo;

        await _context.SaveChangesAsync();

        return Ok(new CategoriaResponseDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome,
            Descricao = categoria.Descricao,
            Ativo = categoria.Ativo,
            QuantidadeProdutos = categoria.Produtos.Count
        });
    }
}
