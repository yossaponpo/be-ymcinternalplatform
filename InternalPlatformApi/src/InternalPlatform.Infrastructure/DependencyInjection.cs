using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Infrastructure.Persistence;
using InternalPlatform.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using InternalPlatform.Application.Abstractions.Reports;
using InternalPlatform.Infrastructure.Reports;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(
            _ => new NpgsqlConnectionFactory(connectionString));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IPdfReportGenerator<IEnumerable<Customer>>, CustomerReportGenerator>();
        services.AddScoped<IPayrollRepository, PayrollRepository>();
        services.AddScoped<ISellRepository, SellRepository>();
        services.AddScoped<IReportPayRollRepository, ReportPayRollRepository>();
        services.AddScoped<IReportSlipRepository, ReportSlipRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IMasterRepository, MasterRepository>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        return services;
    }
}
