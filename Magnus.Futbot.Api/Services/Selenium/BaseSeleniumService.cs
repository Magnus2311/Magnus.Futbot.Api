using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Magnus.Futbot.Api.Services.Selenium
{
    public class BaseSeleniumService
    {
        protected WebDriver webDriver;

        public BaseSeleniumService()
        {
            webDriver = new ChromeDriver();
        }
    }
}
