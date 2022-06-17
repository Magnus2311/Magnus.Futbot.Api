using Magnus.Futbot.Api.Helpers;

namespace Magnus.Futbot.Api.Models.DTOs
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
