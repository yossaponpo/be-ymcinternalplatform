using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Sells.GetReceiptById;

public sealed class GetReceiptByIdHandler(ISellRepository repo)
{
    public Task<Receipt?> HandleAsync(int query, CancellationToken ct)
        => repo.GetReceiptByIdAsync(query, ct);
}
