using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Dashboards;

public sealed class StatCardHandler(IDashboardRepository repo)
{
    public async Task<StatCardsResult> HandleAsync(CancellationToken ct)
    {
        var result = await repo.GetStatsCard(ct);
        return result;
    }
}
