namespace Testing.HealthReport;

internal class RecordPrinter
{
    internal static string EmptyLabel = "Unavailable";
    public string PrintRawData(ReportRecord reportRecord)
    {
        return reportRecord.IsEmpty() ?
            $"{reportRecord.Date:yyyy-MM-dd} {reportRecord.ServiceName} {EmptyLabel}" :
            $"{reportRecord.Date:yyyy-MM-dd} {reportRecord.ServiceName} {reportRecord.UptimePercent:F2}% {reportRecord.UnhealthyPercent:F2}% {reportRecord.DegradedPercent:F2}%";
    }
}