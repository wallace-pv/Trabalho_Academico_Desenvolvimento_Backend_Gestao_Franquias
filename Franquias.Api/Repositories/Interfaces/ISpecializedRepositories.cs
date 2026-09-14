using Franquias.Api.Models;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Repositories.Interfaces;

public interface IUnidadeRepository : IRepository<UnidadeFranqueada>
{
    Task<UnidadeFranqueada?> GetWithDetailsAsync(int id);
    Task<IEnumerable<UnidadeFranqueada>> GetAllWithDetailsAsync(SituacaoUnidade? situacao = null, string? termoBusca = null);
    Task<bool> ExistsCnpjAsync(string cnpj, int? ignoreId = null);
}

public interface IProdutoRepository : IRepository<ProdutoServico>
{
    Task<ProdutoServico?> GetWithDetailsAsync(int id);
    Task<IEnumerable<ProdutoServico>> GetAllWithDetailsAsync(int? categoriaId = null, StatusProduto? status = null, string? termoBusca = null);
    Task<bool> ExistsSkuAsync(string sku, int? ignoreId = null);
}

public interface IEstoqueRepository : IRepository<Estoque>
{
    Task<Estoque?> GetByUnidadeEProdutoAsync(int unidadeId, int produtoId);
    Task<IEnumerable<Estoque>> GetByUnidadeAsync(int unidadeId, bool apenasAbaixoDoMinimo = false);
    Task<IEnumerable<Estoque>> GetEstoqueCriticoGlobalAsync();
    Task AddMovimentacaoAsync(MovimentacaoEstoque movimentacao);
    Task<IEnumerable<MovimentacaoEstoque>> GetMovimentacoesAsync(int? unidadeId = null, int? produtoId = null);
}

public interface IVendaRepository : IRepository<Venda>
{
    Task<Venda?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Venda>> GetVendasPorPeriodoAsync(int? unidadeId, DateTime? dataInicio, DateTime? dataFim);
}

public interface IRoyaltyRepository : IRepository<Royalty>
{
    Task<Royalty?> GetByUnidadeEMesAnoAsync(int unidadeId, int mes, int ano);
    Task<IEnumerable<Royalty>> GetRoyaltiesComDetalhesAsync(int? unidadeId = null, int? ano = null, StatusRoyalty? status = null);
}

public interface IChamadoRepository : IRepository<ChamadoSuporte>
{
    Task<ChamadoSuporte?> GetWithDetailsAsync(int id);
    Task<IEnumerable<ChamadoSuporte>> GetChamadosAsync(int? unidadeId = null, StatusChamado? status = null, PrioridadeChamado? prioridade = null);
}
