using Magnus.Futbot.Common.Interfaces.Helpers;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Selenium.Trading;
using Magnus.Futbot.Services;
using Magnus.Futtbot.Connections.Connection;
using Magnus.Futtbot.Connections.Enums;
using Magnus.Futtbot.Connections.Utils;

namespace Magnus.Futtbot.Connections.Services
{
    public class ProfileService
    {
        private readonly GetUserPileConnection _getUserPileConnection;
        private readonly LoginSeleniumService _loginSeleniumService;
        private readonly ICardsHelper _cardsHelper;

        public ProfileService(GetUserPileConnection getUserPileConnection,
            LoginSeleniumService loginSeleniumService,
            ICardsHelper cardsHelper)
        {
            _getUserPileConnection = getUserPileConnection;
            _loginSeleniumService = loginSeleniumService;
            _cardsHelper = cardsHelper;
        }

        public async Task<TradePile> GetTradePile(ProfileDTO profileDTO)
        {
            var cards = _cardsHelper.GetAllCards();
            if (!EaData.UserXUTSIDs.ContainsKey(profileDTO.Email))
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            var tradePileResponse = await _getUserPileConnection.GetUserTradePile(profileDTO.Email);
            if (tradePileResponse.ConnectionResponseType == ConnectionResponseType.Unauthorized)
            {
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                return await GetTradePile(profileDTO);
            }

            if (tradePileResponse.Data is null)
                return new TradePile();

            var unassignedDataResponse = await _getUserPileConnection.GetUnassignedItems(profileDTO.Email);
            if (unassignedDataResponse.ConnectionResponseType == ConnectionResponseType.Unauthorized)
            {
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                return await GetTradePile(profileDTO);
            }

            if (unassignedDataResponse.Data is null)
                return new TradePile();

            profileDTO.Coins = tradePileResponse.Data.credits;

            var soldItems = tradePileResponse
                .Data
                .auctionInfo
                .Where(ai => ai.tradeState == "closed")
                .GroupBy(ai => ai.itemData.assetId)
                .Select(g => new
                {
                    AuctionInfo = g.First(),
                    Count = g.Count()
                })
                .Select(ai =>
                    new TransferCard
                    {
                        Card = cards.FirstOrDefault(c => c.EAId == ai.AuctionInfo.itemData.assetId),
                        Count = ai.Count
                    }
                ).ToList();

            var availableItems = tradePileResponse
                .Data
                .auctionInfo
                .Where(ai => ai.tradeState is null)
                .Where(ai => ai.itemData.pile == (int)PileType.Available)
                .GroupBy(ai => ai.itemData.assetId)
                .Select(g => new
                {
                    AuctionInfo = g.First(),
                    Count = g.Count()
                })
                .Select(ai =>
                    new TransferCard
                    {
                        Card = cards.FirstOrDefault(c => c.EAId == ai.AuctionInfo.itemData.assetId),
                        Count = ai.Count
                    }
                ).ToList();

            var unsoldItems = tradePileResponse
                .Data
                .auctionInfo
                .Where(ai => ai.tradeState == "expired")
                .Where(ai => ai.itemData.pile == (int)PileType.Available)
                .GroupBy(ai => ai.itemData.assetId)
                .Select(g => new
                {
                    AuctionInfo = g.First(),
                    Count = g.Count()
                })
                .Select(ai =>
                    new TransferCard
                    {
                        Card = cards.FirstOrDefault(c => c.EAId == ai.AuctionInfo.itemData.assetId),
                        Count = ai.Count
                    }
                ).ToList();

            var activeTransfers = tradePileResponse
                .Data
                .auctionInfo
                .Where(ai => ai.tradeState == "active")
                .Where(ai => ai.itemData.pile == (int)PileType.Available)
                .GroupBy(ai => ai.itemData.assetId)
                .Select(g => new
                {
                    AuctionInfo = g.First(),
                    Count = g.Count()
                })
                .Select(ai =>
                    new TransferCard
                    {
                        Card = cards.FirstOrDefault(c => c.EAId == ai.AuctionInfo.itemData.assetId),
                        Count = ai.Count
                    }
                ).ToList();

            var unassignedItems = unassignedDataResponse
                .Data
                .itemData
                .Where(ai => ai.pile == (int)PileType.Unassigned)
                .GroupBy(ai => ai.assetId)
                .Select(g => new
                {
                    AuctionInfo = g.First(),
                    Count = g.Count()
                })
                .Select(ai =>
                    new TransferCard
                    {
                        Card = cards.FirstOrDefault(c => c.EAId == ai.AuctionInfo.assetId) ,
                        Count = ai.Count
                    })
                .ToList();

            return new TradePile 
            {
                TransferList = new TransferList
                {
                    ActiveTransfers = activeTransfers,
                    AvailableItems = availableItems,
                    SoldItems = soldItems,
                    UnsoldItems = unsoldItems
                },
                UnassignedItems = unassignedItems
            };

        }
    }
}
