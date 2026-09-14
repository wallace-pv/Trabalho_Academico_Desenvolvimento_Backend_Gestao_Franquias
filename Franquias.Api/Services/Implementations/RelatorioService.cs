using Franquias.Api.Data;
using Franquias.Api.DTOs.Relatorios;
using Franquias.Api.Models.Enums;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class RelatorioService : IRelatorioService
{
    private readonly AppDbContext _context;

    public RelatorioService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FaturamentoUnidadeDto> ObterFaturamentoUnidadeAsync(int unidadeId, DateTime dataInicio, DateTime dataFim)
    {
        var unidade = await _context.Unidades.FindAsync(unidadeId);
        if (unidade == null)
            throw new KeyNotFoundException($"Unidade com ID {unidadeId} não encontrada.");

        var vendas = await _context.Vendas
            .Where(v => v.UnidadeId == unidadeId &&
                        v.Status == StatusVenda.Concluida &&
                        v.DataHora >= dataInicio &&
                        v.DataHora <= dataFim)
            .ToListAsync();

        var totalFaturado = vendas.Sum(v => v.ValorTotal);
        var totalVendas = vendas.Count;

        return new FaturamentoUnidadeDto
        {
            UnidadeId = unidade.Id,
            CodigoUnidade = unidade.CodigoUnidade,
            NomeUnidade = unidade.Nome,
            DataInicio = dataInicio,
            DataFim = dataFim,
            QuantidadeVendas = totalVendas,
            TotalFaturado = totalFaturado
        };
    }

    public async Task<IEnumerable<RankingUnidadeDto>> ObterRankingUnidadesAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var queryVendas = _context.Vendas
            .Where(v => v.Status == StatusVenda.Concluida)
            .AsQueryable();

        if (dataInicio.HasValue)
            queryVendas = queryVendas.Where(v => v.DataHora >= dataInicio.Value);

        if (dataFim.HasValue)
            queryVendas = queryVendas.Where(v => v.DataHora <= dataFim.Value);

        var vendasLista = await queryVendas
            .Select(v => new { v.UnidadeId, v.ValorTotal })
            .ToListAsync();

        var rankingAgrupado = vendasLista
            .GroupBy(v => v.UnidadeId)
            .Select(g => new
            {
                UnidadeId = g.Key,
                TotalVendas = g.Count(),
                TotalFaturado = g.Sum(v => v.ValorTotal)
            })
            .OrderByDescending(x => x.TotalFaturado)
            .ToList();

        var unidadesIds = rankingAgrupado.Select(r => r.UnidadeId).ToList();
        var unidadesDict = await _context.Unidades
            .Where(u => unidadesIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u);

        var resultado = new List<RankingUnidadeDto>();
        int posicao = 1;

        foreach (var item in rankingAgrupado)
        {
            if (unidadesDict.TryGetValue(item.UnidadeId, out var unidade))
            {
                resultado.Add(new RankingUnidadeDto
                {
                    Posicao = posicao++,
                    UnidadeId = unidade.Id,
                    CodigoUnidade = unidade.CodigoUnidade,
                    NomeUnidade = unidade.Nome,
                    Cidade = unidade.Cidade,
                    Estado = unidade.Estado,
                    TotalVendas = item.TotalVendas,
                    TotalFaturado = item.TotalFaturado
                });
            }
        }

        return resultado;
    }

    public async Task<TotalRoyaltiesDto> ObterTotalRoyaltiesAsync(int? ano = null)
    {
        var query = _context.Royalties.AsQueryable();

        if (ano.HasValue)
            query = query.Where(r => r.AnoReferencia == ano.Value);

        var lista = await query.ToListAsync();

        return new TotalRoyaltiesDto
        {
            TotalGerado = lista.Sum(r => r.ValorRoyalty),
            TotalPago = lista.Where(r => r.Status == StatusRoyalty.Pago).Sum(r => r.ValorRoyalty),
            TotalPendente = lista.Where(r => r.Status == StatusRoyalty.Pendente).Sum(r => r.ValorRoyalty),
            TotalAtrasado = lista.Where(r => r.Status == StatusRoyalty.Atrasado).Sum(r => r.ValorRoyalty),
            QuantidadeCobrancas = lista.Count
        };
    }

    public async Task<IEnumerable<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(int top = 10)
    {
        var itensLista = await _context.ItensVenda
            .Where(i => i.Venda!.Status == StatusVenda.Concluida)
            .Select(i => new { i.ProdutoId, i.Quantidade, i.Subtotal })
            .ToListAsync();

        var agrupado = itensLista
            .GroupBy(i => i.ProdutoId)
            .Select(g => new
            {
                ProdutoId = g.Key,
                QuantidadeVendida = g.Sum(x => x.Quantidade),
                ReceitaGerada = g.Sum(x => x.Subtotal)
            })
            .OrderByDescending(x => x.QuantidadeVendida)
            .Take(top)
            .ToList();

        var produtosIds = agrupado.Select(x => x.ProdutoId).ToList();
        var produtosDict = await _context.Produtos
            .Include(p => p.Categoria)
            .Where(p => produtosIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p);

        var resultado = new List<ProdutoMaisVendidoDto>();
        foreach (var item in agrupado)
        {
            if (produtosDict.TryGetValue(item.ProdutoId, out var produto))
            {
                resultado.Add(new ProdutoMaisVendidoDto
                {
                    ProdutoId = produto.Id,
                    CodigoSKU = produto.CodigoSKU,
                    NomeProduto = produto.Nome,
                    CategoriaNome = produto.Categoria?.Nome ?? "Sem categoria",
                    QuantidadeTotalVendida = item.QuantidadeVendida,
                    ReceitaTotalGerada = item.ReceitaGerada
                });
            }
        }

        return resultado;
    }

    public async Task<IEnumerable<ItemEstoqueCriticoDto>> ObterEstoqueCriticoAsync()
    {
        return await _context.Estoques
            .Include(e => e.Unidade)
            .Include(e => e.Produto)
            .Where(e => e.QuantidadeDisponivel <= e.QuantidadeMinima && e.Unidade!.Situacao == SituacaoUnidade.Ativa)
            .OrderBy(e => e.Unidade!.Nome)
            .Select(e => new ItemEstoqueCriticoDto
            {
                UnidadeId = e.UnidadeId,
                NomeUnidade = e.Unidade!.Nome,
                ProdutoId = e.ProdutoId,
                CodigoSKU = e.Produto!.CodigoSKU,
                NomeProduto = e.Produto!.Nome,
                QuantidadeDisponivel = e.QuantidadeDisponivel,
                QuantidadeMinima = e.QuantidadeMinima
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<ChamadosStatusDto>> ObterChamadosPorStatusAsync()
    {
        return await _context.Chamados
            .GroupBy(c => c.Status)
            .Select(g => new ChamadosStatusDto
            {
                Status = g.Key.ToString(),
                Quantidade = g.Count()
            })
            .ToListAsync();
    }

    public async Task<DashboardGeralDto> ObterDashboardGeralAsync()
    {
        var agora = DateTime.UtcNow;
        var inicioMes = new DateTime(agora.Year, agora.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var fimMes = inicioMes.AddMonths(1).AddTicks(-1);

        var vendasMesValores = await _context.Vendas
            .Where(v => v.Status == StatusVenda.Concluida && v.DataHora >= inicioMes && v.DataHora <= fimMes)
            .Select(v => v.ValorTotal)
            .ToListAsync();
        var faturamentoMes = vendasMesValores.Sum();

        var royaltiesMesValores = await _context.Royalties
            .Where(r => r.MesReferencia == agora.Month && r.AnoReferencia == agora.Year)
            .Select(r => r.ValorRoyalty)
            .ToListAsync();
        var royaltiesMes = royaltiesMesValores.Sum();

        return new DashboardGeralDto
        {
            TotalUnidadesAtivas = await _context.Unidades.CountAsync(u => u.Situacao == SituacaoUnidade.Ativa),
            TotalUnidadesInativas = await _context.Unidades.CountAsync(u => u.Situacao == SituacaoUnidade.Inativa),
            TotalProdutosCadastrados = await _context.Produtos.CountAsync(p => p.Status == StatusProduto.Ativo),
            FaturamentoGlobalMesAtual = faturamentoMes,
            RoyaltiesMesAtual = royaltiesMes,
            ChamadosEmAberto = await _context.Chamados.CountAsync(c => c.Status == StatusChamado.Aberto || c.Status == StatusChamado.EmAndamento),
            ItensEstoqueCriticoTotal = await _context.Estoques.CountAsync(e => e.QuantidadeDisponivel <= e.QuantidadeMinima)
        };
    }
}
