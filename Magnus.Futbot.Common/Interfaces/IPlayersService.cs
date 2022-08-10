using Magnus.Futbot.Common.Models.DTOs;

namespace Magnus.Futbot.Common.Interfaces
{
    public interface IPlayersService
    {
        Task Add(IEnumerable<PlayerDTO> players);
    }
}
