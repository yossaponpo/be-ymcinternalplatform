using InternalPlatform.Api.Endpoints;
using InternalPlatform.Application.Features.Customers.GetCustomerById;
using InternalPlatform.Application.Features.Customers.GetCustomers;
using InternalPlatform.Application.Features.Reports.Customers;
using InternalPlatform.Api.Security;
using InternalPlatform.Infrastructure;
using InternalPlatform.Application.Features.Payrolls.PatchPayroll;
using InternalPlatform.Application.Features.Payrolls.CreatePayroll;
using InternalPlatform.Application.Features.Sells.GetInvoiceById;
using InternalPlatform.Application.Features.Sells.GetReceiptById;
using InternalPlatform.Application.Features.Sells.CreateInvoice;
using InternalPlatform.Application.Features.Sells.CreateReceipt;
using InternalPlatform.Application.Features.Sells.PatchInvoice;
using InternalPlatform.Application.Features.Sells.PatchReceipt;
using InternalPlatform.Application.Features.Reports.Payrolls;

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
builder.Services.AddScoped<GetInvoiceByIdHandler>();
builder.Services.AddScoped<GetReceiptByIdHandler>();
builder.Services.AddScoped<CreateInvoiceHandler>();
builder.Services.AddScoped<CreateReceiptHandler>();
builder.Services.AddScoped<PatchInvoiceHandler>();
builder.Services.AddScoped<PatchReceiptHandler>();
builder.Services.AddScoped<GeneratePayrollPdfHandler>();
builder.Services.AddScoped<GenerateSlipPdfHandler>();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapOpenApi("/openapi/v1.json");

app.MapCustomersEndpoints();
app.MapReportsEndpoints();
app.MapPayrollsEndpoints();
app.MapSellsEndpoints();
app.Run();
