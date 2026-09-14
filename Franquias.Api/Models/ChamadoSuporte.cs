using Franquias.Api.Models.Enums;

namespace Franquias.Api.Models;

public class ChamadoSuporte
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public int UsuarioAberturaId { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public CategoriaChamado Categoria { get; set; } = CategoriaChamado.Geral;
    public PrioridadeChamado Prioridade { get; set; } = PrioridadeChamado.Media;
    public StatusChamado Status { get; set; } = StatusChamado.Aberto;
    public DateTime DataAbertura { get; set; } = DateTime.UtcNow;
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataEncerramento { get; set; }
    public string? RespostaSolucao { get; set; }

    // Relacionamentos de navegação
    public UnidadeFranqueada? Unidade { get; set; }
    public Usuario? UsuarioAbertura { get; set; }
}
