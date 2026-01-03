using System.Data;
using Dapper;
using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;

namespace InternalPlatform.Infrastructure.Persistence.Repositories;

public sealed class PayrollRepository(IDbConnectionFactory db)
    : IPayrollRepository
{
    public async Task<GetPayrollByIdResponse> GetByIdAsync(int payrollId, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"SELECT * FROM payroll_info where payroll_id = @PayrollId", new { PayrollId = payrollId }, cancellationToken: ct);
        var info = await conn.QueryFirstOrDefaultAsync<PayrollInfo>(cmd);

        cmd = new CommandDefinition(@"
SELECT
  dt.payroll_detail_id,
  dt.payroll_id,
  dt.employee_no,
  dt.working_day,
  dt.working_day_amount,
  dt.pay_rate,
  dt.normal_ot_hour,
  dt.normal_ot_amount,
  dt.holiday_work_hour,
  dt.holiday_work_amount,
  dt.holiday_ot_hour,
  dt.holiday_ot_amount,
  dt.public_holiday,
  dt.public_holiday_amount,
  dt.attendance_bonus,
  dt.transport_allowance,
  dt.other_income,
  dt.gross_income,
  dt.social_security_deduction,
  dt.penalty_deduction,
  dt.uniform_deduction,
  dt.reserver_amount,
  dt.total_deduction,
  dt.net_pay,

  e.employee_no AS employee_no_split,
  e.employee_no,
  e.title,
  e.first_name,
  e.last_name,
  e.date_of_birth,
  e.phone_no,
  e.customer_id,
  e.level,
  e.position,
  e.start_work_date,
  e.resigned_date,
  e.address,
  e.emp_type_id,
  e.bank_id,
  e.bank_account_number
FROM payroll_detail dt
LEFT JOIN employee e ON dt.employee_no = e.employee_no
WHERE dt.payroll_id = @PayrollId
ORDER BY dt.payroll_detail_id
", new { PayrollId = payrollId }, cancellationToken: ct);


        var rows = await conn.QueryAsync<PayrollDetail, Employee, PayrollDetailItem>(
    cmd.CommandText,
    (detail, employee) => new PayrollDetailItem
    {
        Detail = detail,
        Employee = employee ?? new Employee()
    },
    param: cmd.Parameters,
    splitOn: "employee_no_split"
);

        var details = rows.ToList();

        return new GetPayrollByIdResponse()
        {
            IsSuccess = info is not null,
            PayrollInfo = info,
            PayrollDetails = details
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
                d.WorkingDayAmount,
                d.PayRate,
                d.NormalOtHour,
                d.NormalOtAmount,
                d.HolidayWorkHour,
                d.HolidayWorkAmount,
                d.HolidayOtHour,
                d.HolidayOtAmount,
                d.PublicHoliday,
                d.PublicHolidayAmount,
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
            working_day_amount,
            pay_rate,
            normal_ot_hour,
            normal_ot_amount,
            holiday_work_hour,
            holiday_work_amount,
            holiday_ot_hour,
            holiday_ot_amount,
            public_holiday,
            public_holiday_amount,
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
            @WorkingDayAmount,
            @PayRate,
            @NormalOtHour,
            @NormalOtAmount,
            @HolidayWorkHour,
            @HolidayWorkAmount,
            @HolidayOtHour,
            @HolidayOtAmount,
            @PublicHoliday,
            @PublicHolidayAmount,
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

    public async Task<SearchPayRollResponse> SearchAsync(SearchPayRollInput input, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"
            select count(*) 
            from payroll_info
            where pay_date >= coalesce(@FromPayDate, pay_date)
              and pay_date <= coalesce(@ToPayDate, pay_date)",
            new
            {
                FromPayDate = input.FromPayDate,
                ToPayDate = input.ToPayDate
            },
            cancellationToken: ct);

        var countItems = await conn.QueryFirstOrDefaultAsync<int>(cmd);
        if (countItems == 0)
        {
            return new SearchPayRollResponse()
            {
                IsSuccess = true,
                CountItems = 0,
                Items = []
            };
        }
        else
        {
            cmd = new CommandDefinition(@"
            SELECT i.*,SUM(dt.net_pay) as total_amount FROM payroll.payroll_info i
            LEFT JOIN payroll.payroll_detail dt on i.payroll_id = dt.payroll_id
            WHERE pay_date >= coalesce(@FromPayDate, pay_date)
              AND pay_date <= coalesce(@ToPayDate, pay_date)
            GROUP BY i.payroll_id,i.pay_date,i.period_start,i.period_end
            ORDER BY i.payroll_id
            offset @Offset rows
            fetch next @PageSize rows only",
            new
            {
                FromPayDate = input.FromPayDate,
                ToPayDate = input.ToPayDate,
                Offset = input.StartIndex,
                PageSize = input.MaxRecords
            },
            cancellationToken: ct);

            var result = await conn.QueryAsync<PayRollItem>(cmd);
            return new SearchPayRollResponse()
            {
                IsSuccess = true,
                CountItems = countItems,
                Items = result.AsList()
            };
        }
    }
}
