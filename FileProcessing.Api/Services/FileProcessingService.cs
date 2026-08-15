using FileProcessing.Api.Models;

namespace FileProcessing.Api.Services
{
    public class FileProcessingService : IFileProcessingService
    {
        private readonly IFileValidator _fileValidator;
        private readonly ILogger<FileProcessingService> _logger;

        public FileProcessingService(ILogger<FileProcessingService> logger, IFileValidator fileValidator)
        {
            _logger = logger;
            _fileValidator = fileValidator;
        }

        public async Task<FileProcessingResult> ProcessFileAsync(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                await _fileValidator.ValidateAsync(file, cancellationToken);
                _logger.LogInformation("File validation passed for file: {FileName}", file.FileName);

                // Process the csv file
                await using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);
                using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
                var records = new List<SalesRecord>();
                await foreach (var record in csv.GetRecordsAsync<SalesRecord>(cancellationToken))
                {
                    records.Add(record);
                }
                ;

                if (records.Count == 0)
                {
                    throw new InvalidDataException("The file contains no records.");
                }

                var averageAmount = records.Average(r => r.Amount);

                _logger.LogInformation("File processed successfully for file: {FileName}. Record count: {RecordCount}, Average amount: {AverageAmount}", file.FileName, records.Count, averageAmount);

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
