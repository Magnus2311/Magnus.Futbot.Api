using Magnus.Futbot.Common.Models.DTOs;

namespace Magnus.Futbot.Common.Models.Responses
{
    public class ConfirmationCodeResponseDTO
    {
        public ConfirmationCodeResponseDTO(ConfirmationCodeStatusType confirmationCodeStatusType,
            ProfileDTO profile)
        {
            Status = confirmationCodeStatusType;
            Profile = profile;
        }

        public ConfirmationCodeStatusType Status { get; }
        public ProfileDTO Profile { get; }
    }
}
