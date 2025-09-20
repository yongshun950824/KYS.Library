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
        public void ValidateRequiredIfFalseAndPass()
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
            Assert.AreEqual(true, isValid);
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void ValidateRequiredIfTrueWithNullValueAndFail()
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
            Assert.AreEqual(false, isValid);
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void ValidateRequiredIfTrueWithEmptyValueAndFail()
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
            Assert.AreEqual(false, isValid);
            Assert.AreEqual(1, results.Count);
        }

        [Test]
        public void ValidateRequiredIfTrueWithValueAndPass()
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
            Assert.AreEqual(true, isValid);
            Assert.AreEqual(0, results.Count);
        }


        [Test]
        public void ValidateRequiredIfTrueWithNullValueAndFailWithErrorMessage()
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
            Assert.AreEqual(false, isValid);
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

            [RequiredIf(nameof(IsRequired), "true",
                ErrorMessage = $"{nameof(Value)} is mandatory when {nameof(IsRequired)} is true.")]
            public string Value { get; set; }
        }
    }
}
