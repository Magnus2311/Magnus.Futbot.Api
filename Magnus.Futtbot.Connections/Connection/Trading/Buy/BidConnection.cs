using Magnus.Futtbot.Connections.Enums;
using System.Net;
using System.Text.Json;

namespace Magnus.Futtbot.Connections.Connection.Trading.Buy
{
    public class BidConnection
    {
        private HttpClient _httpClient;

        public BidConnection(HttpClient httpClient)
        {
        }

        public async Task<ConnectionResponseType> BidPlayer(string username, long tradeId, int bidValue)
        {
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            _httpClient = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Put, $"https://utas.mob.v5.prd.futc-ext.gcp.ea.com/ut/game/fc26/trade/{tradeId}/bid");
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
