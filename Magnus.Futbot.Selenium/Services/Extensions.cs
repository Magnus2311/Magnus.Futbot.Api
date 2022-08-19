using Magnus.Futbot.Common.Models.Selenium.Trading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Magnus.Futbot.Services
{
    public static class SeleniumExtensions
    {
        public static IWebElement? FindElement(this IWebDriver driver, By by, int milliseconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(milliseconds));
            try
            {
                var isElementActive = wait.Until(drv =>
                {
                    try
                    {
                        var element = drv.FindElement(by);
                        return element.Displayed && element.Enabled;
                    }
                    catch
                    {
                        return false;
                    }
                });
                return isElementActive ? driver.FindElement(by) : null;
            }
            catch
            {
                return null;
            }
        }

        public static IEnumerable<IWebElement> FindElements(this IWebDriver driver, By by, int milliseconds)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(milliseconds));
            var isElementActive = wait.Until(drv =>
            {
                try
                {
                    var elements = drv.FindElements(by);
                    return elements.All(e => e.Displayed) && elements.All(e => e.Enabled);
                }
                catch
                {
                    return false;
                }
            });
            return driver.FindElements(by);
        }

        public static void OpenTransfer(this IWebDriver driver)
        {
            driver.FindElement(By.CssSelector("body > main > section > nav > button.ut-tab-bar-item.icon-transfer")).Click();
        }

        public static void OpenTransferTargets(this IWebDriver driver)
        {
            driver.OpenTransfer();
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-2.ut-tile-transfer-targets")).Click();
            Thread.Sleep(500);
        }

        public static void OpenTransferList(this IWebDriver driver)
        {
            driver.OpenTransfer();
            Thread.Sleep(500);
            driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-2.ut-tile-transfer-list")).Click();
            Thread.Sleep(500);
        }

        public static void OpenHomePage(this IWebDriver driver)
        {
            var homeBtn = driver.FindElement(By.CssSelector("body > main > section > nav > button.ut-tab-bar-item.icon-home"), 10000);
            homeBtn?.Click();
        }

        public static void OpenSearchTransfer(this IWebDriver driver)
        {
            driver.OpenTransfer();
            var searchDiv = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-1.ut-tile-transfer-market"), 2000);
            if (searchDiv is not null) searchDiv.Click();
            Thread.Sleep(1000);
        }

        public static PlayerCard ConvertPlayerElementToPlayerCard(this IWebElement player)
            => new PlayerCard
            {
                Name = player.FindElement(By.CssSelector("div > div.entityContainer > div.name")).Text,
                Rating = int.Parse(player.FindElement(By.CssSelector("div > div.entityContainer > div.small.player.item.specials.ut-item-loaded > div.ut-item-view--main.ut-item-view > div.playerOverview > div.rating")).Text)
            };
    }
}