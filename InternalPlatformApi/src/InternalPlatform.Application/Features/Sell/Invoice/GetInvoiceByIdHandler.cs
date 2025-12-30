using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Sells.GetInvoiceById;

public sealed class GetInvoiceByIdHandler(ISellRepository repo)
{
    public Task<Invoice?> HandleAsync(int query, CancellationToken ct)
        => repo.GetInvoiceByIdAsync(query, ct);
}
