namespace Gonzaga.SalesDataProcessor.Api.Common.Middlewares
{
    /// <summary>
    /// Rejects requests that don't present a valid <c>X-API-KEY</c> header matching
    /// the configured <c>ApiKey</c> setting.
    /// </summary>
    public sealed class ApiKeyMiddleware : IMiddleware
    {
        private const string ApiKeyHeaderName = "X-API-KEY";

        private readonly IConfiguration _configuration;
        private readonly ILogger<ApiKeyMiddleware> _logger;

        public ApiKeyMiddleware(IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Validates the API key header on the incoming request, throwing
        /// <see cref="UnauthorizedAccessException"/> if it's missing or incorrect.
        /// </summary>
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                _logger.LogError("API Key was not provided.");
                throw new UnauthorizedAccessException("API Key was not provided.");
            }

            var apiKey = _configuration.GetValue<string>("ApiKey");

            if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(extractedApiKey))
            {
                _logger.LogError("Unauthorized client attempted to access the API.");
                throw new UnauthorizedAccessException("Unauthorized client attempted to access the API.");
            }
            await next(context);
        }
    }
}
