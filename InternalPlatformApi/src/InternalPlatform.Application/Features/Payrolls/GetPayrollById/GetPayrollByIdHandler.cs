using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Payrolls.GetPayrollById;

public sealed class GetPayrollByIdHandler(IPayrollRepository repo)
{
    public Task<Payroll?> HandleAsync(GetPayrollByIdQuery query, CancellationToken ct)
        => repo.GetByIdAsync(query.PayrollId, ct);
}
