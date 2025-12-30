namespace InternalPlatform.Domain.Entities;

public class Receipt
{
    public ReceiptInfo ReceiptInfo { get; init; } = new ReceiptInfo();
    public List<ReceiptDetail> ReceiptDetails { get; init; } = new List<ReceiptDetail>();
    public InvoiceInfo InvoiceInfo { get; init; } = new InvoiceInfo();
    public Customer Customer { get; init; } = new Customer();
}

public sealed class ReceiptInfo
{
    public int ReceiptId { get; init; }
    public string DocumentNo{get; init;} = "";
    public DateTime ReceiptDate { get; init; }
    public int InvoiceId { get; init; }
    public decimal AmountExcludeVat { get; init; }
    public decimal Vat { get; init; }
    public decimal NetAmount { get; init; }
}