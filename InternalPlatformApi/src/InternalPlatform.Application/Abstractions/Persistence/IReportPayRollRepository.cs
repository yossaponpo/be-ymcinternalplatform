using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Abstractions.Persistence;

public interface IReportPayRollRepository
{
    Task<byte[]> GeneratePayrollReportAsync(int payrollId,CancellationToken ct);
}