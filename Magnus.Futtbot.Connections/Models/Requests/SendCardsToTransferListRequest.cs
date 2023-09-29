namespace Magnus.Futtbot.Connections.Models.Requests
{
    public class SendCardsToTransferListRequest
    {
        public SendCardsToTransferListRequest(List<ItemDataForMoving> itemData)
        {
            this.itemData = itemData;
        }

        public List<ItemDataForMoving> itemData { get; }
    }
}
