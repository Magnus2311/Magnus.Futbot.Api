using OpenQA.Selenium.Chrome;
using Titanium.Web.Proxy;

namespace Magnus.Futbot.Models
{
    public class DriverInstance
    {
        public DriverInstance(ChromeDriver driver, ProxyServer proxyServer)
        {
            Driver = driver;
            ProxyServer = proxyServer;
        }

        public ChromeDriver Driver { get; set; }

        public ProxyServer ProxyServer { get; }
    }
}