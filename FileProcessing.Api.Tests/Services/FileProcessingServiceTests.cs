using FileProcessing.Api.Models;
using FileProcessing.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text;

namespace FileProcessing.Api.Tests.Services
{
    public class FileProcessingServiceTests
    {
        [Fact]
        public async Task ProcessFileAsync_ValidCsv_ReturnsCorrectAverageAndRecordsTracker()
        {
            var file = CreateCsvFile("sales.csv", "Id,Name,Amount\n1,A,100\n2,B,300\n");
            var trackerMock = new Mock<IFileProcessingTrackerService>();
            FileProcessingLogEntry? recordedEntry = null;

            trackerMock
                .Setup(t => t.RecordAsync(It.IsAny<FileProcessingLogEntry>(), It.IsAny<CancellationToken>()))
                .Callback<FileProcessingLogEntry, CancellationToken>((entry, _) => recordedEntry = entry)
                .Returns(Task.CompletedTask);

            var service = new FileProcessingService(
                NullLogger<FileProcessingService>.Instance, new FileValidator(), trackerMock.Object);

            var result = await service.ProcessFileAsync(file, CancellationToken.None);

            Assert.Equal("sales.csv", result.FileName);
            Assert.Equal(2, result.RecordCount);
            Assert.Equal(200, result.AverageAmount);

            trackerMock.Verify(
                t => t.RecordAsync(It.IsAny<FileProcessingLogEntry>(), It.IsAny<CancellationToken>()),
                Times.Once);
            Assert.NotNull(recordedEntry);
            Assert.Equal(2, recordedEntry!.RecordCount);
        }

        [Fact]
        public async Task ProcessFileAsync_HeaderOnlyCsv_ThrowsAndDoesNotRecord()
        {
            var file = CreateCsvFile("empty.csv", "Id,Name,Amount\n");
            var trackerMock = new Mock<IFileProcessingTrackerService>();

            var service = new FileProcessingService(
                NullLogger<FileProcessingService>.Instance, new FileValidator(), trackerMock.Object);

            await Assert.ThrowsAsync<InvalidDataException>(
                () => service.ProcessFileAsync(file, CancellationToken.None));

            trackerMock.Verify(
                t => t.RecordAsync(It.IsAny<FileProcessingLogEntry>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }

        private static IFormFile CreateCsvFile(string fileName, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            return new FormFile(stream, 0, stream.Length, "file", fileName);
        }
    }
}
