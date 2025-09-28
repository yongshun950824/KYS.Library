using KYS.Library.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static KYS.Library.Helpers.CompareOperator;

namespace KYS.TestProject.HelpersUnitTests
{
    internal class ExpressionBuilderUnitTest
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void CompileAndExpression_WithoutCriteria_ShouldReturnAll()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>> { };
            var expectedTestResult = input.Where(x => true)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithEqual_ShouldReturnFilteredElements()
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithNotEqual_ShouldReturnFilteredElements()
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithLessThan_ShouldReturnFilteredElements()
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithLessThanOrEqual_ShouldReturnFilteredElements()
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithGreaterThan_ShouldReturnFilteredElements()
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithGreaterThanOrEqual_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.GreaterThanOrEqual,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x >= filters[0].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithoutCriteria_ShouldReturnAll()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>> { };
            var expectedTestResult = input.Where(x => true)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithTwoOptionalCriteriasOne_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.GreaterThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x > filters[0].Value
                || x == filters[1].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithTwoOptionalCriteriasTwo_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThanOrEqual,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x <= filters[0].Value
                || x != filters[1].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithTwoOptionalCriteriasThree_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x < filters[0].Value
                || x != filters[1].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithTwoOptionalCriteriasFour_ShouldReturnFilteredElements()
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
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 4
                }
            };
            var expectedTestResult = input.Where(x => x >= filters[0].Value
                || x != filters[1].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithTwoOptionalCriteriasFive_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 3
                }
            };
            var expectedTestResult = input.Where(x => x < filters[0].Value
                || x == filters[1].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithTwoCriteriasOne_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.GreaterThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x > filters[0].Value
                && x == filters[1].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithTwoCriteriasTwo_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThanOrEqual,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x <= filters[0].Value
                && x != filters[1].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithTwoCriteriasThree_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 9
                }
            };
            var expectedTestResult = input.Where(x => x < filters[0].Value
                && x != filters[1].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithTwoCriteriasFour_ShouldReturnFilteredElements()
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
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 4
                }
            };
            var expectedTestResult = input.Where(x => x >= filters[0].Value
                && x != filters[1].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithTwoCriteriasFive_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 3
                }
            };
            var expectedTestResult = input.Where(x => x < filters[0].Value
                && x == filters[1].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithMultipleCriteriasOne_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.GreaterThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 2
                }
            };
            var expectedTestResult = input.Where(x => x > filters[0].Value
                || x == filters[1].Value
                || x < filters[2].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithMultipleCriteriasTwo_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThanOrEqual,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.GreaterThan,
                    Value = 6
                }
            };
            var expectedTestResult = input.Where(x => x <= filters[0].Value
                || x != filters[1].Value
                || x > filters[2].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithMultipleCriteriasThree_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 11
                }
            };
            var expectedTestResult = input.Where(x => x < filters[0].Value
                || x != filters[1].Value
                || x == filters[2].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithMultipleCriteriasFour_ShouldReturnFilteredElements()
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
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 4
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThanOrEqual,
                    Value = 3
                }
            };
            var expectedTestResult = input.Where(x => x >= filters[0].Value
                || x != filters[1].Value
                || x <= filters[2].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileOrExpression_WithMultipleCriteriasFive_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 3
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 2
                }
            };
            var expectedTestResult = input.Where(x => x < filters[0].Value
                || x == filters[1].Value
                || x != filters[2].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileOrExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithMultipleCriteriasOne_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.GreaterThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 10
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 12
                }
            };
            var expectedTestResult = input.Where(x => x > filters[0].Value
                && x == filters[1].Value
                && x < filters[2].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithMultipleCriteriasTwo_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThanOrEqual,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.GreaterThan,
                    Value = 2
                }
            };
            var expectedTestResult = input.Where(x => x <= filters[0].Value
                && x != filters[1].Value
                && x > filters[2].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithMultipleCriteriasThree_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.GreaterThanOrEqual,
                    Value = 5
                }
            };
            var expectedTestResult = input.Where(x => x < filters[0].Value
                && x != filters[1].Value
                && x >= filters[2].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithMultipleCriteriasFour_ShouldReturnFilteredElements()
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
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.NotEqual,
                    Value = 4
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 11
                }
            };
            var expectedTestResult = input.Where(x => x >= filters[0].Value
                && x != filters[1].Value
                && x == filters[2].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }

        [Test]
        public void CompileAndExpression_WithMultipleCriteriasFive_ShouldReturnFilteredElements()
        {
            // Arrange
            var input = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var filters = new List<FilterCriteria<int>>
            {
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThan,
                    Value = 9
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.Equal,
                    Value = 3
                },
                new FilterCriteria<int>
                {
                    Operator = CompareOperatorConstants.LessThanOrEqual,
                    Value = 5
                }
            };
            var expectedTestResult = input.Where(x => x < filters[0].Value
                && x == filters[1].Value
                && x <= filters[2].Value)
                .ToArray();

            // Act
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
                .ToArray();

            // Assert
            Assert.AreEqual(expectedTestResult.Length, result.Length);
            Assert.IsTrue(expectedTestResult.SequenceEqual(result));
        }
    }
}
