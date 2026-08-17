using CsvHelper;

namespace FileProcessing.Api.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (CsvHelperException ex)
            {
                _logger.LogError(ex, "Rejected request due to malformed CSV: {Path}", context.Request.Path);
                await WriteError(context, StatusCodes.Status400BadRequest, "The CSV file is malformed or contains invalid data.");
            }
            catch (InvalidDataException ex)
            {
                _logger.LogError(ex, "Rejected request: {Path}", context.Request.Path);
                await WriteError(context, StatusCodes.Status400BadRequest, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "Unauthorized access attempt: {Path}", context.Request.Path);
                await WriteError(context, StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception for {Path}", context.Request.Path);
                await WriteError(context, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        private static async Task WriteError(HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = message });
        }
    }
}