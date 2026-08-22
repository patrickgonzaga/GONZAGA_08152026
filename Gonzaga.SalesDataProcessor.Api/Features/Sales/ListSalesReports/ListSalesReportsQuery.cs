using MediatR;

namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.ListSalesReports
{
    /// <summary>
    /// Requests the most recently processed sales files, up to <paramref name="Limit"/> entries.
    /// </summary>
    /// <param name="Limit">The maximum number of report entries to return.</param>
    public record ListSalesReportsQuery(int Limit) : IRequest<SalesReportResponse>;
}
