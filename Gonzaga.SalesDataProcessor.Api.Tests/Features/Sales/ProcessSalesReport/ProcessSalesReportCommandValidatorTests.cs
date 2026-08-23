using Gonzaga.SalesDataProcessor.Api.Features.Sales.ProcessSalesReport;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Gonzaga.SalesDataProcessor.Api.Tests.Features.Sales.ProcessSalesReport
{
    public class ProcessSalesReportCommandValidatorTests
    {
        private readonly ProcessSalesReportCommandValidator _validator = new();

        [Fact]
        public async Task ValidateAsync_EmptyFile_Fails()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);
            fileMock.Setup(f => f.FileName).Returns("test.csv");

            var result = await _validator.ValidateAsync(new ProcessSalesReportCommand { File = fileMock.Object });

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_NonCsvExtension_Fails()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("test.txt");

            var result = await _validator.ValidateAsync(new ProcessSalesReportCommand { File = fileMock.Object });

            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task ValidateAsync_ValidCsvFile_Passes()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("test.csv");

            var result = await _validator.ValidateAsync(new ProcessSalesReportCommand { File = fileMock.Object });

            Assert.True(result.IsValid);
        }
    }
}
