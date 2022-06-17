namespace Magnus.Futbot.Common
{
    public enum ProfileStatusType
    {
        Logged,
        WrongCredentials,
        ConfirmationKeyRequired,
        CaptchaNeeded,

        UnknownError = 999
    }

    public enum ConfirmationCodeStatusType
    {
        Successful,
        WrongCode,
    }
}