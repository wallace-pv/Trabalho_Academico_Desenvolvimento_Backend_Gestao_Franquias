using System.ComponentModel.DataAnnotations;

namespace Franquias.Api.DTOs.Fornecedores;

public class CreateFornecedorDto
{
    [Required]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required]
    public string NomeFantasia { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^\d{2}\.?\d{3}\.?\d{3}\/?\d{4}\-?\d{2}$", ErrorMessage = "Formato de CNPJ inválido.")]
    public string CNPJ { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Telefone { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class UpdateFornecedorDto
{
    [Required]
    public string RazaoSocial { get; set; } = string.Empty;

    [Required]
    public string NomeFantasia { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Telefone { get; set; } = string.Empty;

    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Ativo { get; set; }
}

public class FornecedorResponseDto
{
    public int Id { get; set; }
    public string RazaoSocial { get; set; } = string.Empty;
    public string NomeFantasia { get; set; } = string.Empty;
    public string CNPJ { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Cidade { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
}
