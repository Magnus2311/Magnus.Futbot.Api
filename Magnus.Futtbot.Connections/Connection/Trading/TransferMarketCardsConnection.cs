using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Services;
using Magnus.Futtbot.Connections.Models;
using Magnus.Futtbot.Connections.Utils;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Magnus.Futtbot.Connections.Connection.Trading
{
    public class TransferMarketCardsConnection
    {
        private HttpClient _httpClient;
        private readonly LoginSeleniumService _loginSeleniumService;

        public TransferMarketCardsConnection(HttpClient httpClient, LoginSeleniumService loginSeleniumService)
        {
            _httpClient = httpClient;
            _loginSeleniumService = loginSeleniumService;
        }

        public async Task<AvailableTransferMarketCards?> GetAvailableCardsByPlayer(ProfileDTO profileDTO, long playerEaId, int maxBin)
        {
            if (!EaData.UserXUTSIDs.ContainsKey(profileDTO.Email))
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            HttpClientHandler handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            _httpClient = new HttpClient(handler);

            var request = new HttpRequestMessage(HttpMethod.Get, $"https://utas.mob.v2.fut.ea.com/ut/game/fc24/transfermarket?num=21&start=1&type=player&maskedDefId={playerEaId}&maxb={maxBin}");
            request.Headers.Add("Accept", " */*");
            request.Headers.Add("Accept-Encoding", " gzip, deflate, br");
            request.Headers.Add("Accept-Language", " en-US,en;q=0.9");
            request.Headers.Add("Cache-Control", " no-cache");
            request.Headers.Add("Connection", " keep-alive");
            request.Headers.Add("Host", " utas.mob.v2.fut.ea.com");
            request.Headers.Add("Origin", " https://www.ea.com");
            request.Headers.Add("Referer", " https://www.ea.com/");
            request.Headers.Add("Sec-Fetch-Dest", " empty");
            request.Headers.Add("Sec-Fetch-Mode", " cors");
            request.Headers.Add("Sec-Fetch-Site", " same-site");
            request.Headers.Add("User-Agent", " Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");
            request.Headers.Add("X-UT-SID", EaData.UserXUTSIDs[profileDTO.Email]);
            request.Headers.Add("sec-ch-ua", " \"Google Chrome\";v=\"117\", \"Not;A=Brand\";v=\"8\", \"Chromium\";v=\"117\"");
            request.Headers.Add("sec-ch-ua-mobile", " ?0");
            request.Headers.Add("sec-ch-ua-platform", " \"Windows\"");
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                return await GetAvailableCardsByPlayer(profileDTO, playerEaId, maxBin);
            }

            var contentStr = await response.Content.ReadAsStringAsync();

            return await response.Content.ReadFromJsonAsync<AvailableTransferMarketCards>();
        }
    }
}
