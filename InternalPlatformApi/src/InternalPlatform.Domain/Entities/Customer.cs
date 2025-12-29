namespace InternalPlatform.Domain.Entities;

public sealed class Customer
{
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = "";
    public string Address { get; init; } = "";
    public string Branch { get; init; } = "";
    public string TaxNo { get; init; } = "";
}