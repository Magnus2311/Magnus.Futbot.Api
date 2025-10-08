namespace Magnus.Futbot.Database.Models
{
    public class Filters
    {
        public string MinOvr { get; set; } = string.Empty;
        public string MaxOvr { get; set; } = string.Empty;
        public string PlayerName { get; set; } = string.Empty;
        public string Quality { get; set; } = string.Empty;
        public string EvolutionStatus { get; set; } = string.Empty;
        public string Rarity { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string ChemistryStyle { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string League { get; set; } = string.Empty;
        public string Club { get; set; } = string.Empty;
        public string PlayStyles { get; set; } = string.Empty;
        public int Price { get; set; }
    }
}

