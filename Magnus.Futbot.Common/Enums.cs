namespace Magnus.Futbot.Common
{
    public enum ProfileStatusType
    {
        Logged,
        WrongCredentials,
        ConfirmationKeyRequired,
        CaptchaNeeded,
        AlreadyAdded,

        UnknownError = 999
    }

    public enum ConfirmationCodeStatusType
    {
        Successful,
        WrongCode,
    }

    public enum PromoType
    {
        Bronze,
        BronzeRare,
        Silver,
        SilverRare,
        Gold,
        GoldRare,
        Icon,
        Hero,
        TOTW,
        SBC
    }

    public enum PositionType
    {
        Any
    }

    public enum ChemistryStyleType
    {
        Any
    }

    public enum PlayerCardStatus
    {
        Pending,
        Won,
        Outbidded
    }

    public enum TradeActionType
    {
        Buy,
        Sell,
        Move,
        Pause
    }

    public enum TradeHistoryActionType
    {
        Buy,
        Sell,
        BuyAndSell
    }
}