using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace KYS.EFCore.Library.Extensions;

public static class SpecificationExtensions
{
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
