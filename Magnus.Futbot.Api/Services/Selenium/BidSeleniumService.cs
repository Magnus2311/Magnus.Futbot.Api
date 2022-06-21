using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Hubs.Interfaces;
using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Api.Models.DTOs.Selenium;
using Magnus.Futbot.Common;
using Microsoft.AspNetCore.SignalR;
using OpenQA.Selenium;

namespace Magnus.Futbot.Api.Services.Selenium
{
    public class BidSeleniumService : BaseSeleniumService
    {
        private int _wonPlayers;

        private readonly IHubContext<ProfilesHub, IProfilesClient> _profilesContext;

        public BidSeleniumService(IHubContext<ProfilesHub, IProfilesClient> profilesContext)
        {
            _profilesContext = profilesContext;
        }

        public void BidPlayer(ProfileDTO profileDTO, BidPlayerDTO bidPlayerDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;

            SearchPlayer(driver, bidPlayerDTO);
            BidPlayers(driver, bidPlayerDTO, profileDTO);
        }

        private void SearchPlayer(IWebDriver driver, BidPlayerDTO bidPlayerDTO)
        {
            driver.OpenSearchTransfer();

            var resetBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(1)"), 1000);
            resetBtn?.Click();

            var playerNameInput = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-player-search-control > div > div.ut-player-search-control--input-container > input"), 5000);
            if (playerNameInput is null) return;

            playerNameInput.SendKeys(bidPlayerDTO.Name);

            Thread.Sleep(500);

            var searchItems = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.ut-pinned-list > div.ut-item-search-view > div.inline-list-select.ut-player-search-control.is-open.has-selection.contract-text-input > div > div.inline-list > ul > button"), 1000);
            if (searchItems is null) return;

            foreach (var item in searchItems)
            {
                if (item is null) continue;

                var playerName = item.FindElement(By.XPath("/span[1]")).Text;
                int.TryParse(item.FindElement(By.XPath("/span[2]")).Text, out var baseRating);

                if (baseRating == bidPlayerDTO.BaseRating)
                {
                    item.Click();
                    break;
                }
            }

            if (bidPlayerDTO.PromoType != PromoType.Basic)
            {
                //TO DO: Add logic for picking rarity based on this prop
            }

            if (bidPlayerDTO.PositionType != PositionType.Any)
            {
                //TO DO: Add logic for position type based on this prop
            }

            if (bidPlayerDTO.ChemistryStyleType != ChemistryStyleType.Any)
            {
                //TO DO: Add logic for chemistry style based on this prop
            }

            var searchBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(2)"), 1000);
            resetBtn?.Click();
        }

        private void BidPlayers(IWebDriver driver, BidPlayerDTO bidPlayerDTO, ProfileDTO profileDTO)
        {
            var endDate = DateTime.Now.AddHours(1);
            do
            {
                TryBidForPlayers(driver, bidPlayerDTO, profileDTO);
            }
            while (_wonPlayers < bidPlayerDTO.MaxPlayers && endDate > DateTime.Now);
        }

        private void TryBidForPlayers(IWebDriver driver, BidPlayerDTO bidPlayerDTO, ProfileDTO profileDTO)
        {
            try
            {
                var nextBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > div > button.flat.pagination.next"));
                var allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), 1000);
                if (allPlayers is null)
                {
                    nextBtn?.Click();
                    Thread.Sleep(1000);
                    allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), 1000);
                }

                if (allPlayers.All(p => p.GetAttribute("class").Contains("won") || p.GetAttribute("class").Contains("expired")))
                {
                    var currentlyWon = allPlayers.Count(p => p.GetAttribute("class").Contains("won"));
                    var outbidded = allPlayers.Count(p => p.GetAttribute("class").Contains("expired"));
                    _wonPlayers += currentlyWon;
                    profileDTO.WonTargetsCount += currentlyWon;
                    nextBtn?.Click();
                    Thread.Sleep(1000);
                    allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), 1000);
                }

                var activePlayers = allPlayers.Where(p => !p.GetAttribute("class").Contains("won") && !p.GetAttribute("class").Contains("expired"));
                foreach (var player in activePlayers)
                {
                    var startSpan = player.FindElement(By.CssSelector("div > div.auction > div.auctionStartPrice.auctionValue > span.currency-coins.value"));
                    if (!int.TryParse(startSpan?.Text.Replace(",", ""), out var startPrice)) continue;
                    if (startPrice > bidPlayerDTO.MaxPrice) continue;

                    var bidSpan = player.FindElement(By.CssSelector("div > div.auction > div:nth-child(2) > span.currency-coins.value"));
                    if (bidSpan is null) continue;

                    var bid = int.MaxValue;
                    int.TryParse(bidSpan.Text.Replace(",", ""), out bid);
                    if (bidSpan.Text == "---" || bid < bidPlayerDTO.MaxPrice)
                    {
                        player.Click();

                        var priceInput = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-navigation-container-view.ui-layout-right > div > div > div.DetailPanel > div.bidOptions > div > input"), 1000);
                        if (priceInput is null) continue;

                        var currentPrice = int.MaxValue;
                        int.TryParse(priceInput.Text.Replace(",", ""), out currentPrice);

                        if (currentPrice <= bidPlayerDTO.MaxPrice)
                        {
                            var makeBidBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-navigation-container-view.ui-layout-right > div > div > div.DetailPanel > div.bidOptions > button.btn-standard.call-to-action.bidButton"));
                            makeBidBtn?.Click();
                            profileDTO.ActiveBidsCount += 1;
                        }
                    }
                }
            }
            catch { }
        }
    }
}