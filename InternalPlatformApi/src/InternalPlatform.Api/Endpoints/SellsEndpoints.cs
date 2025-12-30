using InternalPlatform.Application.Features.Sells.CreateInvoice;
using InternalPlatform.Application.Features.Sells.CreateReceipt;
using InternalPlatform.Application.Features.Sells.GetInvoiceById;
using InternalPlatform.Application.Features.Sells.GetReceiptById;
using InternalPlatform.Application.Features.Sells.PatchInvoice;
using InternalPlatform.Application.Features.Sells.PatchReceipt;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Api.Endpoints;

public static class SellsEndpoints
{
    public static IEndpointRouteBuilder MapSellsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/invoices/{id:int}", async (
            int id,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<GetInvoiceByIdHandler>();

            var data = await handler.HandleAsync(id, ct);

            if (data is null)
                return Results.NotFound(new { message = "Data not found" });

            return Results.Ok(data);
        })
        .WithName("GetInvoiceById");

        app.MapPost("/invoices", async (
            Invoice req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<CreateInvoiceHandler>();

            var invoiceId = await handler.HandleAsync(req, ct);

            return Results.Created($"/invoices/{invoiceId}", new { invoiceId });
        })
        .WithName("CreateInvoice");

        app.MapMethods("/invoices/{id:int}", new[] { "PATCH" }, async (
            int id,
            Invoice req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<PatchInvoiceHandler>();

            var ok = await handler.HandleAsync(req, ct);

            return ok ? Results.NoContent() : Results.NotFound();
        })
        .WithName("PatchInvoice");

        app.MapGet("/receipts/{id:int}", async (
            int id,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<GetReceiptByIdHandler>();

            var data = await handler.HandleAsync(id, ct);

            if (data is null)
                return Results.NotFound(new { message = "Data not found" });

            return Results.Ok(data);
        })
        .WithName("GetReceiptById");

        app.MapPost("/receipts", async (
            Receipt req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<CreateReceiptHandler>();

            var receiptId = await handler.HandleAsync(req, ct);

            return Results.Created($"/receipts/{receiptId}", new { receiptId });
        })
        .WithName("CreateReceipt");

        app.MapMethods("/receipts/{id:int}", new[] { "PATCH" }, async (
            int id,
            Receipt req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<PatchReceiptHandler>();

            var ok = await handler.HandleAsync(req, ct);

            return ok ? Results.NoContent() : Results.NotFound();
        })
        .WithName("PatchReceipt");

        return app;
    }
}
