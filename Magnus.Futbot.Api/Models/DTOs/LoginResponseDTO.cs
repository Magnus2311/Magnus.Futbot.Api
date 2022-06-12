using Magnus.Futbot.Api.Helpers;

namespace Magnus.Futbot.Api.Models.DTOs
{
    public class LoginResponseDTO
    {
        public LoginResponseDTO(LoginStatusType loginStatusType)
        {
            LoginStatus = loginStatusType;
        }

        public LoginStatusType LoginStatus { get; set; }
    }
}
