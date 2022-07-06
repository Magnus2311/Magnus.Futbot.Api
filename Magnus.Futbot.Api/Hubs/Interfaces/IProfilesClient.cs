using Magnus.Futbot.Common.Models.DTOs;
using Magnus.Futbot.Common.Models.Responses;

namespace Magnus.Futbot.Api.Hubs.Interfaces
{
    public interface IProfilesClient
    {
        Task OnProfileAdded(LoginResponseDTO loginResponse);
        Task OnProfilesLoaded(IEnumerable<ProfileDTO> profiles);
        Task OnCodeSubmited(ConfirmationCodeResponseDTO confirmationCodeResponseDTO);
        Task OnProfileUpdated(ProfileDTO profileDTO);
    }
}