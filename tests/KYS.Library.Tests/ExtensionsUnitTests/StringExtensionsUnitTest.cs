using KYS.Library.Extensions;
using NUnit.Framework;
using System;

namespace KYS.Library.Tests.ExtensionsUnitTests
{
    internal class StringExtensionsUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void RemovePostFix_WithNull_ShouldReturnNull()
        {
            // Arrange
            string input = null;
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, [","]);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePostFix_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            string input = String.Empty;
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, [","]);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePostFix_WithoutPostfixes_ShouoldReturnOriginalString()
        {
            // Arrange
            string input = "test";
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, null);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePostFix_WithEmptyPostfix_ShouldReturnOriginalString()
        {
            // Arrange
            string input = "test";
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, Array.Empty<string>());

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePostFix_WithUnmatchedPostfix_ShouldReturnOriginalString()
        {
            // Arrange
            string input = ",test,";
            string expectedValue = ",test,";

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, ";");

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePostFix_WithSinglePostfix_ShouldReturnRemovedPostfixString()
        {
            // Arrange
            string input = ",test,";
            string expectedValue = ",test";

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, ",");

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePostFix_WithMultiplePostfixes_ShouldReturnRemovedPostfixesString()
        {
            // Arrange
            string input = "!@test@!";
            string expectedValue = "!@test";

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, ["!", "@"]);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void RemovePreFix_WithNull_ShouldReturnNull()
        {
            // Arrange
            string input = null;
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, [","]);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePreFix_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            string input = String.Empty;
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, [","]);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePreFix_WithoutPrefixes_ShouldReturnOriginalString()
        {
            // Arrange
            string input = "test";
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, null);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePreFix_WithEmptyPrefix_ShouldReturnOriginalString()
        {
            // Arrange
            string input = "test";
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, Array.Empty<string>());

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePreFix_WithUnmatchedPrefix_ShouldReturnOriginalString()
        {
            // Arrange
            string input = ",test,";
            string expectedValue = ",test,";

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, ";");

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePreFix_WithSinglePostfix_ShouldReturnRemovedPrefixString()
        {
            // Arrange
            string input = ",test,";
            string expectedValue = "test,";

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, ",");

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePreFix_WithMultiplePostfixes_ShouldReturnRemovedPostfixesString()
        {
            // Arrange
            string input = "!@test@!";
            string expectedValue = "test@!";

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, ["!", "@"]);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void StringJoin_WithArray_ShouldReturnJoinedString()
        {
            // Arrange
            string[] inputs = new string[] { "a", "b", "c" };
            string expectedValue = "a,b,c";

            // Act
            string actualValue = StringExtensions.Join(",", false, inputs);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void StringJoin_WithNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            string[] inputs = null;
            ArgumentNullException expectedException = new ArgumentNullException("values");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => StringExtensions.Join(",", false, inputs));

            // Assert
            Assert.AreEqual(expectedException.Message, ex.Message);
        }

        [Test]
        public void StringJoin_WithNullAsSeparator_ShouldThrowArgumentNullException()
        {
            // Arrange
            string[] inputs = new string[] { "a", "b", null, "c", null };
            ArgumentNullException expectedException = new ArgumentNullException("separator");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => StringExtensions.Join(inputs, null, false));

            // Assert
            Assert.AreEqual(expectedException.Message, ex.Message);
        }

        [Test]
        public void StringJoin_WithNullEntry_ShouldRemoveEmptyEntriesAndReturnJoinedString()
        {
            // Arrange
            string[] inputs = new string[] { "a", "b", null, "c", null };
            string expectedValue = "a,b,c";

            // Act
            string actualValue = StringExtensions.Join(",", true, inputs);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void ToInvariantString_WithNull_ShouldReturnNull()
        {
            // Arrange
            object value = null;

            // Act & Assert
            Assert.IsNull(value.ToInvariantString());
        }

        [Test]
        public void ToInvariantString_WithNumberAndUsesInvariantCulture_ShouldReturnString()
        {
            // Arrange
            double value = 1234.56;

            // Act
            var result = value.ToInvariantString();

            // Assert
            // InvariantCulture uses "." as decimal separator
            Assert.AreEqual("1234.56", result);
        }

        [Test]
        public void ToInvariantString_WithNonFormattable_ShouldReturnString()
        {
            // Arrage
            string input = "test";

            // Act
            var result = input.ToInvariantString();

            // Assert
            Assert.AreEqual(input, result);
        }

        [Test]
        public void Truncate_WithNullValue_ShouldReturnNull()
        {
            // Arrage
            string input = null;

            // Act
            var result = input.Truncate(1);

            // Assert
            Assert.AreEqual(input, result);
        }

        [Test]
        public void Truncate_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrage
            string input = String.Empty;

            // Act
            var result = input.Truncate(1);

            // Assert
            Assert.AreEqual(input, result);
        }

        [Test]
        public void Truncate_WithLessThanOneLength_ShouldThrowException()
        {
            // Arrage
            string input = String.Empty;
            var expectedEx = new ArgumentException("The length must be positive value and more than zero.", "length");

            // Act
            var ex = Assert.Catch<ArgumentException>(() => input.Truncate(0));

            // Assert
            Assert.IsInstanceOf<ArgumentException>(ex);
            Assert.AreEqual(expectedEx.Message, ex.Message);
        }

        [Test]
        public void Truncate_WithProvidedLengthIsLesser_ShouldReturnTruncatedString()
        {
            // Arrage
            string input = "sample";
            int truncatedLength = 3;

            // Act
            var result = input.Truncate(truncatedLength);

            // Assert
            Assert.AreEqual(input[..Math.Min(input.Length, truncatedLength)], result);
        }

        [Test]
        public void Truncate_WithProvidedLengthIsLonger_ShouldReturnTruncatedString()
        {
            // Arrage
            string input = "sample";
            int truncatedLength = 10;

            // Act
            var result = input.Truncate(truncatedLength);

            // Assert
            Assert.AreEqual(input[..Math.Min(input.Length, truncatedLength)], result);
        }
    }
}
