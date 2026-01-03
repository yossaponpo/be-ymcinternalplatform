using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Auth;

public sealed class EncryptHandler(IAuthRepository repo)
{
    public Task<CommonResponse> HandleAsync(string text, CancellationToken ct)
        => repo.EncryptText(text, ct);
}
