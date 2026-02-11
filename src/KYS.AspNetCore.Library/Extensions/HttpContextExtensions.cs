using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace KYS.AspNetCore.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="HttpContext" />.
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Create a <see cref="ViewDataDictionary"/> instance containing a <see cref="{T}"/> as its model. 
        /// </summary>
        /// <typeparam name="T">The type of <c>model</c>.</typeparam>
        /// <param name="httpContext">The <see cref="HttpContext" /> instance this method extends.</param>
        /// <param name="model">The value to be included.</param>
        /// <returns>A <see cref="ViewDataDictionary"/> containing a <see cref="{T}"/> as its model.</returns>
        public static ViewDataDictionary<T> CreateViewDataDictionary<T>(this HttpContext httpContext, T model)
        {
            var modelMetadataProvider = httpContext.RequestServices.GetService<IModelMetadataProvider>();

            return new ViewDataDictionary<T>(modelMetadataProvider, new ModelStateDictionary())
            {
                Model = model
            };
        }
    }
}
