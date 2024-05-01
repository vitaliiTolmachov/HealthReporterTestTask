using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Testing.HealthReport.UnitTests.RecordPrinter;

public class RecordPrinterTests
{
    private readonly HealthReport.RecordPrinter _recordPrinter = new();

    [Theory, AutoData]
    public void PrintRawData_ShouldPrint_EmptyLabel_ForEmptyRecord(DateTimeOffset recordDate, string serviceName)
    {
        //Arrange
        var emptyRecord = global::ReportRecord.Empty(recordDate, serviceName);

        //Act
        var expected = _recordPrinter.PrintRawData(emptyRecord);

        //Asser
        expected.Should().StartWith($"{recordDate.Date:yyyy-MM-dd}");
        expected.Should().Contain(serviceName);
        expected.Should().EndWith(HealthReport.RecordPrinter.EmptyLabel);
    }

    [Theory, AutoData]
    public void PrintRawData_ShouldPrintStatistics_ForReportRecord(
        DateTimeOffset recordDate,
        string serviceName,
        HealthStatus healthStatus)
    {
        //Arrange
        var reportRecord = new global::ReportRecord(recordDate, serviceName, new[] {healthStatus});

        //Act
        var expected = _recordPrinter.PrintRawData(reportRecord);

        //Assert
        expected.Should().StartWith($"{recordDate.Date:yyyy-MM-dd}");
        expected.Should().Contain(serviceName);
        expected.Should().Contain($"{reportRecord.UptimePercent:F2}%");
        expected.Should().Contain($"{reportRecord.UnhealthyPercent:F2}%");
        expected.Should().Contain($"{reportRecord.DegradedPercent:F2}%");
        expected.Should().NotContain(HealthReport.RecordPrinter.EmptyLabel);

    }
}