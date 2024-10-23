using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Database.Models;
using Magnus.Futbot.Database.Repositories;
using MongoDB.Bson;

namespace Magnus.Futbot.Api.Services
{
    public class TradeHistoryService
    {
        private readonly TradesRepository _tradesRepository;

        public TradeHistoryService(TradesRepository tradesRepository)
        {
            _tradesRepository = tradesRepository;
        }

        public async Task AddTradeAsync(ProfileDTO profileDTO, BuyCardDTO buyCardDTO)
        {
            var trade = new Trade
            {
                UserId = new ObjectId(profileDTO.UserId),
                ProfileId = profileDTO.Id,
                BuyCardDTO = buyCardDTO,
                TradeHistoryActionType = TradeHistoryActionType.Buy,
            };

            await _tradesRepository.Add(trade);
        }

        public async Task AddTradeAsync(ProfileDTO profileDTO, SellCardDTO sellCardDTO)
        {
            var trade = new Trade
            {
                UserId = new ObjectId(profileDTO.UserId),
                ProfileId = profileDTO.Id,
                SellCardDTO = sellCardDTO,
                TradeHistoryActionType = TradeHistoryActionType.Sell,
            };

            await _tradesRepository.Add(trade);
        }

        public async Task AddTradeAsync(ProfileDTO profileDTO, BuyAndSellCardDTO buyAndSellCardDTO)
        {
            var trade = new Trade
            {
                UserId = new ObjectId(profileDTO.UserId),
                ProfileId = profileDTO.Id,
                BuyAndSellCardDTO = buyAndSellCardDTO,
                TradeHistoryActionType = TradeHistoryActionType.BuyAndSell,
            };

            await _tradesRepository.Add(trade);
        }

        public async Task AddTradeAsync(string pidId, string userId, BuyCardDTO buyCardDTO)
        {
            var trade = new Trade
            {
                UserId = new ObjectId(userId),
                PidId = pidId,
                BuyCardDTO = buyCardDTO,
                TradeHistoryActionType = TradeHistoryActionType.Buy,
            };

            await _tradesRepository.Add(trade);
        }

        public async Task<List<Trade>> GetAllTradesAsync(string profileId, string userId)
        {
            var trades = await _tradesRepository.GetAll(new ObjectId(userId));
            var allProfileTrades = trades.Where(t => t.ProfileId == profileId).ToList();

            allProfileTrades.Reverse();

            var lastTrades = allProfileTrades.Take(51).ToList();

            var tradesToDelete = allProfileTrades.Where(apt => !lastTrades.Select(lt => lt.Id).Contains(apt.Id));

            await _tradesRepository.Delete(tradesToDelete.Select(t => t.Id));

            return lastTrades;
        }

        public async Task<List<Trade>> GetAllTradesByPidIdAsync(string pidId, string userId)
        {
            var trades = await _tradesRepository.GetAll(new ObjectId(userId));
            var allProfileTrades = trades.Where(t => t.PidId == pidId).ToList();

            allProfileTrades.Reverse();

            var lastTrades = allProfileTrades.Take(51).ToList();

            var tradesToDelete = allProfileTrades.Where(apt => !lastTrades.Select(lt => lt.Id).Contains(apt.Id));

            await _tradesRepository.Delete(tradesToDelete.Select(t => t.Id));

            return lastTrades;
        }
    }
}
