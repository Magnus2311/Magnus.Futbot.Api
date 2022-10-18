using Magnus.Futbot.Common.Models.Selenium.Actions.Interfaces;
using MongoDB.Bson;

namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public abstract class TradeAction : ITradeAction
    {
        public TradeAction()
        {

        }

        public TradeAction(
            TradeActionType type, 
            string description,
            string profileId,
            Func<Task> action, 
            CancellationTokenSource cancellationTokenSource)
        {
            Id = ObjectId.GenerateNewId().ToString();
            Action = action;
            CancellationTokenSource = cancellationTokenSource;
            Type = type;
            Description = description;
            ProfileId = profileId;
        }

        public string Id { get; set; }
        public Func<Task> Action { get; set; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public TradeActionType Type { get; set;  }

        public string Description { get; set; }
        public int Priority { get; set; }

        public string ProfileId { get; set; }
    }
}
