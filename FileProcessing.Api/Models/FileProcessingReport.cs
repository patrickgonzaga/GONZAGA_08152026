namespace FileProcessing.Api.Models
{
    public class FileProcessingReport
    {
        public int TotalFilesProcessed { get; init; }
        public List<FileProcessingLogEntry> Files { get; init; } = new();
    }
}
