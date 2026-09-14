namespace Franquias.Api.DTOs.Common;

public class PagedResultDto<T>
{
    public int PaginaAtual { get; set; }
    public int ItensPorPagina { get; set; }
    public int TotalItens { get; set; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalItens / (ItensPorPagina > 0 ? ItensPorPagina : 10));
    public IEnumerable<T> Itens { get; set; } = new List<T>();
}
