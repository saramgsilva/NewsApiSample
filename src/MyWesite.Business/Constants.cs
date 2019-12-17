namespace MyWebsite.Business
{
    public static class Constants
    {
        public static string WebClientName { get; private set; } = "newsapi";

        public static string IdsUrl { get; private set; } = "https://hacker-news.firebaseio.com/v0/beststories.json";
        public static string StoryDetailsUrl { get; private set; } = "https://hacker-news.firebaseio.com/v0/item";
    }
}
