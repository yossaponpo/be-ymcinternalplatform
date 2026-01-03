namespace InternalPlatform.Domain.DataModels;



public class SearchEmployeeInput : PaginationInput
{
    public string? EmployeeNo { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
}