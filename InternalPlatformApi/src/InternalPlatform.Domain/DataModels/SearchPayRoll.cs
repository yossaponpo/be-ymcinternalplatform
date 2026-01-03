namespace InternalPlatform.Domain.DataModels;

public class SearchPayRollResponse : CommonResponse
{
    public int CountItems { get; init; }
    public List<PayRollItem> Items { get; init; } = [];
}

public class PayRollItem
{
    public int PayrollId { get; init; }
    public DateTime PayDate { get; init; }
    public DateTime PeriodStart { get; init; }
    public DateTime PeriodEnd { get; init; }
    public decimal TotalAmount { get; init; }
}

public class SearchPayRollInput : PaginationInput
{
    public DateTime? FromPayDate { get; init; }
    public DateTime? ToPayDate { get; init; }
}