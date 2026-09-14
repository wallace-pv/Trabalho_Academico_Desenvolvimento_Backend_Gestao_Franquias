using Franquias.Api.Data;
using Franquias.Api.DTOs.Royalties;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories.Interfaces;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class RoyaltyService : IRoyaltyService
{
    private readonly IRoyaltyRepository _royaltyRepository;
    private readonly AppDbContext _context;

    public RoyaltyService(IRoyaltyRepository royaltyRepository, AppDbContext context)
    {
        _royaltyRepository = royaltyRepository;
        _context = context;
    }

    public async Task<RoyaltyResponseDto> ApurarRoyaltyAsync(GerarRoyaltyDto dto)
    {
        var unidade = await _context.Unidades.FindAsync(dto.UnidadeId);
        if (unidade == null)
            throw new KeyNotFoundException($"Unidade com ID {dto.UnidadeId} não encontrada.");

        var dataInicio = new DateTime(dto.AnoReferencia, dto.MesReferencia, 1, 0, 0, 0, DateTimeKind.Utc);
        var diasNoMes = DateTime.DaysInMonth(dto.AnoReferencia, dto.MesReferencia);
        var dataFim = new DateTime(dto.AnoReferencia, dto.MesReferencia, diasNoMes, 23, 59, 59, DateTimeKind.Utc);

        // Somar faturamento de vendas concluídas no período
        var vendasPeriodo = await _context.Vendas
            .Where(v => v.UnidadeId == dto.UnidadeId &&
                        v.Status == StatusVenda.Concluida &&
                        v.DataHora >= dataInicio &&
                        v.DataHora <= dataFim)
            .Select(v => v.ValorTotal)
            .ToListAsync();
        var faturamentoTotal = vendasPeriodo.Sum();

        decimal percentual = unidade.PercentualRoyalty;
        decimal valorRoyalty = Math.Round(faturamentoTotal * (percentual / 100m), 2);

        // Vencimento padrão: dia 10 do mês subsequente
        var dataVencimento = dataInicio.AddMonths(1).AddDays(9);

        // Verificar se já existe registro de royalty para este mês/ano
        var existente = await _royaltyRepository.GetByUnidadeEMesAnoAsync(dto.UnidadeId, dto.MesReferencia, dto.AnoReferencia);

        if (existente != null)
        {
            if (existente.Status == StatusRoyalty.Pago)
            {
                throw new BadHttpRequestException($"O royalty da competência {dto.MesReferencia:D2}/{dto.AnoReferencia} já foi pago e não pode ser recalculado.");
            }

            existente.FaturamentoBase = faturamentoTotal;
            existente.PercentualCobrado = percentual;
            existente.ValorRoyalty = valorRoyalty;
            existente.DataApuracao = DateTime.UtcNow;
            existente.DataVencimento = dataVencimento;
            if (!string.IsNullOrWhiteSpace(dto.Observacao))
                existente.Observacao = dto.Observacao;

            _royaltyRepository.Update(existente);
            await _royaltyRepository.SaveChangesAsync();
            return MapToResponse(existente, unidade);
        }

        var novoRoyalty = new Royalty
        {
            UnidadeId = dto.UnidadeId,
            MesReferencia = dto.MesReferencia,
            AnoReferencia = dto.AnoReferencia,
            FaturamentoBase = faturamentoTotal,
            PercentualCobrado = percentual,
            ValorRoyalty = valorRoyalty,
            Status = StatusRoyalty.Pendente,
            DataVencimento = dataVencimento,
            DataApuracao = DateTime.UtcNow,
            Observacao = dto.Observacao
        };

        await _royaltyRepository.AddAsync(novoRoyalty);
        await _royaltyRepository.SaveChangesAsync();

        return MapToResponse(novoRoyalty, unidade);
    }

    public async Task<RoyaltyResponseDto> RegistrarPagamentoAsync(int id, PagarRoyaltyDto dto)
    {
        var royalty = await _context.Royalties.Include(r => r.Unidade).FirstOrDefaultAsync(r => r.Id == id);
        if (royalty == null)
            throw new KeyNotFoundException($"Registro de royalty com ID {id} não encontrado.");

        if (royalty.Status == StatusRoyalty.Pago)
            throw new BadHttpRequestException("Este royalty já se encontra com situação QUITADA / PAGA.");

        royalty.Status = StatusRoyalty.Pago;
        royalty.DataPagamento = dto.DataPagamento;
        if (!string.IsNullOrWhiteSpace(dto.Observacao))
            royalty.Observacao = $"{royalty.Observacao} | Pagamento: {dto.Observacao}".Trim(' ', '|');

        _royaltyRepository.Update(royalty);
        await _royaltyRepository.SaveChangesAsync();

        return MapToResponse(royalty, royalty.Unidade!);
    }

    public async Task<IEnumerable<RoyaltyResponseDto>> ListarRoyaltiesAsync(int? unidadeId = null, int? ano = null, StatusRoyalty? status = null)
    {
        var lista = await _royaltyRepository.GetRoyaltiesComDetalhesAsync(unidadeId, ano, status);
        return lista.Select(r => MapToResponse(r, r.Unidade!));
    }

    private static RoyaltyResponseDto MapToResponse(Royalty r, UnidadeFranqueada u)
    {
        return new RoyaltyResponseDto
        {
            Id = r.Id,
            UnidadeId = r.UnidadeId,
            UnidadeNome = u.Nome,
            CodigoUnidade = u.CodigoUnidade,
            MesReferencia = r.MesReferencia,
            AnoReferencia = r.AnoReferencia,
            FaturamentoBase = r.FaturamentoBase,
            PercentualCobrado = r.PercentualCobrado,
            ValorRoyalty = r.ValorRoyalty,
            Status = r.Status,
            DataVencimento = r.DataVencimento,
            DataPagamento = r.DataPagamento,
            DataApuracao = r.DataApuracao,
            Observacao = r.Observacao
        };
    }
}
