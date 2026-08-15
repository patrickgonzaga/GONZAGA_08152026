using FileProcessing.Api.Models;

namespace FileProcessing.Api.Services
{
    public interface IFileProcessingService
    {
        Task<FileProcessingResult> ProcessFileAsync(IFormFile file, CancellationToken cancellationToken);
    }
}
