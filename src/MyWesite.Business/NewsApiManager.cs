using MyWebsite.Dtos;
using MyWebsite.Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MyWebsite.Business
{
    public class NewsApiManager : INewsApiManager
    {
        public async Task<BestStoriesDto> GetBestStoriesAsync(int total)
        {
            var bestStoriesDto = new BestStoriesDto();

            var storiesDto = new List<StoryDto>();
            using (var client = new HttpClient())
            {
                // Get ids 
                HttpResponseMessage response = await client.GetAsync(Constants.IdsUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                BestStories bestStories = JsonConvert.DeserializeObject<BestStories>(responseBody);


                // take then
                var stories = bestStories.Take(total);


                // get story details
                foreach (var item in stories)
                {
                    StoryDto storyDto = await GetStoryDetails(client, item);

                    storiesDto.Add(storyDto);
                }
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