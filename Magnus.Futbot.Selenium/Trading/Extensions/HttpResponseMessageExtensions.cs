using System.IO.Compression;

namespace Magnus.Futbot.Selenium.Trading.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<string> GetResponseAsString(this HttpResponseMessage? httpResponseMessage)
        {
            if (httpResponseMessage is null) return string.Empty;

            using Stream stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            using Stream decompressed = new GZipStream(stream, CompressionMode.Decompress);
            using StreamReader reader = new(decompressed);
            return reader.ReadToEnd();
        }
    }
}
