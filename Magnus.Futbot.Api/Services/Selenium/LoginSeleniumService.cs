using Magnus.Futbot.Api.Helpers;
using Magnus.Futbot.Api.Models.DTOs;
using OpenQA.Selenium;

namespace Magnus.Futbot.Api.Services.Selenium
{
    public class LoginSeleniumService : BaseSeleniumService
    {
        public LoginResponseDTO Login(string username, string password)
        {
            var driverInstance = GetInstance(username);
            var driver = driverInstance.Driver;
            driverInstance.Driver.Navigate().GoToUrl("https://www.ea.com/fifa/ultimate-team/web-app/");
            Thread.Sleep(4000);
            IWebElement? element = null;
            do
            {
                try
                {
                    element = driver.FindElement(By.CssSelector("#Login > div > div > button.btn-standard.call-to-action"));
                }
                catch { }
            }
            while (!(element != null && element.Displayed && element.Enabled));
            element.Click();
            Thread.Sleep(2000);

            IWebElement emailInput = driver.FindElement(By.CssSelector("#email"));
            emailInput.SendKeys(username);

            IWebElement passwordInput = driver.FindElement(By.CssSelector("#password"));
            passwordInput.SendKeys(password);

            IWebElement rememberMeInput = driver.FindElement(By.CssSelector("#rememberMe"));
            if (!rememberMeInput.Selected) rememberMeInput.Click();

            IWebElement signInButton = driver.FindElement(By.CssSelector("#logInBtn"));
            signInButton.Click();
            Thread.Sleep(1500);

            IWebElement? wrongCredentials = null;
            try
            {
                wrongCredentials = driver.FindElement(By.CssSelector("#online-general-error > p"));
            }
            catch { }

            if (wrongCredentials != null) return new LoginResponseDTO(LoginStatusType.WrongCredentials);

            IWebElement? securityCodeRequired = null;
            try
            {
                securityCodeRequired = driver.FindElement(By.CssSelector("#page_header"));
            }
            catch { }

            if (securityCodeRequired != null)
            {
                IWebElement sendCodeBtn = driver.FindElement(By.CssSelector("#btnSendCode"));
                sendCodeBtn.Click();
                return new LoginResponseDTO(LoginStatusType.ConfirmationKeyRequired);
            }


            return new LoginResponseDTO(LoginStatusType.Successful);
        }

        public ConfirmationCodeResponseDTO SubmitCode(string username, string code)
        {
            var driver = GetInstance(username).Driver;

            var codeInput = driver.FindElement(By.CssSelector("#twoFactorCode"));
            if (codeInput != null)
            {
                codeInput.SendKeys(code);
                Thread.Sleep(500);

                var rememberDeviceCheck = driver.FindElement(By.CssSelector("#trustThisDevice"));
                if (rememberDeviceCheck != null && !rememberDeviceCheck.Selected)
                    rememberDeviceCheck.Click();

                var signInBtn = driver.FindElement(By.CssSelector("#btnSubmit"));
                signInBtn?.Click();
                Thread.Sleep(1500);

                var errMessage = driver.FindElement(By.CssSelector("#online-general-error > p"));
                if (errMessage != null) return new ConfirmationCodeResponseDTO(ConfirmationCodeStatusType.WrongCode);
            }

            return new ConfirmationCodeResponseDTO(ConfirmationCodeStatusType.Successful);
        }
    }
}
