using Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared;

namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.ListSalesReports
{
    /// <summary>
    /// Response payload for <c>GET /api/sales/reports</c>.
    /// </summary>
    public class SalesReportResponse
    {
        /// <summary>The total number of files ever recorded, regardless of the page size returned.</summary>
        public int TotalFilesProcessed { get; init; }

        /// <summary>The most recent processed-file entries, up to the requested limit.</summary>
        public List<SalesFileReport> Files { get; init; } = new();
    }
}
