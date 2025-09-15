using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace KYS.AspNetCore.Library.Middlewares
{
    public class APISecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        private static readonly Dictionary<string, string> _securityHeaders = new()
        {
            { "Content-Security-Policy", "default-src 'self'" },
            { "X-Content-Type-Options", "nosniff" },
            { "X-Frame-Options", "SAMEORIGIN" },
            { "X-XSS-Protection", "1; mode=block" },
            { "Strict-Transport-Security", "max-age=31536000; includeSubDomains" }
        };

        public APISecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            foreach (var header in _securityHeaders)
            {
                context.Response.Headers.Append(header.Key, new StringValues(header.Value));
            }

            await _next(context);
        }
    }
}
