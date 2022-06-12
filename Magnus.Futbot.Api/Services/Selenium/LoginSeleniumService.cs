using Magnus.Futbot.Api.Helpers;
using Magnus.Futbot.Api.Models.DTOs;
using OpenQA.Selenium;

namespace Magnus.Futbot.Api.Services.Selenium
{
    public class LoginSeleniumService : BaseSeleniumService
    {
        public LoginResponseDTO Login(string username, string password)
        {

            webDriver.Navigate().GoToUrl("https://www.ea.com/fifa/ultimate-team/web-app/");
            Thread.Sleep(4000);
            IWebElement? element = null;
            do
            {
                try
                {
                    element = webDriver.FindElement(By.CssSelector("#Login > div > div > button.btn-standard.call-to-action"));
                }
                catch { }
            }
            while (!(element != null && element.Displayed && element.Enabled));
            element.Click();
            Thread.Sleep(2000);

            IWebElement emailInput = webDriver.FindElement(By.CssSelector("#email"));
            emailInput.SendKeys(username);

            IWebElement passwordInput = webDriver.FindElement(By.CssSelector("#password"));
            passwordInput.SendKeys(password);

            IWebElement rememberMeInput = webDriver.FindElement(By.CssSelector("#rememberMe"));
            if (!rememberMeInput.Selected) rememberMeInput.Click();

            IWebElement signInButton = webDriver.FindElement(By.CssSelector("#logInBtn"));
            signInButton.Click();
            Thread.Sleep(1500);

            IWebElement? wrongCredentials = null;
            try
            {
                wrongCredentials = webDriver.FindElement(By.CssSelector("#online-general-error > p"));
            }
            catch { }

            if (wrongCredentials != null) return new LoginResponseDTO(LoginStatusType.WrongCredentials);

            IWebElement? securityCodeRequired = null;
            try
            {
                securityCodeRequired = webDriver.FindElement(By.CssSelector("#page_header"));
            }
            catch { }

            if (securityCodeRequired != null)
            {
                IWebElement sendCodeBtn = webDriver.FindElement(By.CssSelector("#btnSendCode"));
                sendCodeBtn.Click();
                return new LoginResponseDTO(LoginStatusType.ConfirmationKeyRequired);
            }


            return new LoginResponseDTO(LoginStatusType.Successful);
        }
    }
}
