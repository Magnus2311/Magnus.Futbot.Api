namespace Magnus.Futbot.Api.Models.EA
{
    public class Root
    {
        public List<LegendsPlayer> LegendsPlayers { get; set; } = new List<LegendsPlayer>();
        public List<Player> Players { get; set; } = new List<Player>();
    }
}