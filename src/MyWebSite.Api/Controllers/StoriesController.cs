using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyWebsite.Business;
using System;
using System.Threading.Tasks;

namespace MyWesite.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly ILogger<StoriesController> _logger;
        private readonly IStoriesManager _storiesManager;

        public StoriesController(ILogger<StoriesController> logger, IStoriesManager storiesManager)
        {
            _logger = logger;
            _storiesManager = storiesManager;
        }            

        [HttpGet("beststories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBestStoriesAsync()
        {
            try
            {
                var result = await _storiesManager.GetBestStoriesAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetBestStories got an error.", ex);

                // should return a message and status code related with error type.
                return null;
            }
        }
    }
}