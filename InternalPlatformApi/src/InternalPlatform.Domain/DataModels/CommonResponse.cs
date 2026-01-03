namespace InternalPlatform.Domain.DataModels;

public class CommonResponse
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = "";
}