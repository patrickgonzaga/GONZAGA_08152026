using Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared;
using Gonzaga.SalesDataProcessor.Api.Features.Sales.CreateSalesReport;
using Microsoft.AspNetCore.Hosting;
using Moq;
using System.Text.Json;

namespace Gonzaga.SalesDataProcessor.Api.Tests.Features.Sales.CreateSalesReport
{
    public class CreateSalesReportCommandHandlerTests : IDisposable
    {
        private readonly string _tempRoot;
        private readonly CreateSalesReportCommandHandler _handler;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateSalesReportCommandHandlerTests()
        {
            _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempRoot);

            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);
            _webHostEnvironment = environmentMock.Object;

            _handler = new CreateSalesReportCommandHandler(_webHostEnvironment);
        }

        [Fact]
        public async Task Handle_AppendsSerializedEntryToLogFile()
        {
            var entry = new SalesFileReport
            {
                FileName = "sales.csv",
                RecordCount = 5,
                AverageAmount = 100m,
                ProcessedAt = DateTimeOffset.UtcNow,
                Duration = 42
            };

            await _handler.Handle(new CreateSalesReportCommand(entry), CancellationToken.None);

            var logFilePath = SalesReportLogFile.GetPath(_webHostEnvironment);
            Assert.True(File.Exists(logFilePath));

            var lines = await File.ReadAllLinesAsync(logFilePath);
            var written = JsonSerializer.Deserialize<SalesFileReport>(lines.Single());
            Assert.Equal("sales.csv", written!.FileName);
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
