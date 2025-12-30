using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Sells.CreateInvoice;

public sealed class CreateInvoiceHandler(ISellRepository repo)
{
    public Task<int> HandleAsync(Invoice query, CancellationToken ct)
        => repo.CreateInvoiceAsync(query, ct);
}
