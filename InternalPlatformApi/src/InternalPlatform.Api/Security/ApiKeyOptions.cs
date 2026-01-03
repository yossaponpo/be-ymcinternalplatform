namespace InternalPlatform.Api.Security;

public sealed class ApiKeyOptions
{
    public string ApiKey
    {
        get
        {
            return Environment.GetEnvironmentVariable("ApiKey") ?? "";
        }
    }
    public string HeaderName
    {
        get
        {
            return Environment.GetEnvironmentVariable("HeaderName") ?? "";
        }
    }
}
