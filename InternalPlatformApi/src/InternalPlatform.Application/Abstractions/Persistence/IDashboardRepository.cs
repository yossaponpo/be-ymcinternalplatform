using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Abstractions.Persistence;

public interface IDashboardRepository
{
    Task<StatCardsResult> GetStatsCard(CancellationToken ct);
}