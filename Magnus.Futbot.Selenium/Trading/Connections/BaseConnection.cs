namespace Magnus.Futbot.Selenium.Trading.Connections
{
    public abstract class BaseConnection
    {
        protected readonly HttpClient _httpClient;

        protected BaseConnection(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", new List<string> { "gzip", "deflate", "br" });
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.DefaultRequestHeaders.Add("Host", "utas.external.s2.fut.ea.com");
            _httpClient.DefaultRequestHeaders.Add("Origin", "https://www.ea.com");
            _httpClient.DefaultRequestHeaders.Add("Referer", "https://www.ea.com/");
            //_httpClient.DefaultRequestHeaders.Add("X-UT-SID", "4826f738-82a9-4972-ab70-26571484bfba");
        }

        protected abstract string UTSID { get; }
        protected abstract Uri BaseAddress { get; set; }
    }
}
