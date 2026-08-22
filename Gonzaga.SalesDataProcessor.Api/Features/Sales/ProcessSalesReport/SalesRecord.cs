namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.ProcessSalesReport
{
    /// <summary>
    /// A single row of an uploaded sales CSV file.
    /// </summary>
    public class SalesRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
