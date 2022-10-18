using Magnus.Futbot.Common.Models.DTOs;

namespace Magnus.Futbot.Common.Interfaces.Services
{
    public interface IPlayersService
    {
        Task Add(IEnumerable<PlayerDTO> players);
    }
}
