using Magnus.Futbot.Api.Helpers;

namespace Magnus.Futbot.Api.Models.DTOs
{
    public class ConfirmationCodeResponseDTO
    {
        public ConfirmationCodeResponseDTO(ConfirmationCodeStatusType confirmationCodeStatusType)
        {
            Status = confirmationCodeStatusType;
        }

        public ConfirmationCodeStatusType Status { get; }
    }
}