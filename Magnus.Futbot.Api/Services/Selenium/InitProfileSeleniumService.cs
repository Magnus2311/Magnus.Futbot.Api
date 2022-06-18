using Magnus.Futbot.Api.Models.DTOs;
using Magnus.Futbot.Api.Models.DTOs.Responses;
using OpenQA.Selenium;

namespace Magnus.Futbot.Api.Services.Selenium
{
    public class InitProfileSeleniumService : BaseSeleniumService
    {
        private readonly LoginSeleniumService _loginSeleniumService;

        public InitProfileSeleniumService(LoginSeleniumService loginSeleniumService)
        {
            _loginSeleniumService = loginSeleniumService;
        }

        public InitProfileResponse InitProfile(ProfileDTO profile)
        {
            var driverInstance = GetInstance(profile.Email);
            var driver = driverInstance.Driver;
            driverInstance.Driver.Navigate().GoToUrl("https://www.ea.com/fifa/ultimate-team/web-app/");

            IWebElement? loginBtn = driver.FindElement(By.CssSelector("#Login > div > div > button.btn-standard.call-to-action"), 6000);
            loginBtn?.Click();
            Thread.Sleep(5000);

            var emailInput = driver.FindElement(By.CssSelector("#email"), 1000);
            if (emailInput is not null)
            {
                var loginResponse = _loginSeleniumService.Login(profile.Email, profile.Password);
                return new InitProfileResponse(loginResponse.LoginStatus);
            }

            var transferBtn = driver.FindElement(By.CssSelector("body > main > section > nav > button.ut-tab-bar-item.icon-transfer"), 10000);
            if (transferBtn is not null) return new InitProfileResponse(Common.ProfileStatusType.Logged);

            return new InitProfileResponse(Common.ProfileStatusType.CaptchaNeeded);
        }
    }
}