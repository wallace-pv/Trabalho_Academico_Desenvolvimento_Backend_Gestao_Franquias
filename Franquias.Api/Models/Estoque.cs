using System.Text.Json.Serialization;

namespace Franquias.Api.Models;

public class Estoque
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public int ProdutoId { get; set; }
    public int QuantidadeDisponivel { get; set; }
    public int QuantidadeMinima { get; set; } = 10;
    public DateTime UltimaAtualizacao { get; set; } = DateTime.UtcNow;

    // Relacionamentos de navegação
    public UnidadeFranqueada? Unidade { get; set; }
    public ProdutoServico? Produto { get; set; }
}
