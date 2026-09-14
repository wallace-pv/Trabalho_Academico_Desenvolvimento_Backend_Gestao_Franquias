namespace Franquias.Api.Models.Enums;

public enum PerfilUsuario
{
    AdminFranqueadora = 1,
    GestorUnidade = 2,
    Operador = 3
}

public enum SituacaoUnidade
{
    Ativa = 1,
    Inativa = 2
}

public enum StatusProduto
{
    Ativo = 1,
    Inativo = 2
}

public enum TipoMovimentacaoEstoque
{
    Entrada = 1,
    SaidaVenda = 2,
    AjustePositivo = 3,
    AjusteNegativo = 4
}

public enum StatusVenda
{
    Concluida = 1,
    Cancelada = 2
}

public enum StatusRoyalty
{
    Pendente = 1,
    Pago = 2,
    Atrasado = 3
}

public enum CategoriaChamado
{
    Financeiro = 1,
    Estoque = 2,
    SuporteTI = 3,
    Marketing = 4,
    Geral = 5
}

public enum PrioridadeChamado
{
    Baixa = 1,
    Media = 2,
    Alta = 3,
    Critica = 4
}

public enum StatusChamado
{
    Aberto = 1,
    EmAndamento = 2,
    Resolvido = 3,
    Cancelado = 4
}
