using Magnus.Futbot.Models;
using OpenQA.Selenium.Chrome;
using System.Collections.Concurrent;

namespace Magnus.Futbot.Services
{
    public abstract class BaseService
    {
        private static readonly ConcurrentDictionary<string, DriverInstance> _chromeDrivers = new();

        public static DriverInstance GetInstance(string username)
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("--disable-backgrounding-occluded-windows");
            chromeOptions.AddArgument(@$"user-data-dir={Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\Google\Chrome\User Data\{username.Split("@").FirstOrDefault()}\Default");

            if (_chromeDrivers.ContainsKey(username))
            {
                try
                {
                    _ = _chromeDrivers[username]!.Driver!.Url;
                }
                catch
                {
                    var tempDriver = new ChromeDriver(chromeOptions);
                    var tempDriverInstsance = new DriverInstance(tempDriver);
                    _chromeDrivers[username] = tempDriverInstsance;
                }

                return _chromeDrivers[username];
            }

            var chromeDriver = new ChromeDriver(chromeOptions);
            var driverInstsance = new DriverInstance(chromeDriver);
            _chromeDrivers.TryAdd(username, driverInstsance);
            return driverInstsance;
        }
    }
}
