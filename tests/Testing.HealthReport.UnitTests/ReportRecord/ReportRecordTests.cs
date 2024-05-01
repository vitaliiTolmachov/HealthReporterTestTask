using System.ComponentModel.DataAnnotations;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Testing.HealthReport.UnitTests.ReportRecord;

public class ReportRecordTests
{
    private const int TotalPercentage = 100;
    
    [Theory, AutoData]
    public void IsEmpty_ShouldReturnTrue_WhenHealthDataEmpty(
        DateTimeOffset date,
        string serviceName)
    {
        //Arrange
        var reportRecord = new Testing.HealthReport.ReportRecord(date, serviceName, Array.Empty<HealthStatus>());
        
        //Act
        var expected = reportRecord.IsEmpty();
        
        //Assert
        expected.Should().BeTrue();
    }

    [Theory, AutoData]
    public void UptimePercent_Calculates_Correct(
        DateTimeOffset date,
        string serviceName,
        [Range(1, 10)]int unhealthyItemsCount,
        [Range(11, 100)] int totalItemsCount)
    {
        //Arrange
        var healthyItems = BuildStatuses(HealthStatus.Healthy, unhealthyItemsCount);
        var otherStatuses = BuildStatuses(HealthStatus.Degraded, totalItemsCount-unhealthyItemsCount);
        var totalItemsForTheDay = healthyItems.Concat(otherStatuses).ToArray();
        var reportRecord = new Testing.HealthReport.ReportRecord(date, serviceName, totalItemsForTheDay);
        
        //Act
        var expected =  (double)unhealthyItemsCount / totalItemsCount * TotalPercentage;
        
        //Assert
        reportRecord.UptimePercent.Should().Be(expected);
    }
    
    [Theory, AutoData]
    public void UnhealthyPercent_Calculates_Correct(
        DateTimeOffset date,
        string serviceName,
        [Range(1, 10)]int unhealthyItemsCount,
        [Range(11, 100)]int totalItemsCount)
    {
        //Arrange
        var healthyItems = BuildStatuses(HealthStatus.Unhealthy, unhealthyItemsCount);
        var otherStatuses = BuildStatuses(HealthStatus.Degraded, totalItemsCount-unhealthyItemsCount);
        var totalItemsForTheDay = healthyItems.Concat(otherStatuses).ToArray();
        var reportRecord = new Testing.HealthReport.ReportRecord(date, serviceName, totalItemsForTheDay);
        
        //Act
        var expected =  (double)unhealthyItemsCount / totalItemsCount * TotalPercentage;
        
        //Assert
        reportRecord.UnhealthyPercent.Should().Be(expected);
    }
    
    [Theory, AutoData]
    public void Degraded_Calculates_Correct(
        DateTimeOffset date,
        string serviceName,
        [Range(1, 10)]int degradedItemsCount,
        [Range(11, 100)]int totalItemsCount)
    {
        //Arrange
        var healthyItems = BuildStatuses(HealthStatus.Degraded, degradedItemsCount);
        var otherStatuses = BuildStatuses(HealthStatus.Healthy, totalItemsCount-degradedItemsCount);
        var totalItemsForTheDay = healthyItems.Concat(otherStatuses).ToArray();
        var reportRecord = new Testing.HealthReport.ReportRecord(date, serviceName, totalItemsForTheDay);
        
        //Act
        var expected =  (double)degradedItemsCount / totalItemsCount * TotalPercentage;
        
        //Assert
        reportRecord.DegradedPercent.Should().Be(expected);
    }

    private IEnumerable<HealthStatus> BuildStatuses(HealthStatus statusType, int count)
    {
        return Enumerable.Repeat(statusType, count);
    }
}