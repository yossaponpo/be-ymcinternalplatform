using InternalPlatform.Api.Endpoints;
using InternalPlatform.Application.Features.Customers.GetCustomerById;
using InternalPlatform.Application.Features.Customers.GetCustomers;
using InternalPlatform.Application.Features.Reports.Customers;
using InternalPlatform.Api.Security;
using InternalPlatform.Infrastructure;
using InternalPlatform.Application.Features.Payrolls.PatchPayroll;
using InternalPlatform.Application.Features.Payrolls.CreatePayroll;

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// register services FIRST
var cs = builder.Configuration.GetConnectionString("Default")
         ?? throw new InvalidOperationException("ConnectionStrings:Default is missing");
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(cs);

builder.Services.Configure<ApiKeyOptions>(builder.Configuration.GetSection("Security"));

builder.Services.AddScoped<GetCustomersHandler>();
builder.Services.AddScoped<GetCustomerByIdHandler>();
builder.Services.AddScoped<GenerateCustomerPdfHandler>();
builder.Services.AddScoped<CreatePayrollHandler>();
builder.Services.AddScoped<PatchPayrollHandler>();


builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapOpenApi("/openapi/v1.json");

app.MapCustomersEndpoints();
app.MapReportsEndpoints();
app.MapPayrollsEndpoints();
app.Run();
