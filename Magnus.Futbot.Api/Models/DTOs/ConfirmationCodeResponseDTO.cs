using Magnus.Futbot.Common;

namespace Magnus.Futbot.Api.Models.DTOs
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