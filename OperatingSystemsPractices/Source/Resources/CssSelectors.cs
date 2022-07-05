namespace OperatingSystemsPractices.Source.Resources
{
    public static class CssSelectors
    {
        public static string PostIdentifier { get { return "div[post_view_hash]:not([data-ad])"; } }
        public static string ShowMorePosts { get { return "button[id='show_more_link']"; } }
        public static string ShowFullPost { get { return "a[class='wall_post_more']"; } }
        public static string FeedRow { get { return "div[class*='feed_row']"; } }
        public static string FeedRows { get { return "div[id='feed_rows']"; } }
        public static string PostViewHash { get { return "div[post_view_hash]"; } }
        public static string WallPostText { get { return "div[class*='wall_post_text']"; } }
        public static string WallPost { get { return "div[class='wall_text']"; } }
        public static string Photo { get { return "a[aria-label='фотография']"; } }
        public static string Href { get { return "a[href]"; } }
    }
}
