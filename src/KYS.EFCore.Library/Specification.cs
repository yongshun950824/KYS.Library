using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KYS.EFCore.Library;

/// <summary>
/// Base class for the Specification pattern: encapsulates a filter criterion for <typeparamref name="T"/>,
/// plus the related entities to include and whether to track the results.
/// </summary>
/// <typeparam name="T">The type of the object the specification applies to.</typeparam>
/// <remarks>
/// Use <see cref="Extensions.SpecificationExtensions.ApplySpecification{TEntity}"/> to query a <c>DbContext</c> with it,
/// or <see cref="IsSatisfiedBy"/> to check an object in memory.
/// </remarks>
public abstract class Specification<T>
{
    /// <summary>
    /// Gets the navigation properties to eager-load when querying.
    /// </summary>
    public List<Expression<Func<T, object>>> Includes { get; } = [];

    /// <summary>
    /// Gets a value indicating whether query results should not be tracked by the change tracker.
    /// </summary>
    public bool AsNoTracking { get; private set; } = false;

    /// <summary>
    /// Adds a navigation property to eager-load when querying.
    /// </summary>
    /// <param name="includeExpression">The expression that selects the navigation property.</param>
    public void AddInclude(Expression<Func<T, object>> includeExpression)
    {
        Includes.Add(includeExpression);
    }

    /// <summary>
    /// Marks the specification so that query results are not tracked.
    /// </summary>
    public void ApplyAsNoTracking()
    {
        AsNoTracking = true;
    }

    /// <summary>
    /// Returns the filter criterion as an expression that can be translated by a LINQ provider.
    /// </summary>
    /// <returns>The filter criterion.</returns>
    public abstract Expression<Func<T, bool>> ToExpression();

    /// <summary>
    /// Determines whether the specified object satisfies the specification.
    /// </summary>
    /// <param name="entity">The object to check.</param>
    /// <returns><see langword="true"/> if <paramref name="entity"/> satisfies the criterion; otherwise <see langword="false"/>.</returns>
    /// <remarks>The expression from <see cref="ToExpression"/> is compiled on every call.</remarks>
    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }

    /// <summary>
    /// Combines this specification with another one using a logical AND.
    /// </summary>
    /// <param name="other">The specification to combine with.</param>
    /// <returns>A new specification that is satisfied when both specifications are satisfied.</returns>
    /// <remarks>The <see cref="Includes"/> and <see cref="AsNoTracking"/> settings of the operands are not carried over.</remarks>
    public Specification<T> And(Specification<T> other)
    {
        return new AndSpecification<T>(this, other);
    }

    /// <summary>
    /// Combines this specification with another one using a logical OR.
    /// </summary>
    /// <param name="other">The specification to combine with.</param>
    /// <returns>A new specification that is satisfied when either specification is satisfied.</returns>
    /// <remarks>The <see cref="Includes"/> and <see cref="AsNoTracking"/> settings of the operands are not carried over.</remarks>
    public Specification<T> Or(Specification<T> other)
    {
        return new OrSpecification<T>(this, other);
    }
}

/// <summary>
/// Specification that is satisfied when both of its operands are satisfied.
/// </summary>
/// <typeparam name="T">The type of the object the specification applies to.</typeparam>
public sealed class AndSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    /// <summary>
    /// Initializes a new instance of the <see cref="AndSpecification{T}"/> class.
    /// </summary>
    /// <param name="left">The first specification.</param>
    /// <param name="right">The second specification.</param>
    public AndSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    /// <summary>
    /// Returns an expression that combines the operands' expressions with <see cref="Expression.AndAlso(Expression, Expression)"/>.
    /// </summary>
    /// <returns>The combined filter criterion.</returns>
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.AndAlso(
            Expression.Invoke(leftExpression, parameter),
            Expression.Invoke(rightExpression, parameter)
        );

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}

/// <summary>
/// Specification that is satisfied when either of its operands is satisfied.
/// </summary>
/// <typeparam name="T">The type of the object the specification applies to.</typeparam>
public sealed class OrSpecification<T> : Specification<T>
{
    private readonly Specification<T> _left;
    private readonly Specification<T> _right;

    /// <summary>
    /// Initializes a new instance of the <see cref="OrSpecification{T}"/> class.
    /// </summary>
    /// <param name="left">The first specification.</param>
    /// <param name="right">The second specification.</param>
    public OrSpecification(Specification<T> left, Specification<T> right)
    {
        _left = left;
        _right = right;
    }

    /// <summary>
    /// Returns an expression that combines the operands' expressions with <see cref="Expression.OrElse(Expression, Expression)"/>.
    /// </summary>
    /// <returns>The combined filter criterion.</returns>
    public override Expression<Func<T, bool>> ToExpression()
    {
        var leftExpression = _left.ToExpression();
        var rightExpression = _right.ToExpression();

        var parameter = Expression.Parameter(typeof(T));
        var combined = Expression.OrElse(
            Expression.Invoke(leftExpression, parameter),
            Expression.Invoke(rightExpression, parameter)
        );

        return Expression.Lambda<Func<T, bool>>(combined, parameter);
    }
}
