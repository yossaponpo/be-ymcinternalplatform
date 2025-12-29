namespace InternalPlatform.Api.Customers;

public sealed record CustomerListResponse(
    int CustomerId,
    string CustomerName,
    string Branch,
    string TaxNo
);
