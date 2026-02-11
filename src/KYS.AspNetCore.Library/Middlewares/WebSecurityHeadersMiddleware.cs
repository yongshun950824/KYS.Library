using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace KYS.AspNetCore.Library.Middlewares
{
    /// <summary>
    /// Middleware that injects security-hardening HTTP headers into all outgoing responses.
    /// </summary>
    /// <remarks>
    /// This middleware targets vulnerabilities such as Cross-Site Scripting (XSS), 
    /// Clickjacking, and Protocol Downgrade attacks. It should be registered early in the request pipeline.
    /// </remarks>
    public class WebSecurityHeadersMiddleware
    {
        /// <summary>
        /// Static collection of headers to be applied to the HttpContext.Response.
        /// </summary>
        private readonly RequestDelegate _next;

        private static readonly Dictionary<string, string> _securityHeaders = new()
        {
            { "Content-Security-Policy", "default-src 'self'; img-src 'self' https: data:; connect-src *; script-src 'self' 'unsafe-inline' 'unsafe-eval' https:; font-src 'self' https://netdna.bootstrapcdn.com/; style-src 'self' 'unsafe-inline';" },
            { "X-Content-Type-Options", "nosniff" },
            { "X-Frame-Options", "SAMEORIGIN" },
            { "X-XSS-Protection", "1; mode=block" },
            { "Strict-Transport-Security", "max-age=31536000; includeSubDomains" }
        };

        public WebSecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Processes the request and applies the security headers to the response.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
