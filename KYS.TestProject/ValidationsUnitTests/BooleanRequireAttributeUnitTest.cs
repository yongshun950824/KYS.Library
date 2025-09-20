﻿using KYS.Library.Validations;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KYS.TestProject.ValidationsUnitTests
{
    internal class BooleanRequireAttributeUnitTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void ValidateBooleanRequireAndPass()
        {
            // Arrange
            IsEnabledMandatoryModel testInput = new IsEnabledMandatoryModel
            {
                IsEnabled = true
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
        public void ValidateBooleanRequireAndFail()
        {
            // Arrange
            IsEnabledMandatoryModel testInput = new IsEnabledMandatoryModel
            {
                IsEnabled = false
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
        public void ValidateBooleanRequireAndFailWithErrorMessage()
        {
            // Arrange
            IsEnabledMandatoryWithErrorMessageModel testInput = new IsEnabledMandatoryWithErrorMessageModel
            {
                IsEnabled = false
            };

            ValidationContext validationContext = new ValidationContext(testInput);
            List<ValidationResult> results = new List<ValidationResult>();

            // Act
            bool isValid = Validator.TryValidateObject(testInput, validationContext, results, true);

            // Assert
            Assert.AreEqual(false, isValid);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual($"{nameof(IsEnabledMandatoryWithErrorMessageModel.IsEnabled)} is mandatory.", results[0].ErrorMessage);
        }

        private class IsEnabledMandatoryModel
        {
            [BooleanRequire]
            public bool IsEnabled { get; set; }
        }

        private class IsEnabledMandatoryWithErrorMessageModel
        {
            [BooleanRequire($"{nameof(IsEnabled)} is mandatory.")]
            public bool IsEnabled { get; set; }
        }
    }
}
