using System.ComponentModel.DataAnnotations;
using Franquias.Api.Models.Enums;

namespace Franquias.Api.DTOs.Unidades;

public class CreateUnidadeDto
{
    [Required]
    public int FranqueadoraId { get; set; }

    [Required]
    public CreateResponsavelDto Responsavel { get; set; } = null!;

    [Required(ErrorMessage = "O código da unidade é obrigatório.")]
    public string CodigoUnidade { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome da unidade é obrigatório.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CNPJ é obrigatório.")]
    [RegularExpression(@"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}\-?\d{2}$", ErrorMessage = "Formato de CNPJ inválido.")]
    public string CNPJ { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    public string Logradouro { get; set; } = string.Empty;

    [Required]
    public string Numero { get; set; } = string.Empty;

    public string? Complemento { get; set; }

    [Required]
    public string Bairro { get; set; } = string.Empty;

    [Required]
    public string Cidade { get; set; } = string.Empty;

    [Required]
    public string Estado { get; set; } = string.Empty;

    [Required]
    public string CEP { get; set; } = string.Empty;

    public DateTime DataInicioOperacao { get; set; } = DateTime.UtcNow;

    [Range(0, 100)]
    public decimal PercentualRoyalty { get; set; } = 5.0m;
}

public class CreateResponsavelDto
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string CPF { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Telefone { get; set; } = string.Empty;

    public DateTime? DataNascimento { get; set; }
}

public class UpdateUnidadeDto
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Telefone { get; set; } = string.Empty;

    public string Logradouro { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public string? Complemento { get; set; }
    public string Bairro { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string CEP { get; set; } = string.Empty;

    public SituacaoUnidade Situacao { get; set; }

    [Range(0, 100)]
    public decimal PercentualRoyalty { get; set; }
}

public class UnidadeResponseDto
{
    public int Id { get; set; }
    public int FranqueadoraId { get; set; }
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
    public SituacaoUnidade Situacao { get; set; }
    public string SituacaoDescricao => Situacao.ToString();
    public decimal PercentualRoyalty { get; set; }
    public DateTime DataCadastro { get; set; }
    public ResponsavelDto Responsavel { get; set; } = null!;
}

public class ResponsavelDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
}
