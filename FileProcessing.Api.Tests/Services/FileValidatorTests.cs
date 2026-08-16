using FileProcessing.Api.Services;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FileProcessing.Api.Tests.Services
{
    public class FileValidatorTests
    {
        private readonly FileValidator validator = new();

        [Fact]
        public async Task ValidateAsync_NullFile_ThrowsInvalidDataException()
        {
            await Assert.ThrowsAsync<InvalidDataException>(() => validator.ValidateAsync(null, CancellationToken.None));
        }

        [Fact]  
        public async Task ValidateAsync_EmptyFile_ThrowsInvalidDataException()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            await Assert.ThrowsAsync<InvalidDataException>(() => validator.ValidateAsync(fileMock.Object, CancellationToken.None));
        }   

        [Fact]
        public async Task ValidateAsync_ValidCsvFile_DoesNotThrow()
        {
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            await validator.ValidateAsync(fileMock.Object, CancellationToken.None);
        }
    }
}
