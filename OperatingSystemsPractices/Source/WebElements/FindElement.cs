using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace OperatingSystemsPractices.Source.WebElements
{
    internal class FindElement
    {
        public static IWebElement ByCssSelector(string cssSelectorToFind, ChromeDriver chromeDriver)
        {
            IWebElement searchedWebElement;
            try
            {
                searchedWebElement = chromeDriver.FindElement(By.CssSelector(cssSelectorToFind));
            }
            catch (Exception)
            {
                return null;
            }
            return searchedWebElement;
        }
        public static IWebElement ByCssSelector(string cssSelectorToFind, IWebElement webElement)
        {
            IWebElement searchedWebElement;
            try
            {
                searchedWebElement = webElement.FindElement(By.CssSelector(cssSelectorToFind));
            }
            catch (Exception)
            {
                return null;
            }
            return searchedWebElement;
        }
    }
}
