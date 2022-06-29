using Magnus.Futbot.Database.Models;

namespace Magnus.Futbot.Api.Hubs.Interfaces
{
    public interface IPlayersClient
    {
        Task OnPlayersLoaded(IEnumerable<PlayerDocument> players);
    }
}