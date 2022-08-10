namespace Magnus.Futbot.Common.Models.Responses
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
