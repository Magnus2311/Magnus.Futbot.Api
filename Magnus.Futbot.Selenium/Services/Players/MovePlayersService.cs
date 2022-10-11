using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Players
{
    public class MovePlayersService : BaseService
    {
        public void SendTransferTargetsToTransferList(ProfileDTO profileDTO, Action<ProfileDTO> updateProfile)
        {
            var driverInstance = GetInstance(profileDTO.Email);
            var tknSrc = new CancellationTokenSource();

            var moveAction = new MoveAction(new Func<Task>(async () =>
            { 
                if (!driverInstance.Driver.Url.Contains("https://www.ea.com/fifa/ultimate-team/web-app/"))
                {
                    LoginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                }

                await driverInstance.Driver.OpenTransferTargets();

                var players = driverInstance.Driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > div > section:nth-child(3) > ul > li"));
                foreach (var player in players)
                {
                    player.Click();

                    var sendToTransferList = driverInstance.Driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-button-group > button:nth-child(8)"));
                    if (sendToTransferList != null && sendToTransferList.Enabled && sendToTransferList.Displayed)
                    {
                        sendToTransferList.Click();
                        profileDTO.WonTargetsCount--;
                        profileDTO.TransferListCount++;

                        // More logic for altering Trasnfer Pile should be added
                        updateProfile(profileDTO);
                    }
                }
            }), tknSrc, "Move items from Transfer Target to Transfer List");

            driverInstance.AddAction(moveAction);
        }

        public void SendUnassignedItemsToTransferList(ProfileDTO profileDTO, Action<ProfileDTO> updateProfile)
        {
            var driverInstance = GetInstance(profileDTO.Email);
            var tknSrc = new CancellationTokenSource();

            var moveAction = new MoveAction(new Func<Task>(async () =>
            {
                if (!driverInstance.Driver.Url.Contains("https://www.ea.com/fifa/ultimate-team/web-app/"))
                {
                    LoginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                }

                await driverInstance.Driver.OpenUnassignedItems(profileDTO);

                var players = driverInstance.Driver.FindElements(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-unassigned-view.ui-layout-left > section > ul > li"));
                foreach (var player in players)
                {
                    player.Click();

                    var sendToTransferList = driverInstance.Driver.TryFindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section > div > div > div.DetailPanel > div.ut-button-group > button:nth-child(8)"));
                    if (sendToTransferList != null && sendToTransferList.Enabled && sendToTransferList.Displayed)
                    {
                        sendToTransferList.Click();
                        profileDTO.WonTargetsCount--;
                        profileDTO.TransferListCount++;

                        // More logic for altering Trasnfer Pile should be added
                        updateProfile(profileDTO);
                    }
                }
            }), tknSrc, "Move Unassigned items to Transfer List");

            driverInstance.AddAction(moveAction);
        }
    }
}
