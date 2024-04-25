using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Testing.HealthReport;

internal class HealthyReportGenerator(IDateTimeProvider dateTimeProvider, IEnumerable<HealthDataItem> healthData)
{
    internal IReadOnlyCollection<ReportRecord> GenerateHealthinessReportForPasDays(long generatePeriod)
    {
        if (generatePeriod == default)
            throw new ArgumentOutOfRangeException(nameof(generatePeriod), "Expected value more than zero");

        if (!healthData.Any())
            return Array.Empty<ReportRecord>().AsReadOnly();
        
        
        var startDate = dateTimeProvider.OffsetNow.Date.Date.AddDays(-generatePeriod);
        var endDate = dateTimeProvider.OffsetNow.Date.Date;
        
        var dataGroupedByService = GroupHealthDataByService();

        var reportRecords = new List<ReportRecord>(endDate.Subtract(startDate).Days * dataGroupedByService.Count());

        foreach (IGrouping<string,HealthDataItem> serviceDataItems in dataGroupedByService)
        {
            var serviceName = serviceDataItems.Key;
            for (var currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                var serviceDataItemsForDate = serviceDataItems.Where(item => item.Date.Date == currentDate.Date).ToList();
    
                if (serviceDataItemsForDate.Count == 0)
                {
                    reportRecords.Add(ReportRecord.Empty(currentDate, serviceName));
                }
                else
                {
                    var totalItems = serviceDataItemsForDate.Count;
                    var healthyItems = serviceDataItemsForDate.Count(item => item.Status == HealthStatus.Healthy);
                    var unhealthyItems = serviceDataItemsForDate.Count(item => item.Status == HealthStatus.Unhealthy);
                    var degradedItems = serviceDataItemsForDate.Count(item => item.Status == HealthStatus.Degraded);

                    var uptimePercent = (double) healthyItems / totalItems * 100;
                    var unhealthyPercent = (double) unhealthyItems / totalItems * 100;
                    var degradedPercent = (double) degradedItems / totalItems * 100;
                    
                    var reportRecord = new ReportRecord(currentDate, serviceName, uptimePercent, unhealthyPercent,
                        degradedPercent);
                    reportRecords.Add(reportRecord);
                }
            }
        }

        return reportRecords.AsReadOnly();
    }

    private IGrouping<string, HealthDataItem>[] GroupHealthDataByService()
    {
        var healthDataGroupedByService = healthData.GroupBy(x => x.Service);
        var dataGroupedByService = healthDataGroupedByService as IGrouping<string, HealthDataItem>[] ?? healthDataGroupedByService.ToArray();
        return dataGroupedByService;
    }
}