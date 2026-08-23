using Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared;
using MediatR;
using System.Text.Json;

namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.ListSalesReports
{
    /// <summary>
    /// Reads the sales processing log file and returns the most recent entries.
    /// </summary>
    public class ListSalesReportsHandler : IRequestHandler<ListSalesReportsQuery, SalesReportResponse>
    {
        private readonly ILogger<ListSalesReportsHandler> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ListSalesReportsHandler(ILogger<ListSalesReportsHandler> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Returns the total number of processed files and the most recent
        /// <paramref name="request"/>.Limit entries, newest first.
        /// </summary>
        public async Task<SalesReportResponse> Handle(ListSalesReportsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling ListSalesReportsQuery with limit: {Limit}", request.Limit);

            var logFilePath = SalesReportLogFile.GetPath(_webHostEnvironment);

            if (!File.Exists(logFilePath))
            {
                return new SalesReportResponse
                {
                    TotalFilesProcessed = 0,
                    Files = new List<SalesFileReport>()
                };
            }

            var lines = await File.ReadAllLinesAsync(logFilePath, cancellationToken);
            var totalCount = lines.Count(l => !string.IsNullOrWhiteSpace(l));

            var entries = lines
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => JsonSerializer.Deserialize<SalesFileReport>(l))
                .OrderByDescending(e => e!.ProcessedAt)
                .Take(request.Limit)
                .ToList();

            return new SalesReportResponse
            {
                TotalFilesProcessed = totalCount,
                Files = entries!
            };
        }
    }
}
