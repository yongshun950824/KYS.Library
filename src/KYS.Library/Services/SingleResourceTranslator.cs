using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using CSharpFunctionalExtensions;

namespace KYS.Library.Services
{
    /// <summary>
    /// A service for translation with support only one resource (file).
    /// </summary>
    public sealed class SingleResourceTranslator : BaseTranslator
    {
        private readonly ResourceManager _resourceManager;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResourceTranslator"/> class.
        /// </summary>
        /// <param name="resourceType">The type for the resource file.</param>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        private SingleResourceTranslator(Type resourceType,
            CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null) : base(currentCulture, cultures)
        {
            _resourceManager = new ResourceManager(resourceType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleResourceTranslator"/> class.
        /// </summary>
        /// <param name="baseName">The name of the resource file.</param>
        /// <param name="assembly">The assembly which the resource file is located.</param>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        private SingleResourceTranslator(string baseName,
            Assembly assembly,
            CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null) : base(currentCulture, cultures)
        {
            _resourceManager = new ResourceManager(baseName, assembly);
        }
        #endregion

        #region Static Creation Methods
        /// <summary>
        /// Creates an instance of <see cref="SingleResourceTranslator"/> with the specified resource type. The current culture and available cultures will be set to default values.
        /// </summary>
        /// <param name="resourceType">The type for the resource file.</param>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        /// <returns>A <see cref="Result{T}"/> containing the created translator instance.</returns>
        public static Result<SingleResourceTranslator> Create(Type resourceType, CultureInfo currentCulture = null, List<CultureInfo> cultures = null)
        {
            var instance = new SingleResourceTranslator(resourceType, currentCulture, cultures);

            return InitializeTranslator(instance);
        }

        /// <summary>
        /// Creates an instance of <see cref="SingleResourceTranslator"/> with the specified base name and assembly. The current culture and available cultures will be set to default values.
        /// </summary>
        /// <param name="baseName">The name of the resource file.</param>
        /// <param name="assembly">The assembly which the resource file is located.</param>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        /// <returns>A <see cref="Result{T}"/> containing the created translator instance.</returns>
        public static Result<SingleResourceTranslator> Create(string baseName,
            Assembly assembly,
            CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null)
        {
            var instance = new SingleResourceTranslator(baseName, assembly, currentCulture, cultures);

            return InitializeTranslator(instance);
        }
        #endregion

        /// <summary>
        /// Loads the languages and their resources from the assembly. The method iterates through the available cultures and resource files, attempting to load the resources for each culture. If a resource file is not found for a culture, it safely ignores the exception and continues loading other resources. If any other exception occurs during loading, it returns a failure result with the error message.
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing the loaded language resources.</returns>
        protected override Result<List<LanguageResources>> LoadLanguages()
        {
            List<LanguageResources> languageValues = [];

            foreach (CultureInfo culture in Cultures)
            {
                var result = Result.Try(
                    () => LoadResources(_resourceManager, culture),
                    ex => ex
                );

                if (result.IsFailure && result.Error is CultureNotFoundException)
                {
                    // Safe to ignore CultureNotFoundException
                    result = Result.Success<Maybe<LanguageResources>, Exception>(Maybe.None);
                }
                else if (result.IsFailure)
                {
                    return Result.Failure<List<LanguageResources>>(result.Error.Message);
                }

                var resource = result.Value;
                if (resource.HasValue)
                {
                    languageValues.Add(resource.Value);
                }
            }

            return Result.Success(languageValues);
        }

        private Maybe<LanguageResources> LoadResources(ResourceManager resourceManager, CultureInfo culture)
        {
            ResourceSet rs = resourceManager.GetResourceSet(culture, true, false);
            if (rs != null)
            {
                var resource = rs.Cast<DictionaryEntry>()
                    .ToDictionary(k => k.Key.ToString(), v => v.Value.ToString());

                return new LanguageResources
                {
                    CultureName = culture.Name,
                    Resources = new Dictionary<string, Dictionary<string, string>>
                    {
                        { _resourceManager.BaseName, resource }
                    }
                };
            }

            return Maybe.None;
        }
    }
}