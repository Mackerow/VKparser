using System.Threading;
using System.Windows.Controls;
using System.Windows.Navigation;
using OpenQA.Selenium.Chrome;
using OperatingSystemsPractices.Source.Settings;

namespace OperatingSystemsPractices.Source.Pages.VkParser
{    
    public partial class OpenChrome : Page
    {
        ChromeDriver chromeDriver;
        int numberOfPosts;

        public OpenChrome(int numberOfPosts)
        {
            InitializeComponent();

            this.numberOfPosts = numberOfPosts;

            Thread thread = new Thread(() => Start()) { IsBackground = true };
            thread.Start();
        }

        private void Start()
        {
            try
            {
                chromeDriver = new ChromeDriver(Selenium.ChromeService, Selenium.ChromeOptions);
            }
            catch
            {
                Error(ErrorMessages.Selenium.OpenChrome);
                return;
            }

            NextStep();
        }

        private void NextStep()
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new OpenVkPage(chromeDriver, numberOfPosts)));
        }

        private void Error(string message)
        {
            Dispatcher.InvokeAsync(() => NavigationService.Navigate(new ErrorPage(message)));
        }
    }
}
