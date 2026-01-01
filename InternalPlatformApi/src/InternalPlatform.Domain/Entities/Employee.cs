namespace InternalPlatform.Domain.Entities;

public sealed class Employee
{
    public string EmployeeNo { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime? DateOfBirth { get; set; }
    public string? PhoneNo { get; set; }
    public int? CustomerId { get; set; }
    public string Level { get; set; } = null!;
    public string Position { get; set; } = null!;
    public DateTime StartWorkDate { get; set; }
    public DateTime? ResignedDate { get; set; }
    public string? Address { get; set; }
    public int EmpTypeId { get; set; }
    public int? BankId { get; set; }
    public string? BankAccountNumber { get; set; }
}