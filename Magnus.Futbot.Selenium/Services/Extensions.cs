using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.Database.Card;
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
            if (!driver.Url.Contains("https://www.ea.com/fifa/ultimate-team/web-app/"))
                driver.Navigate().GoToUrl("https://www.ea.com/fifa/ultimate-team/web-app/");

            driver.FindElement(By.CssSelector("body > main > section > nav > button.ut-tab-bar-item.icon-transfer")).Click();
            Thread.Sleep(500);
        }

        public static bool OpenUnassignedItems(this IWebDriver driver)
        {
            driver.OpenHomePage();
            var unassignedBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.ut-unassigned-tile-view.tile.col-1-1"));
            if (unassignedBtn is not null
                && unassignedBtn.Enabled
                && unassignedBtn.Displayed)
            {
                unassignedBtn.Click();
                Thread.Sleep(500);
                return true;
            }

            return false;
        }

        public static void OpenTransferTargets(this IWebDriver driver)
        {
            driver.OpenTransfer();
            driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-2.ut-tile-transfer-targets")).Click();
            Thread.Sleep(1000);
        }

        public static void OpenTransferList(this IWebDriver driver)
        {
            driver.OpenTransfer();
            driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-2.ut-tile-transfer-list")).Click();
            Thread.Sleep(1000);
        }

        public static void OpenHomePage(this IWebDriver driver)
        {
            var homeBtn = driver.FindElement(By.CssSelector("body > main > section > nav > button.ut-tab-bar-item.icon-home"), 10000);
            homeBtn?.Click();
            Thread.Sleep(1000);
        }

        public static void OpenSearchTransfer(this IWebDriver driver)
        {
            driver.OpenTransfer();
            var searchDiv = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-1.ut-tile-transfer-market"), 2000);
            if (searchDiv is not null) searchDiv.Click();
            Thread.Sleep(1000);
        }

        public static (string Name, int Rating) GetCardNameAndRating(this IWebElement player)
            => new()
            {
                Name = player.FindElement(By.ClassName("name")).Text,
                Rating = int.Parse(player.FindElement(By.ClassName("rating")).Text)
            };

        public static TransferCard ConvertCardToTransferCard(this IWebElement player, IEnumerable<Card> cards)
        {
            // Remove this line
            return new TransferCard
            {
                PossibleCards = new List<Card>
                {
                    cards.FirstOrDefault()!
                },
                PlayerCardStatus = PlayerCardStatus.Won,
                BougthFor = 14000
            };

            var transferCard = new TransferCard();
            var (Name, Rating) = player.GetCardNameAndRating();
            IWebElement? canvas = null;

            var availableCards = cards.Where(c => c.Rating == Rating && c.Name.Contains(Name));
            if (availableCards.Count() == 1)
                return new TransferCard
                {
                    PossibleCards = availableCards,
                    PlayerCardStatus = PlayerCardStatus.Won,
                };


            canvas ??= player.TryFindElement(By.CssSelector("div > div.entityContainer > div.small.player.item.specials.ut-item-loaded > canvas"));
            if (canvas is not null)
            {
                var currentCards = availableCards.Where(c =>
                    c.PromoType != PromoType.Bronze
                    && c.PromoType != PromoType.BronzeRare
                    && c.PromoType != PromoType.Silver
                    && c.PromoType != PromoType.SilverRare
                    && c.PromoType != PromoType.Gold
                    && c.PromoType != PromoType.GoldRare);

                return new TransferCard
                {
                    PossibleCards = currentCards,
                    PlayerCardStatus = PlayerCardStatus.Won,
                };
            }

            canvas ??= player.TryFindElement(By.CssSelector("div > div.entityContainer > div.small.player.item.common.ut-item-loaded > canvas"));
            if (canvas is not null)
            {
                var currentCards = availableCards.Where(c =>
                     c.PromoType == PromoType.Bronze
                     || c.PromoType == PromoType.Silver
                     || c.PromoType == PromoType.Gold);

                return new TransferCard
                {
                    PossibleCards = currentCards,
                    PlayerCardStatus = PlayerCardStatus.Won,
                };
            }

            canvas ??= player.TryFindElement(By.CssSelector("div > div.entityContainer > div.small.player.item.rare.ut-item-loaded > canvas"));
            if (canvas is not null)
            {
                var currentCards = availableCards.Where(c =>
                     c.PromoType == PromoType.BronzeRare
                     || c.PromoType == PromoType.SilverRare
                     || c.PromoType == PromoType.GoldRare);

                return new TransferCard
                {
                    PossibleCards = currentCards,
                    PlayerCardStatus = PlayerCardStatus.Won,
                };
            }

            return new TransferCard()
            {
                PossibleCards = new List<Card>
                {
                    new Card()
                    {
                        Name = Name,
                        Rating = Rating
                    }
                }
            };
        }

        public static int GetCoins(this IWebDriver driver)
        {
            var coinsDiv = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-bar-view.navbar-style-landscape.currency-purchase > div.view-navbar-currency > div.view-navbar-currency-coins"), 1000);
            if (coinsDiv is not null && int.TryParse(coinsDiv.Text.Replace(",", ""), out var coins)) return coins;

            return 0;
        }

        public static IWebElement? TryFindElement(this IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch
            {
                return null;
            }
        }

        public static IWebElement? TryFindElement(this IWebElement element, By by)
        {
            try
            {
                return element.FindElement(by);
            }
            catch
            {
                return null;
            }
        }
    }
}