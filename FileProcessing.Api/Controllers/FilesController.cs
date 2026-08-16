using Asp.Versioning;
using FileProcessing.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessing.Api.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/files")]
    public class FilesController : ControllerBase
    {
        private readonly IFileProcessingService _fileProcessingService;
        private readonly IFileProcessingTrackerService _fileProcessingTracker;
        protected readonly ILogger<FilesController> _logger;
        public FilesController(ILogger<FilesController> logger, IFileProcessingService fileProcessingService, IFileProcessingTrackerService fileProcessingTracker)
        {
            _logger = logger;
            _fileProcessingService = fileProcessingService;
            _fileProcessingTracker = fileProcessingTracker;
        }

        /// <summary>
        /// Retrieves a report of processed files, limited to the specified number of entries.
        /// </summary>
        /// <returns></returns>
        [HttpGet("reports")]
        public async Task<ActionResult> GetReports([FromQuery] int limit, CancellationToken cancellationToken)
        {
            var effectiveLimit = limit <= 0 ? 20 : limit;
            var report = await _fileProcessingTracker.GetReportsAsync(effectiveLimit, cancellationToken);
            return Ok(report);
        }

        /// <summary>
        /// Processes the uploaded CSV file and returns the result.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("process")]
        public async Task<ActionResult> Process(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _fileProcessingService.ProcessFileAsync(file, cancellationToken);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the file.");
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
