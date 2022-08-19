using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Trading;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Players
{
    public class FullPlayersDataService : BaseService
    {
        public static ProfileDTO FetchTradingPlayers(ProfileDTO profile)
        {
            return profile;
        }

        public static IEnumerable<PlayerCard> GetTransferListCards(ProfileDTO profileDTO)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferList();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));

            foreach (var player in players)
            {
                yield return player.ConvertPlayerElementToPlayerCard();
            }
        }
    }
}
