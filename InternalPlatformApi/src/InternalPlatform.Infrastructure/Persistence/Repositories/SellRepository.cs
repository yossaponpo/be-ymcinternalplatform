using Dapper;
using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Infrastructure.Persistence.Repositories;

public sealed class SellRepository(IDbConnectionFactory db)
    : ISellRepository
{

    public async Task<Invoice> GetInvoiceByIdAsync(int invoiceId, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"SELECT * FROM invoice_info where invoice_id = @InvoiceId", new { InvoiceId = invoiceId }, cancellationToken: ct);
        var info = await conn.QueryFirstOrDefaultAsync<InvoiceInfo>(cmd);
        cmd = new CommandDefinition(@"SELECT * FROM invoice_detail where invoice_id = @InvoiceId", new { InvoiceId = invoiceId }, cancellationToken: ct);
        var details = await conn.QueryAsync<InvoiceDetail>(cmd);

        return new Invoice()
        {
            InvoiceInfo = info!,
            InvoiceDetails = details.ToList()
        };
    }

    public Task<int> CreateInvoiceAsync(Invoice invoice, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> InvoiceExistsAsync(int invoiceId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpdateInvoiceAsync(Invoice invoice, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<Receipt> GetReceiptByIdAsync(int receiptId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateReceiptAsync(Receipt receipt, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ReceiptExistsAsync(int invoiceId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpdateReceiptAsync(Receipt receipt, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
