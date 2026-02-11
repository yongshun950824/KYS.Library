using KYS.AspNetCore.Library.Extensions;
using KYS.AspNetCore.Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KYS.AspNetCore.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for <see cref="ViewResult" />.
    /// </summary>
    public static class ViewResultHelper
    {
        /// <summary>
        /// Read <c>HttpResponseMessage</c> and render it to a <c>ViewResult</c>.
        /// <br /><br />
        /// Idea from <a href="https://stackoverflow.com/questions/79674066/how-to-convert-httpresponsemessage-to-viewresult/79674130#79674130">How to convert HttpResponseMessage to ViewResult?</a>
        /// </summary>
        /// <typeparam name="T">The expected model type to be deserialized from the response body.</typeparam>
        /// <param name="httpContext">The current HTTP context, used to initialize the <see cref="ViewDataDictionary"/>.</param>
        /// <param name="response">The raw response received from an API call.</param>
        /// <param name="viewName">The name or path of the view to render on a successful API response.</param>
        /// <param name="errorViewName">The name or path of the view to render if the API returns a non-success status code. Defaults to the Shared Error view.</param>
        /// <returns>
        /// A <see cref="ViewResult"/> containing an <see cref="ApiReponseModel{T}"/> as its model.
        /// </returns>
        /// <remarks>
        /// This method automatically handles:
        /// <list type="bullet">
        /// <item>Checking <see cref="HttpResponseMessage.IsSuccessStatusCode"/>.</item>
        /// <item>Asynchronous reading of the response content.</item>
        /// <item>Deserialization of JSON content into type <typeparamref name="T"/>.</item>
        /// <item>Mapping API status codes and reason phrases to the view model.</item>
        /// </list>
        /// </remarks>
        public static async Task<ViewResult> ToViewResultAsync<T>(
            HttpContext httpContext,
            HttpResponseMessage response,
            string viewName,
            string errorViewName = "/Views/Shared/Error.cshtml")
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorRespContentString = await response.Content.ReadAsStringAsync();

                return new ViewResult
                {
                    ViewName = errorViewName,
                    ViewData = httpContext.CreateViewDataDictionary
                    (
                        new ApiReponseModel<T>
                        (
                            response.StatusCode,
                            response.ReasonPhrase,
                            JsonConvert.DeserializeObject<T>(errorRespContentString)
                        )
                    )
                };
            }

            var respContentString = await response.Content.ReadAsStringAsync();

            return new ViewResult
            {
                ViewName = viewName,
                ViewData = httpContext.CreateViewDataDictionary
                (
                    new ApiReponseModel<T>
                    (
                        response.StatusCode,
                        JsonConvert.DeserializeObject<T>(respContentString)
                    )
                )
            };
        }
    }
}
