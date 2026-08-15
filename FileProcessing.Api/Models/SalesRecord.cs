namespace FileProcessing.Api.Models
{
    public class SalesRecord
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
