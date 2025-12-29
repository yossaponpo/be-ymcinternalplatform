using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using InternalPlatform.Application.Abstractions.Reports;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Infrastructure.Reports;

public sealed class CustomerReportGenerator
    : IPdfReportGenerator<IEnumerable<Customer>>
{
    public byte[] Generate(IEnumerable<Customer> customers)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            new CustomerReportDocument(customers).Compose(container);
        }).GeneratePdf();
    }
}