using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using OpenQA.Selenium;

namespace Magnus.Futbot.Services
{
    public class LoginSeleniumService : BaseService
    {
        public static ProfileStatusType Login(string username, string password)
        {
            var driverInstance = GetInstance(username);
            var driver = driverInstance.Driver;
            driverInstance.Driver.Navigate().GoToUrl("https://www.ea.com/fifa/ultimate-team/web-app/");
            Thread.Sleep(4000);
            IWebElement? loginBtn = null;
            do
            {
                try
                {
                    loginBtn = driver.FindElement(By.CssSelector("#Login > div > div > button.btn-standard.call-to-action"));
                }
                catch { }
            }
            while (!(loginBtn != null && loginBtn.Displayed && loginBtn.Enabled));
            loginBtn.Click();
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

            if (wrongCredentials != null) return ProfileStatusType.WrongCredentials;

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
                return ProfileStatusType.ConfirmationKeyRequired;
            }


            return ProfileStatusType.Logged;
        }

        public static ConfirmationCodeStatusType SubmitCode(SubmitCodeDTO submitCodeDTO)
        {
            var driver = GetInstance(submitCodeDTO.Email).Driver;

            var codeInput = driver.FindElement(By.CssSelector("#twoFactorCode"));
            if (codeInput != null)
            {
                codeInput.SendKeys(submitCodeDTO.Code);
                Thread.Sleep(500);

                var rememberDeviceCheck = driver.FindElement(By.CssSelector("#trustThisDevice"));
                if (rememberDeviceCheck != null && !rememberDeviceCheck.Selected)
                    rememberDeviceCheck.Click();

                var signInBtn = driver.FindElement(By.CssSelector("#btnSubmit"));
                signInBtn?.Click();
                Thread.Sleep(1500);

                try
                {
                    var errMessage = driver.FindElement(By.CssSelector("#online-general-error > p"));
                    if (errMessage != null) return ConfirmationCodeStatusType.WrongCode;
                }
                catch { }
            }

            return ConfirmationCodeStatusType.Successful;
        }
    }
}
