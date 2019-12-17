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

        [Test]
        public async Task Test_BestSoties_CountEquals1()
        {
            var result = await _newsManager.GetBestStoriesAsync(1);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Stories);
            Assert.IsTrue(result.Stories.Count == 1);
        }

        [Test]
        public async Task Test_BestSoties_CountEquals20()
        {
            var result = await _newsManager.GetBestStoriesAsync(20);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Stories);
            Assert.IsTrue(result.Stories.Count == 20);

            foreach (var story in result.Stories)
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