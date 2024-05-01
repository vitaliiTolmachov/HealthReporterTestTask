using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Testing.HealthReport;

internal record ReportRecord
{
    private readonly HealthStatus[] _healthStatusesForDate;
    private const int TotalPercentage = 100;
    internal ReportRecord(
        DateTimeOffset date,
        string serviceName,
        HealthStatus[] healthStatusesForDate)
    {
        this.Date = date;
        this.ServiceName = serviceName;
        this._healthStatusesForDate = healthStatusesForDate;
    }

    public DateTimeOffset Date { get; }
    public string ServiceName { get; }
    public double UptimePercent => CalculatePercentage(HealthyItemsCount);
    public double UnhealthyPercent => CalculatePercentage(UnhealthyItemsCount);
    public double DegradedPercent => CalculatePercentage(DegradedItemsCount);
    
    public static ReportRecord Empty(DateTimeOffset currentDate, string serviceName)
    {
        return new ReportRecord(currentDate, serviceName, Array.Empty<HealthStatus>());
    }

    public bool IsEmpty()
    {
        return TotalItems == 0;
    }

    private int TotalItems => _healthStatusesForDate.Length;
    private int HealthyItemsCount => _healthStatusesForDate
        .Count(healthStatus => healthStatus == HealthStatus.Healthy);
    
    private int UnhealthyItemsCount => _healthStatusesForDate
        .Count(healthStatus => healthStatus == HealthStatus.Unhealthy);
    
    private int DegradedItemsCount => _healthStatusesForDate
        .Count(healthStatus => healthStatus == HealthStatus.Degraded);
    
    private double CalculatePercentage(int itemsPerTypeCount)
    {
        return (double) itemsPerTypeCount / TotalItems * TotalPercentage;
    }
}