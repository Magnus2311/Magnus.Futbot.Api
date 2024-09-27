using Magnus.Futbot.Common.Interfaces.Services;
using Magnus.Futbot.Models;
using OpenQA.Selenium.Chrome;
using System.Collections.Concurrent;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using Magnus.Futbot.Common.Events;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;

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
            var chromeOptions = new ChromeOptions();

            var port = 18882;
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
                    var ps = new ProxyServer();

                    // Set up the endpoint
                    var eep = new ExplicitProxyEndPoint(System.Net.IPAddress.Any, port + _chromeDrivers.Count, true);
                    ps.AddEndPoint(eep);
                    ps.Start();

                    ps.BeforeRequest += async (sender, e) => await OnRequest(sender, e);

                    // Set up proxy for Selenium
                    var p = new OpenQA.Selenium.Proxy()
                    {
                        HttpProxy = $"http://localhost:{port + _chromeDrivers.Count}",
                        SslProxy = $"http://localhost:{port + _chromeDrivers.Count}",
                        FtpProxy = $"http://localhost:{port + _chromeDrivers.Count}"
                    };
                    chromeOptions.Proxy = p;
                    var tempDriver = new ChromeDriver(chromeOptions);
                    var tempDriverInstsance = new DriverInstance(tempDriver, ps);
                    _chromeDrivers[username] = tempDriverInstsance;
                }

                return _chromeDrivers[username];
            }

            var proxyServer = new ProxyServer();

            // Set up the endpoint
            var eep2 = new ExplicitProxyEndPoint(System.Net.IPAddress.Any, port + _chromeDrivers.Count, true);
            proxyServer.AddEndPoint(eep2);
            proxyServer.Start();

            proxyServer.BeforeRequest += async (sender, e) => await OnRequest(sender, e);

            // Set up proxy for Selenium
            var proxy = new OpenQA.Selenium.Proxy()
            {
                HttpProxy = $"http://localhost:{port + _chromeDrivers.Count}",
                SslProxy = $"http://localhost:{port + _chromeDrivers.Count}",
                FtpProxy = $"http://localhost:{port + _chromeDrivers.Count}"
            };

            chromeOptions.Proxy = proxy;

            new DriverManager().SetUpDriver(new ChromeConfig());

            var chromeDriver = new ChromeDriver(chromeOptions);
            var driverInstsance = new DriverInstance(chromeDriver, proxyServer);
            _chromeDrivers.TryAdd(username, driverInstsance);
            AttachOnRequestHandler(driverInstsance, username);
            return driverInstsance;
        }

        public async Task OnRequest(object sender, SessionEventArgs e)
        {
            var username = e.UserData as string;
            if (username == null) return;

            foreach (var header in e.HttpClient.Request.Headers)
            {
                if (header?.Name.ToUpperInvariant() == "X-UT-SID")
                    EventAggregator.Instance.OnXUTSIDUpdated(sender, new XUTSIDUpdatedEventArgs(username, header.Value));
            }
        }

        public void AttachOnRequestHandler(DriverInstance driverInstance, string username)
        {
            driverInstance.ProxyServer.BeforeRequest += async (s, e) =>
            {
                e.UserData = username;
                await OnRequest(s, e);
            };
        }
    }
}
