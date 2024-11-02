namespace RpsApi.Utils;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

public static class QueryExtensions
{
    public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> query, bool? ascending, string? sortBy)
    {
        if (string.IsNullOrWhiteSpace(sortBy) || ascending == null)
        {
            return query;
        }

        var propertyInfo = typeof(T).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (propertyInfo == null)
        {
            throw new ArgumentException($"Property '{sortBy}' does not exist on type '{typeof(T).Name}'");
        }

        var parameter = Expression.Parameter(typeof(T), "x");
        var propertyAccess = Expression.MakeMemberAccess(parameter, propertyInfo);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var methodName = ascending.Value ? "OrderBy" : "OrderByDescending";
        var resultExpression = Expression.Call(typeof(Queryable), methodName, new Type[] { typeof(T), propertyInfo.PropertyType },
            query.Expression, Expression.Quote(orderByExpression));

        return query.Provider.CreateQuery<T>(resultExpression);
    }
    
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        var skip = (pageNumber - 1) * pageSize;
        return query.Skip(skip).Take(pageSize);
    }
}