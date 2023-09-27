using Magnus.Futtbot.Connections.Constants;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace Magnus.Futtbot.Connections.Connection.Auth
{
    public class Login
    {
        public static HttpClient _httpClient;

        public static void Main()
        {
            _httpClient = new HttpClient();

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            _httpClient.DefaultRequestHeaders.Add("DNT", "1");
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        }

        public async Task LoginAsync()
        {

            var response = await _httpClient.GetAsync(Urls.Main);

            if (response == null || !response.IsSuccessStatusCode)
                return;

            var responseStr = await response.Content.ReadAsStringAsync();

            if (responseStr.IndexOf("<title>FIFA Football | FUT Web App | EA SPORTS</title>") > 0)
            {
                GetNucleus();
                return;
            }

            if (responseStr.IndexOf("<title>Log In</title>") > 0)
            {
                LoginForm(response.RequestMessage?.RequestUri);
            }
        }

        public async Task GetNucleus()
        {
            var response = await _httpClient.GetAsync(Urls.Nucleus);
            if (response is null || !response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync();
            Regex regex = new Regex(@"EASW_ID\W*=\W*'(\d*)'");
            Match match = regex.Match(body);

            if (match == null || !match.Groups[1].Success)
                return;

            var nucleusId = match.Groups[1].Value;

            await GetShards(nucleusId);
        }

        public async Task GetShards(string nucleusId)
        {
            _httpClient.DefaultRequestHeaders.Add("Easw-Session-Data-Nucleus-Id", nucleusId);
            _httpClient.DefaultRequestHeaders.Add("X-UT-Embed-Error", true.ToString());
            _httpClient.DefaultRequestHeaders.Add("X-UT-Route", "https://utas.fut.ea.com");
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            _httpClient.DefaultRequestHeaders.Add("Referer", Urls.Referer);

            var response = await _httpClient.GetAsync(Urls.Shards);
            if (response is null || !response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync();
            var shardInfos = new List<string>();

            await GetAccount(shardInfos);
        }

        public async Task GetAccount(List<string> shardInfos)
        {
            var shard = shardInfos.FirstOrDefault(s => s.Contains("clientFacingIpPort"));
            // Тук трябва да е shard.clientFacingIpPort
            _httpClient.DefaultRequestHeaders.Add("X-UT-Route", $"https://{shard}");

            var response = await _httpClient.GetAsync(Urls.Accounts);
            if (response is null || !response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync();
            // Тук трябва да взема от persona за съответната конзола
        }
    }
}
