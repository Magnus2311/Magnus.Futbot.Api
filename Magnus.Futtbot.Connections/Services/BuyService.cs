﻿using Magnus.Futbot.Common.fcmodels;
using Magnus.Futbot.Common.Interfaces.Helpers;
using Magnus.Futbot.Common.Models;
using Magnus.Futbot.Common.Models.Database.Card;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Common.Models.Selenium.Trading;
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
        private readonly IEnumerable<Card> _cards;

        public BuyService(TransferMarketCardsConnection transferMarketCardsConnection,
            BidConnection bidConnection, LoginSeleniumService loginSeleniumService,
            MoveService moveService, ProfileService profileService,
            ICardsHelper cardsHelper)
        {
            _transferMarketCardsConnection = transferMarketCardsConnection;
            _bidConnection = bidConnection;
            _loginSeleniumService = loginSeleniumService;
            _moveService = moveService;
            _profileService = profileService;
            _cards = cardsHelper.GetAllCards().Where(c => c is not null);
        }

        public async Task Buy(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource, Func<long, Task>? sellAction, Action<ProfileDTO> updateProfile)
        {
            if (buyCardDTO.IsBin)
                await BinAsync(profileDTO, buyCardDTO, cancellationTokenSource, sellAction, updateProfile);
            else
                await BidAsync(profileDTO, buyCardDTO, cancellationTokenSource, sellAction, updateProfile);
        }

        private async Task BidAsync(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource, Func<long, Task>? sellAction, Action<ProfileDTO> updateProfile)
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

                if (sellAction != null)
                {
                    // Да се добави в сингълтън колекция и някакъв worker да минава и да я чисти и да ги продава

                    //var availableItems = tradePileResponse
                    //.Data
                    //.auctionInfo
                    //.Where(ai => ai.tradeState is null)
                    //.Where(ai => ai.itemData.pile == (int)PileType.Available)
                    //.GroupBy(ai => ai.itemData.assetId)
                    //.Select(g => new
                    //{
                    //    AuctionInfo = g.First(),
                    //    Count = g.Count()
                    //})
                    //.Select(ai =>
                    //    new TransferCard
                    //    {
                    //        Card = cards.FirstOrDefault(c => c.EAId == ai.AuctionInfo.itemData.assetId),
                    //        Count = ai.Count
                    //    }
                    //).ToList();
                }
               

                var availableCards = await GetAvailableForBidding(profileDTO, buyCardDTO, tradingData, cancellationTokenSource);

                if (availableCards == null)
                {
                    tradingData.PauseForAWhile += 1;
                    continue;
                }

                foreach (var availableCard in availableCards.auctionInfo)
                {
                    if (tradingData.AlreadyBoughtCount >= buyCardDTO.Count)
                        break;

                    var nextBidStep = NumberSteps.GetNextStep(availableCard.currentBid, availableCard.startingBid);

                    var responseType = await BidAsync(profileDTO, availableCard, nextBidStep);

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
                        tradingData.LoginFailedAttempts = 0;
                        tradingData.PauseForAWhile = 0;

                        Console.Write($"Player bidded succesfully: {buyCardDTO.Card!.Name} for {nextBidStep} coins! Account: {profileDTO.Email}");

                        await Task.Delay(300);
                    }
                }

                await Task.Delay(2000);
            }
        }

        public async Task BinAsync(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource, Func<long, Task>? sellAction, Action<ProfileDTO> updateProfile)
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
                    var responseType = await BidAsync(profileDTO, availableCard, availableCard.buyNowPrice);

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

                        var card = _cards.FirstOrDefault(c => c.EAId == availableCard.itemData.assetId);

                        if (availableCard != null)
                            profileDTO.History.Add(availableCard);

                        if (profileDTO.TradePile.TransferList.ActiveTransfers.Any(at => at.Card == card))
                            profileDTO.TradePile.TransferList.ActiveTransfers.FirstOrDefault(at => at.Card == card)!.Count++;
                        else
                            profileDTO.TradePile.TransferList.ActiveTransfers.Add(new TransferCard
                            {
                                Card = card,
                                Count = 1
                            });

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

        public async Task<ConnectionResponseType> BidAsync(ProfileDTO profileDTO, Auctioninfo availableCard, int price)
            => await _bidConnection.BidPlayer(profileDTO.Email, availableCard.tradeId, price);

        public async Task<AvailableTransferMarketCards?> GetAvailableByRotating(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, BuyingData tradingData, CancellationTokenSource cancellationTokenSource)
        {
            while (tradingData.MinBin < buyCardDTO?.Price && tradingData.MinBin <= 500 && !cancellationTokenSource.IsCancellationRequested)
            {
                var availableCardsResponse = await GetAvailableCardsByMaxBinResponse(profileDTO, buyCardDTO, tradingData.MinBin);

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
                    if (tradingData.PauseForAWhile > 10) cancellationTokenSource.Cancel();

                    Thread.Sleep(1000 * tradingData.PauseForAWhile);
                }

                tradingData.MinBin += tradingData.MinBin == 0 ? 200 : 50;
            }

            tradingData.MinBin = 0;

            return null;
        }

        public async Task<AvailableTransferMarketCards?> GetAvailableForBidding(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, BuyingData tradingData, CancellationTokenSource cancellationTokenSource)
        {
            var availableTransferCards = new List<Auctioninfo>();
            var startingIndex = 0;

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                var availableCardsResponse = await GetAvailableCardsByPlayerResponse(profileDTO, buyCardDTO, startingIndex);

                if (availableCardsResponse.ConnectionResponseType == ConnectionResponseType.Success
                    && availableCardsResponse.Data is not null
                    && availableCardsResponse.Data.auctionInfo.Any())
                {
                    tradingData.PauseForAWhile = 0;
                    availableTransferCards.AddRange(availableCardsResponse.Data.auctionInfo);

                    if (availableTransferCards.LastOrDefault()?.expires > 40)
                    {
                        availableCardsResponse.Data.auctionInfo = availableTransferCards
                            .Where(c =>
                                c.tradeState == "active"
                                && c.bidState != "highest"
                                && c.currentBid < buyCardDTO.Price
                                && c.startingBid <= buyCardDTO.Price
                                && c.expires <= 40)
                            .ToList();

                        return availableCardsResponse.Data;
                    }

                    if (startingIndex == 1)
                        startingIndex = 20;
                    else
                        startingIndex += 20;
                }
                else if (availableCardsResponse.ConnectionResponseType == ConnectionResponseType.PauseForAWhile)
                {
                    tradingData.PauseForAWhile++;
                    await Task.Delay(1000 * tradingData.PauseForAWhile);
                }
            }

            return null;
        }

        private async Task<ConnectionResponse<AvailableTransferMarketCards>> GetAvailableCardsByMaxBinResponse(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, int minBin)
        {
            var availableCardsResponse = await _transferMarketCardsConnection.GetAvailableCardsByPlayerAndMaxPrice(profileDTO, buyCardDTO.Card.EAId, minBin, buyCardDTO.Price);

            if (availableCardsResponse.ConnectionResponseType == ConnectionResponseType.Unauthorized)
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            return availableCardsResponse;
        }

        private async Task<ConnectionResponse<AvailableTransferMarketCards>> GetAvailableCardsByPlayerResponse(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, int startingIndex)
        {
            var availableCardsResponse = await _transferMarketCardsConnection.GetAvailableCardsByPlayer(profileDTO, buyCardDTO.Card.EAId, startingIndex);

            if (availableCardsResponse.ConnectionResponseType == ConnectionResponseType.Unauthorized)
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            return availableCardsResponse;
        }
    }
}
