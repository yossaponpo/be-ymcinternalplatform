using System.Security.Cryptography;
using Microsoft.Extensions.Options;

namespace InternalPlatform.Api.Security;

public sealed class ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> options)
{
    private readonly ApiKeyOptions _opt = options.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        // AllowAnonymous support via endpoint metadata
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null)
        {
            await next(context);
            return;
        }

        if (context.Request.Path.StartsWithSegments("/openapi"))
        {
            await next(context);
            return;
        }

        // Validate config
        if (string.IsNullOrWhiteSpace(_opt.ApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsync("API key is not configured.");
            return;
        }

        var headerName = string.IsNullOrWhiteSpace(_opt.HeaderName) ? "X-API-KEY" : _opt.HeaderName;

        if (!context.Request.Headers.TryGetValue(headerName, out var providedKey) ||
            string.IsNullOrWhiteSpace(providedKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Missing API key.");
            return;
        }

        // Constant-time compare to reduce timing attacks
        if (!FixedTimeEquals(providedKey!, _opt.ApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Invalid API key.");
            return;
        }

        await next(context);
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        var aBytes = System.Text.Encoding.UTF8.GetBytes(a);
        var bBytes = System.Text.Encoding.UTF8.GetBytes(b);

        return aBytes.Length == bBytes.Length &&
               CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
    }
}
