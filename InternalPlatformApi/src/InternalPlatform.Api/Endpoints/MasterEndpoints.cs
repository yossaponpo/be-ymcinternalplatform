using InternalPlatform.Application.Features.Auth;
using InternalPlatform.Application.Features.Masters;
using InternalPlatform.Application.Features.Payrolls.CreatePayroll;
using InternalPlatform.Application.Features.Payrolls.GetPayrollById;
using InternalPlatform.Application.Features.Payrolls.PatchPayroll;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Api.Endpoints;

public static class MasterEndpoints
{
    public static IEndpointRouteBuilder MapMasterEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/master/employees", async (
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<GetAllEmployeesHandler>();
            var response = await handler.HandleAsync(ct);

            return Results.Ok(response);
        })
        .WithName("GetAllEmployees");

        app.MapPost("/master/employee", async (
            Employee req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<AddEditEmployeeHandler>();
            var response = await handler.HandleAsync(req, ct);

            return Results.Ok(response);
        })
        .WithName("AddEditEmployee");

        app.MapPost("/master/employees/search", async (
            SearchEmployeeInput req,
            HttpContext http,
            CancellationToken ct) =>
        {
            var handler = http.RequestServices.GetRequiredService<SearchEmployeeHandler>();
            var response = await handler.HandleAsync(req, ct);

            return Results.Ok(response);
        })
        .WithName("SearchEmployee");

        
        return app;
    }


}
