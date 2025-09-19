using Magnus.Futbot.Common.fcmodels;
using Magnus.Futtbot.Connections.Enums;
using Magnus.Futtbot.Connections.Models.Responses;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Magnus.Futtbot.Connections.Connection
{
    public class GetUserPileConnection
    {
        private HttpClient _httpClient;

        public GetUserPileConnection(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ConnectionResponse<TradePileData>> GetUserTradePile(string username)
        {
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            _httpClient = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://utas.mob.v5.prd.futc-ext.gcp.ea.com/ut/game/fc26/tradepile");
            request.SetCommonHeaders(username);
            var content = new StringContent(string.Empty);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.UnavailableForLegalReasons
                || response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Thread.Sleep(1000);
                return new(ConnectionResponseType.PauseForAWhile, null);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new(ConnectionResponseType.Unauthorized, null);

            if (!response.IsSuccessStatusCode)
            {
                Thread.Sleep(1000);
                return new(ConnectionResponseType.PauseForAWhile, null);
            }

            var availableCards = await response.Content.ReadFromJsonAsync<TradePileData>();
            return new(ConnectionResponseType.Success, availableCards);
        }

        public async Task<ConnectionResponse<UnassignedData>> GetUnassignedItems(string username)
        {
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            _httpClient = new HttpClient(handler);
            var request = new HttpRequestMessage(HttpMethod.Get, "https://utas.mob.v5.prd.futc-ext.gcp.ea.com/ut/game/fc26/purchased/items");
            request.SetCommonHeaders(username);
            var content = new StringContent(string.Empty);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.UnavailableForLegalReasons
                || response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Thread.Sleep(1000);
                return new(ConnectionResponseType.PauseForAWhile, null);
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return new(ConnectionResponseType.Unauthorized, null);

            if (!response.IsSuccessStatusCode)
            {
                Thread.Sleep(1000);
                return new(ConnectionResponseType.PauseForAWhile, null);
            }

            var unassignedCards = await response.Content.ReadFromJsonAsync<UnassignedData>();
            return new(ConnectionResponseType.Success, unassignedCards);
        }
    }
}