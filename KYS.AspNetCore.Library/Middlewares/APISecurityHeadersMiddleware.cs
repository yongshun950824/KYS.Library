using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace KYS.AspNetCore.Library.Middlewares
{
    public class APISecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public APISecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Append("Content-Security-Policy", new StringValues("default-src 'self'"));
            context.Response.Headers.Append("X-Content-Type-Options", new StringValues("nosniff"));
            context.Response.Headers.Append("X-Frame-Options", new StringValues("SAMEORIGIN"));
            context.Response.Headers.Append("X-XSS-Protection", new StringValues("1; mode=block"));
            context.Response.Headers.Append("Strict-Transport-Security", new StringValues("max-age=31536000; includeSubDomains"));

            await _next(context);
        }
    }
}
