using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Trade.Sell
{
    public class SellService : BaseService
    {
        public ProfileDTO SellPlayer(SellCardDTO sellCard, ProfileDTO profileDTO, Action<ProfileDTO> updateProfile)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferList();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));
            players.ToList().AddRange(driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(2) > ul > li")));

            try
            {
                var martchingPlayers = players.Where((p) =>
                {
                    var (Name, Rating) = p.GetCardNameAndRating();
                    return sellCard.Card.Name.Contains(Name)
                        && sellCard.Card.Rating == Rating;
                }).Take(sellCard.Count); 
                
                foreach (var player in martchingPlayers)
                {
                    player.Click();

                    driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.ut-button-group > button"))
                        .Click();
                    Thread.Sleep(300);
                    var bidPrice = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > div:nth-child(2) > div.ut-numeric-input-spinner-control > input"));
                    bidPrice.Click();
                    bidPrice.SendKeys($"{sellCard.FromBid}");
                    Thread.Sleep(100);

                    var binPrice = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > div:nth-child(3) > div.ut-numeric-input-spinner-control > input"));
                    binPrice.Click();
                    binPrice.SendKeys($"{sellCard.FromBin}");
                    Thread.Sleep(100);

                    var listBtn = driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-quick-list-panel-view > div.panelActions.open > button"));
                    listBtn.Click();
                    Thread.Sleep(100);

                    updateProfile(profileDTO);
                }
            }
            catch
            {
                SellPlayer(sellCard, profileDTO, updateProfile);
            }

            return profileDTO;
        }
    }
}
