using System.Threading;
using System.Windows.Controls;
using System.Windows.Navigation;
using OpenQA.Selenium.Chrome;
using OperatingSystemsPractices.Source.Resources;

namespace OperatingSystemsPractices.Source.Pages.VkParser
{
    public partial class OpenVkPage : Page
    {
        ChromeDriver chromeDriver;
        int numberOfPosts;

        public OpenVkPage(ChromeDriver chromeDriver, int numberOfPosts)
        {
            InitializeComponent();

            this.chromeDriver = chromeDriver;
            this.numberOfPosts = numberOfPosts;

            Thread thread = new Thread(() => Start()) { IsBackground = true };
            thread.Start();
        }

        private void Start()
        {
            chromeDriver.Navigate().GoToUrl(Urls.Vk);
            NextStep();
        }

        private void NextStep()
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new LoadPostsPage(chromeDriver, numberOfPosts)));
        }
    }
}
