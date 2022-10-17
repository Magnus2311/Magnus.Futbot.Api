using Magnus.Futbot.Common.Models.Selenium.Actions.Interfaces;

namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public abstract class TradeAction : ITradeAction
    {
        public TradeAction(
            TradeActionType type, 
            string description,
            string profileId,
            Func<Task> action, 
            CancellationTokenSource cancellationTokenSource)
        {
            Id = Guid.NewGuid().ToString();
            Action = action;
            CancellationTokenSource = cancellationTokenSource;
            Type = type;
            Description = description;
            ProfileId = profileId;
        }

        public string Id { get; }
        public Func<Task> Action { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
        public TradeActionType Type { get; }

        public string Description { get; }
        public int Priority { get; set; }

        public string ProfileId { get; set; }
    }
}
