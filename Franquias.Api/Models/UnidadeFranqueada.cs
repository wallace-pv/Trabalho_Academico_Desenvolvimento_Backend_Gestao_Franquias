using System.Text.Json.Serialization;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class UnidadeFranqueada
{
    public int Id { get; set; }
    public int FranqueadoraId { get; set; }
    public int ResponsavelId { get; set; }

    public string CodigoUnidade { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;

    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;

    public DateTime DataInicioOperacao { get; set; }
    public SituacaoUnidade Situacao { get; set; } = SituacaoUnidade.Ativa;
    public decimal PercentualRoyalty { get; set; } = 5.0m;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    // Relacionamentos de navegação
    public Franqueadora? Franqueadora { get; set; }
    public ResponsavelFranqueado? Responsavel { get; set; }

    [JsonIgnore]
    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

    [JsonIgnore]
    public ICollection<Estoque> Estoques { get; set; } = new List<Estoque>();

    [JsonIgnore]
    public ICollection<MovimentacaoEstoque> MovimentacoesEstoque { get; set; } = new List<MovimentacaoEstoque>();

    [JsonIgnore]
    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();

    [JsonIgnore]
    public ICollection<Royalty> Royalties { get; set; } = new List<Royalty>();

    [JsonIgnore]
    public ICollection<ChamadoSuporte> Chamados { get; set; } = new List<ChamadoSuporte>();
}
