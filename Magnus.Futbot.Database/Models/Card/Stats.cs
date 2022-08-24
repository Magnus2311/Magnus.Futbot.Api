namespace Magnus.Futbot.Database.Models.Card
{
    public class Stats
    {
        public Pace Pace { get; set; } = new Pace();
        public Shooting Shooting { get; set; } = new Shooting();
        public Passing Passing { get; set; } = new Passing();
        public DribblingData Dribbling { get; set; } = new DribblingData();
        public Defending Defending { get; set; } = new Defending();
        public Physicality Physicality { get; set; } = new Physicality();
        public AdditionalData AdditionalData { get; set; } = new AdditionalData();
    }
}
