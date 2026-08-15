namespace FileProcessing.Api.Services
{
    public class FileValidator : IFileValidator
    {
        public async Task ValidateAsync(IFormFile? file, CancellationToken cancellationToken)
        {
            if (file == null)
            {
                throw new InvalidDataException("A file is required.");
            }

            if (file.Length == 0)
            {
                throw new InvalidDataException("The file is empty.");
            }

            if (!string.Equals(Path.GetExtension(file.FileName), ".csv", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException("The file must be a CSV file.");
            }

        }
    }
}
