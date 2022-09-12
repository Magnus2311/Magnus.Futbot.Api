using Magnus.Futbot.Common.Models.Database.Card;

namespace Magnus.Futbot.Common.Interfaces.Helpers
{
    public interface ICardsHelper
    {
        IEnumerable<Card> GetAllCards();
    }
}
