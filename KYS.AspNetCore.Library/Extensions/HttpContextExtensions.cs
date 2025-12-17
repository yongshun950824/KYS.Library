using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace KYS.AspNetCore.Library.Extensions
{
    public static class HttpContextExtensions
    {
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
