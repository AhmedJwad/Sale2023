using Sale.Shared.DTOs;

namespace Sale.Api.Helpers
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable,
          PaginationDTO pagination)
        {
            return queryable
                .Skip((pagination.page - 1) * pagination.REcordNumber)
                .Take(pagination.REcordNumber);
        }

    }
}
