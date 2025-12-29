using InternalPlatform.Application.Features.Reports.Customers;

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

        return app;
    }
}
