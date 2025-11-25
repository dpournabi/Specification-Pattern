using Domain.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Extensions;
public static class SpecificationExtensions
{
    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> specification)
        where T : class
    {
        // Filter criteria
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        // Includes
        query = specification.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        // Ordering
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        // Paging
        if (specification.IsPagingEnabled)
        {
            query = query.Skip(specification.Skip).Take(specification.Take);
        }

        return query;
    }
}