using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using OpenQA.Selenium.Chrome;
using OperatingSystemsPractices.Source.Vk;

namespace OperatingSystemsPractices.Source.Pages.VkParser
{
    public partial class CloseChrome : Page
    {
        ChromeDriver chromeDriver;
        List<Post> posts;

        public CloseChrome(ChromeDriver chromeDriver, List<Post> posts)
        {
            InitializeComponent();

            this.chromeDriver = chromeDriver;
            this.posts = posts;

            Thread thread = new Thread(() => Start()) { IsBackground = true };
            thread.Start();
        }

        private void Start()
        {
            chromeDriver.Quit();

            NextStep();
        }

        private void NextStep()
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new WriteAndReadJsons(posts)));
        }
    }
}
