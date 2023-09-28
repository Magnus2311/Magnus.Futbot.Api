using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Models;
using OpenQA.Selenium.Chrome;
using System.Collections.Concurrent;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using Magnus.Futbot.Common.Events;

namespace Magnus.Futbot.Services
{
    public abstract class BaseService
    {
        private static readonly ConcurrentDictionary<string, DriverInstance> _chromeDrivers = new();
        private readonly IActionsService _actionsService;
        public static event EventHandler<XUTSIDUpdatedEventArgs> HeaderReceived;

        public BaseService(IActionsService actionsService)
        {
            _actionsService = actionsService;
        }

        public DriverInstance GetInstance(string username)
        {
            var proxyServer = new ProxyServer();

            // Set up the endpoint
            var explicitEndPoint = new ExplicitProxyEndPoint(System.Net.IPAddress.Any, 18882, true);
            proxyServer.AddEndPoint(explicitEndPoint);
            proxyServer.Start();

            proxyServer.BeforeRequest += async (sender, e) => await OnRequest(sender, e, username);

            // Set up proxy for Selenium
            var proxy = new OpenQA.Selenium.Proxy()
            {
                HttpProxy = "http://localhost:18882",
                SslProxy = "http://localhost:18882",
                FtpProxy = "http://localhost:18882"
            };

            var chromeOptions = new ChromeOptions
            {
                Proxy = proxy
            };

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


        public static async Task OnRequest(object sender, SessionEventArgs e, string username)
        {
            foreach (var header in e.HttpClient.Request.Headers)
            {
                if (header?.Name.ToUpperInvariant() == "X-UT-SID")
                    EventAggregator.Instance.OnXUTSIDUpdated(sender, new XUTSIDUpdatedEventArgs(username, header.Value));
            }
        }
    }
}
