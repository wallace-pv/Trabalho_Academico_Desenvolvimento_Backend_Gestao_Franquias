using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Categorias;

public class CreateCategoriaDto
{
    [Required(ErrorMessage = "O nome da categoria é obrigatório.")]
    [StringLength(80, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }
}

public class UpdateCategoriaDto
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
}

public class CategoriaResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public bool Ativo { get; set; }
    public int QuantidadeProdutos { get; set; }
}
