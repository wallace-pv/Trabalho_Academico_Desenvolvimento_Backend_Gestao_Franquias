using System.Text.Json.Serialization;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [JsonIgnore]
    public string SenhaHash { get; set; } = string.Empty;

    public PerfilUsuario Perfil { get; set; } = PerfilUsuario.Operador;
    public int? UnidadeId { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
    public DateTime? UltimoLogin { get; set; }

    // Relacionamento de navegação
    public UnidadeFranqueada? Unidade { get; set; }

    [JsonIgnore]
    public ICollection<Venda> Vendas { get; set; } = new List<Venda>();

    [JsonIgnore]
    public ICollection<ChamadoSuporte> ChamadosAbertos { get; set; } = new List<ChamadoSuporte>();
}
