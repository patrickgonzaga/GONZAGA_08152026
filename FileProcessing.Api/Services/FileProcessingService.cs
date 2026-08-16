using FileProcessing.Api.Models;
using System.Diagnostics;

namespace FileProcessing.Api.Services
{
    /// <summary>
    /// Service responsible for processing uploaded files.
    /// </summary>
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IFileValidator _fileValidator;
        private readonly IFileProcessingTrackerService _fileProcessingTracker;
        private readonly ILogger<FileProcessingService> _logger;

        public FileProcessingService(ILogger<FileProcessingService> logger, IFileValidator fileValidator, IFileProcessingTrackerService fileProcessingTracker)
        {
            _logger = logger;
            _fileValidator = fileValidator;
            _fileProcessingTracker = fileProcessingTracker;
        }

        public async Task<FileProcessingResult> ProcessFileAsync(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                await _fileValidator.ValidateAsync(file, cancellationToken);
                _logger.LogInformation("File validation passed for file: {FileName}", file.FileName);

                var stopWatch = Stopwatch.StartNew();

                // Process the csv file
                await using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                var records = new List<SalesRecord>();

                await foreach (var record in csv.GetRecordsAsync<SalesRecord>(cancellationToken))
                {
                    records.Add(record);
                }

                if (records.Count == 0)
                {
                    throw new InvalidDataException("The file contains no records.");
                }

                var averageAmount = records.Average(r => r.Amount);

                stopWatch.Stop();

                await _fileProcessingTracker.RecordAsync(new FileProcessingLogEntry
                {
                    FileName = file.FileName,
                    RecordCount = records.Count,
                    AverageAmount = averageAmount,
                    ProcessedAt = DateTimeOffset.UtcNow,
                    Duration = stopWatch.ElapsedMilliseconds,
                }, cancellationToken);

                _logger.LogInformation("File processed successfully: {FileName}, Records: {RecordCount}, Average Amount: {AverageAmount}, Duration: {Duration}ms",
                    file.FileName, records.Count, averageAmount, stopWatch.ElapsedMilliseconds);

                return new FileProcessingResult
                {
                    FileName = file.FileName,
                    RecordCount = records.Count,
                    AverageAmount = averageAmount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the file.");
                throw;
            }
        }
    }
}
