using Franquias.Api.Data;
using Franquias.Api.DTOs.Produtos;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories.Interfaces;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly AppDbContext _context;

    public ProdutoService(IProdutoRepository produtoRepository, AppDbContext context)
    {
        _produtoRepository = produtoRepository;
        _context = context;
    }

    public async Task<ProdutoResponseDto> CriarProdutoAsync(CreateProdutoDto dto)
    {
        if (await _produtoRepository.ExistsSkuAsync(dto.CodigoSKU))
        {
            throw new BadHttpRequestException($"Já existe um produto com o SKU '{dto.CodigoSKU}'.");
        }

        var categoriaExists = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!categoriaExists)
        {
            throw new BadHttpRequestException("A categoria informada não existe.");
        }

        if (dto.FornecedorId.HasValue)
        {
            var fornecedorExists = await _context.Fornecedores.AnyAsync(f => f.Id == dto.FornecedorId.Value);
            if (!fornecedorExists)
            {
                throw new BadHttpRequestException("O fornecedor informado não existe.");
            }
        }

        var produto = new ProdutoServico
        {
            CategoriaId = dto.CategoriaId,
            FornecedorId = dto.FornecedorId,
            CodigoSKU = dto.CodigoSKU.Trim().ToUpper(),
            Nome = dto.Nome.Trim(),
            Descricao = dto.Descricao.Trim(),
            PrecoBase = dto.PrecoBase,
            EstoqueMinimoPadrao = dto.EstoqueMinimoPadrao,
            EServico = dto.EServico,
            Status = StatusProduto.Ativo,
            DataCadastro = DateTime.UtcNow
        };

        await _produtoRepository.AddAsync(produto);
        await _produtoRepository.SaveChangesAsync();

        var produtoCriado = await _produtoRepository.GetWithDetailsAsync(produto.Id);
        return MapToResponse(produtoCriado!);
    }

    public async Task<ProdutoResponseDto> AtualizarProdutoAsync(int id, UpdateProdutoDto dto)
    {
        var produto = await _produtoRepository.GetWithDetailsAsync(id);
        if (produto == null)
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

        var categoriaExists = await _context.Categorias.AnyAsync(c => c.Id == dto.CategoriaId);
        if (!categoriaExists)
        {
            throw new BadHttpRequestException("A categoria informada não existe.");
        }

        if (dto.FornecedorId.HasValue)
        {
            var fornecedorExists = await _context.Fornecedores.AnyAsync(f => f.Id == dto.FornecedorId.Value);
            if (!fornecedorExists)
            {
                throw new BadHttpRequestException("O fornecedor informado não existe.");
            }
        }

        produto.CategoriaId = dto.CategoriaId;
        produto.FornecedorId = dto.FornecedorId;
        produto.Nome = dto.Nome.Trim();
        produto.Descricao = dto.Descricao.Trim();
        produto.PrecoBase = dto.PrecoBase;
        produto.EstoqueMinimoPadrao = dto.EstoqueMinimoPadrao;
        produto.EServico = dto.EServico;
        produto.Status = dto.Status;

        _produtoRepository.Update(produto);
        await _produtoRepository.SaveChangesAsync();

        var produtoAtualizado = await _produtoRepository.GetWithDetailsAsync(id);
        return MapToResponse(produtoAtualizado!);
    }

    public async Task<ProdutoResponseDto?> ObterPorIdAsync(int id)
    {
        var produto = await _produtoRepository.GetWithDetailsAsync(id);
        return produto == null ? null : MapToResponse(produto);
    }

    public async Task<IEnumerable<ProdutoResponseDto>> ListarProdutosAsync(int? categoriaId = null, StatusProduto? status = null, string? termoBusca = null)
    {
        var lista = await _produtoRepository.GetAllWithDetailsAsync(categoriaId, status, termoBusca);
        return lista.Select(MapToResponse);
    }

    public async Task<bool> InativarProdutoAsync(int id)
    {
        var produto = await _produtoRepository.GetByIdAsync(id);
        if (produto == null)
            throw new KeyNotFoundException($"Produto com ID {id} não encontrado.");

        produto.Status = StatusProduto.Inativo;
        _produtoRepository.Update(produto);
        return await _produtoRepository.SaveChangesAsync() > 0;
    }

    private static ProdutoResponseDto MapToResponse(ProdutoServico p)
    {
        return new ProdutoResponseDto
        {
            Id = p.Id,
            CategoriaId = p.CategoriaId,
            CategoriaNome = p.Categoria?.Nome ?? string.Empty,
            FornecedorId = p.FornecedorId,
            FornecedorNome = p.Fornecedor?.NomeFantasia,
            CodigoSKU = p.CodigoSKU,
            Nome = p.Nome,
            Descricao = p.Descricao,
            PrecoBase = p.PrecoBase,
            EstoqueMinimoPadrao = p.EstoqueMinimoPadrao,
            EServico = p.EServico,
            Status = p.Status,
            DataCadastro = p.DataCadastro
        };
    }
}
