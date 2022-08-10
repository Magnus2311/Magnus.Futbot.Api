using Newtonsoft.Json;

namespace Magnus.Futbot.Initializer.Connections
{
    public class EaConnectionService
    {
        private readonly HttpClient _httpClient;
        private readonly string _eaPlayersApi;
        private readonly ILogger<EaConnectionService> _logger;

        public EaConnectionService(ILogger<EaConnectionService> logger, 
            HttpClient httpClient, 
            IConfiguration configuration)
        {
            _httpClient = httpClient;
            _eaPlayersApi = configuration["EA:PLAYERS:GET:URL"];
            _logger = logger;
        }

        public async Task<string> FetchPlayers()
        {
            try
            {
                return await _httpClient.GetStringAsync(_eaPlayersApi);
            }
            catch (Exception ex)
            {
                _logger.LogError(JsonConvert.SerializeObject(ex));
            }

            return string.Empty;
        }
    }
}
