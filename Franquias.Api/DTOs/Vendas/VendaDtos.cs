using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs.Vendas;

public class CreateVendaDto
{
    [Required]
    public int UnidadeId { get; set; }

    public string? Observacao { get; set; }

    [Required(ErrorMessage = "A venda deve conter itens.")]
    [MinLength(1, ErrorMessage = "A venda deve possuir pelo menos um item.")]
    public List<CreateItemVendaDto> Itens { get; set; } = new();
}

public class CreateItemVendaDto
{
    [Required]
    public int ProdutoId { get; set; }

    [Range(1, 10000, ErrorMessage = "A quantidade vendida deve ser de pelo menos 1 unidade.")]
    public int Quantidade { get; set; }
}

public class VendaResponseDto
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public string UnidadeNome { get; set; } = string.Empty;
    public int UsuarioId { get; set; }
    public string UsuarioNome { get; set; } = string.Empty;
    public string CodigoVenda { get; set; } = string.Empty;
    public DateTime DataHora { get; set; }
    public decimal ValorTotal { get; set; }
    public StatusVenda Status { get; set; }
    public string StatusDescricao => Status.ToString();
    public string? Observacao { get; set; }
    public List<ItemVendaResponseDto> Itens { get; set; } = new();
}

public class ItemVendaResponseDto
{
    public int Id { get; set; }
    public int ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public string ProdutoSKU { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
