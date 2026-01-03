using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Abstractions.Persistence;

public interface IPayrollRepository
{
    Task<GetPayrollByIdResponse> GetByIdAsync(int payrollId, CancellationToken ct);
    Task<int> CreateAsync(Payroll payroll, CancellationToken ct);
    Task<bool> ExistsAsync(int payrollId, CancellationToken ct);
    Task UpdateAsync(Payroll payroll, CancellationToken ct);
    Task<SearchPayRollResponse> SearchAsync(SearchPayRollInput input, CancellationToken ct);
}