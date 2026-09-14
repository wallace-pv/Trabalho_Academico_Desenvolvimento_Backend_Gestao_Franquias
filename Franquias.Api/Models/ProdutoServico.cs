using System.Text.Json.Serialization;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class ProdutoServico
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public int? FornecedorId { get; set; }

    public string CodigoSKU { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal PrecoBase { get; set; }
    public int EstoqueMinimoPadrao { get; set; } = 10;
    public bool EServico { get; set; } = false;
    public StatusProduto Status { get; set; } = StatusProduto.Ativo;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    // Relacionamentos de navegação
    public CategoriaProduto? Categoria { get; set; }
    public Fornecedor? Fornecedor { get; set; }

    [JsonIgnore]
    public ICollection<Estoque> Estoques { get; set; } = new List<Estoque>();

    [JsonIgnore]
    public ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();

    [JsonIgnore]
    public ICollection<ItemVenda> ItensVenda { get; set; } = new List<ItemVenda>();
}
