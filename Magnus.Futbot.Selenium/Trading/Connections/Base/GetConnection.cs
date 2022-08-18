namespace Magnus.Futbot.Selenium.Trading.Connections.Base
{
    public abstract class GetConnection : BaseConnection
    {
        public GetConnection(HttpClient httpClient) : base(httpClient)
        {
        }

        protected override string UTSID { get; set; } = string.Empty;

        protected override Uri BaseAddress { get; set; } = new Uri(string.Empty);

        protected async Task<HttpResponseMessage> GetAsync()
            => await _httpClient.GetAsync(BaseAddress);
    }
}
