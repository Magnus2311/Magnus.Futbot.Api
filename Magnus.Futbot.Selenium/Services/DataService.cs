using Magnus.Futbot.Common.Models.DTOs;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Magnus.Futbot.Services
{
    public class DataSeleniumService : BaseService
    {
        public static ProfileDTO GetBasicData(ProfileDTO profile)
        {
            var driver = GetInstance(profile.Email).Driver;

            var activeBids = GetActiveBidsCount(driver);
            var outbidded = GetOutbiddedCount(driver);

            profile.UnassignedCount = GetUnassignedItems(driver);
            profile.Coins = driver.GetCoins();
            profile.TransferListCount = GetTransferListCount(driver);
            profile.ActiveBidsCount = activeBids;
            profile.Outbidded = outbidded;
            profile.WonTargetsCount = GetTotalTargets(driver) - activeBids - outbidded;

            return profile;
        }

        private static int GetTotalTargets(ChromeDriver driver)
        {
            driver.OpenTransfer();
            var totalTargetsSpan = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-2.ut-tile-transfer-targets > div.tileContent > div > div.total-transfers > span.value"), 1000);
            if (totalTargetsSpan is not null && int.TryParse(totalTargetsSpan.Text, out var totalTargetsCount)) return totalTargetsCount;

            return 0;
        }

        private static int GetOutbiddedCount(ChromeDriver driver)
        {
            driver.OpenTransfer();
            var outbiddedSpan = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-2.ut-tile-transfer-targets > div.tileContent > div > div.finished-transfers > span.value"), 1000);
            if (outbiddedSpan is not null && int.TryParse(outbiddedSpan.Text, out var outbiddedCount)) return outbiddedCount;

            return 0;
        }

        public static int GetUnassignedItems(IWebDriver driver)
        {
            driver.OpenHomePage();
            var unassignedItemsSpan = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.ut-unassigned-tile-view.tile.col-1-1 > div.data-container > span.itemsNumber"), 1000);
            if (unassignedItemsSpan is not null && int.TryParse(unassignedItemsSpan.Text, out var unassignedItemsCount)) return unassignedItemsCount;

            return 0;
        }

        public static int GetTransferListCount(IWebDriver driver)
        {
            driver.OpenTransfer();
            var transferListSpan = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-2.ut-tile-transfer-list > div.tileContent > div > div.total-transfers > span.value"), 2000);
            if (transferListSpan is not null && int.TryParse(transferListSpan.Text, out var transferListCount)) return transferListCount;

            return 0;
        }

        private static int GetActiveBidsCount(ChromeDriver driver)
        {
            driver.OpenTransfer();
            var activeBidsSpan = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div.tile.col-1-2.ut-tile-transfer-targets > div.tileContent > div > div.active-transfers > span.value"), 1000);
            if (activeBidsSpan is not null && int.TryParse(activeBidsSpan.Text, out var activeBidsCount)) return activeBidsCount;

            return 0;
        }
    }
}