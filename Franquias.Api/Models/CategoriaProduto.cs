using System.Text.Json.Serialization;

namespace Franquias.Api.Models;

public class CategoriaProduto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; } = true;

    [JsonIgnore]
    public ICollection<ProdutoServico> Produtos { get; set; } = new List<ProdutoServico>();
}
