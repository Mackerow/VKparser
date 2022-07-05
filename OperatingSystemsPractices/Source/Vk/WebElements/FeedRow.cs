using OpenQA.Selenium;
using OperatingSystemsPractices.Source.WebElements;
using System;
using System.Collections.Generic;
using OperatingSystemsPractices.Source.Resources;

namespace OperatingSystemsPractices.Source.Vk.WebElements
{
    public class FeedRow
    {
        public static Post GetPost(IWebDriver driver, IWebElement feedRowElement)
        {
            if (!Element.Displayed(feedRowElement))
                return null;

            if (!IsPost(feedRowElement))
                return null;

            LoadFullPost(driver, feedRowElement);

            Post post = new Post()
            {
                Id = PostId(feedRowElement),
                Text = PostText(feedRowElement),
                Photos = PostPhotosLinks(feedRowElement),
                Hrefs = PostHrefs(feedRowElement)
            };

            return post;
        }

        private static bool IsPost(IWebElement feedRowElement)
        {
            return FindElement.ByCssSelector(CssSelectors.PostIdentifier, feedRowElement) != null;
        }

        private static void LoadFullPost(IWebDriver driver, IWebElement feedRowElement)
        {
            IWebElement showMore = FindElement.ByCssSelector(CssSelectors.ShowFullPost, feedRowElement);
            if (showMore != null)
            {
                Element.MoveTo(driver, showMore);
                Element.TryClick(showMore);
            }
        }

        private static string PostId(IWebElement feedRowElement)
        {
            IWebElement idElement = FindElement.ByCssSelector(CssSelectors.PostViewHash, feedRowElement);
            return idElement == null ? string.Empty : idElement.GetAttribute("data-post-id");
        }

        private static string PostText(IWebElement feedRowElement)
        {
            IWebElement textElement = FindElement.ByCssSelector(CssSelectors.WallPostText, feedRowElement);
            return textElement != null ? textElement.Text : string.Empty;
        }

        private static string[] PostPhotosLinks(IWebElement feedRowElement)
        {
            IWebElement wallText = FindElement.ByCssSelector(CssSelectors.WallPost, feedRowElement);
            if (wallText == null)
                return Array.Empty<string>();

            List<IWebElement> photoElements = FindElements.ByCssSelector(CssSelectors.Photo, wallText);
            if (photoElements == null || photoElements.Count == 0)
                return Array.Empty<string>();

            string[] photoLinksArray = new string[photoElements.Count];
            int currentIndex = 0;

            foreach (IWebElement photoElement in photoElements)
            {
                photoLinksArray[currentIndex] = photoElement.GetAttribute("onclick");
                photoLinksArray[currentIndex] = photoLinksArray[currentIndex].Replace(@"\/", "/");
                photoLinksArray[currentIndex] = photoLinksArray[currentIndex].Substring(photoLinksArray[currentIndex].IndexOf(@"https://"));
                photoLinksArray[currentIndex] = photoLinksArray[currentIndex].Substring(0, photoLinksArray[currentIndex].IndexOf("\""));
                currentIndex++;
            }

            return photoLinksArray;
        }

        private static string[] PostPhotosIds(IWebElement feedRowElement)
        {
            IWebElement wallText = FindElement.ByCssSelector(CssSelectors.WallPost, feedRowElement);
            if (wallText == null)
                return Array.Empty<string>();


            List<IWebElement> photoElements = FindElements.ByCssSelector(CssSelectors.Photo, wallText);
            if (photoElements == null || photoElements.Count == 0)
                return Array.Empty<string>();

            string[] photosIds = new string[photoElements.Count];
            int currentIndex = 0;

            foreach (IWebElement photoElement in photoElements)
            {
                photosIds[currentIndex] = photoElement.GetAttribute("onclick");
                photosIds[currentIndex] = photosIds[currentIndex].Substring(photosIds[currentIndex].IndexOf("'") + 1);
                photosIds[currentIndex] = photosIds[currentIndex].Substring(0, photosIds[currentIndex].IndexOf("'"));
                currentIndex++;
            }

            return photosIds;
        }

        private static string[] PostHrefs(IWebElement feedRowElement)
        {
            IWebElement wallText = FindElement.ByCssSelector(CssSelectors.WallPostText, feedRowElement);
            if (wallText == null)
                return Array.Empty<string>();

            List<IWebElement> hrefElements = FindElements.ByCssSelector(CssSelectors.Href, wallText);
            if (hrefElements == null || hrefElements.Count == 0)
                return Array.Empty<string>();

            string[] hrefs = new string[hrefElements.Count];
            int currentIndex = 0;

            foreach (IWebElement photoElement in hrefElements)
            {
                hrefs[currentIndex] = photoElement.GetAttribute("href");

                hrefs[currentIndex] = hrefs[currentIndex].Replace(@"%3A", ":");
                hrefs[currentIndex] = hrefs[currentIndex].Replace(@"%2F", "/");

                if (hrefs[currentIndex].Contains(@"https://vk.com/away.php?to="))
                {
                    hrefs[currentIndex] = hrefs[currentIndex].Replace(@"https://vk.com/away.php?to=", "");
                    hrefs[currentIndex] = hrefs[currentIndex].Substring(0, hrefs[currentIndex].IndexOf("&"));
                }

                currentIndex++;
            }

            return hrefs;
        }
    }
}
