using CsvHelper;
using Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared;
using Gonzaga.SalesDataProcessor.Api.Features.Sales.CreateSalesReport;
using MediatR;
using System.Diagnostics;

namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.ProcessSalesReport
{
    /// <summary>
    /// Parses an uploaded CSV file of sales records, computes summary statistics,
    /// and dispatches a <see cref="CreateSalesReportCommand"/> to persist the result.
    /// </summary>
    public class ProcessSalesReportCommandHandler : IRequestHandler<ProcessSalesReportCommand, SalesFileReport>
    {
        private readonly ILogger<ProcessSalesReportCommandHandler> _logger;
        private readonly ISender _sender;

        public ProcessSalesReportCommandHandler(ILogger<ProcessSalesReportCommandHandler> logger, ISender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        /// <summary>
        /// Reads and parses the uploaded CSV, computes record count and average amount,
        /// records the result via <see cref="CreateSalesReportCommand"/>, and returns it.
        /// </summary>
        /// <exception cref="InvalidDataException">The file has no data rows.</exception>
        public async Task<SalesFileReport> Handle(ProcessSalesReportCommand request, CancellationToken cancellationToken)
        {
            var stopWatch = Stopwatch.StartNew();

            await using var stream = request.File.OpenReadStream();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
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

            _logger.LogInformation("Processed {RecordCount} records from file {FileName} in {ElapsedMilliseconds} ms. Average Amount: {AverageAmount}",
                records.Count, request.File.FileName, stopWatch.ElapsedMilliseconds, averageAmount);

            // Process the file and return the result
            var result = new SalesFileReport
            {
                FileName = request.File.FileName,
                RecordCount = records.Count,
                AverageAmount = averageAmount,
                Duration = stopWatch.ElapsedMilliseconds,
                ProcessedAt = DateTimeOffset.UtcNow
            };

            await _sender.Send(new CreateSalesReportCommand(result), cancellationToken);

            return result;
        }
    }
}
