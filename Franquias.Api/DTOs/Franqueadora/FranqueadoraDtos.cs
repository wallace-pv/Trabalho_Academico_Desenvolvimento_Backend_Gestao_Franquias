using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Franqueadora;

public class FranqueadoraDto
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public decimal PercentualPadraoRoyalty { get; set; }
    public DateTime DataFundacao { get; set; }
    public bool Ativo { get; set; }
}

public class UpdateFranqueadoraDto
{
    [Required]
    public string NomeFantasia { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Telefone { get; set; } = string.Empty;

    [Range(0, 100, ErrorMessage = "O percentual padrão deve estar entre 0 e 100.")]
    public decimal PercentualPadraoRoyalty { get; set; }
}
