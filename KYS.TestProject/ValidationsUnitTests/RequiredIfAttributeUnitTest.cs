using KYS.Library.Validations;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KYS.TestProject.ValidationsUnitTests
{
    internal class RequiredIfAttributeUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void RequiredIfAttribute_WithNonMandatoryAndNullValue_ShouldPassValidation()
        {
            // Arrange
            RequiredIfModel testInput = new RequiredIfModel
            {
                IsRequired = "false",
                Value = null
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
        public void RequiredIfAttribute_WithMandatoryAndNullValue_ShouldFailValidation()
        {
            // Arrange
            RequiredIfModel testInput = new RequiredIfModel
            {
                IsRequired = "true",
                Value = null
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
        public void RequiredIfAttribute_WithMandatoryAndEmptyValue_ShouldFailValidation()
        {
            // Arrange
            RequiredIfModel testInput = new RequiredIfModel
            {
                IsRequired = "true",
                Value = String.Empty
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
        public void RequiredIfAttribute_WithMandatoryAndValue_ShouldPassValidation()
        {
            // Arrange
            RequiredIfModel testInput = new RequiredIfModel
            {
                IsRequired = "true",
                Value = "test"
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
        public void RequiredIfAttribute_WithMandatoryAndNullValue_ShouldFailValidationWithErrorMessage()
        {
            // Arrange
            RequiredWithErrorMessageModel testInput = new RequiredWithErrorMessageModel
            {
                IsRequired = "true",
                Value = null
            };

            ValidationContext validationContext = new ValidationContext(testInput);
            List<ValidationResult> results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(testInput, validationContext, results, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual($"{nameof(RequiredWithErrorMessageModel.Value)} is mandatory when {nameof(RequiredWithErrorMessageModel.IsRequired)} is true.",
                results.FirstOrDefault()?.ErrorMessage);
        }

        private class RequiredIfModel
        {
            public string IsRequired { get; set; }

            [RequiredIf(nameof(IsRequired), "true")]
            public string Value { get; set; }
        }

        private class RequiredWithErrorMessageModel
        {
            public string IsRequired { get; set; }

            [RequiredIf(otherPropertyName: nameof(IsRequired),
                matchedValue: "true",
                errorMessage: $"{nameof(Value)} is mandatory when {nameof(IsRequired)} is true.")]
            public string Value { get; set; }
        }
    }
}
