using KYS.AspNetCore.Library.Extensions;
using KYS.AspNetCore.Library.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace KYS.AspNetCore.Library.Helpers
{
    public static class ViewResultHelper
    {
        /// <summary>
        /// Read <c>HttpResponseMessage</c> and render it to a <c>ViewResult</c>.
        /// <br /><br />
        /// Idea from <a href="https://stackoverflow.com/questions/79674066/how-to-convert-httpresponsemessage-to-viewresult/79674130#79674130">How to convert HttpResponseMessage to ViewResult?</a>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpContext"></param>
        /// <param name="response"></param>
        /// <param name="viewName"></param>
        /// <param name="errorViewName"></param>
        /// <returns></returns>
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
