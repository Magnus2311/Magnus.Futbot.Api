using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Trade.Sell
{
    public class SellService : BaseService
    {
        public void SellPlayer(SellCardDTO sellCard, ProfileDTO profileDTO, Action<ProfileDTO> updateProfile, CancellationTokenSource cancellationTokenSource)
        {
            var driverInstance = GetInstance(profileDTO.Email);

            var tradeAction = new SellCardAction(new Func<Task>(async () =>
            {
                if (cancellationTokenSource.IsCancellationRequested) return;

                driverInstance.Driver.OpenTransferList();

                await TrySellPlayer(driverInstance.Driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);
            }), cancellationTokenSource, sellCard);

            driverInstance.AddAction(tradeAction);
        }

        public async Task SellCurrentPlayer(SellCardDTO sellCard, ProfileDTO profileDTO, CancellationTokenSource cancellationTokenSource)
        {
            var driver = GetInstance(profileDTO.Email).Driver;

            if (cancellationTokenSource.IsCancellationRequested) return;

            await InsertPriceValuesAndList(driver, sellCard);
        }

        public void RelistPlayers(ProfileDTO profileDTO, CancellationTokenSource cancellationTokenSource)
        {
            var driverInstance = GetInstance(profileDTO.Email);

            var tradeAction = new SellCardAction(new Func<Task>(async () =>
            {
                if (cancellationTokenSource.IsCancellationRequested) return;
                InitProfileService.InitProfile(profileDTO);

                if (cancellationTokenSource.IsCancellationRequested) return;
                driverInstance.Driver.OpenTransferList();

                // Clear Sold
                driverInstance.Driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(1) > header > button"))?.Click();
                // Re-list All
                driverInstance.Driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(2) > header > button"))?.Click();
                await Task.Delay(300, cancellationTokenSource.Token);
                driverInstance.Driver.TryFindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > div > button:nth-child(2)"))?.Click();
            }), cancellationTokenSource, null);

            driverInstance.AddAction(tradeAction);
        }

        private async Task TrySellPlayer(IWebDriver driver, SellCardDTO sellCard, ProfileDTO profileDTO, Action<ProfileDTO> updateProfile, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));
                players.ToList().AddRange(driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(2) > ul > li")));

                var martchingPlayers = players.Where((p) =>
                {
                    var (Name, Rating) = p.GetCardNameAndRating();
                    return sellCard.Card.Name.Contains(Name)
                        && sellCard.Card.Rating == Rating;
                }).Take(sellCard.Count);

                foreach (var player in martchingPlayers)
                {
                    player.Click();

                    await InsertPriceValuesAndList(driver, sellCard);

                    updateProfile(profileDTO);
                }
            }
            catch
            {
                await TrySellPlayer(driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);
            }
        }

        private async Task InsertPriceValuesAndList(IWebDriver driver, SellCardDTO sellCardDTO)
        {
            driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.ut-button-group > button"))
                        .Click();
            await Task.Delay(300);
            var bidPrice = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > div:nth-child(2) > div.ut-numeric-input-spinner-control > input"));
            bidPrice.Click();
            await Task.Delay(200);
            bidPrice.SendKeys(Keys.Backspace);
            bidPrice.SendKeys($"{sellCardDTO.FromBid}");
            await Task.Delay(200);

            var binPrice = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > div:nth-child(3) > div.ut-numeric-input-spinner-control > input"));
            binPrice.Click();
            await Task.Delay(100);
            binPrice.SendKeys(Keys.Backspace);
            binPrice.SendKeys($"{sellCardDTO.FromBin}");
            await Task.Delay(100);

            var listBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > button"));
            listBtn.Click();
            await Task.Delay(100);
        }
    }
}
