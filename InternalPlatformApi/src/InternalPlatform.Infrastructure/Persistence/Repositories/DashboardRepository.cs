using Dapper;
using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;
using InternalPlatform.Infrastructure.Extensions;

namespace InternalPlatform.Infrastructure.Persistence.Repositories;

public sealed class DashboardRepository(IDbConnectionFactory db)
    : IDashboardRepository
{
    public async Task<StatCardsResult> GetStatsCard(CancellationToken ct)
    {
        var result = new StatCardsResult
        {
            ActiveEmployees = 0,
            TotalEmployees = 0,
            Income = 0,
            Outcome = 0,
            ServiceStatusText = string.Empty
        };
        try
        {
            using var conn = db.Create();
            conn.Open();
            var activeCmd = new CommandDefinition(@"select count(*) from employee where resigned_date is null", cancellationToken: ct);
            var activeEmp = await conn.QueryFirstOrDefaultAsync<int>(activeCmd);
            result.ActiveEmployees = activeEmp;
            var totalCmd = new CommandDefinition(@"select count(*) from employee", cancellationToken: ct);
            var totalEmp = await conn.QueryFirstOrDefaultAsync<int>(totalCmd);
            result.TotalEmployees = totalEmp;
            var incomeCmd = new CommandDefinition(@"select coalesce(sum(total_amount),0) from invoice_detail", cancellationToken: ct);
            var income = await conn.QueryFirstOrDefaultAsync<decimal>(incomeCmd);
            result.Income = income;
            var outcomeCmd = new CommandDefinition(@"select coalesce(sum(net_pay),0) from payroll_detail", cancellationToken: ct);
            var outcome = await conn.QueryFirstOrDefaultAsync<decimal>(outcomeCmd);
            result.Outcome = outcome;
            result.ServiceStatusText = "Active";
        }
        catch
        {
            result.ServiceStatusText = "Some Systems are experiencing issues";
        }
        return result;
    }
}