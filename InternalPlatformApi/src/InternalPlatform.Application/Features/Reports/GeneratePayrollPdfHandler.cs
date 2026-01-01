using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Application.Abstractions.Reports;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Reports.Payrolls;

public sealed class GeneratePayrollPdfHandler(
    IReportPayRollRepository reports)
{
    public async Task<byte[]> HandleAsync(int payrollId,CancellationToken ct)
    {
        var data = await reports.GeneratePayrollReportAsync(payrollId, ct);
        return data;
    }
}
