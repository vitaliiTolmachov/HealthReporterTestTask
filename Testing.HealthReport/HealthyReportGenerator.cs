using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Testing.HealthReport;

internal class HealthyReportGenerator
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IEnumerable<HealthDataItem> _healthData;

    public HealthyReportGenerator(IDateTimeProvider dateTimeProvider, IEnumerable<HealthDataItem> healthData)
    {
        _dateTimeProvider = dateTimeProvider;
        _healthData = healthData;
    }

    internal IReadOnlyCollection<ReportRecord> GenerateHealthinessReportForPasDays(int generatePeriod)
    {
        if (generatePeriod <= 0)
            throw new ArgumentException("Expected non-negative value", nameof(generatePeriod));
        
        var startDate = _dateTimeProvider.OffsetNow.Date.Date.AddDays(-generatePeriod);
        var endDate = _dateTimeProvider.OffsetNow.Date.Date;
        
        var reportRecords = new List<ReportRecord>(endDate.Subtract(startDate).Days);
        for (var currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
        {
            var healthItemsForDate = _healthData.Where(item => item.Date.Date == currentDate.Date).ToList();
    
            if (healthItemsForDate.Count == 0)
            {
                reportRecords.Add(ReportRecord.Empty(currentDate));
            }
            else
            {
                var totalItems = healthItemsForDate.Count;
                var healthyItems = healthItemsForDate.Count(item => item.Status == HealthStatus.Healthy);
                var unhealthyItems = healthItemsForDate.Count(item => item.Status == HealthStatus.Unhealthy);
                var degradedItems = healthItemsForDate.Count(item => item.Status == HealthStatus.Degraded);

                var uptimePercent = (double) healthyItems / totalItems * 100;
                var unhealthyPercent = (double) unhealthyItems / totalItems * 100;
                var degradedPercent = (double) degradedItems / totalItems * 100;
        
                var serviceName = healthItemsForDate.First().Service;
                var reportRecord = new ReportRecord(currentDate, serviceName, uptimePercent, unhealthyPercent,
                    degradedPercent);
                reportRecords.Add(reportRecord);
            }
        }

        return reportRecords.AsReadOnly();
    }
}