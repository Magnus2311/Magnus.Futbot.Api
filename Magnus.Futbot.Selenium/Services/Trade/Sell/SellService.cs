using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Services;

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

            var martchingPlayers = players.Where((p) =>
            {
                var (Name, Rating) = p.GetCardNameAndRating();
                return sellCard.Card.Name.Contains(Name)
                    && sellCard.Card.Rating == Rating;
            });

            foreach (var player in martchingPlayers)
            {
                player.Click();
                // Add logic for inserting coins

                updateProfile(profileDTO);
            }

            return profileDTO;
        }
    }
}
