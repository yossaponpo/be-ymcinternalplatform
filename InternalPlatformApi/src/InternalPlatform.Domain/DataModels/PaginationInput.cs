namespace InternalPlatform.Domain.DataModels;

public class PaginationInput
{
    public int StartIndex { get; init; } = 1;
    public int MaxRecords { get; init; } = 10;
}