using Franquias.Api.Data;
using Franquias.Api.DTOs.Unidades;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories.Interfaces;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class UnidadeService : IUnidadeService
{
    private readonly IUnidadeRepository _unidadeRepository;
    private readonly AppDbContext _context;

    public UnidadeService(IUnidadeRepository unidadeRepository, AppDbContext context)
    {
        _unidadeRepository = unidadeRepository;
        _context = context;
    }

    public async Task<UnidadeResponseDto> CadastrarUnidadeAsync(CreateUnidadeDto dto)
    {
        var cnpjLimpo = dto.CNPJ.Trim();
        if (await _unidadeRepository.ExistsCnpjAsync(cnpjLimpo))
        {
            throw new BadHttpRequestException($"Já existe uma unidade franqueada cadastrada com o CNPJ '{dto.CNPJ}'.");
        }

        var franqueadoraExists = await _context.Franqueadoras.AnyAsync(f => f.Id == dto.FranqueadoraId);
        if (!franqueadoraExists)
        {
            throw new BadHttpRequestException("A franqueadora vinculada não foi localizada.");
        }

        // Cadastra o responsável da unidade
        var responsavel = new ResponsavelFranqueado
        {
            Nome = dto.Responsavel.Nome.Trim(),
            CPF = dto.Responsavel.CPF.Trim(),
            Email = dto.Responsavel.Email.Trim().ToLower(),
            Telefone = dto.Responsavel.Telefone.Trim(),
            DataNascimento = dto.Responsavel.DataNascimento,
            DataCadastro = DateTime.UtcNow
        };
        await _context.Responsaveis.AddAsync(responsavel);
        await _context.SaveChangesAsync();

        var unidade = new UnidadeFranqueada
        {
            FranqueadoraId = dto.FranqueadoraId,
            ResponsavelId = responsavel.Id,
            CodigoUnidade = dto.CodigoUnidade.Trim().ToUpper(),
            Nome = dto.Nome.Trim(),
            CNPJ = cnpjLimpo,
            Email = dto.Email.Trim().ToLower(),
            Telefone = dto.Telefone.Trim(),
            Logradouro = dto.Logradouro.Trim(),
            Numero = dto.Numero.Trim(),
            Complemento = dto.Complemento?.Trim(),
            Bairro = dto.Bairro.Trim(),
            Cidade = dto.Cidade.Trim(),
            Estado = dto.Estado.Trim().ToUpper(),
            CEP = dto.CEP.Trim(),
            DataInicioOperacao = dto.DataInicioOperacao,
            Situacao = SituacaoUnidade.Ativa,
            PercentualRoyalty = dto.PercentualRoyalty,
            DataCadastro = DateTime.UtcNow
        };

        await _unidadeRepository.AddAsync(unidade);
        await _unidadeRepository.SaveChangesAsync();

        return MapToResponse(unidade, responsavel);
    }

    public async Task<UnidadeResponseDto> AtualizarUnidadeAsync(int id, UpdateUnidadeDto dto)
    {
        var unidade = await _unidadeRepository.GetWithDetailsAsync(id);
        if (unidade == null)
            throw new KeyNotFoundException($"Unidade com ID {id} não encontrada.");

        unidade.Nome = dto.Nome.Trim();
        unidade.Email = dto.Email.Trim().ToLower();
        unidade.Telefone = dto.Telefone.Trim();
        unidade.Logradouro = dto.Logradouro.Trim();
        unidade.Numero = dto.Numero.Trim();
        unidade.Complemento = dto.Complemento?.Trim();
        unidade.Bairro = dto.Bairro.Trim();
        unidade.Cidade = dto.Cidade.Trim();
        unidade.Estado = dto.Estado.Trim().ToUpper();
        unidade.CEP = dto.CEP.Trim();
        unidade.Situacao = dto.Situacao;
        unidade.PercentualRoyalty = dto.PercentualRoyalty;

        _unidadeRepository.Update(unidade);
        await _unidadeRepository.SaveChangesAsync();

        return MapToResponse(unidade, unidade.Responsavel!);
    }

    public async Task<UnidadeResponseDto?> ObterPorIdAsync(int id)
    {
        var unidade = await _unidadeRepository.GetWithDetailsAsync(id);
        return unidade == null ? null : MapToResponse(unidade, unidade.Responsavel!);
    }

    public async Task<IEnumerable<UnidadeResponseDto>> ListarUnidadesAsync(SituacaoUnidade? situacao = null, string? termoBusca = null)
    {
        var lista = await _unidadeRepository.GetAllWithDetailsAsync(situacao, termoBusca);
        return lista.Select(u => MapToResponse(u, u.Responsavel!));
    }

    public async Task<bool> InativarUnidadeAsync(int id)
    {
        var unidade = await _unidadeRepository.GetByIdAsync(id);
        if (unidade == null)
            throw new KeyNotFoundException($"Unidade com ID {id} não encontrada.");

        unidade.Situacao = SituacaoUnidade.Inativa;
        _unidadeRepository.Update(unidade);
        return await _unidadeRepository.SaveChangesAsync() > 0;
    }

    private static UnidadeResponseDto MapToResponse(UnidadeFranqueada u, ResponsavelFranqueado r)
    {
        return new UnidadeResponseDto
        {
            Id = u.Id,
            FranqueadoraId = u.FranqueadoraId,
            CodigoUnidade = u.CodigoUnidade,
            Nome = u.Nome,
            CNPJ = u.CNPJ,
            Email = u.Email,
            Telefone = u.Telefone,
            Logradouro = u.Logradouro,
            Numero = u.Numero,
            Complemento = u.Complemento,
            Bairro = u.Bairro,
            Cidade = u.Cidade,
            Estado = u.Estado,
            CEP = u.CEP,
            DataInicioOperacao = u.DataInicioOperacao,
            Situacao = u.Situacao,
            PercentualRoyalty = u.PercentualRoyalty,
            DataCadastro = u.DataCadastro,
            Responsavel = new ResponsavelDto
            {
                Id = r.Id,
                Nome = r.Nome,
                CPF = r.CPF,
                Email = r.Email,
                Telefone = r.Telefone
            }
        };
    }
}
