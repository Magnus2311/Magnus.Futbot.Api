using Magnus.Futbot.Common.Models.DTOs;

namespace Magnus.Futbot.Common.Models.Responses
{
    public class LoginResponseDTO
    {
        public LoginResponseDTO(ProfileStatusType loginStatusType,
            ProfileDTO profile)
        {
            LoginStatus = loginStatusType;
            ProfileDTO = profile;
        }

        public ProfileStatusType LoginStatus { get; set; }
        public ProfileDTO ProfileDTO { get; set; }
    }
}
