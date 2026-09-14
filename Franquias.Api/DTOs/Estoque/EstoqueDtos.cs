using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs.Estoque;

public class MovimentarEstoqueDto
{
    [Required]
    public int UnidadeId { get; set; }

    [Required]
    public int ProdutoId { get; set; }

    [Required]
    public TipoMovimentacaoEstoque Tipo { get; set; }

    [Range(1, 100000, ErrorMessage = "A quantidade movimentada deve ser de no mínimo 1.")]
    public int Quantidade { get; set; }

    public string? Observacao { get; set; }
}

public class DefinirEstoqueMinimoDto
{
    [Required]
    public int UnidadeId { get; set; }

    [Required]
    public int ProdutoId { get; set; }

    [Range(0, 10000)]
    public int QuantidadeMinima { get; set; }
}

public class EstoqueResponseDto
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public string UnidadeNome { get; set; } = string.Empty;
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string ProdutoSKU { get; set; } = string.Empty;
    public int QuantidadeDisponivel { get; set; }
    public int QuantidadeMinima { get; set; }
    public bool EstoqueCritico => QuantidadeDisponivel <= QuantidadeMinima;
    public DateTime UltimaAtualizacao { get; set; }
}

public class MovimentacaoResponseDto
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public string UnidadeNome { get; set; } = string.Empty;
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public TipoMovimentacaoEstoque Tipo { get; set; }
    public string TipoDescricao => Tipo.ToString();
    public int Quantidade { get; set; }
    public int SaldoAnterior { get; set; }
    public int SaldoAtual { get; set; }
    public string? Observacao { get; set; }
    public string? UsuarioNome { get; set; }
    public DateTime DataHora { get; set; }
}
