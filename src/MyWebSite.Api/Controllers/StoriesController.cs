using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MyWebsite.Business;
using MyWebsite.Dtos;
using System;
using System.Threading.Tasks;

namespace MyWesite.Api.Controllers
{
    /// <summary>
    /// Define the Stories controller class.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
    [ApiController]
    [Route("api/[controller]")]
    public class StoriesController : ControllerBase
    {
        private readonly ILogger<StoriesController> _logger;
        private readonly IStoriesManager _storiesManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoriesController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="storiesManager">The stories manager.</param>
        public StoriesController(ILogger<StoriesController> logger, IStoriesManager storiesManager)
        {
            _logger = logger;
            _storiesManager = storiesManager;
        }

        /// <summary>
        /// Gets the 20 best stories with cache asynchronous.
        /// </summary>
        /// <param name="cache">The memory cache.</param>
        /// <returns>The 20 Best Stories.</returns>
        [HttpGet("beststories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetBestStoriesWithCacheAsync([FromServices]IMemoryCache cache)
        {
            try
            {
                BestStoriesDto data = null; ;

                // get data in cache
                if (cache.TryGetValue<BestStoriesDto>(Constants.CacheStoriesKey, out data))
                {
                    return Ok(data);
                }
                else
                {
                    // get data from News API server
                    data = await _storiesManager.GetBestStoriesAsync();

                    // save data in cache
                    cache.Set(Constants.CacheStoriesKey, data, TimeSpan.FromMinutes(5));

                    // return data using status code 200
                    return Ok(data);
                }             
            }
            catch (Exception ex)
            {
                // logs the error
                _logger.LogError("GetBestStories got an error.", ex);

                // should return a message and status code related with error type.
                return null;
            }
        }
    }
}