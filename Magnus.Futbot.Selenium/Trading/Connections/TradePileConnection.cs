using Magnus.Futbot.Common.Models.Selenium.Trading;
using Magnus.Futbot.Selenium.Trading.Connections.Base;
using Newtonsoft.Json;

namespace Magnus.Futbot.Selenium.Trading.Connections
{
    public class TradePileConnection : GetConnection
    {
        public TradePileConnection(HttpClient httpClient) : base(httpClient)
        {
            BaseAddress = new Uri("https://utas.external.s2.fut.ea.com/ut/game/fifa22/tradepile");
        }

        public async Task<TradePile?> GetTradePileAsync(string utsid)
        {
            UTSID = utsid;
            var response = await GetAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<TradePile>(await response.Content.ReadAsStringAsync());

            return null;
        }
    }
}
