using KYS.Library.Extensions;
using NUnit.Framework;

namespace KYS.TestProject.ExtensionsUnitTests
{
    internal class DecimalExtensionsUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void RoundUpToZeroDecimalPlaces()
        {
            // Arrange
            decimal input1 = 100;
            decimal input2 = 100.10M;
            decimal input3 = 100.9M;

            // Act
            decimal result1 = input1.Round(0);
            decimal result2 = input2.Round(0);
            decimal result3 = input3.Round(0);

            // Assert
            Assert.AreEqual(100, result1);
            Assert.AreEqual(100, result2);
            Assert.AreEqual(101, result3);
        }

        [Test]
        public void RoundUpToTwoDecimalPlaces()
        {
            // Arrange
            decimal input1 = 100;
            decimal input2 = 100.144M;
            decimal input3 = 100.145M;

            // Act
            decimal result1 = input1.Round(2);
            decimal result2 = input2.Round(2);
            decimal result3 = input3.Round(2);

            // Assert
            Assert.AreEqual(100, result1);
            Assert.AreEqual(100.14M, result2);
            Assert.AreEqual(100.15M, result3);
        }

        [Test]
        public void RoundUpToThreeDecimalPlaces()
        {
            // Arrange
            decimal input1 = 100;
            decimal input2 = 100.1444M;
            decimal input3 = 100.1495M;

            // Act
            decimal result1 = input1.Round(3);
            decimal result2 = input2.Round(3);
            decimal result3 = input3.Round(3);

            // Assert
            Assert.AreEqual(100, result1);
            Assert.AreEqual(100.144M, result2);
            Assert.AreEqual(100.15M, result3);
        }
    }
}
