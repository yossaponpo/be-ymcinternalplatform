namespace InternalPlatform.Domain.DataModels;

public class PaginationResult<T>
{
    public int CountItems { get; set; }
    public List<T> Items { get; set; } = new();
}