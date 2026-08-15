namespace FileProcessing.Api.Middlewares
{
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

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
            {
                _logger.LogWarning("API Key was not provided.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            var apiKey = _configuration.GetValue<string>("ApiKey");

            if (string.IsNullOrEmpty(apiKey) || !apiKey.Equals(extractedApiKey))
            {
                _logger.LogWarning("Unauthorized client attempted to access the API.");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }
            await next(context);
        }
    }
}
