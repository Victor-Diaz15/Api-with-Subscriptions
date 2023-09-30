using WebApiAutores.DTOs;

namespace WebApiAutores.Utilidades
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> query, PaginacionDto paginacionDto)
        {
            return query
                .Skip((paginacionDto.Pagina - 1) * paginacionDto.RecordsPorPagina)
                .Take(paginacionDto.RecordsPorPagina);
        }
    }
}
