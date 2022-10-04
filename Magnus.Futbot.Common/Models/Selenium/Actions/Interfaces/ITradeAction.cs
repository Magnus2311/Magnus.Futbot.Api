namespace Magnus.Futbot.Common.Models.Selenium.Actions.Interfaces
{
    public interface ITradeAction
    {
        public string Id { get; }
        Func<Task> Action { get; }
        CancellationTokenSource CancellationTokenSource { get; }
        string Type { get; }
        string Description { get; }
        int Priority { get; protected set; }
    }
}
