using KYS.Library.Services;
using KYS.TestProject.Resources;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace KYS.TestProject.ServicesUnitTests
{
    public class MultiResourcesTranslationServiceUnitTest
    {
        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Constructor_WithoutInitializeCulture_ShouldGetCurrentCulture()
        {
            // Arrange
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly()
            );
            CultureInfo expectedValue = CultureInfo.CurrentCulture;

            // Act
            var actualValue = translationService.CurrentCulture;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Constructor_WithInitializeThaiCulture_ShouldGetCorrectCulture()
        {
            // Arrange
            CultureInfo expectedValue = new CultureInfo("th-TH");
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                expectedValue
            );

            // Act
            var actualValue = translationService.CurrentCulture;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Constructor_WithoutInitializeCultureInfos_ShouldGetAllCultures()
        {
            // Arrange
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                new CultureInfo("th-TH")
            );

            var expectedValue = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .ToList();

            // Act
            var actualValue = translationService.Cultures;

            // Assert
            Assert.AreEqual(expectedValue.Count, actualValue.Count);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Constructor_WithInitializeCultureInfos_ShouldGetCurrentCutures()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );

            var expectedValue = new List<CultureInfo> { CultureInfo.InvariantCulture, cultureInfo };

            // Act
            var actualValue = translationService.Cultures;

            // Assert
            Assert.AreEqual(expectedValue.Count, actualValue.Count);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Constructor_WithFirstConstructor_ShouldGetLanguages()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );

            string notExpectedValue = JsonSerializer.Serialize(new { }, _serializerOptions);   // Empty object

            // Act
            var languages = translationService.GetLanguages();
            string serializedLanguageObj = JsonSerializer.Serialize(languages, _serializerOptions);
            Console.WriteLine(serializedLanguageObj);

            // Assert
            Assert.IsNotNull(languages);
            Assert.AreNotEqual(notExpectedValue, serializedLanguageObj);
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "th-TH"));
        }

        [Test]
        public void Constructor_WithSecondConstructor_ShouldGetLanguages()
        {
            // Arrange
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                new CultureInfo("th-TH"),
                new List<CultureInfo> { new CultureInfo("th-TH"), new CultureInfo("zh-CN") }
            );

            string notExpectedValue = JsonSerializer.Serialize(new { }, _serializerOptions);   // Empty object

            // Act
            var languages = translationService.GetLanguages();
            string serializedLanguageObj = JsonSerializer.Serialize(languages, _serializerOptions);
            Console.WriteLine(serializedLanguageObj);

            // Assert
            Assert.IsNotNull(languages);
            Assert.AreNotEqual(notExpectedValue, serializedLanguageObj);
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "th-TH"));
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "zh-CN"));
        }

        [Test]
        public void TranslateToEnglish_ShouldReturnTranslatedEnglishText()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );

            string input = "เด็ก";
            string expectedValue = "Child";

            // Act
            string actualValue = translationService.TranslateToEnglish(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void TranslateToEnglish_WithUnknown_ShouldReturnOriginalValue()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );

            string input = "unknown";
            string expectedValue = "unknown";

            // Act
            string actualValue = translationService.TranslateToEnglish(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void TranslateToEnglish_WithUnknownAndDisableReturnedOriginalValue_ShouldThrowException()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );

            string input = "unknown";
            ArgumentNullException expectedEx = new ArgumentNullException($"Provided {input} doesn't support for English language translation.");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => translationService.TranslateToEnglish(input, false));

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void Translate_WithoutCulture_ShouldReturnDefaultCultureText()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );

            string input = "Spouse";
            // "คู่สมรส"
            string expectedValue = Helpers.GetTranslatedText(input, resourceType, cultureInfo) ?? input;

            // Act
            string actualValue = translationService.Translate(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Translate_ToUnsupportedCulture_ShouldReturnOriginalValue()
        {
            // Arrange
            CultureInfo culture = new CultureInfo("th-TH");
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                culture,
                new List<CultureInfo> { culture }
            );

            string input = "Spouse";
            string cultureName = "en-MY";
            string expectedValue = "Spouse";

            // Act
            string actualValue = translationService.Translate(input, cultureName);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Translate_ToSpecificCulture_ShouldReturnTranslatedText()
        {
            // Arrange
            CultureInfo culture = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                cultures: new List<CultureInfo> { culture }
            );

            string input = "Spouse";
            // "คู่สมรส"
            string expectedValue = Helpers.GetTranslatedText(input, resourceType, culture) ?? input;

            // Act
            string actualValue = translationService.Translate(input, culture: culture);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Translate_ToDefaultCulture_ShouldReturnOriginalValue()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                currentCulture: cultureInfo,
                cultures: new List<CultureInfo> { cultureInfo }
            );

            string input = "unknown";
            string expectedValue = "unknown";

            // Act
            string actualValue = translationService.Translate(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Translate_WithDisabledReturnedOriginalValueAndToDefaultCulture_ShouldThrowException()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                currentCulture: cultureInfo,
                cultures: new List<CultureInfo> { cultureInfo }
            );

            string input = "unknown";
            ArgumentNullException expectedEx = new ArgumentNullException($"Provided {input} doesn't support for {cultureInfo.Name} language translation.");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => translationService.Translate(input, false));

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void Translate_WithSpecificResource_ShouldReturnTranslatedText()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );

            string input = "Spouse";
            // คู่สมรส
            string expectedValue = Helpers.GetTranslatedText(input, resourceType, cultureInfo) ?? input;

            // Act
            string actualValue = translationService.Translate(input, resourceType);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void Translate_WithInvalidResource_ShouldThrowException()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(string);     // Invalid resource
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );

            string input = "Spouse";
            ArgumentException expectedEx = new ArgumentException($"{resourceType.FullName} resource does not existed.");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => translationService.Translate(input, resourceType));

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void Translate_WithResourceAndDisabledReturnedOriginalValue_ShouldThrowException()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );

            string input = "unknown";
            ArgumentNullException expectedEx = new ArgumentNullException($"Provided {input} doesn't support for {cultureInfo.Name} language translation.");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => translationService.Translate(input, resourceType, false));

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }


        [Test]
        public void Translate_IgnoreCase_ShouldReturnTranslatedText()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
            MultiResourcesTranslationService translationService = new MultiResourcesTranslationService(
                Assembly.GetExecutingAssembly(),
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );

            string input = "spouse";
            string resourceKey = "Spouse";
            // คู่สมรส
            string expectedValue = Helpers.GetTranslatedText(resourceKey, resourceType, cultureInfo) ?? input;

            // Act
            string actualValue = translationService.Translate(input, resourceType);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}