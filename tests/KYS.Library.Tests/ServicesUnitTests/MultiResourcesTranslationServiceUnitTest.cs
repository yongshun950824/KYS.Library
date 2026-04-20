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
    public class MultiResourcesTranslationServiceUnitTest
    {
        private static readonly JsonSerializerOptions _serializerOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        private const string UNSUPPORTED_LANGUAGE_ERROR_MESSAGE = "Provided {0} doesn't support for {1} language translation.";
        private const string UNSUPPORTED_LANGUAGE_FOR_RESOURCE_ERROR_MESSAGE = "Provided {0} doesn't support for {1} language translation in {2}.";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Create_WithoutInitializeCulture_ShouldGetCurrentCulture()
        {
            // Arrange
            CultureInfo expectedValue = CultureInfo.CurrentCulture;

            // Act
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly()
            ).Value;
            var actualValue = translationService.CurrentCulture;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Create_WithCurrentCulture_ShouldGetCorrectCulture()
        {
            // Arrange
            CultureInfo expectedValue = new("th-TH");

            // Act
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                currentCulture: expectedValue
            ).Value;
            var actualValue = translationService.CurrentCulture;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Create_WithCurrentCulture_ShouldGetAllCultures()
        {
            // Arrange
            var expectedValue = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .ToList();

            // Act
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                currentCulture: new CultureInfo("th-TH")
            ).Value;
            var actualValue = translationService.Cultures;

            // Assert
            Assert.AreEqual(expectedValue.Count, actualValue.Count);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Create_WithCurrentCultureAndCultureInfos_ShouldGetCurrentCultures()
        {
            // Arrange
            CultureInfo cultureInfo = new("th-TH");
            var expectedValue = new List<CultureInfo> { CultureInfo.InvariantCulture, cultureInfo };

            // Act
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                cultureInfo,
                [cultureInfo]
            ).Value;
            var actualValue = translationService.Cultures;

            // Assert
            Assert.AreEqual(expectedValue.Count, actualValue.Count);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Create_WithCurrentCultureAndCultureInfos_ShouldGetLanguages()
        {
            // Arrange
            CultureInfo cultureInfo = new("th-TH");

            // Act
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                cultureInfo,
                [cultureInfo]
            ).Value;
            var languages = translationService.GetLanguages();

            // Assert
            Assert.IsNotNull(languages);
            Assert.NotEquilaventTo(new { }, languages);
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "th-TH"));
        }

        [Test]
        public void Create_WithAssemblyAndCurrentCultureAndCultureInfos_ShouldGetLanguages()
        {
            // Arrange
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                new CultureInfo("th-TH"),
                [new CultureInfo("th-TH"), new CultureInfo("zh-CN")]
            ).Value;

            // Act
            var languages = translationService.GetLanguages();

            // Assert
            Assert.IsNotNull(languages);
            Assert.NotEquilaventTo(new { }, languages);
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "th-TH"));
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "zh-CN"));
        }

        [Test]
        public void Create_WithoutAssembly_ShouldGetLanguages()
        {
            // Arrange
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                assembly: null,
                currentCulture: new CultureInfo("th-TH"),
                cultures: [new CultureInfo("th-TH"), new CultureInfo("zh-CN")]
            ).Value;
            string notExpectedValue = JsonSerializer.Serialize(new { }, _serializerOptions);   // Empty object

            // Act
            var languages = translationService.GetLanguages();

            // Assert
            Assert.IsNotNull(languages);
            Assert.NotEquilaventTo(new { }, languages);
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "th-TH"));
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "zh-CN"));
        }

        [Test]
        public void TranslateToEnglish_ShouldReturnTranslatedEnglishText()
        {
            // Arrange
            CultureInfo cultureInfo = new("th-TH");
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                [cultureInfo]
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
        public void TranslateToEnglish_WithUnknown_ShouldReturnTranslatedEnglishText()
        {
            // Arrange
            CultureInfo cultureInfo = new("th-TH");
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                [cultureInfo]
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
            CultureInfo cultureInfo = new("th-TH");
            Type resourceType = typeof(Resource);
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                [cultureInfo]
            ).Value;

            string input = "Spouse";
            // "คู่สมรส"
            string expectedValue = Helpers.GetTranslatedText(input, resourceType, cultureInfo) ?? input;

            // Act
            Result<string> result = translationService.Translate(input);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Value);
        }

        [Test]
        public void Translate_ToUnsupportedCulture_ShouldReturnResultFailure()
        {
            // Arrange
            CultureInfo culture = new("th-TH");
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                culture,
                [culture]
            ).Value;

            string input = "Spouse";
            string cultureName = "en-MY";

            // Act
            Result<string> result = translationService.Translate(input, cultureName);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo(String.Format(UNSUPPORTED_LANGUAGE_ERROR_MESSAGE, input, cultureName)));
        }

        [Test]
        public void Translate_ToSpecificCulture_ShouldReturnTranslatedText()
        {
            // Arrange
            CultureInfo culture = new("th-TH");
            Type resourceType = typeof(Resource);
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                cultures: [culture]
            ).Value;

            string input = "Spouse";
            // "คู่สมรส"
            string expectedValue = Helpers.GetTranslatedText(input, resourceType, culture) ?? input;

            // Act
            Result<string> result = translationService.Translate(input, culture: culture);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Value);
        }

        [Test]
        public void Translate_WithUnknown_ShouldReturnResultFailure()
        {
            // Arrange
            CultureInfo cultureInfo = new("th-TH");
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                currentCulture: cultureInfo,
                cultures: [cultureInfo]
            ).Value;

            string input = "unknown";

            // Act
            Result<string> result = translationService.Translate(input);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo(String.Format(UNSUPPORTED_LANGUAGE_ERROR_MESSAGE, input, cultureInfo.Name)));
        }

        [Test]
        public void Translate_WithSpecificResource_ShouldReturnTranslatedText()
        {
            // Arrange
            CultureInfo cultureInfo = new("th-TH");
            Type resourceType = typeof(Resource);
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                [cultureInfo]
            ).Value;

            string input = "Spouse";
            // คู่สมรส
            string expectedValue = Helpers.GetTranslatedText(input, resourceType, cultureInfo) ?? input;

            // Act
            Result<string> result = translationService.Translate(input, resourceType);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Value);
        }

        [Test]
        public void Translate_WithInvalidResource_ShouldReturnResultFailure()
        {
            // Arrange
            CultureInfo cultureInfo = new("th-TH");
            Type resourceType = typeof(string);     // Invalid resource
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                [cultureInfo]
            ).Value;

            string input = "Spouse";

            // Act
            Result<string> result = translationService.Translate(input, resourceType);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual($"{resourceType.FullName} resource does not exist.", result.Error);
        }

        [Test]
        public void Translate_IgnoreCase_ShouldReturnTranslatedText()
        {
            // Arrange
            CultureInfo cultureInfo = new("th-TH");
            Type resourceType = typeof(Resource);
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                [cultureInfo]
            ).Value;

            string input = "spouse";
            string resourceKey = "Spouse";
            // คู่สมรส
            string expectedValue = Helpers.GetTranslatedText(resourceKey, resourceType, cultureInfo) ?? input;

            // Act
            Result<string> result = translationService.Translate(input, resourceType);

            // Assert
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expectedValue, result.Value);
        }

        [Test]
        public void Translate_WithSpecificResourceAndUnknown_ShouldReturnTranslatedText()
        {
            // Arrange
            CultureInfo cultureInfo = new("th-TH");
            Type resourceType = typeof(Resource);
            MultiResourcesTranslator translationService = MultiResourcesTranslator.Create
            (
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                [cultureInfo]
            ).Value;

            string input = "unknown";

            // Act
            Result<string> result = translationService.Translate(input, resourceType);

            // Assert
            Assert.IsFalse(result.IsSuccess);
            Assert.That(result.Error, Is.EqualTo(String.Format(UNSUPPORTED_LANGUAGE_FOR_RESOURCE_ERROR_MESSAGE, input, cultureInfo.Name, resourceType.FullName)));
        }
    }
}