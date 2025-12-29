using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Abstractions.Persistence;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken ct);
}