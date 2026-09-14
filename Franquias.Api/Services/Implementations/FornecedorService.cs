using Franquias.Api.Data;
using Franquias.Api.DTOs.Fornecedores;
using Franquias.Api.Models;
using Franquias.Api.Repositories.Interfaces;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class FornecedorService : IFornecedorService
{
    private readonly IRepository<Fornecedor> _repository;
    private readonly AppDbContext _context;

    public FornecedorService(IRepository<Fornecedor> repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<FornecedorResponseDto> CriarFornecedorAsync(CreateFornecedorDto dto)
    {
        var cnpjLimpo = dto.CNPJ.Trim();
        var exists = await _context.Fornecedores.AnyAsync(f => f.CNPJ == cnpjLimpo);
        if (exists)
        {
            throw new BadHttpRequestException($"Já existe um fornecedor cadastrado com o CNPJ '{dto.CNPJ}'.");
        }

        var fornecedor = new Fornecedor
        {
            RazaoSocial = dto.RazaoSocial.Trim(),
            NomeFantasia = dto.NomeFantasia.Trim(),
            CNPJ = cnpjLimpo,
            Email = dto.Email.Trim().ToLower(),
            Telefone = dto.Telefone.Trim(),
            Cidade = dto.Cidade.Trim(),
            Estado = dto.Estado.Trim().ToUpper(),
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await _repository.AddAsync(fornecedor);
        await _repository.SaveChangesAsync();

        return MapToResponse(fornecedor);
    }

    public async Task<FornecedorResponseDto> AtualizarFornecedorAsync(int id, UpdateFornecedorDto dto)
    {
        var fornecedor = await _repository.GetByIdAsync(id);
        if (fornecedor == null)
            throw new KeyNotFoundException($"Fornecedor com ID {id} não encontrado.");

        fornecedor.RazaoSocial = dto.RazaoSocial.Trim();
        fornecedor.NomeFantasia = dto.NomeFantasia.Trim();
        fornecedor.Email = dto.Email.Trim().ToLower();
        fornecedor.Telefone = dto.Telefone.Trim();
        fornecedor.Cidade = dto.Cidade.Trim();
        fornecedor.Estado = dto.Estado.Trim().ToUpper();
        fornecedor.Ativo = dto.Ativo;

        _repository.Update(fornecedor);
        await _repository.SaveChangesAsync();

        return MapToResponse(fornecedor);
    }

    public async Task<FornecedorResponseDto?> ObterPorIdAsync(int id)
    {
        var fornecedor = await _repository.GetByIdAsync(id);
        return fornecedor == null ? null : MapToResponse(fornecedor);
    }

    public async Task<IEnumerable<FornecedorResponseDto>> ListarFornecedoresAsync(string? termoBusca = null, bool? ativo = null)
    {
        var query = _context.Fornecedores.AsQueryable();

        if (ativo.HasValue)
            query = query.Where(f => f.Ativo == ativo.Value);

        if (!string.IsNullOrWhiteSpace(termoBusca))
        {
            var termo = termoBusca.Trim().ToLower();
            query = query.Where(f =>
                f.NomeFantasia.ToLower().Contains(termo) ||
                f.RazaoSocial.ToLower().Contains(termo) ||
                f.CNPJ.Contains(termo) ||
                f.Cidade.ToLower().Contains(termo));
        }

        var list = await query.OrderBy(f => f.NomeFantasia).ToListAsync();
        return list.Select(MapToResponse);
    }

    private static FornecedorResponseDto MapToResponse(Fornecedor f)
    {
        return new FornecedorResponseDto
        {
            Id = f.Id,
            RazaoSocial = f.RazaoSocial,
            NomeFantasia = f.NomeFantasia,
            CNPJ = f.CNPJ,
            Email = f.Email,
            Telefone = f.Telefone,
            Cidade = f.Cidade,
            Estado = f.Estado,
            Ativo = f.Ativo,
            DataCadastro = f.DataCadastro
        };
    }
}
