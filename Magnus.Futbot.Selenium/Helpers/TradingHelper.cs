using Magnus.Futbot.Common;

namespace Magnus.Futbot.Selenium.Helpers
{
    public static class TradingHelper
    {
        public static PromoType GetPromoTypeByRevision(string revision)
        {
            switch (revision)
            {
                case "Icon Moments":
                case "Icon":
                    return PromoType.Icon;
                default:
                    return PromoType.Gold;
            }
        }
    }
}