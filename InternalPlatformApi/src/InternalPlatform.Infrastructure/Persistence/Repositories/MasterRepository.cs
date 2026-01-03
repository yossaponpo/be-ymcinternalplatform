using Dapper;
using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.DataModels;
using InternalPlatform.Domain.Entities;
using InternalPlatform.Infrastructure.Extensions;

namespace InternalPlatform.Infrastructure.Persistence.Repositories;

public sealed class MasterRepository(IDbConnectionFactory db)
    : IMasterRepository
{
    public async Task<List<Employee>> GetAllEmployeesAsync(CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"select *
            from employee
            order by employee_no", cancellationToken: ct);
        var result = await conn.QueryAsync<Employee>(cmd);
        return result.ToList();
    }

    public async Task<CommonResponse> AddOrEditEmployee(Employee input, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"INSERT INTO employee (employee_no, title, first_name, last_name, date_of_birth, phone_no, customer_id, level, position, start_work_date, resigned_date, address, emp_type_id, bank_id, bank_account_number)
VALUES (@EmployeeNo, @Title, @FirstName, @LastName, @DateOfBirth, @PhoneNo, @CustomerId, @Level, @Position, @StartWorkDate, @ResignedDate, @Address, @EmpTypeId, @BankId, @BankAccountNumber)
ON CONFLICT (employee_no)
DO UPDATE
SET
	title = EXCLUDED.title,
	first_name = EXCLUDED.first_name,
	last_name = EXCLUDED.last_name,
	date_of_birth = EXCLUDED.date_of_birth,
	phone_no = EXCLUDED.phone_no,
	customer_id = EXCLUDED.customer_id,
	level = EXCLUDED.level,
	position = EXCLUDED.position,
	start_work_date = EXCLUDED.start_work_date,
	resigned_date = EXCLUDED.resigned_date,
	address = EXCLUDED.address,
	emp_type_id = EXCLUDED.emp_type_id,
	bank_id = EXCLUDED.bank_id,
	bank_account_number = EXCLUDED.bank_account_number;", input, cancellationToken: ct);
        await conn.ExecuteAsync(cmd);
        return new CommonResponse
        {
            IsSuccess = true
        };
    }

    public async Task<PaginationResult<Employee>> SearchEmployeesAsync(SearchEmployeeInput input, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmdCount = new CommandDefinition(@"select count(*)
            from employee
            where employee_no like @EmployeeNo or first_name like @FirstName or last_name like @LastName",new {EmployeeNo = input.EmployeeNo.ToLikeSQL(), FirstName = input.FirstName.ToLikeSQL(), LastName = input.LastName.ToLikeSQL()}, cancellationToken: ct);
        var count = await conn.QuerySingleAsync<int>(cmdCount);
        if(count == 0)
        {
            return new PaginationResult<Employee>
            {
                CountItems = 0,
                Items = new List<Employee>()
            };
        }
        var cmdData = new CommandDefinition(@"select *
            from employee
            where employee_no like @EmployeeNo or first_name like @FirstName or last_name like @LastName
            order by employee_no
            offset @Offset rows
            fetch next @PageSize rows only",new {EmployeeNo = input.EmployeeNo.ToLikeSQL(), FirstName = input.FirstName.ToLikeSQL(), LastName = input.LastName.ToLikeSQL(),Offset = input.StartIndex,PageSize = input.MaxRecords}, cancellationToken: ct);
        var result = await conn.QueryAsync<Employee>(cmdData);
        return new PaginationResult<Employee>
        {
            CountItems = count,
            Items = result.ToList()
        };
    }

    public async Task<Employee> GetEmployeeAsync(string employeeNo, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"select *
            from employee
            where employee_no = @EmployeeNo", new { EmployeeNo = employeeNo }, cancellationToken: ct);
        var result = await conn.QuerySingleOrDefaultAsync<Employee>(cmd);
        return result!;
    }
}