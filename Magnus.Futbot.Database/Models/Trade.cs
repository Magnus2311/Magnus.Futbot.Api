using Magnus.Futbot.Common;
using Magnus.Futbot.Common.Models.Database.Interfaces;
using Magnus.Futbot.Common.Models.DTOs.Trading;
using MongoDB.Bson;

namespace Magnus.Futbot.Database.Models
{
    public class Trade : IEntity
    {
        public BuyCardDTO BuyCardDTO { get; set; }

        public SellCardDTO SellCardDTO { get; set; }

        public  BuyAndSellCardDTO BuyAndSellCardDTO { get; set; }

        public TradeHistoryActionType TradeHistoryActionType { get; set; }

        public string ProfileId { get; set; }

        public ObjectId Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ObjectId UserId { get; set; }
    }
}
