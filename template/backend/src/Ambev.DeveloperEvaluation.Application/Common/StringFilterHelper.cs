using System.Linq.Expressions;

namespace Ambev.DeveloperEvaluation.Application.Common;

public static class StringFilterHelper
{
    public static IQueryable<T> ApplyStringFilter<T>(
        this IQueryable<T> query,
        Expression<Func<T, string>> propertySelector,
        string? filterValue)
    {
        if (string.IsNullOrWhiteSpace(filterValue))
            return query;

        var parameter = propertySelector.Parameters[0];
        var property = propertySelector.Body;

        Expression filterExpression;

        if (filterValue.StartsWith('*') && filterValue.EndsWith('*'))
        {
            var value = filterValue.Trim('*');
            var method = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
            filterExpression = Expression.Call(property, method, Expression.Constant(value));
        }
        else if (filterValue.EndsWith('*'))
        {
            var value = filterValue.TrimEnd('*');
            var method = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!;
            filterExpression = Expression.Call(property, method, Expression.Constant(value));
        }
        else if (filterValue.StartsWith('*'))
        {
            var value = filterValue.TrimStart('*');
            var method = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!;
            filterExpression = Expression.Call(property, method, Expression.Constant(value));
        }
        else
        {
            filterExpression = Expression.Equal(property, Expression.Constant(filterValue));
        }

        var lambda = Expression.Lambda<Func<T, bool>>(filterExpression, parameter);
        return query.Where(lambda);
    }
}
