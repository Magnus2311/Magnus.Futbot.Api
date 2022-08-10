using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium;
using OpenQA.Selenium;

namespace Magnus.Futbot.Services.Trade.Buy
{
    public class BidService : BaseService
    {
        private int _wonPlayers;

        public async Task BidPlayer(ProfileDTO profileDTO, BidPlayerDTO bidPlayerDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;

            SearchPlayer(driver, bidPlayerDTO);
            await BidPlayers(driver, bidPlayerDTO, profileDTO);
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

        private async Task BidPlayers(IWebDriver driver, BidPlayerDTO bidPlayerDTO, ProfileDTO profileDTO)
        {
            var endDate = DateTime.Now.AddHours(1);
            do
            {
                await TryBidForPlayers(driver, bidPlayerDTO, profileDTO);
            }
            while (_wonPlayers < bidPlayerDTO.MaxPlayers && endDate > DateTime.Now);
        }

        private async Task TryBidForPlayers(IWebDriver driver, BidPlayerDTO bidPlayerDTO, ProfileDTO profileDTO)
        {
            try
            {
                var nextBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > div > button.flat.pagination.next"));
                var prevBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > div > button.flat.pagination.prev"));
                var allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), 1000);
                if (allPlayers is null)
                {
                    nextBtn?.Click();
                    prevBtn?.Click();
                    Thread.Sleep(1000);
                    allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), 1000);
                }

                if (allPlayers.All(p => p.GetAttribute("class").Contains("won") || p.GetAttribute("class").Contains("expired")))
                {
                    var currentlyWon = allPlayers.Count(p => p.GetAttribute("class").Contains("won"));
                    _wonPlayers += currentlyWon;
                    profileDTO.WonTargetsCount += currentlyWon;
                    // Send kafka message
                    nextBtn?.Click(); //outbid
                    prevBtn?.Click();
                    Thread.Sleep(1000);
                    allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), 1000);
                }


                var outbidded = allPlayers.Count(p => p.GetAttribute("class").Contains("outbid"));
                profileDTO.Outbidded += outbidded;
                // Send kafka message

                var activePlayers = allPlayers.Where(p => !p.GetAttribute("class").Contains("won") && !p.GetAttribute("class").Contains("expired") && !p.GetAttribute("class").Contains("highest-bid"));
                foreach (var player in activePlayers)
                {
                    var startSpan = player.FindElement(By.CssSelector("div > div.auction > div.auctionStartPrice.auctionValue > span.currency-coins.value"));
                    if (!int.TryParse(startSpan?.Text.Replace(",", ""), out var startPrice)) continue;
                    if (startPrice > bidPlayerDTO.MaxPrice) continue;

                    var bidSpan = player.FindElement(By.CssSelector("div > div.auction > div:nth-child(2) > span.currency-coins.value"));
                    if (bidSpan is null) continue;

                    var bid = int.MaxValue;
                    int.TryParse(bidSpan.Text.Replace(",", ""), out bid);
                    var remainingTime = player.FindElement(By.CssSelector("div > div.auction > div.auction-state > span.time")).Text;
                    if ((bidSpan.Text == "---" || bid < bidPlayerDTO.MaxPrice) && remainingTime.Contains("Seconds"))
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
                            // Send kafka message
                        }
                    }
                }
            }
            catch { }
        }
    }
}