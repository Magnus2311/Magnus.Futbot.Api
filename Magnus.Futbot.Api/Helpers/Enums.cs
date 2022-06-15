﻿namespace Magnus.Futbot.Api.Helpers
{
    public enum LoginStatusType
    {
        Successful,
        WrongCredentials,
        ConfirmationKeyRequired,

        UnknownError = 999
    }

    public enum ConfirmationCodeStatusType
    {
        Successful,
        WrongCode,
    }
}