using KYS.Library.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static KYS.Library.Helpers.CompareOperator;

namespace KYS.Library.Helpers
{
    public static class ExpressionBuilder
    {
        public static Expression<Func<T, bool>> BuildAndExpression<T>(List<FilterCriteria<T>> filterCriterias)
            where T : IComparable
        {
            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            BinaryExpression[] binaryExpressions = filterCriterias.Select(x => BuildCompareExpression(param, x))
                .ToArray();

            BinaryExpression combineExpression = BuildAndExpression(binaryExpressions);

            LambdaExpression lambda = combineExpression != null
                ? Expression.Lambda<Func<T, bool>>(combineExpression, param)
                : Expression.Lambda<Func<T, bool>>(Expression.Constant(true), param);

            return (Expression<Func<T, bool>>)lambda;
        }

        public static Expression<Func<T, bool>> BuildOrExpression<T>(List<FilterCriteria<T>> filterCriterias)
            where T : IComparable
        {
            ParameterExpression param = Expression.Parameter(typeof(T), "x");
            BinaryExpression[] binaryExpressions = filterCriterias.Select(x => BuildCompareExpression(param, x))
                .ToArray();

            BinaryExpression combineExpression = BuildOrExpression(binaryExpressions);

            LambdaExpression lambda = combineExpression != null
                ? Expression.Lambda<Func<T, bool>>(combineExpression, param)
                : Expression.Lambda<Func<T, bool>>(Expression.Constant(true), param);

            return (Expression<Func<T, bool>>)lambda;
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
                _ => throw new Exception("Operation is not supported"),
            };

            return binaryExpression;
        }
    }

    public class FilterCriteria<T>
        where T : IComparable
    {
        public CompareOperatorConstants Operator { get; set; }
        public T Value { get; set; }
    }
}
