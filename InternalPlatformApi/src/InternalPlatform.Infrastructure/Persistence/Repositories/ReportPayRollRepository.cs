using Dapper;
using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;
using InternalPlatform.Infrastructure.Extensions;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InternalPlatform.Infrastructure.Persistence.Repositories;

public sealed class ReportPayRollRepository(IDbConnectionFactory db)
    : IReportPayRollRepository, IDocument
{
    private PayrollInfo? _payrollInfo;
    private List<PayrollDetail> _payrollDetails = new();
    private List<Employee> _employees = new();

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;
    public async Task<byte[]> GeneratePayrollReportAsync(int payrollId, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"select * from payroll_info where payroll_id = @PayrollId", new { PayrollId = payrollId }, cancellationToken: ct);
        _payrollInfo = await conn.QueryFirstAsync<PayrollInfo>(cmd);
        //Get payroll details
        cmd = new CommandDefinition(@"select * from payroll_detail where payroll_id = @PayrollId", new { PayrollId = payrollId }, cancellationToken: ct);
        _payrollDetails = (await conn.QueryAsync<PayrollDetail>(cmd)).ToList();
        //Get employee details
        cmd = new CommandDefinition(@"select * from employee where employee_no = any(@EmployeeNos)", new { EmployeeNos = _payrollDetails.Select(pd => pd.EmployeeNo).ToArray() }, cancellationToken: ct);
        _employees = (await conn.QueryAsync<Employee>(cmd)).ToList();

        QuestPDF.Settings.License = LicenseType.Community;

        var fontsDir = Path.Combine(AppContext.BaseDirectory, "Fonts");

        if (Directory.Exists(fontsDir))
        {
            foreach (var file in Directory.GetFiles(fontsDir, "*.ttf"))
            {
                using var fs = File.OpenRead(file);
                FontManager.RegisterFont(fs);
            }
        }

        // Generate the report
        return this.GeneratePdf();
    }

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4.Landscape());
            page.Margin(10);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Kanit"));

            page.Header().Element(c => HeaderSection(c, _payrollInfo!));
            page.Content().Element(ContentSection);
        });
    }

    private void HeaderSection(IContainer container, PayrollInfo info)
    {
        container.PaddingBottom(10).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("บริษัท วาย.แม็ค.เซอร์วิส จำกัด").FontSize(8).SemiBold();
            });
            row.RelativeItem().Column(col =>
            {
                col.Item().Text($"ประจำวันที่ {info.PeriodStart.Day} {info.PeriodStart.Month.ToThaiMonthName()} {info.PeriodStart.Year} ถึง {info.PeriodEnd.Day} {info.PeriodEnd.Month.ToThaiMonthName()} {info.PeriodEnd.Year} จ่าย {info.PayDate.Day} {info.PayDate.Month.ToThaiMonthName()} {info.PayDate.Year}")
                    .FontSize(8);
            });
        });
    }

    private void ContentSection(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(c =>
{
    c.ConstantColumn(10);        // ลำดับ
    c.RelativeColumn(2);         // ชื่อสกุล

    c.RelativeColumn(1.2f);      // อัตรา

    // 5 groups x 2
    c.RelativeColumn(1); c.RelativeColumn(1);  // วันทำงานปกติ (วัน, บาท)
    c.RelativeColumn(1); c.RelativeColumn(1);  // OT ปกติ (ชม, บาท)
    c.RelativeColumn(1); c.RelativeColumn(1);  // ทำงานวันหยุด (วัน, บาท)
    c.RelativeColumn(1); c.RelativeColumn(1);  // OT วันหยุด (ชม, บาท)
    c.RelativeColumn(1); c.RelativeColumn(1);  // ประเพณี (วัน, บาท)

    // 10 singles
    for (int i = 0; i < 10; i++)
        c.RelativeColumn(1.1f);
});

            table.Header(header =>
{
    // ===== Row 1 (Main header) =====
    header.Cell().RowSpan(2).Element(HeaderCellStyle).AlignCenter().Text("");                 // ลำดับ
    header.Cell().RowSpan(2).Element(HeaderCellStyle).AlignCenter().Text("ชื่อสกุล").SemiBold();

    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("อัตรา").SemiBold();           // rate (มี unit row)

    header.Cell().ColumnSpan(2).Element(HeaderCellStyle).AlignCenter().Text("วันทำงานปกติ").SemiBold();
    header.Cell().ColumnSpan(2).Element(HeaderCellStyle).AlignCenter().Text("ล่วงเวลาปกติ").SemiBold();
    header.Cell().ColumnSpan(2).Element(HeaderCellStyle).AlignCenter().Text("ทำงานวันหยุด").SemiBold();
    header.Cell().ColumnSpan(2).Element(HeaderCellStyle).AlignCenter().Text("ล่วงเวลาวันหยุด").SemiBold();
    header.Cell().ColumnSpan(2).Element(HeaderCellStyle).AlignCenter().Text("ประเพณี").SemiBold();

    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("เบี้ยขยัน").SemiBold();
    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("ค่ารถ").SemiBold();
    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("รายรับอื่นๆ").SemiBold();
    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("รวมรับ").SemiBold();

    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("ปกส.").SemiBold();
    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("ค่าปรับ").SemiBold();
    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("ค่าเสื้อ").SemiBold();
    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("สำรองจ่าย").SemiBold();
    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("รวมจ่าย").SemiBold();
    header.Cell().Element(HeaderCellStyle).AlignCenter().Text("คงเหลือรับ").SemiBold();

    // ===== Row 2 (Units row) =====
    // หมายเหตุ: ไม่ต้องใส่ unit สำหรับ 2 คอลัมน์แรก เพราะ RowSpan(2) ไปแล้ว

    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold();          // อัตรา

    // วันทำงานปกติ (2 คอลัมน์)
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("วัน").SemiBold();
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold();

    // ล่วงเวลาปกติ (2 คอลัมน์)
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("ชั่วโมง").SemiBold();
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold();

    // ทำงานวันหยุด (2 คอลัมน์)
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("วัน").SemiBold();
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold();

    // ล่วงเวลาวันหยุด (2 คอลัมน์)
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("ชั่วโมง").SemiBold();
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold();

    // ประเพณี (2 คอลัมน์)
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("วัน").SemiBold();
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold();

    // Single columns (ทั้งหมดเป็น “บาท”)
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold(); // เบี้ยขยัน
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold(); // ค่ารถ
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold(); // รายรับอื่นๆ
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold(); // รวมรับ

    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold(); // ปกส.
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold(); // ค่าปรับ
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold(); // ค่าเสื้อ
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold(); // สำรองจ่าย
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold(); // รวมจ่าย
    header.Cell().Element(SubHeaderCellStyle).AlignCenter().Text("บาท").SemiBold(); // คงเหลือรับ
});
            int rowNo = 1;

            foreach (var detail in _payrollDetails)
            {
                var employee = _employees.FirstOrDefault(e => e.EmployeeNo == detail.EmployeeNo);
                table.Cell().Element(CellStyle).AlignCenter().Text(rowNo.ToString());
                table.Cell().Element(CellStyle).Text($"{employee?.Title}{employee?.FirstName} {employee?.LastName}");
                table.Cell().Element(CellStyle).AlignRight().Text(detail.PayRate.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.WorkingDay.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.WorkingDayAmount.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.NormalOtHour.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.NormalOtAmount.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.HolidayWorkHour.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.HolidayWorkAmount.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.HolidayOtHour.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.HolidayOtAmount.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.PublicHoliday.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.PublicHolidayAmount.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.AttendanceBonus.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.TransportAllowance.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.OtherIncome.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.GrossIncome.ToString("N2"));

                table.Cell().Element(CellStyle).AlignRight().Text(detail.SocialSecurityDeduction.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.PenaltyDeduction.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.UniformDeduction.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.ReserverAmount.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.TotalDeduction.ToString("N2"));
                table.Cell().Element(CellStyle).AlignRight().Text(detail.NetPay.ToString("N2"));

                rowNo++;
            }

            table.Cell().Element(TableFooterCellStyle).AlignCenter().Text(string.Empty);
            table.Cell().Element(TableFooterCellStyle).AlignCenter().Text("รวม");
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(string.Empty);
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.WorkingDay).ToString("N2"));

            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.WorkingDayAmount).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.NormalOtHour).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.NormalOtAmount).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.HolidayWorkHour).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.HolidayWorkAmount).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.HolidayOtHour).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.HolidayOtAmount).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.PublicHoliday).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.PublicHolidayAmount).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.AttendanceBonus).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.TransportAllowance).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.OtherIncome).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.GrossIncome).ToString("N2"));

            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.SocialSecurityDeduction).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.PenaltyDeduction).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.UniformDeduction).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.ReserverAmount).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.TotalDeduction).ToString("N2"));
            table.Cell().Element(TableFooterCellStyle).AlignRight().Text(_payrollDetails.Sum(d => d.NetPay).ToString("N2"));
        });
    }

    private IContainer HeaderCellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .DefaultTextStyle(x =>
                x.FontFamily("Kanit")
                 .FontSize(8)
                 .Bold());
    }

    private IContainer SubHeaderCellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .DefaultTextStyle(x =>
                x.FontFamily("Kanit")
                 .FontSize(8)
                 .SemiBold());
    }

    private IContainer CellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .DefaultTextStyle(x =>
                x.FontFamily("Kanit")
                 .FontSize(6));
    }

    private IContainer TableFooterCellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5)
            .DefaultTextStyle(x =>
                x.FontFamily("Kanit")
                 .FontSize(6)
                 .SemiBold());
    }

}
