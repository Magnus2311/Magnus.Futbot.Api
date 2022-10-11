using Magnus.Futbot.Common.Models.DTOs;
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

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));
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

        public ProfileDTO SendUnassignedItemsToTransferList(ProfileDTO profileDTO, Action<ProfileDTO> updateProfile)
        {
            var driver = GetInstance(profileDTO.Email).Driver;
            driver.OpenUnassignedItems(profileDTO);

            var players = driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-unassigned-view.ui-layout-left > section > ul > li"));
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
    }
}
