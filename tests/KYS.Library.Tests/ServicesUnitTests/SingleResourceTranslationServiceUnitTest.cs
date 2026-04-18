using CSharpFunctionalExtensions;
using KYS.Library.Services;
using KYS.Library.Tests.Resources;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace KYS.Library.Tests.ServicesUnitTests
{
    public class SingleResourceTranslationServiceUnitTest
    {
        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        private const string UNSUPPORTED_LANGUAGE_ERROR_MESSAGE = "Provided {0} doesn't support for {1} language translation.";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Create_WithoutInitializeCulture_ShouldReturnCurrentCulture()
        {
            // Arrange
            CultureInfo expectedValue = CultureInfo.CurrentCulture;

            // Act
            var result = SingleResourceTranslator.Create(typeof(Resource));
            SingleResourceTranslator translationService = result.Value;
            var actualValue = translationService.CurrentCulture;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Create_WithInitializeThaiCulture_ShouldGetCorrectCulture()
        {
            // Arrange
            CultureInfo expectedValue = new("th-TH");

            // Act
            var result = SingleResourceTranslator.Create(typeof(Resource), expectedValue);
            SingleResourceTranslator translationService = result.Value;
            var actualValue = translationService.CurrentCulture;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Create_WithoutInitializeCultureInfos_ShouldGetAllCultures()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");

            // Act
            var result = SingleResourceTranslator.Create(typeof(Resource), cultureInfo);
            SingleResourceTranslator translationService = result.Value;
            var expectedValue = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .ToList();
            var actualValue = translationService.Cultures;

            // Assert
            Assert.AreEqual(expectedValue.Count, actualValue.Count);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Create_WithInitializeCultureInfos_ShouldGetCurrentCutures()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            var expectedValue = new List<CultureInfo> { CultureInfo.InvariantCulture, cultureInfo };

            // Act
            var result = SingleResourceTranslator.Create(typeof(Resource), cultureInfo, new List<CultureInfo> { cultureInfo });
            SingleResourceTranslator translationService = result.Value;
            var actualValue = translationService.Cultures;

            // Assert
            Assert.AreEqual(expectedValue.Count, actualValue.Count);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Constructor_WithSecondConstructor_ShouldGetLanguages()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            string notExpectedValue = JsonSerializer.Serialize(new { }, _serializerOptions);   // Empty object

            // Act
            var result = SingleResourceTranslator.Create(
                typeof(Resource),
                cultureInfo
            );
            SingleResourceTranslator translationService = result.Value;
            var languages = translationService.GetLanguages();
            string serializedLanguageObj = JsonSerializer.Serialize(languages, _serializerOptions);

            // Assert
            Assert.IsNotNull(languages);
            Assert.AreNotEqual(notExpectedValue, serializedLanguageObj);
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "th-TH"));
        }

        [Test]
        public void Constructor_WithThirdConstructor_ShouldGetLanguages()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            string notExpectedValue = JsonSerializer.Serialize(new { }, _serializerOptions);   // Empty object

            // Act
            var result = SingleResourceTranslator.Create(
                typeof(Resource).FullName,
                Assembly.GetExecutingAssembly(),
                cultureInfo
            );
            SingleResourceTranslator translationService = result.Value;
            var languages = translationService.GetLanguages();
            string serializedLanguageObj = JsonSerializer.Serialize(languages, _serializerOptions);

            // Assert
            Assert.IsNotNull(languages);
            Assert.AreNotEqual(notExpectedValue, serializedLanguageObj);
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "th-TH"));
        }

        [Test]
        public void TranslateToEnglish_ShouldReturnTranslatedEnglishText()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            SingleResourceTranslator translationService = SingleResourceTranslator.Create(
                typeof(Resource),
                cultureInfo
            ).Value;
            string input = "เด็ก";
            string expectedValue = "Child";

            // Act
            Result<string> result = translationService.TranslateToEnglish(input);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Value);
        }

        [Test]
        public void TranslateToEnglish_WithUnknown_ShouldReturnResultFailure()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            SingleResourceTranslator translationService = SingleResourceTranslator.Create(
                typeof(Resource),
                cultureInfo
            ).Value;
            string input = "unknown";

            // Act
            Result<string> result = translationService.TranslateToEnglish(input);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo(String.Format(UNSUPPORTED_LANGUAGE_ERROR_MESSAGE, input, "English")));
        }

        [Test]
        public void Translate_WithoutCulture_ShouldReturnDefaultCultureText()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
            SingleResourceTranslator translationService = SingleResourceTranslator.Create(
                resourceType,
                cultureInfo
            ).Value;
            string input = "Spouse";
            // คู่สมรส
            string expectedValue = Helpers.GetTranslatedText(input, resourceType, cultureInfo) ?? input;

            // Act
            Result<string> result = translationService.Translate(input);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Value);
        }

        [Test]
        public void Translate_ToUnsupportedCulture_ShouldReturnOriginalValue()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            SingleResourceTranslator translationService = SingleResourceTranslator.Create(
                typeof(Resource),
                cultureInfo
            ).Value;
            string input = "Spouse";
            string cultureName = "en-MY";

            // Act
            Result<string> result = translationService.Translate(input, cultureName: cultureName);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo(String.Format(UNSUPPORTED_LANGUAGE_ERROR_MESSAGE, input, cultureName)));
        }

        [Test]
        public void Translate_ToSpecificCulture_ShouldReturnTranslatedText()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
            SingleResourceTranslator translationService = SingleResourceTranslator.Create(
                resourceType,
                cultureInfo
            ).Value;
            string input = "Spouse";
            // คู่สมรส
            string expectedValue = Helpers.GetTranslatedText(input, resourceType, cultureInfo) ?? input;

            // Act
            Result<string> result = translationService.Translate(input, culture: cultureInfo);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Value);
        }

        [Test]
        public void Translate_ToDefaultCulture_ShouldReturnOriginalValue()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            SingleResourceTranslator translationService = SingleResourceTranslator.Create(
                typeof(Resource),
                cultureInfo
            ).Value;
            string input = "unknown";

            // Act
            Result<string> result = translationService.Translate(input);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo(String.Format(UNSUPPORTED_LANGUAGE_ERROR_MESSAGE, input, cultureInfo.Name)));
        }

        [Test]
        public void Translate_IgnoreCase_ShouldReturnTranslatedText()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
            SingleResourceTranslator translationService = SingleResourceTranslator.Create(
                resourceType,
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            ).Value;
            string input = "spouse";
            string resourceKey = "Spouse";
            // คู่สมรส
            string expectedValue = Helpers.GetTranslatedText(resourceKey, resourceType, cultureInfo) ?? input;

            // Act
            Result<string> result = translationService.Translate(input);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Value);
        }
    }
}