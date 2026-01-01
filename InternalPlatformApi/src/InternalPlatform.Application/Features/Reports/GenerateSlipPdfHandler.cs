using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Application.Abstractions.Reports;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Reports.Payrolls;

public sealed class GenerateSlipPdfHandler(
    IReportSlipRepository reports)
{
    public async Task<byte[]> HandleAsync(int payrollDetailId,CancellationToken ct)
    {
        var data = await reports.GenerateSlipReportAsync(payrollDetailId, ct);
        return data;
    }
}
