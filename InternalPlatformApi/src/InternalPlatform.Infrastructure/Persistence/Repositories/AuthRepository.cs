using Dapper;
using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;
using InternalPlatform.Infrastructure.Persistence;

namespace InternalPlatform.Infrastructure.Persistence.Repositories;

public sealed class AuthRepository(IDbConnectionFactory db)
    : IAuthRepository
{
    public async Task<CommonResponse> EncryptText(string text, CancellationToken ct)
    {
        var encryptedText = Extensions.Extensions.Encrypt(text);
        return new CommonResponse
        {
            IsSuccess = true,
            Message = encryptedText
        };
    }

    public async Task<CommonResponse> LoginAsync(string email, string password, CancellationToken ct)
    {
        var conn = db.Create();
        var cmd = new CommandDefinition(@"SELECT * FROM user_profile where email = @Email", new { Email = email }, cancellationToken: ct);
        var user = await conn.QueryFirstOrDefaultAsync<UserProfile>(cmd);
        if(user == null)
        {
            return new CommonResponse
            {
                IsSuccess = false,
                Message = "Login failed."
            };
        }else
        {
            var decryptedPassword = Extensions.Extensions.Decrypt(user.Password);
            if(decryptedPassword == password) // For demonstration purposes only
            {
                return new CommonResponse
                {
                    IsSuccess = true
                };
            }
            else
            {
                return new CommonResponse
                {
                    IsSuccess = false,
                    Message = "Login failed."
                };
            }
        }
    }
}
