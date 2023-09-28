using Magnus.Futtbot.Connections.Constants;
using Magnus.Futtbot.Connections.Models.Auth;
using System;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Magnus.Futtbot.Connections.Connection.Auth
{
    public class Login
    {
        private HttpClient _httpClient;

        public Login(LoginDetails loginDetails, HttpClient httpClient)
        {
            LoginDetails = loginDetails;

            _httpClient = httpClient;

            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            _httpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            _httpClient.DefaultRequestHeaders.Add("DNT", "1");
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
        }

        public LoginDetails LoginDetails { get; }

        public async Task LoginAsync()
        {

            var response = await _httpClient.GetAsync(Urls.Main);

            if (response == null || !response.IsSuccessStatusCode)
                return;

            var responseStr = await response.Content.ReadAsStringAsync();

            if (responseStr.IndexOf("<title>FIFA Football | FUT Web App | EA SPORTS</title>") > 0)
            {
                await GetNucleus();
                return;
            }

            if (responseStr.IndexOf("<title>Log In</title>") > 0)
            {
                await LoginForm(response.RequestMessage?.RequestUri);
            }
        }

        public async Task LoginForm(Uri? url)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", LoginDetails.Email),
                new KeyValuePair<string, string>("password", LoginDetails.Password),
                new KeyValuePair<string, string>("_rememberMe", "on"),
                new KeyValuePair<string, string>("rememberMe", "on"),
                new KeyValuePair<string, string>("_eventId", "submit"),
                new KeyValuePair<string, string>("facebookAuth", "")
            });

            var response = await _httpClient.PostAsync(url, formContent);

            if (response is null || !response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync();

            if (body.IndexOf("<title>FIFA Football | FUT Web App | EA SPORTS</title>") > 0)
                await GetNucleus();

            if (body.IndexOf("<title>Log In</title>") > 0)
                return;

            if (body.IndexOf("<title>Set Up an App Authenticator</title>") > 0)
                return;

            if (body.IndexOf("<title>Login Verification</title>") > 0)
                await SendTwoFactorCode(response.RequestMessage?.RequestUri);
        }

        public async Task SendTwoFactorCode(Uri? uri)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("twofactorCode", LoginDetails.TwoFactorCode),
                new KeyValuePair<string, string>("_eventId", "submit"),
                new KeyValuePair<string, string>("_trustThisDevice", "on"),
                new KeyValuePair<string, string>("trustThisDevice", "on"),
            });

            var response = await _httpClient.PostAsync(uri, formContent);

            if (response is null || !response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync();
            if (body.IndexOf("<title>FIFA Football | FUT Web App | EA SPORTS</title>") > 0) 
                await GetNucleus();
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

            await GetAccount(shardInfos, nucleusId);
        }

        public async Task GetAccount(List<string> shardInfos, string nucleusId)
        {
            var shard = shardInfos.FirstOrDefault(s => s.Contains("clientFacingIpPort"));
            // Тук трябва да е shard.clientFacingIpPort
            _httpClient.DefaultRequestHeaders.Add("X-UT-Route", $"https://{shard}");

            var response = await _httpClient.GetAsync(Urls.Accounts);
            if (response is null || !response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadAsStringAsync();
            // Тук трябва да взема от persona за съответната конзола

            await GetSession(nucleusId);
        }

        public async Task GetSession(string nucleusId)
        {
            long.TryParse(nucleusId, out var nucleusIdLong);

            var sessionRequestBody = new SessionRequestBody(nucleusIdLong, "");

            var response = await _httpClient.PostAsJsonAsync(Urls.Session, sessionRequestBody);
            if (response is null || !response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadFromJsonAsync<SessionResponseBody>();
            if (body is null || string.IsNullOrEmpty(body.Sid))
                return;

            await Phishing(body.Sid);
        }

        public async Task Phishing(string sid)
        {
            _httpClient.DefaultRequestHeaders.Add("X-UT-SID", sid);

            var response = await _httpClient.GetAsync(Urls.Question);
            if (response is null || !response.IsSuccessStatusCode)
                return;

            var body = await response.Content.ReadFromJsonAsync<PhishingResponseBody>();
            if (body is null)
                return;

            if (Utils.Utils.IsApiMessage(body) && !string.IsNullOrEmpty(body.Token))
            {
                // Нещос е случва за което нямам идея какво е
            }

            if (!string.IsNullOrEmpty(body.Question))
            {
                await Validate();
            }
        }

        public async Task Validate()
        {
        }
    }
}
