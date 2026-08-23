using Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared;
using MediatR;

namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.ProcessSalesReport
{
    /// <summary>
    /// Requests processing of an uploaded CSV file of sales records.
    /// </summary>
    public record ProcessSalesReportCommand : IRequest<SalesFileReport>
    {
        /// <summary>The uploaded CSV file to parse.</summary>
        public required IFormFile File { get; init; }
    }
}
