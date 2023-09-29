using Magnus.Futtbot.Connections.Connection.Moving;
using Magnus.Futtbot.Connections.Enums;
using Magnus.Futtbot.Connections.Models.Requests;

namespace Magnus.Futtbot.Connections.Services
{
    public class MoveService
    {
        private readonly SendItemsConnection _sendItemsConnection;

        public MoveService(SendItemsConnection sendItemsConnection)
        {
            _sendItemsConnection = sendItemsConnection;
        }

        public Task<ConnectionResponseType> SendWonItemsToTransferList(string username, SendCardsToTransferListRequest sendCardsToTransferListRequest)
        {
            return _sendItemsConnection.SendWonItemsToTransferList(username, sendCardsToTransferListRequest);
        }
    }
}