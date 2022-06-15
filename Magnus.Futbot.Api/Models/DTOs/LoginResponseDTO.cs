using Magnus.Futbot.Api.Helpers;

namespace Magnus.Futbot.Api.Models.DTOs
{
    public class LoginResponseDTO
    {
        public LoginResponseDTO(LoginStatusType loginStatusType,
            ProfileDTO profile)
        {
            LoginStatus = loginStatusType;
            ProfileDTO = profile;
        }

        public LoginStatusType LoginStatus { get; set; }
        public ProfileDTO ProfileDTO { get; set; }
    }
}
