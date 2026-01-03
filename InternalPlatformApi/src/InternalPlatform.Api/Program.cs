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
using InternalPlatform.Application.Features.Auth;
using InternalPlatform.Application.Features.Payrolls.SearchPayroll;
using InternalPlatform.Application.Features.Payrolls.GetPayrollById;
using InternalPlatform.Application.Features.Masters;
using InternalPlatform.Application.Features.Dashboards;

var builder = WebApplication.CreateBuilder(args);

Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

// register services FIRST
var cs = Environment.GetEnvironmentVariable("ConnectionStrings")
         ?? throw new InvalidOperationException("ConnectionStrings is missing");
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(cs);


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
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<EncryptHandler>();
builder.Services.AddScoped<SearchPayrollHandler>();
builder.Services.AddScoped<GetPayrollByIdHandler>();
builder.Services.AddScoped<GetAllEmployeesHandler>();
builder.Services.AddScoped<AddEditEmployeeHandler>();
builder.Services.AddScoped<SearchEmployeeHandler>();
builder.Services.AddScoped<StatCardHandler>();
builder.Services.AddScoped<GetEmployeeByIdHandler>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("Frontend");
app.UseMiddleware<ApiKeyMiddleware>();
app.MapOpenApi("/openapi/v1.json");
app.MapCustomersEndpoints();
app.MapReportsEndpoints();
app.MapPayrollsEndpoints();
app.MapSellsEndpoints();
app.MapMasterEndpoints();
app.MapAuthEndpoints();
app.MapDashboardEndpoints();
app.Run();
