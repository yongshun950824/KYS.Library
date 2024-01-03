using KYS.Library.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
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
            Func<int, bool> compileExpr = ExpressionBuilder.CompileAndExpression(filters);
            var result = input
                .Where(compileExpr)
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithTwoOptionalCriterias1()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithTwoOptionalCriterias2()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithTwoOptionalCriterias3()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithTwoOptionalCriterias4()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithTwoOptionalCriterias5()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithTwoCriterias1()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithTwoCriterias2()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithTwoCriterias3()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithTwoCriterias4()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithTwoCriterias5()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithMultipleOptionalCriterias1()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithMultipleOptionalCriterias2()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithMultipleOptionalCriterias3()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithMultipleOptionalCriterias4()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithMultipleOptionalCriterias5()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithMultipleCriterias1()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithMultipleCriterias2()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithMultipleCriterias3()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithMultipleCriterias4()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }

        [Test]
        public void FilterArrayWithMultipleCriterias5()
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
            Assert.IsTrue(Enumerable.SequenceEqual(expectedTestResult, result));
        }
    }
}
