namespace InternalPlatform.Domain.Entities;

public class ReceiptDetail
{
    public int ReceiptDetailId { get; init; }
    public int ReceiptId { get; init; }
    public int RowNo { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal UnitAmount { get; init; }
    public string Unit { get; init; } = string.Empty;
    public decimal PricePerUnit { get; init; }
    public decimal TotalAmount { get; init; }
}