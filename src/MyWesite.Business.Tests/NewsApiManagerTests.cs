using MyWebsite.Business;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MyWesite.Business.Tests
{
    [TestFixture]
    public class NewsApiManagerTests
    {
        private NewsApiManager _newsManager;

        [SetUp]
        public void Setup()
        {
            _newsManager = new NewsApiManager();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(20)]
        public async Task Test_BestStories(int value)
        {
            var result = await _newsManager.GetBestStoriesAsync(value);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == value);

            foreach (var story in result)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(story.PostedBy));
                Assert.IsTrue(story.Score >= 0);
                Assert.IsNotNull(story.Time);
                Assert.IsTrue(story.Time != new System.DateTime());
                Assert.IsTrue(!string.IsNullOrEmpty(story.Title));
                Assert.IsTrue(!string.IsNullOrEmpty(story.Uri));
                Assert.IsTrue(story.CommentCount >= 0);
            }
        }
    }
}