using InternalPlatform.Api.Endpoints;
using InternalPlatform.Application.Features.Customers.GetCustomerById;
using InternalPlatform.Application.Features.Customers.GetCustomers;
using InternalPlatform.Infrastructure;
using InternalPlatform.Api.Security;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// ✅ register services FIRST
var cs = builder.Configuration.GetConnectionString("Default")
         ?? throw new InvalidOperationException("ConnectionStrings:Default is missing");

builder.Services.AddInfrastructure(cs);
builder.Services.Configure<ApiKeyOptions>(builder.Configuration.GetSection("Security"));
// any other DI registrations
builder.Services.AddScoped<GetCustomersHandler>();
builder.Services.AddScoped<GetCustomerByIdHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. Use header: X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = ParameterLocation.Header
    });
});

// ✅ THEN build
var app = builder.Build();
app.UseMiddleware<ApiKeyMiddleware>();
// middleware and endpoints AFTER build
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapCustomersEndpoints();

app.Run();
