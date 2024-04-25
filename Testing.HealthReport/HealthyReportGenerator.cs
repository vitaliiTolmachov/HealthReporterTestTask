using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Testing.HealthReport;

internal class HealthyReportGenerator
{
    private readonly IEnumerable<HealthDataItem> _healthData;
    private string ServiceName { get; }
    private DateTimeOffset StartDate { get; }
    private DateTimeOffset EndDate { get; }

    public HealthyReportGenerator(string serviceName, DateTimeOffset startDate, DateTimeOffset endDate, IEnumerable<HealthDataItem> healthData)
    {
        ServiceName = serviceName;
        StartDate = startDate;
        EndDate = endDate;
        _healthData = healthData;
    }

    internal IReadOnlyCollection<ReportRecord> GenerateHealthinessReport()
    {
        var reportRecords = new List<ReportRecord>();
        for (var currentDate = StartDate; currentDate <= EndDate; currentDate = currentDate.AddDays(1))
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