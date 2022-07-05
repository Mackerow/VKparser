namespace OperatingSystemsPractices.Source.ErrorMessages
{
    public static class Selenium
    {
        public static string Default { get { return "An error occurred!"; } }
        public static string OpenChrome { get { return "Chrome must be closed before starting!"; } }
        public static string NoPosts { get { return "There is no posts to parse!"; } }
        public static string NullFeedRowsElement { get { return "Unable to find 'feed_rows' element!"; } }
        public static string NullFeedRowElements { get { return "Unable to find 'feed_row ' elements!"; } }
    }
}
