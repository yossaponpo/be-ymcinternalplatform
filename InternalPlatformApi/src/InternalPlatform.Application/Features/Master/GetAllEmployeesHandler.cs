using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Masters;

public sealed class GetAllEmployeesHandler(IMasterRepository repo)
{
    public async Task<List<Employee>> HandleAsync(CancellationToken ct)
    {
        var result = await repo.GetAllEmployeesAsync(ct);
        return result;
    }
}
