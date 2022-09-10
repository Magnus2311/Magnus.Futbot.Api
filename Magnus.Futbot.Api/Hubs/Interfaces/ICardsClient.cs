using Magnus.Futbot.Common.Models.Database.Card;

namespace Magnus.Futbot.Api.Hubs.Interfaces
{
    public interface ICardsClient
    {
        Task OnCardsLoaded(IEnumerable<Card> cards);
        Task OnCardAdded(Card card);
    }
}
