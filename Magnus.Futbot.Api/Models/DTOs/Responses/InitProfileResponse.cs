using Magnus.Futbot.Api.Services.Selenium;
using Magnus.Futbot.Common;

namespace Magnus.Futbot.Api.Models.DTOs.Responses
{
    public class InitProfileResponse
    {
        public InitProfileResponse(ProfileStatusType profileStatus)
        {
            PofileStatus = profileStatus;
        }

        public ProfileStatusType PofileStatus { get; set; }
    }
}