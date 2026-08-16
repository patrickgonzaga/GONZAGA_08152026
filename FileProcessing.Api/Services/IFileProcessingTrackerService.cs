using FileProcessing.Api.Models;

namespace FileProcessing.Api.Services
{
    public interface IFileProcessingTrackerService
    {
        /// <summary>
        /// Records a log entry for a processed file.
        /// </summary>
        /// <param name="logEntry"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RecordAsync(FileProcessingLogEntry logEntry, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a report of processed files, limited to the specified number of entries.
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<FileProcessingReport> GetReportsAsync(int limit, CancellationToken cancellationToken);
    }
}
