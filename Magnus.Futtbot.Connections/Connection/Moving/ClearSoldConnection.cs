using Magnus.Futtbot.Connections.Enums;
using System.Net;

namespace Magnus.Futtbot.Connections.Connection.Moving
{
    public class ClearSoldConnection
    {
        private readonly HttpClient _httpClient;

        public ClearSoldConnection(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ConnectionResponseType> ClearSold(string username)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, "https://utas.mob.v2.fut.ea.com/ut/game/fc24/trade/sold");
            request.SetCommonHeaders(username);
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.UnavailableForLegalReasons
                || response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                Thread.Sleep(1000);
                return ConnectionResponseType.PauseForAWhile;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                return ConnectionResponseType.Unauthorized;

            if (!response.IsSuccessStatusCode)
                return ConnectionResponseType.Unknown;

            return ConnectionResponseType.Success;
        }
    }
}
