namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public class MoveAction : TradeAction
    {
        public MoveAction(
            Func<Task> action,
            CancellationTokenSource cancellationTokenSource,
            string description) : base("Move action", $"{description}", action, cancellationTokenSource)
        {
            Priority = 3;
        }
    }
}
