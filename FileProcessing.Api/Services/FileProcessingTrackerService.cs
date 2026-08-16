using FileProcessing.Api.Models;
using System.Text.Json;

namespace FileProcessing.Api.Services
{
    /// <summary>
    /// Tracks the processing of files and provides reports on processed files.
    /// </summary>
    public class FileProcessingTrackerService : IFileProcessingTrackerService
    {
        private readonly string _logFilePath;
        private readonly SemaphoreSlim _semaphoreSlim = new (1, 1);

        public FileProcessingTrackerService(IWebHostEnvironment webHostEnvironment)
        {
            var dataDirectory = Path.Combine(webHostEnvironment.ContentRootPath, "Data");
            Directory.CreateDirectory(dataDirectory);
            _logFilePath = Path.Combine(dataDirectory, "file-processing.log");
        }
        public async Task<FileProcessingReport> GetReportsAsync(int limit, CancellationToken cancellationToken)
        {
            if (!File.Exists(_logFilePath))
            {
                return new FileProcessingReport
                {
                    TotalFilesProcessed = 0,
                    Files = new List<FileProcessingLogEntry>()
                };
            }

            var lines =  await File.ReadAllLinesAsync(_logFilePath, cancellationToken);
            var totalCount = lines.Count(l => !string.IsNullOrWhiteSpace(l));

            var entries = lines
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => JsonSerializer.Deserialize<FileProcessingLogEntry>(l))
                .OrderByDescending(e => e.ProcessedAt)
                .Take(limit)
                .ToList();

            return new FileProcessingReport
            {
                TotalFilesProcessed = totalCount,
                Files = entries
            };
        }

        public async Task RecordAsync(FileProcessingLogEntry logEntry, CancellationToken cancellationToken)
        {
            var line = JsonSerializer.Serialize(logEntry);
            await _semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                await File.AppendAllTextAsync(_logFilePath, line + Environment.NewLine, cancellationToken);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
