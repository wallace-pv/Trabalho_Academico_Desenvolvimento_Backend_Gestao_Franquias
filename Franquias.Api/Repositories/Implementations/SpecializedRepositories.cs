using Microsoft.EntityFrameworkCore;
using Franquias.Api.Data;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories.Interfaces;

namespace Franquias.Api.Repositories.Implementations;

public class UnidadeRepository : Repository<UnidadeFranqueada>, IUnidadeRepository
{
    public UnidadeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<UnidadeFranqueada?> GetWithDetailsAsync(int id)
    {
        return await _context.Unidades
            .Include(u => u.Franqueadora)
            .Include(u => u.Responsavel)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<IEnumerable<UnidadeFranqueada>> GetAllWithDetailsAsync(SituacaoUnidade? situacao = null, string? termoBusca = null)
    {
        var query = _context.Unidades
            .Include(u => u.Franqueadora)
            .Include(u => u.Responsavel)
            .AsQueryable();

        if (situacao.HasValue)
        {
            query = query.Where(u => u.Situacao == situacao.Value);
        }

        if (!string.IsNullOrWhiteSpace(termoBusca))
        {
            var termo = termoBusca.Trim().ToLower();
            query = query.Where(u =>
                u.Nome.ToLower().Contains(termo) ||
                u.Cidade.ToLower().Contains(termo) ||
                u.CNPJ.Contains(termo) ||
                (u.Responsavel != null && u.Responsavel.Nome.ToLower().Contains(termo)));
        }

        return await query.OrderBy(u => u.Nome).ToListAsync();
    }

    public async Task<bool> ExistsCnpjAsync(string cnpj, int? ignoreId = null)
    {
        var cleanCnpj = cnpj.Trim();
        var query = _context.Unidades.Where(u => u.CNPJ == cleanCnpj);
        if (ignoreId.HasValue)
        {
            query = query.Where(u => u.Id != ignoreId.Value);
        }
        return await query.AnyAsync();
    }
}

public class ProdutoRepository : Repository<ProdutoServico>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ProdutoServico?> GetWithDetailsAsync(int id)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<ProdutoServico>> GetAllWithDetailsAsync(int? categoriaId = null, StatusProduto? status = null, string? termoBusca = null)
    {
        var query = _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.Fornecedor)
            .AsQueryable();

        if (categoriaId.HasValue)
        {
            query = query.Where(p => p.CategoriaId == categoriaId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(termoBusca))
        {
            var termo = termoBusca.Trim().ToLower();
            query = query.Where(p =>
                p.Nome.ToLower().Contains(termo) ||
                p.CodigoSKU.ToLower().Contains(termo) ||
                p.Categoria!.Nome.ToLower().Contains(termo));
        }

        return await query.OrderBy(p => p.Nome).ToListAsync();
    }

    public async Task<bool> ExistsSkuAsync(string sku, int? ignoreId = null)
    {
        var cleanSku = sku.Trim().ToUpper();
        var query = _context.Produtos.Where(p => p.CodigoSKU.ToUpper() == cleanSku);
        if (ignoreId.HasValue)
        {
            query = query.Where(p => p.Id != ignoreId.Value);
        }
        return await query.AnyAsync();
    }
}

public class EstoqueRepository : Repository<Estoque>, IEstoqueRepository
{
    public EstoqueRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Estoque?> GetByUnidadeEProdutoAsync(int unidadeId, int produtoId)
    {
        return await _context.Estoques
            .Include(e => e.Unidade)
            .Include(e => e.Produto)
            .FirstOrDefaultAsync(e => e.UnidadeId == unidadeId && e.ProdutoId == produtoId);
    }

    public async Task<IEnumerable<Estoque>> GetByUnidadeAsync(int unidadeId, bool apenasAbaixoDoMinimo = false)
    {
        var query = _context.Estoques
            .Include(e => e.Produto)
            .Include(e => e.Unidade)
            .Where(e => e.UnidadeId == unidadeId);

        if (apenasAbaixoDoMinimo)
        {
            query = query.Where(e => e.QuantidadeDisponivel <= e.QuantidadeMinima);
        }

        return await query.OrderBy(e => e.Produto!.Nome).ToListAsync();
    }

    public async Task<IEnumerable<Estoque>> GetEstoqueCriticoGlobalAsync()
    {
        return await _context.Estoques
            .Include(e => e.Unidade)
            .Include(e => e.Produto)
            .Where(e => e.QuantidadeDisponivel <= e.QuantidadeMinima && e.Unidade!.Situacao == SituacaoUnidade.Ativa)
            .OrderBy(e => e.Unidade!.Nome)
            .ThenBy(e => e.Produto!.Nome)
            .ToListAsync();
    }

    public async Task AddMovimentacaoAsync(MovimentacaoEstoque movimentacao)
    {
        await _context.MovimentacoesEstoque.AddAsync(movimentacao);
    }

    public async Task<IEnumerable<MovimentacaoEstoque>> GetMovimentacoesAsync(int? unidadeId = null, int? produtoId = null)
    {
        var query = _context.MovimentacoesEstoque
            .Include(m => m.Unidade)
            .Include(m => m.Produto)
            .Include(m => m.Usuario)
            .AsQueryable();

        if (unidadeId.HasValue)
            query = query.Where(m => m.UnidadeId == unidadeId.Value);

        if (produtoId.HasValue)
            query = query.Where(m => m.ProdutoId == produtoId.Value);

        return await query.OrderByDescending(m => m.DataHora).Take(100).ToListAsync();
    }
}

public class VendaRepository : Repository<Venda>, IVendaRepository
{
    public VendaRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Venda?> GetWithDetailsAsync(int id)
    {
        return await _context.Vendas
            .Include(v => v.Unidade)
            .Include(v => v.Usuario)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Venda>> GetVendasPorPeriodoAsync(int? unidadeId, DateTime? dataInicio, DateTime? dataFim)
    {
        var query = _context.Vendas
            .Include(v => v.Unidade)
            .Include(v => v.Usuario)
            .Include(v => v.Itens)
                .ThenInclude(i => i.Produto)
            .AsQueryable();

        if (unidadeId.HasValue)
            query = query.Where(v => v.UnidadeId == unidadeId.Value);

        if (dataInicio.HasValue)
            query = query.Where(v => v.DataHora >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(v => v.DataHora <= dataFim.Value);

        return await query.OrderByDescending(v => v.DataHora).ToListAsync();
    }
}

public class RoyaltyRepository : Repository<Royalty>, IRoyaltyRepository
{
    public RoyaltyRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Royalty?> GetByUnidadeEMesAnoAsync(int unidadeId, int mes, int ano)
    {
        return await _context.Royalties
            .Include(r => r.Unidade)
            .FirstOrDefaultAsync(r => r.UnidadeId == unidadeId && r.MesReferencia == mes && r.AnoReferencia == ano);
    }

    public async Task<IEnumerable<Royalty>> GetRoyaltiesComDetalhesAsync(int? unidadeId = null, int? ano = null, StatusRoyalty? status = null)
    {
        var query = _context.Royalties
            .Include(r => r.Unidade)
            .AsQueryable();

        if (unidadeId.HasValue)
            query = query.Where(r => r.UnidadeId == unidadeId.Value);

        if (ano.HasValue)
            query = query.Where(r => r.AnoReferencia == ano.Value);

        if (status.HasValue)
            query = query.Where(r => r.Status == status.Value);

        return await query.OrderByDescending(r => r.AnoReferencia).ThenByDescending(r => r.MesReferencia).ToListAsync();
    }
}

public class ChamadoRepository : Repository<ChamadoSuporte>, IChamadoRepository
{
    public ChamadoRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ChamadoSuporte?> GetWithDetailsAsync(int id)
    {
        return await _context.Chamados
            .Include(c => c.Unidade)
            .Include(c => c.UsuarioAbertura)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<ChamadoSuporte>> GetChamadosAsync(int? unidadeId = null, StatusChamado? status = null, PrioridadeChamado? prioridade = null)
    {
        var query = _context.Chamados
            .Include(c => c.Unidade)
            .Include(c => c.UsuarioAbertura)
            .AsQueryable();

        if (unidadeId.HasValue)
            query = query.Where(c => c.UnidadeId == unidadeId.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (prioridade.HasValue)
            query = query.Where(c => c.Prioridade == prioridade.Value);

        return await query.OrderByDescending(c => c.DataAbertura).ToListAsync();
    }
}
