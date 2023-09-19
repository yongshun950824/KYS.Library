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

namespace KYS.TestProject
{
    public class SingleResourceTranslationServiceUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void NotInitializeCurrentCulture()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource)
            );
            CultureInfo expectedValue = CultureInfo.CurrentCulture;

            // Act
            var actualValue = translationService.CurrentCulture;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void InitializeThaiCulture()
        {
            // Arrange
            CultureInfo expectedValue = new CultureInfo("th-TH");
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                expectedValue
            );

            // Act
            var actualValue = translationService.CurrentCulture;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void NotInitializeCultureInfos()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
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
        public void InitializeCultureInfos()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH"),
                new List<CultureInfo> { new CultureInfo("th-TH") }
            );
            var expectedValue = new List<CultureInfo> { CultureInfo.InvariantCulture, new CultureInfo("th-TH") };

            // Act
            var actualValue = translationService.Cultures;

            // Assert
            Assert.AreEqual(expectedValue.Count, actualValue.Count);
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void GetLanguagesWithSecondConstructor()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
            string notExpectedValue = JsonSerializer.Serialize("{}");   // Empty object

            // Act
            var languages = translationService.GetLanguages();
            string serializedLanguageObj = JsonSerializer.Serialize(languages, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            // Assert
            Assert.IsNotNull(languages);
            Assert.AreNotEqual(notExpectedValue, serializedLanguageObj);
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "th-TH"));
        }

        [Test]
        public void GetLanguagesWithThirdConstructor()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource).FullName,
                Assembly.GetExecutingAssembly(),
                new CultureInfo("th-TH")
            );
            string notExpectedValue = JsonSerializer.Serialize("{}");   // Empty object

            // Act
            var languages = translationService.GetLanguages();
            string serializedLanguageObj = JsonSerializer.Serialize(languages, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            // Assert
            Assert.IsNotNull(languages);
            Assert.AreNotEqual(notExpectedValue, serializedLanguageObj);
            Assert.IsNotNull(languages.FirstOrDefault(x => x.CultureName == "th-TH"));
        }

        [Test]
        public void TranslateForChildInEnglish()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
            string input = "เด็ก";
            string expectedValue = "Child";

            // Act
            string actualValue = translationService.TranslateToEnglish(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void TranslateUnknownInEnglishAndReturnOriginalValue()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
            string input = "unknown";
            string expectedValue = "unknown";

            // Act
            string actualValue = translationService.TranslateToEnglish(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void TranslateUnknownInEnglishAndThrowException()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
            string input = "unknown";
            ArgumentNullException expectedEx = new ArgumentNullException($"Provided {input} doesn't support for English language translation.");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => translationService.TranslateToEnglish(input, false));

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void TranslateSpouseToDefaultCulture()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
            string input = "Spouse";
            string expectedValue = "คู่สมรส";

            // Act
            string actualValue = translationService.Translate(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void TranslateSpouseToUnsupportedCultureAndReturnOriginalValue()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
            string input = "Spouse";
            string cultureName = "en-MY";
            string expectedValue = "Spouse";

            // Act
            string actualValue = translationService.Translate(input, cultureName: cultureName);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void TranslateSpouseToSpecificCulture()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource)
            );
            string input = "Spouse";
            CultureInfo culture = new CultureInfo("th-TH");
            string expectedValue = "คู่สมรส";

            // Act
            string actualValue = translationService.Translate(input, culture: culture);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}