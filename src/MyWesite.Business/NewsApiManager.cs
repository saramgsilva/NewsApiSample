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
    /// <summary>
    /// Defines the News API Manager
    /// </summary>
    /// <seealso cref="MyWebsite.Business.INewsApiManager" />
    public class NewsApiManager : INewsApiManager
    {
        /// <summary>
        /// Gets the best stories asynchronous.
        /// </summary>
        /// <param name="total">The total.</param>
        /// <returns>The 20 best stories dto.</returns>
        public async Task<BestStoriesDto> GetBestStoriesAsync(int total)
        {
            var bestStoriesDto = new BestStoriesDto();

            var storiesDto = new List<StoryDto>();

            // create the webclient object
            using (var client = new HttpClient())
            {
                // Get ids 
                HttpResponseMessage response = await client.GetAsync(Constants.IdsUrl);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                BestStories bestStories = JsonConvert.DeserializeObject<BestStories>(responseBody);

                // take stories based on total
                var stories = bestStories.Take(total);
                
                // get story details
                foreach (var item in stories)
                {
                    StoryDto storyDto = await GetStoryDetails(client, item);

                    storiesDto.Add(storyDto);
                }
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