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
using System.Threading;

namespace Magnus.Futtbot.Connections.Services
{
    public class BuyService
    {
        private readonly TransferMarketCardsConnection _transferMarketCardsConnection;
        private readonly BidConnection _bidConnection;
        private readonly LoginSeleniumService _loginSeleniumService;
        private readonly MoveService _moveService;

        public BuyService(TransferMarketCardsConnection transferMarketCardsConnection, 
            BidConnection bidConnection, LoginSeleniumService loginSeleniumService,
            MoveService moveService)
        {
            _transferMarketCardsConnection = transferMarketCardsConnection;
            _bidConnection = bidConnection;
            _loginSeleniumService = loginSeleniumService;
            _moveService = moveService;
        }

        public async Task Buy(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource)
        {
            var tradingData = new TradingData();

            await Buy(profileDTO, buyCardDTO, cancellationTokenSource, tradingData);
        }

        public async Task Buy(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource, TradingData tradingData)
        {
            if (tradingData.AlreadyBoughtCount >= buyCardDTO.Count)
                cancellationTokenSource.Cancel();

            if (tradingData.LoginFailedAttempts > 5)
                cancellationTokenSource.Cancel();

            if (tradingData.PauseForAWhile > 50)
                cancellationTokenSource.Cancel();

            if (!EaData.UserXUTSIDs.ContainsKey(profileDTO.Email))
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var availableCards = await GetAvailableByRotating(profileDTO, buyCardDTO, cancellationTokenSource);
                
                if (availableCards == null)
                {
                    await Buy(profileDTO, buyCardDTO, cancellationTokenSource, tradingData);
                    return;
                }

                foreach (var availableCard in availableCards.auctionInfo.OrderBy(ai => ai.buyNowPrice))
                {
                    var responseType = await _bidConnection.BidPlayer(profileDTO.Email, availableCard.tradeId, availableCard.buyNowPrice);
                    Thread.Sleep(300);
                    if (responseType == ConnectionResponseType.PauseForAWhile)
                    {
                        tradingData.PauseForAWhile += 1;
                        await Buy(profileDTO, buyCardDTO, cancellationTokenSource, tradingData);
                        cancellationTokenSource.Cancel();
                        return;
                    }

                    if (responseType == ConnectionResponseType.Unauthorized)
                    {
                        tradingData.LoginFailedAttempts += 1;
                        await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                        await Buy(profileDTO, buyCardDTO, cancellationTokenSource, tradingData);
                        cancellationTokenSource.Cancel();
                        return;
                    }

                    if (responseType == ConnectionResponseType.Success)
                    {
                        tradingData.LoginFailedAttempts = 0;
                        tradingData.AlreadyBoughtCount += 1;
                        Console.Write($"Player won succesfully: {buyCardDTO.Card.Name} for {availableCard.buyNowPrice} coins!");

                        var sendResponse = await _moveService.SendWonItemsToTransferList(profileDTO.Email, new SendCardsToTransferListRequest(new List<ItemDataForMoving>()
                        {
                            new(availableCard.itemData.id, "trade")
                        }));

                        Thread.Sleep(300);

                        if (responseType == ConnectionResponseType.Unauthorized)
                        {
                            tradingData.LoginFailedAttempts += 1;
                            await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                            await Buy(profileDTO, buyCardDTO, cancellationTokenSource, tradingData);
                            cancellationTokenSource.Cancel();
                            return;
                        }
                    }
                }
            }
        }

        public async Task<AvailableTransferMarketCards?> GetAvailableByRotating(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource)
        {
            var minBin = 0;

            while (minBin < buyCardDTO?.Price && minBin <= 500)
            {
                var availableCardsResponse = await GetAvailableCardsResponse(profileDTO, buyCardDTO, minBin);

                if (availableCardsResponse.ConnectionResponseType == ConnectionResponseType.Success 
                    && availableCardsResponse.Data is not null 
                    && availableCardsResponse.Data.auctionInfo.Any())
                    return availableCardsResponse.Data;

                minBin += minBin == 0 ? 200 : 50;
            }

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
