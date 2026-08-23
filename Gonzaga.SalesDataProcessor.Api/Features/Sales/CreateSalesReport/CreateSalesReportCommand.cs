using Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared;
using MediatR;

namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.CreateSalesReport
{
    /// <summary>
    /// Persists a <see cref="SalesFileReport"/> to the sales processing log.
    /// </summary>
    public record CreateSalesReportCommand(SalesFileReport LogEntry) : IRequest;
}
