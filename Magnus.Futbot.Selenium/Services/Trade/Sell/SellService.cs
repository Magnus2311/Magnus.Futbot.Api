using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Models;
using Magnus.Futbot.Services;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Magnus.Futbot.Selenium.Services.Trade.Sell
{
    public class SellService : BaseService
    {
        public TradeAction SellPlayer(SellCardDTO sellCard, ProfileDTO profileDTO, Action<ProfileDTO> updateProfile, CancellationTokenSource cancellationTokenSource)
        {
            var driverInstance = GetInstance(profileDTO.Email);

            var tradeAction = new SellCardAction(new Func<Task>(async () =>
            {
                if (cancellationTokenSource.IsCancellationRequested) return;

                await driverInstance.Driver.OpenTransferList();

                await TrySellPlayer(driverInstance.Driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);
            }), cancellationTokenSource, sellCard);

            return driverInstance.AddAction(tradeAction);
        }

        public TradeAction SellPlayerFromTransferTargets(SellCardDTO sellCard, ProfileDTO profileDTO, Action<ProfileDTO> updateProfile, CancellationTokenSource cancellationTokenSource)
        {
            var driverInstance = GetInstance(profileDTO.Email);

            var tradeAction = new SellCardAction(new Func<Task>(async () =>
            {
                if (cancellationTokenSource.IsCancellationRequested) return;

                await driverInstance.Driver.OpenTransferTargets();

                await TrySellPlayerFromTransferTargets(driverInstance.Driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);
            }), cancellationTokenSource, sellCard);

            return driverInstance.AddAction(tradeAction);
        }

        public async Task SellCurrentPlayer(SellCardDTO sellCard, ProfileDTO profileDTO, Action<ProfileDTO> updateProfile, CancellationTokenSource cancellationTokenSource)
        {
            var driver = GetInstance(profileDTO.Email).Driver;

            if (cancellationTokenSource.IsCancellationRequested) return;

            await InsertPriceValuesAndList(driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);
        }

        public TradeAction RelistPlayers(ProfileDTO profileDTO, CancellationTokenSource cancellationTokenSource)
        {
            var driverInstance = GetInstance(profileDTO.Email);

            var tradeAction = new SellCardAction(new Func<Task>(async () =>
            {
                if (cancellationTokenSource.IsCancellationRequested) return;
                await InitProfileService.InitProfile(profileDTO);

                if (cancellationTokenSource.IsCancellationRequested) return;
                await driverInstance.Driver.OpenTransferList();

                // Clear Sold
                driverInstance.Driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(1) > header > button"))?.Click();
                // Re-list All
                driverInstance.Driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(2) > header > button"))?.Click();
                await Task.Delay(300, cancellationTokenSource.Token);
                driverInstance.Driver.TryFindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > div > button:nth-child(2)"))?.Click();
            }), cancellationTokenSource, null);

            return driverInstance.AddAction(tradeAction);
        }

        private async Task TrySellPlayer(IWebDriver driver, SellCardDTO sellCard, ProfileDTO profileDTO, Action<ProfileDTO> updateProfile, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                var players = new List<IWebElement>();

                players.AddRange(driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li")));
                players.AddRange(driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(2) > ul > li")));                

                var martchingPlayers = players.Where((p) =>
                {
                    var (Name, Rating) = p.GetCardNameAndRating();
                    return sellCard.Card.Name.Contains(Name)
                        && sellCard.Card.Rating == Rating;
                }).Take(sellCard.Count);

                foreach (var player in martchingPlayers)
                {
                    player.Click();

                    await InsertPriceValuesAndList(driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);

                    updateProfile(profileDTO);
                }
            }
            catch
            {
                await TrySellPlayer(driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);
            }
        }

        private async Task TrySellPlayerFromTransferTargets(IWebDriver driver, SellCardDTO sellCard, ProfileDTO profileDTO, Action<ProfileDTO> updateProfile, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                driver
                    .TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(4) > header > button"))
                    ?.Click();

                var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));

                var martchingPlayers = players.Where((p) =>
                {
                    var (Name, Rating) = p.GetCardNameAndRating();
                    return sellCard.Card.Name.Contains(Name)
                        && sellCard.Card.Rating == Rating;
                }).Take(sellCard.Count);

                foreach (var player in martchingPlayers)
                {
                    player.Click();

                    await InsertPriceValuesAndList(driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);

                    updateProfile(profileDTO);
                }
            }
            catch
            {
                await TrySellPlayerFromTransferTargets(driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);
            }
        }

        private async Task InsertPriceValuesAndList(IWebDriver driver, SellCardDTO sellCardDTO, ProfileDTO profileDTO, Action<ProfileDTO> updateProfile, CancellationTokenSource cancellationTokenSource)
        {
            await Task.Delay(300, cancellationTokenSource.Token);
            var showListBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.ut-button-group > button"));
            showListBtn.Click();
            await Task.Delay(300, cancellationTokenSource.Token);

            var notification = driver.TryFindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > p"));
            if (notification is not null && notification.Text == "This item cannot be listed for transfer as you have reached your transfer limit.")
            {
                (driver as ChromeDriver)?.ExecuteScript("services.User.maxAllowedAuctions = 100");
                driver.TryFindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > div > button"))?.Click();
                await Task.Delay(300, cancellationTokenSource.Token);
                showListBtn.Click();
                await Task.Delay(300, cancellationTokenSource.Token);
            }

            notification = driver.TryFindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > p"));
            if (notification is not null && notification.Text == "This item cannot be listed for transfer as you have reached your transfer limit.")
            {
                driver.TryFindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > div > button"))?.Click();
                await Task.Delay(300, cancellationTokenSource.Token);
                await driver.OpenTransferList();

                // Clear Sold
                driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(1) > header > button"))?.Click();

                await Task.Delay(300, cancellationTokenSource.Token);
                await driver.OpenTransferTargets();
                await TrySellPlayerFromTransferTargets(driver, sellCardDTO, profileDTO, updateProfile, cancellationTokenSource);
            }
            else
            {
                var bidPrice = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > div:nth-child(2) > div.ut-numeric-input-spinner-control > input"));
                bidPrice.Click();
                await Task.Delay(200, cancellationTokenSource.Token);
                bidPrice.SendKeys(Keys.Backspace);
                await Task.Delay(300, cancellationTokenSource.Token);
                bidPrice.SendKeys($"{sellCardDTO.FromBid}");

                await Task.Delay(200, cancellationTokenSource.Token);

                var binPrice = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > div:nth-child(3) > div.ut-numeric-input-spinner-control > input"));
                binPrice.Click();
                await Task.Delay(300, cancellationTokenSource.Token);
                binPrice.SendKeys(Keys.Backspace);
                await Task.Delay(300, cancellationTokenSource.Token);
                binPrice.SendKeys($"{sellCardDTO.FromBin}");
                await Task.Delay(300, cancellationTokenSource.Token);

                var listBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > button"));
                listBtn.Click();
                await Task.Delay(100, cancellationTokenSource.Token);
            }
        }
    }
}
