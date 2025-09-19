using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Magnus.Futbot.Initializer.Models.Players;
using System.Text.RegularExpressions;
using Magnus.Futbot.Common.Models.Database.Card;
using Magnus.Futbot.Database.Repositories;

namespace Magnus.Futbot.Initializer.Connections
{
    public class EaConnectionService(ILogger<EaConnectionService> logger,
        IConfiguration configuration,
        HttpClient httpClient,
        CardsRepository cardsRepository)
    {
        private readonly string _eaPlayersApi = configuration["EA:PLAYERS:GET:URL"];

        private void ConfigureHttpClientHeaders()
        {
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "en-GB,en-US;q=0.9,en;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Origin", "https://www.ea.com");
            httpClient.DefaultRequestHeaders.Add("Referer", "https://www.ea.com/");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua", "\"Chromium\";v=\"140\", \"Not=A?Brand\";v=\"24\", \"Google Chrome\";v=\"140\"");
            httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua-Mobile", "?0");
            httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");
            httpClient.DefaultRequestHeaders.Add("X-Feature", "{\"disable_share_image\":false,\"enable_addon_bundle_sections\":true,\"enable_age_gate\":true,\"enable_age_gate_refactor\":true,\"enable_bf2042_glacier_theme\":true,\"enable_checkout_page\":true,\"enable_college_football_ratings\":true,\"enable_currency\":false,\"enable_events_page\":true,\"enable_franchise_hub\":false,\"enable_franchise_newsletter\":false,\"enable_im_resize_query_param\":true,\"enable_language_redirection\":true,\"enable_legal_disclaimer_page\":false,\"enable_mobile_download_flow_optimization\":true,\"enable_multimedia_consent\":false,\"enable_newsletter_with_incentive\":true,\"enable_next_ratings_release\":true,\"enable_non_mobile_download_flow_optimization\":true,\"enable_portal\":false,\"enable_portal_filter\":false,\"enable_postlaunch_webstore_focus\":true,\"enable_postlaunch_webstore_image_link_ab_test\":false,\"enable_postlaunch_webstore_pdp_promotion\":true,\"enable_ratings_up_down_vote\":true,\"enable_showcase_edition\":false,\"enable_spotlight_carousel\":true,\"enable_translations_api_route\":false,\"enable_ugc_page\":false,\"enable_ugx\":false}");
        }

        public async Task<string> FetchPlayers()
        {
            try
            {
                ConfigureHttpClientHeaders();
                return await httpClient.GetStringAsync(_eaPlayersApi);
            }
            catch (Exception ex)
            {
                logger.LogError(JsonConvert.SerializeObject(ex));
            }

            return string.Empty;
        }

        public async Task<List<Card>> FetchAllPlayers(CancellationToken cancellationToken = default)
        {
            var all = new List<Card>();
            try
            {
                ConfigureHttpClientHeaders();
                var firstResponseString = await httpClient.GetStringAsync(_eaPlayersApi, cancellationToken);
                var firstResponse = JsonConvert.DeserializeObject<EaPlayersResponse>(firstResponseString);
                if (firstResponse == null || firstResponse.Items == null || firstResponse.Items.Count == 0)
                {
                    return all;
                }

                // Filter out already existing cards by EAId before inserting
                var firstIds = firstResponse.Items.Select(c => c.EAId).ToList();
                var existingFirst = await cardsRepository.GetExistingEaIdsAsync(firstIds);
                var newFirst = firstResponse.Items.Where(c => !existingFirst.Contains(c.EAId)).ToList();

                all.AddRange(firstResponse.Items);
                if (newFirst.Count > 0)
                {
                    await cardsRepository.Add(newFirst);
                }

                var totalItems = firstResponse.TotalItems;
                var pageSize = firstResponse.Items.Count;
                if (pageSize <= 0 || totalItems <= pageSize)
                {
                    return all;
                }

                var currentOffset = ExtractOffset(_eaPlayersApi);
                if (currentOffset < 0) currentOffset = 0;

                while (all.Count < totalItems)
                {
                    currentOffset += pageSize;
                    var nextUrl = BuildUrlWithOffset(_eaPlayersApi, currentOffset);
                    var pageString = await httpClient.GetStringAsync(nextUrl, cancellationToken);
                    var page = JsonConvert.DeserializeObject<EaPlayersResponse>(pageString);
                    if (page?.Items == null || page.Items.Count == 0)
                    {
                        break;
                    }
                    // Filter out already existing cards by EAId before inserting
                    var pageIds = page.Items.Select(c => c.EAId).ToList();
                    var existingPage = await cardsRepository.GetExistingEaIdsAsync(pageIds);
                    var newPage = page.Items.Where(c => !existingPage.Contains(c.EAId)).ToList();

                    all.AddRange(page.Items);
                    if (newPage.Count > 0)
                    {
                        await cardsRepository.Add(newPage);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(JsonConvert.SerializeObject(ex));
            }

            return all;
        }

        private static int ExtractOffset(string url)
        {
            var match = Regex.Match(url, @"[?&]offset=(\d+)");
            if (match.Success && int.TryParse(match.Groups[1].Value, out var offset))
            {
                return offset;
            }
            return 0;
        }

        private static string BuildUrlWithOffset(string url, int offset)
        {
            if (Regex.IsMatch(url, @"[?&]offset=\d+"))
            {
                return Regex.Replace(url, @"([?&]offset=)(\d+)", m => m.Groups[1].Value + offset.ToString());
            }

            return url.Contains("?") ? $"{url}&offset={offset}" : $"{url}?offset={offset}";
        }
    }
}
