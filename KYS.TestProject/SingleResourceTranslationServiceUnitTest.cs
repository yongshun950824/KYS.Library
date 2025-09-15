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
        public void NotInitializeCurrentCulture()
        {
            // Arrange
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource)
            );
#pragma warning restore S3242
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
#pragma warning disable S3242
            CultureInfo expectedValue = new CultureInfo("th-TH");
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                expectedValue
            );
#pragma warning restore S3242

            // Act
            var actualValue = translationService.CurrentCulture;

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void NotInitializeCultureInfos()
        {
            // Arrange
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
#pragma warning restore S3242

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
            CultureInfo cultureInfo = new CultureInfo("th-TH");
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );
#pragma warning restore S3242

            var expectedValue = new List<CultureInfo> { CultureInfo.InvariantCulture, cultureInfo };

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
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
#pragma warning restore S3242

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
        public void GetLanguagesWithThirdConstructor()
        {
            // Arrange
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource).FullName,
                Assembly.GetExecutingAssembly(),
                new CultureInfo("th-TH")
            );
#pragma warning restore S3242

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
        public void TranslateForChildInEnglish()
        {
            // Arrange
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
#pragma warning restore S3242

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
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
#pragma warning restore S3242

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
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
#pragma warning restore S3242

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
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                resourceType,
                cultureInfo
            );
#pragma warning restore S3242

            string input = "Spouse";
            //string expectedValue = "คู่สมรส";
            string expectedValue = Helpers.GetTranslatedText(input, resourceType, cultureInfo) ?? input;

            // Act
            string actualValue = translationService.Translate(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void TranslateSpouseToUnsupportedCultureAndReturnOriginalValue()
        {
            // Arrange
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
#pragma warning restore S3242

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
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);

#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                resourceType
            );
#pragma warning restore S3242

            string input = "Spouse";
            //string expectedValue = "คู่สมรส";
            string expectedValue = Helpers.GetTranslatedText(input, resourceType, cultureInfo) ?? input;

            // Act
            string actualValue = translationService.Translate(input, culture: cultureInfo);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void TranslateSpouseToDefaultCultureAndReturnOriginalValue()
        {
            // Arrange
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
#pragma warning restore S3242

            string input = "unknown";
            string expectedValue = "unknown";

            // Act
            string actualValue = translationService.Translate(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Test]
        public void TranslateSpouseToDefaultCultureAndThrowException()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                cultureInfo
            );
#pragma warning restore S3242

            string input = "unknown";
            ArgumentNullException expectedEx = new ArgumentNullException($"Provided {input} doesn't support for {cultureInfo.Name} language translation.");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => translationService.Translate(input, false));

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void TranslateSpouseWithSpecificResourceAndThrowNotSupportException()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                resourceType,
                cultureInfo
            );
#pragma warning restore S3242

            string input = "unknown";
            NotSupportedException expectedEx = new NotSupportedException($"Translate with specified resource is not supported in {nameof(SingleResourceTranslationService)}");

            // Act
            var ex = Assert.Throws<NotSupportedException>(() => translationService.Translate(input, resourceType));

            // Assert
            Assert.That(ex.Message, Is.EqualTo(expectedEx.Message));
        }

        [Test]
        public void TranslateSpouseIgnoreCase()
        {
            // Arrange
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
#pragma warning disable S3242
            using ITranslationService translationService = new SingleResourceTranslationService(
                resourceType,
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );
#pragma warning restore S3242

            string input = "spouse";
            string resourceKey = "Spouse";
            //string expectedValue = "คู่สมรส";
            string expectedValue = Helpers.GetTranslatedText(resourceKey, resourceType, cultureInfo) ?? input;

            // Act
            string actualValue = translationService.Translate(input);

            // Assert
            Assert.AreEqual(expectedValue, actualValue);
        }
    }
}