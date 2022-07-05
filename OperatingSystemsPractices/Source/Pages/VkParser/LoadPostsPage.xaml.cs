using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Navigation;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OperatingSystemsPractices.Source.WebElements;
using OperatingSystemsPractices.Source.Resources;

namespace OperatingSystemsPractices.Source.Pages.VkParser
{
    public partial class LoadPostsPage : Page
    {
        ChromeDriver chromeDriver;
        int numberOfPosts;

        public LoadPostsPage(ChromeDriver chromeDriver, int numberOfPosts)
        {
            InitializeComponent();

            this.chromeDriver = chromeDriver;
            this.numberOfPosts = numberOfPosts;

            Thread thread = new Thread(() => Start()) { IsBackground = true };
            thread.Start();
        }

        private void Start()
        {
            UpdateProgressBar(0, 1);

            int numberOfLoads = numberOfPosts <= 7 ? 0 : numberOfPosts / 10 + (numberOfPosts % 10 != 0 ? (+1) : (+0));

            for (int loadNumber = 1; loadNumber <= numberOfLoads; loadNumber++)
            {
                if (!TryLoad()) break;
                UpdateProgressBar(loadNumber, numberOfLoads);
            }

            UpdateProgressBar(1, 1);

            List<IWebElement> loadedPosts = FindElements.ByCssSelector(CssSelectors.PostIdentifier, chromeDriver);
            if (loadedPosts == null || loadedPosts.Count == 0)
            {
                Error(ErrorMessages.Selenium.NoPosts);
                return;
            }
            if (loadedPosts.Count < numberOfPosts)
            {
               numberOfPosts = loadedPosts.Count;
            }

            NextStep();
        }

        private bool TryLoad()
        {
            while (true)
            {
                IWebElement showMorePosts = FindElement.ByCssSelector(CssSelectors.ShowMorePosts, chromeDriver);
                if (showMorePosts == null || showMorePosts.GetAttribute("style") == "display: none;")
                    return false;

                if (showMorePosts.Text == "")
                {
                    Thread.Sleep(50);
                    continue;
                }

                Element.MoveTo(chromeDriver, showMorePosts);
                return true;
            }           
        }

        private void NextStep()
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new ParsePostsPage(chromeDriver, numberOfPosts)));
        }

        private void Error(string message)
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new ErrorPage(message)));
        }

        private void UpdateProgressBar(int value, int maximum)
        {
            ProgressBar.Dispatcher.Invoke(() => {
                ProgressBar.Value = value;
                ProgressBar.Maximum = maximum;
            });
        }
    }
}
