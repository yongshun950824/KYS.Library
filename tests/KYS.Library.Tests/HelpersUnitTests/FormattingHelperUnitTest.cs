using System;
using System.Collections.Generic;
using KYS.Library.Helpers;
using NUnit.Framework;
using static KYS.Library.Helpers.FormattingHelper;

namespace KYS.Library.Tests.HelpersUnitTests
{
    internal class FormattingHelperUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        public static IEnumerable<dynamic> TestCases
        {
            get
            {
                var testCases = new List<dynamic>
                {
                    new { Input = "this", Formatting = Formatting.SnakeCase, ExpectedValue = "this" },
                    new { Input = "thisIsASentence", Formatting = Formatting.SnakeCase, ExpectedValue = "this_is_a_sentence" },
                    new { Input = "ThisIsASentence", Formatting = Formatting.SnakeCase, ExpectedValue = "this_is_a_sentence" },
                };

                for (int i = 0; i < testCases.Count; i++)
                {
                    yield return new TestCaseData(
                        testCases[i].Input,
                        testCases[i].Formatting,
                        testCases[i].ExpectedValue,
                        i
                    );
                }
            }
        }

        [Test]
        public void Convert_WithEmptyString_ShouldReturnResultSuccessWithOriginalValue()
        {
            // Arrange
            string input = String.Empty;

            // Act
            var result = FormattingHelper.Convert(input, Formatting.SnakeCase);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(input, result.Value);
        }

        [Test]
        public void Convert_WithNullString_ShouldReturnResultSuccessWithOriginalValue()
        {
            // Arrange
            string input = null;

            // Act
            var result = FormattingHelper.Convert(input, Formatting.SnakeCase);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(input, result.Value);
        }

        [Test]
        public void Convert_WithInvalidEnum_ShouldReturnResultFailure()
        {
            // Arrange
            Formatting unknown = (Formatting)9;
            string input = "test";

            // Act
            var result = FormattingHelper.Convert(input, unknown);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual($"Invalid formatting option: {unknown}.", result.Error);
        }

        [TestCaseSource(nameof(TestCases))]
        public void Convert_WithTestCase_ShouldReturnResultSuccessWithSnakeCaseValue(string input,
            Formatting format,
            string expectedValue,
            int i)
        {
            // Act
            var result = FormattingHelper.Convert(input, format);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Value);
        }
    }
}
