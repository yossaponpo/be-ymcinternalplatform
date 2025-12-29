using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Payrolls.CreatePayroll;

public sealed class CreatePayrollHandler(IPayrollRepository repo)
{
    public async Task<int> HandleAsync(Payroll req, CancellationToken ct)
    {
        var payrollId = await repo.CreateAsync(req, ct);
        return payrollId;
    }
}
