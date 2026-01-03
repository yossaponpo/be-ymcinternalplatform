using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Payrolls.SearchPayroll;

public sealed class SearchPayrollHandler(IPayrollRepository repo)
{
    public async Task<SearchPayRollResponse> HandleAsync(SearchPayRollInput input, CancellationToken ct)
    {
        var result = await repo.SearchAsync(input, ct);

        return result;
    }
}
