namespace Magnus.Futtbot.Connections.Enums
{
    public enum ConnectionResponseType
    {
        Success,
        Unauthorized,
        PauseForAWhile,
        UpgradeRequired,

        Unknown = 9999
    }
}
