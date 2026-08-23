using Asp.Versioning.Builder;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.ListSalesReports
{
    /// <summary>
    /// Maps <c>GET /api/sales/reports</c>.
    /// </summary>
    public static class ListSalesReportsEndpoint
    {
        /// <summary>
        /// Registers the list-sales-reports route, dispatching a
        /// <see cref="ListSalesReportsQuery"/> for the requested page size.
        /// </summary>
        public static void MapListSalesReports(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
        {
            app.MapGet("/api/sales/reports", async ([FromQuery] int limit, ISender sender, CancellationToken cancellationToken) =>
            {
                var query = new ListSalesReportsQuery(limit <= 0 ? 20 : limit);
                var result = await sender.Send(query, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("ListSalesReports")
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1)
            .WithSummary("Lists all sales reports and their processing status.")
            .Produces<SalesReportResponse>(StatusCodes.Status200OK);
        }
    }
}
