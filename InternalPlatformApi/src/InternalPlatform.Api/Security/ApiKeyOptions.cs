namespace InternalPlatform.Api.Security;

public sealed class ApiKeyOptions
{
    public string ApiKey { get; init; } = "";
    public string HeaderName { get; init; } = "X-API-KEY";
}
