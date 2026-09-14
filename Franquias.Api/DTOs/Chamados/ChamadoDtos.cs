using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs.Chamados;

public class CreateChamadoDto
{
    [Required]
    public int UnidadeId { get; set; }

    [Required(ErrorMessage = "O título do chamado é obrigatório.")]
    [StringLength(150, MinimumLength = 5)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição do chamado é obrigatória.")]
    public string Descricao { get; set; } = string.Empty;

    [Required]
    public CategoriaChamado Categoria { get; set; }

    [Required]
    public PrioridadeChamado Prioridade { get; set; }
}

public class UpdateChamadoDto
{
    [Required]
    public StatusChamado Status { get; set; }

    public string? RespostaSolucao { get; set; }
}

public class ChamadoResponseDto
{
    public int Id { get; set; }
    public int UnidadeId { get; set; }
    public string UnidadeNome { get; set; } = string.Empty;
    public int UsuarioAberturaId { get; set; }
    public string UsuarioAberturaNome { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public CategoriaChamado Categoria { get; set; }
    public string CategoriaDescricao => Categoria.ToString();
    public PrioridadeChamado Prioridade { get; set; }
    public string PrioridadeDescricao => Prioridade.ToString();
    public StatusChamado Status { get; set; }
    public string StatusDescricao => Status.ToString();
    public DateTime DataAbertura { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataEncerramento { get; set; }
    public string? RespostaSolucao { get; set; }
}
