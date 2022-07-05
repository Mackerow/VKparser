using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;
using OperatingSystemsPractices.Source.Vk;
using OperatingSystemsPractices.Source.Vk.WebElements;
using OperatingSystemsPractices.Source.WebElements;
using OperatingSystemsPractices.Source.ErrorMessages;
using OperatingSystemsPractices.Source.Resources;

namespace OperatingSystemsPractices.Source.Pages.VkParser
{
    public partial class ParsePostsPage : Page
    {
        ChromeDriver chromeDriver;
        int numberOfPosts;
        int numberOfWebElements;
        int numberOfParsedWebElements;
        int numberOfThreads = 8;
        List<Post> posts;

        public ParsePostsPage(ChromeDriver chromeDriver, int numberOfPosts)
        {
            InitializeComponent();

            this.chromeDriver = chromeDriver;
            this.numberOfPosts = numberOfPosts;

            Thread thread = new Thread(() => Start()) { IsBackground = true };
            thread.Start();
        }

        private void Start()
        {
            IWebElement feedRowsElement = FindElement.ByCssSelector(CssSelectors.FeedRows, chromeDriver);
            if (feedRowsElement == null) { Error(Selenium.NullFeedRowsElement); return; }

            List<IWebElement> feedRowElements = FindElements.ByCssSelector(CssSelectors.FeedRow, feedRowsElement);
            if (feedRowElements == null) { Error(Selenium.NullFeedRowElements); return; }

            posts = GetPostsAsync(feedRowElements);

            if (!posts.Any())
            {
                Error(Selenium.NoPosts);
                return;
            }

            NextStep();
        }

        private List<Post> GetPostsAsync(List<IWebElement> feedRowElements)
        {
            UpdateProgressBar(0, 1);

            numberOfWebElements = feedRowElements.Count();
            numberOfParsedWebElements = 0;

            List<IWebElement>[] webElementsForEachThread = SplitWebElementsForThreads(feedRowElements);
            List<Post>[] postsForEachThread = new List<Post>[numberOfThreads];

            Thread[] threads = CreateThreads(webElementsForEachThread, postsForEachThread);
            foreach (Thread thread in threads) thread.Start();
            foreach (Thread thread in threads) thread.Join();

            UpdateProgressBar(1, 1);

            UpdateStatus("Merging posts...");
            UpdateProgressBar(0, 1);

            List<Post> posts = new List<Post>();

            int numberOfMergedPosts = 0;
            foreach (List<Post> postsInThreads in postsForEachThread)
            {
                posts = posts.Concat(postsInThreads).ToList();
                numberOfMergedPosts++;
                UpdateProgressBar(numberOfMergedPosts, numberOfThreads);
            }

            return posts.Count() > numberOfPosts ? posts.GetRange(0, numberOfPosts) : posts;
        }

        private List<IWebElement>[] SplitWebElementsForThreads(List<IWebElement> webElements)
        {
            List<IWebElement>[] webElementsForEachThread = new List<IWebElement>[numberOfThreads];

            int numberOfWebElementsForEachThread = webElements.Count / numberOfThreads;
            int numberOfAdditionalWebElements = webElements.Count % numberOfThreads;

            int[] numberOfPostsForEachThread = new int[numberOfThreads];
            for (int i = 0; i < numberOfThreads; i++)
            {
                numberOfPostsForEachThread[i] = numberOfWebElementsForEachThread;
                if (numberOfAdditionalWebElements > 0)
                {
                    numberOfPostsForEachThread[i]++;
                    numberOfAdditionalWebElements--;
                }
            }

            for (int threadIndex = 0, startFrom = 0; threadIndex < numberOfThreads; threadIndex++)
            {
                webElementsForEachThread[threadIndex] = webElements.GetRange(startFrom, numberOfPostsForEachThread[threadIndex]);
                startFrom += numberOfPostsForEachThread[threadIndex];
            }

            return webElementsForEachThread;
        }

        private Thread[] CreateThreads(List<IWebElement>[] webElementsForEachThread, List<Post>[] postsForEachThread)
        {
            Thread[] threads = new Thread[numberOfThreads];

            for (int threadIndex = 0; threadIndex < numberOfThreads; threadIndex++)
            {
                int setThreadIndex = threadIndex;
                threads[threadIndex] = new Thread(() =>
                {
                    postsForEachThread[setThreadIndex] = GetPostsSynchronously(webElementsForEachThread[setThreadIndex]);
                })
                { IsBackground = true };
            }

            return threads;
        }

        private List<Post> GetPostsSynchronously(List<IWebElement> feedRows)
        {
            List<Post> posts = new List<Post>();

            foreach (var webElement in feedRows)
            {
                Post post = FeedRow.GetPost(chromeDriver, webElement);
                if (post != null) posts.Add(post);
                numberOfParsedWebElements++;
                UpdateProgressBar(numberOfParsedWebElements, numberOfWebElements);
            }

            return posts;
        }

        private void NextStep()
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new CloseChrome(chromeDriver, posts)));
        }

        private void Error(string message)
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new ErrorPage(message)));
        }

        private void UpdateProgressBar(int numberOfParsedPosts, int numberOfPosts)
        {
            ProgressBar.Dispatcher.Invoke(() => ProgressBar.Value = numberOfParsedPosts);
            ProgressBar.Dispatcher.Invoke(() => ProgressBar.Maximum = numberOfPosts);
        }

        private void UpdateStatus(string text)
        {
            StatusTextBlock.Dispatcher.Invoke(() => StatusTextBlock.Text = text);
        }
    }
}
