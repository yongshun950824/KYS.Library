using KYS.Library.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static KYS.Library.Helpers.CompareOperator;

namespace KYS.TestProject
{
    internal class ExpressionBuilderUnitTest
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void FilterArrayWithoutCriteria()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>> { };
            var expectedTestResult = input.Where(x => true)
                .ToArray();

            // Act
            Expression<Func<int, bool>> expression = ExpressionBuilder.BuildAndExpression(filters);
            var result = input
                .Where(expression.Compile())
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithEqual()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x == filters[0].Value)
                .ToArray();

            // Act
            Expression<Func<int, bool>> expression = ExpressionBuilder.BuildAndExpression(filters);
            var result = input
                .Where(expression.Compile())
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithNotEqual()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x != filters[0].Value)
                .ToArray();

            // Act
            Expression<Func<int, bool>> expression = ExpressionBuilder.BuildAndExpression(filters);
            var result = input
                .Where(expression.Compile())
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithLessThan()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x < filters[0].Value)
                .ToArray();

            // Act
            Expression<Func<int, bool>> expression = ExpressionBuilder.BuildAndExpression(filters);
            var result = input
                .Where(expression.Compile())
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithLessThanOrEqual()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThanOrEqual,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x <= filters[0].Value)
                .ToArray();

            // Act
            Expression<Func<int, bool>> expression = ExpressionBuilder.BuildAndExpression(filters);
            var result = input
                .Where(expression.Compile())
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithGreaterThan()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.GreaterThan,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x > filters[0].Value)
                .ToArray();

            // Act
            Expression<Func<int, bool>> expression = ExpressionBuilder.BuildAndExpression(filters);
            var result = input
                .Where(expression.Compile())
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithGreaterThanOrEqual()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.GreaterThanOrEqual,
                    Value = 9
                },
            };
            var expectedTestResult = input.Where(x => x >= filters[0].Value)
                .ToArray();

            // Act
            Expression<Func<int, bool>> expression = ExpressionBuilder.BuildAndExpression(filters);
            var result = input
                .Where(expression.Compile())
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }
    }
}
