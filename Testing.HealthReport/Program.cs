// Print health report for past 14 days for each particular day, use IDateTimeProvider to get current date
// Format: {ServiceName} {Date} {Uptime} {UptimePercent} {UnhealthyPercent} {DegradedPercent}
// Consider health data could be unavailable, for example monitoring started 1 day ago, in that case display Unavailable for periods preceding

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Testing.HealthReport;

IDateTimeProvider dateProvider = new DateTimeProvider();
var healthData = new List<HealthDataItem>()
{
    new ("Service1", DateTimeOffset.Parse("2023-07-01 05:50:34 +03:00"), HealthStatus.Healthy),
    new ("Service1", DateTimeOffset.Parse("2023-07-02 05:50:34 +03:00"), HealthStatus.Unhealthy),
    new ("Service1", DateTimeOffset.Parse("2023-07-09 05:50:34 +03:00"), HealthStatus.Healthy),
    new ("Service1", DateTimeOffset.Parse("2023-07-10 03:50:34 +03:00"), HealthStatus.Degraded),
    new ("Service1", DateTimeOffset.Parse("2023-07-10 03:55:04 +03:00"), HealthStatus.Healthy),
    new ("Service1", DateTimeOffset.Parse("2023-07-11 03:55:04 +03:00"), HealthStatus.Unhealthy),
    new ("Service1", DateTimeOffset.Parse("2023-07-11 04:15:04 +03:00"), HealthStatus.Healthy)
};
Console.WriteLine($"Report generated date: {dateProvider.OffsetNow}");

var endDate = dateProvider.OffsetNow;
var startDate = endDate.AddDays(-14);

for (var currentDate = startDate; currentDate <= endDate; currentDate = currentDate.AddDays(1))
{
    var healthItemsForDate = healthData.Where(item => item.Date.Date == currentDate.Date).ToList();
    
    if (healthItemsForDate.Count == 0)
    {
        Console.WriteLine($"{currentDate.Date:yyyy-MM-dd} Unavailable");
    }
    else
    {
        var totalItems = healthItemsForDate.Count;
        var healthyItems = healthItemsForDate.Count(item => item.Status == HealthStatus.Healthy);
        var degradedItems = healthItemsForDate.Count(item => item.Status == HealthStatus.Degraded);
        var unhealthyItems = healthItemsForDate.Count(item => item.Status == HealthStatus.Unhealthy);

        var uptimePercent = (double) healthyItems / totalItems * 100;
        var degradedPercent = (double) degradedItems / totalItems * 100;
        var unhealthyPercent = (double) unhealthyItems / totalItems * 100;
        
        var serviceName = healthItemsForDate.First().Service;

        Console.WriteLine(
            $"{currentDate.Date:yyyy-MM-dd} {serviceName} {uptimePercent:F2}% {unhealthyPercent:F2}% {degradedPercent:F2}%");
    }
}