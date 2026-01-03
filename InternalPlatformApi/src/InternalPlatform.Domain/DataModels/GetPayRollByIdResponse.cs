using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Domain.DataModels;

public class GetPayrollByIdResponse : CommonResponse
{
    public PayrollInfo PayrollInfo { get; init; } = new PayrollInfo();
    public List<PayrollDetailItem> PayrollDetails { get; init; } = [];
}

public class PayrollDetailItem
{
    public PayrollDetail Detail { get; init; } = new();
    public Employee Employee { get; init; } = new();
}