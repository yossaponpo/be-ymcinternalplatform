namespace InternalPlatform.Application.Abstractions.Reports;

public interface IPdfReportGenerator<in T>
{
    byte[] Generate(T data);
}