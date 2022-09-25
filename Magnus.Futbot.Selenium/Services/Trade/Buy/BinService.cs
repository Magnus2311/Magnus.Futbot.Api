﻿using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Selenium.Services.Trade.Filters;
using Magnus.Futbot.Services;
using OpenQA.Selenium;
using System.Threading;

namespace Magnus.Futbot.Selenium.Services.Trade.Buy
{
    public class BinService : BaseService
    {
        private int _wonPlayers;
        private Action<ProfileDTO>? _updateAction;
        private readonly FiltersService _filtersService;

        public BinService(FiltersService filtersService)
        {
            _filtersService = filtersService;
        }

        public async Task<ProfileDTO> BinPlayer(
            ProfileDTO profileDTO, 
            BuyCardDTO buyCardDTO, 
            Action<ProfileDTO> updateAction,
            CancellationTokenSource cancellationTokenSource)
        {
            _updateAction = updateAction;
            var driver = GetInstance(profileDTO.Email).Driver;

            if (!driver.Url.Contains("https://www.ea.com/fifa/ultimate-team/web-app/"))
                LoginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            _filtersService.InsertFilters(profileDTO.Email, buyCardDTO);

            await SetPrice(driver, buyCardDTO, cancellationTokenSource);

            while (_wonPlayers < buyCardDTO.Count && !cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    await TryBinPlayer(driver, buyCardDTO, profileDTO, cancellationTokenSource);
                }
                catch
                { }
            }

            return profileDTO;
        }

        public async Task<ProfileDTO> TryBinPlayer(
            IWebDriver driver, 
            BuyCardDTO buyCardDTO, 
            ProfileDTO profileDTO,
            CancellationTokenSource cancellationTokenSource)
        {
            if (cancellationTokenSource.Token.IsCancellationRequested) return profileDTO;

            await Task.Delay(100, cancellationTokenSource.Token);
            var allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), 1000);
            if (allPlayers is null || allPlayers.Count() == 0)
            {
                Thread.Sleep(1000);
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

                var lowerBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.search-prices > div:nth-child(6) > div.ut-numeric-input-spinner-control > button.btn-standard.decrement-value"));
                lowerBtn?.Click();

                var currentValue = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.search-prices > div:nth-child(6) > div.ut-numeric-input-spinner-control > input"));

                if (double.TryParse(currentValue.GetAttribute("value"), out var currentPrice))
                {
                    if (currentPrice / (double)buyCardDTO.Price < 0.7)
                    {
                        currentValue.Click();
                        await Task.Delay(100, cancellationTokenSource.Token);
                        currentValue.SendKeys($"{buyCardDTO.Price}");
                    }
                }
                
                var searchBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(2)"), 1000);
                searchBtn?.Click();
            }
            else
            {
                foreach (var player in allPlayers)
                {
                    Thread.Sleep(100);
                    if (driver.GetCoins() < buyCardDTO.Price) cancellationTokenSource.Cancel();

                    player.Click();
                    await Task.Delay(100, cancellationTokenSource.Token);

                    var binBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-navigation-container-view.ui-layout-right > div > div > div.DetailPanel > div.bidOptions > button.btn-standard.buyButton"));
                    binBtn?.Click();

                    await Task.Delay(100, cancellationTokenSource.Token);
                    var okBtn = driver.FindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > div > button:nth-child(1)"));
                    okBtn.Click();

                    // Send item to transfer list

                    var errorMessage = driver.TryFindElement(By.CssSelector("#NotificationLayer > div > p"));
                    if (errorMessage is not null)
                    {
                        if (errorMessage.Text == "Bid status changed, auction data will be updated.") continue;

                        Thread.Sleep(15000);
                        await BinPlayer(profileDTO, buyCardDTO, _updateAction, cancellationTokenSource);
                    }

                    profileDTO.Coins = driver.GetCoins();
                    profileDTO.WonTargetsCount++;
                    _updateAction(profileDTO);
                    _wonPlayers++;

                    Thread.Sleep(500);
                    driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-navigation-container-view.ui-layout-right > div > div > div.DetailPanel > div.ut-button-group > button:nth-child(8)"))?.Click();
                }

                Thread.Sleep(1000);
                var backBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-bar-view.navbar-style-landscape > button.ut-navigation-button-control"));
                backBtn?.Click();

                var lowerBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.search-prices > div:nth-child(6) > div.ut-numeric-input-spinner-control > button.btn-standard.decrement-value"));
                lowerBtn?.Click();

                var currentValue = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.search-prices > div:nth-child(6) > div.ut-numeric-input-spinner-control > input"));

                if (double.TryParse(currentValue.GetAttribute("value"), out var currentPrice))
                {
                    if (currentPrice / (double)buyCardDTO.Price < 0.7)
                    {
                        currentValue.Click();
                        await Task.Delay(100, cancellationTokenSource.Token);
                        currentValue.SendKeys($"{buyCardDTO.Price}");
                    }
                }

                var searchBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(2)"), 1000);
                searchBtn?.Click();
            }

            return profileDTO;
        }

        public async Task SetPrice(IWebDriver driver, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource)
        {
            var currentValue = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.search-prices > div:nth-child(6) > div.ut-numeric-input-spinner-control > input"));
            currentValue.Click();
            await Task.Delay(100, cancellationTokenSource.Token);
            currentValue.SendKeys($"{buyCardDTO.Price}");

            var searchBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(2)"), 1000);
            searchBtn?.Click();
        }
    }
}
