namespace MyWebsite.Business
{
    /// <summary>Defines the constants from project.</summary>
    public static class Constants
    {
        /// <summary>Gets the name of the web client.</summary>
        /// <value>The name of the web client.</value>
        public static string WebClientName { get; private set; } = "newsapi";

        /// <summary>Gets the cache stories key.</summary>
        /// <value>The cache stories key.</value>
        public static string CacheStoriesKey { get; private set; } = "beststories";

        /// <summary>Gets the ids URL.</summary>
        /// <value>The URL.</value>
        public static string IdsUrl { get; private set; } = "https://hacker-news.firebaseio.com/v0/beststories.json";

        /// <summary>Gets the story details URL.</summary>
        /// <value>The URL.</value>
        public static string StoryDetailsUrl { get; private set; } = "https://hacker-news.firebaseio.com/v0/item";
    }
}
