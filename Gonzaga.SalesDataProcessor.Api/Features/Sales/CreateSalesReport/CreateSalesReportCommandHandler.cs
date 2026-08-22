using Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared;
using MediatR;
using System.Text.Json;

namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.CreateSalesReport
{
    /// <summary>
    /// Appends a processed-file log entry to the sales processing log file.
    /// </summary>
    public class CreateSalesReportCommandHandler : IRequestHandler<CreateSalesReportCommand>
    {
        // A plain lock is enough here: the write is a tiny, infrequent line append,
        // so blocking briefly is simpler than juggling SemaphoreSlim's WaitAsync/Release.
        // Static because handlers are resolved per-request but all requests share one log file.
        private static readonly object WriteLock = new();

        private readonly IWebHostEnvironment _webHostEnvironment;

        public CreateSalesReportCommandHandler(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Serializes <paramref name="request"/>'s log entry and appends it as a new line
        /// to the sales processing log file, creating the file/directory if needed.
        /// </summary>
        public Task Handle(CreateSalesReportCommand request, CancellationToken cancellationToken)
        {
            var logFilePath = SalesReportLogFile.GetPath(_webHostEnvironment);
            var line = JsonSerializer.Serialize(request.LogEntry);

            lock (WriteLock)
            {
                File.AppendAllText(logFilePath, line + Environment.NewLine);
            }

            return Task.CompletedTask;
        }
    }
}
