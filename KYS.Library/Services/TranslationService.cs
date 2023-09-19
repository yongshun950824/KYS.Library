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
    public interface ITranslationService
    {
        List<LanguageResource> GetLanguages();
        string TranslateToEnglish(string input, bool isReturnedOriginalValue = true);
        string Translate(string input);
        string Translate(string input, bool isReturnedOriginalValue = true);
        string Translate(string input, bool isReturnedOriginalValue = true, string cultureName = null);
        string Translate(string input, bool isReturnedOriginalValue = true, CultureInfo culture = null);

        #region Properties
        CultureInfo CurrentCulture { get; }
        List<CultureInfo> Cultures { get; }
        #endregion
    }

    public class SingleResourceTranslationService : ITranslationService, IDisposable
    {
        private readonly List<LanguageResource> _resourceLanguages = new List<LanguageResource>();
        private readonly List<CultureInfo> _cultures = new List<CultureInfo>();
        private readonly CultureInfo _currentCulture;
        private readonly ResourceManager _resourceManager;

        #region Constructors
        public SingleResourceTranslationService()
        {
            _cultures = InitCultureInfos(null);
            _currentCulture = CultureInfo.CurrentCulture;
        }

        public SingleResourceTranslationService(Type resourceType, CultureInfo currentCulture = null, List<CultureInfo> cultures = null)
        {
            _cultures = InitCultureInfos(cultures);
            _currentCulture = InitCurrentCulture(currentCulture);

            _resourceManager = new ResourceManager(resourceType);
            _resourceLanguages = LoadLanguages();
        }

        public SingleResourceTranslationService(string baseName, Assembly assembly, CultureInfo currentCulture = null, List<CultureInfo> cultures = null)
        {
            _cultures = InitCultureInfos(cultures);
            _currentCulture = InitCurrentCulture(currentCulture);

            _resourceManager = new ResourceManager(baseName, assembly);
            _resourceLanguages = LoadLanguages();
        }
        #endregion

        public CultureInfo CurrentCulture
        {
            get { return _currentCulture; }
        }

        public List<CultureInfo> Cultures
        {
            get { return _cultures; }
        }

        public List<LanguageResource> GetLanguages()
        {
            return _resourceLanguages;
        }

        /// <summary>
        /// Translate <c>input</c> to English word. Return the original value if not found.
        /// </summary>
        /// <param name="input"></param>
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

        /// <summary>
        /// Translate to current/initialized language.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Translate(string input)
        {
            return Translate(input, true, _currentCulture);
        }

        /// <summary>
        /// Translate to current/initialized language.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isReturnedOriginalValue"></param>
        /// <returns></returns>
        public string Translate(string input, bool isReturnedOriginalValue = true)
        {
            return Translate(input, isReturnedOriginalValue, _currentCulture);
        }

        /// <summary>
        /// Translate to selected language for provided <c>cultureName</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="isReturnedOriginalValue"></param>
        /// <param name="cultureName"></param>
        /// <returns></returns>
        public string Translate(string input, bool isReturnedOriginalValue = true, string cultureName = null)
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
        /// <param name="isReturnedOriginalValue"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string Translate(string input, bool isReturnedOriginalValue = true, CultureInfo culture = null)
        {
            culture ??= _currentCulture;

            ResourceSet rs = _resourceManager.GetResourceSet(culture, true, false);

            var obj = rs?.GetObject(input);
            if (obj == null && !isReturnedOriginalValue)
            {
                throw new ArgumentNullException($"Provided {input} doesn't support for language translation.");
            }

            return !String.IsNullOrEmpty(obj?.ToString())
                ? obj.ToString()
                : input;
        }

        /// <summary>
        /// Initialize provided culture as current culture. Otherwise, use current culture of the system.
        /// </summary>
        /// <param name="currentCulture"></param>
        /// <returns></returns>
        private static CultureInfo InitCurrentCulture(CultureInfo currentCulture)
        {
            return currentCulture ?? CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Initialize supported cultures. Otherwise, use all available cultures.
        /// </summary>
        /// <param name="cultures"></param>
        /// <returns></returns>
        private static List<CultureInfo> InitCultureInfos(List<CultureInfo> cultures)
        {
            if (cultures.IsNullOrEmpty())
            {
                return CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .ToList();
            }

            cultures.Insert(0, CultureInfo.InvariantCulture);

            return cultures;
        }

        private List<LanguageResource> LoadLanguages()
        {
            List<LanguageResource> languageValues = new List<LanguageResource>();

            foreach (CultureInfo culture in _cultures)
            {
                try
                {
                    ResourceSet rs = _resourceManager.GetResourceSet(culture, true, false);
                    if (rs != null)
                    {
                        var resource = rs.Cast<DictionaryEntry>()
                            .ToDictionary(k => k.Key.ToString(), v => v.Value.ToString());

                        languageValues.Add(new LanguageResource
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

                }
            }

            return languageValues;
        }

        public void Dispose()
        {

        }
    }

    public class LanguageResource
    {
        public string CultureName { get; set; }
        public Dictionary<string, Dictionary<string, string>> Resources { get; set; }
    }
}
