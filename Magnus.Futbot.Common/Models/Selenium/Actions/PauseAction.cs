namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public class PauseAction : TradeAction
    {
        public PauseAction()
        {

        }

        public PauseAction(
            string profileId,
            Func<Task> action,
            CancellationTokenSource cancellationTokenSource,
            string description) : base(TradeActionType.Move, $"{description}", profileId, action, cancellationTokenSource)
        {
            Priority = 999;
        }
    }
}
