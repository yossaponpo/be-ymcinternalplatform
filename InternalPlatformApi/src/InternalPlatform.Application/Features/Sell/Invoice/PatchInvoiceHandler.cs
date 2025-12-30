using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Sells.PatchInvoice;

public sealed class PatchInvoiceHandler(ISellRepository repo)
{
    public async Task<bool> HandleAsync(Invoice query, CancellationToken ct)
    {
        if (!await repo.InvoiceExistsAsync(query.InvoiceInfo.InvoiceId, ct))
            return false;

        await repo.UpdateInvoiceAsync(query, ct);

        return true;
    }
}
