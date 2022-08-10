using System.IO.Compression;
using System.Text.Json;

namespace Magnus.Futbot.Selenium.Trading.Connections
{
    public abstract class PutConnection<T> : BaseConnection
    {
        protected PutConnection(HttpClient httpClient) : base(httpClient)
        {
        }

        protected async Task<HttpResponseMessage> PutAsync(T content)
            => await _httpClient.PutAsync(BaseAddress, new StringContent(JsonSerializer.Serialize(content)));
    }
}
