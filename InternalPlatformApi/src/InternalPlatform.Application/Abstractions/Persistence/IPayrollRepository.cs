using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Abstractions.Persistence;

public interface IPayrollRepository
{
    Task<Payroll> GetByIdAsync(int payrollId, CancellationToken ct);
    Task<int> CreateAsync(Payroll payroll, CancellationToken ct);
    Task<bool> ExistsAsync(int payrollId, CancellationToken ct);
    Task UpdateAsync(Payroll payroll, CancellationToken ct);
}