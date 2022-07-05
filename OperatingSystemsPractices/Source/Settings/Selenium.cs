using OpenQA.Selenium.Chrome;
using OperatingSystemsPractices.Source.Resources;

namespace OperatingSystemsPractices.Source.Settings
{
    public static class Selenium
    {
        public static ChromeDriverService ChromeService
        {
            get
            {
                ChromeDriverService chromeService = ChromeDriverService.CreateDefaultService();
                chromeService.HideCommandPromptWindow = true;
                return chromeService;
            }
        }
        public static ChromeOptions ChromeOptions
        {
            get
            {
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("user-data-dir=" + Folders.ChromeUserData);
                chromeOptions.AddArgument("--window-size=700,1000");
                chromeOptions.AddArgument("--window-position=0,0");
                return chromeOptions;
            }
        }
    }
}
