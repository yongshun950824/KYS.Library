using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using CSharpFunctionalExtensions;

namespace KYS.Library.Services
{
    /// <summary>
    /// Defines a service contract for language translation with support for multiple resources (files).
    /// </summary>
    public interface IMultiResourcesTranslator
    {
        /// <summary>
        /// Translate to selected language for <b>specified resource</b> and provided <c>culture</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="resourceType">The resource file to be used.</param>
        /// <param name="culture">The culture chosen for the translation. By default, uses the current culture if it is not provided.</param>
        /// <returns>A <see cref="Result{T}"/> containing the translated English text.</returns>
        Result<string> Translate(string input, Type resourceType, CultureInfo culture = null);
    }

    /// <summary>
    /// A service for transaction with supports multiple resources (file).
    /// </summary>
    public class MultiResourcesTranslator : BaseTranslator, IMultiResourcesTranslator
    {
        private readonly Assembly _assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiResourcesTranslator"/> class.
        /// </summary>
        /// <param name="assembly">The assembly which the resource files are located.</param>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        protected MultiResourcesTranslator(Assembly assembly,
            CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null) : base(currentCulture, cultures)
        {
            _assembly = assembly;
        }

        #region Static Creation Methods
        /// <summary>
        /// Creates an instance of <see cref="MultiResourcesTranslator"/> with the specified current culture. The assembly will be set to the executing assembly, and the cultures will be loaded from the assembly's resource files.
        /// </summary>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <returns>A <see cref="Result{T}"/> containing the created translator instance.</returns>
        public static Result<MultiResourcesTranslator> Create(CultureInfo currentCulture)
        {
            var instance = new MultiResourcesTranslator(Assembly.GetExecutingAssembly(),
                currentCulture);

            return InitializeTranslator(instance);
        }

        /// <summary>
        /// Creates an instance of <see cref="MultiResourcesTranslator"/> with the specified current culture and available cultures. The assembly will be set to the executing assembly, and the cultures will be loaded from the assembly's resource files.
        /// </summary>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        /// <returns>A <see cref="Result{T}"/> containing the created translator instance.</returns>
        public static Result<MultiResourcesTranslator> Create(CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null)
        {
            var instance = new MultiResourcesTranslator(Assembly.GetExecutingAssembly(),
                currentCulture,
                cultures);

            return InitializeTranslator(instance);
        }

        /// <summary>
        /// Creates an instance of <see cref="MultiResourcesTranslator"/> with the specified assembly, current culture and available cultures. The cultures will be loaded from the specified assembly's resource files.
        /// </summary>
        /// <param name="assembly">The assembly which the resource files are located.</param>
        /// <param name="currentCulture">The <see cref="CultureInfo"/> instance represents the current culture.</param>
        /// <param name="cultures">The list of available <see cref="CultureInfo"/> instance(s).</param>
        /// <returns>A <see cref="Result{T}"/> containing the created translator instance.</returns>
        public static Result<MultiResourcesTranslator> Create(Assembly assembly = null,
            CultureInfo currentCulture = null,
            List<CultureInfo> cultures = null)
        {
            var instance = new MultiResourcesTranslator(assembly ?? Assembly.GetExecutingAssembly(),
                currentCulture,
                cultures);

            return InitializeTranslator(instance);
        }
        #endregion

        /// <summary>
        /// Translate to selected language for <b>specified resource</b> and provided <c>culture</c>. By default, translate to current/initialized language.
        /// </summary>
        /// <param name="input">A key for the resource.</param>
        /// <param name="resourceType">The resource file to be used.</param>
        /// <param name="culture">The culture chosen for the translation. By default, uses the current culture if it is not provided.</param>
        /// <returns>A <see cref="Result{T}"/> containing the translated text. Otherwise, returns the original value if <c>isReturnedOriginalValue</c> is true.</returns>
        public Result<string> Translate(string input, Type resourceType, CultureInfo culture = null)
        {
            culture ??= CurrentCulture;

            Dictionary<string, string> resourceSet = null;

            if (ResourceLookups.TryGetValue(culture.Name, out var cultureLookup) &&
                cultureLookup.TryGetValue(resourceType.FullName, out resourceSet) &&
                resourceSet.TryGetValue(input, out string translatedText))
            {
                return Result.Success(translatedText);
            }

            if (resourceSet == null)
            {
                return Result.Failure<string>($"{resourceType.FullName} resource does not exist.");
            }

            return Result.Failure<string>($"Provided {input} doesn't support for {culture.Name} language translation in {resourceType.FullName}.");
        }

        /// <summary>
        /// Loads the languages and their resources from the assembly. The method iterates through the available cultures and resource files, attempting to load the resources for each culture. If a resource file is not found for a culture, it safely ignores the exception and continues loading other resources. If any other exception occurs during loading, it returns a failure result with the error message.
        /// </summary>
        /// <returns>A <see cref="Result{T}"/> containing the loaded language resources.</returns>
        protected override Result<List<LanguageResources>> LoadLanguages()
        {
            List<LanguageResources> languageValues = [];

            // Get all resources with remove postfix: ".resource"
            string[] resNames = _assembly.GetManifestResourceNames()
                .Select(x => Path.GetFileNameWithoutExtension(x))
                .ToArray();

            foreach (CultureInfo culture in Cultures)
            {
                Dictionary<string, Dictionary<string, string>> resources = [];

                foreach (var name in resNames)
                {
                    var resourceManager = new ResourceManager(name, _assembly);

                    var result = Result.Try(
                        () => LoadResource(resourceManager, culture),
                        ex => ex
                    );

                    if (result.IsFailure && result.Error is CultureNotFoundException)
                    {
                        // Safe to ignore CultureNotFoundException
                        result = Result.Success<Maybe<Dictionary<string, string>>, Exception>(Maybe.None);
                    }
                    else if (result.IsFailure)
                    {
                        return Result.Failure<List<LanguageResources>>(result.Error.Message);
                    }

                    var resource = result.Value;
                    if (resource.HasValue)
                    {
                        resources.Add(resourceManager.BaseName, resource.Value);
                    }
                }

                languageValues.Add(new LanguageResources
                {
                    CultureName = culture.Name,
                    Resources = resources
                });
            }

            return Result.Success(languageValues);
        }

        private static Maybe<Dictionary<string, string>> LoadResource(ResourceManager resourceManager, CultureInfo culture)
        {
            ResourceSet rs = resourceManager.GetResourceSet(culture, true, false);
            if (rs != null)
            {
                return new Dictionary<string, string>(rs.Cast<DictionaryEntry>()
                    .ToDictionary(k => k.Key.ToString(), v => v.Value.ToString()));
            }

            return Maybe.None;
        }
    }
}
