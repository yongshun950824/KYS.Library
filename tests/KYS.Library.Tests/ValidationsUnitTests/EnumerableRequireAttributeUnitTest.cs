using KYS.Library.Validations;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KYS.Library.Tests.ValidationsUnitTests
{
    internal class EnumerableRequireAttributeUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void EnumerableRequireAttribute_WithNonEmptyList_ShouldPassValidation()
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
            Assert.IsTrue(isValid);
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void EnumerableRequireAttribute_WithEmptyList_ShouldFailValidation()
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
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void EnumerableRequireAttribute_WithNullList_ShouldFailValidation()
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
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void EnumerableRequireAttribute_WithEmptyList_ShouldFailValidationWithErrorMessage()
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
            Assert.IsFalse(isValid);
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
