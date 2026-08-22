using Gonzaga.SalesDataProcessor.Api.Features.Sales.CreateSalesReport;
using Gonzaga.SalesDataProcessor.Api.Features.Sales.ProcessSalesReport;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text;

namespace Gonzaga.SalesDataProcessor.Api.Tests.Features.Sales.ProcessSalesReport
{
    public class ProcessSalesReportHandlerTests
    {
        [Fact]
        public async Task Handle_ValidCsv_ReturnsCorrectAverageAndDispatchesCreateSalesReportCommand()
        {
            var file = CreateCsvFile("sales.csv", "Id,Name,Amount\n1,A,100\n2,B,300\n");
            var senderMock = new Mock<ISender>();
            CreateSalesReportCommand? dispatchedCommand = null;

            senderMock
                .Setup(s => s.Send(It.IsAny<CreateSalesReportCommand>(), It.IsAny<CancellationToken>()))
                .Callback<object, CancellationToken>((cmd, _) => dispatchedCommand = (CreateSalesReportCommand)cmd)
                .Returns(Task.CompletedTask);

            var handler = new ProcessSalesReportCommandHandler(
                NullLogger<ProcessSalesReportCommandHandler>.Instance, senderMock.Object);

            var result = await handler.Handle(new ProcessSalesReportCommand { File = file }, CancellationToken.None);

            Assert.Equal("sales.csv", result.FileName);
            Assert.Equal(2, result.RecordCount);
            Assert.Equal(200, result.AverageAmount);

            senderMock.Verify(
                s => s.Send(It.IsAny<CreateSalesReportCommand>(), It.IsAny<CancellationToken>()),
                Times.Once);
            Assert.NotNull(dispatchedCommand);
            Assert.Equal(2, dispatchedCommand!.LogEntry.RecordCount);
        }

        [Fact]
        public async Task Handle_HeaderOnlyCsv_ThrowsAndDoesNotDispatchCreateSalesReportCommand()
        {
            var file = CreateCsvFile("empty.csv", "Id,Name,Amount\n");
            var senderMock = new Mock<ISender>();

            var handler = new ProcessSalesReportCommandHandler(
                NullLogger<ProcessSalesReportCommandHandler>.Instance, senderMock.Object);

            await Assert.ThrowsAsync<InvalidDataException>(
                () => handler.Handle(new ProcessSalesReportCommand { File = file }, CancellationToken.None));

            senderMock.Verify(
                s => s.Send(It.IsAny<CreateSalesReportCommand>(), It.IsAny<CancellationToken>()),
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
