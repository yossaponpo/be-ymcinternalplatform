using InternalPlatform.Api.Customers;
using InternalPlatform.Application.Features.Customers.GetCustomers;

namespace InternalPlatform.Api.Endpoints;

public static class CustomersEndpoints
{
    public static IEndpointRouteBuilder MapCustomersEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /customers
        app.MapGet("/customers", async (
            GetCustomersHandler handler,
            CancellationToken ct) =>
        {
            var customers = await handler.HandleAsync(ct);

            var response = customers.Select(c =>
                new CustomerListResponse(
                    c.CustomerId,
                    c.CustomerName,
                    c.Branch,
                    c.TaxNo
                ));

            return Results.Ok(response);
        })
        .WithName("GetCustomers");

        return app;
    }
}
