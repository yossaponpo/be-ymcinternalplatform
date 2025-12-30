using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Abstractions.Persistence;

public interface ISellRepository
{
    Task<Invoice> GetInvoiceByIdAsync(int invoiceId, CancellationToken ct);
    Task<int> CreateInvoiceAsync(Invoice invoice, CancellationToken ct);
    Task<bool> InvoiceExistsAsync(int invoiceId, CancellationToken ct);
    Task UpdateInvoiceAsync(Invoice invoice, CancellationToken ct);
    Task<Receipt> GetReceiptByIdAsync(int receiptId, CancellationToken ct);
    Task<int> CreateReceiptAsync(Receipt receipt, CancellationToken ct);
    Task<bool> ReceiptExistsAsync(int invoiceId, CancellationToken ct);
    Task UpdateReceiptAsync(Receipt receipt, CancellationToken ct);
}