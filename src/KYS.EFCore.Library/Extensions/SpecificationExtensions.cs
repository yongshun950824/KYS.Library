using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace KYS.EFCore.Library.Extensions;

/// <summary>
/// Provides extension methods for querying a <see cref="DbContext"/> with a <see cref="Specification{T}"/>.
/// </summary>
public static class SpecificationExtensions
{
    /// <summary>
    /// Builds a query on the <see cref="DbSet{TEntity}"/> of <typeparamref name="TEntity"/> that applies the
    /// no-tracking option, the includes and the filter of the <paramref name="specification"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to query.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance this method extends.</param>
    /// <param name="specification">The specification describing the query.</param>
    /// <returns>A query that has not been executed yet.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="specification"/> is <see langword="null"/>.</exception>
    public static IQueryable<TEntity> ApplySpecification<TEntity>(
        this DbContext context,
        Specification<TEntity> specification) where TEntity : class
    {
        ArgumentNullException.ThrowIfNull(specification);

        var query = context.Set<TEntity>().AsQueryable();

        if (specification.AsNoTracking)
        {
            query = query.AsNoTracking();
        }

        foreach (var include in specification.Includes)
        {
            query = query.Include(include);
        }

        var predicate = specification.ToExpression();
        return query.Where(predicate);
    }
}
