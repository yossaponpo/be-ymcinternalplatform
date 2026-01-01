namespace InternalPlatform.Domain.Entities;

public class PayrollDetail
{
    public int PayrollDetailId { get; init; }

    public int PayrollId { get; init; }

    public string EmployeeNo { get; init; } = "";

    public decimal WorkingDay { get; init; }
    public decimal WorkingDayAmount { get; init; }

    public decimal PayRate { get; init; }

    public decimal NormalOtHour { get; init; }

    public decimal NormalOtAmount { get; init; }

    public decimal HolidayWorkHour { get; init; }

    public decimal HolidayWorkAmount { get; init; }

    public decimal HolidayOtHour { get; init; }

    public decimal HolidayOtAmount { get; init; }

    public decimal PublicHoliday { get; init; }
    public decimal PublicHolidayAmount { get; init; }

    public decimal AttendanceBonus { get; init; }

    public decimal TransportAllowance { get; init; }

    public decimal OtherIncome { get; init; }

    public decimal GrossIncome { get; init; }

    public decimal SocialSecurityDeduction { get; init; }

    public decimal PenaltyDeduction { get; init; }

    public decimal UniformDeduction { get; init; }

    public decimal ReserverAmount { get; init; }

    public decimal TotalDeduction { get; init; }

    public decimal NetPay { get; init; }
}