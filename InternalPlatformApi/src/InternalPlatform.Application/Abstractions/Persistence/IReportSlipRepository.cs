using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Abstractions.Persistence;

public interface IReportSlipRepository
{
    Task<byte[]> GenerateSlipReportAsync(int payrollDetailId,CancellationToken ct);
}