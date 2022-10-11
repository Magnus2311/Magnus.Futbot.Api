using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.Selenium.Profiles;
using OpenQA.Selenium;

namespace Magnus.Futbot.Services
{
    public class LoginSeleniumService : BaseService
    {
        public static async Task<ProfileStatusType> Login(string username, string password)
        {
            var driverInstance = GetInstance(username);
            var driver = driverInstance.Driver;
            driverInstance.Driver.Navigate().GoToUrl("https://www.ea.com/fifa/ultimate-team/web-app/");
            IWebElement? loginBtn = null;
            try
            {
                do
                {
                    try
                    {
                        loginBtn = driver.FindElement(By.CssSelector("#Login > div > div > button.btn-standard.call-to-action"), TimeSpan.FromSeconds(10));
                    }
                    catch { }
                }
                while (!(loginBtn != null && loginBtn.Displayed && loginBtn.Enabled));
                loginBtn.Click();
                await Task.Delay(2000);
            }
            catch (StaleElementReferenceException)
            {
                return ProfileStatusType.Logged;
            }

            IWebElement emailInput = driver.FindElement(By.CssSelector("#email"));
            emailInput.SendKeys(username);

            IWebElement passwordInput = driver.FindElement(By.CssSelector("#password"));
            passwordInput.SendKeys(password);

            IWebElement rememberMeInput = driver.FindElement(By.CssSelector("#rememberMe"));
            if (!rememberMeInput.Selected) rememberMeInput.Click();

            IWebElement signInButton = driver.FindElement(By.CssSelector("#logInBtn"));
            signInButton.Click();

            IWebElement? wrongCredentials = null;
            try
            {
                wrongCredentials = driver.FindElement(By.CssSelector("#online-general-error > p"), TimeSpan.FromSeconds(2));
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

        public static async Task<ConfirmationCodeStatusType> SubmitCode(SubmitCodeDTO submitCodeDTO)
        {
            var driver = GetInstance(submitCodeDTO.Email).Driver;

            var codeInput = driver.FindElement(By.CssSelector("#twoFactorCode"));
            if (codeInput != null)
            {
                codeInput.SendKeys(submitCodeDTO.Code);
                await Task.Delay(500);

                var rememberDeviceCheck = driver.FindElement(By.CssSelector("#trustThisDevice"));
                if (rememberDeviceCheck != null && !rememberDeviceCheck.Selected)
                    rememberDeviceCheck.Click();

                var signInBtn = driver.FindElement(By.CssSelector("#btnSubmit"));
                signInBtn?.Click();
                await Task.Delay(1500);

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
