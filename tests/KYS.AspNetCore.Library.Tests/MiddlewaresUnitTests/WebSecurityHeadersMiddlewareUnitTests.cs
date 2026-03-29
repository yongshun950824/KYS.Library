using KYS.AspNetCore.Library.Middlewares;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace KYS.AspNetCore.Library.Tests.MiddlewaresUnitTests;

public class WebSecurityHeadersMiddlewareUnitTests
{
    [Test]
    public async Task InvokeAsync_ShouldAddSecurityHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        RequestDelegate next = (HttpContext ctx) => Task.CompletedTask;
        var middleware = new WebSecurityHeadersMiddleware(next);

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(context.Response.Headers.TryGetValue("Content-Security-Policy", out var cspValue));
        Assert.AreEqual("default-src 'self'; img-src 'self' https: data:; connect-src *; script-src 'self' 'unsafe-inline' 'unsafe-eval' https:; font-src 'self' https://netdna.bootstrapcdn.com/; style-src 'self' 'unsafe-inline';", cspValue.ToString());

        Assert.True(context.Response.Headers.TryGetValue("X-Content-Type-Options", out var contentTypeOptionsValue));
        Assert.AreEqual("nosniff", contentTypeOptionsValue.ToString());

        Assert.True(context.Response.Headers.TryGetValue("X-Frame-Options", out var frameOptionsValue));
        Assert.AreEqual("SAMEORIGIN", frameOptionsValue.ToString());

        Assert.True(context.Response.Headers.TryGetValue("X-XSS-Protection", out var xssProtectionValue));
        Assert.AreEqual("1; mode=block", xssProtectionValue.ToString());

        Assert.True(context.Response.Headers.TryGetValue("Strict-Transport-Security", out var hstsValue));
        Assert.AreEqual("max-age=31536000; includeSubDomains", hstsValue);
    }
}
