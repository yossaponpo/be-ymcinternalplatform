namespace InternalPlatform.Api.Customers;

public sealed record CustomerResponse(
    int CustomerId,
    string CustomerName,
    string Address,
    string Branch,
    string TaxNo
);
