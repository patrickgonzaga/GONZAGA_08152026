namespace FileProcessing.Api.Models
{
    public class FileProcessingResult
    {
        public string FileName { get; init; } = string.Empty;
        public int RecordCount { get; init; }
        public decimal AverageAmount { get; init; }
    }
}
