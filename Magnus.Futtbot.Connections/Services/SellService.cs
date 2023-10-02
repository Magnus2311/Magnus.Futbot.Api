﻿using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futbot.Services;
using Magnus.Futtbot.Connections.Connection;
using Magnus.Futtbot.Connections.Connection.Moving;
using Magnus.Futtbot.Connections.Connection.Trading.Sell;
using Magnus.Futtbot.Connections.Enums;
using Magnus.Futtbot.Connections.Models;
using Magnus.Futtbot.Connections.Models.Requests;
using Magnus.Futtbot.Connections.Utils;

namespace Magnus.Futtbot.Connections.Services
{
    public class SellService
    {
        private readonly GetUserPileConnection _getUserPileConnection;
        private readonly SellConnection _sellConnection;
        private readonly SendItemsConnection _sendItemsConnection;
        private readonly LoginSeleniumService _loginSeleniumService;

        public SellService(GetUserPileConnection getUserPileConnection,
            SellConnection sellConnection,
            SendItemsConnection sendItemsConnection,
            LoginSeleniumService loginSeleniumService)
        {
            _getUserPileConnection = getUserPileConnection;
            _sellConnection = sellConnection;
            _sendItemsConnection = sendItemsConnection;
            _loginSeleniumService = loginSeleniumService;
            SellingData = new SellingData();
        }

        public SellingData SellingData { get; }

        public async Task SellCard(ProfileDTO profileDTO, SellCardDTO sellCardDTO, CancellationTokenSource cancellationTokenSource)
        {
            if (!EaData.UserXUTSIDs.ContainsKey(profileDTO.Email))
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);

            var getUserPileResponse = await _getUserPileConnection.GetUserTradePile(profileDTO);
            if (getUserPileResponse.ConnectionResponseType == ConnectionResponseType.Unauthorized)
            {
                await _loginSeleniumService.Login(profileDTO.Email, profileDTO.Password);
                await SellCard(profileDTO, sellCardDTO, cancellationTokenSource);
                return;
            }

            if (getUserPileResponse.Data is null)
            {
                await SellCard(profileDTO, sellCardDTO, cancellationTokenSource);
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
                    await SellCard(profileDTO, sellCardDTO, cancellationTokenSource);
                    return;
                }

                var availablePlayersForSelling = getUserPileResponse.Data.auctionInfo.Where(ai => ai.tradeState != "active");
                var currentPlayerForSale = availablePlayersForSelling.Where(p => p.itemData.assetId == sellCardDTO.Card.EAId);

                foreach (var player in currentPlayerForSale)
                {
                    if (SellingData.AlreadySoldTrades.Contains(player.itemData.id)) return;

                    SellingData.AlreadySoldTrades.Add(player.itemData.id);

                    Thread.Sleep(600);
                    var sellCardResponse = await _sellConnection.SellCard(profileDTO.Email, new SellCardRequest(sellCardDTO.FromBin, DurationType.OneHour, new SellCardItemData(player.itemData.id), sellCardDTO.FromBid));
                    if (sellCardResponse == ConnectionResponseType.Unauthorized)
                    {
                        await SellCard(profileDTO, sellCardDTO, cancellationTokenSource);
                        return;
                    }
                }
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
    }
}