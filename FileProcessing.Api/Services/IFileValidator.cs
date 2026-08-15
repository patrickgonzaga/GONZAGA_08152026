namespace FileProcessing.Api.Services
{
    public interface IFileValidator
    {
        Task ValidateAsync(IFormFile? file, CancellationToken cancellationToken);
    }
}
