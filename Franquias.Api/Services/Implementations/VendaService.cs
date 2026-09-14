using Franquias.Api.Data;
using Franquias.Api.DTOs.Vendas;
using Franquias.Api.Models;
using Franquias.Api.Models.Enums;
using Franquias.Api.Repositories.Interfaces;
using Franquias.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Franquias.Api.Services.Implementations;

public class VendaService : IVendaService
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IEstoqueService _estoqueService;
    private readonly AppDbContext _context;

    public VendaService(IVendaRepository vendaRepository, IEstoqueService estoqueService, AppDbContext context)
    {
        _vendaRepository = vendaRepository;
        _estoqueService = estoqueService;
        _context = context;
    }

    public async Task<VendaResponseDto> RegistrarVendaAsync(CreateVendaDto dto, int usuarioId)
    {
        // 1. Validação de Unidade: deve existir e estar ATIVA
        var unidade = await _context.Unidades.FindAsync(dto.UnidadeId);
        if (unidade == null)
        {
            throw new KeyNotFoundException($"Unidade com ID {dto.UnidadeId} não encontrada.");
        }

        if (unidade.Situacao != SituacaoUnidade.Ativa)
        {
            throw new BadHttpRequestException($"Não é permitido registrar vendas para a unidade '{unidade.Nome}' pois ela se encontra INATIVA.");
        }

        // 2. Validação de itens: deve ter pelo menos um item
        if (dto.Itens == null || !dto.Itens.Any())
        {
            throw new BadHttpRequestException("A venda deve possuir pelo menos um item.");
        }

        // 3. Validação do usuário
        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
        {
            throw new KeyNotFoundException($"Usuário com ID {usuarioId} não encontrado.");
        }

        // Se o usuário for Gestor ou Operador, validar se a venda pertence à sua unidade
        if (usuario.Perfil != PerfilUsuario.AdminFranqueadora && usuario.UnidadeId.HasValue && usuario.UnidadeId.Value != dto.UnidadeId)
        {
            throw new BadHttpRequestException("O usuário só pode registrar vendas para a sua própria unidade franqueada.");
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            decimal valorTotal = 0;
            var itensVenda = new List<ItemVenda>();

            // 4. Buscar e validar produtos e estoque antes de persistir
            foreach (var itemDto in dto.Itens)
            {
                if (itemDto.Quantidade <= 0)
                {
                    throw new BadHttpRequestException("A quantidade de cada item deve ser estritamente maior que zero.");
                }

                var produto = await _context.Produtos.FindAsync(itemDto.ProdutoId);
                if (produto == null)
                {
                    throw new KeyNotFoundException($"Produto com ID {itemDto.ProdutoId} não encontrado.");
                }

                if (produto.Status != StatusProduto.Ativo)
                {
                    throw new BadHttpRequestException($"O produto '{produto.Nome}' está inativo e não pode ser comercializado.");
                }

                decimal precoUnitario = produto.PrecoBase;
                decimal subtotal = precoUnitario * itemDto.Quantidade;
                valorTotal += subtotal;

                itensVenda.Add(new ItemVenda
                {
                    ProdutoId = produto.Id,
                    Quantidade = itemDto.Quantidade,
                    PrecoUnitario = precoUnitario,
                    Subtotal = subtotal
                });

                // Se não for serviço, baixa o estoque via IEstoqueService (que impede saldo negativo)
                if (!produto.EServico)
                {
                    await _estoqueService.MovimentarEstoqueAsync(new DTOs.Estoque.MovimentarEstoqueDto
                    {
                        UnidadeId = dto.UnidadeId,
                        ProdutoId = produto.Id,
                        Tipo = TipoMovimentacaoEstoque.SaidaVenda,
                        Quantidade = itemDto.Quantidade,
                        Observacao = $"Venda realizada na unidade {unidade.Nome}"
                    }, usuarioId);
                }
            }

            // Gerar código único de venda
            var codigoVenda = $"VND-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var venda = new Venda
            {
                UnidadeId = dto.UnidadeId,
                UsuarioId = usuarioId,
                CodigoVenda = codigoVenda,
                DataHora = DateTime.UtcNow,
                ValorTotal = valorTotal,
                Status = StatusVenda.Concluida,
                Observacao = dto.Observacao?.Trim(),
                Itens = itensVenda
            };

            await _vendaRepository.AddAsync(venda);
            await _vendaRepository.SaveChangesAsync();

            await transaction.CommitAsync();

            var vendaCompleta = await _vendaRepository.GetWithDetailsAsync(venda.Id);
            return MapToResponse(vendaCompleta!);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<VendaResponseDto?> ObterPorIdAsync(int id)
    {
        var venda = await _vendaRepository.GetWithDetailsAsync(id);
        return venda == null ? null : MapToResponse(venda);
    }

    public async Task<IEnumerable<VendaResponseDto>> ListarVendasAsync(int? unidadeId, DateTime? dataInicio, DateTime? dataFim)
    {
        var vendas = await _vendaRepository.GetVendasPorPeriodoAsync(unidadeId, dataInicio, dataFim);
        return vendas.Select(MapToResponse);
    }

    private static VendaResponseDto MapToResponse(Venda v)
    {
        return new VendaResponseDto
        {
            Id = v.Id,
            UnidadeId = v.UnidadeId,
            UnidadeNome = v.Unidade?.Nome ?? string.Empty,
            UsuarioId = v.UsuarioId,
            UsuarioNome = v.Usuario?.Nome ?? string.Empty,
            CodigoVenda = v.CodigoVenda,
            DataHora = v.DataHora,
            ValorTotal = v.ValorTotal,
            Status = v.Status,
            Observacao = v.Observacao,
            Itens = v.Itens.Select(i => new ItemVendaResponseDto
            {
                Id = i.Id,
                ProdutoId = i.ProdutoId,
                ProdutoNome = i.Produto?.Nome ?? string.Empty,
                ProdutoSKU = i.Produto?.CodigoSKU ?? string.Empty,
                Quantidade = i.Quantidade,
                PrecoUnitario = i.PrecoUnitario,
                Subtotal = i.Subtotal
            }).ToList()
        };
    }
}
