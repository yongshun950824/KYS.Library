using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CSharpFunctionalExtensions;
using KYS.Library.Extensions;

namespace KYS.Library.Services
{
    /// <summary>
    /// Defines a service contract for language translation.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Gets the list of <see cref="LanguageResources" /> containing the culture name and its resources.
        /// </summary>
        /// <returns>The list of <see cref="LanguageResources" /> containing the culture name and its resources.</returns>
        List<LanguageResources> GetLanguages();

        /// <summary>
        /// Translate <c>input</c> to English word (by resource key).
        /// </summary>
        /// <param name="input">The value to be translated.</param>
        /// <returns>A <see cref="Result{T}"/> containing the translated English text.</returns>
        Result<string> TranslateToEnglish(string input);

        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="cultureName">The culture chosen for the translation. For example: <c>"en-US"</c>.</param>
        /// <returns>A <see cref="Result{T}"/> containing the translated text.</returns>
        Result<string> Translate(string input, string cultureName);

        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="culture">The culture chosen for the translation. By default, uses the current culture if it is not provided.</param>
        /// <returns>A <see cref="Result{T}"/> containing the translated text.</returns>
        Result<string> Translate(string input, CultureInfo culture = null);
    }

    /// <summary>
    /// An abstract class for the translation service. 
    /// </summary>
    public abstract class BaseTranslator : ITranslator
    {
        private static readonly string[] ENGLISH_CULTURES = ["en", "en-US"];

        /// <summary>
        /// Initialize the base translation service.
        /// </summary>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        protected BaseTranslator(CultureInfo currentCulture = null, List<CultureInfo> cultures = null)
        {
            Cultures = InitCultureInfos(cultures);
            CurrentCulture = InitCurrentCulture(currentCulture);
        }

        /// <summary>
        /// Gets the current culture.
        /// </summary>
        public CultureInfo CurrentCulture { get; init; }

        /// <summary>
        /// Get the list of available cultures.
        /// </summary>
        public List<CultureInfo> Cultures { get; init; }

        /// <summary>
        /// Get the list of available cultures.
        /// </summary>
        public List<LanguageResources> LanguageResources { get; private set; }

        /// <summary>
        /// Gets the lookup for translations.
        /// </summary>
        protected Dictionary<string, Dictionary<string, string>> CultureLookups { get; private set; }

        /// <summary>
        /// Gets the lookup for English translations.
        /// </summary>
        protected Dictionary<string, string> EnglishLookup { get; private set; }

        /// <summary>
        /// Gets the lookup for resource translations.
        /// </summary>
        protected Dictionary<string, Dictionary<string, Dictionary<string, string>>> ResourceLookups { get; private set; }

        /// <summary>
        /// Initialize provided culture as current culture. Otherwise, use current culture of the system.
        /// </summary>
        /// <param name="currentCulture">Provided culture to be used as current culture.</param>
        /// <returns>The <see cref="CultureInfo"/> instance if provided or current culture.</returns>
        protected static CultureInfo InitCurrentCulture(CultureInfo currentCulture)
        {
            return currentCulture ?? CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Initialize supported cultures. Otherwise, use all available cultures.
        /// </summary>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        /// <returns>The list of available culture(s).</returns>
        protected static List<CultureInfo> InitCultureInfos(List<CultureInfo> cultures)
        {
            if (cultures.IsNullOrEmpty())
            {
                return CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .ToList();
            }

            cultures.Insert(0, CultureInfo.InvariantCulture);

            return cultures;
        }

        /// <summary>
        /// Get the list of available languages and their resources.
        /// </summary> 
        /// <returns>The list of available languages and their resources.</returns>
        public List<LanguageResources> GetLanguages()
        {
            return LanguageResources;
        }

        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="cultureName">The culture chosen for the translation. For example: <c>"en-US"</c>.</param>
        /// <returns>A <see cref="Result{T}"/> containing the translated text.</returns>
        public Result<string> Translate(string input, string cultureName)
        {
            CultureInfo culture = null;
            if (!String.IsNullOrEmpty(cultureName))
            {
                culture = new CultureInfo(cultureName);
            }

            return Translate(input, culture);
        }

        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="culture">The culture chosen for the translation. By default, uses the current culture if it is not provided.</param>
        /// <returns>A <see cref="Result{T}"/> containing the translated text.</returns>
        public Result<string> Translate(string input, CultureInfo culture = null)
        {
            culture ??= CurrentCulture;

            if (CultureLookups.TryGetValue(culture.Name, out var cultureDict)
                && cultureDict.TryGetValue(input, out string translatedText))
            {
                return Result.Success(translatedText);
            }

            return Result.Failure<string>($"Provided {input} doesn't support for {culture.Name} language translation.");
        }

        /// <summary>
        /// Translate <c>input</c> to English word (by resource key). Return the original value if not found.
        /// </summary>
        /// <param name="input">The value to be translated.</param>
        /// <returns>A <see cref="Result{T}"/> containing the translated English text.</returns>
        public Result<string> TranslateToEnglish(string input)
        {
            if (EnglishLookup.TryGetValue(input, out string translatedText))
            {
                return Result.Success(translatedText);
            }

            return Result.Failure<string>($"Provided {input} doesn't support for English language translation.");
        }

        /// <summary>
        /// Load the resource files available in the provided assembly. 
        /// <br />
        /// By default, load from current assembly when <c>assembly</c> is not provided.
        /// </summary>
        /// <param name="assembly">The assembly which the resource file is located.</param>
        /// <returns>A <see cref="Result{T}"/> containing the list of <see cref="LanguageResources" />.</returns>
        protected abstract Result<List<LanguageResources>> LoadLanguages();

        /// <summary>
        /// Initialize the translator by loading the languages and building the lookups.
        /// </summary>
        /// <typeparam name="T">The type of the translator inherits from <see cref="BaseTranslator"/>.</typeparam>
        /// <param name="translator">The translator to initialize.</param>
        /// <returns>A <see cref="Result{T}"/> containing the translator instance.</returns>
        protected static Result<T> InitializeTranslator<T>(T translator) where T : BaseTranslator
        {
            return Result.Success(translator)
                .Tap(t => t.LoadLanguages())
                .Tap(t => t.LanguageResources = t.LoadLanguages().Value)
                .Tap(t => (translator.CultureLookups, translator.EnglishLookup, translator.ResourceLookups) = BuildLookups(translator.LanguageResources));
        }

        private static (Dictionary<string, Dictionary<string, string>> cultureLookups,
            Dictionary<string, string> englishLookup,
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> resourceLookups)
            BuildLookups(List<LanguageResources> languageResources)
        {
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            Dictionary<string, Dictionary<string, string>> cultureLookups = new(comparer);
            Dictionary<string, string> englishLookup = new(comparer);
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> resourceLookups = new(comparer);

            // Priortize English culture for English lookup
            var orderedLanguages = languageResources
                .OrderByDescending(x => ENGLISH_CULTURES.Contains(x.CultureName))
                .ThenBy(x => x.CultureName)
                .ToList();

            foreach (var language in orderedLanguages)
            {
                var cultureDictionary = new Dictionary<string, string>(comparer);
                var resourceLookup = new Dictionary<string, Dictionary<string, string>>(comparer);

                foreach (var resource in language.Resources)
                {
                    foreach (var kvp in resource.Value)
                    {
                        // key: text in English, value: resource value in specific culture 
                        cultureDictionary[kvp.Key] = kvp.Value;
                        // key: resource value in specific culture, value: text in English
                        englishLookup[kvp.Value] = kvp.Key;
                    }

                    // key: resource file name, value: key-value pair of text in English and resource value in specific culture
                    resourceLookup[resource.Key] = cultureDictionary;
                }

                cultureLookups[language.CultureName] = cultureDictionary;
                resourceLookups[language.CultureName] = resourceLookup;
            }

            return (cultureLookups, englishLookup, resourceLookups);
        }
    }

    /// <summary>
    /// Represents the blueprint for the culture name and its resources.
    /// </summary>
    public sealed class LanguageResources
    {
        /// <summary>
        /// Gets or sets the culture name.
        /// </summary>
        public string CultureName { get; set; }
        /// <summary>
        /// Gets or sets the key-value pairs with the resource file name and its languages in key-value pair. 
        /// </summary>
        public Dictionary<string, Dictionary<string, string>> Resources { get; set; }
    }
}