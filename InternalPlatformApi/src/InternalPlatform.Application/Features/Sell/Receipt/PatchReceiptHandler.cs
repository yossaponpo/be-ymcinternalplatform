using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Sells.PatchReceipt;

public sealed class PatchReceiptHandler(ISellRepository repo)
{
    public async Task<bool> HandleAsync(Receipt query, CancellationToken ct)
    {
        if (!await repo.ReceiptExistsAsync(query.ReceiptInfo.ReceiptId, ct))
            return false;

        await repo.UpdateReceiptAsync(query, ct);

        return true;
    }
}
