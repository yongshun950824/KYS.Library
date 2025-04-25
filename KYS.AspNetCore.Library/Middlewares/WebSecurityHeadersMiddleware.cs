using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace KYS.AspNetCore.Library.Middlewares
{
    public class WebSecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Headers.Add("Content-Security-Policy", new StringValues("default-src 'self'; img-src 'self' https: data:; connect-src *; script-src 'self' 'unsafe-inline' 'unsafe-eval' https:; font-src 'self' https://netdna.bootstrapcdn.com/; style-src 'self' 'unsafe-inline';"));
            context.Response.Headers.Add("X-Content-Type-Options", new StringValues("nosniff"));
            context.Response.Headers.Add("X-Frame-Options", new StringValues("SAMEORIGIN"));
            context.Response.Headers.Add("X-XSS-Protection", new StringValues("1; mode=block"));
            context.Response.Headers.Add("Strict-Transport-Security", new StringValues("max-age=31536000; includeSubDomains"));

            await _next(context);
        }
    }
}
