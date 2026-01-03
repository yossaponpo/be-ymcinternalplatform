using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Masters;

public sealed class AddEditEmployeeHandler(IMasterRepository repo)
{
    public async Task<CommonResponse> HandleAsync(Employee input,CancellationToken ct)
    {
        var result = await repo.AddOrEditEmployee(input,ct);
        return result;
    }
}
