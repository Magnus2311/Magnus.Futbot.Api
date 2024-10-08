﻿using Magnus.Futbot.Common.fcmodels;
using Magnus.Futbot.Common.Interfaces.Helpers;
using Magnus.Futbot.Common.Models.Database.Card;
using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Common.Models.Selenium.Trading;
using Magnus.Futbot.Services;
using Magnus.Futtbot.Connections.Connection;
using Magnus.Futtbot.Connections.Connection.Moving;
using Magnus.Futtbot.Connections.Connection.Trading.Sell;
using Magnus.Futtbot.Connections.Enums;
using Magnus.Futtbot.Connections.Models;
using Magnus.Futtbot.Connections.Models.Requests;
using Magnus.Futtbot.Connections.Models.Responses;
using Magnus.Futtbot.Connections.Utils;

namespace Magnus.Futtbot.Connections.Services
{
    public class SellService
    {
        private readonly GetUserPileConnection _getUserPileConnection;
        private readonly SellConnection _sellConnection;
        private readonly SendItemsConnection _sendItemsConnection;
        private readonly LoginSeleniumService _loginSeleniumService;
        private readonly IEnumerable<Card?> _cards;

        public SellService(GetUserPileConnection getUserPileConnection,
            SellConnection sellConnection,
            SendItemsConnection sendItemsConnection,
            LoginSeleniumService loginSeleniumService,
            ICardsHelper cardsHelper)
        {
            _getUserPileConnection = getUserPileConnection;
            _sellConnection = sellConnection;
            _sendItemsConnection = sendItemsConnection;
            _loginSeleniumService = loginSeleniumService;
            SellingData = new SellingData();
            _cards = cardsHelper.GetAllCards();
        }

        public SellingData SellingData { get; }

        public async Task SellCard(ProfileDTO profileDTO, SellCardDTO sellCardDTO, CancellationTokenSource cancellationTokenSource, Action<ProfileDTO> updateProfile)
        {
            if (!EaData.UserXUTSIDs.ContainsKey(profileDTO.Email))
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            var getUserPileResponse = await _getUserPileConnection.GetUserTradePile(profileDTO.Email);
            if (getUserPileResponse.ConnectionResponseType == ConnectionResponseType.Unauthorized)
            {
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                await SellCard(profileDTO, sellCardDTO, cancellationTokenSource, updateProfile);
                return;
            }

            if (getUserPileResponse.Data is null)
            {
                await SellCard(profileDTO, sellCardDTO, cancellationTokenSource, updateProfile);
                return;
            }

            if (getUserPileResponse.ConnectionResponseType == ConnectionResponseType.Success)
            {
                var unassignedPlayers = getUserPileResponse.Data.auctionInfo.Where(ai => ai.itemData.pile == (int)PileType.Unassigned);

                var sendWonItemsToTransferListResponse = await _sendItemsConnection.SendWonItemsToTransferList(
                    profileDTO.Email,
                    new SendCardsToTransferListRequest(unassignedPlayers.Select(p => new ItemDataForMoving(p.itemData.id, "trade")).ToList()));

                if (sendWonItemsToTransferListResponse == ConnectionResponseType.Unauthorized)
                {
                    await SellCard(profileDTO, sellCardDTO, cancellationTokenSource, updateProfile);
                    return;
                }

                var availablePlayersForSelling = getUserPileResponse.Data.auctionInfo.Where(ai => ai.tradeState != "active");
                var currentPlayerForSale = availablePlayersForSelling.Where(p => p.itemData.assetId == sellCardDTO.Card.EAId);

                while (sellCardDTO.Count > SellingData.AlreadySoldTrades.Count)
                {
                    var notSoldPlayers = currentPlayerForSale.Where(p => !SellingData.AlreadySoldTrades.Contains(p.itemData.id));
                    if (!notSoldPlayers.Any())
                        break;

                    var playersCountToSell = sellCardDTO.Count - SellingData.AlreadySoldTrades.Count;
                    var soldPlayers = TrySellCards(profileDTO, sellCardDTO, currentPlayerForSale.ToArray(), playersCountToSell);
                    await foreach (var player in soldPlayers)
                    {
                        if (player.ConnectionResponseType == ConnectionResponseType.Unauthorized)
                        {
                            await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                            continue;
                        }

                        if (player.Data is null) continue;

                        if (player.ConnectionResponseType == ConnectionResponseType.Success)
                        {
                            SellingData.AlreadySoldTrades.Add(player.Data.id);

                            var card = _cards.Where(c => c is not null).FirstOrDefault(c => c.EAId == player.Data.assetId);
                            if (profileDTO.TradePile.TransferList.ActiveTransfers.Any(at => at.Card == card))
                                profileDTO.TradePile.TransferList.ActiveTransfers.FirstOrDefault(at => at.Card == card)!.Count++;
                            else
                                profileDTO.TradePile.TransferList.ActiveTransfers.Add(new TransferCard
                                {
                                    Card = card,
                                    Count = 1
                                });

                            if (profileDTO.TradePile.TransferList.AvailableItems.Any(at => at.Card == card))
                            {
                                var availableCard = profileDTO.TradePile.TransferList.AvailableItems.FirstOrDefault(at => at.Card == card);
                                if (availableCard != null)
                                {
                                    if (availableCard.Count <= 1)
                                        profileDTO.TradePile.TransferList.AvailableItems.Remove(availableCard);
                                    else
                                    {
                                        availableCard.Count--;
                                    }
                                }
                            }

                            if (profileDTO.TradePile.TransferList.UnsoldItems.Any(at => at.Card == card))
                            {
                                var unsoldCard = profileDTO.TradePile.TransferList.UnsoldItems.FirstOrDefault(at => at.Card == card);
                                if (unsoldCard != null)
                                {
                                    if (unsoldCard.Count <= 1)
                                        profileDTO.TradePile.TransferList.UnsoldItems.Remove(unsoldCard);
                                    else
                                    {
                                        unsoldCard.Count--;
                                    }
                                }
                            }

                            updateProfile(profileDTO);
                        }
                    }
                }
            }
        }

        public async IAsyncEnumerable<ConnectionResponse<Itemdata>> TrySellCards(ProfileDTO profileDTO, SellCardDTO sellCardDTO, Auctioninfo[] playersForSale, int playersToSell)
        {
            foreach (var player in playersForSale.Take(playersToSell))
            {
                Thread.Sleep(1500);
                var sellCardResponse = await _sellConnection.SellCard(profileDTO.Email, new SellCardRequest(sellCardDTO.FromBin, DurationType.OneHour, new SellCardItemData(player.itemData.id), sellCardDTO.FromBid));
                if (sellCardResponse == ConnectionResponseType.Unauthorized)
                    break;

                if (sellCardResponse != ConnectionResponseType.Success)
                    yield return new ConnectionResponse<Itemdata>(ConnectionResponseType.Unknown, player.itemData);

                yield return new ConnectionResponse<Itemdata>(ConnectionResponseType.Success, player.itemData);
            }
        }

        public async Task SellCardById(ProfileDTO profileDTO, SellCardDTO sellCardDTO, long tradeId)
        {
            if (SellingData.AlreadySoldTrades.Contains(tradeId)) return;

            SellingData.AlreadySoldTrades.Add(tradeId);
            var sellCardResponse = await _sellConnection.SellCard(profileDTO.Email, new SellCardRequest(sellCardDTO.FromBin, DurationType.OneHour, new SellCardItemData(tradeId), sellCardDTO.FromBid));
            if (sellCardResponse == ConnectionResponseType.Unauthorized)
            {
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                await SellCardById(profileDTO, sellCardDTO, tradeId);
                return;
            }
        }

        public async Task RelistAll(ProfileDTO profileDTO, CancellationTokenSource tknSrc)
        {
            if (!EaData.UserXUTSIDs.ContainsKey(profileDTO.Email))
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            var sellCardResponse = await _sellConnection.RelistAll(profileDTO.Email);
            if (sellCardResponse == ConnectionResponseType.Unauthorized)
            {
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                await RelistAll(profileDTO, tknSrc);
                return;
            }
        }

        public async Task<ConnectionResponseType> ClearSoldCards(ProfileDTO profileDTO, CancellationTokenSource tknSrc)
        {
            if (!EaData.UserXUTSIDs.ContainsKey(profileDTO.Email))
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            var clearSoldCardsResponse = await _sellConnection.ClearSold(profileDTO.Email);
            if (clearSoldCardsResponse == ConnectionResponseType.Unauthorized)
            {
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                clearSoldCardsResponse = await ClearSoldCards(profileDTO, tknSrc);
                return clearSoldCardsResponse;
            }

            return clearSoldCardsResponse;
        }
    }
}
