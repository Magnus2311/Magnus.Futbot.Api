using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Selenium.Services.Trade.Filters;
using OpenQA.Selenium;

namespace Magnus.Futbot.Services.Trade.Buy
{
    public class BidService : BaseService
    {
        private int _wonPlayers;
        private Action<ProfileDTO>? _updateAction;
        private readonly FiltersService _filtersService;

        public BidService(FiltersService filtersService)
        {
            _filtersService = filtersService;
        }

        public ProfileDTO BidPlayer(ProfileDTO profileDTO, BuyCardDTO bidPlayerDTO, Action<ProfileDTO> updateAction)
        {
            _updateAction = updateAction;
            var driver = GetInstance(profileDTO.Email).Driver;

            if (!driver.Url.Contains("https://www.ea.com/fifa/ultimate-team/web-app/"))
            {
                LoginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
            }

            _filtersService.InsertFilters(profileDTO.Email, bidPlayerDTO);
            var searchBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(2)"), 1000);
            searchBtn?.Click();
            return BidPlayers(driver, bidPlayerDTO, profileDTO);
        }

        private ProfileDTO BidPlayers(IWebDriver driver, BuyCardDTO bidPlayerDTO, ProfileDTO profileDTO)
        {
            var endDate = DateTime.Now.AddHours(1);
            do
            {
                profileDTO = TryBidForPlayers(driver, bidPlayerDTO, profileDTO);
            }
            while (_wonPlayers < bidPlayerDTO.Count && endDate > DateTime.Now);

            return profileDTO;
        }

        private ProfileDTO TryBidForPlayers(IWebDriver driver, BuyCardDTO bidPlayerDTO, ProfileDTO profileDTO)
        {
            try
            {
                Thread.Sleep(500);
                var allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), 1000);
                if (allPlayers is null)
                {
                    var nextBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > div > button.flat.pagination.next"));
                    nextBtn?.Click();
                    var prevBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > div > button.flat.pagination.prev"));
                    prevBtn?.Click();
                    Thread.Sleep(1000);
                    allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), 1000);
                }

                var winningPlayers = allPlayers.Count(ap => ap.GetAttribute("class").Contains("won") || ap.GetAttribute("class").Contains("highest-bid"));
                if (winningPlayers + _wonPlayers >= bidPlayerDTO.Count) return profileDTO;

                if (allPlayers.All(p => p.GetAttribute("class").Contains("won") || p.GetAttribute("class").Contains("expired")))
                {
                    if (driver.GetCoins() < bidPlayerDTO.Price) return profileDTO;

                    var currentlyWon = allPlayers.Count(p => p.GetAttribute("class").Contains("won"));
                    _wonPlayers += currentlyWon;
                    profileDTO.WonTargetsCount += currentlyWon;
                    profileDTO.Coins = driver.GetCoins();

                    _updateAction(profileDTO);
                    
                    var nextBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > div > button.flat.pagination.next"));

                    nextBtn?.Click(); //outbid
                    Thread.Sleep(500);
                    var prevBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > div > button.flat.pagination.prev"));
                    prevBtn?.Click();
                    Thread.Sleep(1000);
                    allPlayers = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-pinned-list-container.SearchResults.ui-layout-left > div > ul > li"), 1000);
                }

                var outbidded = allPlayers.Count(p => p.GetAttribute("class").Contains("outbid"));
                profileDTO.Outbidded += outbidded;
                profileDTO.Coins = driver.GetCoins();

                _updateAction(profileDTO);

                var activePlayers = allPlayers.Where(p => !p.GetAttribute("class").Contains("won") && !p.GetAttribute("class").Contains("expired") && !p.GetAttribute("class").Contains("highest-bid"));
                foreach (var player in activePlayers)
                {
                    var startSpan = player.FindElement(By.CssSelector("div > div.auction > div.auctionStartPrice.auctionValue > span.currency-coins.value"));
                    if (!int.TryParse(startSpan?.Text.Replace(",", ""), out var startPrice)) continue;
                    if (startPrice > bidPlayerDTO.Price) continue;

                    var bidSpan = player.FindElement(By.CssSelector("div > div.auction > div:nth-child(2) > span.currency-coins.value"));
                    if (bidSpan is null) continue;

                    var bid = int.MaxValue;
                    _ = int.TryParse(bidSpan.Text.Replace(",", ""), out bid);
                    var remainingTime = player.FindElement(By.CssSelector("div > div.auction > div.auction-state > span.time")).Text;
                    if ((bidSpan.Text == "---" || bid < bidPlayerDTO.Price) && remainingTime.Contains("Seconds"))
                    {
                        player.Click();

                        var priceInput = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-navigation-container-view.ui-layout-right > div > div > div.DetailPanel > div.bidOptions > div > input"), 1000);
                        if (priceInput is null) continue;

                        var currentPrice = int.MaxValue;
                        _ = int.TryParse(priceInput.Text.Replace(",", ""), out currentPrice);

                        if (currentPrice <= bidPlayerDTO.Price && driver.GetCoins() > currentPrice)
                        {
                            var makeBidBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-navigation-container-view.ui-layout-right > div > div > div.DetailPanel > div.bidOptions > button.btn-standard.call-to-action.bidButton"));
                            makeBidBtn?.Click();

                            Thread.Sleep(50);

                            try
                            {
                                var errorMessage = driver.FindElement(By.CssSelector("#NotificationLayer > div > p"));
                                if (errorMessage is not null)
                                {
                                    _wonPlayers += allPlayers.Count(ap => ap.GetAttribute("class").Contains("won") || ap.GetAttribute("class").Contains("highest-bid"));
                                    // Should go to Transfer Targets and finish currently bidding items and continue
                                    driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-bar-view.navbar-style-landscape > button.ut-navigation-button-control")).Click();
                                    var searchBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div.ut-pinned-list-container.ut-content-container > div > div.button-container > button:nth-child(2)"), 1000);
                                    searchBtn?.Click();
                                }
                            }
                            catch { }

                            profileDTO.ActiveBidsCount += 1;
                            profileDTO.Coins = driver.GetCoins();
                            _updateAction(profileDTO);
                        }
                    }
                }
            }
            catch { }
            return profileDTO;
        }
    }
}