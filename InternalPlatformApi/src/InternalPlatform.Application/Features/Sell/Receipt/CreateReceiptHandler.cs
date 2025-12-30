using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Sells.CreateReceipt;

public sealed class CreateReceiptHandler(ISellRepository repo)
{
    public Task<int> HandleAsync(Receipt query, CancellationToken ct)
        => repo.CreateReceiptAsync(query, ct);
}
