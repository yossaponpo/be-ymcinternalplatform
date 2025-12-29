using Dapper;
using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Infrastructure.Persistence.Repositories;

public sealed class PayrollRepository(IDbConnectionFactory db)
    : IPayrollRepository
{
    public async Task<Payroll> GetByIdAsync(int payrollId, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"SELECT * FROM payroll_info where payroll_id = @PayrollId", new { PayrollId = payrollId }, cancellationToken: ct);
        var info = await conn.QueryFirstOrDefaultAsync<PayrollInfo>(cmd);
        cmd = new CommandDefinition(@"SELECT * FROM payroll_detail where payroll_id = @PayrollId", new { PayrollId = payrollId }, cancellationToken: ct);
        var details = await conn.QueryAsync<PayrollDetail>(cmd);

        return new Payroll()
        {
            PayrollInfo = info!,
            PayrollDetails = details.ToList()
        };
    }

    public async Task<int> CreateAsync(Payroll payroll, CancellationToken ct)
    {
        using var conn = db.Create();
        conn.Open();

        using var tx = conn.BeginTransaction();

        try
        {
            // Insert header and get ID
            var payrollId = await conn.ExecuteScalarAsync<int>(
                @"insert into payroll_info (pay_date, period_start, period_end)
        values (@PayDate, @PeriodStart, @PeriodEnd)
        returning payroll_id",
                payroll.PayrollInfo,
                tx);

            // Assign ID to details
            var details = payroll.PayrollDetails.Select(d => new
            {
                PayrollId = payrollId,
                d.EmployeeNo,
                d.WorkingDay,
                d.PayRate,
                d.NormalOtHour,
                d.NormalOtAmount,
                d.HolidayWorkHour,
                d.HolidayWorkAmount,
                d.HolidayOtHour,
                d.HolidayOtAmount,
                d.AttendanceBonus,
                d.TransportAllowance,
                d.OtherIncome,
                d.GrossIncome,
                d.SocialSecurityDeduction,
                d.PenaltyDeduction,
                d.UniformDeduction,
                d.ReserverAmount,
                d.TotalDeduction,
                d.NetPay
            }).ToList();

            // Insert details
            await conn.ExecuteAsync(@"
        insert into payroll_detail
        (
            payroll_id,
            employee_no,
            working_day,
            pay_rate,
            normal_ot_hour,
            normal_ot_amount,
            holiday_work_hour,
            holiday_work_amount,
            holiday_ot_hour,
            holiday_ot_amount,
            attendance_bonus,
            transport_allowance,
            other_income,
            gross_income,
            social_security_deduction,
            penalty_deduction,
            uniform_deduction,
            reserver_amount,
            total_deduction,
            net_pay
        )
        values
        (
            @PayrollId,
            @EmployeeNo,
            @WorkingDay,
            @PayRate,
            @NormalOtHour,
            @NormalOtAmount,
            @HolidayWorkHour,
            @HolidayWorkAmount,
            @HolidayOtHour,
            @HolidayOtAmount,
            @AttendanceBonus,
            @TransportAllowance,
            @OtherIncome,
            @GrossIncome,
            @SocialSecurityDeduction,
            @PenaltyDeduction,
            @UniformDeduction,
            @ReserverAmount,
            @TotalDeduction,
            @NetPay
        )",
                details,
                tx);

            tx.Commit();
            return payrollId;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int payrollId, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"select * from payroll_info where payroll_id = @PayrollId", new { PayrollId = payrollId }, cancellationToken: ct);
        var result = await conn.QueryFirstOrDefaultAsync<PayrollInfo>(cmd);
        return result is not null;
    }

    public async Task UpdateAsync(Payroll payroll, CancellationToken ct)
    {
        using var conn = db.Create();
        conn.Open();

        using var tx = conn.BeginTransaction();

        try
        {
            // Insert header and get ID
            var payrollId = await conn.ExecuteScalarAsync<int>(
                @"update payroll_info set 
                pay_date=@PayDate,
                period_start = @PeriodStart,
                period_end = @PeriodEnd)
                where payroll_id = @PayrollId",
                payroll.PayrollInfo,
                tx);

            // Delete details
            await conn.ExecuteAsync(
                @"delete from payroll_detail where payroll_id = @PayrollId",
                new { PayrollId = payrollId },
                tx);

            // Assign ID to details
            var details = payroll.PayrollDetails.Select(d => new
            {
                PayrollId = payrollId,
                d.EmployeeNo,
                d.WorkingDay,
                d.PayRate,
                d.NormalOtHour,
                d.NormalOtAmount,
                d.HolidayWorkHour,
                d.HolidayWorkAmount,
                d.HolidayOtHour,
                d.HolidayOtAmount,
                d.AttendanceBonus,
                d.TransportAllowance,
                d.OtherIncome,
                d.GrossIncome,
                d.SocialSecurityDeduction,
                d.PenaltyDeduction,
                d.UniformDeduction,
                d.ReserverAmount,
                d.TotalDeduction,
                d.NetPay
            }).ToList();

            // Insert details
            await conn.ExecuteAsync(@"
        insert into payroll_detail
        (
            payroll_id,
            employee_no,
            working_day,
            pay_rate,
            normal_ot_hour,
            normal_ot_amount,
            holiday_work_hour,
            holiday_work_amount,
            holiday_ot_hour,
            holiday_ot_amount,
            attendance_bonus,
            transport_allowance,
            other_income,
            gross_income,
            social_security_deduction,
            penalty_deduction,
            uniform_deduction,
            reserver_amount,
            total_deduction,
            net_pay
        )
        values
        (
            @PayrollId,
            @EmployeeNo,
            @WorkingDay,
            @PayRate,
            @NormalOtHour,
            @NormalOtAmount,
            @HolidayWorkHour,
            @HolidayWorkAmount,
            @HolidayOtHour,
            @HolidayOtAmount,
            @AttendanceBonus,
            @TransportAllowance,
            @OtherIncome,
            @GrossIncome,
            @SocialSecurityDeduction,
            @PenaltyDeduction,
            @UniformDeduction,
            @ReserverAmount,
            @TotalDeduction,
            @NetPay
        )",
                details,
                tx);

            tx.Commit();
        }
        catch
        {
            tx.Rollback();
            throw;
        }

    }
}
