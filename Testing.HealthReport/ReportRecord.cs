internal record ReportRecord(
    DateTimeOffset Date,
    string? ServiceName,
    double? UptimePercent,
    double? UnhealthyPercent,
    double? DegradedPercent)
{
    public static ReportRecord Empty(DateTimeOffset currentDate, string serviceName)
    {
        return new ReportRecord(currentDate, serviceName, default, default, default);
    }

    public bool IsEmpty()
    {
        return this is {UptimePercent: null} &&
               this is {UnhealthyPercent: null} &&
               this is {DegradedPercent: null};
    }
}