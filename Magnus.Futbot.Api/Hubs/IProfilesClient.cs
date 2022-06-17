using Magnus.Futbot.Api.Models.DTOs;

namespace Magnus.Futbot.Api.Hubs
{
    public interface IProfilesClient
    {
        Task OnProfileAdded(LoginResponseDTO loginResponse);
        Task OnProfilesLoaded(IEnumerable<ProfileDTO> profiles);
        Task OnCodeSubmited(ConfirmationCodeResponseDTO confirmationCodeResponseDTO);
    }
}