using KYS.Library.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using static KYS.Library.Helpers.CompareOperator;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for <see cref="System.Linq.Expressions" />.
    /// </summary>
    public static class ExpressionBuilder
    {
        /// <summary>
        /// Builds a composite <see cref="Expression{TDelegate}"/> that represents
        /// a logical AND operation across multiple filter conditions.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object being filtered. Must implement <see cref="IComparable"/>.
        /// </typeparam>
        /// <param name="filterCriterias">
        /// A list of <see cref="FilterCriteria{T}"/> objects that define the individual comparison expressions
        /// to be combined with logical AND.
        /// </param>
        /// <returns>
        /// An <see cref="Expression{TDelegate}"/> of type <see cref="Func{T, Boolean}"/> that evaluates to
        /// <see langword="true"/> only if <b>all</b> the filter conditions are satisfied.
        /// If <paramref name="filterCriterias"/> is empty, the method returns an expression that always evaluates to <see langword="true"/>.
        /// </returns>
        public static Expression<Func<T, bool>> BuildAndExpression<T>(List<FilterCriteria<T>> filterCriterias)
            where T : IComparable
        {
            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            BinaryExpression[] binaryExpressions = filterCriterias.Select(x => BuildCompareExpression(param, x))
                .ToArray();

            BinaryExpression combineExpression = BuildAndExpression(binaryExpressions);

            Expression<Func<T, bool>> lambda = combineExpression != null
                ? Expression.Lambda<Func<T, bool>>(combineExpression, param)
                : Expression.Lambda<Func<T, bool>>(Expression.Constant(true), param);

            return lambda;
        }

        /// <summary>
        /// Builds and compiles a composite predicate function that represents
        /// a logical AND operation across multiple filter conditions.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object being filtered. Must implement <see cref="IComparable"/>.
        /// </typeparam>
        /// <param name="filterCriterias">
        /// A list of <see cref="FilterCriteria{T}"/> objects defining the comparison
        /// expressions to be combined with logical AND.
        /// </param>
        /// <returns>
        /// A compiled <see cref="Func{T, Boolean}"/> delegate that evaluates to
        /// <see langword="true"/> only if all filter conditions are satisfied.
        /// </returns>
        public static Func<T, bool> CompileAndExpression<T>(List<FilterCriteria<T>> filterCriterias)
            where T : IComparable
        {
            return BuildAndExpression(filterCriterias)
                .CompileExpression();
        }

        /// <summary>
        /// Builds a composite <see cref="Expression{TDelegate}"/> that represents
        /// a logical OR operation across multiple filter conditions.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object being filtered. Must implement <see cref="IComparable"/>.
        /// </typeparam>
        /// <param name="filterCriterias">
        /// A list of <see cref="FilterCriteria{T}"/> objects that define the individual comparison expressions
        /// to be combined with logical OR.
        /// </param>
        /// <returns>
        /// An <see cref="Expression{TDelegate}"/> of type <see cref="Func{T, Boolean}"/> that evaluates to
        /// <see langword="true"/> only if <b>any</b> filter conditions are satisfied.
        /// If <paramref name="filterCriterias"/> is empty, the method returns an expression that always evaluates to <see langword="true"/>.
        /// </returns>
        public static Expression<Func<T, bool>> BuildOrExpression<T>(List<FilterCriteria<T>> filterCriterias)
            where T : IComparable
        {
            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            BinaryExpression[] binaryExpressions = filterCriterias.Select(x => BuildCompareExpression(param, x))
                .ToArray();

            BinaryExpression combineExpression = BuildOrExpression(binaryExpressions);

            Expression<Func<T, bool>> lambda = combineExpression != null
                ? Expression.Lambda<Func<T, bool>>(combineExpression, param)
                : Expression.Lambda<Func<T, bool>>(Expression.Constant(true), param);

            return lambda;
        }

        /// <summary>
        /// Builds and compiles a composite predicate function that represents
        /// a logical OR operation across multiple filter conditions.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object being filtered. Must implement <see cref="IComparable"/>.
        /// </typeparam>
        /// <param name="filterCriterias">
        /// A list of <see cref="FilterCriteria{T}"/> objects defining the comparison
        /// expressions to be combined with logical OR.
        /// </param>
        /// <returns>
        /// A compiled <see cref="Func{T, Boolean}"/> delegate that evaluates to
        /// <see langword="true"/> only if any filter conditions are satisfied.
        /// </returns>
        public static Func<T, bool> CompileOrExpression<T>(List<FilterCriteria<T>> filterCriterias)
            where T : IComparable
        {
            return BuildOrExpression(filterCriterias)
                .CompileExpression();
        }

        /// <summary>
        /// Compiles the specified expression into an executable predicate function.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the object being filtered. Must implement <see cref="IComparable"/>.
        /// </typeparam>
        /// <param name="lambda">
        /// The <see cref="Expression{TDelegate}"/> representing a predicate to compile.
        /// </param>
        /// <returns>
        /// A compiled <see cref="Func{T, Boolean}"/> delegate that executes the provided expression.
        /// </returns>
        public static Func<T, bool> CompileExpression<T>(this Expression<Func<T, bool>> lambda)
            where T : IComparable
        {
            return lambda.Compile();
        }

        private static BinaryExpression BuildAndExpression(params BinaryExpression[] expressions)
        {
            if (expressions.IsNullOrEmpty())
                return null;

            BinaryExpression combineExpression = expressions[0];

            foreach (var expression in expressions.Skip(1))
            {
                combineExpression = Expression.And(combineExpression, expression);
            }

            return combineExpression;
        }

        private static BinaryExpression BuildOrExpression(params BinaryExpression[] expressions)
        {
            if (expressions.IsNullOrEmpty())
                return null;

            BinaryExpression combineExpression = expressions[0];

            foreach (var expression in expressions.Skip(1))
            {
                combineExpression = Expression.Or(combineExpression, expression);
            }

            return combineExpression;
        }

        private static BinaryExpression BuildCompareExpression<T>(ParameterExpression param, FilterCriteria<T> filterCriteria)
            where T : IComparable
        {
            var constant = Expression.Constant(filterCriteria.Value);
            BinaryExpression binaryExpression = filterCriteria.Operator switch
            {
                CompareOperatorConstants.Equal => Expression.Equal(param, constant),
                CompareOperatorConstants.NotEqual => Expression.NotEqual(param, constant),
                CompareOperatorConstants.LessThan => Expression.LessThan(param, constant),
                CompareOperatorConstants.LessThanOrEqual => Expression.LessThanOrEqual(param, constant),
                CompareOperatorConstants.GreaterThan => Expression.GreaterThan(param, constant),
                CompareOperatorConstants.GreaterThanOrEqual => Expression.GreaterThanOrEqual(param, constant),
                _ => throw new InvalidEnumArgumentException(nameof(filterCriteria.Operator), (int)filterCriteria.Operator, typeof(CompareOperatorConstants)),
            };

            return binaryExpression;
        }
    }

    /// <summary>
    /// Represents a filter condition that defines a comparison operation
    /// and a value to compare against within a query or expression.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value to compare. Must implement <see cref="IComparable"/>.
    /// </typeparam>
    public class FilterCriteria<T>
        where T : IComparable
    {
        /// <summary>
        /// Gets or sets the comparison operator used for evaluating the filter condition.
        /// </summary>
        public CompareOperatorConstants Operator { get; set; }
        /// <summary>
        /// Gets or sets the value to be used in the comparison.
        /// </summary>
        public T Value { get; set; }
    }
}
