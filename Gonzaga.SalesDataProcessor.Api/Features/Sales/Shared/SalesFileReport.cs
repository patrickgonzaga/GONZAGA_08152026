namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared
{
    /// <summary>
    /// A report of a single processed sales file. Shared across slices: it's the
    /// result of ProcessSalesReport, the entry CreateSalesReport persists, and an
    /// item within ListSalesReports' response — not solely an HTTP response shape.
    /// </summary>
    public class SalesFileReport
    {
        public string FileName { get; init; } = string.Empty;
        public int RecordCount { get; init; }
        public decimal AverageAmount { get; init; }
        public DateTimeOffset ProcessedAt { get; init; }
        public long Duration { get; init; }
    }
}
