using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using Magnus.Futbot.Services;
using OpenQA.Selenium;

namespace Magnus.Futbot.Selenium.Services.Players
{
    public class MovePlayersService : BaseService
    {
        private readonly LoginSeleniumService _loginSeleniumService;
        private readonly UserActionsService _userActionsService;

        public MovePlayersService(
            IActionsService actionsService,
            LoginSeleniumService loginSeleniumService,
            UserActionsService userActionsService) : base(actionsService)
        {
            _loginSeleniumService = loginSeleniumService;
            _userActionsService = userActionsService;
        }

        public void SendTransferTargetsToTransferList(ProfileDTO profileDTO, Action<ProfileDTO> updateProfile)
        {
            var driverInstance = GetInstance(profileDTO.Email);
            var tknSrc = new CancellationTokenSource();

            var moveAction = new MoveAction(profileDTO.Id, new Func<Task>(async () =>
            {
                if (!driverInstance.Driver.Url.Contains("https://www.ea.com/ea-sports-fc/ultimate-team/web-app/"))
                {
                    await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
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

            _userActionsService.AddAction(profileDTO.Email, moveAction);
        }

        public void SendUnassignedItemsToTransferList(ProfileDTO profileDTO, Action<ProfileDTO> updateProfile)
        {
            var driverInstance = GetInstance(profileDTO.Email);
            var tknSrc = new CancellationTokenSource();

            var moveAction = new MoveAction(profileDTO.Id, new Func<Task>(async () =>
            {
                if (!driverInstance.Driver.Url.Contains("https://www.ea.com/ea-sports-fc/ultimate-team/web-app/"))
                {
                    await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                }

                await driverInstance.Driver.OpenUnassignedItems(profileDTO);

                var circleButton = driverInstance.Driver.FindElement(By.CssSelector("body > main > section > section > div.ut-navigation-container-view--content > div > div > section.ut-unassigned-view.ui-layout-left > section > header > button.ut-image-button-control.filter-btn"));
                circleButton?.Click();

                var sendAllToTransferList = driverInstance.Driver.FindElement(By.CssSelector("body > div.view-modal-container.form-modal > div > div > button:nth-child(2)"));
                sendAllToTransferList?.Click();

                var okBtn = driverInstance.Driver.FindElement(By.CssSelector("body > div.view-modal-container.form-modal > section > div > div > button:nth-child(1)"));
                okBtn?.Click();
            }), tknSrc, "Move Unassigned items to Transfer List");

            _userActionsService.AddAction(profileDTO.Email, moveAction);
        }
    }
}
