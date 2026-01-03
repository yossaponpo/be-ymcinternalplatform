namespace InternalPlatform.Domain.Entities;

public sealed class UserProfile
{
    public string Email { get; init; } = "";
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string Password { get; init; } = "";
}