using System.Net;

namespace KYS.AspNetCore.Library.Models
{
    public class ApiReponseModel<T>
    {
        public HttpStatusCode StatusCode { get; private set; }
        public string ResponseMessage { get; private set; }
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
