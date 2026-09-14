using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.DTOs.Franqueadora;

namespace Franquias.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FranqueadorasController : ControllerBase
{
    private readonly AppDbContext _context;

    public FranqueadorasController(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Consulta os dados da Franqueadora / Matriz.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FranqueadoraDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObterFranqueadora()
    {
        var franqueadoras = await _context.Franqueadoras.ToListAsync();
        var dtos = franqueadoras.Select(f => new FranqueadoraDto
        {
            Id = f.Id,
            RazaoSocial = f.RazaoSocial,
            NomeFantasia = f.NomeFantasia,
            CNPJ = f.CNPJ,
            Email = f.Email,
            Telefone = f.Telefone,
            PercentualPadraoRoyalty = f.PercentualPadraoRoyalty,
            DataFundacao = f.DataFundacao,
            Ativo = f.Ativo
        });

        return Ok(dtos);
    }

    /// <summary>
    /// Atualiza configurações da Franqueadora.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "AdminFranqueadora")]
    [ProducesResponseType(typeof(FranqueadoraDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarFranqueadora(int id, [FromBody] UpdateFranqueadoraDto dto)
    {
        var f = await _context.Franqueadoras.FindAsync(id);
        if (f == null)
            return NotFound(new { mensagem = "Franqueadora não encontrada." });

        f.NomeFantasia = dto.NomeFantasia;
        f.Email = dto.Email;
        f.Telefone = dto.Telefone;
        f.PercentualPadraoRoyalty = dto.PercentualPadraoRoyalty;

        await _context.SaveChangesAsync();

        return Ok(new FranqueadoraDto
        {
            Id = f.Id,
            RazaoSocial = f.RazaoSocial,
            NomeFantasia = f.NomeFantasia,
            CNPJ = f.CNPJ,
            Email = f.Email,
            Telefone = f.Telefone,
            PercentualPadraoRoyalty = f.PercentualPadraoRoyalty,
            DataFundacao = f.DataFundacao,
            Ativo = f.Ativo
        });
    }
}
