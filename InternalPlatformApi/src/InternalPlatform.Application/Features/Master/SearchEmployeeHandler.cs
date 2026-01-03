using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Masters;

public sealed class SearchEmployeeHandler(IMasterRepository repo)
{
    public async Task<PaginationResult<Employee>> HandleAsync(SearchEmployeeInput input, CancellationToken ct)
    {
        var result = await repo.SearchEmployeesAsync(input, ct);
        return result;
    }
}
