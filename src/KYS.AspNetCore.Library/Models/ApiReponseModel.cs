using System.Net;

namespace KYS.AspNetCore.Library.Models
{
    /// <summary>
    /// Represents the blueprint for the base response with status code, response message and data.
    /// </summary>
    /// <typeparam name="T">The type of response data.</typeparam>
    public class ApiReponseModel<T>
    {
        /// <summary>
        /// Gets the response status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }
        /// <summary>
        /// Gets the response message.
        /// </summary>
        public string ResponseMessage { get; private set; }
        /// <summary>
        /// Gets the response body data.
        /// </summary>
        public T Data { get; private set; }

        public ApiReponseModel(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public ApiReponseModel(HttpStatusCode statusCode, T data)
        {
            StatusCode = statusCode;
            Data = data;
        }

        public ApiReponseModel(HttpStatusCode statusCode, string responseMessage)
        {
            StatusCode = statusCode;
            ResponseMessage = responseMessage;
        }

        public ApiReponseModel(HttpStatusCode statusCode, string responseMessage, T data)
        {
            StatusCode = statusCode;
            ResponseMessage = responseMessage;
            Data = data;
        }
    }
}
