using Franquias.Api.Data;
using Franquias.Api.DTOs.Estoque;
using Franquias.Api.DTOs.Relatorios;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories.Interfaces;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class EstoqueService : IEstoqueService
{
    private readonly IEstoqueRepository _estoqueRepository;
    private readonly AppDbContext _context;

    public EstoqueService(IEstoqueRepository estoqueRepository, AppDbContext context)
    {
        _estoqueRepository = estoqueRepository;
        _context = context;
    }

    public async Task<EstoqueResponseDto> MovimentarEstoqueAsync(MovimentarEstoqueDto dto, int? usuarioId = null)
    {
        var unidade = await _context.Unidades.FindAsync(dto.UnidadeId);
        if (unidade == null)
            throw new KeyNotFoundException($"Unidade com ID {dto.UnidadeId} não encontrada.");

        var produto = await _context.Produtos.FindAsync(dto.ProdutoId);
        if (produto == null)
            throw new KeyNotFoundException($"Produto com ID {dto.ProdutoId} não encontrado.");

        if (produto.EServico)
            throw new BadHttpRequestException("Não é possível controlar estoque de itens classificados como serviços.");

        var estoque = await _estoqueRepository.GetByUnidadeEProdutoAsync(dto.UnidadeId, dto.ProdutoId);
        if (estoque == null)
        {
            estoque = new Estoque
            {
                UnidadeId = dto.UnidadeId,
                ProdutoId = dto.ProdutoId,
                QuantidadeDisponivel = 0,
                QuantidadeMinima = produto.EstoqueMinimoPadrao,
                UltimaAtualizacao = DateTime.UtcNow
            };
            await _estoqueRepository.AddAsync(estoque);
            await _estoqueRepository.SaveChangesAsync();
        }

        int saldoAnterior = estoque.QuantidadeDisponivel;
        int saldoNovo = saldoAnterior;

        switch (dto.Tipo)
        {
            case TipoMovimentacaoEstoque.Entrada:
            case TipoMovimentacaoEstoque.AjustePositivo:
                saldoNovo = saldoAnterior + dto.Quantidade;
                break;

            case TipoMovimentacaoEstoque.SaidaVenda:
            case TipoMovimentacaoEstoque.AjusteNegativo:
                if (saldoAnterior < dto.Quantidade)
                {
                    throw new BadHttpRequestException(
                        $"Operação não permitida. O estoque do produto '{produto.Nome}' na unidade '{unidade.Nome}' " +
                        $"não pode ficar negativo. Saldo disponível: {saldoAnterior}, Quantidade solicitada: {dto.Quantidade}.");
                }
                saldoNovo = saldoAnterior - dto.Quantidade;
                break;
        }

        estoque.QuantidadeDisponivel = saldoNovo;
        estoque.UltimaAtualizacao = DateTime.UtcNow;
        _estoqueRepository.Update(estoque);

        var movimentacao = new MovimentacaoEstoque
        {
            UnidadeId = dto.UnidadeId,
            ProdutoId = dto.ProdutoId,
            UsuarioId = usuarioId,
            Tipo = dto.Tipo,
            Quantidade = dto.Quantidade,
            SaldoAnterior = saldoAnterior,
            SaldoAtual = saldoNovo,
            Observacao = dto.Observacao,
            DataHora = DateTime.UtcNow
        };

        await _estoqueRepository.AddMovimentacaoAsync(movimentacao);
        await _estoqueRepository.SaveChangesAsync();

        return new EstoqueResponseDto
        {
            Id = estoque.Id,
            UnidadeId = estoque.UnidadeId,
            UnidadeNome = unidade.Nome,
            ProdutoId = estoque.ProdutoId,
            ProdutoNome = produto.Nome,
            ProdutoSKU = produto.CodigoSKU,
            QuantidadeDisponivel = estoque.QuantidadeDisponivel,
            QuantidadeMinima = estoque.QuantidadeMinima,
            UltimaAtualizacao = estoque.UltimaAtualizacao
        };
    }

    public async Task<IEnumerable<EstoqueResponseDto>> ObterEstoquePorUnidadeAsync(int unidadeId, bool apenasAbaixoDoMinimo = false)
    {
        var estoques = await _estoqueRepository.GetByUnidadeAsync(unidadeId, apenasAbaixoDoMinimo);
        return estoques.Select(e => new EstoqueResponseDto
        {
            Id = e.Id,
            UnidadeId = e.UnidadeId,
            UnidadeNome = e.Unidade?.Nome ?? string.Empty,
            ProdutoId = e.ProdutoId,
            ProdutoNome = e.Produto?.Nome ?? string.Empty,
            ProdutoSKU = e.Produto?.CodigoSKU ?? string.Empty,
            QuantidadeDisponivel = e.QuantidadeDisponivel,
            QuantidadeMinima = e.QuantidadeMinima,
            UltimaAtualizacao = e.UltimaAtualizacao
        });
    }

    public async Task<IEnumerable<ItemEstoqueCriticoDto>> ObterEstoqueCriticoGlobalAsync()
    {
        var criticos = await _estoqueRepository.GetEstoqueCriticoGlobalAsync();
        return criticos.Select(e => new ItemEstoqueCriticoDto
        {
            UnidadeId = e.UnidadeId,
            NomeUnidade = e.Unidade?.Nome ?? string.Empty,
            ProdutoId = e.ProdutoId,
            CodigoSKU = e.Produto?.CodigoSKU ?? string.Empty,
            NomeProduto = e.Produto?.Nome ?? string.Empty,
            QuantidadeDisponivel = e.QuantidadeDisponivel,
            QuantidadeMinima = e.QuantidadeMinima
        });
    }

    public async Task<IEnumerable<MovimentacaoResponseDto>> ObterHistoricoMovimentacoesAsync(int? unidadeId = null, int? produtoId = null)
    {
        var movimentacoes = await _estoqueRepository.GetMovimentacoesAsync(unidadeId, produtoId);
        return movimentacoes.Select(m => new MovimentacaoResponseDto
        {
            Id = m.Id,
            UnidadeId = m.UnidadeId,
            UnidadeNome = m.Unidade?.Nome ?? string.Empty,
            ProdutoId = m.ProdutoId,
            ProdutoNome = m.Produto?.Nome ?? string.Empty,
            Tipo = m.Tipo,
            Quantidade = m.Quantidade,
            SaldoAnterior = m.SaldoAnterior,
            SaldoAtual = m.SaldoAtual,
            Observacao = m.Observacao,
            UsuarioNome = m.Usuario?.Nome,
            DataHora = m.DataHora
        });
    }
}
