using InternalPlatform.Api.Customers;
using InternalPlatform.Application.Features.Customers.GetCustomerById;
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

            return Results.Ok(customers);
        })
        .WithName("GetCustomers");

        app.MapGet("/customers/{id:int}", async (
            int id,
            GetCustomerByIdHandler handler,
            CancellationToken ct) =>
        {
            var customer = await handler.HandleAsync(
                new GetCustomerByIdQuery(id),
                ct
            );

            if (customer is null)
                return Results.NotFound(new { message = "Customer not found" });

            return Results.Ok(customer);
        })
        .WithName("GetCustomerById");

        return app;
    }
}
