namespace InternalPlatform.Domain.DataModels;

public sealed class LoginInput
{
    public string Email { get; init; } = "";
    public string Password { get; init; } = "";
}