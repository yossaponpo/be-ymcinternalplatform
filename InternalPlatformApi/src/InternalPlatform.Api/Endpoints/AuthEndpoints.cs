using InternalPlatform.Application.Features.Auth;
using InternalPlatform.Application.Features.Payrolls.CreatePayroll;
using InternalPlatform.Application.Features.Payrolls.GetPayrollById;
using InternalPlatform.Application.Features.Payrolls.PatchPayroll;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (
            LoginInput req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<LoginHandler>();
            var response = await handler.HandleAsync(req, ct);

            return Results.Ok(response);
        })
        .WithName("Login");

        app.MapPost("/auth/encrypt", async (
            LoginInput req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<EncryptHandler>();
            var response = await handler.HandleAsync(req.Password, ct);

            return Results.Ok(response);
        })
        .WithName("Encrypt");
        return app;
    }
}
