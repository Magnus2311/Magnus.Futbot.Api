using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using Magnus.Futtbot.Connections.Connection.Trading;

namespace Magnus.Futtbot.Connections.Services
{
    public class BuyService
    {
        private readonly TransferMarketCardsConnection _transferMarketCardsConnection;

        public BuyService(TransferMarketCardsConnection transferMarketCardsConnection)
        {
            _transferMarketCardsConnection = transferMarketCardsConnection;
        }

        public async Task Buy(ProfileDTO profileDTO, BuyCardDTO buyCardDTO, CancellationTokenSource cancellationTokenSource)
        {
            var availableCards = await _transferMarketCardsConnection.GetAvailableCardsByPlayer(profileDTO, buyCardDTO.Card.EAId, buyCardDTO.Price);
        }
    }
}
