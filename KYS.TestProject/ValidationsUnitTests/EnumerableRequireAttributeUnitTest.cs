using KYS.Library.Validations;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KYS.TestProject.ValidationsUnitTests
{
    internal class EnumerableRequireAttributeUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ValidateEnumerableRequireAndPass()
        {
            // Arrange
            EnumerableModel testInput = new EnumerableModel
            {
                List = new List<string> { "a" }
            };

            ValidationContext validationContext = new ValidationContext(testInput);
            List<ValidationResult> results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(testInput, validationContext, results, true);

            // Assert
            Assert.AreEqual(true, isValid);
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void ValidateEnumerableRequireAndFail()
        {
            // Arrange
            EnumerableModel testInput = new EnumerableModel
            {
                List = new List<string> { }
            };

            ValidationContext validationContext = new ValidationContext(testInput);
            List<ValidationResult> results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(testInput, validationContext, results, true);

            // Assert
            Assert.AreEqual(false, isValid);
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void ValidateEnumerableRequireWithNullAndFail()
        {
            // Arrange
            EnumerableModel testInput = new EnumerableModel
            {
                List = null
            };

            ValidationContext validationContext = new ValidationContext(testInput);
            List<ValidationResult> results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(testInput, validationContext, results, true);

            // Assert
            Assert.AreEqual(false, isValid);
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void ValidateEnumerableRequireAndFailWithErrorMessage()
        {
            // Arrange
            EnumerableWithErrorMessageModel testInput = new EnumerableWithErrorMessageModel
            {
                List = new List<string> { }
            };

            ValidationContext validationContext = new ValidationContext(testInput);
            List<ValidationResult> results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(testInput, validationContext, results, true);

            // Assert
            Assert.AreEqual(false, isValid);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual($"{nameof(EnumerableWithErrorMessageModel.List)} is mandatory.", results[0].ErrorMessage);
        }

        private class EnumerableModel
        {
            [EnumerableRequire]
            public IEnumerable<string> List { get; set; }
        }

        private class EnumerableWithErrorMessageModel
        {
            [EnumerableRequire($"{nameof(List)} is mandatory.")]
            public IEnumerable<string> List { get; set; }
        }
    }
}
