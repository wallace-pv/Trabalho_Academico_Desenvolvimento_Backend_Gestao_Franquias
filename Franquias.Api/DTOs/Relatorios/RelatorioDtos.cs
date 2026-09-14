namespace Franquias.Api.DTOs.Relatorios;

public class FaturamentoUnidadeDto
{
    public int UnidadeId { get; set; }
    public string CodigoUnidade { get; set; } = string.Empty;
    public string NomeUnidade { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public int QuantidadeVendas { get; set; }
    public decimal TotalFaturado { get; set; }
    public decimal TicketMedio => QuantidadeVendas > 0 ? Math.Round(TotalFaturado / QuantidadeVendas, 2) : 0;
}

public class RankingUnidadeDto
{
    public int Posicao { get; set; }
    public int UnidadeId { get; set; }
    public string CodigoUnidade { get; set; } = string.Empty;
    public string NomeUnidade { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int TotalVendas { get; set; }
    public decimal TotalFaturado { get; set; }
}

public class TotalRoyaltiesDto
{
    public decimal TotalGerado { get; set; }
    public decimal TotalPago { get; set; }
    public decimal TotalPendente { get; set; }
    public decimal TotalAtrasado { get; set; }
    public int QuantidadeCobrancas { get; set; }
}

public class ProdutoMaisVendidoDto
{
    public int ProdutoId { get; set; }
    public string CodigoSKU { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public string CategoriaNome { get; set; } = string.Empty;
    public int QuantidadeTotalVendida { get; set; }
    public decimal ReceitaTotalGerada { get; set; }
}

public class ItemEstoqueCriticoDto
{
    public int UnidadeId { get; set; }
    public string NomeUnidade { get; set; } = string.Empty;
    public int ProdutoId { get; set; }
    public string CodigoSKU { get; set; } = string.Empty;
    public string NomeProduto { get; set; } = string.Empty;
    public int QuantidadeDisponivel { get; set; }
    public int QuantidadeMinima { get; set; }
    public int DeficitEstoque => QuantidadeMinima - QuantidadeDisponivel;
}

public class ChamadosStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Quantidade { get; set; }
}

public class DashboardGeralDto
{
    public int TotalUnidadesAtivas { get; set; }
    public int TotalUnidadesInativas { get; set; }
    public int TotalProdutosCadastrados { get; set; }
    public decimal FaturamentoGlobalMesAtual { get; set; }
    public decimal RoyaltiesMesAtual { get; set; }
    public int ChamadosEmAberto { get; set; }
    public int ItensEstoqueCriticoTotal { get; set; }
}
