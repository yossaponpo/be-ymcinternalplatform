using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Application.Abstractions.Persistence;

public interface IAuthRepository
{
    Task<CommonResponse> LoginAsync(string email, string password, CancellationToken ct);
    Task<CommonResponse> EncryptText(string text, CancellationToken ct);
}