using KYS.Library.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace KYS.Library.Services
{
    /// <summary>
    /// Defines a service contract for language translation.
    /// </summary>
    public interface IBaseTranslationService
    {
        #region Properties
        /// <summary>
        /// Gets the current culture.
        /// </summary>
        CultureInfo CurrentCulture { get; }
        /// <summary>
        /// Gets the list of available cultures.
        /// </summary>
        List<CultureInfo> Cultures { get; }
        #endregion
    }

    /// <summary>
    /// Defines a service contract for language translation.
    /// </summary>
    public interface ITranslationService : IBaseTranslationService
    {
        /// <summary>
        /// Gets the list of <see cref="LanguageResources" /> containing the culture name and its resources.
        /// </summary>
        /// <returns>The list of <see cref="LanguageResources" /> containing the culture name and its resources.</returns>
        List<LanguageResources> GetLanguages();
        /// <summary>
        /// Translate <c>input</c> to English word (by resource key). Return the original value if not found.
        /// </summary>
        /// <param name="input">The value to be translated.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <returns>Translated to English text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        string TranslateToEnglish(string input, bool isReturnedOriginalValue = true);
        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <param name="cultureName">The culture chosen for the translation. For example: <c>"en-US"</c>.</param>
        /// <returns>Translated text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        string Translate(string input, string cultureName, bool isReturnedOriginalValue = true);
        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <param name="culture">The culture chosen for the translation. By default, uses the current culture if it is not provided.</param>
        /// <returns>Translated text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        string Translate(string input, bool isReturnedOriginalValue = true, CultureInfo culture = null);
        /// <summary>
        /// Translate to selected language for <b>specified resource</b> and provided <c>culture</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="resourceType">The resource file to be used.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <param name="culture">The culture chosen for the translation. By default, uses the current culture if it is not provided.</param>
        /// <returns>Translated text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        string Translate(string input, Type resourceType, bool isReturnedOriginalValue = true, CultureInfo culture = null);
    }

    /// <summary>
    /// An abstract class for the translation service. 
    /// </summary>
    public abstract class BaseTranslationService : IBaseTranslationService
    {
        protected readonly List<CultureInfo> _cultures;
        protected readonly CultureInfo _currentCulture;

        /// <summary>
        /// Initialize the base translation service.
        /// </summary>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        protected BaseTranslationService(CultureInfo currentCulture = null, List<CultureInfo> cultures = null)
        {
            _cultures = InitCultureInfos(cultures);
            _currentCulture = InitCurrentCulture(currentCulture);
        }

        /// <summary>
        /// Gets the current culture.
        /// </summary>
        public CultureInfo CurrentCulture
        {
            get { return _currentCulture; }
        }

        /// <summary>
        /// Get the list of available cultures.
        /// </summary>
        public List<CultureInfo> Cultures
        {
            get { return _cultures; }
        }

        /// <summary>
        /// Initialize provided culture as current culture. Otherwise, use current culture of the system.
        /// </summary>
        /// <param name="currentCulture">Provided culture to be used as current culture.</param>
        /// <returns>The <c>CultureInfo</c> instance if provided or current culture.</returns>
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
        /// Load the resource files available in the provided assembly. 
        /// <br />
        /// By default, load from current assembly when <c>assembly</c> is not provided.
        /// </summary>
        /// <param name="assembly">The assembly which the resource file is located.</param>
        /// <returns>The list of <see cref="LanguageResources" />.</returns>
        protected abstract List<LanguageResources> LoadLanguages(Assembly assembly = null);
    }

    /// <summary>
    /// A service for translation with support only one resource (file).
    /// </summary>
    public sealed class SingleResourceTranslationService : BaseTranslationService, ITranslationService
    {
        private readonly List<LanguageResources> _resourceLanguages;
        private readonly ResourceManager _resourceManager;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResourceTranslationService"/> class.
        /// </summary>
        /// <param name="resourceType">The type for the resource file.</param>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        public SingleResourceTranslationService(Type resourceType,
            CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null) : base(currentCulture, cultures)
        {
            _resourceManager = new ResourceManager(resourceType);
            _resourceLanguages = LoadLanguages();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResourceTranslationService"/> class.
        /// </summary>
        /// <param name="baseName">The name of the resource file.</param>
        /// <param name="assembly">The assembly which the resource file is located.</param>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        public SingleResourceTranslationService(string baseName,
            Assembly assembly,
            CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null) : base(currentCulture, cultures)
        {
            _resourceManager = new ResourceManager(baseName, assembly);
            _resourceLanguages = LoadLanguages();
        }
        #endregion

        public List<LanguageResources> GetLanguages()
        {
            return _resourceLanguages;
        }

        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <param name="cultureName">The culture chosen for the translation. For example: <c>"en-US"</c>.</param>
        /// <returns>Translated text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        public string Translate(string input, string cultureName, bool isReturnedOriginalValue = true)
        {
            CultureInfo culture = null;
            if (!String.IsNullOrEmpty(cultureName))
            {
                culture = new CultureInfo(cultureName);
            }

            return Translate(input, isReturnedOriginalValue, culture);
        }

        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <param name="culture">The culture chosen for the translation. By default, uses the current culture if it is not provided.</param>
        /// <returns>Translated text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        public string Translate(string input, bool isReturnedOriginalValue = true, CultureInfo culture = null)
        {
            culture ??= _currentCulture;

            List<KeyValuePair<string, string>> langKvps = _resourceLanguages
                .Where(x => x.CultureName == culture.Name)
                .SelectMany(x => x.Resources)
                .SelectMany(x => x.Value.ToList())
                .ToList();

            string translatedText = langKvps.FirstOrDefault(x => x.Key.Equals(input, StringComparison.OrdinalIgnoreCase))
                .Value;
            if (String.IsNullOrEmpty(translatedText) && !isReturnedOriginalValue)
            {
                throw new ArgumentNullException($"Provided {input} doesn't support for {culture.Name} language translation.");
            }

            return !String.IsNullOrEmpty(translatedText)
                ? translatedText
                : input;
        }

        /// <summary>
        /// Not supported for translation with specified resource.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="resourceType"></param>
        /// <param name="isReturnedOriginalValue"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public string Translate(string input, Type resourceType, bool isReturnedOriginalValue = true, CultureInfo culture = null)
        {
            throw new NotSupportedException($"Translate with specified resource is not supported in {nameof(SingleResourceTranslationService)}");
        }

        /// <summary>
        /// Translate <c>input</c> to English word (by resource key). Return the original value if not found.
        /// </summary>
        /// <param name="input">The value to be translated.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <returns>Translated to English text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        public string TranslateToEnglish(string input, bool isReturnedOriginalValue = true)
        {
            List<KeyValuePair<string, string>> langKvps = _resourceLanguages
                .SelectMany(x => x.Resources)
                .SelectMany(x => x.Value.ToList())
                .ToList();

            string translatedText = langKvps.FirstOrDefault(x => x.Value == input).Key;
            if (String.IsNullOrEmpty(translatedText) && !isReturnedOriginalValue)
            {
                throw new ArgumentNullException($"Provided {input} doesn't support for English language translation.");
            }

            return !String.IsNullOrEmpty(translatedText)
                ? translatedText
                : input;
        }

        protected sealed override List<LanguageResources> LoadLanguages(Assembly assembly = null)
        {
            List<LanguageResources> languageValues = new List<LanguageResources>();

            foreach (CultureInfo culture in _cultures)
            {
                try
                {
                    ResourceSet rs = _resourceManager.GetResourceSet(culture, true, false);
                    if (rs != null)
                    {
                        var resource = rs.Cast<DictionaryEntry>()
                            .ToDictionary(k => k.Key.ToString(), v => v.Value.ToString());

                        languageValues.Add(new LanguageResources
                        {
                            CultureName = culture.Name,
                            Resources = new Dictionary<string, Dictionary<string, string>>
                            {
                                { _resourceManager.BaseName, resource }
                            }
                        });
                    }
                }
                catch (CultureNotFoundException)
                {
                    // Safe to ignore CultureNotFoundException
                }
            }

            return languageValues;
        }
    }

    /// <summary>
    /// A service for transaction with supports multiple resources (file).
    /// </summary>
    public sealed class MultiResourcesTranslationService : BaseTranslationService, ITranslationService
    {
        private readonly List<LanguageResources> _resourceLanguages;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiResourcesTranslationService"/> class.
        /// </summary>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        public MultiResourcesTranslationService(CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null) : base(currentCulture, cultures)
        {
            _resourceLanguages = LoadLanguages(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiResourcesTranslationService"/> class.
        /// </summary>
        /// <param name="assembly">The assembly which the resource files are located.</param>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        public MultiResourcesTranslationService(Assembly assembly,
            CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null) : base(currentCulture, cultures)
        {
            _resourceLanguages = LoadLanguages(assembly);
        }

        public List<LanguageResources> GetLanguages()
        {
            return _resourceLanguages;
        }

        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <param name="cultureName">The culture chosen for the translation. For example: <c>"en-US"</c>.</param>
        /// <returns>Translated text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        public string Translate(string input, string cultureName, bool isReturnedOriginalValue = true)
        {
            CultureInfo culture = null;
            if (!String.IsNullOrEmpty(cultureName))
            {
                culture = new CultureInfo(cultureName);
            }

            return Translate(input, isReturnedOriginalValue, culture);
        }

        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <param name="culture">The culture chosen for the translation. By default, uses the current culture if it is not provided.</param>
        /// <returns>Translated text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        public string Translate(string input, bool isReturnedOriginalValue = true, CultureInfo culture = null)
        {
            culture ??= _currentCulture;

            List<KeyValuePair<string, string>> langKvps = _resourceLanguages
                .Where(x => x.CultureName == culture.Name)
                .SelectMany(x => x.Resources)
                .SelectMany(x => x.Value.ToList())
                .ToList();

            string translatedText = langKvps.FirstOrDefault(x => x.Key.Equals(input, StringComparison.OrdinalIgnoreCase)).Value;
            if (String.IsNullOrEmpty(translatedText) && !isReturnedOriginalValue)
            {
                throw new ArgumentNullException($"Provided {input} doesn't support for {culture.Name} language translation.");
            }

            return !String.IsNullOrEmpty(translatedText)
                ? translatedText
                : input;
        }

        /// <summary>
        /// Translate to selected language for <b>specified resource</b> and provided <c>culture</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="resourceType">The resource file to be used.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <param name="culture">The culture chosen for the translation. By default, uses the current culture if it is not provided.</param>
        /// <returns>Translated to English text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public string Translate(string input, Type resourceType, bool isReturnedOriginalValue = true, CultureInfo culture = null)
        {
            culture ??= _currentCulture;

            bool isResourceExisted = _resourceLanguages.SelectMany(x => x.Resources)
                .Any(x => x.Key == resourceType.FullName);
            if (!isResourceExisted)
            {
                throw new ArgumentException($"{resourceType.FullName} resource does not existed.");
            }

            List<KeyValuePair<string, string>> langKvps = _resourceLanguages
                .Where(x => x.CultureName == culture.Name)
                .SelectMany(x => x.Resources)
                .Where(x => x.Key == resourceType.FullName)
                .SelectMany(x => x.Value.ToList())
                .ToList();

            string translatedText = langKvps.FirstOrDefault(x => x.Key.Equals(input, StringComparison.OrdinalIgnoreCase))
                .Value;
            if (String.IsNullOrEmpty(translatedText) && !isReturnedOriginalValue)
            {
                throw new ArgumentNullException($"Provided {input} doesn't support for {culture.Name} language translation.");
            }

            return !String.IsNullOrEmpty(translatedText)
                ? translatedText
                : input;
        }

        /// <summary>
        /// Translate <c>input</c> to English word (by resource key). Return the original value if not found.
        /// </summary>
        /// <param name="input">The value to be translated.</param>
        /// <param name="isReturnedOriginalValue">If <see langword="false"/>, <see cref="ArgumentNullException" /> will be thrown if unable to translate.</param>
        /// <returns>Translated to English text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        public string TranslateToEnglish(string input, bool isReturnedOriginalValue = true)
        {
            List<KeyValuePair<string, string>> langKvps = _resourceLanguages
                .SelectMany(x => x.Resources)
                .SelectMany(x => x.Value.ToList())
                .ToList();

            string translatedText = langKvps.FirstOrDefault(x => x.Value == input).Key;
            if (String.IsNullOrEmpty(translatedText) && !isReturnedOriginalValue)
            {
                throw new ArgumentNullException($"Provided {input} doesn't support for English language translation.");
            }

            return !String.IsNullOrEmpty(translatedText)
                ? translatedText
                : input;
        }

        protected sealed override List<LanguageResources> LoadLanguages(Assembly assembly = null)
        {
            List<LanguageResources> languageValues = new List<LanguageResources>();

            // Get all resources with remove postfix: ".resource"
            string[] resNames = assembly.GetManifestResourceNames()
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToArray();

            foreach (CultureInfo culture in _cultures)
            {
                Dictionary<string, Dictionary<string, string>> resources = new();

                foreach (var name in resNames)
                {
                    var resourceManager = new ResourceManager(name, assembly);

                    try
                    {
                        ResourceSet rs = resourceManager.GetResourceSet(culture, true, false);
                        if (rs != null)
                        {
                            var resource = rs.Cast<DictionaryEntry>()
                                .ToDictionary(k => k.Key.ToString(), v => v.Value.ToString());

                            resources.Add(resourceManager.BaseName, resource);
                        }
                    }
                    catch (CultureNotFoundException)
                    {
                        // Safe to ignore CultureNotFoundException
                    }
                }

                languageValues.Add(new LanguageResources
                {
                    CultureName = culture.Name,
                    Resources = resources
                });
            }

            return languageValues;
        }
    }

    /// <summary>
    /// Represents the blueprint for the culture name and its resources.
    /// </summary>
    public class LanguageResources
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
