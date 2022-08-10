using System.IO.Compression;
using System.Net;
using System.Text.Json;


namespace Magnus.Futbot.Selenium.Trading
{
    public class Test
    {
        public async Task TradingTest()
        {
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

            HttpClient client = new(handler);

            var values = new Dictionary<string, string>
  {
      { "bid", "350" },
  };

            var baseAddress = new Uri("https://utas.external.s2.fut.ea.com/ut/game/fifa22/trade/343980889049/bid");

            var content = new StringContent(JsonSerializer.Serialize(new { bid = "350" }));
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Encoding", new List<string> { "gzip", "deflate", "br" });
            client.DefaultRequestHeaders.Add("Accept-Language", new List<string> { "en-US", "en;q=0.9", "bg;q=0.8" });
            client.DefaultRequestHeaders.Add("Connection", "keep-alive");
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Host", "utas.external.s2.fut.ea.com");
            client.DefaultRequestHeaders.Add("Origin", "https://www.ea.com");
            client.DefaultRequestHeaders.Add("Referer", "https://www.ea.com/");
            client.DefaultRequestHeaders.Add("X-UT-SID", "4826f738-82a9-4972-ab70-26571484bfba");

            var response = await client.PutAsync(baseAddress, content);
            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (Stream decompressed = new GZipStream(stream, CompressionMode.Decompress))
            using (StreamReader reader = new StreamReader(decompressed))
            {
                var responseString = reader.ReadToEnd();
                Console.WriteLine(responseString);
                Console.ReadLine();
            }

        }
    }
}
