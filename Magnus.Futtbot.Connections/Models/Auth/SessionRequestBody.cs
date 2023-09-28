namespace Magnus.Futtbot.Connections.Models.Auth
{
    public class SessionRequestBody
    {
        public SessionRequestBody(long nucleusPersonaId, string authCode)
        {
            NucleusPersonaId = nucleusPersonaId;
            Identification = new(authCode);
        }

        public bool IsReadOnly { get; } = false;

        // Това трябва да се сетва спрямо платформата
        public string Sku { get; } = "FUT24WEB";

        public int ClientVersion { get; } = 1;

        public long NucleusPersonaId { get; }

        // Това трябва да се сетва спрямо платформата
        public string GameSku { get; } = "FFA24PS5";

        public string Locale { get; } = "en-US";

        public string Method { get; } = "authCode";

        public int PriorityLevel { get; } = 4;

        public Identification Identification { get; }
    }

    public class Identification
    {
        public Identification(string authCode)
        {
            AuthCode = authCode;
        }

        public string AuthCode { get; }

        public string RedirectUrl { get; } = "nucleus:rest";
    }
}
