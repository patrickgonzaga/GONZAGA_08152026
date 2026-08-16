using FileProcessing.Api.Models;

namespace FileProcessing.Api.Services
{
    public interface IFileProcessingService
    {
        /// <summary>
        /// Processes the uploaded file and returns the result.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<FileProcessingResult> ProcessFileAsync(IFormFile file, CancellationToken cancellationToken);
    }
}
