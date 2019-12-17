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
    public class NewsController : ControllerBase
    {
        private readonly ILogger<NewsController> _logger;
        private readonly INewsApiManager _newsApiManager;

        public NewsController(ILogger<NewsController> logger, INewsApiManager newsApiManager)
        {
            _logger = logger;
            _newsApiManager = newsApiManager;
        }
     
        [HttpGet("beststories/{total}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBestStoriesAsync(int total)
        {
            try
            {
                if (total <= 0)
                {
                    return BadRequest();
                }
                var result = await _newsApiManager.GetBestStoriesAsync(total);

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