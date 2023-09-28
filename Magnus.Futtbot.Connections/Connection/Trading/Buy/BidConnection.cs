using Magnus.Futtbot.Connections.Utils;
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

        public async Task BidPlayer(string username, long tradeId, int bidValue)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"https://utas.mob.v2.fut.ea.com/ut/game/fc24/trade/{tradeId}/bid");
            request.Headers.Add("Accept", " */*");
            request.Headers.Add("Accept-Encoding", " gzip, deflate, br");
            request.Headers.Add("Accept-Language", " en-US,en;q=0.9");
            request.Headers.Add("Connection", " keep-alive");
            request.Headers.Add("Content-Length", " 11");
            request.Headers.Add("Host", " utas.mob.v2.fut.ea.com");
            request.Headers.Add("Origin", " https://www.ea.com");
            request.Headers.Add("Referer", " https://www.ea.com/");
            request.Headers.Add("Sec-Fetch-Dest", " empty");
            request.Headers.Add("Sec-Fetch-Mode", " cors");
            request.Headers.Add("Sec-Fetch-Site", " same-site");
            request.Headers.Add("User-Agent", " Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");
            request.Headers.Add("X-UT-SID", EaData.UserXUTSIDs[username]);
            request.Headers.Add("sec-ch-ua", " \"Google Chrome\";v=\"117\", \"Not;A=Brand\";v=\"8\", \"Chromium\";v=\"117\"");
            request.Headers.Add("sec-ch-ua-mobile", " ?0");
            request.Headers.Add("sec-ch-ua-platform", " \"Windows\"");

            var bidObj = new
            {
                bid = bidValue
            };

            var content = new StringContent(JsonSerializer.Serialize(bidObj), null, " application/json");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
        }
    }
}
