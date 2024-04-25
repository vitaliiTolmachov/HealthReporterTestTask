// Print health report for past 14 days for each particular day, use IDateTimeProvider to get current date
// Format: {ServiceName} {Date} {Uptime} {UptimePercent} {UnhealthyPercent} {DegradedPercent}
// Consider health data could be unavailable, for example monitoring started 1 day ago, in that case display Unavailable for periods preceding

using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
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

var reportGenerator = new HealthyReportGenerator("Service1", startDate, endDate, healthData);
var reportData = reportGenerator.GenerateHealthinessReport();
var reportPrinter = new RecordPrinter();

foreach (var reportRecord in reportData)
{
    Console.WriteLine(reportPrinter.PrintRawData(reportRecord));
}