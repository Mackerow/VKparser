using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace OperatingSystemsPractices.Source.WebElements
{
    internal class FindElements
    {
        public static List<IWebElement> ByCssSelector(string cssSelectorToFind, ChromeDriver chromeDriver)
        {
            List<IWebElement> searchedWebElements;
            try
            {
                searchedWebElements = chromeDriver.FindElements(By.CssSelector(cssSelectorToFind)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
            return searchedWebElements;
        }
        public static List<IWebElement> ByCssSelector(string cssSelectorToFind, IWebElement webElement)
        {
            List<IWebElement> searchedWebElements;
            try
            {
                searchedWebElements = webElement.FindElements(By.CssSelector(cssSelectorToFind)).ToList();
            }
            catch (Exception)
            {
                return null;
            }
            return searchedWebElements;
        }
    }
}
