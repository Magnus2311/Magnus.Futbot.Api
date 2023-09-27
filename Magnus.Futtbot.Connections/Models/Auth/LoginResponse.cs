namespace Magnus.Futtbot.Connections.Models.Auth
{
    public class LoginResponse
    {
        public int NucleusId { get; set; }
        public string? ShardInfos { get; set; }
        public string? Shard { get; set; }
        public string? Persona { get; set; }
        public string? SessionData { get; set; }
        public string? ApiRequest { get; set; }
    }
}
