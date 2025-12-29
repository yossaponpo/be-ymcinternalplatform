using Dapper;
using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;
using InternalPlatform.Infrastructure.Persistence;

namespace InternalPlatform.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(IDbConnectionFactory db)
    : ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"select *
            from customer
            order by customer_id", cancellationToken: ct);

        var result = await conn.QueryAsync<Customer>(cmd);
        return result.AsList();
    }

    public async Task<Customer?> GetByIdAsync(int customerId, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"select *
            from customer
            where customer_id = @CustomerId", new { CustomerId = customerId }, cancellationToken: ct);

        var result = await conn.QueryFirstOrDefaultAsync<Customer>(cmd);
        return result;
    }
}
