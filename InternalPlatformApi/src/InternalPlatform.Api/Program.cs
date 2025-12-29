using System.Data;
using InternalPlatform.Infrastructure;
using InternalPlatform.Api.Endpoints;
using InternalPlatform.Application.Features.Customers.GetCustomers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.OpenApi;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Missing ConnectionStrings:Default in appsettings.json");

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "InternalPlatform API", Version = "v1" });
});

builder.Services.AddScoped<IDbConnection>(_ =>
{
    var conn = new NpgsqlConnection(connectionString);
    return conn;
});

var app = builder.Build();

// Global exception handling (safe, API-friendly)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";

        // Avoid leaking internal details in production
        var includeDetails = app.Environment.IsDevelopment();

        var problem = Results.Problem(
            title: "An unexpected error occurred.",
            detail: includeDetails ? exception?.ToString() : null,
            statusCode: StatusCodes.Status500InternalServerError,
            instance: context.Request.Path
        );

        await problem.ExecuteAsync(context);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var cs = builder.Configuration.GetConnectionString("Default")
         ?? throw new InvalidOperationException("ConnectionStrings:Default is missing");

builder.Services.AddInfrastructure(cs);
builder.Services.AddScoped<GetCustomersHandler>();

app.MapCustomersEndpoints();

app.Run();