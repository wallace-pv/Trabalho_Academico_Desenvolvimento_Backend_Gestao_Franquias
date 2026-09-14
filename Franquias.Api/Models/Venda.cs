using System.Text.Json.Serialization;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class Venda
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public int UsuarioId { get; set; }
    public string CodigoVenda { get; set; } = string.Empty;
    public DateTime DataHora { get; set; } = DateTime.UtcNow;
    public decimal ValorTotal { get; set; }
    public StatusVenda Status { get; set; } = StatusVenda.Concluida;
    public string? Observacao { get; set; }

    // Relacionamentos de navegação
    public UnidadeFranqueada? Unidade { get; set; }
    public Usuario? Usuario { get; set; }
    public ICollection<ItemVenda> Itens { get; set; } = new List<ItemVenda>();
}
