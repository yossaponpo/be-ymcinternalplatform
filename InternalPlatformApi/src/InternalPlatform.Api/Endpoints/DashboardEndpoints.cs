using InternalPlatform.Application.Features.Auth;
using InternalPlatform.Application.Features.Dashboards;
using InternalPlatform.Application.Features.Payrolls.CreatePayroll;
using InternalPlatform.Application.Features.Payrolls.GetPayrollById;
using InternalPlatform.Application.Features.Payrolls.PatchPayroll;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Api.Endpoints;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/dashboard/stats", async (
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<StatCardHandler>();
            var response = await handler.HandleAsync(ct);

            return Results.Ok(response);
        })
        .WithName("GetDashboardStats");
        
        return app;
    }
}
