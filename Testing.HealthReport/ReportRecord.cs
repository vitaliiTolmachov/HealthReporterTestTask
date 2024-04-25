namespace Testing.HealthReport;

internal record ReportRecord(
    DateTimeOffset Date,
    string? ServiceName,
    double? UptimePercent,
    double? UnhealthyPercent,
    double? DegradedPercent)
{
    public static ReportRecord Empty(DateTimeOffset currentDate)
    {
        return new ReportRecord(currentDate, default, default, default, default);
    }

    public bool IsEmpty()
    {
        return this.ServiceName == default &&
               this is {UptimePercent: null} &&
               this is {UnhealthyPercent: null} &&
               this is {DegradedPercent: null};
    }
}