using System.Linq.Expressions;

namespace projectFamilyTasks.Infrastructure.Persistence;

public static class SortingExtensions
{
    public static IOrderedQueryable<T> OrderByDirection<T, TKey>(
        this IQueryable<T> query, Expression<Func<T, TKey>> key, bool descending)
    {
        return descending ? query.OrderByDescending(key) : query.OrderBy(key);
    }
}
