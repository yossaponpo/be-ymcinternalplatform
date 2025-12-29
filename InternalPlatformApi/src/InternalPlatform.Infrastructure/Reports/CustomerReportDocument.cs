using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Infrastructure.Reports;

public sealed class CustomerReportDocument(IEnumerable<Customer> customers)
    : IDocument
{
    public DocumentMetadata GetMetadata() => new()
    {
        Title = "Customer Report",
        Author = "InternalPlatform"
    };

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(20);
            page.DefaultTextStyle(TextStyle.Default.FontSize(10));

            page.Header().Text("Customer Report")
                .FontSize(18)
                .SemiBold()
                .AlignCenter();

            page.Content().PaddingTop(10).Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(50);   // ID
                    cols.RelativeColumn(2);   // Name
                    cols.RelativeColumn();    // Branch
                    cols.RelativeColumn();    // Tax
                });

                table.Header(h =>
                {
                    h.Cell().Element(HeaderCell).Text("ID");
                    h.Cell().Element(HeaderCell).Text("Name");
                    h.Cell().Element(HeaderCell).Text("Branch");
                    h.Cell().Element(HeaderCell).Text("Tax No");
                });

                foreach (var c in customers)
                {
                    table.Cell().Element(DataCell).Text(c.CustomerId.ToString());
                    table.Cell().Element(DataCell).Text(c.CustomerName);
                    table.Cell().Element(DataCell).Text(c.Branch);
                    table.Cell().Element(DataCell).Text(c.TaxNo);
                }
            });

            page.Footer()
                .AlignCenter()
                .Text(x =>
                {
                    x.Span("Generated on ");
                    x.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm")).SemiBold();
                });
        });
    }

    static IContainer HeaderCell(IContainer c) =>
        c.Padding(5).Background(Colors.Grey.Lighten2).BorderBottom(1).BorderColor(Colors.Grey.Medium)
         .DefaultTextStyle(x => x.SemiBold());

    static IContainer DataCell(IContainer c) =>
        c.Padding(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
}
