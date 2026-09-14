using System.Text.Json.Serialization;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class MovimentacaoEstoque
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public int ProdutoId { get; set; }
    public int? UsuarioId { get; set; }
    public TipoMovimentacaoEstoque Tipo { get; set; }
    public int Quantidade { get; set; }
    public int SaldoAnterior { get; set; }
    public int SaldoAtual { get; set; }
    public string? Observacao { get; set; }
    public DateTime DataHora { get; set; } = DateTime.UtcNow;

    // Relacionamentos de navegação
    public UnidadeFranqueada? Unidade { get; set; }
    public ProdutoServico? Produto { get; set; }
    public Usuario? Usuario { get; set; }
}
