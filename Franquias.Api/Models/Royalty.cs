using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class Royalty
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public int MesReferencia { get; set; }
    public int AnoReferencia { get; set; }
    public decimal FaturamentoBase { get; set; }
    public decimal PercentualCobrado { get; set; }
    public decimal ValorRoyalty { get; set; }
    public StatusRoyalty Status { get; set; } = StatusRoyalty.Pendente;
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public DateTime DataApuracao { get; set; } = DateTime.UtcNow;
    public string? Observacao { get; set; }

    // Relacionamento de navegação
    public UnidadeFranqueada? Unidade { get; set; }
}
