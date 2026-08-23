namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.Shared
{
    /// <summary>
    /// Location of the append-only log file that backs the sales processing report.
    /// </summary>
    public static class SalesReportLogFile
    {
        private const string DirectoryName = "Data";
        private const string FileName = "sales-processing.log";

        /// <summary>
        /// Resolves the full path to the sales processing log file, creating its
        /// directory if needed. Used by both the write side (CreateSalesReport) and
        /// the read side (ListSalesReports) so the path is only ever built in one place.
        /// </summary>
        public static string GetPath(IWebHostEnvironment webHostEnvironment)
        {
            var dataDirectory = Path.Combine(webHostEnvironment.ContentRootPath, DirectoryName);
            Directory.CreateDirectory(dataDirectory);
            return Path.Combine(dataDirectory, FileName);
        }
    }
}
