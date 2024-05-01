using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Testing.HealthReport;

internal class ReportGenerator(IDateTimeProvider dateTimeProvider)
{
    internal IReadOnlyCollection<ReportRecord> GenerateReport(long pastDaysCount, IEnumerable<HealthDataItem> healthData)
    {
        var healthDataItems = healthData as HealthDataItem[] ?? healthData.ToArray();
        if (healthDataItems.Length == 0 || pastDaysCount == default)
            return Array.Empty<ReportRecord>().AsReadOnly();
        
        
        var startDate = dateTimeProvider.OffsetNow.Date.Date.AddDays(-pastDaysCount);
        var endDate = dateTimeProvider.OffsetNow.Date.Date;
        
        var dataGroupedByService = GroupHealthDataByService(healthDataItems);
        var expectedReportRecordsCount = endDate.Subtract(startDate).Days * dataGroupedByService.Count();

        var reportRecords = new List<ReportRecord>(expectedReportRecordsCount);

        foreach (IGrouping<string,HealthDataItem> serviceDataItems in dataGroupedByService)
        {
            var serviceName = serviceDataItems.Key;
            for (var currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
            {
                var healthyStatusesForDate = GetHealthyStatusesForDate(serviceDataItems, currentDate);
                reportRecords.Add(new ReportRecord(currentDate.Date, serviceName, healthyStatusesForDate));
            }
        }

        return reportRecords.AsReadOnly();
    }

    private static HealthStatus[] GetHealthyStatusesForDate(IGrouping<string, HealthDataItem> serviceDataItems, DateTime currentDate)
    {
        return serviceDataItems
            .Where(item => item.Date.Date == currentDate.Date)
            .Select(x => x.Status)
            .ToArray();
    }

    private IGrouping<string, HealthDataItem>[] GroupHealthDataByService(IEnumerable<HealthDataItem> healthData)
    {
        var healthDataGroupedByService = healthData.GroupBy(x => x.Service);
        var dataGroupedByService = healthDataGroupedByService as IGrouping<string, HealthDataItem>[] ?? healthDataGroupedByService.ToArray();
        return dataGroupedByService;
    }
}