using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Models;
using OpenQA.Selenium.Chrome;
using System.Collections.Concurrent;

namespace Magnus.Futbot.Services
{
    public abstract class BaseService
    {
        private static readonly ConcurrentDictionary<string, DriverInstance> _chromeDrivers = new();
        private readonly IActionsService _actionsService;

        public BaseService(IActionsService actionsService)
        {
            _actionsService = actionsService;
        }

        public DriverInstance GetInstance(string username)
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
                    var tempDriverInstsance = new DriverInstance(tempDriver, _actionsService);
                    _chromeDrivers[username] = tempDriverInstsance;
                }

                return _chromeDrivers[username];
            }

            var chromeDriver = new ChromeDriver(chromeOptions);
            var driverInstsance = new DriverInstance(chromeDriver, _actionsService);
            _chromeDrivers.TryAdd(username, driverInstsance);
            return driverInstsance;
        }
    }
}
