using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Masters;

public sealed class GetEmployeeByIdHandler(IMasterRepository repo)
{
    public async Task<Employee> HandleAsync(string employeeNo, CancellationToken ct)
    {
        var result = await repo.GetEmployeeAsync(employeeNo, ct);
        return result;
    }
}
