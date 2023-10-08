using Magnus.Futbot.Common.Models.Selenium.Trading;

namespace Magnus.Futbot.Common.Models.DTOs
{
    public class ProfileDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public ProfileStatusType Status { get; set; }
        public int Coins { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UTSID { get; set; } = string.Empty;
        public TradePile TradePile { get; set; } = new TradePile();
        public bool AutoRelist { get; set; }

        public override int GetHashCode()
            => HashCode.Combine(Email);
    }
}
