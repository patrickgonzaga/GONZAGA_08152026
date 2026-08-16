using FileProcessing.Api.Models;
using FileProcessing.Api.Services;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace FileProcessing.Api.Tests.Services
{
    public class FileProcessingTrackerServiceTests
    {
        private readonly string _tempRoot;
        private readonly IFileProcessingTrackerService _fileProcessingTrackerService;

        public FileProcessingTrackerServiceTests()
        {
            _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempRoot);

            var environmentMock = new Mock<IWebHostEnvironment>();
            environmentMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);

            _fileProcessingTrackerService = new FileProcessingTrackerService(environmentMock.Object);
        }

        [Fact]
        public async Task GetReportsAsync_NothingRecordedYet_ReturnsEmptyReport()
        {
            var report = await _fileProcessingTrackerService.GetReportsAsync(20, CancellationToken.None);

            Assert.Equal(0, report.TotalFilesProcessed);
            Assert.Empty(report.Files);
        }

        [Fact]
        public async Task RecordAsync_ThenGetReportsAsync_ReturnsTheRecordedEntry()
        {
            await _fileProcessingTrackerService.RecordAsync(new FileProcessingLogEntry
            {
                FileName = "sales.csv",
                RecordCount = 5,
                AverageAmount = 100m,
                ProcessedAt = DateTimeOffset.UtcNow,
                Duration = 42
            }, CancellationToken.None);

            var report = await _fileProcessingTrackerService.GetReportsAsync(20, CancellationToken.None);

            Assert.Equal(1, report.TotalFilesProcessed);
            Assert.Single(report.Files);
            Assert.Equal("sales.csv", report.Files[0].FileName);
        }

        [Fact]
        public async Task GetReportsAsync_MoreEntriesThanLimit_TotalReflectsAllEntries_NotJustThePage()
        {
            for (var i = 0; i < 3; i++)
            {
                await _fileProcessingTrackerService.RecordAsync(new FileProcessingLogEntry
                {
                    FileName = $"file{i}.csv",
                    RecordCount = 1,
                    AverageAmount = 1m,
                    ProcessedAt = DateTimeOffset.UtcNow.AddSeconds(i),
                    Duration = 1
                }, CancellationToken.None);
            }

            var report = await _fileProcessingTrackerService.GetReportsAsync(2, CancellationToken.None);

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
