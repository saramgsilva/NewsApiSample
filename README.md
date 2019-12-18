# News API Sample

Repository with an ASP.Net Core 2.2 sample based in a News API.


# Table of Contents

1. [The source code](#the-source-code)
2. [How run the project](#how-run-the-project)
3. [The result ](#the-result )


## The source code

The example provides an endpoint that consume the News API

In the **Startup.cs file** you can find important configurations:

        /// <summary>
        /// Configures the services. This method gets called by the runtime. 
        /// Use this method to add services to the container.
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // register web client using retry policy 
            services.AddHttpClient(Constants.WebClientName)
                    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                    .AddPolicyHandler(GetRetryPolicy());

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // add memory cache
            services.AddMemoryCache();

            // register interfaces
            services.AddScoped<INewsApiManager, NewsApiManager>();
            services.AddScoped<IStoriesManager, StoriesManager>();
        }


        /// <summary>
        /// Gets the retry policy.
        /// </summary>
        /// <returns></returns>
        private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                // HttpRequestException, 5XX and 408
                .HandleTransientHttpError()
                // 404
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                // Retry two times after delay
                .WaitAndRetryAsync(2, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }



The **StoriesController.cs** file defines the endpoint **"api/stories/beststories/"** which will consume the News APIs


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

The project uses different layers, and the controller will use the business layer using the **StoriesManager class**:


        /// <summary>
        /// Defines the Stories Manager.
        /// </summary>
        /// <seealso cref="MyWebsite.Business.IStoriesManager" />
        public class StoriesManager : IStoriesManager
        {
            private readonly IHttpClientFactory _clientFactory;

            /// <summary>
            /// Initializes a new instance of the <see cref="StoriesManager"/> class.
            /// </summary>
            /// <param name="clientFactory">The client factory.</param>
            public StoriesManager(IHttpClientFactory clientFactory)
            {
                _clientFactory = clientFactory;
            }

            /// <summary>
            /// Gets the 20 best stories asynchronous.
            /// </summary>
            /// <returns>The best stories dto</returns>
            public async Task<BestStoriesDto> GetBestStoriesAsync()
            {
                var bestStoriesDto = new BestStoriesDto();
                var storiesDto = new List<StoryDto>();

                var client = _clientFactory.CreateClient(Constants.WebClientName);
                var response1 = await client.GetAsync(Constants.IdsUrl);

                response1.EnsureSuccessStatusCode();
                var responseBody1 = await response1.Content.ReadAsStringAsync();
                var bestStories = JsonConvert.DeserializeObject<BestStories>(responseBody1);

                // take 20 stories
                var stories = bestStories.Take(20);

                // get story details
                foreach (var item in stories)
                {
                    StoryDto storyDto = await GetStoryDetails(client, item);

                    storiesDto.Add(storyDto);
                }

                // apply order in a descending way (score)
                storiesDto = storiesDto.OrderByDescending(i => i.Score).ToList();
                bestStoriesDto.AddRange(storiesDto);

                return bestStoriesDto;
            }

            /// <summary>
            /// Gets the story details.
            /// </summary>
            /// <param name="client">The client.</param>
            /// <param name="item">The item.</param>
            /// <returns>The Story Dto</returns>
            private static async Task<StoryDto> GetStoryDetails(HttpClient client, int item)
            {
                // get story by id
                var url = $"{Constants.StoryDetailsUrl}/{item}.json";

                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var story = JsonConvert.DeserializeObject<Story>(responseBody);

                var storyDto = new StoryDto();

                // convert data
                storyDto.CommentCount = story.Descendants;
                storyDto.PostedBy = story.By;
                storyDto.Score = story.Score;
                storyDto.Time = new DateTime(story.Time);
                storyDto.Title = story.Title;
                storyDto.Uri = story.Url;

                return storyDto;
            }
        }


**Note**:

The **NewsController class** uses **NewsApiManager** which has a simple implementation using Webclient - it does not 
handler communs errors, when a call is done, and the controller does not uses cache.



## How run the project

To run the project, you should:

* Open the **src folder** and then open the **MyWebSite solution**.
* Select the **MyWebSite.Api project** and then **Set as StartUp Project**

as showed in the following image:


![Architecture diagram](images/img1.png)

* After it, you can run the project **pressing F5** and selecting **IIS Express**.



## The result 


The enpoint **https://localhost:44387/api/stories/beststories/** should be showed, because it was configured in launchSettings.json file.

The browser should show the following output:


    // 20191218144703
    // https://localhost:44387/api/stories/beststories/

    [
      {
        "title": "Nebraska farmers vote overwhelmingly for Right to Repair",
        "uri": "https://uspirg.org/blogs/blog/usp/nebraska-farmers-vote-overwhelmingly-right-repair",
        "postedBy": "howard941",
        "time": "0001-01-01T00:02:37.6521229",
        "score": 1528,
        "commentCount": 419
      },
      {
        "title": "JetBrains: $270M revenue, 405K paying users, $0 raised",
        "uri": "https://twitter.com/chetanp/status/1205907182396395525",
        "postedBy": "matt2000",
        "time": "0001-01-01T00:02:37.6425141",
        "score": 1157,
        "commentCount": 653
      },
      {
        "title": "SQL Murder Mystery",
        "uri": "https://mystery.knightlab.com/",
        "postedBy": "kickscondor",
        "time": "0001-01-01T00:02:37.6456703",
        "score": 813,
        "commentCount": 79
      },
      {
        "title": "Google Brass Set 2023 as Deadline to Beat Amazon, Microsoft in Cloud",
        "uri": "https://www.theinformation.com/articles/google-brass-set-2023-as-deadline-to-beat-amazon-microsoft-in-cloud?pu=hackernewsyw3xln&utm_source=hackernews&utm_medium=unlock",
        "postedBy": "devhwrng",
        "time": "0001-01-01T00:02:37.660059",
        "score": 745,
        "commentCount": 762
      },
      {
        "title": "ICANN Delays .ORG Sale Approval",
        "uri": "https://www.icann.org/news/blog/org-update",
        "postedBy": "watchdogtimer",
        "time": "0001-01-01T00:02:37.6458084",
        "score": 721,
        "commentCount": 86
      },
      {
        "title": "Engineer says Google fired her for notifying co-workers of right to organize",
        "uri": "https://www.nbcnews.com/news/all/security-engineer-says-google-fired-her-trying-notify-co-workers-n1103031",
        "postedBy": "danso",
        "time": "0001-01-01T00:02:37.6592268",
        "score": 712,
        "commentCount": 676
      },
      {
        "title": "Over 100 PBS local stations start streaming on YouTube TV",
        "uri": "https://techcrunch.com/2019/12/17/over-100-pbs-local-stations-start-streaming-today-on-youtube-tv/",
        "postedBy": "samaysharma",
        "time": "0001-01-01T00:02:37.661506",
        "score": 566,
        "commentCount": 133
      },
      {
        "title": "Google is not a search engine, but an ad engine",
        "uri": "https://twitter.com/dhh/status/1205582897593430017",
        "postedBy": "jlelse",
        "time": "0001-01-01T00:02:37.6415769",
        "score": 542,
        "commentCount": 284
      },
      {
        "title": "Calm Technology",
        "uri": "https://calmtech.com/",
        "postedBy": "brundolf",
        "time": "0001-01-01T00:02:37.6453062",
        "score": 450,
        "commentCount": 154
      },
      {
        "title": "The Essential Guide to Electronics in Shenzhen (2016) [pdf]",
        "uri": "https://bunniefoo.com/bunnie/essential/essential-guide-shenzhen-web.pdf",
        "postedBy": "BerislavLopac",
        "time": "0001-01-01T00:02:37.6610956",
        "score": 438,
        "commentCount": 175
      },
      {
        "title": "Kansas City is first major city in U.S. to offer no-cost public transportation",
        "uri": "https://www.citylab.com/transportation/2019/12/free-transit-how-much-cost-kansas-city-bus-streetcar-fare/603397/",
        "postedBy": "jonbaer",
        "time": "0001-01-01T00:02:37.6529671",
        "score": 434,
        "commentCount": 372
      },
      {
        "title": "WebAssembly becomes a W3C Recommendation",
        "uri": "https://www.w3.org/2019/12/pressrelease-wasm-rec.html.en",
        "postedBy": "galaxyLogic",
        "time": "0001-01-01T00:02:37.6508871",
        "score": 432,
        "commentCount": 238
      },
      {
        "title": "23andMe to share customer gene data with GlaxoSmithKline for $300M",
        "uri": "https://www.tomsguide.com/us/23andme-gsk-dna-data-deal,news-27685.html",
        "postedBy": "Taurenking",
        "time": "0001-01-01T00:02:37.6597371",
        "score": 424,
        "commentCount": 263
      },
      {
        "title": "You might literally be buying trash on Amazon",
        "uri": "https://www.wsj.com/articles/you-might-be-buying-trash-on-amazonliterally-11576599910",
        "postedBy": "mudil",
        "time": "0001-01-01T00:02:37.6600369",
        "score": 403,
        "commentCount": 416
      },
      {
        "title": "#include &lt;/etc/shadow&gt;",
        "uri": "https://blog.hboeck.de/archives/898-include-etcshadow.html",
        "postedBy": "pcr910303",
        "time": "0001-01-01T00:02:37.655377",
        "score": 394,
        "commentCount": 114
      },
      {
        "title": "Google claims copyright on employee side projects",
        "uri": "https://twitter.com/marcan42/status/1207234468928356352",
        "postedBy": "zoobab",
        "time": "0001-01-01T00:02:37.6663315",
        "score": 386,
        "commentCount": 184
      },
      {
        "title": "An opinionated approach to GNU Make",
        "uri": "https://tech.davis-hansson.com/p/make/",
        "postedBy": "DarkCrusader2",
        "time": "0001-01-01T00:02:37.6583628",
        "score": 383,
        "commentCount": 189
      },
      {
        "title": "Story of Mattermost: Open-Sourced Competitor to Slack",
        "uri": "https://breakoutstartups.substack.com/p/breakout-startups-23-mattermost",
        "postedBy": "ankitkumar98",
        "time": "0001-01-01T00:02:37.6603294",
        "score": 372,
        "commentCount": 215
      },
      {
        "title": "LogMeIn Acquired by Private Equity",
        "uri": "https://techcrunch.com/2019/12/17/logmein-agrees-to-be-acquired-by-francisco-partners-and-evergreen-for-4-3b/",
        "postedBy": "AznHisoka",
        "time": "0001-01-01T00:02:37.6593738",
        "score": 360,
        "commentCount": 277
      },
      {
        "title": "Boeing to Suspend 737 Max Production in January",
        "uri": "https://www.wsj.com/articles/boeing-to-suspend-737-max-production-in-january-11576532032",
        "postedBy": "JumpCrisscross",
        "time": "0001-01-01T00:02:37.6534084",
        "score": 331,
        "commentCount": 257
      }
    ]