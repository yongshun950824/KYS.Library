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
    public interface IBaseTranslationService
    {
        #region Properties
        CultureInfo CurrentCulture { get; }
        List<CultureInfo> Cultures { get; }
        #endregion
    }

    public interface ITranslationService : IBaseTranslationService
    {
        List<LanguageResources> GetLanguages();
        string TranslateToEnglish(string input, bool isReturnedOriginalValue = true);
        string Translate(string input, string cultureName, bool isReturnedOriginalValue = true);
        string Translate(string input, bool isReturnedOriginalValue = true, CultureInfo culture = null);
        string Translate(string input, Type resourceType, bool isReturnedOriginalValue = true, CultureInfo culture = null);
    }

    public abstract class BaseTranslationService : IBaseTranslationService
    {
        protected readonly List<CultureInfo> _cultures;
        protected readonly CultureInfo _currentCulture;

        protected BaseTranslationService(CultureInfo currentCulture = null, List<CultureInfo> cultures = null)
        {
            _cultures = InitCultureInfos(cultures);
            _currentCulture = InitCurrentCulture(currentCulture);
        }

        public CultureInfo CurrentCulture
        {
            get { return _currentCulture; }
        }

        public List<CultureInfo> Cultures
        {
            get { return _cultures; }
        }

        /// <summary>
        /// Initialize provided culture as current culture. Otherwise, use current culture of the system.
        /// </summary>
        /// <param name="currentCulture"></param>
        /// <returns></returns>
        protected static CultureInfo InitCurrentCulture(CultureInfo currentCulture)
        {
            return currentCulture ?? CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Initialize supported cultures. Otherwise, use all available cultures.
        /// </summary>
        /// <param name="cultures"></param>
        /// <returns></returns>
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

        protected abstract List<LanguageResources> LoadLanguages(Assembly assembly = null);
    }

    public sealed class SingleResourceTranslationService : BaseTranslationService, ITranslationService
    {
        private readonly List<LanguageResources> _resourceLanguages;
        private readonly ResourceManager _resourceManager;

        #region Constructors
        public SingleResourceTranslationService(Type resourceType,
            CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null) : base(currentCulture, cultures)
        {
            _resourceManager = new ResourceManager(resourceType);
            _resourceLanguages = LoadLanguages();
        }

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
        /// <param name="input"></param>
        /// <param name="isReturnedOriginalValue">If <c>false</c>, <c>ArgumentNullException</c> thrown if unable to translate.</param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
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
        /// Translate to selected language for provided <c>culture</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isReturnedOriginalValue">If <c>false</c>, <c>ArgumentNullException</c> thrown if unable to translate.</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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

        public string Translate(string input, Type resourceType, bool isReturnedOriginalValue = true, CultureInfo culture = null)
        {
            throw new NotSupportedException($"Translate with specified resource is not supported in {nameof(SingleResourceTranslationService)}");
        }

        /// <summary>
        /// Translate <c>input</c> to English word (by resource key). Return the original value if not found.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isReturnedOriginalValue">If <c>false</c>, <c>ArgumentNullException</c> thrown if unable to translate.</param>
        /// <returns></returns>
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

    public sealed class MultiResourcesTranslationService : BaseTranslationService, ITranslationService
    {
        private readonly List<LanguageResources> _resourceLanguages;

        public MultiResourcesTranslationService(CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null) : base(currentCulture, cultures)
        {
            _resourceLanguages = LoadLanguages(Assembly.GetExecutingAssembly());
        }

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
        /// <param name="input"></param>
        /// <param name="isReturnedOriginalValue">If <c>false</c>, <c>ArgumentNullException</c> thrown if unable to translate.</param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
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
        /// Translate to selected language for provided <c>culture</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isReturnedOriginalValue">If <c>false</c>, <c>ArgumentNullException</c> thrown if unable to translate.</param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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
        /// <param name="input"></param>
        /// <param name="resourceType"></param>
        /// <param name="isReturnedOriginalValue"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
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
        /// <param name="input"></param>
        /// <param name="isReturnedOriginalValue">If <c>false</c>, <c>ArgumentNullException</c> thrown if unable to translate.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
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

    public class LanguageResources
    {
        public string CultureName { get; set; }
        public Dictionary<string, Dictionary<string, string>> Resources { get; set; }
    }
}
