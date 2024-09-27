using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futtbot.Connections.Enums;
using Magnus.Futtbot.Connections.Models;
using Magnus.Futtbot.Connections.Models.Responses;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Magnus.Futtbot.Connections.Connection.Trading
{
    public class TransferMarketCardsConnection
    {
        private HttpClient _httpClient;

        public TransferMarketCardsConnection(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ConnectionResponse<AvailableTransferMarketCards>> GetAvailableCardsByPlayerAndMaxPrice(ProfileDTO profileDTO, long playerEaId, int minBin, int maxBin)
        {
            Thread.Sleep(1000);
            Console.WriteLine($"Trying to get players for {playerEaId} with maxBin: {maxBin} on {DateTime.Now:dd:MM:yyyy hh:mm:ss)}");
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            _httpClient = new HttpClient(handler);

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://utas.mob.v4.prd.futc-ext.gcp.ea.com/ut/game/fc25/transfermarket?num=21&start=1&type=player&maskedDefId={playerEaId}&minb={minBin}&maxb={maxBin}");
            request.SetCommonHeaders(profileDTO.Email);
            var content =  new StringContent(string.Empty);
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

            if ((int)response.StatusCode == 521)
                return new(ConnectionResponseType.Unauthorized, null);

            if (!response.IsSuccessStatusCode)
            {
                Thread.Sleep(1000);
                return new(ConnectionResponseType.PauseForAWhile, null);
            }

            var availableCards = await response.Content.ReadFromJsonAsync<AvailableTransferMarketCards>();
            return new(ConnectionResponseType.Success, availableCards);
        }

        public async Task<ConnectionResponse<AvailableTransferMarketCards>> GetAvailableCardsByPlayer(ProfileDTO profileDTO, long playerEaId, int startingIndex)
        {
            Thread.Sleep(1000);
            Console.WriteLine($"Trying to get bid players for {playerEaId} on {DateTime.Now:dd:MM:yyyy hh:mm:ss)}");
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            _httpClient = new HttpClient(handler);

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://utas.mob.v4.prd.futc-ext.gcp.ea.com/ut/game/fc25/transfermarket?num=21&start={startingIndex}&type=player&maskedDefId={playerEaId}");
            request.SetCommonHeaders(profileDTO.Email);
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

            if ((int)response.StatusCode == 521)
                return new(ConnectionResponseType.Unauthorized, null);

            if (!response.IsSuccessStatusCode)
            {
                Thread.Sleep(1000);
                return new(ConnectionResponseType.PauseForAWhile, null);
            }

            var availableCards = await response.Content.ReadFromJsonAsync<AvailableTransferMarketCards>();
            return new(ConnectionResponseType.Success, availableCards);
        }
    }
}
