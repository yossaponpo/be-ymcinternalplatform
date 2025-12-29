using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Application.Abstractions.Reports;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Reports.Customers;

public sealed class GenerateCustomerPdfHandler(
    ICustomerRepository customers,
    IPdfReportGenerator<IEnumerable<Customer>> pdf)
{
    public async Task<byte[]> HandleAsync(CancellationToken ct)
    {
        var data = await customers.GetAllAsync(ct);
        return pdf.Generate(data);
    }
}
