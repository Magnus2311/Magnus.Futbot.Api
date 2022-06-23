namespace Magnus.Futbot.Database.Models
{
    public class PlayerDocument
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
}