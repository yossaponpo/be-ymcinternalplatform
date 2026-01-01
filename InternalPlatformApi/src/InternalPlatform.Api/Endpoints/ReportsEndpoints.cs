using InternalPlatform.Application.Features.Reports.Customers;
using InternalPlatform.Application.Features.Reports.Payrolls;

namespace InternalPlatform.Api.Endpoints;

public static class ReportsEndpoints
{
    public static IEndpointRouteBuilder MapReportsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/reports/customers/pdf", async (
            GenerateCustomerPdfHandler handler,
            CancellationToken ct) =>
        {
            var pdf = await handler.HandleAsync(ct);

            return Results.File(
                pdf,
                "application/pdf",
                "customers-report.pdf");
        });

        app.MapGet("/reports/payrolls/{id:int}", async (
            int id,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<GeneratePayrollPdfHandler>();

            var data = await handler.HandleAsync(id, ct);

            if (data is null)
                return Results.NotFound(new { message = "Data not found" });

            return Results.File(
                data,
                "application/pdf",
                "customers-report.pdf");
        })
        .WithName("GetPdfPayrollById");

        app.MapGet("/reports/slips/{id:int}", async (
            int id,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<GenerateSlipPdfHandler>();

            var data = await handler.HandleAsync(id, ct);

            if (data is null)
                return Results.NotFound(new { message = "Data not found" });

            return Results.File(
                data,
                "application/pdf",
                "slips-report.pdf");
        })
        .WithName("GetPdfSlipById");

        return app;
    }
}
