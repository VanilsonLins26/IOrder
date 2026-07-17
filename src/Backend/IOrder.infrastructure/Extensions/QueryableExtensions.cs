

using Microsoft.EntityFrameworkCore;

namespace IOrder.infrastructure.Extensions;

public static class QueryableExtensions
{
    public static async Task<(List<T> Items, int TotalCount)> ToPaginatedTupleAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, count);
    }
}