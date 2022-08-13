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
        Basic
    }

    public enum PositionType
    {
        Any
    }

    public enum ChemistryStyleType
    {
        Any
    }
}