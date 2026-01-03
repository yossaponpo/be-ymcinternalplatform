using InternalPlatform.Application.Features.Payrolls.CreatePayroll;
using InternalPlatform.Application.Features.Payrolls.GetPayrollById;
using InternalPlatform.Application.Features.Payrolls.PatchPayroll;
using InternalPlatform.Application.Features.Payrolls.SearchPayroll;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace InternalPlatform.Api.Endpoints;

public static class PayrollsEndpoints
{
    public static IEndpointRouteBuilder MapPayrollsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/payrolls/{id:int}", async (
            int id,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<GetPayrollByIdHandler>();

            var data = await handler.HandleAsync(new GetPayrollByIdQuery(id), ct);

            if (data is null)
                return Results.NotFound(new { message = "Data not found" });

            return Results.Ok(data);
        })
        .WithName("GetPayrollById");

        app.MapPost("/payrolls", async (
            Payroll req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<CreatePayrollHandler>();

            var payrollId = await handler.HandleAsync(req, ct);

            return Results.Created($"/payrolls/{payrollId}", new { payrollId });
        })
        .WithName("CreatePayroll");

        app.MapMethods("/payrolls/{id:int}", new[] { "PATCH" }, async (
            int id,
            Payroll req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<PatchPayrollHandler>();

            var ok = await handler.HandleAsync(id, req, ct);

            return ok ? Results.NoContent() : Results.NotFound();
        })
        .WithName("PatchPayroll");

        app.MapPost("/payrolls/search", async (
            SearchPayRollInput req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<SearchPayrollHandler>();

            var result = await handler.HandleAsync(req, ct);

            return Results.Ok(result);
        })
        .WithName("SearchPayroll");


        return app;
    }
}
