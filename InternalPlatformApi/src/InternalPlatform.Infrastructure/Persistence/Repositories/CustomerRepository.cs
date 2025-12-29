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
        const string sql = """
            select
                customer_id   as "CustomerId",
                customer_name as "CustomerName",
                address       as "Address",
                branch        as "Branch",
                tax_no        as "TaxNo"
            from customer
            order by customer_name
            """;

        using var conn = db.Create();
        var cmd = new CommandDefinition(sql, cancellationToken: ct);

        var result = await conn.QueryAsync<Customer>(cmd);
        return result.AsList();
    }
}
