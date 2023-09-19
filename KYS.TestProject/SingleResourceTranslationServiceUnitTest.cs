﻿using KYS.Library.Services;
using KYS.TestProject.Resources;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
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
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
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
        public void GetLanguagesWithSecondConstructor()
        {
            // Arrange
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
            string notExpectedValue = JsonSerializer.Serialize(new { }, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });   // Empty object

            // Act
            var languages = translationService.GetLanguages();
            string serializedLanguageObj = JsonSerializer.Serialize(languages, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
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
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource).FullName,
                Assembly.GetExecutingAssembly(),
                new CultureInfo("th-TH")
            );
            string notExpectedValue = JsonSerializer.Serialize(new { }, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });   // Empty object

            // Act
            var languages = translationService.GetLanguages();
            string serializedLanguageObj = JsonSerializer.Serialize(languages, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            });
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
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);
            ITranslationService translationService = new SingleResourceTranslationService(
                resourceType,
                cultureInfo
            );
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
            CultureInfo cultureInfo = new CultureInfo("th-TH");
            Type resourceType = typeof(Resource);

            ITranslationService translationService = new SingleResourceTranslationService(
                resourceType
            );
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
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                new CultureInfo("th-TH")
            );
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
            ITranslationService translationService = new SingleResourceTranslationService(
                typeof(Resource),
                cultureInfo
            );
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
            ITranslationService translationService = new SingleResourceTranslationService(
                resourceType,
                cultureInfo
            );
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
            ITranslationService translationService = new SingleResourceTranslationService(
                resourceType,
                cultureInfo,
                new List<CultureInfo> { cultureInfo }
            );
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