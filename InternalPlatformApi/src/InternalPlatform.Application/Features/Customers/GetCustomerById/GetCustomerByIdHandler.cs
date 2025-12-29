using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Customers.GetCustomerById;

public sealed class GetCustomerByIdHandler(ICustomerRepository repo)
{
    public Task<Customer?> HandleAsync(GetCustomerByIdQuery query, CancellationToken ct)
        => repo.GetByIdAsync(query.CustomerId, ct);
}
