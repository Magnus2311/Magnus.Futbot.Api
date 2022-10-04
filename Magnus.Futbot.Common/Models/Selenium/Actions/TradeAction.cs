using Magnus.Futbot.Common.Models.Selenium.Actions.Interfaces;

namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public abstract class TradeAction : ITradeAction
    {
        public TradeAction(
            string type, 
            string description,
            Func<Task> action, 
            CancellationTokenSource cancellationTokenSource)
        {
            Id = Guid.NewGuid().ToString();
            Action = action;
            CancellationTokenSource = cancellationTokenSource;
            Type = type;
            Description = description;
        }

        public string Id { get; }
        public Func<Task> Action { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
        public string Type { get; }

        public string Description { get; }
        public int Priority { get; set; }
    }
}
