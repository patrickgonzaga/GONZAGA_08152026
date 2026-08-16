namespace FileProcessing.Api.Models
{
    public class FileProcessingLogEntry
    {
        public string FileName { get; init; } = string.Empty;
        public int RecordCount { get; init; }
        public decimal AverageAmount { get; init; }
        public DateTimeOffset ProcessedAt { get; init; }
        public long Duration { get; init; }
    }
}
