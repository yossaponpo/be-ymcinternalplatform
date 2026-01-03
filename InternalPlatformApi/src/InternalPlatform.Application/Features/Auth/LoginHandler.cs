using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Features.Auth;

public sealed class LoginHandler(IAuthRepository repo)
{
    public Task<CommonResponse> HandleAsync(LoginInput query, CancellationToken ct)
        => repo.LoginAsync(query.Email, query.Password, ct);
}
