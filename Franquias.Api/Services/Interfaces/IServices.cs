using Franquias.Api.DTOs.Auth;
using Franquias.Api.DTOs.Categorias;
using Franquias.Api.DTOs.Chamados;
using Franquias.Api.DTOs.Estoque;
using Franquias.Api.DTOs.Fornecedores;
using Franquias.Api.DTOs.Franqueadora;
using Franquias.Api.DTOs.Produtos;
using Franquias.Api.DTOs.Relatorios;
using Franquias.Api.DTOs.Royalties;
using Franquias.Api.DTOs.Unidades;
using Franquias.Api.DTOs.Vendas;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<UsuarioResponseDto> RegistrarUsuarioAsync(RegisterUsuarioDto request);
    Task<IEnumerable<UsuarioResponseDto>> ListarUsuariosAsync(int? unidadeId = null, bool? ativo = null);
    Task<UsuarioResponseDto> AtualizarStatusUsuarioAsync(int id, bool ativo);
}

public interface IUnidadeService
{
    Task<UnidadeResponseDto> CadastrarUnidadeAsync(CreateUnidadeDto dto);
    Task<UnidadeResponseDto> AtualizarUnidadeAsync(int id, UpdateUnidadeDto dto);
    Task<UnidadeResponseDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<UnidadeResponseDto>> ListarUnidadesAsync(SituacaoUnidade? situacao = null, string? termoBusca = null);
    Task<bool> InativarUnidadeAsync(int id);
}

public interface IProdutoService
{
    Task<ProdutoResponseDto> CriarProdutoAsync(CreateProdutoDto dto);
    Task<ProdutoResponseDto> AtualizarProdutoAsync(int id, UpdateProdutoDto dto);
    Task<ProdutoResponseDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<ProdutoResponseDto>> ListarProdutosAsync(int? categoriaId = null, StatusProduto? status = null, string? termoBusca = null);
    Task<bool> InativarProdutoAsync(int id);
}

public interface IEstoqueService
{
    Task<EstoqueResponseDto> MovimentarEstoqueAsync(MovimentarEstoqueDto dto, int? usuarioId = null);
    Task<IEnumerable<EstoqueResponseDto>> ObterEstoquePorUnidadeAsync(int unidadeId, bool apenasAbaixoDoMinimo = false);
    Task<IEnumerable<ItemEstoqueCriticoDto>> ObterEstoqueCriticoGlobalAsync();
    Task<IEnumerable<MovimentacaoResponseDto>> ObterHistoricoMovimentacoesAsync(int? unidadeId = null, int? produtoId = null);
}

public interface IVendaService
{
    Task<VendaResponseDto> RegistrarVendaAsync(CreateVendaDto dto, int usuarioId);
    Task<VendaResponseDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<VendaResponseDto>> ListarVendasAsync(int? unidadeId, DateTime? dataInicio, DateTime? dataFim);
}

public interface IRoyaltyService
{
    Task<RoyaltyResponseDto> ApurarRoyaltyAsync(GerarRoyaltyDto dto);
    Task<RoyaltyResponseDto> RegistrarPagamentoAsync(int id, PagarRoyaltyDto dto);
    Task<IEnumerable<RoyaltyResponseDto>> ListarRoyaltiesAsync(int? unidadeId = null, int? ano = null, StatusRoyalty? status = null);
}

public interface IFornecedorService
{
    Task<FornecedorResponseDto> CriarFornecedorAsync(CreateFornecedorDto dto);
    Task<FornecedorResponseDto> AtualizarFornecedorAsync(int id, UpdateFornecedorDto dto);
    Task<FornecedorResponseDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<FornecedorResponseDto>> ListarFornecedoresAsync(string? termoBusca = null, bool? ativo = null);
}

public interface IChamadoService
{
    Task<ChamadoResponseDto> AbrirChamadoAsync(CreateChamadoDto dto, int usuarioId);
    Task<ChamadoResponseDto> AtualizarStatusChamadoAsync(int id, UpdateChamadoDto dto);
    Task<ChamadoResponseDto?> ObterPorIdAsync(int id);
    Task<IEnumerable<ChamadoResponseDto>> ListarChamadosAsync(int? unidadeId = null, StatusChamado? status = null, PrioridadeChamado? prioridade = null);
}

public interface IRelatorioService
{
    Task<FaturamentoUnidadeDto> ObterFaturamentoUnidadeAsync(int unidadeId, DateTime dataInicio, DateTime dataFim);
    Task<IEnumerable<RankingUnidadeDto>> ObterRankingUnidadesAsync(DateTime? dataInicio = null, DateTime? dataFim = null);
    Task<TotalRoyaltiesDto> ObterTotalRoyaltiesAsync(int? ano = null);
    Task<IEnumerable<ProdutoMaisVendidoDto>> ObterProdutosMaisVendidosAsync(int top = 10);
    Task<IEnumerable<ItemEstoqueCriticoDto>> ObterEstoqueCriticoAsync();
    Task<IEnumerable<ChamadosStatusDto>> ObterChamadosPorStatusAsync();
    Task<DashboardGeralDto> ObterDashboardGeralAsync();
}
