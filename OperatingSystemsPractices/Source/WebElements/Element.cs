using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace OperatingSystemsPractices.Source.WebElements
{
    internal class Element
    {
        public static bool Displayed(IWebElement webElement)
        {
            try
            {
                if (!webElement.Displayed)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static bool TryClick(IWebElement webElement)
        {
            while (true)
            {
                try { webElement.Click(); }
                catch (ElementClickInterceptedException) { continue; }
                catch (Exception) { return false; }
                return true;
            }
        }
        public static void MoveTo(IWebDriver driver, IWebElement webElement)
        {
            Actions actions = new Actions(driver);
            actions.MoveToElement(webElement);
            actions.Perform();
        }
    }
}
