using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Customers.GetCustomers;

public sealed class GetCustomersHandler(ICustomerRepository repo)
{
    public Task<IReadOnlyList<Customer>> HandleAsync(CancellationToken ct)
        => repo.GetAllAsync(ct);
}