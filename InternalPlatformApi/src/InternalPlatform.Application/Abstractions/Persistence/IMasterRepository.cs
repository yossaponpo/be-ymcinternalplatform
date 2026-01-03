using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Abstractions.Persistence;

public interface IMasterRepository
{
    Task<List<Employee>> GetAllEmployeesAsync(CancellationToken ct);
    Task<CommonResponse> AddOrEditEmployee(Employee input, CancellationToken ct);
    Task<PaginationResult<Employee>> SearchEmployeesAsync(SearchEmployeeInput input, CancellationToken ct);
    Task<Employee> GetEmployeeAsync(string employeeNo, CancellationToken ct);
}