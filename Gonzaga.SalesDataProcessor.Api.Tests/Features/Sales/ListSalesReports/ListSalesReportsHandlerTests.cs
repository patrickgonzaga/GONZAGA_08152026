using Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared;
using Gonzaga.SalesDataProcessor.Api.Features.Sales.ListSalesReports;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text.Json;

namespace Gonzaga.SalesDataProcessor.Api.Tests.Features.Sales.ListSalesReports
{
    public class ListSalesReportsHandlerTests : IDisposable
    {
        private readonly string _tempRoot;
        private readonly ListSalesReportsHandler _handler;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ListSalesReportsHandlerTests()
        {
            _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempRoot);

            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);
            _webHostEnvironment = environmentMock.Object;

            _handler = new ListSalesReportsHandler(
                NullLogger<ListSalesReportsHandler>.Instance, _webHostEnvironment);
        }

        [Fact]
        public async Task Handle_NothingRecordedYet_ReturnsEmptyReport()
        {
            var report = await _handler.Handle(new ListSalesReportsQuery(20), CancellationToken.None);

            Assert.Equal(0, report.TotalFilesProcessed);
            Assert.Empty(report.Files);
        }

        [Fact]
        public async Task Handle_MoreEntriesThanLimit_TotalReflectsAllEntries_NotJustThePage()
        {
            var logFilePath = SalesReportLogFile.GetPath(_webHostEnvironment);

            for (var i = 0; i < 3; i++)
            {
                var entry = new SalesFileReport
                {
                    FileName = $"file{i}.csv",
                    RecordCount = 1,
                    AverageAmount = 1m,
                    ProcessedAt = DateTimeOffset.UtcNow.AddSeconds(i),
                    Duration = 1
                };
                await File.AppendAllTextAsync(logFilePath, JsonSerializer.Serialize(entry) + Environment.NewLine);
            }

            var report = await _handler.Handle(new ListSalesReportsQuery(2), CancellationToken.None);

            Assert.Equal(3, report.TotalFilesProcessed); // total across ALL recorded files
            Assert.Equal(2, report.Files.Count);          // but only 2 returned, per the limit
            Assert.Equal("file2.csv", report.Files[0].FileName); // most recent first
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempRoot))
            {
                Directory.Delete(_tempRoot, recursive: true);
            }
        }
    }
}
