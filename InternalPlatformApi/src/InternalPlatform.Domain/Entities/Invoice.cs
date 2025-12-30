namespace InternalPlatform.Domain.Entities;

public class Invoice
{
    public InvoiceInfo InvoiceInfo { get; init; } = new InvoiceInfo();
    public List<InvoiceDetail> InvoiceDetails { get; init; } = new List<InvoiceDetail>();
    public Customer Customer { get; init; } = new Customer();
}

public sealed class InvoiceInfo
{
    public int InvoiceId { get; init; }
    public string DocumentNo{get; init;} = "";
    public DateTime InvoiceDate { get; init; }
    public int CustomerId { get; init; }
    public string IssuerName { get; init; } = "";
    public decimal AmountExcludeVat { get; init; }
    public decimal Vat { get; init; }
    public decimal NetAmount { get; init; }
}