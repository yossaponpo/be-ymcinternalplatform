using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Api.Payrolls;

public sealed record GetPayrollByIdResponse(
    Payroll Payroll
);
