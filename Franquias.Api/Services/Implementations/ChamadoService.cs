using Franquias.Api.Data;
using Franquias.Api.DTOs.Chamados;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories.Interfaces;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class ChamadoService : IChamadoService
{
    private readonly IChamadoRepository _chamadoRepository;
    private readonly AppDbContext _context;

    public ChamadoService(IChamadoRepository chamadoRepository, AppDbContext context)
    {
        _chamadoRepository = chamadoRepository;
        _context = context;
    }

    public async Task<ChamadoResponseDto> AbrirChamadoAsync(CreateChamadoDto dto, int usuarioId)
    {
        var unidade = await _context.Unidades.FindAsync(dto.UnidadeId);
        if (unidade == null)
            throw new KeyNotFoundException($"Unidade com ID {dto.UnidadeId} não encontrada.");

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
            throw new KeyNotFoundException($"Usuário com ID {usuarioId} não encontrado.");

        var chamado = new ChamadoSuporte
        {
            UnidadeId = dto.UnidadeId,
            UsuarioAberturaId = usuarioId,
            Titulo = dto.Titulo.Trim(),
            Descricao = dto.Descricao.Trim(),
            Categoria = dto.Categoria,
            Prioridade = dto.Prioridade,
            Status = StatusChamado.Aberto,
            DataAbertura = DateTime.UtcNow
        };

        await _chamadoRepository.AddAsync(chamado);
        await _chamadoRepository.SaveChangesAsync();

        var chamadoCriado = await _chamadoRepository.GetWithDetailsAsync(chamado.Id);
        return MapToResponse(chamadoCriado!);
    }

    public async Task<ChamadoResponseDto> AtualizarStatusChamadoAsync(int id, UpdateChamadoDto dto)
    {
        var chamado = await _chamadoRepository.GetWithDetailsAsync(id);
        if (chamado == null)
            throw new KeyNotFoundException($"Chamado com ID {id} não encontrado.");

        chamado.Status = dto.Status;
        chamado.DataAtualizacao = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(dto.RespostaSolucao))
        {
            chamado.RespostaSolucao = dto.RespostaSolucao.Trim();
        }

        if (dto.Status == StatusChamado.Resolvido || dto.Status == StatusChamado.Cancelado)
        {
            chamado.DataEncerramento = DateTime.UtcNow;
        }

        _chamadoRepository.Update(chamado);
        await _chamadoRepository.SaveChangesAsync();

        return MapToResponse(chamado);
    }

    public async Task<ChamadoResponseDto?> ObterPorIdAsync(int id)
    {
        var chamado = await _chamadoRepository.GetWithDetailsAsync(id);
        return chamado == null ? null : MapToResponse(chamado);
    }

    public async Task<IEnumerable<ChamadoResponseDto>> ListarChamadosAsync(int? unidadeId = null, StatusChamado? status = null, PrioridadeChamado? prioridade = null)
    {
        var chamados = await _chamadoRepository.GetChamadosAsync(unidadeId, status, prioridade);
        return chamados.Select(MapToResponse);
    }

    private static ChamadoResponseDto MapToResponse(ChamadoSuporte c)
    {
        return new ChamadoResponseDto
        {
            Id = c.Id,
            UnidadeId = c.UnidadeId,
            UnidadeNome = c.Unidade?.Nome ?? string.Empty,
            UsuarioAberturaId = c.UsuarioAberturaId,
            UsuarioAberturaNome = c.UsuarioAbertura?.Nome ?? string.Empty,
            Titulo = c.Titulo,
            Descricao = c.Descricao,
            Categoria = c.Categoria,
            Prioridade = c.Prioridade,
            Status = c.Status,
            DataAbertura = c.DataAbertura,
            DataAtualizacao = c.DataAtualizacao,
            DataEncerramento = c.DataEncerramento,
            RespostaSolucao = c.RespostaSolucao
        };
    }
}
