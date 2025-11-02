using KYS.Library.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using static KYS.Library.Helpers.FormattingHelper;

namespace KYS.TestProject.HelpersUnitTests
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
        public void Convert_WithEmptyString_ShouldReturnOriginalValue()
        {
            // Arrange
            string input = String.Empty;

            // Act
            var result = FormattingHelper.Convert(input, Formatting.SnakeCase);

            // Assert
            Assert.AreEqual(input, result);
        }

        [Test]
        public void Convert_WithNullString_ShouldReturnOriginalValue()
        {
            // Arrange
            string input = null;

            // Act
            var result = FormattingHelper.Convert(input, Formatting.SnakeCase);

            // Assert
            Assert.AreEqual(input, result);
        }

        [Test]
        public void Convert_WithInvalidEnum_ShouldThrowException()
        {
            // Arrange
            Formatting unknown = (Formatting)9;
            string input = "test";
            var expectedEx = new InvalidEnumArgumentException("format", (int)unknown, typeof(Formatting));

            // Act
            var ex = Assert.Catch<InvalidEnumArgumentException>(
                () => FormattingHelper.Convert(input, unknown)
            );

            // Assert
            Assert.IsInstanceOf<InvalidEnumArgumentException>(ex);
            Assert.AreEqual(expectedEx.Message, ex.Message);
        }

        [TestCaseSource("TestCases")]
        public void Convert_WithTestCase_ShouldReturnSnakeCaseValue(string input,
            Formatting format,
            string expectedValue,
            int i)
        {
            // Act
            string actualValue = FormattingHelper.Convert(input, format);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}
