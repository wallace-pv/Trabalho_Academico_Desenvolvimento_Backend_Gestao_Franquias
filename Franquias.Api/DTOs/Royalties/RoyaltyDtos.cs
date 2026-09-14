using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs.Royalties;

public class GerarRoyaltyDto
{
    [Required]
    public int UnidadeId { get; set; }

    [Range(1, 12, ErrorMessage = "Mês de referência deve estar entre 1 e 12.")]
    public int MesReferencia { get; set; }

    [Range(2020, 2100, ErrorMessage = "Ano de referência inválido.")]
    public int AnoReferencia { get; set; }

    public string? Observacao { get; set; }
}

public class PagarRoyaltyDto
{
    [Required]
    public DateTime DataPagamento { get; set; } = DateTime.UtcNow;

    public string? Observacao { get; set; }
}

public class RoyaltyResponseDto
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public string UnidadeNome { get; set; } = string.Empty;
    public string CodigoUnidade { get; set; } = string.Empty;
    public int MesReferencia { get; set; }
    public int AnoReferencia { get; set; }
    public decimal FaturamentoBase { get; set; }
    public decimal PercentualCobrado { get; set; }
    public decimal ValorRoyalty { get; set; }
    public StatusRoyalty Status { get; set; }
    public string StatusDescricao => Status.ToString();
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public DateTime DataApuracao { get; set; }
    public string? Observacao { get; set; }
}
