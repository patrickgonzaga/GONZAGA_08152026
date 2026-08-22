using Asp.Versioning;
using Asp.Versioning.Builder;

namespace Gonzaga.SalesDataProcessor.Api.Common.Endpoints
{
    /// <summary>
    /// Builds the shared <see cref="ApiVersionSet"/> used by all Minimal API endpoints.
    /// </summary>
    public static class ApiVersioning
    {
        /// <summary>
        /// Creates the API version set (currently v1) that endpoint mappings attach to
        /// via <c>WithApiVersionSet</c>.
        /// </summary>
        public static ApiVersionSet CreateVersionSet(IEndpointRouteBuilder app) =>
            app.NewApiVersionSet("Sales Data Processor API")
                .HasApiVersion(new ApiVersion(1, 0))
                .ReportApiVersions()
                .Build();
    }
}
