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

            var tradeAction = new TradeAction(new Func<Task>(async () =>
            {
                if (cancellationTokenSource.IsCancellationRequested) return;

                driverInstance.Driver.OpenTransferList();

                TrySellPlayer(driverInstance.Driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);
            }), false, null, sellCard, cancellationTokenSource);

            driverInstance.AddAction(tradeAction);
        }

        public void SellCurrentPlayer(SellCardDTO sellCard, ProfileDTO profileDTO, CancellationTokenSource cancellationTokenSource)
        {
            var driver = GetInstance(profileDTO.Email).Driver;

            if (cancellationTokenSource.IsCancellationRequested) return;

            InsertPriceValuesAndList(driver, sellCard);
        }

        public void RelistPlayers(string email, CancellationTokenSource cancellationTokenSource)
        {
            var driverInstance = GetInstance(email);

            var tradeAction = new TradeAction(new Func<Task>(async () =>
            {
                if (cancellationTokenSource.IsCancellationRequested) return;

                driverInstance.Driver.OpenTransferList();

                // Clear Sold
                driverInstance.Driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(1) > header > button"))?.Click();
                // Re-list All
                driverInstance.Driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(2) > header > button"))?.Click();
                await Task.Delay(300, cancellationTokenSource.Token);
                driverInstance.Driver.TryFindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > div > button:nth-child(2)"))?.Click();
            }), false, null, null, cancellationTokenSource);

            driverInstance.AddAction(tradeAction);
        }

        private void TrySellPlayer(IWebDriver driver, SellCardDTO sellCard, ProfileDTO profileDTO, Action<ProfileDTO> updateProfile, CancellationTokenSource cancellationTokenSource)
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

                    InsertPriceValuesAndList(driver, sellCard);

                    updateProfile(profileDTO);
                }
            }
            catch
            {
                TrySellPlayer(driver, sellCard, profileDTO, updateProfile, cancellationTokenSource);
            }
        }

        private void InsertPriceValuesAndList(IWebDriver driver, SellCardDTO sellCardDTO)
        {
            driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.ut-button-group > button"))
                        .Click();
            Thread.Sleep(300);
            var bidPrice = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > div:nth-child(2) > div.ut-numeric-input-spinner-control > input"));
            bidPrice.Click();
            Thread.Sleep(100);
            bidPrice.SendKeys(Keys.Backspace);
            bidPrice.SendKeys($"{sellCardDTO.FromBid}");
            Thread.Sleep(100);

            var binPrice = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > div:nth-child(3) > div.ut-numeric-input-spinner-control > input"));
            binPrice.Click();
            Thread.Sleep(100);
            binPrice.SendKeys(Keys.Backspace);
            binPrice.SendKeys($"{sellCardDTO.FromBin}");
            Thread.Sleep(100);

            var listBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > button"));
            listBtn.Click();
            Thread.Sleep(100);
        }
    }
}
