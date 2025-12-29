using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Infrastructure.Persistence;
using InternalPlatform.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}
