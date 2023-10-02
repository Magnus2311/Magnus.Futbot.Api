using OpenQA.Selenium.Chrome;

namespace Magnus.Futbot.Models
{
    public class DriverInstance
    {
        public DriverInstance(ChromeDriver driver)
        {
            Driver = driver;
        }

        public ChromeDriver Driver { get; set; }
    }
}