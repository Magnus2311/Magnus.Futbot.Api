using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Services;
using Magnus.Futtbot.Connections.Connection.Trading;
using Magnus.Futtbot.Connections.Connection.Trading.Buy;
using Magnus.Futtbot.Connections.Enums;
using Magnus.Futtbot.Connections.Models;
using Magnus.Futtbot.Connections.Models.Requests;
using Magnus.Futtbot.Connections.Models.Responses;
using Magnus.Futtbot.Connections.Utils;

namespace Magnus.Futtbot.Connections.Services
{
    public class BuyService
    {
        private readonly TransferMarketCardsConnection _transferMarketCardsConnection;
        private readonly BidConnection _bidConnection;
        private readonly LoginSeleniumService _loginSeleniumService;
        private readonly MoveService _moveService;
        private readonly ProfileService _profileService;

        public BuyService(TransferMarketCardsConnection transferMarketCardsConnection,
            BidConnection bidConnection, LoginSeleniumService loginSeleniumService,
            MoveService moveService, ProfileService profileService)
        {
            _transferMarketCardsConnection = transferMarketCardsConnection;
            _bidConnection = bidConnection;
            _loginSeleniumService = loginSeleniumService;
            _moveService = moveService;
            _profileService = profileService;
        }

        public async Task Buy(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource, Func<long, Task>? sellAction, Action<ProfileDTO> updateProfile)
        {
            var tradingData = new BuyingData();

            profileDTO.TradePile = await _profileService.GetTradePile(profileDTO);
            updateProfile(profileDTO);

            while (tradingData.AlreadyBoughtCount < buyCardDTO.Count
                && !cancellationTokenSource.IsCancellationRequested
                && tradingData.LoginFailedAttempts <= 5
                && tradingData.PauseForAWhile <= 50)
            {
                if (!EaData.UserXUTSIDs.ContainsKey(profileDTO.Email))
                    await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

                var availableCards = await GetAvailableByRotating(profileDTO, buyCardDTO, tradingData, cancellationTokenSource);

                if (availableCards == null)
                {
                    tradingData.PauseForAWhile += 1;
                    continue;
                }

                foreach (var availableCard in availableCards.auctionInfo.OrderBy(ai => ai.buyNowPrice))
                {
                    if (tradingData.AlreadyBoughtCount >= buyCardDTO.Count)
                        continue;

                    if (tradingData.AlreadyBiddedTrades.Contains(availableCard.tradeId))
                    {
                        tradingData.MinBin += 50;
                        continue;
                    }

                    tradingData.AlreadyBiddedTrades.Add(availableCard.tradeId);
                    var responseType = await Buy(profileDTO, availableCard);

                    if (responseType == ConnectionResponseType.Unauthorized)
                    {
                        tradingData.LoginFailedAttempts += 1;
                        await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                        continue;
                    }

                    if (responseType == ConnectionResponseType.PauseForAWhile)
                    {
                        tradingData.PauseForAWhile += 1;
                        continue;
                    }

                    if (responseType == ConnectionResponseType.UpgradeRequired)
                    {
                        profileDTO.TradePile = await _profileService.GetTradePile(profileDTO);
                        updateProfile(profileDTO);
                        continue;
                    }

                    if (responseType == ConnectionResponseType.Success)
                    {
                        profileDTO.Coins -= availableCard.buyNowPrice;
                        profileDTO.WonTargetsCount++;
                        tradingData.AlreadyBoughtCount += 1;
                        tradingData.LoginFailedAttempts = 0;
                        tradingData.PauseForAWhile = 0;

                        updateProfile(profileDTO);

                        Console.Write($"Player won succesfully: {buyCardDTO.Card!.Name} for {availableCard.buyNowPrice} coins! Account: {profileDTO.Email}");

                        var sendResponse = await _moveService.SendWonItemsToTransferList(profileDTO.Email, new SendCardsToTransferListRequest(new List<ItemDataForMoving>()
                        {
                            new(availableCard.itemData.id, "trade")
                        }));

                        Thread.Sleep(300);

                        if (sendResponse == ConnectionResponseType.Unauthorized)
                        {
                            tradingData.LoginFailedAttempts += 1;
                            await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                            continue;
                        }

                        if (sendResponse == ConnectionResponseType.Success && sellAction is not null)
                                await sellAction(availableCard.itemData.id);
                    }
                }
            }

        }

        public async Task<ConnectionResponseType> Buy(ProfileDTO profileDTO, Auctioninfo availableCard)
            => await _bidConnection.BidPlayer(profileDTO.Email, availableCard.tradeId, availableCard.buyNowPrice);
        
        public async Task<AvailableTransferMarketCards?> GetAvailableByRotating(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, BuyingData tradingData, CancellationTokenSource cancellationTokenSource)
        {
            while (tradingData.MinBin < buyCardDTO?.Price && tradingData.MinBin <= 500 && !cancellationTokenSource.IsCancellationRequested)
            {
                var availableCardsResponse = await GetAvailableCardsResponse(profileDTO, buyCardDTO, tradingData.MinBin);

                if (availableCardsResponse.ConnectionResponseType == ConnectionResponseType.Success
                    && availableCardsResponse.Data is not null
                    && availableCardsResponse.Data.auctionInfo.Any())
                {
                    tradingData.PauseForAWhile = 0;
                    return availableCardsResponse.Data;
                }

                if (availableCardsResponse.ConnectionResponseType == ConnectionResponseType.PauseForAWhile)
                {
                    tradingData.PauseForAWhile++;
                    Thread.Sleep(1000 * tradingData.PauseForAWhile);
                }

                tradingData.MinBin += tradingData.MinBin == 0 ? 200 : 50;
            }

            tradingData.MinBin = 0;

            return null;
        }

        private async Task<ConnectionResponse<AvailableTransferMarketCards>> GetAvailableCardsResponse(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, int minBin)
        {
            var availableCardsResponse = await _transferMarketCardsConnection.GetAvailableCardsByPlayer(profileDTO, buyCardDTO.Card.EAId, minBin, buyCardDTO.Price);

            if (availableCardsResponse.ConnectionResponseType == ConnectionResponseType.Unauthorized)
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            return availableCardsResponse;
        }
    }
}
