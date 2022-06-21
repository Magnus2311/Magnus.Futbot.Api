using Magnus.Futbot.Common;

namespace Magnus.Futbot.Api.Models.DTOs.Selenium
{
    public class BidPlayerDTO
    {
        public string Name { get; set; } = string.Empty;
        public int BaseRating { get; set; }
        public int Rating { get; set; }
        public PromoType PromoType { get; set; }
        public int MaxPrice { get; set; }
        public int MaxPlayers { get; set; }
        public PositionType PositionType { get; set; }
        public ChemistryStyleType ChemistryStyleType { get; set; }
    }
}