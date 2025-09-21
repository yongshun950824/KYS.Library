using KYS.Library.Extensions;
using NUnit.Framework;
using System;

namespace KYS.TestProject.ExtensionsUnitTests
{
    internal class StringExtensionsUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void RemovePostFixWithNullString()
        {
            // Arrange
            string input = null;
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, new string[] { "," });

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePostFixWithEmptyString()
        {
            // Arrange
            string input = String.Empty;
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, new string[] { "," });

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePostFixWithoutProvidingPostfixes()
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
        public void RemovePostFixWithEmptyPostfixes()
        {
            // Arrange
            string input = "test";
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, new string[] { });

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePostFixWithUnmatchedPostfix()
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
        public void RemovePostFixWithSinglePostfix()
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
        public void RemovePostFixWithMultiplePostfixes()
        {
            // Arrange
            string input = "!@test@!";
            string expectedValue = "!@test";

            // Act
            string actualValue = StringExtensions.RemovePostFix(input, new string[] { "!", "@" });

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }
        [Test]
        public void RemovePreFixWithNullString()
        {
            // Arrange
            string input = null;
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, new string[] { "," });

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePreFixWithEmptyString()
        {
            // Arrange
            string input = String.Empty;
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, new string[] { "," });

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePreFixWithoutProvidingPrefixes()
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
        public void RemovePreFixWithEmptyPrefixes()
        {
            // Arrange
            string input = "test";
            string expectedValue = input;

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, new string[] { });

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void RemovePreFixWithUnmatchedPrefix()
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
        public void RemovePreFixWithSinglePostfix()
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
        public void RemovePreFixWithMultiplePostfixes()
        {
            // Arrange
            string input = "!@test@!";
            string expectedValue = "test@!";

            // Act
            string actualValue = StringExtensions.RemovePreFix(input, new string[] { "!", "@" });

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void StringJoin()
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
        public void StringJoinWithArrayIsNull()
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
        public void StringJoinWithSeparatorIsNull()
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
        public void StringJoinByRemovingEmptyEntries()
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
        public void ToInvariantString_Null_ReturnsNull()
        {
            // Arrange
            object value = null;

            // Act & Assert
            Assert.IsNull(value.ToInvariantString());
        }

        [Test]
        public void ToInvariantString_Number_UsesInvariantCulture()
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
        public void ToInvariantString_NonFormattable_UsesToString()
        {
            // Arrage
            string input = "test";

            // Act
            var result = input.ToInvariantString();

            // Assert
            Assert.AreEqual(input, result);
        }
    }
}
