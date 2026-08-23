using Asp.Versioning.Builder;
using Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared;
using MediatR;

namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.ProcessSalesReport
{
    /// <summary>
    /// Maps <c>POST /api/sales/process</c>.
    /// </summary>
    public static class ProcessSalesReportEndpoint
    {
        /// <summary>
        /// Registers the process-sales-file route, dispatching a
        /// <see cref="ProcessSalesReportCommand"/> for the uploaded file.
        /// </summary>
        public static void MapProcessSalesReport(this IEndpointRouteBuilder app, ApiVersionSet versionSet)
        {
            app.MapPost("/api/sales/process", async (IFormFile file, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new ProcessSalesReportCommand
                {
                    File = file
                };
                var result = await sender.Send(command, cancellationToken);
                return Results.Ok(result);
            })
            .WithName("ProcessSalesReport")
            .WithApiVersionSet(versionSet)
            .MapToApiVersion(1)
            .WithSummary("Processes a sales report file and returns the processing result.")
            .Produces<SalesFileReport>(StatusCodes.Status200OK)
            .DisableAntiforgery();
        }
    }
}
