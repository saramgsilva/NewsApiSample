using MyWebsite.Dtos;
using MyWebsite.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyWebsite.Business
{
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
}
