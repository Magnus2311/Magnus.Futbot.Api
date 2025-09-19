using Magnus.Futtbot.Connections.Enums;
using Magnus.Futtbot.Connections.Models.Requests;
using Magnus.Futtbot.Connections.Utils;
using System.Net;
using System.Text.Json;

namespace Magnus.Futtbot.Connections.Connection.Moving
{
    public class SendItemsConnection
    {
        private readonly HttpClient _httpClient;

        public SendItemsConnection(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ConnectionResponseType> SendWonItemsToTransferList(string username, SendCardsToTransferListRequest sendCardsToTransferListRequest)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "https://utas.mob.v5.prd.futc-ext.gcp.ea.com/ut/game/fc26/item");
            request.SetCommonHeaders(username);
            var content = new StringContent(JsonSerializer.Serialize(sendCardsToTransferListRequest), null, "application/json");
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

            if (!response.IsSuccessStatusCode)
                return ConnectionResponseType.Unknown;

            return ConnectionResponseType.Success;
        }
    }
}
