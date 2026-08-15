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
        protected readonly ILogger<FilesController> _logger;
        public FilesController(ILogger<FilesController> logger, IFileProcessingService fileProcessingService)
        {
            _logger = logger;
            _fileProcessingService = fileProcessingService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok();
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
