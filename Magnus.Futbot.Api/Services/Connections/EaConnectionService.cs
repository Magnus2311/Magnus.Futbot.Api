namespace Magnus.Futbot.Api.Services.Connections
{
    public class EaConnectionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _eaPlayersApi;

        public EaConnectionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _eaPlayersApi = configuration["EA:PLAYERS:GET:URL"];
        }

        public async Task<string> FetchPlayers()
        {
            try
            {
                return await _httpClient.GetStringAsync(_eaPlayersApi);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return string.Empty;
        }
    }
}
