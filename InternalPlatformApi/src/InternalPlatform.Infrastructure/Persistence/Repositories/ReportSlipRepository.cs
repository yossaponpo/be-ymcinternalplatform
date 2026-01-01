using Dapper;
using InternalPlatform.Application.Abstractions.Persistence;
using InternalPlatform.Domain.Entities;
using InternalPlatform.Infrastructure.Extensions;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InternalPlatform.Infrastructure.Persistence.Repositories;

public sealed class ReportSlipRepository(IDbConnectionFactory db)
    : IReportSlipRepository, IDocument
{
    private PayrollInfo? _payrollInfo;
    private PayrollDetail _payrollDetail = new();
    private Employee _employee = new();

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;
    public async Task<byte[]> GenerateSlipReportAsync(int payrollDetailId, CancellationToken ct)
    {
        using var conn = db.Create();
        var cmd = new CommandDefinition(@"select * from payroll_detail where payroll_detail_id = @PayrollDetailId", new { PayrollDetailId = payrollDetailId }, cancellationToken: ct);
        _payrollDetail = await conn.QueryFirstOrDefaultAsync<PayrollDetail>(cmd);
        cmd = new CommandDefinition(@"select * from payroll_info where payroll_id = @PayrollId", new { PayrollId = _payrollDetail.PayrollId }, cancellationToken: ct);
        _payrollInfo = await conn.QueryFirstAsync<PayrollInfo>(cmd);

        //Get employee details
        cmd = new CommandDefinition(@"select * from employee where employee_no = @EmployeeNo", new { EmployeeNo = _payrollDetail.EmployeeNo }, cancellationToken: ct);
        _employee = await conn.QueryFirstOrDefaultAsync<Employee>(cmd);

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
            page.Size(PageSizes.A4.Portrait());
            page.Margin(30);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Kanit"));

            page.Header().Element(c => HeaderSection(c, _payrollInfo!));
            page.Content().Element(ContentSection);
            page.Footer().PaddingTop(8).Row(row =>
            {
                row.RelativeItem().Text("This payslip is generated electronically.").FontColor(Colors.Grey.Darken1);
                row.ConstantItem(120).AlignRight().Text($"Pay Date: {_payrollInfo.PayDate:dd/MM/yyyy}").SemiBold();
            });
        });
    }

    private void HeaderSection(IContainer container, PayrollInfo info)
    {
        var directory = Path.Combine(AppContext.BaseDirectory, "Images", "ymc-logo.png");
        var file = File.OpenRead(directory);
        container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(10).Row(row =>
        {
            // Logo
            row.ConstantItem(70).Height(60).Image(file).FitArea();

            // Company details
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("บริษัท วาย.แม็ค.เซอร์วิส จำกัด").FontSize(14).Bold();
                col.Item().Text("724/44 ม.1 ต.หนองขาม อ.ศรีราชา จ.ชลบุรี 20230").FontColor(Colors.Grey.Darken2);
                col.Item().Text("724/44 Moo.1 Nongkham Sriracha Chonburi 20230").FontColor(Colors.Grey.Darken2);
            });

            // Payslip title
            row.ConstantItem(160).AlignRight().Column(col =>
            {
                col.Item().Text("PAY SLIP").FontSize(16).Bold().AlignRight();
                col.Item().Text($"{info.PeriodStart.Day} {info.PeriodStart.Month.ToThaiMonthName()} {info.PeriodStart.Year} - {info.PeriodEnd.Day} {info.PeriodEnd.Month.ToThaiMonthName()} {info.PeriodEnd.Year} จ่าย {info.PayDate.Day} {info.PayDate.Month.ToThaiMonthName()} {info.PayDate.Year} ").FontColor(Colors.Grey.Darken2).AlignRight();
            });
        });
    }

    private void ContentSection(IContainer container)
    {
        container.PaddingTop(14).Column(col =>
        {
            col.Spacing(10);

            col.Item().Element(EmployeeBlock);

            col.Item().Row(row =>
            {
                row.RelativeItem().Element(EarningsBlock);
                row.ConstantItem(12);
                row.RelativeItem().Element(DeductionsBlock);
            });

            col.Item().Element(SummaryBlock);
        });
    }

    private void EmployeeBlock(IContainer container)
    {
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
        {
            col.Item().Text("ข้อมูลพนักงาน").Bold();

            col.Item().PaddingTop(6).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"รหัสพนักงาน: {_employee.EmployeeNo}");
                    c.Item().Text($"ชื่อ-สกุล: {_employee.Title} {_employee.FirstName} {_employee.LastName}");
                    c.Item().Text($"ตำแหน่ง: {_employee.Position}");
                });
            });
        });
    }

    private static void IncomeLinesTable(IContainer container, PayrollDetail detail)
    {
        container.Table(t =>
        {
            t.ColumnsDefinition(c =>
            {
                c.RelativeColumn(3);
                c.RelativeColumn(1);
            });

            t.Cell().Element(Cell).Text("อัตรา(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.PayRate.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("วันทำงานปกติ(วัน)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.WorkingDay.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("วันทำงานปกติ(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.WorkingDayAmount.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("ล่วงเวลาปกติ(ชม.)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.NormalOtHour.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("ล่วงเวลาปกติ(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.NormalOtAmount.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("ทำงานวันหยุด(ชม.)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.HolidayOtHour.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("ทำงานวันหยุด(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.HolidayOtAmount.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("ล่วงเวลาวันหยุด(ชม.)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.HolidayOtHour.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("ล่วงเวลาวันหยุด(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.HolidayOtAmount.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("วันหยุดประเพณี(วัน)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.PublicHoliday.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("วันหยุดประเพณี(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.PublicHolidayAmount.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("เบี้ยขยัน(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.AttendanceBonus.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("ค่ารถ(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.TransportAllowance.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("รายรับอื่นๆ(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.OtherIncome.ToString("N2")).SemiBold();

            static IContainer Cell(IContainer c) =>
                c.PaddingVertical(2);
        });
    }

    private static void DeductionsLinesTable(IContainer container, PayrollDetail detail)
    {
        container.Table(t =>
        {
            t.ColumnsDefinition(c =>
            {
                c.RelativeColumn(3);
                c.RelativeColumn(1);
            });

            t.Cell().Element(Cell).Text("ปกส(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.SocialSecurityDeduction.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("ค่าปรับ(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.PenaltyDeduction.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("ค่าเสื้อ(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.UniformDeduction.ToString("N2")).SemiBold();

            t.Cell().Element(Cell).Text("สำรองจ่าย(บาท)").Bold();
            t.Cell().Element(Cell).AlignRight().Text(detail.ReserverAmount.ToString("N2")).SemiBold();


            static IContainer Cell(IContainer c) =>
                c.PaddingVertical(2);
        });
    }

    private void EarningsBlock(IContainer container)
    {
        var total = _payrollDetail.GrossIncome;

        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
        {
            col.Item().Text("รายได้").Bold();

            col.Item().PaddingTop(6).Element(c => IncomeLinesTable(c, _payrollDetail));

            col.Item().PaddingTop(8).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Row(r =>
            {
                r.RelativeItem().Text("รวมรายได้").Bold();
                r.ConstantItem(110).AlignRight().Text(total.ToString("N2")).Bold();
            });
        });
    }

    private void DeductionsBlock(IContainer container)
    {
        var total = _payrollDetail.TotalDeduction;

        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(col =>
        {
            col.Item().Text("รายการหัก").Bold();

            col.Item().PaddingTop(6).Element(c => DeductionsLinesTable(c, _payrollDetail));

            col.Item().PaddingTop(8).BorderTop(1).BorderColor(Colors.Grey.Lighten2).Row(r =>
            {
                r.RelativeItem().Text("รวมจ่าย").Bold();
                r.ConstantItem(110).AlignRight().Text(total.ToString("N2")).Bold();
            });
        });
    }

    private void SummaryBlock(IContainer container)
    {
        var totalEarn = _payrollDetail.GrossIncome;
        var totalDed = _payrollDetail.TotalDeduction;
        var netPay = _payrollDetail.NetPay;

        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Row(row =>
        {
            row.RelativeItem().Text("คงเหลือรับ (บาท)").FontSize(12).Bold();
            row.ConstantItem(140).AlignRight().Text(netPay.ToString("N2")).FontSize(12).Bold();
        });
    }
}
