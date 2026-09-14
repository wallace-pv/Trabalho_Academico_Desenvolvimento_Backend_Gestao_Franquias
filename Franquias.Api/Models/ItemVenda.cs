using System.Text.Json.Serialization;

namespace Franquias.Api.Models;

public class ItemVenda
{
    public int Id { get; set; }
    public int VendaId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }

    // Relacionamentos de navegação
    [JsonIgnore]
    public Venda? Venda { get; set; }
    public ProdutoServico? Produto { get; set; }
}
