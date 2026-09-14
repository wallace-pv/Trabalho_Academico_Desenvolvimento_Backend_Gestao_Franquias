using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs.Produtos;

public class CreateProdutoDto
{
    [Required]
    public int CategoriaId { get; set; }

    public int? FornecedorId { get; set; }

    [Required]
    [StringLength(30, MinimumLength = 3)]
    public string CodigoSKU { get; set; } = string.Empty;

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    [Range(0.01, 1000000.00, ErrorMessage = "O preço base deve ser maior que zero.")]
    public decimal PrecoBase { get; set; }

    [Range(0, 10000)]
    public int EstoqueMinimoPadrao { get; set; } = 10;

    public bool EServico { get; set; } = false;
}

public class UpdateProdutoDto
{
    [Required]
    public int CategoriaId { get; set; }

    public int? FornecedorId { get; set; }

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    [Range(0.01, 1000000.00)]
    public decimal PrecoBase { get; set; }

    [Range(0, 10000)]
    public int EstoqueMinimoPadrao { get; set; }

    public bool EServico { get; set; }
    public StatusProduto Status { get; set; }
}

public class ProdutoResponseDto
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public string CategoriaNome { get; set; } = string.Empty;
    public int? FornecedorId { get; set; }
    public string? FornecedorNome { get; set; }
    public string CodigoSKU { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal PrecoBase { get; set; }
    public int EstoqueMinimoPadrao { get; set; }
    public bool EServico { get; set; }
    public StatusProduto Status { get; set; }
    public string StatusDescricao => Status.ToString();
    public DateTime DataCadastro { get; set; }
}
