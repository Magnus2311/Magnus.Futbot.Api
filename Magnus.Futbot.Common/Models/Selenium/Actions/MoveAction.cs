namespace Magnus.Futbot.Common.Models.Selenium.Actions
{
    public class MoveAction : TradeAction
    {
        public MoveAction()
        {

        }

        public MoveAction(
            string profileId,
            Func<Task> action,
            CancellationTokenSource cancellationTokenSource,
            string description) : base(TradeActionType.Move, $"{description}", profileId, action, cancellationTokenSource)
        {
            Priority = 3;
        }
    }
}
