using Magnus.Futtbot.Connections.Enums;

namespace Magnus.Futtbot.Connections.Models.Requests
{
    public class SellCardRequest
    {
        public SellCardRequest(int buyNowPrice, DurationType duration, SellCardItemData itemData, int startingBid)
        {
            this.buyNowPrice = buyNowPrice;
            this.duration = DurationTypeToDuration(duration);
            this.itemData = itemData;
            this.startingBid = startingBid;
        }

        public int buyNowPrice { get; }

        public int duration { get; }

        public SellCardItemData itemData { get; }

        public int startingBid { get; }

        private int DurationTypeToDuration(DurationType durationType)
        {
            switch (durationType)
            {
                case DurationType.OneHour: return 3600;
                case DurationType.ThreeHours: return 3600 * 3;
                case DurationType.SixHours: return 3600 * 6;
                case DurationType.TwelveHours: return 3600 * 12;
                case DurationType.OneDay: return 3600 * 24;
                case DurationType.ThreeDays: return 3600 * 24 * 3;
                default: return 3600;
            }
        }
    }
}
