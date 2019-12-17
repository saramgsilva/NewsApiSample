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
    public class StoriesManager : IStoriesManager
    {
        private readonly IHttpClientFactory _clientFactory;

        public StoriesManager(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<BestStoriesDto> GetBestStoriesAsync()
        {
            var bestStoriesDto = new BestStoriesDto();
            var storiesDto = new List<StoryDto>();

            var client = _clientFactory.CreateClient(Constants.WebClientName);
            var response1 = await client.GetAsync(Constants.IdsUrl);

            response1.EnsureSuccessStatusCode();
            var responseBody1 = await response1.Content.ReadAsStringAsync();
            var bestStories = JsonConvert.DeserializeObject<BestStories>(responseBody1);

            // take then
            var stories = bestStories.Take(20);

            // get story details
            foreach (var item in stories)
            {
                StoryDto storyDto = await GetStoryDetails(client, item);

                storiesDto.Add(storyDto);
            }

            storiesDto = storiesDto.OrderByDescending(i => i.Score).ToList();
            bestStoriesDto.Stories = storiesDto;
            return bestStoriesDto;
        }

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
