using System.ComponentModel.DataAnnotations;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;

namespace Testing.HealthReport.UnitTests.ReportGenerator;

public class ReportGeneratorTests
{
    private HealthReport.ReportGenerator _reportGenerator;

    public ReportGeneratorTests()
    {
        _reportGenerator = new HealthReport.ReportGenerator(new DateTimeProvider());
    }

    [Theory, AutoData]
    public void GenerateHealthinessReportForPasDays_ShouldReturnEmptyReport_ForZeroDaysReport(
        string serviceName,
        DateTimeOffset date,
        HealthStatus healthStatus,
        [Range(1, 10)] int healthDataItems)
    {
        //Arrange
        var healthData = new HealthDataItem(serviceName, date, healthStatus);

        //Act
        var expected = _reportGenerator.GenerateReport(0, Enumerable.Repeat(healthData, healthDataItems));

        //Arrange
        expected.Should().BeEmpty();
    }
    
    [Theory, AutoData]
    public void GenerateHealthinessReportForPasDays_ShouldReturnEmptyReport_ForEmptyHealthData(long reportDays)
    {

        //Act
        var expected = _reportGenerator.GenerateReport(reportDays, Enumerable.Empty<HealthDataItem>());

        //Arrange
        expected.Should().BeEmpty();
    }

    [Theory, AutoData]
    public void GenerateReport_Should_ProvideRecordsForAllDates([Range(1, 100)]long daysCount, DateTime currentDate, string serviceName, HealthStatus healthStatus)
    {
        //Arrange
        var dateTimeProviderMock = SetUpDateTimeProviderMock(currentDate);
        _reportGenerator = new HealthReport.ReportGenerator(dateTimeProviderMock);
        var healthDataItem = new HealthDataItem(serviceName, currentDate, healthStatus); 
        
        //Act
        var expected = _reportGenerator.GenerateReport(daysCount, new[] {healthDataItem});

        var expectedDateRange = BuildReportDates(daysCount, currentDate);
        var reportDateRange = expected.Select(x => x.Date.Date).ToArray();
        reportDateRange.Should().BeEquivalentTo(expectedDateRange);
    }
    
    [Theory, AutoData]
    public void GenerateReport_Should_SupportManyServices([Range(1, 100)]long daysCount, DateTime currentDate, string serviceName1, string serviceName2)
    {
        //Arrange
        var dateTimeProviderMock = SetUpDateTimeProviderMock(currentDate);
        _reportGenerator = new HealthReport.ReportGenerator(dateTimeProviderMock);
        var healthDataItemForService1 = new HealthDataItem(serviceName1, currentDate, HealthStatus.Degraded); 
        var healthDataItemForService2 = new HealthDataItem(serviceName2, currentDate, HealthStatus.Healthy); 
        
        //Act
        var expected = _reportGenerator.GenerateReport(daysCount, new[] {healthDataItemForService1, healthDataItemForService2});

        var servicesInReport = expected.Select(x => x.ServiceName).Distinct();
        servicesInReport.Should().BeEquivalentTo(new []{serviceName1, serviceName2});
    }

    private DateTime[] BuildReportDates(long daysCount, DateTime endDate)
    {
        var daysRange = new List<DateTime>();
        var startDate = endDate.AddDays(-daysCount);
        
        for (var reportDate = startDate; reportDate <= endDate; reportDate = reportDate.AddDays(1))
        {
            daysRange.Add(reportDate.Date);
        }

        return daysRange.ToArray();
    }

    private IDateTimeProvider SetUpDateTimeProviderMock(DateTime currentDate)
    {
        var dateTimeProviderMock = Substitute.For<IDateTimeProvider>();
        dateTimeProviderMock.OffsetNow.ReturnsForAnyArgs(currentDate.Date);
        dateTimeProviderMock.Now.ReturnsForAnyArgs(currentDate.Date);
        return dateTimeProviderMock;
    }
}