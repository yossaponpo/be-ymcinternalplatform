using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Payrolls.PatchPayroll;

public sealed class PatchPayrollHandler(IPayrollRepository repo)
{
    public async Task<bool> HandleAsync(int payrollId, Payroll req, CancellationToken ct)
    {
        if (!await repo.ExistsAsync(payrollId, ct))
            return false;

        await repo.UpdateAsync(req, ct);

        return true;
    }
}
