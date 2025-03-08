using System.Linq.Expressions;
using static Plutus.Infrastructure.Dtos.ListRequest;

namespace Plutus.Infrastructure.Helpers;

public static class QueriableExtensions
{
    public static IQueryable<T> ApplyUserFilter<T>(this IQueryable<T> query, string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, "UserId");
        var value = Expression.Constant(userId);
        var comparisonExpression = Expression.Equal(property, value);

        var lambda = Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);

        return query.Where(lambda);
    }

    public static IQueryable<T> ApplyDateFilter<T>(this IQueryable<T> query, string dateProperty, IDateFilterInfo dateFilterInfo)
    {

        if (dateFilterInfo == null || dateFilterInfo.StartDate == null || dateFilterInfo.EndDate == null)
        {
            return query;
        }

        var parameter = Expression.Parameter(typeof(T), "e");
        var startDate = Expression.Constant(dateFilterInfo.StartDate.Value.Date);
        var endDate = Expression.Constant(dateFilterInfo.EndDate.Value.Date);
        // Get the property of the entity and compare only the Date part
        var property = Expression.Property(parameter, dateProperty);
        property = Expression.Property(property, "Date");

        var greaterThanOrEqual = Expression.GreaterThanOrEqual(property, startDate);
        var lessThanOrEqual = Expression.LessThanOrEqual(property, endDate);

        var andExpression = Expression.AndAlso(greaterThanOrEqual, lessThanOrEqual);

        var lambda = Expression.Lambda<Func<T, bool>>(andExpression, parameter);

        return query.Where(lambda);
    }

    public static IOrderedQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortField, SortOrderType sortOrder)
    {
        if (string.IsNullOrEmpty(sortField))
        {
            return query.OrderBy(e => 0); // No sorting specified, return the original query
        }

        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, sortField);

        // Convert the property to object
        var convertedProperty = Expression.Convert(property, typeof(object));
        var lambda = Expression.Lambda<Func<T, object>>(convertedProperty, parameter);

        return sortOrder == SortOrderType.Asc
            ? query.OrderBy(lambda)
            : query.OrderByDescending(lambda);
    }

    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, ListRequestFilter filter)
    {
        if (filter == null)
        {
            return query; // No filter specified, return the original query
        }

        if (string.IsNullOrWhiteSpace(filter.Field))
        {
            return query; // No field specified, return the original query
        }

        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, filter.Field);
        var value = Expression.Constant(filter.Value);
        Expression comparisonExpression = null;

        switch (filter.Operator)
        {
            case StringComparisonType.Contains:
                comparisonExpression = Expression.Call(property, nameof(string.Contains), null, value);
                break;
            case StringComparisonType.Equals:
                // if the prop is bool we need to convert the value to bool
                if (property.Type == typeof(bool))
                {
                    value = Expression.Constant(bool.Parse(filter.Value));
                }
                comparisonExpression = Expression.Equal(property, value);
                break;
            case StringComparisonType.DoesNotEqual:
                comparisonExpression = Expression.NotEqual(property, value);
                break;
            case StringComparisonType.StartsWith:
                comparisonExpression = Expression.Call(property, nameof(string.StartsWith), null, value);
                break;
            case StringComparisonType.EndsWith:
                comparisonExpression = Expression.Call(property, nameof(string.EndsWith), null, value);
                break;
            case StringComparisonType.IsEmpty:
                comparisonExpression = Expression.Equal(property, Expression.Constant(string.Empty));
                break;
            case StringComparisonType.IsNotEmpty:
                comparisonExpression = Expression.NotEqual(property, Expression.Constant(string.Empty));
                break;
            case StringComparisonType.DoesNotContain:
                comparisonExpression = Expression.Not(Expression.Call(property, nameof(string.Contains), null, value));
                break;
            // Add other cases as needed
        }

        if (comparisonExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(comparisonExpression, parameter);
            return query.Where(lambda);
        }

        return query; // Return the original query if no valid comparison was made
    }

    public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return query.Skip(pageNumber * pageSize)
            .Take(pageSize);
    }
}
