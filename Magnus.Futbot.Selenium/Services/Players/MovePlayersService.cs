using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Players
{
    public class MovePlayersService : BaseService
    {
        public ProfileDTO SendTransferTargetsToTransferList(ProfileDTO profileDTO, Action<ProfileDTO> updateProfile)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenTransferTargets();

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li.listFUTItem.has-auction-data.won.selected > div > div.entityContainer > div.player-stats-data-component > ul > li"));
            foreach (var player in players)
            {
                player.Click();

                var sendToTransferList = driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-button-group > button:nth-child(8)"));
                if (sendToTransferList != null && sendToTransferList.Enabled && sendToTransferList.Displayed)
                {
                    sendToTransferList.Click();
                    profileDTO.WonTargetsCount--;
                    profileDTO.TransferListCount++;

                    // More logic for altering Trasnfer Pile should be added
                    updateProfile(profileDTO);
                }
            }

            return profileDTO;
        }

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
