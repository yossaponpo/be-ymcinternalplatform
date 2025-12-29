namespace InternalPlatform.Domain.Entities;

public class Payroll
{
    public PayrollInfo PayrollInfo { get; init; } = new PayrollInfo();
    public List<PayrollDetail> PayrollDetails { get; init; } = new List<PayrollDetail>();
}

public sealed class PayrollInfo
{
    public int PayrollId { get; init; }

    public DateTime PayDate { get; init; }

    public DateTime PeriodStart { get; init; }

    public DateTime PeriodEnd { get; init; }
}