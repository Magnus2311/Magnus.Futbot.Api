using System.Collections.Concurrent;
using Magnus.Futbot.Common.Models.Selenium.Actions;
using OpenQA.Selenium.Chrome;

namespace Magnus.Futbot.Models
{
    public class DriverInstance
    {
        public DriverInstance(ChromeDriver driver)
        {
            Driver = driver;
        }

        private ConcurrentQueue<TradeAction> PendingActions { get; set; } = new();

        public ChromeDriver Driver { get; set; }

        public void AddAction(TradeAction action)
        {
            if (!PendingActions.IsEmpty)
            {
                PendingActions.Enqueue(new TradeAction(() =>
                {
                    action.Action.Invoke();

                    if (PendingActions.TryDequeue(out var nextAction))
                    {
                        if (!nextAction.CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            nextAction.Action.Invoke();
                        }
                    }
                }, action.IsBuy, action.BuyCardDTO, action.SellCardDTO, action.CancellationTokenSource));
            }
            else
            {
                var tempAction = new TradeAction(() =>
                {
                    action.Action.Invoke();

                    PendingActions.TryDequeue(out _);
                    if (PendingActions.TryDequeue(out var nextAction))
                    {
                        if (!nextAction.CancellationTokenSource.Token.IsCancellationRequested)
                        {
                            nextAction.Action.Invoke();
                        }
                    }
                }, action.IsBuy, action.BuyCardDTO, action.SellCardDTO, action.CancellationTokenSource);

                PendingActions.Enqueue(tempAction);
                tempAction.Action.Invoke();
            }
        }
    }
}