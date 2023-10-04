using Magnus.Futtbot.Connections.Enums;
using System.Net;
using System.Text.Json;

namespace Magnus.Futtbot.Connections.Connection.Trading.Buy
{
    public class BidConnection
    {
        private readonly HttpClient _httpClient;

        public BidConnection(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ConnectionResponseType> BidPlayer(string username, long tradeId, int bidValue)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"https://utas.mob.v2.fut.ea.com/ut/game/fc24/trade/{tradeId}/bid");
            request.SetCommonHeaders(username);

            var bidObj = new
            {
                bid = bidValue
            };

            var content = new StringContent(JsonSerializer.Serialize(bidObj), null, "application/json");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == HttpStatusCode.UnavailableForLegalReasons
                || response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Thread.Sleep(1000);
                return ConnectionResponseType.PauseForAWhile;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return ConnectionResponseType.Unauthorized;

            if (response.StatusCode == HttpStatusCode.UpgradeRequired)
                return ConnectionResponseType.UpgradeRequired;

            if (!response.IsSuccessStatusCode)
                return ConnectionResponseType.PauseForAWhile;

            return ConnectionResponseType.Success;
        }
    }
}
