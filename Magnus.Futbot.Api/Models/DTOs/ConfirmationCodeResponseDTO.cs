using Magnus.Futbot.Api.Helpers;

namespace Magnus.Futbot.Api.Models.DTOs
{
    public class ConfirmationCodeResponseDTO
    {
        public ConfirmationCodeResponseDTO(ConfirmationCodeStatusType confirmationCodeStatusType,
            string email)
        {
            Status = confirmationCodeStatusType;
            Email = email;
        }

        public ConfirmationCodeStatusType Status { get; }
        public string Email { get; }
    }
}