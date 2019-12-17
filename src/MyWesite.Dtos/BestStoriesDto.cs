using System.Collections.Generic;

namespace MyWebsite.Dtos
{
    public class BestStoriesDto
    {
        public IList<StoryDto> Stories { get; set; } = new List<StoryDto>();
    }
}