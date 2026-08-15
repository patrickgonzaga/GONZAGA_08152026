using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessing.Api.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api")]
    public class FilesController : ControllerBase
    {
        protected readonly ILogger<FilesController> _logger;
        public FilesController(ILogger<FilesController> logger)
        {
            _logger = logger;
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
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Upload()
        {
            return Ok();
        }
    }
}
