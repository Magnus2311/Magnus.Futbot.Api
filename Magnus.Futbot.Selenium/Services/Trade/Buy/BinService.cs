using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Selenium.Services.Trade.Filters;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Trade.Buy
{
    public class BinService : BaseService
    {
        private int _wonPlayers;
        private Action<ProfileDTO> _updateAction;
        private readonly FiltersService _filtersService;

        public BinService(FiltersService filtersService)
        {
            _filtersService = filtersService;
        }

        public TradeAction BinPlayer(
            ProfileDTO profileDTO, 
            BuyCardDTO buyCardDTO, 
            Action<ProfileDTO> updateAction,
            CancellationTokenSource cancellationTokenSource,
            Func<Task>? sellAction)
        {
            _updateAction = updateAction;
            var driverInstance = GetInstance(profileDTO.Email);

            var tradeAction = new BuyAction(new Func<Task>(async () =>
            {
                await SetupForBin(driverInstance.Driver, profileDTO, buyCardDTO, updateAction, cancellationTokenSource, sellAction);

            }), cancellationTokenSource, buyCardDTO);

            return driverInstance.AddAction(tradeAction);
        }

        private async Task SetupForBin(
            IWebDriver driver,
            ProfileDTO profileDTO,
            BuyCardDTO buyCardDTO,
            Action<ProfileDTO> updateAction,
            CancellationTokenSource cancellationTokenSource,
            Func<Task>? sellAction)
        {
            if (!driver.Url.Contains("https://www.ea.com/fifa/ultimate-team/web-app/"))
                LoginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            _filtersService.InsertFilters(profileDTO.Email, buyCardDTO);

            await SetPrice(driver, buyCardDTO, cancellationTokenSource);

            while (_wonPlayers < buyCardDTO.Count && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    await TryBinPlayer(driver, buyCardDTO, profileDTO, updateAction, cancellationTokenSource, sellAction);
                }
                catch
                {
                    await SetupForBin(driver, profileDTO, buyCardDTO, updateAction, cancellationTokenSource, sellAction);
                }
            }
        }

        public async Task TryBinPlayer(
            IWebDriver driver,
            BuyCardDTO buyCardDTO,
            ProfileDTO profileDTO,
            Action<ProfileDTO> updateAction,
            CancellationTokenSource cancellationTokenSource,
            Func<Task>? sellAction)
        {
            if (cancellationTokenSource.Token.IsCancellationRequested) updateAction(profileDTO);

            await Task.Delay(300);
            var allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), TimeSpan.FromSeconds(2));
            if (allPlayers is null || allPlayers.Count() == 0)
            {
                await Task.Delay(2000);
                var title = driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-bar-view.navbar-style-landscape > h1"))?.Text;
                if (title == "TRANSFERS")
                {
                    cancellationTokenSource.Cancel();
                }
                else
                {
                    var backBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-bar-view.navbar-style-landscape > button.ut-navigation-button-control"));
                    backBtn?.Click();
                }

                await Task.Delay(150, cancellationTokenSource.Token);
                await ConfigurePrices(driver, buyCardDTO, cancellationTokenSource);

                var searchBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(2)"), 1000);
                searchBtn?.Click();
            }
            else
            {
                foreach (var player in allPlayers)
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested) updateAction(profileDTO);

                    await Task.Delay(300, cancellationTokenSource.Token);
                    if (driver.GetCoins() < buyCardDTO.Price) cancellationTokenSource.Cancel();

                    player.Click();
                    await Task.Delay(300, cancellationTokenSource.Token);

                    var binBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-navigation-container-view.ui-layout-right > div > div > div.DetailPanel > div.bidOptions > button.btn-standard.buyButton"));
                    binBtn?.Click();

                    await Task.Delay(100, cancellationTokenSource.Token);

                    var popupText = driver.TryFindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > p"))?.Text;
                    if (!string.IsNullOrEmpty(popupText))
                    {
                        if (popupText == "You cannot get this Item because you have 5 or more Unassigned Items.")
                        {
                            cancellationTokenSource.Cancel();
                            return;
                        }
                    }

                    var okBtn = driver.FindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > div > button:nth-child(1)"));
                    okBtn.Click();

                    var errorMessage = driver.TryFindElement(By.CssSelector("#NotificationLayer > div > p"));
                    if (errorMessage is not null)
                    {
                        if (errorMessage.Text == "Bid status changed, auction data will be updated.") continue;
                        else if (errorMessage.Text != "Player Moved to Transfer List")
                        {
                            await Task.Delay(15000, cancellationTokenSource.Token);
                            await SetupForBin(driver, profileDTO, buyCardDTO, _updateAction, cancellationTokenSource, sellAction);
                        }
                    }

                    await sellAction?.Invoke();

                    profileDTO.Coins = driver.GetCoins();
                    profileDTO.WonTargetsCount++;
                    _updateAction(profileDTO);
                    _wonPlayers++;

                    await Task.Delay(300, cancellationTokenSource.Token);
                    driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-navigation-container-view.ui-layout-right > div > div > div.DetailPanel > div.ut-button-group > button:nth-child(8)"))
                        ?.Click();
                }

                await Task.Delay(300, cancellationTokenSource.Token);
                var backBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-bar-view.navbar-style-landscape > button.ut-navigation-button-control"));
                backBtn?.Click();

                await ConfigurePrices(driver, buyCardDTO, cancellationTokenSource);

                var searchBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(2)"), 1000);
                searchBtn?.Click();
            }

            updateAction(profileDTO);
        }

        private async Task SetPrice(IWebDriver driver, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource)
        {
            var currentValue = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.search-prices > div:nth-child(6) > div.ut-numeric-input-spinner-control > input"));
            currentValue.Click();
            await Task.Delay(100, cancellationTokenSource.Token);
            currentValue.SendKeys($"{buyCardDTO.Price}");

            var searchBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(2)"), 1000);
            searchBtn?.Click();
        }

        private async Task ConfigurePrices(IWebDriver driver, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource)
        {
            var minBinPlus = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.search-prices > div:nth-child(5) > div.ut-numeric-input-spinner-control > button.btn-standard.increment-value"));
            minBinPlus?.Click();

            var minBinPlusValue = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.search-prices > div:nth-child(5) > div.ut-numeric-input-spinner-control > input"));
            if (double.TryParse(minBinPlusValue.GetAttribute("value"), out var currentPrice))
            {
                if (currentPrice > ((double)buyCardDTO.Price * 0.8) || currentPrice > 1000)
                {
                    minBinPlusValue.Click();
                    await Task.Delay(100, cancellationTokenSource.Token);
                    minBinPlusValue.SendKeys(Keys.Backspace);
                }
            }
        }
    }
}
